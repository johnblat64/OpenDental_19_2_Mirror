using System.Drawing;

namespace CodeBase {

	///<summary>The blue theme.</summary>
	public class OdThemeBlue:OdThemeOriginal {

		///<summary>Sets the theme to blue colors.</summary>
		public override void SetTheme() {
			base.SetTheme();
			//ODGrid
			SetSolidBrush(ref _gridTitleTopBrush,Color.FromArgb(156,175,230));
			SetSolidBrush(ref _gridTitleBottomBrush,Color.FromArgb(60,90,150));
			SetSolidBrush(ref _gridBackGrndBrush,Color.FromArgb(202,212,222));
			SetSolidBrush(ref _gridHeaderBackBrush,Color.FromArgb(223,234,245));
			SetSolidBrush(ref _gridTitleTextBrush,Color.White);
			SetPen(ref _gridOutlinePen,Color.FromArgb(47,70,117));
			//toolbar button
			_toolBarTogglePushedTopColor=Color.FromArgb(255,255,255);
			_toolBarTogglePushedBottomColor=Color.FromArgb(191,201,229);
			_toolBarHoverTopColor=Color.FromArgb(255,255,255);
			_toolBarHoverBottomColor=Color.FromArgb(171,181,209);
			_toolBarTopColor=Color.FromArgb(255,255,255);
			_toolBarBottomColor=Color.FromArgb(171,181,209);
			_toolBarPushedTopColor=Color.FromArgb(225,225,225);
			_toolBarPushedBottomColor=Color.FromArgb(141,151,179);
			SetFont(ref _toolBarFont,"Microsoft Sans Serif",8.25f,FontStyle.Regular);
			//ButtonPanel
			SetSolidBrush(ref _buttonPanelBackgroundShadowBrush,Color.FromArgb(202,212,222));
			SetPen(ref _buttonPanelOutlinePen,Color.FromArgb(47,70,117));
		}

	}
}
