using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Threading;

namespace CodeBase {
	///<summary>Contains all color information for the currently selected ODColorTheme</summary>
	public class ODColorTheme {
		#region Private Variables
		
		private static OdTheme _themeCur=OdTheme.None;
		private static ConcurrentDictionary<string,Action> _dictOnThemeChangedActions=new ConcurrentDictionary<string,Action>();
		///<summary>Locks objects so we don't half set them in the middle of changing themes.</summary>
		private static ReaderWriterLockSlim _locker=new ReaderWriterLockSlim();
		///<summary>True if the current theme has flat icons in main toolbar and main outlook bar.</summary>
		private static bool _hasFlatIcons=false;
		#region Private Grid Variables
		//odgrid
		///<summary>ODGrid title top color.</summary>
		protected static SolidBrush _gridTitleTopBrush=null;
		///<summary>ODGrid title bottom color.</summary>
		protected static SolidBrush _gridTitleBottomBrush=null;
		///<summary>ODGrid background color (color of the grid itself, not the rows).</summary>
		protected static SolidBrush _gridBackGrndBrush=null;
		///<summary>ODGrid column header color.</summary>
		protected static SolidBrush _gridHeaderBackBrush=null;
		///<summary>ODGrid title text color.</summary>
		protected static SolidBrush _gridTitleTextBrush=null;
		///<summary>ODGrid title text color.</summary>
		protected static SolidBrush _gridHeaderTextBrush=null;
		///<summary>ODGrid outline color.</summary>
		protected static Pen _gridLinePen=null;
		///<summary></summary>
		protected static Pen _gridOutlinePen=null;
		///<summary>.</summary>
		protected static Pen _gridColumnSeparatorPen=null;
		///<summary>.</summary>
		protected static Pen _gridInnerLinePen=null;
		///<summary>ODGrid title font.</summary>
		protected static Font _gridTitleFontOverride=null;
		///<summary>ODGrid column header font.</summary>
		protected static Font _gridHeaderFontOverride=null;
		#endregion
		#region Private Toolbar Variables
		//odtoolbar
		///<summary>.</summary>
		protected static SolidBrush _toolBarTextForeBrush=null;
		///<summary>.</summary>
		protected static SolidBrush _toolBarTextForeBrushError=null;
		///<summary>.</summary>
		protected static Pen _toolBarPenOutlinePen=null;
		///<summary>.</summary>
		protected static Pen _toolBarPenOutlinePenError=null;
		///<summary>.</summary>
		protected static SolidBrush _toolBarTextDisabledBrush=null;
		///<summary>.</summary>
		protected static SolidBrush _toolBarTextDisabledBrushError=null;
		///<summary>.</summary>
		protected static Pen _toolBarDividerPen=null;
		///<summary>.</summary>
		protected static Pen _toolBarDividerPenError=null;
		///<summary></summary>
		protected static SolidBrush _toolBarDisabledOverlayBrush=null;
		///<summary>Toolbar Button Text Font.</summary>
		protected static Font _toolBarFont=null;
		///<summary></summary>
		protected static Color _toolBarTogglePushedTopColor;
		///<summary></summary>
		protected static Color _toolBarTogglePushedTopColorError;
		///<summary></summary>
		protected static Color _toolBarTogglePushedBottomColor;
		///<summary></summary>
		protected static Color _toolBarTogglePushedBottomColorError;
		///<summary></summary>
		protected static Color _toolBarHoverTopColor;
		///<summary></summary>
		protected static Color _toolBarHoverTopColorError;
		///<summary></summary>
		protected static Color _toolBarHoverBottomColor;
		///<summary></summary>
		protected static Color _toolBarHoverBottomColorError;
		///<summary></summary>
		protected static Color _toolBarTopColor;
		///<summary></summary>
		protected static Color _toolBarTopColorError;
		///<summary></summary>
		protected static Color _toolBarBottomColor;
		///<summary></summary>
		protected static Color _toolBarBottomColorError;
		///<summary></summary>
		protected static Color _toolBarPushedTopColor;
		///<summary></summary>
		protected static Color _toolBarPushedTopColorError;
		///<summary></summary>
		protected static Color _toolBarPushedBottomColor;
		///<summary></summary>
		protected static Color _toolBarPushedBottomColorError;
		///<summary></summary>
		protected static Color _colorNotify;
		///<summary></summary>
		protected static Color _colorNotifyDark;
		#endregion
		#region Private Button Variables
		///<summary>Button border color.</summary>
		protected static Pen _butBorderPen = null;
		///<summary>Button disabled text color.</summary>
		protected static SolidBrush _butDisabledTextBrush = null;
		///<summary>Button shading at lower and right borders.</summary>
		protected static SolidBrush _butDarkestBrush = null;
		///<summary>Button overall main color. </summary>
		protected static SolidBrush _butMainBrush = null;
		///<summary>Button light color. </summary>
		protected static SolidBrush _butLightestBrush = null;
		///<summary>Button shading at lower and right borders. Button can gradient between three colors -- Darkest, Main, Lightest.</summary>
		protected static SolidBrush _butPressedDarkestBrush = null;
		///<summary>Button overall main color. Button can gradient between three colors -- Darkest, Main, Lightest.</summary>
		protected static SolidBrush _butPressedMainBrush = null;
		///<summary>Button light color.  Button can gradient between three colors -- Darkest, Main, Lightest.</summary>
		protected static SolidBrush _butPressedLightestBrush = null;
		///<summary>Button default dark color.  Replaces DarkestColor when the button is the default, focused button.</summary>
		protected static SolidBrush _butDefaultDarkBrush = null;
		///<summary>Button hover dark color.  Replaces DarkestColor when the button is hovered over.</summary>
		protected static SolidBrush _butHoverDarkBrush = null;//the outline when hovering
		///<summary>Button hover light color.  Replaces MainColor when the button is hovered over.</summary>
		protected static SolidBrush _butHoverMainBrush = null;
		///<summary>Button hover light color.  Replaces LightestColor when the button is hovered over.</summary>
		protected static SolidBrush _butHoverLightBrush = null;
		///<summary>Button text color.</summary>
		protected static SolidBrush _butTextBrush = null;
		///<summary>A slight glow that text on buttons give off. Normally white.</summary>
		protected static SolidBrush _butGlowBrush = null;
		///<summary>The "glare" of the button, near the center. Set to clear if you don't want a glare effect.</summary>
		protected static Color _butCenterColor;
		///<summary>Overlays the bottom half of the button. The middle of the button will be set to _butReflectionTopColor, then gradient to this color.
		///Set to clear if you don't want a gradient effect.</summary>
		protected static Color _butReflectionBotColor;
		///<summary>Overlays the bottom half of the button. The middle of the button will be set to this color, then gradient to _butReflectionBotColor.
		///Set to clear if you don't want a gradient effect.</summary>
		protected static Color _butReflectionTopColor;
		///<summary></summary>
		protected static float _butCornerRadiusOverride;

