﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {

	///<summary>A class that directly manipulates a UserControl which uses the dynamic sheetDef framework.</summary>
	public class SheetLayoutController {

		///<summary>List of all custom and internal SheetDefs for this dynamic control based on given _sheetType.
		///Will always contain the internal definition as last item in list.</summary>
		public List<SheetDef> ListSheetDefsLayout;
		///<summary>The control that contains other controls that are placed and sized based on their corresponding sheetFieldDefs.</summary>
		private UserControl _controlHosting;
		///<summary>The sheetDef layout from ListLayoutSheetDefs which is currently being used to set the layout of this control.</summary>
		private SheetDef _sheetDefDynamicLayoutCur;
		///<summary>The sheet type which is used to define the dynamic layout.</summary>
		private SheetTypeEnum _sheetType;
		///<summary>Used when there are controls that are not part of they dynamic framework but still need to show.
		///After drawing and placing all controls these controls are set invisible.
		///Controls in list will effect the dynamic controls fill logic based on their position and visibility.</summary>
		private List<Control> _listControlsStatic;
		///<summary>The currently logged in UserOd.UserNum.</summary>
		private long _userNumCur;

		///<summary>Returns the currently loaded layout. May be null if ListLayoutSheetDefs is empty and the layout hasn't been initialized.</summary>
		public SheetDef SheetDefDynamicLayoutCur {
			get {
				return _sheetDefDynamicLayoutCur;
			}
		}

		///<summary></summary>
		public SheetLayoutController(UserControl controlHosting,params Control[] arrayStaticControls) {
			_controlHosting=controlHosting;
			if(controlHosting is ContrChart) {
				_sheetType=SheetTypeEnum.ChartModule;
			}
			else {
				throw new ApplicationException("Host control of type "+controlHosting.GetType().Name+" not supported yet.");
			}
			_listControlsStatic=arrayStaticControls.ToList();
		}
		
		///<summary>Refreshes dynamic layout sheetDefs for the current user.
		///This should only be called if a dynamic sheetDef was added/modified/deleted, the SheetLayoutMode has changed, or a new user signed in.</summary>
		public void RefreshSheetLayout(SheetFieldLayoutMode sheetFieldLayoutMode,Dictionary<string,Control> dictionaryControls) {
			int countPreviousTotal=(ListSheetDefsLayout?.Count??0);//Null when first loading
			long defaultSheetDefNum=PrefC.GetLong(PrefName.ChartDefaultLayoutSheetDefNum);
			ListSheetDefsLayout=SheetDefs.GetCustomForType(_sheetType);
			ListSheetDefsLayout.Add(SheetsInternal.GetSheetDef(SheetsInternal.GetInternalType(_sheetType)));
			ListSheetDefsLayout=ListSheetDefsLayout
				.OrderBy(x => x.SheetDefNum!=defaultSheetDefNum)//default sheetdef should be at the top of the list
				.ThenBy(x => x.SheetDefNum==0)//if the internal sheetdef is not the default it should be last
				.ThenBy(x => x.Description)//order custom sheetdefs by description
				.ThenBy(x => x.SheetDefNum).ToList();//then by SheetDefNum to be deterministic order
			SheetDef sheetDefLayout;
			if(_sheetDefDynamicLayoutCur==null) {//Initial load.
				sheetDefLayout=GetDefaultLayout();
				_userNumCur=Security.CurUser.UserNum;
			}
			else if(_sheetDefDynamicLayoutCur.SheetDefNum==0 && countPreviousTotal!=ListSheetDefsLayout.Count) {
				//Current layout is associated to the internal sheetdef and there is a new sheetdef that should now be used.
				sheetDefLayout=ListSheetDefsLayout[0];
			}
			else {//Not first time loading and user was not viewing default internal layout or user was viewing default layout but did not add any new ones.
				//Either a new user logged in or the current user navigated to FormLayoutSheetDefs.
				//If new user logged in then get the user's default layout.  Otherwise same user so the matching logic will work below, unless it was 
				//deleted. If matching works then we still force re-draw below.
				if(_userNumCur!=Security.CurUser.UserNum) {//Logged in user has changed.
					sheetDefLayout=GetDefaultLayout();
					_userNumCur=Security.CurUser.UserNum;
				}
				else {
					sheetDefLayout=ListSheetDefsLayout.FirstOrDefault(x => x.SheetDefNum==_sheetDefDynamicLayoutCur.SheetDefNum);//Try and reselect the same layout.
				}
				if(sheetDefLayout==null) {//Layout not found. Either deleted or new user signed on.
					sheetDefLayout=ListSheetDefsLayout[0];//Will always have at least the internal def.
				}
			}
			InitLayoutForSheetDef(sheetDefLayout,sheetFieldLayoutMode,dictionaryControls,true);//Force refresh in case they edit current layout.
		}

		///<summary>Attempts to find a UserOdPref indicating to the the most recently loaded layout for user, defaulting to the practice default if not 
		///found, then defaulting to the first SheetDef in listLayoutSheetDefs.  Returns null if listLayoutSheetDefs is null or empty.</summary>
		private SheetDef GetDefaultLayout() {
			List<UserOdPref> listPrefs=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.DynamicChartLayout);
			UserOdPref userPref=listPrefs.FirstOrDefault();//There is only at most a single link per user.
			if(userPref!=null && ListSheetDefsLayout.Any(x => x.SheetDefNum==userPref.Fkey)) {
				return ListSheetDefsLayout.FirstOrDefault(x => x.SheetDefNum==userPref.Fkey);//Use the layout from the user pref.
			}
			else if(ListSheetDefsLayout.Any(x => x.SheetDefNum==PrefC.GetLong(PrefName.ChartDefaultLayoutSheetDefNum))) {//Use practice default.
				return ListSheetDefsLayout.FirstOrDefault(x => x.SheetDefNum==PrefC.GetLong(PrefName.ChartDefaultLayoutSheetDefNum));
			}
			else {
				return ListSheetDefsLayout[0];//Use first in the list.
			}
		}

		///<summary>Uses the given sheetDef to set contorl's location, size and anchors.
		///Usually only called from outside base DynamicLayoutControl when UI is switching to a different layout sheetDef.</summary>
		public void InitLayoutForSheetDef(SheetDef sheetDef,SheetFieldLayoutMode sheetFieldLayoutMode,Dictionary<string,Control> dictionaryControls,bool isForcedRefresh=false) {
			if(!isForcedRefresh && _sheetDefDynamicLayoutCur!=null && sheetDef.SheetDefNum==_sheetDefDynamicLayoutCur.SheetDefNum){
				//Not forcing a refresh and _dynamicLayoutCur and sheetDef are the same sheet. No point in re-running logic.  Prevents flicker.
				return;
			}
			_sheetDefDynamicLayoutCur=sheetDef;
			if(_sheetDefDynamicLayoutCur.SheetDefNum!=0) {//0 represents an internal sheetDef, internal sheet defs do not need their field and params set.
				SheetDefs.GetFieldsAndParameters(_sheetDefDynamicLayoutCur);
			}
			List<SheetFieldDef> listSheetFieldDefs=_sheetDefDynamicLayoutCur.SheetFieldDefs.Where(x => x.LayoutMode==sheetFieldLayoutMode)
				.OrderByDescending(x => x.FieldType==SheetFieldType.Grid)//Grids First
				.ThenBy(x => x.FieldName.Contains("Button"))//Buttons last, always drawn on top.
				.ToList();
			List<Control> listDynamicControls=new List<Control>();
			foreach(SheetFieldDef fieldDef in listSheetFieldDefs) {
				Control control=GetControlForField(fieldDef,dictionaryControls);
				if(control==null) {
					continue;//For example, HQ only controls when running from customer location.
				}
				//The height above and below calculations assumes the staic controls are all entire at the top or bottom of the this dynamic control.
				int heightAbove=GetHeightAbove(fieldDef.YPos);
				int heightBelow=GetHeightBelow(fieldDef.YPos);
				control.MinimumSize=new Size(0,0);//Do not limit min size
				control.MaximumSize=new Size(0,0);//Do not limit max size
				SheetUtil.SetControlSizeAndAnchors(fieldDef,control,_controlHosting,(heightAbove+heightBelow));
				control.Location=new Point(fieldDef.XPos,(fieldDef.YPos+heightAbove));
				control.Visible=true;
				listDynamicControls.Add(control);
			}
			RefreshGridVerticalSpace(sheetFieldLayoutMode,dictionaryControls);
			RefreshGridHorizontalSpace(sheetFieldLayoutMode,dictionaryControls);
			_listControlsStatic.ForEach(x => x.BringToFront());
			_controlHosting.Controls.OfType<Control>().Where(x => !listDynamicControls.Contains(x) && !_listControlsStatic.Contains(x)).ForEach(x => x.Visible=false);
			UpdateChartLayoutUserPref();
		}

		///<summary>Updates, if necessary, the user's preference dictating which Dynamic Chart Layout will load by default.</summary>
		private void UpdateChartLayoutUserPref() {
			UserOdPref userPref=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.DynamicChartLayout);
			if(!userPref.IsNew && userPref.Fkey==_sheetDefDynamicLayoutCur.SheetDefNum) {//Existing user pref points to the current layout already
				return;
			}
			userPref.Fkey=_sheetDefDynamicLayoutCur.SheetDefNum;
			UserOdPrefs.Upsert(userPref);
		}

		///<summary>Once all controls are set in their initial place this is called to adjust any controls which might be overlapping on the X axis
		///due to a grid having GrowthBehaviorEnum.FillDownFitColumns.
		///Since the width of the grid is determined by the sum of the column widths we might need to move some controls to the right.</summary>
		public void RefreshGridHorizontalSpace(SheetFieldLayoutMode sheetFieldLayoutModeCur,Dictionary<string,Control> dictionaryControls) {
			List<SheetFieldDef> listFillDownDefs=_sheetDefDynamicLayoutCur.SheetFieldDefs
				.FindAll(x => x.LayoutMode==sheetFieldLayoutModeCur && x.GrowthBehavior==GrowthBehaviorEnum.FillDownFitColumns);
			List<Control> listDynmaicControls=_sheetDefDynamicLayoutCur.SheetFieldDefs.FindAll(x => x.LayoutMode==sheetFieldLayoutModeCur)
				.Select(x => GetControlForField(x,dictionaryControls)).ToList();
			//There is a grid that might be overlaping controls to the right of the grid since the width of the grid is defined by its columns.
			//We only expect there to be one field with FillDownFitColumns growth behavior, however we loop in case more are added later.
			foreach(SheetFieldDef fillFieldDef in listFillDownDefs) {
				Control control=GetControlForField(fillFieldDef,dictionaryControls);
				int gridMaxX=(control.Location.X+control.Width);
				foreach(Control controlIntersect in listDynmaicControls.FindAll(x => x!=control && x.Bounds.IntersectsWith(control.Bounds))) {
					int diff=(gridMaxX-controlIntersect.Location.X)+1;
					controlIntersect.Location=new Point(controlIntersect.Location.X+diff,controlIntersect.Location.Y);
				}
			}
		}

		///<summary>Once all controls are set in their initial place this is called to adjust any controls which might be overlapping on the Y axis
		///due to a grid having GrowthBehaviorEnum.FillDownFitColumns.
		///This should only be called if there is a control which can collapse, like the Chart Module tabProc control.</summary>
		public void RefreshGridVerticalSpace(SheetFieldLayoutMode sheetFieldLayoutModeCur,Dictionary<string,Control> dictControls) {
			List<SheetFieldDef> listFillDownDefs=_sheetDefDynamicLayoutCur.SheetFieldDefs
				.FindAll(x => x.LayoutMode==sheetFieldLayoutModeCur && x.GrowthBehavior==GrowthBehaviorEnum.FillDownFitColumns);
			List<Control> listDynmaicControls=_sheetDefDynamicLayoutCur.SheetFieldDefs.FindAll(x => x.LayoutMode==sheetFieldLayoutModeCur)
				.Select(x => GetControlForField(x,dictControls)).ToList();
			//tabProc might be overlaping controls vertically below because tabProcs has two different heights depending on state.
			//We only expect there to be one field with FillDownFitColumns growth behavior, however we loop in case more are added later.
			foreach(SheetFieldDef fillFieldDef in listFillDownDefs) {
				Control control=GetControlForField(fillFieldDef,dictControls);
				int minY=GetHeightAbove(fillFieldDef.YPos);//Uses y position relative to the sheet def.
				List<Control> listAboveControls=listDynmaicControls.FindAll(x => IsControlAbove(control,x));
				if(listAboveControls.Count>0) {
					minY=listAboveControls.Max(x => x.Bottom)+1;
				}
				fillFieldDef.YPos=minY;//Now y position is relative to current control locations within Chart.
															 //Do not need to use GetHeightAbove(), all controls in listAboveControls have the above offset accounted for in their possitions.
				int heightBelow=GetHeightBelow(fillFieldDef.YPos);
				SheetUtil.SetControlSizeAndAnchors(fillFieldDef,control,_controlHosting,(heightBelow));
				control.Location=new Point(fillFieldDef.XPos,fillFieldDef.YPos);
			}
		}

		///<summary>Attempts to return a corresponding control for the given fieldDef from dictControls.
		///Returns null if not found.</summary>
		private Control GetControlForField(SheetFieldDef sheetFieldDef,Dictionary<string,Control> dictionaryControls) {
			Control control;
			dictionaryControls.TryGetValue(sheetFieldDef.FieldName,out control);
			return control;//Can be null
		}

		///<summary>The height above and below calculations assumes the staic controls are all entire at the top or bottom of the this dynamic control.</summary>
		private int GetHeightAbove(int yPos) {
			int heightAbove=0;
			List<Control> listStaticControlsAbove=_listControlsStatic.FindAll(x => x.Visible && x.Location.Y<=yPos);
			if(listStaticControlsAbove.Count>0) {
				heightAbove=listStaticControlsAbove.Sum(x => x.Height);//ex toolbarmain
			}
			return heightAbove;
		}

		///<summary>The height above and below calculations assumes the staic controls are all entire at the top or bottom of the this dynamic control.</summary>
		private int GetHeightBelow(int yPos) {
			int heightBelow=0;
			List<Control> listStaticControlsBelow=_listControlsStatic.FindAll(x => x.Visible && x.Location.Y>=yPos);
			if(listStaticControlsBelow.Count>0) {
				heightBelow=listStaticControlsBelow.Sum(x => x.Height);//ex image panel
			}
			return heightBelow;
		}

		///<summary>Does not check for vertical overlap.</summary>
		private bool IsControlAbove(Control control,Control controlOther) {
			return (controlOther.Top<control.Top
				 && (controlOther.Left.Between(control.Left,control.Right)
				|| controlOther.Right.Between(control.Left,control.Right))
			);
		}

	}
}
