using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormGridSelection:ODForm {
		
		///<summary></summary>
		private List<ODGridColumn> _listGridColumns;
		///<summary></summary>
		private List<ODGridRow> _listGridRows;
		///<summary>Set to the tag of the rows that are double selected.</summary>
		public List<object> ListSelectedTags=new List<object>();

		/// <summary>listGridRows must have tags set.</summary>
		public FormGridSelection(List<ODGridColumn> listGridColumns,List<ODGridRow> listGridRows,string formTitle,string gridTitle,GridSelectionMode selectMode=GridSelectionMode.One) {
			InitializeComponent();
			Lan.F(this);
			_listGridColumns=listGridColumns;
			_listGridRows=listGridRows;
			this.Text=formTitle;
			gridMain.Title=gridTitle;
			gridMain.SelectionMode=selectMode;
		}
		
		private void FormGridSelection_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			_listGridColumns.ForEach(x => gridMain.Columns.Add(x) );
			_listGridRows.ForEach(x => gridMain.Rows.Add(x) );
			gridMain.EndUpdate();
		}

		private void gridEras_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			ListSelectedTags=new List<object>(gridMain.SelectedTags<object>());
			this.DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MessageBox.Show(Lan.g(this.Text+" "+gridMain.Title,"No items are selected.  Please select an item before continuing."));
				return;
			}
			ListSelectedTags=new List<object>(gridMain.SelectedTags<object>());
			this.DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}