		#endregion
		#region Private OutlookBar Variables
		//main modules outlook bar
		///<summary>Newer themes are square, others are rounded.</summary>
		protected static float _outlookHoverCornerRadius=0;
		///<summary>When an item is hovered over.</summary>
		protected static SolidBrush _outlookHotBrush=null;
		///<summary>When an item is pressed.</summary>
		protected static SolidBrush _outlookPressedBrush=null;
		///<summary>When the item is currently selected.</summary>
		protected static SolidBrush _outlookSelectedBrush=null;
		///<summary>Outline.</summary>
		protected static Pen _outlookOutlinePen=null;
		///<summary>Text Color.</summary>
		protected static SolidBrush _outlookTextBrush=null;
		///<summary>Background color.</summary>
		protected static SolidBrush _outlookBackBrush=null;
		///<summary>.</summary>
		protected static int _outlookApptImageIndex=0;
		///<summary>.</summary>
		protected static int _outlookFamilyImageIndex=1;
		///<summary>.</summary>
		protected static int _outlookAcctImageIndex=2;
		///<summary>.</summary>
		protected static int _outlookTreatPlanImageIndex=3;
		///<summary>.</summary>
		protected static int _outlookChartImageIndex=4;
		///<summary>.</summary>
		protected static int _outlookImagesImageIndex=5;
		///<summary>.</summary>
		protected static int _outlookManageImageIndex=6;
		///<summary>.</summary>
		protected static int _outlookEcwTreatPlanImageIndex=7;
		///<summary>.</summary>
		protected static int _outlookEcwChartImageIndex=8;
		///<summary></summary>
		protected static bool _isOutlookImageInverse=false;
		#endregion
		#region Private ButtonPanel Variables
		///<summary>Button panel brush for label text.</summary>
		protected static SolidBrush _buttonPanelLabelBrush=null;
		///<summary>Button panel brush for label background.</summary>
		protected static SolidBrush _buttonPanelLabelBackgroundBrush=null;
		///<summary>Button panel brush for panel background shadow.</summary>
		protected static SolidBrush _buttonPanelBackgroundShadowBrush=null;
		///<summary>Button panel brush for panel background.</summary>
		protected static SolidBrush _buttonPanelBackgroundBrush=null;
		///<summary>Button panel pen for control outline.</summary>
		protected static Pen _buttonPanelOutlinePen=null;
		#endregion

