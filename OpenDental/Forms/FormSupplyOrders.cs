using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Drawing.Printing;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormSupplyOrders:ODForm {
		private List<Supplier> _listSuppliers;
		private List<SupplyOrder> _listOrdersAll;
		private List<SupplyOrder> _listOrders;
		private DataTable _tableOrderItems;
		private int _pagesPrinted;
		private bool _headingPrinted;
		private int _headingPrintH;

		public FormSupplyOrders() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormSupplyOrders_Load(object sender,EventArgs e) {
			Height=SystemInformation.WorkingArea.Height;//max height
			Location=new Point(Location.X,0);//move to top of screen
			_listSuppliers = Suppliers.GetAll();
			_listOrdersAll = SupplyOrders.GetAll();
			_listOrders = new List<SupplyOrder>();
			FillComboSupplier();
			FillGridOrders();
			gridOrders.ScrollToEnd();
		}

		private void FillComboSupplier() {
			_listSuppliers=Suppliers.GetAll();
			comboSupplier.Items.Clear();
			comboSupplier.Items.Add(Lan.g(this,"All"));//add all to begining of list for composite listings.
			comboSupplier.SelectedIndex=0;
			for(int i=0;i<_listSuppliers.Count;i++) {
				comboSupplier.Items.Add(_listSuppliers[i].Name);
			}
		}

		private void comboSupplier_SelectedIndexChanged(object sender,EventArgs e) {
			FillGridOrders();
			gridOrders.ScrollToEnd();
			FillGridOrderItem();
		}

		private void gridOrder_CellClick(object sender,ODGridClickEventArgs e) {
			FillGridOrderItem();
		}

		private void gridOrder_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormSupplyOrderEdit FormSOE = new FormSupplyOrderEdit();
			FormSOE.ListSupplier = _listSuppliers;
			FormSOE.Order = _listOrders[e.Row];
			FormSOE.ShowDialog();
			if(FormSOE.DialogResult!=DialogResult.OK) {
				return;
			}
			_listOrdersAll = SupplyOrders.GetAll();
			FillGridOrders();
			FillGridOrderItem();
		}

		private void butAddSupply_Click(object sender,EventArgs e) {
			if(gridOrders.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a supply order to add items to first.");
				return;
			}
			FormSupplies FormSup = new FormSupplies();
			FormSup.IsSelectMode = true;
			FormSup.SelectedSupplierNum = _listOrders[gridOrders.GetSelectedIndex()].SupplierNum;
			FormSup.ShowDialog();
			if(FormSup.DialogResult!=DialogResult.OK) {
				return;
			}
			
			for(int i=0;i<FormSup.ListSelectedSupplies.Count;i++) {
				//check for existing----			
				if(_tableOrderItems.Rows.OfType<DataRow>().Any(x => PIn.Long(x["SupplyNum"].ToString())==FormSup.ListSelectedSupplies[i].SupplyNum)) {
					//MsgBox.Show(this,"Selected item already exists in currently selected order. Please edit quantity instead.");
					continue;
				}
				SupplyOrderItem orderitem = new SupplyOrderItem();
				orderitem.SupplyNum = FormSup.ListSelectedSupplies[i].SupplyNum;
				orderitem.Qty=1;
				orderitem.Price = FormSup.ListSelectedSupplies[i].Price;
				orderitem.SupplyOrderNum = _listOrders[gridOrders.GetSelectedIndex()].SupplyOrderNum;
				//soi.SupplyOrderItemNum
				SupplyOrderItems.Insert(orderitem);
			}
			UpdatePriceAndRefresh();
		}

		private void FillGridOrders() {
			FilterListOrder();
			gridOrders.BeginUpdate();
			gridOrders.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g(this,"Date Placed"),80);
			gridOrders.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Amount"),70,HorizontalAlignment.Right);
			gridOrders.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Shipping"),70,HorizontalAlignment.Right);
			gridOrders.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Supplier"),120);
			gridOrders.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Note"),200);
			gridOrders.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Placed By"),100);
			gridOrders.Columns.Add(col);
			gridOrders.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<_listOrders.Count;i++) {
				row=new ODGridRow();
				bool isPending=false;
				if(_listOrders[i].DatePlaced.Year>2200) {
					isPending=true;
				}
				if(isPending) {
					row.Cells.Add(Lan.g(this,"pending"));
				}
				else {
					row.Cells.Add(_listOrders[i].DatePlaced.ToShortDateString());
				}
				row.Cells.Add(_listOrders[i].AmountTotal.ToString("c"));
				row.Cells.Add(_listOrders[i].ShippingCharge.ToString("c"));
				row.Cells.Add(Suppliers.GetName(_listSuppliers,_listOrders[i].SupplierNum));
				row.Cells.Add(_listOrders[i].Note);
				if(isPending || _listOrders[i].UserNum==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Userods.GetName(_listOrders[i].UserNum));
				}
				row.Tag=_listOrders[i];
				gridOrders.Rows.Add(row);
			}
			gridOrders.EndUpdate();
		}

		private void FilterListOrder() {
			_listOrders.Clear();
			long supplier=0;
			if(comboSupplier.SelectedIndex < 1) {//this includes selecting All or not having anything selected.
				supplier = 0;
			}
			else {
				supplier=_listSuppliers[comboSupplier.SelectedIndex-1].SupplierNum;//SelectedIndex-1 because All is added before all the other items in the list.
			}
			foreach(SupplyOrder order in _listOrdersAll) {
				if(supplier==0) {//Either the ALL supplier is selected or no supplier is selected.
					_listOrders.Add(order);
				}
				else if(order.SupplierNum == supplier) {
					_listOrders.Add(order);
					continue;
				}
			}
		}

		private void FillGridOrderItem() {
			long orderNum=0;
			if(gridOrders.GetSelectedIndex()!=-1) {//an order is selected
				orderNum=_listOrders[gridOrders.GetSelectedIndex()].SupplyOrderNum;
			}
			_tableOrderItems=SupplyOrderItems.GetItemsForOrder(orderNum);
			gridItems.BeginUpdate();
			gridItems.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g(this,"Catalog #"),80);
			gridItems.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Description"),320);
			gridItems.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Qty"),60,HorizontalAlignment.Center);
			col.IsEditable=true;
			gridItems.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Price/Unit"),70,HorizontalAlignment.Right);
			col.IsEditable=true;
			gridItems.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Subtotal"),70,HorizontalAlignment.Right);
			gridItems.Columns.Add(col);
			gridItems.Rows.Clear();
			ODGridRow row;
			double price;
			int qty;
			double subtotal;
			for(int i=0;i<_tableOrderItems.Rows.Count;i++) {
				row=new ODGridRow();
				row.Cells.Add(_tableOrderItems.Rows[i]["CatalogNumber"].ToString());
				row.Cells.Add(_tableOrderItems.Rows[i]["Descript"].ToString());
				qty=PIn.Int(_tableOrderItems.Rows[i]["Qty"].ToString());
				row.Cells.Add(qty.ToString());
				price=PIn.Double(_tableOrderItems.Rows[i]["Price"].ToString());
				row.Cells.Add(price.ToString("n"));
				subtotal=((double)qty)*price;
				row.Cells.Add(subtotal.ToString("n"));
				gridItems.Rows.Add(row);
			}
			gridItems.EndUpdate();
		}

		private void gridOrderItem_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormSupplyOrderItemEdit FormSOIE = new FormSupplyOrderItemEdit();
			FormSOIE.ItemCur = SupplyOrderItems.CreateObject(PIn.Long(_tableOrderItems.Rows[e.Row]["SupplyOrderItemNum"].ToString()));
			FormSOIE.ListSupplier = Suppliers.GetAll();
			FormSOIE.ShowDialog();
			if(FormSOIE.DialogResult!=DialogResult.OK) {
				return;
			}
			SupplyOrderItems.Update(FormSOIE.ItemCur);
			UpdatePriceAndRefresh();
		}

		private void butNewOrder_Click(object sender,EventArgs e) {
			if(comboSupplier.SelectedIndex < 1) {//Includes no items or the ALL supplier being selected.
				MsgBox.Show(this,"Please select a supplier first.");
				return;
			}
			for(int i=0;i<_listOrders.Count;i++) {
				if(_listOrders[i].DatePlaced.Year>2200) {
					MsgBox.Show(this,"Not allowed to add a new order when there is already one pending.  Please finish the other order instead.");
					return;
				}
			}
			SupplyOrder order=new SupplyOrder();
			if(comboSupplier.SelectedIndex==0) {//Supplier "All".
				order.SupplierNum=0;
			}
			else {//Specific supplier selected.
				order.SupplierNum=_listSuppliers[comboSupplier.SelectedIndex-1].SupplierNum;//SelectedIndex-1 because "All" is first option.
			}
			order.IsNew=true;
			order.DatePlaced=new DateTime(2500,1,1);
			order.Note="";
			order.UserNum=0;//This will get set when the order is placed.
			SupplyOrders.Insert(order);
			_listOrdersAll=SupplyOrders.GetAll();//Refresh the list all.
			FillGridOrders();
			gridOrders.SetSelected(_listOrders.Count-1,true);
			gridOrders.ScrollToEnd();
			FillGridOrderItem();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(_tableOrderItems.Rows.Count<1) {
				MsgBox.Show(this,"Supply list is Empty.");
				return;
			}
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(
				pd2_PrintPage,
				Lan.g(this,"Supplies order from")+" "+_listOrders[gridOrders.GetSelectedIndex()].DatePlaced.ToShortDateString()+" "+Lan.g(this,"printed"),
				margins:new Margins(50,50,40,30)
			);
		}

		private void pd2_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			Font mainFont=new Font("Arial",9);
			int yPos=bounds.Top;
			#region printHeading
			//TODO: Decide what information goes in the heading.
			if(!_headingPrinted) {
				text=Lan.g(this,"Supply List");
				g.DrawString(text,headingFont,Brushes.Black,425-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=Lan.g(this,"Order Number")+": "+_listOrders[gridOrders.SelectedIndices[0]].SupplyOrderNum;
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				text=Lan.g(this,"Date")+": "+_listOrders[gridOrders.SelectedIndices[0]].DatePlaced.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				Supplier supCur=Suppliers.GetOne(_listOrders[gridOrders.SelectedIndices[0]].SupplierNum);
				text=supCur.Name;
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				text=supCur.Phone;
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				text=supCur.Note;
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				yPos+=15;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridItems.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void gridItems_CellLeave(object sender,ODGridClickEventArgs e) {
			//no need to check which cell was edited, just reprocess both cells
			int qtyNew=0;//default value.
			try {
				qtyNew=PIn.Int(gridItems.Rows[e.Row].Cells[2].Text);//0 if not valid input
			}
			catch { }
			double priceNew=PIn.Double(gridItems.Rows[e.Row].Cells[3].Text);//0 if not valid input
			SupplyOrderItem suppOI=SupplyOrderItems.CreateObject(PIn.Long(_tableOrderItems.Rows[e.Row]["SupplyOrderItemNum"].ToString()));
			suppOI.Qty=qtyNew;
			suppOI.Price=priceNew;
			SupplyOrderItems.Update(suppOI);
			SupplyOrders.UpdateOrderPrice(suppOI.SupplyOrderNum);
			gridItems.Rows[e.Row].Cells[2].Text=qtyNew.ToString();//to standardize formatting.  They probably didn't type .00
			gridItems.Rows[e.Row].Cells[3].Text=priceNew.ToString("n");//to standardize formatting.  They probably didn't type .00
			gridItems.Rows[e.Row].Cells[4].Text=(qtyNew*priceNew).ToString("n");//to standardize formatting.  They probably didn't type .00
			gridItems.Invalidate();
			int si=gridOrders.GetSelectedIndex();
			_listOrdersAll=SupplyOrders.GetAll();
			FillGridOrders();
			gridOrders.SetSelected(si,true);
		}

		private void UpdatePriceAndRefresh() {
			SupplyOrder gridSelect=gridOrders.SelectedTag<SupplyOrder>();
			SupplyOrders.UpdateOrderPrice(_listOrders[gridOrders.GetSelectedIndex()].SupplyOrderNum);
			_listOrdersAll=SupplyOrders.GetAll();
			FillGridOrders();
			for(int i=0;i<gridOrders.Rows.Count;i++) {
				if(gridSelect!=null && ((SupplyOrder)gridOrders.Rows[i].Tag).SupplyOrderNum==gridSelect.SupplyOrderNum) {
					gridOrders.SetSelected(i,true);
				}
			}
			FillGridOrderItem();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			//maybe rename to close, since most saving happens automatically.
			DialogResult=DialogResult.Cancel;
		}


	}
}