using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental.UI {
	public partial class PinBoardJ:Control {
		#region Fields - Private
		private Bitmap _bitmapTempPinAppt;
		//<summary>As user begins a drag, this starts out false.  Flips to true after 3 pixels.  But consequences are local, so useless to have here.</summary>
		//private bool _boolApptMoved;
		///<summary>This user control is how we drag appts over to the main appt area.  In beginning, it gets added to parent, where it stays and gets reused repeatedly.  We control it from here to hide the complexity.</summary>
		private DoubleBufferedControl contrTempPinAppt;
		private bool _isMouseDown;
		private List<PinBoardItem> _listPinBoardItems;
		///<summary>Initial position of the appointment being dragged, in coordinates of ContrAppt.</summary>
		private Point	_pointApptOrigin;
		///<summary>Position of mouse down, in coordinates of this pinboard control.</summary>
		private Point _pointMouseOrigin;
		#endregion Fields - Private

		#region Properties
		
		[Browsable(false)]
		[DefaultValue(null)]		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		public List<PinBoardItem> ListPinBoardItems{ 
			get{
				if(_listPinBoardItems==null){
					_listPinBoardItems=new List<PinBoardItem>();
				}
				return _listPinBoardItems; 
			}
			set{
				_listPinBoardItems = value; 
			}
		}

		///<Summary>Gets or sets the selected index, which will have dark outline.  Parent should unselect other normal appts, so that only a pinboard appt OR a normal appt is selected, not both.</Summary>
		[Browsable(false)]
		[DefaultValue(-1)]
		public int SelectedIndex{
			get{
				return _selectedIndex;
			}
			set{
				if(_selectedIndex==value){
					return;
				}
				if(ListPinBoardItems==null){
					return;
				}
				if(value > ListPinBoardItems.Count-1){
					return;
				}
				_selectedIndex=value;
				Invalidate();
			}
		}
		private int _selectedIndex=-1;
		#endregion Properties

		#region Constructor
		public PinBoardJ() {
			InitializeComponent();

		}
		#endregion Constructor

		#region Events - Raise
		protected void OnSelectedIndexChanged() {
			if(SelectedIndexChanged!=null) {
				SelectedIndexChanged(this,new EventArgs());
			}
		}
		///<summary></summary>
		[Category("Appointments"),Description("Event raised when user _clicks_ to select a different appointment.")]
		public event EventHandler SelectedIndexChanged;

		///<summary>.</summary>
		private void OnApptMovedFromPinboard(DataRow dataRow,Bitmap bitmap,Point location) {
			if(ApptMovedFromPinboard!=null){
				ApptMovedFromPinboard(this,new ApptDataEventArgs(dataRow,bitmap,location));
			}
		}
		[Category("Appointment"),Description("Occurs when an appointment is dragged from pinboard.  Location is in coordinates of ContrAppt.")]
		public event ApptDataEventHandler ApptMovedFromPinboard;

		///<summary>.</summary>
		private void OnPreparingToDragFromPinboard(DataRow dataRow) {
			if(PreparingToDragFromPinboard!=null){
				PreparingToDragFromPinboard(this,new ApptDataEventArgs(dataRow,null,new Point(0,0)));
			}
		}
		[Category("Appointment"),Description("Occurs when mouse down, and preparing to drag an appt from pinboard.")]
		public event ApptDataEventHandler PreparingToDragFromPinboard;
		#endregion Events - Raise

		#region Events - OnPaint
		protected override void OnPaint(PaintEventArgs pe) {
			Graphics g=pe.Graphics;
			g.Clear(Color.White);
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.DrawRectangle(Pens.Black,0,0,Width-1,Height-1);
			//for(int i=0;i<_dataTableAppointments.Rows.Count;i++) {
			if(ListPinBoardItems==null || ListPinBoardItems.Count==0) {
				StringFormat format=new StringFormat();
				format.Alignment=StringAlignment.Center;
				format.LineAlignment=StringAlignment.Center;
				g.DrawString("Drag Appointments to this Pinboard",Font,Brushes.Gray,new RectangleF(0,0,Width,Height-20),format);
				return;
			}
			for(int i=0;i<ListPinBoardItems.Count;i++) {
				int locY=i*13;
				g.DrawImage(ListPinBoardItems[i].BitmapAppt,1,locY);
				if(_selectedIndex==i){
					DataRow dataRow=ListPinBoardItems[i].DataRowAppt;
					Pen penProvOutline;
					if(dataRow["ProvNum"].ToString()!="0" && dataRow["IsHygiene"].ToString()=="0") {//dentist
						penProvOutline=new Pen(Providers.GetOutlineColor(PIn.Long(dataRow["ProvNum"].ToString())),3f);
					}
					else if(dataRow["ProvHyg"].ToString()!="0" && dataRow["IsHygiene"].ToString()=="1") {//hygienist
						penProvOutline=new Pen(Providers.GetOutlineColor(PIn.Long(dataRow["ProvHyg"].ToString())),3f);
					}
					else {//unknown
						penProvOutline=new Pen(Color.Black,3f);//Do not use Pens.Black because we will be disposing this pen later on.
					}
					GraphicsPath graphicsPathOutline=UserControlApptsPanelJ.GetRoundedPath(
						new RectangleF(1.5f,locY+.5f,ListPinBoardItems[i].BitmapAppt.Width-2,ListPinBoardItems[i].BitmapAppt.Height-1.5f));
					g.DrawPath(penProvOutline,graphicsPathOutline);
					//g.DrawRectangle(penProvOutline,1.5f,locY+.5f,ListPinBoardItems[i].BitmapAppt.Width-2,ListPinBoardItems[i].BitmapAppt.Height-1);
					penProvOutline.Dispose();
				}
			}
		}

		///<summary></summary>
		private void TempPinAppt_Paint(object sender,PaintEventArgs e) {
			e.Graphics.DrawImage(_bitmapTempPinAppt,0,0);
			using(Pen pen=new Pen(Color.Black,1f)){
				e.Graphics.DrawLine(pen,0,contrTempPinAppt.Height-1,contrTempPinAppt.Width,contrTempPinAppt.Height-1);
			}
		}
		#endregion Events - OnPaint

		#region Events - Mouse
		
		protected override void OnMouseDown(MouseEventArgs e) {
			//set selected index before firing the mouse down event.
			//figure out which appt mouse is on.  Start at end and work backwards
			int index=-1;
			for(int i=ListPinBoardItems.Count-1;i>=0;i--){
				int top=13*i;
				if(e.Y<top || e.Y>top+ListPinBoardItems[i].BitmapAppt.Height){
					continue;
				}
				index=i;
				break;
			}
			bool changed=index!=SelectedIndex;
			if(changed){
				SelectedIndex=index;
			}
			base.OnMouseDown(e);
			if(changed){
				Invalidate();
				OnSelectedIndexChanged();//fires to parent, which sets selected appt in main area to -1.
			}
			//--Prepare to drag-----------------------------------------------------
			if(SelectedIndex==-1) {
				return;
			}
			if(_isMouseDown) {//User right clicked while dragging appt around.
				return;
			}
			if(e.Button==MouseButtons.Right) {
				/*
				ContextMenu cmen=new ContextMenu();
				MenuItem menuItemProv=new MenuItem(Lan.g(this,"Change Provider"));
				menuItemProv.Click+=new EventHandler(menuItemProv_Click);
				cmen.MenuItems.Add(menuItemProv);
				cmen.Show(pinBoard,e.Location);*/
				return;
			}
			_isMouseDown=true;
			OnPreparingToDragFromPinboard(ListPinBoardItems[SelectedIndex].DataRowAppt);//this event goes to ContrAppt, 
			//which then sends new bitmap here via SetBitmapTempPinAppt
			contrTempPinAppt.Visible=false;//Visible flag gets flipped when the mouse moves. We are just preparing it to be shown here.
			_pointMouseOrigin=e.Location;
			contrTempPinAppt.Location=new Point(
				this.Location.X+Parent.Location.X,
				this.Location.Y+Parent.Location.Y+SelectedIndex*13);
			_pointApptOrigin=contrTempPinAppt.Location;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!_isMouseDown) {
				return;
			}
			if((Math.Abs(e.X-_pointMouseOrigin.X)<3)//we don't want it to be visible until user has actually started moving significantly
				&&(Math.Abs(e.Y-_pointMouseOrigin.Y)<3)) {
				return;
			}
			//since this usercontrol belongs to ContrAppt, coordinates are in ContrAppt frame.
			contrTempPinAppt.Location=new Point(
				_pointApptOrigin.X+e.X-_pointMouseOrigin.X,
				_pointApptOrigin.Y+e.Y-_pointMouseOrigin.Y);
			contrTempPinAppt.Visible=true;
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(!_isMouseDown) {
				return;
			}
			_isMouseDown=false;
			if((Math.Abs(e.X-_pointMouseOrigin.X)<7) && (Math.Abs(e.Y-_pointMouseOrigin.Y)<7)) { //Mouse has not moved enough to be considered an appt move.
				contrTempPinAppt.Visible=false;
				return;
			}
			if(contrTempPinAppt.Location.X>((Panel)this.Parent).Left) { 
				contrTempPinAppt.Visible=false;
				return;
			}
			//Dragged to main appt area and released.  Don't hide it yet.
			OnApptMovedFromPinboard(ListPinBoardItems[SelectedIndex].DataRowAppt,ListPinBoardItems[SelectedIndex].BitmapAppt,contrTempPinAppt.Location);
		}
		#endregion Events - Mouse

		#region Events - Other
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(!DesignMode){
				contrTempPinAppt=new DoubleBufferedControl();
				contrTempPinAppt.Visible=false;
				contrTempPinAppt.Size=new Size(100,100);
				contrTempPinAppt.Paint+=TempPinAppt_Paint;
				Parent.Parent.Controls.Add(contrTempPinAppt);
				contrTempPinAppt.BringToFront();
			}
		}
		#endregion Events - Other

		#region Methods - Private
		
		#endregion Methods - Private

		#region Methods - Public
		///<Summary></Summary>
		public void AddAppointment(Bitmap bitmap,long aptNum,DataRow dataRow) {
			for(int i=0;i<ListPinBoardItems.Count;i++){
				if(ListPinBoardItems[i].AptNum==aptNum){
					_selectedIndex=i;
					return;//don't add the appointment because it's already on the pinboard.
				}
			}
			/*		
			if(_dataTableAppointments==null){
				_dataTableAppointments=dataRow.Table.Clone();//creates table with correct column schema
			}
			//todo: check if appt is already on the pinboard
			_dataTableAppointments.ImportRow(dataRow);
			Invalidate();*/
			PinBoardItem pinBoardItem=new PinBoardItem();
			pinBoardItem.AptNum=aptNum;
			pinBoardItem.BitmapAppt=new Bitmap(bitmap);//so that we can dispose of the one that was passed in
			pinBoardItem.DataRowAppt=dataRow;
			ListPinBoardItems.Add(pinBoardItem);
			_selectedIndex=ListPinBoardItems.Count-1;
			Invalidate();

			/*
			DataTable tableAppts=Appointments.RefreshOneApt(aptNum,false).Tables["Appointments"];
			if(tableAppts.Rows.Count==0) {
				MsgBox.Show(this,"Appointment no longer exists.");
				return null;
			}
			DataRow row=tableAppts.Rows[0];
			if(row["AptStatus"].ToString()=="6") {//planned so do it again the right way
				tableAppts=Appointments.RefreshOneApt(aptNum,true).Tables["Appointments"];
				if(tableAppts.Rows.Count==0) {
					MsgBox.Show(this,"Planned appointment no longer exists.");
					return null;
				}
				row=tableAppts.Rows[0];
			}
			//The appt fields are not in DS.Tables["ApptFields"] since the appt is not visible on the schedule.
			DataTable tableApptFields=Appointments.GetApptFields(tableAppts);
			AddAppointment(row,tableApptFields,Appointments.GetPatFields(tableAppts.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList()));*/
		}
		
		/*
		///<Summary>Supply a datarow that contains all the database values needed for the appointment that is being added.</Summary>
		public void AddAppointment(DataRow row,DataTable tableApptFields,DataTable tablePatFields) {
			//if appointment is already on the pinboard, just select it.
			for(int i = 0;i<apptList.Count;i++) {
				if(apptList[i].AptNum.ToString()==row["AptNum"].ToString()) {
					//Highlight it
					selectedIndex=i;
					apptList[i].IsSelected=true;
					Invalidate();
				}
			}
			ContrApptSingle pinApptSingle=new ContrApptSingle(
				row,
				tableApptFields,
				tablePatFields,
				new Point(0,13*apptList.Count),
				OverlapOrdering.GetOverlapTimes(PIn.Long(row["AptNum"].ToString()))) {
				ThisIsPinBoard=true,
				IsSelected=true,
			};
			pinApptSingle.Width=Width-2;			
			apptList.Add(pinApptSingle);
			selectedIndex=apptList.Count-1;
			Invalidate();
			return pinApptSingle;
		}*/

		/// <summary></summary>
		public void ClearSelected(){
			if(SelectedIndex==-1){
				return;
			}
			ListPinBoardItems.RemoveAt(SelectedIndex);
			SelectedIndex=-1;
		}

		///<summary>Used by parent form when a dialog needs to be displayed, but mouse might be down.  This forces a mouse up, and cleans up any mess so that dlg can show.</summary>
		public void MouseUpForced() {
			contrTempPinAppt.Visible=false;
			_isMouseDown=false;
		}

		///<summary></summary>
		public void HideDraggableTempApptSingle() {
			contrTempPinAppt.Visible=false;
		}

		///<summary>During mouse down, this is used to send a different size bitmap for dragging around.</summary>
		public void SetBitmapTempPinAppt(Bitmap bitmap){
			_bitmapTempPinAppt=new Bitmap(bitmap);
			contrTempPinAppt.Size=new Size(_bitmapTempPinAppt.Width,_bitmapTempPinAppt.Height);
			//Graphics
			contrTempPinAppt.Region=new Region(UserControlApptsPanelJ.GetRoundedPath(new RectangleF(0,0,contrTempPinAppt.Width,contrTempPinAppt.Height)));
				//_bitmapTempPinAppt.GetBounds(ref GraphicsUnit.Pixel));
				//.Width,_bitmapTempPinAppt.Height));
		}

		/*
		///<summary>If an appt is already on the pinboard, and the information in it is change externally, this 'refreshes' the data.</summary>
		public void ResetData(long aptNum) {
			ContrApptSingle ctrl=apptList.FirstOrDefault(x => x.AptNum==aptNum);
			if(ctrl==null) {
				return
			}
			DataTable tableAppts=Appointments.RefreshOneApt(aptNum,false).Tables["Appointments"];
			DataRow row=tableAppts.Rows[0];
			if(row["AptStatus"].ToString()=="6") {//planned so do it again the right way
				tableAppts=Appointments.RefreshOneApt(aptNum,true).Tables["Appointments"];
				row=tableAppts.Rows[0];
			}
			//The appt fields are not in DS.Tables["ApptFields"] since the appt is not visible on the schedule.
			DataTable tableApptFields=Appointments.GetApptFields(tableAppts);
			ctrl.ResetData(row
				,tableApptFields
				,Appointments.GetPatFields(tableAppts.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList())
				,ctrl.Location
				,OverlapOrdering.GetOverlapTimes(PIn.Long(row["AptNum"].ToString()))
				,false);
			ctrl.Width=this.Width-2;			
			Invalidate();
		}*/
		#endregion Methods - Public

		
	}

	[Serializable]
	public class PinBoardItem{
		///<summary>Sized to be 2 pixels narrower than the pinboard width.</summary>
		public Bitmap BitmapAppt;
		public long AptNum;
		public DataRow DataRowAppt;
	}

}