		#region ODForm
		///<summary></summary>
		protected static Color _formBackColor;
		protected static Font _fontMenuItem;
		#endregion

		#endregion

		#region Getters

		///<summary>The current theme in use.</summary>
		public static OdTheme ThemeCur {
			get { return _themeCur; }
		}

		///<summary>Boolean.  True if the current theme is one of the original open dental themes.</summary>
		public static bool ThemeCurIsOldTheme {
			get {
				try {
					return (_themeCur==OdTheme.Blue || _themeCur==OdTheme.Original);
				}
				catch {
					return true;
				}
			}
		}

		private static T Get<T>(ref T obj) {
			if(_themeCur==OdTheme.None) {
				SetTheme(OdTheme.Original);//Locks for write to block reading.
			}
			_locker.EnterReadLock();//Block writing.
			try {
				return obj;
			}
			finally {
				_locker.ExitReadLock();
			}
		}

		///<summary>True if the current theme has flat icons in main toolbar and main outlook bar.</summary>
		public static bool HasFlatIcons {
			get { return _hasFlatIcons;	}
		}

		///<summary>Orginally used for buttons.  The color of the outline of the button.</summary>
		public static Pen ButBorderPen {
			get {	return Get(ref _butBorderPen);}
		}

		///<summary>Orginally used for buttons.  The center color of the button. Needs to remain a color for now.</summary>
		public static Color ButCenterColor {
			get { return Get(ref _butCenterColor); }
		}

		///<summary>Orginally used for buttons.  -1 if no override. The roundness of the button corners.</summary>
		public static float ButCornerRadiusOverride {
			get { return Get(ref _butCornerRadiusOverride); }
		}

		///<summary>Orginally used for buttons.  The darkest color of the button.</summary>
		public static SolidBrush ButDarkestBrush {
			get { return Get(ref _butDarkestBrush); }
		}

		///<summary>Orginally used for buttons.  The default darkest color of the button.</summary>
		public static SolidBrush ButDefaultDarkBrush {
			get { return Get(ref _butDefaultDarkBrush); }
		}

		///<summary>Orginally used for buttons.  The text color on the button when it is disabled.</summary>
		public static SolidBrush ButDisabledTextBrush {
			get { return Get(ref _butDisabledTextBrush); }
		}

		///<summary>Orginally used for buttons.  The color of the glow for the button.</summary>
		public static SolidBrush ButGlowBrush {
			get { return Get(ref _butGlowBrush); }
		}

		///<summary>Orginally used for buttons.  The darkest color of the button when it is being hovered over.</summary>
		public static SolidBrush ButHoverDarkBrush {
			get { return Get(ref _butHoverDarkBrush); }
		}

		///<summary>Orginally used for buttons.  The lightest color of the button when it is being hovered over.</summary>
		public static SolidBrush ButHoverLightBrush {
			get { return Get(ref _butHoverLightBrush); }
		}

		///<summary>Orginally used for buttons.  The main color of the button when it is being hovered over. </summary>
		public static SolidBrush ButHoverMainBrush {
			get { return Get(ref _butHoverMainBrush); }
		}

		///<summary>Orginally used for buttons.  The lightest color of the button.</summary>
		public static SolidBrush ButLightestBrush {
			get { return Get(ref _butLightestBrush); }
		}

		///<summary>Orginally used for buttons.  The main color of the button.</summary>
		public static SolidBrush ButMainBrush {
			get { return Get(ref _butMainBrush); }
		}

		///<summary>Orginally used for buttons.  The darkest color of the button when it is being pressed.</summary>
		public static SolidBrush ButPressedDarkestBrush {
			get { return Get(ref _butPressedDarkestBrush); }
		}

