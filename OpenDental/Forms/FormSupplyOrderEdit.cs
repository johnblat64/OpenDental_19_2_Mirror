using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormSupplyOrderEdit:ODForm {
		public SupplyOrder Order;
		public List<Supplier> ListSupplier;

		///<Summary>This form is only going to be used to edit existing supplyOrders, not to add new ones.</Summary>
		public FormSupplyOrderEdit() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormSupplyOrderEdit_Load(object sender,EventArgs e) {
			textSupplier.Text=Suppliers.GetName(ListSupplier,Order.SupplierNum);
			if(Order.DatePlaced.Year>2200){
				textDatePlaced.Text=DateTime.Today.ToShortDateString();
				Order.UserNum=Security.CurUser.UserNum;
			}
			else{
				textDatePlaced.Text=Order.DatePlaced.ToShortDateString();
			}
			textAmountTotal.Text=Order.AmountTotal.ToString("n");
			textShippingCharge.Text=Order.ShippingCharge.ToString("n");
			textNote.Text=Order.Note;
			comboUser.Items.Clear();
			ODBoxItem<Userod> listBoxItemUser=new ODBoxItem<Userod>(Lan.g(this,"None"),new Userod { UserNum=0 });
			comboUser.Items.Add(listBoxItemUser);
			List<Userod> listUsers=Userods.GetUsers().FindAll(x => !x.IsHidden);
			foreach(Userod user in listUsers) {
				ODBoxItem<Userod> listBoxItemUsers=new ODBoxItem<Userod>(user.UserName,user);
				comboUser.Items.Add(listBoxItemUsers);
				if(Order.UserNum==user.UserNum) {
					comboUser.SelectedItem=listBoxItemUsers;
				}
			}
			if(!listUsers.Select(x => x.UserNum).Contains(Order.UserNum)) {
				//Order was placed by a hidden user.
				comboUser.IndexSelectOrSetText(-1,() => { return Userods.GetName(Order.UserNum); });
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//if(Order.IsNew){//never
			//	DialogResult=DialogResult.Cancel;
			//}
			if(textDatePlaced.Text!=""){
				MsgBox.Show(this,"Not allowed to delete unless date is blank.");
				return;
			}
			if(!MsgBox.Show(this,true,"Delete entire order?")){
				return;
			}
			SupplyOrders.DeleteObject(Order);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDatePlaced.errorProvider1.GetError(textDatePlaced)!=""
				|| textAmountTotal.errorProvider1.GetError(textAmountTotal)!=""
				|| textShippingCharge.errorProvider1.GetError(textShippingCharge)!="")
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDatePlaced.Text==""){
				Order.DatePlaced=new DateTime(2500,1,1);
				Order.UserNum=0;//even if they had set a user, set it back because the order hasn't been placed. 
			}
			else{
				Order.DatePlaced=PIn.Date(textDatePlaced.Text);
				if(comboUser.SelectedIndex<=-1) {
					//if there was a hidden user on the order, do not change, keep it as it was. 
				}
				else {
					Order.UserNum=comboUser.SelectedTag<Userod>().UserNum;
				}
			}
			Order.AmountTotal=PIn.Double(textAmountTotal.Text);
			Order.Note=textNote.Text;
			Order.ShippingCharge=PIn.Double(textShippingCharge.Text);
			SupplyOrders.Update(Order);//never new
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}