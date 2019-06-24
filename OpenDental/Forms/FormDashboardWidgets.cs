using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDashboardWidgets:ODForm {
		///<summary>The dashboard that has been selected.</summary>
		public SheetDef SheetDefDashboardWidget;

		public FormDashboardWidgets(bool isCEMT=false) {
			InitializeComponent();
			Lan.F(this);
		}
		
		private void FormDashboard_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			List<SheetDef> listDashboardWidgets=SheetDefs.GetCustomForType(SheetTypeEnum.PatientDashboardWidget)
				.Where(x => Security.IsAuthorized(Permissions.DashboardWidget,x.SheetDefNum,true)).ToList();
			List<SheetDef> listSelectedDashboards=gridMain.SelectedTags<SheetDef>();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new ODGridColumn("Dashboard Name",0,HorizontalAlignment.Left));
			gridMain.Rows.Clear();
			foreach(SheetDef sheetDashboardWidget in listDashboardWidgets) {
				ODGridRow row=new ODGridRow();
				row.Cells.Add(sheetDashboardWidget.Description);
				row.Tag=sheetDashboardWidget;
				gridMain.Rows.Add(row);
			}
			gridMain.EndUpdate();
			for(int i=0;i<gridMain.Rows.Count;i++) {
				if(((SheetDef)gridMain.Rows[i].Tag).SheetDefNum.In(listSelectedDashboards.Select(x => x.SheetDefNum))) {
					gridMain.SetSelected(i,true);
				}
			}
		}
		
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SheetDefDashboardWidget=gridMain.SelectedTag<SheetDef>();
			DialogResult=DialogResult.OK;
		}

		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			FormDashboardWidgetSetup FormDS=new FormDashboardWidgetSetup();
			if(FormDS.ShowDialog()==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			SheetDefDashboardWidget=gridMain.SelectedTag<SheetDef>();
			DialogResult=DialogResult.OK;
		}

    private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}