		///<summary>Orginally used for buttons.  The lightest color of the button when it is being pressed.</summary>
		public static SolidBrush ButPressedLightestBrush {
			get { return Get(ref _butPressedLightestBrush); }
		}

		///<summary>Orginally used for buttons.  The main color of the button when it is being pressed.</summary>
		public static SolidBrush ButPressedMainBrush {
			get { return Get(ref _butPressedMainBrush); }
		}

		///<summary>Orginally used for buttons.  The bottom color of the button reflection.</summary>
		public static Color ButReflectionBottomColor {
			get { return Get(ref _butReflectionBotColor); }
		}

		///<summary>Orginally used for buttons.  The top color of the button reflection. Used to make a linear gradient brush, just for color.</summary>
		public static Color ButReflectionTopColor {
			get { return Get(ref _butReflectionTopColor); }
		}

		///<summary>Orginally used for buttons.  The color of the text on the button.</summary>
		public static SolidBrush ButTextColor {
			get { return Get(ref _butTextBrush); }
		}

		///<summary>The same orange color as the unread task notification color.</summary>
		public static Color ColorNotify {
			get {return _colorNotify; }
		}

		///<summary>The darker version of ColorNotify.  This color is useful for coloring grid cell text, because it contrasts well with the 
		///normal white background, as well as the slate gray background of selected cells, as well as the normal black text.  Calculated by using website 
		///paletton.com.  This darker color is not calculated by using a simple division or subtraction,
		///but is instead calculated using a more advanced formula.</summary>
		public static Color ColorNotifyDark {
			get { return _colorNotifyDark;}
		}

		///<summary>Usually set to the same font as the rest of the theme. This has been created specifically for menuItems since their font size 
		///sometimes needs to be different. </summary>
		public static Font FontMenuItem {
			get { return Get(ref _fontMenuItem);}
		}

		///<summary>Originally for OdForm.  The background color of the form.</summary>
		public static Color FormBackColor {
			get {	return Get(ref _formBackColor); }
		}

		///<summary>Originally from OdGrid.  The grid's background color when no rows are present.</summary>
		public static SolidBrush GridBackGrndBrush {
			get { return Get(ref _gridBackGrndBrush); }
		}

		///<summary>Originally from OdGrid.  The color of the vertical line that separates two columns in the grid.</summary>
		public static Pen GridColumnSeparatorPen {
			get { return Get(ref _gridColumnSeparatorPen); }
		}

		///<summary>Originally from OdGrid.  Null if no override.  The secondary header bar font.</summary>
		public static Font GridHeaderFont {
			get { return Get(ref _gridHeaderFontOverride); }
		}

		///<summary>Originally from OdGrid.</summary>
		public static Pen GridInnerLinePen {
			get { return Get(ref _gridInnerLinePen); }
		}

		///<summary>Originally from OdGrid.  The color of the text for the main title bar.</summary>
		public static SolidBrush GridTextBrush {
			get { return Get(ref _gridTitleTextBrush); }
		}

		///<summary>Originally from OdGrid.  Null if no override.  The font for the main title bar. </summary>
		public static Font GridTitleFont {
			get { return Get(ref _gridTitleFontOverride); }
		}

		///<summary>Originally from OdGrid.  The secondary title, header bar's color.</summary>
		public static SolidBrush GridHeaderBackBrush {
			get { return Get(ref _gridHeaderBackBrush); }
		}

		///<summary>Originally from OdGrid.  The color of the text for the secondary headers bar.</summary>
		public static SolidBrush GridHeaderTextBrush {
			get { return Get(ref _gridHeaderTextBrush); }
		}

		///<summary>Originally used for outlook bar.  Booelan for if the images of an opposite color scheme (ex. white instead of black).</summary>
		public static bool IsImageInverse {
			get { return Get(ref _isOutlookImageInverse); }
		}

		///<summary>Originally from OdGrid.  The grid line(?).</summary>
		public static Pen GridLinePen {
			get { return Get(ref _gridLinePen); }
		}

		/// <summary>Originally from OdGrid. The outline of the control.</summary>
		public static Pen GridOutlinePen {
			get { return Get(ref _gridOutlinePen); }
		}

