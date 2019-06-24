using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormJobTimeLog:ODForm {
		private Job _jobCur;

		public FormJobTimeLog(Job jobCur) {
			_jobCur=jobCur;
			InitializeComponent();
			Lan.F(this);
		}

		private void FormJobTimeLog_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			List<JobReview> listTime=new List<JobReview>();
			listTime.AddRange(_jobCur.ListJobTimeLogs);
			listTime.AddRange(_jobCur.ListJobReviews);
			listTime=listTime.OrderByDescending(x => x.DateTStamp).ToList();
			List<Userod> listUsers=Userods.GetAll();
			gridJobs.Columns.Add(new ODGridColumn("Date",75) { TextAlign=HorizontalAlignment.Center });
			gridJobs.Columns.Add(new ODGridColumn("User",75));
			gridJobs.Columns.Add(new ODGridColumn("Type",125) { TextAlign=HorizontalAlignment.Center });
			gridJobs.Columns.Add(new ODGridColumn("Time",75) { TextAlign=HorizontalAlignment.Center });
			gridJobs.Rows.Clear();
			foreach(JobReview review in listTime) {
				ODGridRow row=new ODGridRow() { Tag=review };
				row.Cells.Add(review.DateTStamp.ToShortDateString());
				row.Cells.Add(listUsers.FirstOrDefault(x => x.UserNum==review.ReviewerNum).UserName);
				row.Cells.Add(review.ReviewStatus.ToString());
				row.Cells.Add(Math.Round(review.Hours,2).ToString());
				gridJobs.Rows.Add(row);
			}
			gridJobs.EndUpdate();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}
	}
}
