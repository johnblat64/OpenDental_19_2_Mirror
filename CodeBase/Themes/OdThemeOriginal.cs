using System.Drawing;

namespace CodeBase {
	///<summary>The original theme.</summary>
	public class OdThemeOriginal:ODColorTheme {

		///<summary>Sets the theme to orignal colors.</summary>
		public override void SetTheme() {
			base.SetTheme();
			//ODGrid
			SetSolidBrush(ref _gridTitleTopBrush,Color.White);
			SetSolidBrush(ref _gridTitleBottomBrush,Color.FromArgb(213,213,223));
			SetSolidBrush(ref _gridBackGrndBrush,Color.FromArgb(224,223,227));
			SetSolidBrush(ref _gridHeaderBackBrush,Color.FromArgb(210,210,210));
			SetSolidBrush(ref _gridTitleTextBrush,Color.Black);
			SetSolidBrush(ref _gridHeaderTextBrush,Color.Black);
			SetPen(ref _gridLinePen,Color.FromArgb(180,180,180));
			SetPen(ref _gridOutlinePen,Color.FromArgb(119,119,146));
			SetPen(ref _gridInnerLinePen,Color.FromArgb(102,102,122));
			SetPen(ref _gridColumnSeparatorPen,Color.FromArgb(120,120,120));
			_gridTitleFontOverride=null;//No override
			_gridHeaderFontOverride=null;//No override
			//toolbar buttons
			_toolBarTogglePushedTopColor=Color.FromArgb(248,248,248);
			_toolBarTogglePushedTopColorError=Color.FromArgb(255,212,212);
			_toolBarTogglePushedBottomColor=Color.FromArgb(248,248,248);
			_toolBarTogglePushedBottomColorError=Color.FromArgb(255,118,118);
			_toolBarHoverTopColor=Color.FromArgb(240,240,240);
			_toolBarHoverTopColorError=Color.FromArgb(255,192,192);
			_toolBarHoverBottomColor=Color.FromArgb(240,240,240);
			_toolBarHoverBottomColorError=Color.FromArgb(255,98,98);
			_toolBarTopColor=SystemColors.Control;
			_toolBarTopColorError=Color.FromArgb(255,192,192);//base color for other error colors (top)
			_toolBarBottomColor=SystemColors.Control;
			_toolBarBottomColorError=Color.FromArgb(255,98,98);//base color for other error colors (bottom)
			_toolBarPushedTopColor=Color.FromArgb(210,210,210);
			_toolBarPushedBottomColor=Color.FromArgb(210,210,210);
			_toolBarPushedTopColorError=Color.FromArgb(225,162,162);
			_toolBarPushedBottomColorError=Color.FromArgb(225,68,68);
			_colorNotify=Color.FromArgb(252,178,129);
			_colorNotifyDark=Color.FromArgb(182,98,44);
			SetSolidBrush(ref _toolBarTextForeBrush,Color.Black);
			SetSolidBrush(ref _toolBarTextForeBrushError,Color.Black);//derived from DefaultForeColor on ODToolBar.
			SetPen(ref _toolBarPenOutlinePen,Color.SlateGray);
			SetPen(ref _toolBarPenOutlinePenError,Color.SlateGray);
			SetSolidBrush(ref _toolBarTextDisabledBrush,SystemColors.GrayText);
			SetSolidBrush(ref _toolBarTextDisabledBrushError,SystemColors.GrayText);
			SetPen(ref _toolBarDividerPen,Color.FromArgb(180,180,180));
			SetPen(ref _toolBarDividerPenError,Color.FromArgb(180,180,180));
			SetSolidBrush(ref _toolBarDisabledOverlayBrush,Color.Empty);
			SetFont(ref _toolBarFont,FontFamily.GenericSansSerif,8.25f,FontStyle.Regular);
			SetFont(ref _fontMenuItem,"Microsoft Sans Serif",9,FontStyle.Regular);
			_butCornerRadiusOverride=4;
			SetPen(ref _butBorderPen,Color.FromArgb(28,81,128));
			SetSolidBrush(ref _butDisabledTextBrush,Color.FromArgb(161,161,146));
			SetSolidBrush(ref _butDarkestBrush,Color.FromArgb(157,164,196));
			SetSolidBrush(ref _butLightestBrush,Color.FromArgb(255,255,255));
			SetSolidBrush(ref _butMainBrush,Color.FromArgb(223,224,235));
			SetSolidBrush(ref _butPressedDarkestBrush,Color.FromArgb(157,164,196));
			SetSolidBrush(ref _butPressedLightestBrush,Color.FromArgb(255,255,255));
			SetSolidBrush(ref _butPressedMainBrush,Color.FromArgb(223,224,235));
			SetSolidBrush(ref _butDefaultDarkBrush,Color.FromArgb(50,70,230));
			SetSolidBrush(ref _butHoverDarkBrush,Color.FromArgb(255,190,100));
			SetSolidBrush(ref _butHoverLightBrush,Color.FromArgb(255,210,130));
			SetSolidBrush(ref _butHoverMainBrush,Color.FromArgb(223,224,235));
			SetSolidBrush(ref _butTextBrush,Color.Black);
			SetSolidBrush(ref _butGlowBrush,Color.White);
			_butCenterColor=Color.FromArgb(255,255,255,255);
			_butReflectionBotColor=Color.FromArgb(0,0,0,0);
			_butReflectionTopColor=Color.FromArgb(50,0,0,0);
			//outlook bar
			_outlookHoverCornerRadius=4;
			SetSolidBrush(ref _outlookHotBrush,Color.FromArgb(235,235,235));
			SetSolidBrush(ref _outlookPressedBrush,Color.FromArgb(210,210,210));
			SetSolidBrush(ref _outlookSelectedBrush,Color.FromArgb(255,255,255));
			SetPen(ref _outlookOutlinePen,Color.FromArgb(28,81,128));
			SetSolidBrush(ref _outlookTextBrush,Color.Black);
			SetSolidBrush(ref _outlookBackBrush,SystemColors.Control);
			SetOutlookImages(false);
			_isOutlookImageInverse=false;
			//odform
			_formBackColor=SystemColors.Control;
			//ButtonPanel
			SetSolidBrush(ref _buttonPanelLabelBackgroundBrush,Color.White);
			SetSolidBrush(ref _buttonPanelLabelBrush,Color.Black);
			SetSolidBrush(ref _buttonPanelBackgroundShadowBrush,Color.FromArgb(224,223,227));
			SetSolidBrush(ref _buttonPanelBackgroundBrush,Color.White);
			SetPen(ref _buttonPanelOutlinePen,Color.FromArgb(119,119,146));
		}

	}
}