		///<summary>Originally used for outlook bar.  The index of the image for the account module.</summary>
		public static int OutlookAcctImageIndex {
			get { return Get(ref _outlookAcctImageIndex); }
		}

		///<summary>Originally used for outlook bar.  The index of the image for the appointment module.</summary>
		public static int OutlookApptImageIndex {
			get { return Get(ref _outlookApptImageIndex); }
		}

		///<summary>Originally used for outlook bar.  The background color of the outlook bar buttons when not selected.</summary>
		public static SolidBrush OutlookBackBrush {
			get { return Get(ref _outlookBackBrush); }
		}

		///<summary>Originally used for outlook bar.  The index of the image for the chart module.</summary>
		public static int OutlookChartImageIndex {
			get { return Get(ref _outlookChartImageIndex); }
		}

		///<summary>Originally used for outlook bar.  The index of the image for ecw chart.</summary>
		public static int OutlookEcwChartImageIndex {
			get { return Get(ref _outlookEcwChartImageIndex); }
		}

		///<summary>Originally used for outlook bar.  The index of the image for ecw treatment plan.</summary>
		public static int OutlookEcwTreatPlanImageIndex {
			get { return Get(ref _outlookEcwTreatPlanImageIndex); }
		}

		///<summary>Originally used for outlook bar.  The index of the image for the family module.</summary>
		public static int OutlookFamilyImageIndex {
			get { return Get(ref _outlookFamilyImageIndex); }
		}

		///<summary>Originally used for outlook bar.  The color of the outlook button when hovering over it.</summary>
		public static SolidBrush OutlookHotBrush {
			get { return Get(ref _outlookHotBrush); }
		}

		///<summary>Originally used for outlook bar.  The roundness of the outlook button corners.</summary>
		public static float OutlookHoverCornerRadius {
			get { return Get(ref _outlookHoverCornerRadius); }
		}

		///<summary>Originally used for outlook bar.  The index of the image for image module.</summary>
		public static int OutlookImagesImageIndex {
			get { return Get(ref _outlookImagesImageIndex); }
		}

		///<summary>Originally used for outlook bar.  The index of the image for the manage module.</summary>
		public static int OutlookManageImageIndex {
			get { return Get(ref _outlookManageImageIndex); }
		}

		///<summary>Originally used for outlook bar.  The color of the outline of the outlook bar button when it is selected.</summary>
		public static Pen OutlookOutlineColor {
			get { return Get(ref _outlookOutlinePen); }
		}

		///<summary>Originally used for outlook bar.  The color of the outlook bar button when it is actively being pressed.</summary>
		public static SolidBrush OutlookPressedBrush {
			get { return Get(ref _outlookPressedBrush); }
		}

		///<summary>Originally used for outlook bar.  The color of the outlook button when it is seleted.</summary>
		public static SolidBrush OutlookSelectedBrush {
			get { return Get(ref _outlookSelectedBrush); }
		}

		///<summary>Originally used for outlook bar.  The text color of the outlook bar buttons.</summary>
		public static SolidBrush OutlookTextBrush {
			get { return Get(ref _outlookTextBrush); }
		}

		///<summary>Originally used for outlook bar.  The index of the image for the treatment plan module.</summary>
		public static int OutlookTreatPlanImageIndex {
			get { return Get(ref _outlookTreatPlanImageIndex); }
		}

		///<summary>Originally from OdGrid.  The bottom color of the main title bar.</summary>
		public static SolidBrush TitleBottomBrush {
			get { return Get(ref _gridTitleBottomBrush); }
		}

		///<summary>Originally from OdGrid.  The top color of the main title bar.</summary>
		public static SolidBrush TitleTopBrush {
			get { return Get(ref _gridTitleTopBrush); }
		}

		///<summary>Originally used in the toolbar.  The font of toolbar buttons.</summary>
		public static Font ToolBarFont {
			get { return Get(ref _toolBarFont); }
		}

		///<summary>Originally used in the toolbar.</summary>
		public static SolidBrush ToolBarDisabledOverlayBrush {
			get { return Get(ref _toolBarDisabledOverlayBrush); }
		}

		///<summary>Originally used in the toolbar.  The color of the vertical line that separates tool bar buttons.</summary>
		public static Pen ToolBarDividerPen {
			get { return Get(ref _toolBarDividerPen); }
		}

