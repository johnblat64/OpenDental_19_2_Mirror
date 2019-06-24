using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWebChatTools:ODForm {

		public FormWebChatTools() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormWebChatTools_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGridWebChats(),
				checkShowEndedSessions,dateRangeWebChat,comboUsers,textChatTextContains,textSessionNum);
			dateRangeWebChat.SetDateTimeFrom(DateTimeOD.Today.AddDays(-7));
			dateRangeWebChat.SetDateTimeTo(DateTimeOD.Today.AddYears(1));
			comboUsers.Items.Clear();
			foreach(Userod user in Userods.GetUsers()) {
				comboUsers.Items.Add(new ODBoxItem<Userod>(user.UserName,user));
				if(Security.CurUser.UserNum==user.UserNum) {//Select the current user by default.
					comboUsers.SetSelected(comboUsers.Items.Count-1,true);
				}
			}
			FillGridWebChats();
		}

		public override void OnProcessSignals(List<Signalod> listSignals) {
			if(listSignals.Exists(x => x.IType==InvalidType.WebChatSessions)) {
				FillGridWebChats();
			}
		}

		///<summary>Sets all of the items in the user combobox to selected when clicked.</summary>
		private void butSelectAllUsers_Click(object sender,EventArgs e) {
			comboUsers.SetSelected(true);
			FillGridWebChats();
		}

		private void gridWebChatSessions_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			FormWebChatSession form=new FormWebChatSession((WebChatSession)gridWebChatSessions.Rows[e.Row].Tag,
				() => {
					FillGridWebChats();//Refresh to show if session ended or if tech has been assigned to the session.
				});
			form.Show();
		}

		///<summary>Only for ODHQ triage.</summary>
		private void FillGridWebChats() {
			gridWebChatSessions.BeginUpdate();
			gridWebChatSessions.Rows.Clear();
			if(gridWebChatSessions.Columns.Count==0) {
				gridWebChatSessions.Columns.Add(new ODGridColumn("DateTime",80,HorizontalAlignment.Center));
				gridWebChatSessions.Columns.Add(new ODGridColumn("IsEnded",60,HorizontalAlignment.Center));
				gridWebChatSessions.Columns.Add(new ODGridColumn("Owner",80,HorizontalAlignment.Left));
				gridWebChatSessions.Columns.Add(new ODGridColumn("PatNum",80,HorizontalAlignment.Right));
				gridWebChatSessions.Columns.Add(new ODGridColumn("SessionNum",90,HorizontalAlignment.Right));
				gridWebChatSessions.Columns.Add(new ODGridColumn("Question",0,HorizontalAlignment.Left));
			}
			List <WebChatSession> listChatSessions=null;
			List <WebChatMessage> listChatMessages=null;
			//If connection to webchat is lost or not visible from a specific network location, then continue, in order to keep the call center operational.
			ODException.SwallowAnyException(() => {
				listChatSessions=WebChatSessions.GetSessions(checkShowEndedSessions.Checked,dateRangeWebChat.GetDateTimeFrom(),dateRangeWebChat.GetDateTimeTo());
				listChatMessages=WebChatMessages.GetAllForSessions(listChatSessions.Select(x => x.WebChatSessionNum).ToArray());
			});
			if(listChatSessions!=null) {//Will only be null if connection to webchat database failed.
				List<Userod> listSelectedUsers=comboUsers.SelectedTags<Userod>();
				List<string> listSelectedUsernames=listSelectedUsers.Select(x => x.UserName).ToList();
				string searchText=textChatTextContains.Text.ToLower();
				foreach(WebChatSession webChatSession in listChatSessions) {
					bool isRelevantSession=false;
					if(string.IsNullOrEmpty(webChatSession.TechName)) {
						isRelevantSession=true;//Unclaimed web chat sessions are visible to all technicians, so they can consider taking ownership.
					}
					else if(listSelectedUsernames.Count==0) {
						isRelevantSession=true;//Filter for usernames is empty.  Show chat sessions for all users.
					}
					else if(listSelectedUsernames.Contains(webChatSession.TechName)) {
						isRelevantSession=true;
					}
					else if(listChatMessages.Exists(x => x.WebChatSessionNum==webChatSession.WebChatSessionNum && listSelectedUsernames.Contains(x.UserName))) {
						isRelevantSession=true;
					}
					if(!isRelevantSession) {
						continue;
					}
					List <string> listMessagesForSession=listChatMessages
						.Where(x => x.WebChatSessionNum==webChatSession.WebChatSessionNum)
						.Select(x => x.MessageText.ToLower())
						.ToList();
					if(!string.IsNullOrEmpty(textSessionNum.Text) && !webChatSession.WebChatSessionNum.ToString().Contains(textSessionNum.Text)) {
						continue;
					}
					if(!string.IsNullOrEmpty(searchText) && !webChatSession.QuestionText.ToLower().Contains(searchText)
						&& !listMessagesForSession.Exists(x => x.Contains(searchText)))
					{
						continue;
					}
					ODGridRow row=new ODGridRow();
					row.Tag=webChatSession;
					row.Cells.Add(webChatSession.DateTcreated.ToString());
					row.Cells.Add((webChatSession.DateTend.Year > 1880)?"X":"");
					if(string.IsNullOrEmpty(webChatSession.TechName)) {
						row.Cells.Add("NEEDS TECH");
						row.Bold=true;
						row.ColorBackG=Color.Red;
						row.ColorText=Color.White;
					}
					else {
						row.Cells.Add(webChatSession.TechName);
					}
					row.Cells.Add((webChatSession.PatNum==0)?"":webChatSession.PatNum.ToString());
					row.Cells.Add(webChatSession.WebChatSessionNum.ToString());
					row.Cells.Add(webChatSession.QuestionText);
					gridWebChatSessions.Rows.Add(row);
				}
			}
			gridWebChatSessions.EndUpdate();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		
	}
}