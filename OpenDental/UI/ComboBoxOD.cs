using System.ComponentModel;
using System.Windows.Forms;

namespace OpenDental.UI {
	public partial class ComboBoxOD:ComboBox {
		private bool _allowScroll=true;

		[Category("Behavior"), Description("Set to true by default.  Allows the combobox to scroll with the scroll wheel."), DefaultValue(true)]
		public bool AllowScroll {
			get { return _allowScroll;}
			set { _allowScroll=value;}
		}

		protected override void OnMouseWheel(MouseEventArgs e) {
			if(!_allowScroll) {
				((HandledMouseEventArgs)e).Handled=true;
				return;
			}			
			base.OnMouseWheel(e);
		}
	}
}