		///<summary>Originally used in the toolbar.  The color of the vertical line that separates error tool bar buttons.</summary>
		public static Pen ToolBarDividerPenError {
			get { return Get(ref _toolBarDividerPenError); }
		}

		///<summary>Originally used in the toolbar.  The color of the toolbar button outline.</summary>
		public static Pen ToolBarOutlinePen {
			get { return Get(ref _toolBarPenOutlinePen); }
		}

		///<summary>Originally used in the toolbar.  The color of the error toolbar button outline.</summary>
		public static Pen ToolBarPenOutlinePenError {
			get { return Get(ref _toolBarPenOutlinePenError); }
		}

		///<summary>Originally used in the toolbar.  The color of the text for toolbar buttons when disabled.</summary>
		public static SolidBrush ToolBarTextDisabledBrush {
			get { return Get(ref _toolBarTextDisabledBrush); }
		}

		///<summary>Originally used in the toolbar.  The color of the text for toolbar buttons when disabled.</summary>
		public static SolidBrush ToolBarTextDisabledBrushError {
			get { return Get(ref _toolBarTextDisabledBrushError); }
		}

		///<summary>Originally used in the toolbar.  The main color of the text for toolbar buttons.</summary>
		public static SolidBrush ToolBarTextForeBrush {
			get { return Get(ref _toolBarTextForeBrush); }
		}

		///<summary>Originally used in the toolbar.  The main color of the text for toolbar buttons.</summary>
		public static SolidBrush ToolBarTextForeBrushError {
			get { return Get(ref _toolBarTextForeBrushError); }
		}

		///<summary>Originally used in the toolbar.  The top gradient color of the tool bar button when pressed and is a toggle</summary>
		public static Color ToolBarTogglePushedTopColor {
			get { return Get(ref _toolBarTogglePushedTopColor); }
		}

		///<summary>Originally used in the toolbar.  The top gradient color of the tool bar button when pressed, and is a toggle
		///Created from algorithm Color=(Math.Min(255,baseColor.R+20,),Math.Min(255,baseColor.G+20),Math.Min(255,baseColor.B+20))</summary>
		public static Color ToolBarTogglePushedTopColorError {
			get { return Get(ref _toolBarTogglePushedTopColorError); }
		}

		///<summary>Originally used in the toolbar.  The bottomw color of the tool bar button when pressed and is a toggle
		///Created from algorithm. newColor=(Math.Min(255,baseColor.R+20,),Math.Min(255,baseColor.G+20),Math.Min(255,baseColor.B+20))</summary>
		public static Color ToolBarTogglePushedBottomColor {
			get { return Get(ref _toolBarTogglePushedBottomColor); }
		}

		///<summary>Originally used in the toolbar.  The bottomw color of the tool bar button when pressed and is a toggle and error.</summary>
		public static Color ToolBarTogglePushedBottomColorError {
			get { return Get(ref _toolBarTogglePushedBottomColorError); }
		}

		///<summary>Originally used in the toolbar.  The top color of the tool bar button when hovered over.</summary>
		public static Color ToolBarHoverTopColor {
			get { return Get(ref _toolBarHoverTopColor); }
		}

		///<summary>Originally used in the toolbar.  The top color of the tool bar button when hovered over.</summary>
		public static Color ToolBarHoverTopColorError {
			get { return Get(ref _toolBarHoverTopColorError); }
		}

		///<summary>Originally used in the toolbar.  The bottom color of the tool bar button when hovered over.</summary>
		public static Color ToolBarHoverBottomColor {
			get { return Get(ref _toolBarHoverBottomColor); }
		}

		///<summary>Originally used in the toolbar.  The bottom color of the tool bar button when hovered over.</summary>
		public static Color ToolBarHoverBottomColorError {
			get { return Get(ref _toolBarHoverBottomColorError); }
		}

		///<summary>Originally used in the toolbar.  The top color of the tool bar buttons.</summary>
		public static Color ToolBarTopColor {
			get { return Get(ref _toolBarTopColor); }
		}

		///<summary>Originally used in the toolbar.  The top color of the tool bar buttons.</summary>
		public static Color ToolBarTopColorError {
			get { return Get(ref _toolBarTopColorError); }
		}

		///<summary>Originally used in the toolbar.  The bottom color of the toolbar buttons.</summary>
		public static Color ToolBarBottomColor {
			get { return Get(ref _toolBarBottomColor); }
		}

