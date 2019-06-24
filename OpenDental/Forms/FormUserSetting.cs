using OpenDentBusiness;
using System;
using System.Linq;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental {
	/// <summary>
	/// This form (per Nathan) should be used for any future features that could be categorized as a user setting. The intent of this class was to
	/// create a place for specific user settings.
	/// </summary>
	public partial class FormUserSetting:ODForm {
		private UserOdPref _suppressLogOffMessage;
		private UserOdPref _themePref=null;

		public FormUserSetting() {
			InitializeComponent();
			Lan.F(this);
		}
		private void FormUserSetting_Load(object sender,EventArgs e) {
			_suppressLogOffMessage=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.SuppressLogOffMessage).FirstOrDefault();
			if(_suppressLogOffMessage!=null) {//Does exist in the database
				checkSuppressMessage.Checked=true;
			}
			FillThemeCombo();
		}

		private void FillThemeCombo() {
			foreach(OdTheme theme in Enum.GetValues(typeof(OdTheme))) {
				if(theme==OdTheme.None) {
					continue;
				}
				comboTheme.Items.Add(theme);
			}
			_themePref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.UserTheme).FirstOrDefault();
			if(_themePref!=null) {//user has chosen a theme before. Display their currently chosen theme.
				comboTheme.SelectedIndex=comboTheme.Items.IndexOf((OdTheme)_themePref.Fkey);
			}
			else {//user has not chosen a theme before. Show them the current default.
				comboTheme.SelectedIndex=PrefC.GetInt(PrefName.ColorTheme);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(checkSuppressMessage.Checked && _suppressLogOffMessage==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.SuppressLogOffMessage
				});
			}
			else if(!checkSuppressMessage.Checked && _suppressLogOffMessage!=null) {
				UserOdPrefs.Delete(_suppressLogOffMessage.UserOdPrefNum);
			}
			if(_themePref==null) {
				_themePref=new UserOdPref() {UserNum=Security.CurUser.UserNum,FkeyType=UserOdFkeyType.UserTheme};
			}
			_themePref.Fkey=comboTheme.SelectedIndex;
			UserOdPrefs.Upsert(_themePref);
			if(PrefC.GetBool(PrefName.ThemeSetByUser)) {
				UserOdPrefs.SetThemeForUserIfNeeded();
			}
			else {
				//No need to return, just showing a warning so they know why the theme will not change.
				MsgBox.Show("Theme will not take effect until the miscellaneous preference has been set for users can set their own theme.");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}