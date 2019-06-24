using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase {
	///<summary>Modern "flat" blue theme</summary>
	public class OdThemeModernBlue:OdThemeOriginal {
		///<summary>Sets the theme to blue colors.</summary>
		public override void SetTheme() {
			base.SetTheme();
			//ODGrid
			SetSolidBrush(ref _gridTitleTopBrush,Color.FromArgb(60,90,150));
			SetSolidBrush(ref _gridTitleBottomBrush,_gridTitleTopBrush.Color);
			SetSolidBrush(ref _gridBackGrndBrush,Color.FromArgb(202,212,222));
			SetSolidBrush(ref _gridHeaderBackBrush,Color.FromArgb(223,234,245));
			SetSolidBrush(ref _gridTitleTextBrush,Color.White);
			SetSolidBrush(ref _gridHeaderTextBrush,Color.Black);
			SetPen(ref _gridOutlinePen,Color.FromArgb(47,70,117));
			//toolbar button
			_toolBarTogglePushedTopColor=Color.FromArgb(255,255,255);
			_toolBarTogglePushedBottomColor=_toolBarTogglePushedTopColor;
			_toolBarHoverTopColor=Color.FromArgb(192,193,216);
			_toolBarHoverBottomColor=_toolBarHoverTopColor;
			_toolBarPushedTopColor=Color.FromArgb(141,151,179);
			_toolBarPushedBottomColor=_toolBarPushedTopColor;
			SetFont(ref _toolBarFont,"Microsoft Sans Serif",8.25f,FontStyle.Regular);
			//Button
			_butCornerRadiusOverride=2f;//-1
			SetPen(ref _butBorderPen,Color.FromArgb(68,70,111));
			//SetSolidBrush(ref _butMainBrush,Color.FromArgb(223,224,235));
			SetSolidBrush(ref _butDarkestBrush,_butMainBrush.Color);
			SetSolidBrush(ref _butLightestBrush,_butMainBrush.Color);//same as the above
			SetSolidBrush(ref _butGlowBrush,Color.FromArgb(0,0,0,0));
			_butCenterColor=Color.FromArgb(0,0,0,0);
			_butReflectionTopColor=Color.FromArgb(0,0,0,0);
			SetSolidBrush(ref _butHoverMainBrush,_butMainBrush.Color);
			SetSolidBrush(ref _butHoverDarkBrush,Color.Gray);
			SetSolidBrush(ref _butHoverLightBrush,Color.White);
			SetSolidBrush(ref _butPressedMainBrush,_toolBarHoverTopColor);
			//outlook bar
			_outlookHoverCornerRadius=1;
			SetSolidBrush(ref _outlookHotBrush,Color.FromArgb(141,151,179));
			SetSolidBrush(ref _outlookPressedBrush,_toolBarHoverTopColor);//Pressed and selected need to be the same color if there is no gradient
			SetSolidBrush(ref _outlookSelectedBrush,_toolBarHoverTopColor);
			SetPen(ref _outlookOutlinePen,SystemColors.Control);
			SetOutlookImages(true);
			_isOutlookImageInverse=false;
			//ButtonPanel
			SetSolidBrush(ref _buttonPanelBackgroundShadowBrush,Color.FromArgb(202,212,222));
			SetPen(ref _buttonPanelOutlinePen,Color.FromArgb(47,70,117));
		}
	}
}