		///<summary>Originally used in the toolbar.  The bottom color of the toolbar buttons.</summary>
		public static Color ToolBarBottomColorError {
			get { return Get(ref _toolBarBottomColorError); }
		}

		///<summary>Originally used in the toolbar.  The top color of the toolbar buttons when they're disabled.</summary>
		public static Color ToolBarPressedTopColor {
			get { return Get(ref _toolBarPushedTopColor); }//used to be tool bar dark color
		}

		///<summary></summary>
		public static Color ToolBarPressedTopColorError {
			get {	return Get(ref _toolBarPushedTopColorError); }
		}

		///<summary>Originally used in the toolbar.  The bottom color of the toolbar buttons when thye're disabled.</summary>
		public static Color ToolBarPressedBottomColor {
			get { return Get(ref _toolBarPushedBottomColor); }//used to be tool bar dark color
		}

		///<summary></summary>
		public static Color ToolBarPressedBottomColorError {
			get { return Get(ref _toolBarPushedBottomColorError); }
		}

		///<summary>Text of the label. Originally from panel in the chart module.</summary>
		public static SolidBrush ButtonPanelLabelBrush {
			get { return Get(ref _buttonPanelLabelBrush); }
		}

		///<summary>Background of the label. Originally from panel in the chart module.</summary>
		public static SolidBrush ButtonPanelLabelBackgroundBrush {
			get { return Get(ref _buttonPanelLabelBackgroundBrush); }
		}

		///<summary>Color of the shadow for entire panel. Originally from panel in the chart module.</summary>
		public static SolidBrush ButtonPanelBackgroundShadowBrush {
			get { return Get(ref _buttonPanelBackgroundShadowBrush); }
		}

		///<summary>Color of the entire panel. Originally from panel in the chart module.</summary>
		public static SolidBrush ButtonPanelBackgroundBrush {
			get { return Get(ref _buttonPanelBackgroundBrush); }
		}

		///<summary>Outline of the button panel. Orignally from panel in the chart module.</summary>
		public static Pen ButtonPanelOutlinePen {
			get { return Get(ref _buttonPanelOutlinePen); }
		}

		/////<summary>There might be a function in ContrImages which does this (CodeBase?).  Consider cerntralizing if possible.</summary>
		//public static ImageAttributes GetInverseAttributes() {
		//	ColorMatrix c=new ColorMatrix(new float[][] {
		//		new float[] {-1, 0, 0, 0, 0},
		//		new float[] {0, -1, 0, 0, 0},
		//		new float[] {0, 0, -1, 0, 0},
		//		new float[] {0, 0, 0, 1, 0},
		//		new float[] {1, 1, 1, 0, 1}
		//	});
		//	ImageAttributes attributes=new ImageAttributes();
		//	attributes.SetColorMatrix(c);
		//	return attributes;
		//}
		#endregion Getters

		#region Setters
		///<summary>Disposes of the previous brush (do not pass in a system brush), then initializes the font using the given parameters.</summary>
		protected static void SetSolidBrush(ref SolidBrush brush,Color color) {
			if(brush==null) {
				brush=new SolidBrush(color);
			}
			else {
				brush.Color=color;
			}			
		}

		///<summary>Disposes of the previous pen (do not pass in a system pen), then initializes the font using the given parameters.</summary>
		protected static void SetPen(ref Pen pen,Color color) {
			Pen tempPen=pen;
			pen=new Pen(color);
			if(tempPen!=null) {
				tempPen.Dispose();
				tempPen=null;
			}
		}

		///<summary>Disposes of the previous font if it is not a system font, then initializes the font using the given parameters.</summary>
		protected static void SetFont(ref Font font,FontFamily family,float emSize,FontStyle style) {
			if(font!=null && font.FontFamily==family && font.Size==emSize && font.Style==style) {
				return;//No changed needed.
			}
			Font tempFont=font;
			font=new Font(family,emSize,style);
			if(tempFont!=null && !tempFont.IsSystemFont) {
				//For some reason disposing the font was causing errors when switching the theme.
				//Theme switching does not happen often, thus we are OK with leaving it floating in memory.
				//tempFont.Dispose();
				tempFont=null;
			}			
		}

