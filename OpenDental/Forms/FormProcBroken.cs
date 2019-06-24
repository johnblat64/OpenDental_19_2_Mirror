using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProcBroken:ODForm {
		public bool IsNew;
		private Procedure _procCur;
		private Procedure _procOld;
		///<summary>Cached list of clinics available to user. Also includes a dummy Clinic at index 0 for "none".</summary>
		private List<Clinic> _listClinics;
		///<summary>Filtered list of providers based on which clinic is selected. If no clinic is selected displays all providers. Also includes a dummy clinic at index 0 for "none"</summary>
		private List<Provider> _listProviders;
		///<summary>Used to keep track of the current clinic selected. This is because it may be a clinic that is not in _listClinics.</summary>
		private long _selectedClinicNum;
		///<summary>Instead of relying on _listProviders[comboProv.SelectedIndex] to determine the selected Provider we use this variable to store it explicitly.</summary>
		private long _selectedProvNum;
		private bool _isNonRefundable;

		public bool IsProcDeleted { get; private set; }

		public double AmountTotal {
			get {
				return _procCur.ProcFee;
			}
		}

		public FormProcBroken(Procedure proc,bool isNonRefundable) {
			_procCur=proc;
			_procOld=proc.Copy();
			_isNonRefundable=isNonRefundable;
			InitializeComponent();
			Lan.F(this);
		}

		private void FormProcBroken_Load(object sender,EventArgs e) {
			textDateEntry.Text=_procCur.DateEntryC.ToShortDateString();
			textProcDate.Text=_procCur.ProcDate.ToShortDateString();
			textAmount.Text=_procCur.ProcFee.ToString("f");
			_selectedProvNum=_procCur.ProvNum;
			comboProv.SelectedIndex=-1;//initializes to 0; must be -1 for fillComboProv
			if(PrefC.HasClinicsEnabled) {
				_listClinics=new List<Clinic>() { new Clinic() { Abbr=Lan.g(this,"None") } }; //Seed with "None"
				Clinics.GetForUserod(Security.CurUser).ForEach(x => _listClinics.Add(x));//do not re-organize from cache. They could either be alphabetizeded or sorted by item order.
				_listClinics.ForEach(x => comboClinic.Items.Add(x.Abbr));
				_selectedClinicNum=_procCur.ClinicNum;
				comboClinic.IndexSelectOrSetText(_listClinics.FindIndex(x => x.ClinicNum==_selectedClinicNum),() => { return Clinics.GetAbbr(_selectedClinicNum); });
			}
			else {
				labelClinic.Visible=false;
				comboClinic.Visible=false;
				fillComboProv();
			}
			textUser.Text=Userods.GetName(_procCur.UserNum);
			textChartNotes.Text=_procCur.Note;
			textAccountNotes.Text=_procCur.BillingNote;
			labelAmountDescription.Text="";
			if(_isNonRefundable) {
				labelAmountDescription.Text=AmountTotal.ToString("c")+": Non-Refundable portion";
			}
		}

		private void comboClinic_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboClinic.SelectedIndex>-1) {
				_selectedClinicNum=_listClinics[comboClinic.SelectedIndex].ClinicNum;
			}
			fillComboProv();
		}

		private void comboProv_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboProv.SelectedIndex>-1) {
				_selectedProvNum=_listProviders[comboProv.SelectedIndex].ProvNum;
			}
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			FormProviderPick FormP = new FormProviderPick(_listProviders);
			FormP.SelectedProvNum=_selectedProvNum;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;
			}
			_selectedProvNum=FormP.SelectedProvNum;
			comboProv.IndexSelectOrSetText(_listProviders.FindIndex(x => x.ProvNum==_selectedProvNum),() => { return Providers.GetLongDesc(_selectedProvNum); });
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void fillComboProv() {
			if(comboProv.SelectedIndex>-1) {//valid prov selected, non none or nothing.
				_selectedProvNum = _listProviders[comboProv.SelectedIndex].ProvNum;
			}
			_listProviders=Providers.GetProvsForClinic(_selectedClinicNum);
			//Fill comboProv
			comboProv.Items.Clear();
			_listProviders.ForEach(x => comboProv.Items.Add(x.Abbr));
			comboProv.IndexSelectOrSetText(_listProviders.FindIndex(x => x.ProvNum==_selectedProvNum),() => { return Providers.GetLongDesc(_selectedProvNum); });
		}

		private void butAutoNoteChart_Click(object sender,EventArgs e) {
			FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				textChartNotes.AppendText(FormA.CompletedNote);
			}
		}

		private void butAutoNoteAccount_Click(object sender,EventArgs e) {
			FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				textAccountNotes.AppendText(FormA.CompletedNote);
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(textProcDate.errorProvider1.GetError(textProcDate)!=""
				|| textAmount.errorProvider1.GetError(textAmount)!="")
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textProcDate.Text=="") {
				MsgBox.Show(this,"Please enter a date first.");
				return;
			}
			if(_procCur.ProcStatus==ProcStat.C && PIn.Date(textProcDate.Text).Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Completed procedures cannot be set for future dates.");
				return;
			}
			if(textAmount.Text=="") {
				MsgBox.Show(this,"Please enter an amount.");
				return;
			}
			if(_selectedProvNum!=_procOld.ProvNum && PrefC.GetBool(PrefName.ProcProvChangesClaimProcWithClaim)) {
				List<ClaimProc> listClaimProc=ClaimProcs.GetForProc(ClaimProcs.Refresh(_procOld.PatNum),_procOld.ProcNum);
				if(listClaimProc.Any(x => x.Status==ClaimProcStatus.Received
					|| x.Status==ClaimProcStatus.Supplemental
					|| x.Status==ClaimProcStatus.CapClaim)) 
				{
					MsgBox.Show(this,"The provider cannot be changed when this procedure is attached to a claim.");
					return;
				}
			}
			_procCur.ProcDate=PIn.Date(textProcDate.Text);
			_procCur.ProcFee=PIn.Double(textAmount.Text);
			_procCur.Note=textChartNotes.Text;
			_procCur.BillingNote=textAccountNotes.Text;
			_procCur.ProvNum=_selectedProvNum;
			_procCur.ClinicNum=_selectedClinicNum;
			Procedures.Update(_procCur,_procOld);
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(_procCur.CodeNum);
			string logText=procedureCode.ProcCode+" ("+_procCur.ProcStatus+"), "+Lan.g(this,"Fee")+": "+_procCur.ProcFee.ToString("c")+", "+procedureCode.Descript;
			SecurityLogs.MakeLogEntry(IsNew ? Permissions.ProcComplCreate : Permissions.ProcComplEdit,_procCur.PatNum,logText);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormProcBroken_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.Cancel && IsNew) {
				if(Patients.GetPat(_procCur.PatNum).DiscountPlanNum!=0) {
					Adjustments.DeleteForProcedure(_procCur.ProcNum);//Delete discount plan adjustment
				}
				Procedures.Delete(_procCur.ProcNum);
				IsProcDeleted=true;
			}
		}
	}
}