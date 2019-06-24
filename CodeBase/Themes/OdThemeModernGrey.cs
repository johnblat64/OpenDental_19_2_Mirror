using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase {
	///<summary>Modern "flat" grey theme</summary>
	public class OdThemeModernGrey:OdThemeOriginal {
		///<summary>Sets the theme to orignal colors.</summary>
		public override void SetTheme() {
			base.SetTheme();
			//ODGrid
			SetSolidBrush(ref _gridTitleTopBrush,Color.FromArgb(213,213,223));
			//Button
			_butCornerRadiusOverride=2f;
			SetPen(ref _butBorderPen,Color.FromArgb(192,193,216));
			SetSolidBrush(ref _butDarkestBrush,Color.FromArgb(223,224,235));
			SetSolidBrush(ref _butLightestBrush,Color.FromArgb(223,224,235));
			_butCenterColor				=Color.FromArgb(0,0,0,0);
			_butReflectionTopColor=Color.FromArgb(0,0,0,0);
			SetSolidBrush(ref _butGlowBrush,Color.FromArgb(0,0,0,0));
			SetSolidBrush(ref _butHoverDarkBrush,Color.Gray);
			SetSolidBrush(ref _butHoverLightBrush,Color.White);
			SetSolidBrush(ref _butPressedMainBrush,Color.FromArgb(192,193,216));
			//outlook bar
			_outlookHoverCornerRadius=1;
			SetSolidBrush(ref _outlookHotBrush,Color.FromArgb(210,210,210));
			SetSolidBrush(ref _outlookPressedBrush,Color.FromArgb(235,235,235));//Pressed and selected need to be the same color if there is no gradient
			SetSolidBrush(ref _outlookSelectedBrush,_outlookPressedBrush.Color);
			SetOutlookImages(true);
			_isOutlookImageInverse=false;
		}
	}
}