		///<summary>Disposes of the previous font if it is not a system font, then initializes the font using the given parameters.</summary>
		protected static void SetFont(ref Font font,string familyName,float emSize,FontStyle style) {
			if(font!=null && font.FontFamily.Name==familyName && font.Size==emSize && font.Style==style) {
				return;//No changed needed.
			}
			Font tempFont=font;
			font=new Font(familyName,emSize,style);
			if(tempFont!=null && !tempFont.IsSystemFont) {
				//For some reason disposing the font was causing errors when switching the theme.
				//Theme switching does not happen often, thus we are OK with leaving it floating in memory.
				//tempFont.Dispose();
				tempFont=null;
			}
		}

		/// <summary>Sets the and reloads the theme colors based on the passed in enum.</summary>
		public static void SetTheme(OdTheme themeCur) {
			if(_themeCur==themeCur) {
				return;
			}
			_locker.EnterWriteLock();
			try {
				_themeCur=themeCur;
				ODColorTheme themeSetter=null;
				switch(themeCur) {
					#region Original
					case OdTheme.Original:
					default:
						themeSetter=new OdThemeOriginal();
						break;
					#endregion
					#region Blue Theme
					case OdTheme.Blue:
						themeSetter=new OdThemeBlue();
						break;
					#endregion
					case OdTheme.MonoFlatBlue:
						themeSetter=new OdThemeModernBlue();
						break;
					case OdTheme.MonoFlatGray:
						themeSetter=new OdThemeModernGrey();
						break;
				}
				themeSetter.SetTheme();
			}
			finally {
				_locker.ExitWriteLock();
			}
			foreach(Action action in _dictOnThemeChangedActions.Values) {
				action.Invoke();
			}
		}

		///<summary>To be implemented in next version.  Will set all of the colors for the overridden theme.</summary>
		public virtual void SetTheme() {
		}

		///<summary></summary>
		protected static void SetOutlookImages(bool hasFlatIcons) {
			_hasFlatIcons=hasFlatIcons;
			if(hasFlatIcons) {
				_outlookApptImageIndex					=9;
				_outlookFamilyImageIndex				=10;
				_outlookAcctImageIndex					=11;
				_outlookTreatPlanImageIndex			=12;
				_outlookChartImageIndex					=13;
				_outlookImagesImageIndex				=14;
				_outlookManageImageIndex				=15;
				_outlookEcwTreatPlanImageIndex	=12;
				_outlookEcwChartImageIndex			=13;
			}
			else {
				_outlookApptImageIndex					=0;
				_outlookFamilyImageIndex				=1;
				_outlookAcctImageIndex					=2;
				_outlookTreatPlanImageIndex			=3;
				_outlookChartImageIndex					=4;
				_outlookImagesImageIndex				=5;
				_outlookManageImageIndex				=6;
				_outlookEcwTreatPlanImageIndex	=7;
				_outlookEcwChartImageIndex			=8;
			}
		}
		#endregion Setters

		///<summary>Thread safe.  If the same action is added multiple times, then the action will only be called once when the theme is changed.</summary>
		public static void AddOnThemeChanged(Action action) {
			_dictOnThemeChangedActions[action.Method.ToString()]=action;
		}

		///<summary></summary>
		public static Image InvertImageIfNeeded(Image img) {
			if(!IsImageInverse) {
				return img;
			}
			Bitmap inversion=new Bitmap(img);
			for(int y=0;(y<=(inversion.Height-1));y++) {
				for(int x=0;(x<=(inversion.Width-1));x++) {
					Color inv=inversion.GetPixel(x,y);
					inv=Color.FromArgb(inv.A,(255-inv.R),(255-inv.G),(255-inv.B));
					inversion.SetPixel(x,y,inv);
				}
			}
			return inversion;
		}

	}

	///<summary>.</summary>
	public enum OdTheme {
		///<summary>-1 - Do not save to database.  Only used during initalization to indicate that a default must be set.</summary>
		[Description("None")]
		None=-1,
		///<summary>0 - Original</summary>
		[Description("Original")]
		Original,
		///<summary>1 - Blue</summary>
		[Description("Blue")]
		Blue,
		///<summary>2 - MonoFlat Gray</summary>
		[Description("MonoFlat Gray")]
		MonoFlatGray,
		///<summary>3 - MonoFlat Blue</summary>
		[Description("MonoFlat Blue")]
		MonoFlatBlue,
	}

}
