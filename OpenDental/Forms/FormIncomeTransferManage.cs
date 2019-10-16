using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormIncomeTransferManage:ODForm {
		///<summary>List of current paysplits for this payment.</summary>
		private List<PaySplit> _listSplitsCur;
		private Family _famCur;
		private Patient _patCur;
		///<summary>Payment from the Payment window, amount gets modified only in the case that the original paymentAmt is zero.  It gets increased to
		///whatever the paysplit total is when the window is closed.</summary>
		private Payment _paymentCur;
		private List<PaySplits.PaySplitAssociated> _listSplitsAssociated;
		private PaymentEdit.ConstructResults _results;
		///<summary>A dictionary or patients that we may need to reference to fill the grids to eliminate unnecessary calls to the DB.
		///Should contain all patients in the current family along with any patients of payment plans of which a member of this family is the guarantor.</summary>
		private Dictionary<long,Patient> _dictPatients;
		private bool _hasInvalidProcWithPayplan;
		///<summary>To be clear and make the distinction that these splits were created from the transfer unallocated logic and need to be handled in
		///a unique way to be able to determine the account entries linked to them that need to be modified.</summary>
		private List<PaySplit> _listPaySplitsCreatedFromUnearned=new List<PaySplit>();
		///<summary>Gets set when an unallocated transfer has been made automatically and inserted upon entering this window.</summary>
		private long _unallocatedPayNum=0;

		public FormIncomeTransferManage(Family famCur,Patient patCur,Payment payCur) {
			_famCur=famCur;
			_patCur=patCur;
			_paymentCur=payCur;
			InitializeComponent();
			Lan.F(this);
		}

		private void FormIncomeTransferManage_Load(object sender,EventArgs e) {
			if(Security.IsAuthorized(Permissions.PaymentCreate,true)) {
				TransferUnallocatedToUnearned();
			}
			Init(false);
		}

		///<summary></summary>
		private void TransferUnallocatedToUnearned() {
			_listSplitsCur=new List<PaySplit>();
			_listSplitsAssociated=new List<PaySplits.PaySplitAssociated>();
			List<PaySplit> listSplitsForPats=PaySplits.GetForPats(_famCur.ListPats.Select(x => x.PatNum).ToList());
			if(listSplitsForPats.IsNullOrEmpty()) {
				return;
			}
			//Pass in an invalid payNum of 0 which will get set correctly later if there are in fact splits to transfer.
			TransferUnallocatedSplitToUnearned(listSplitsForPats,0);
			if(_listSplitsCur.Count==0) {
				return;
			}
			if(!_listSplitsCur.Sum(x => x.SplitAmt).IsZero()) {
				//Display the UnearnedType and the SplitAmt for each split in the list.
				string splitInfo=string.Join("\r\n  ",_listSplitsCur
					.Select(x => $"SplitAmt: {x.SplitAmt}"));
				//Show the sum of all splits first and then give a breakdown of each individual split.
				string details=$"Sum of unallocatedTransfers.ListSplitsCur: {_listSplitsCur.Sum(x => x.SplitAmt)}\r\n"
					+$"Individual Split Info:\r\n  {splitInfo}";
				FriendlyException.Show("Error transferring unallocated paysplits.  Please call support.",new ApplicationException(details),"Close");
				//Close the window and do not let the user create transfers because something is wrong.
				DialogResult=DialogResult.Cancel;
				Close();
				return;
			}
			//There are unallocated paysplits that need to be transferred to unearned.
			_unallocatedPayNum=CreateAndInsertUnallocatedPayment();
			foreach(PaySplit split in _listSplitsCur) {
				split.PayNum=_unallocatedPayNum;//Set the PayNum because it was purposefully set to 0 above to save queries.
				PaySplits.Insert(split);
			}
			foreach(PaySplits.PaySplitAssociated splitAssociated in _listSplitsAssociated) {
				if(splitAssociated.PaySplitLinked!=null && splitAssociated.PaySplitOrig!=null) {
					PaySplits.UpdateFSplitNum(splitAssociated.PaySplitOrig.SplitNum,splitAssociated.PaySplitLinked.SplitNum);
				}
			}
			_listSplitsCur.Clear();//will get re-initialized in init, but just to be sure, clear here to remove what we just inserted.
			_listSplitsAssociated.Clear();
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,_patCur.PatNum
				,$"Unallocated splits automatically transferred to unearned for payment {_unallocatedPayNum}.");
		}
		
		///<summary>Method to encapsulate the creation of a new payment that is specifically meant to store payment information for the unallocated
		///payment transfer that can *sometimes* happen upon loading this window.</summary>
		private long CreateAndInsertUnallocatedPayment() {
			//user clicked ok and has permisson to save splits to the database. 
			Payment unallocatedTransferPayment=new Payment();
			unallocatedTransferPayment.PatNum=_patCur.PatNum;
			unallocatedTransferPayment.PayDate=DateTime.Today;
			unallocatedTransferPayment.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {//if clinics aren't enabled default to 0
				unallocatedTransferPayment.ClinicNum=Clinics.ClinicNum;
				if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
					unallocatedTransferPayment.ClinicNum=_patCur.ClinicNum;
				}
				else if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.SelectedExceptHQ) {
					unallocatedTransferPayment.ClinicNum=(Clinics.ClinicNum==0 ? _patCur.ClinicNum : Clinics.ClinicNum);
				}
			}
			unallocatedTransferPayment.DateEntry=DateTime.Today;
			unallocatedTransferPayment.PaymentSource=CreditCardSource.None;
			unallocatedTransferPayment.PayAmt=0;
			unallocatedTransferPayment.PayType=0;
			long payNum=Payments.Insert(unallocatedTransferPayment);
			return payNum;
		}

		///<summary>Performs all of the Load functionality.  Public so it can be called from unit tests.</summary>
		public void Init(bool isTest) {
			_listSplitsCur=new List<PaySplit>();
			_listSplitsAssociated=new List<PaySplits.PaySplitAssociated>();
			_dictPatients=Patients.GetAssociatedPatients(_patCur.PatNum).ToDictionary(x => x.PatNum,x => x);
			_results=PaymentEdit.ConstructAndLinkChargeCredits(_famCur.ListPats.Select(x => x.PatNum).ToList(),_patCur.PatNum,new List<PaySplit>(),
				_paymentCur,new List<AccountEntry>(),true,false);
			FillGridSplits();
		}

		///<summary>Fills the paysplit grid.</summary>
		private void FillGridSplits() {
			//Fill left grid with paysplits created
			gridSplits.BeginUpdate();
			gridSplits.Columns.Clear();
			gridSplits.Columns.Add(new ODGridColumn(Lan.g(this,"Date"),65,HorizontalAlignment.Center));
			gridSplits.Columns.Add(new ODGridColumn(Lan.g(this,"Prov"),40));
			if(PrefC.HasClinicsEnabled) {//Clinics
				gridSplits.Columns.Add(new ODGridColumn(Lan.g(this,"Clinic"),40));
			}
			gridSplits.Columns.Add(new ODGridColumn(Lan.g(this,"Patient"),100));
			gridSplits.Columns.Add(new ODGridColumn(Lan.g(this,"ProcCode"),60));
			gridSplits.Columns.Add(new ODGridColumn(Lan.g(this,"Type"),100));
			gridSplits.Columns.Add(new ODGridColumn(Lan.g(this,"Amount"),55,HorizontalAlignment.Right));
			gridSplits.Rows.Clear();
			ODGridRow row;
			decimal splitTotal=0;
			Dictionary<long,Procedure> dictProcs=Procedures.GetManyProc(_listSplitsCur.Where(x => x.ProcNum>0).Select(x => x.ProcNum).Distinct().ToList(),false).ToDictionary(x => x.ProcNum);
			for(int i=0;i<_listSplitsCur.Count;i++) {
				splitTotal+=(decimal)_listSplitsCur[i].SplitAmt;
				row=new ODGridRow();
				row.Tag=_listSplitsCur[i];
				row.Cells.Add(_listSplitsCur[i].DatePay.ToShortDateString());//Date
				row.Cells.Add(Providers.GetAbbr(_listSplitsCur[i].ProvNum));//Prov
				if(PrefC.HasClinicsEnabled) {//Clinics
					if(_listSplitsCur[i].ClinicNum!=0) {
						row.Cells.Add(Clinics.GetClinic(_listSplitsCur[i].ClinicNum).Description);//Clinic
					}
					else {
						row.Cells.Add("");//Clinic
					}
				}
				Patient patCur;
				if(!_dictPatients.TryGetValue(_listSplitsCur[i].PatNum,out patCur)) {
					patCur=Patients.GetPat(_listSplitsCur[i].PatNum);
				}
				row.Cells.Add(patCur.GetNameFL());//Patient
				Procedure proc=new Procedure();
				if(_listSplitsCur[i].ProcNum>0 && !dictProcs.TryGetValue(_listSplitsCur[i].ProcNum,out proc)) {
					proc=Procedures.GetOneProc(_listSplitsCur[i].ProcNum,false);
				}
				row.Cells.Add(ProcedureCodes.GetStringProcCode(proc?.CodeNum??0));//ProcCode
				string type="";
				if(_listSplitsCur[i].PayPlanNum!=0) {
					type+="PayPlanCharge";//Type
					if(_listSplitsCur[i].IsInterestSplit && _listSplitsCur[i].ProcNum==0 && _listSplitsCur[i].ProvNum!=0) {
						type+=" (interest)";
					}
				}
				if(_listSplitsCur[i].ProcNum!=0) {//Procedure
					string procDesc=Procedures.GetDescription(proc??new Procedure());
					if(type!="") {
						type+="\r\n";
					}
					type+="Proc: "+procDesc;//Type
				}
				if(_listSplitsCur[i].ProvNum==0) {//Unattached split
					if(type!="") {
						type+="\r\n";
					}
					type+="Unallocated";//Type
				}
				if(_listSplitsCur[i].ProvNum!=0 && _listSplitsCur[i].UnearnedType!=0) {
					if(type!="") {
						type+="\r\n";
					}
					type+=Defs.GetDef(DefCat.PaySplitUnearnedType,_listSplitsCur[i].UnearnedType).ItemName;
				}
				row.Cells.Add(type);
				if(row.Cells[row.Cells.Count-1].Text=="Unallocated") {
					row.Cells[row.Cells.Count-1].ColorText=Color.Red;
				}
				row.Cells.Add(_listSplitsCur[i].SplitAmt.ToString("f"));//Amount
				gridSplits.Rows.Add(row);
			}
			gridSplits.EndUpdate();
			textSplitTotal.Text=splitTotal.ToString("f");
			FillGridCharges();
		}

		///<summary>Fills charge grid, and then split grid.</summary>
		private void FillGridCharges() {
			//Fill right-hand grid with all the charges, filtered based on checkbox and filters.
			List<int> listExpandedRows=new List<int>();
			for(int i=0;i<gridCharges.Rows.Count;i++) {
				if(gridCharges.Rows[i].DropDownState==ODGridDropDownState.Down) {
					listExpandedRows.Add(i);//Keep track of expanded rows
				}
			}
			gridCharges.BeginUpdate();
			gridCharges.Columns.Clear();
			gridCharges.Columns.Add(new ODGridColumn(Lan.g(this,"Prov"),100));
			gridCharges.Columns.Add(new ODGridColumn(Lan.g(this,"Patient"),100));
			if(PrefC.HasClinicsEnabled) {
				gridCharges.Columns.Add(new ODGridColumn(Lan.g(this,"Clinic"),60));
			}
			gridCharges.Columns.Add(new ODGridColumn(Lan.g(this,"Codes"),-200));//negative so it will dynamically grow when no clinic column is present
			gridCharges.Columns.Add(new ODGridColumn(Lan.g(this,"Amt Orig"),61,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridCharges.Columns.Add(new ODGridColumn(Lan.g(this,"Amt Start"),61,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridCharges.Columns.Add(new ODGridColumn(Lan.g(this,"Amt End"),61,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridCharges.Rows.Clear();
			decimal chargeTotal=0;
			//Item1=ProvNum,Item2=ClinicNum,Item3=PatNum
			List<Tuple<long,long,long>> listAddedProvNums=new List<Tuple<long,long,long>>();//this needs to be prov/clinic/patnum
			List<Tuple<long,long,long>> listAddedParents=new List<Tuple<long,long,long>>();//prov/clinic/pat
			foreach(AccountEntry entryCharge in _results.ListAccountCharges) {
				if(Math.Round(entryCharge.AmountStart,3)==0) {
					continue;
				}
				if(listAddedProvNums.Any(x => x.Item1==entryCharge.ProvNum && x.Item2==entryCharge.ClinicNum && x.Item3==entryCharge.PatNum)) {
					continue;
				}
				listAddedProvNums.Add(Tuple.Create(entryCharge.ProvNum,entryCharge.ClinicNum,entryCharge.PatNum));
				List<AccountEntry> listEntriesForProvAndClinicAndPatient=_results.ListAccountCharges
					.FindAll(x => x.ProvNum==entryCharge.ProvNum && x.ClinicNum==entryCharge.ClinicNum && x.PatNum==entryCharge.PatNum);
				List<ODGridRow> listRows=CreateChargeRows(listEntriesForProvAndClinicAndPatient,listExpandedRows);
				foreach(ODGridRow row in listRows) {
					AccountEntry accountEntry=(AccountEntry)row.Tag;
					if(accountEntry.Tag==null) {//Parent row
						chargeTotal+=PIn.Decimal(row.Cells[row.Cells.Count-1].Text);
						listAddedParents.Add(Tuple.Create(accountEntry.ProvNum,accountEntry.ClinicNum,accountEntry.PatNum));
					}
					else if(!listAddedParents.Exists(x => x.Item1==accountEntry.ProvNum && x.Item2==accountEntry.ClinicNum && x.Item3==accountEntry.PatNum)) {//In case a parent AND child are selected, don't add child amounts if parent was added already
						chargeTotal+=accountEntry.AmountEnd;
					}
					gridCharges.Rows.Add(row);
				}
			}
			gridCharges.EndUpdate();
			textChargeTotal.Text=chargeTotal.ToString("f");
		}

		private List<ODGridRow> CreateChargeRows(List<AccountEntry> listEntries,List<int> listExpandedRows) {
			List<ODGridRow> listRows=new List<ODGridRow>();
			//Parent row
			ODGridRow row=new ODGridRow();
			//A placeholder accountEntry.  The Tag being null tells us it's a parent row.
			row.Tag=new AccountEntry() { ProvNum=listEntries[0].ProvNum,PatNum=listEntries[0].PatNum,ClinicNum=listEntries[0].ClinicNum };
			row.Cells.Add(Providers.GetAbbr(listEntries[0].ProvNum));//Provider
			Patient pat;
			if(!_dictPatients.TryGetValue(listEntries[0].PatNum,out pat)) {
				pat=Patients.GetLim(listEntries[0].PatNum);
				_dictPatients[pat.PatNum]=pat;
			}
			row.Cells.Add(pat.LName+", "+pat.FName);//Patient
			if(PrefC.HasClinicsEnabled) {
				row.Cells.Add(Clinics.GetAbbr(listEntries[0].ClinicNum));
			}
			string procCodes="";
			int addedCodes=0;
			for(int i=0;i<listEntries.Count;i++) {
				if(Math.Round(listEntries[i].AmountStart,3)==0) {
					continue;
				}
				if(listEntries[i].GetType()==typeof(Procedure)) {
					if(procCodes!="") {
						procCodes+=", ";
					}
					procCodes+=ProcedureCodes.GetStringProcCode(((Procedure)listEntries[i].Tag).CodeNum);
					addedCodes++;
				}
				else if(listEntries[i].GetType()==typeof(PaySplit) && ((PaySplit)listEntries[i].Tag).ProcNum!=0) {
					if(procCodes!="") {
						procCodes+=", ";
					}
					//Look for the procedure in AccountEntries (it should be there) - We don't want to go to the DB
					AccountEntry entry=_results.ListAccountCharges.Find(x => x.GetType()==typeof(Procedure) && x.PriKey==((PaySplit)listEntries[i].Tag).ProcNum);
					if(entry!=null) {//Can be null if paysplit is on a procedure that's outside the family.  
						procCodes+=ProcedureCodes.GetStringProcCode(((Procedure)entry.Tag).CodeNum);
						addedCodes++;
					}
				}
				else if(listEntries[i].GetType()==typeof(Adjustment) && ((Adjustment)listEntries[i].Tag).ProcNum!=0) {
					if(procCodes!="") {
						procCodes+=", ";
					}
					//Look for the procedure in AccountEntries (it should be there) - We don't want to go to the DB
					AccountEntry entry=_results.ListAccountCharges.Find(x => x.GetType()==typeof(Procedure) && x.PriKey==((Adjustment)listEntries[i].Tag).ProcNum);
					if(entry!=null) {//Can be null if paysplit is on a procedure that's outside the family.  
						procCodes+=ProcedureCodes.GetStringProcCode(((Procedure)entry.Tag).CodeNum);
						addedCodes++;
					}
				}
				if(addedCodes==9) {//1 less than above, this column is shorter when filtering by prov + clinic
					procCodes+=", (...)";
					break;
				}
			}
			row.Cells.Add(procCodes);
			//parent row should be sum of all child rows, and we skip entries that have an amount start of 0 for children so skip for parent as well. 
			List<AccountEntry> listEntriesWithAmountStart=listEntries.FindAll(x => !x.AmountStart.IsEqual(0));
			row.Cells.Add(listEntriesWithAmountStart.Sum(x => x.AmountOriginal).ToString("f"));//Amount Original
			row.Cells.Add(listEntriesWithAmountStart.Sum(x => x.AmountStart).ToString("f"));//Amount Start
			row.Cells.Add(listEntriesWithAmountStart.Sum(x => x.AmountEnd).ToString("f"));//Amount End
			row.Bold=true;
			listRows.Add(row);
			if(listExpandedRows.Contains(listRows.Count-1)) {
				row.DropDownState=ODGridDropDownState.Down;//If the expanded rows contains the one we just added, expand it
			}
			//Child rows
			foreach(AccountEntry entryCharge in listEntries) {
				if(Math.Round(entryCharge.AmountStart,3)==0) {
					continue;
				}
				ODGridRow childRow=new ODGridRow();
				childRow.Tag=entryCharge; 
				childRow.Cells.Add("     "+Providers.GetAbbr(entryCharge.ProvNum));//Provider
				childRow.Cells.Add(pat.LName+", "+pat.FName);//Patient
				if(PrefC.HasClinicsEnabled) {
					childRow.Cells.Add(Clinics.GetAbbr(entryCharge.ClinicNum));
				}
				procCodes="";
				if(entryCharge.GetType()==typeof(Procedure)) {
					procCodes=((Procedure)entryCharge.Tag).ProcDate.ToShortDateString()+" "+ProcedureCodes.GetStringProcCode(((Procedure)entryCharge.Tag).CodeNum);
				}
				else if(entryCharge.GetType()==typeof(PayPlanCharge)) {
					procCodes="PayPlanCharge";
				}
				else if(entryCharge.GetType()==typeof(PaySplit) && ((PaySplit)entryCharge.Tag).UnearnedType!=0) {
					PaySplit split=(PaySplit)entryCharge.Tag;
					if(split.ProvNum==0) {
						procCodes=entryCharge.Date.ToShortDateString()+" Unallocated";
					}
					else {
						procCodes=entryCharge.Date.ToShortDateString()+" "+Defs.GetName(DefCat.PaySplitUnearnedType,split.UnearnedType);
					}
				}
				else if(entryCharge.GetType()==typeof(PaySplit) && ((PaySplit)entryCharge.Tag).ProcNum!=0) {
					AccountEntry entry=_results.ListAccountCharges.Find(x => x.GetType()==typeof(Procedure) && x.PriKey==((PaySplit)entryCharge.Tag).ProcNum);
					if(entry!=null) {//Entry can be null if there is a split for X patient and the attached procedure is for Y patient outside family.  Possible with old data.
						procCodes=entryCharge.Date.ToShortDateString()+" PaySplit: "+ProcedureCodes.GetStringProcCode(((Procedure)entry.Tag).CodeNum);
					}
				}
				else if(entryCharge.GetType()==typeof(Adjustment) && ((Adjustment)entryCharge.Tag).ProcNum!=0) {
					AccountEntry entry=_results.ListAccountCharges.Find(x => x.GetType()==typeof(Procedure) && x.PriKey==((Adjustment)entryCharge.Tag).ProcNum);
					if(entry!=null) {//Entry can be null if there is a split for X patient and the attached procedure is for Y patient outside family.  Possible with old data.
						procCodes=entryCharge.Date.ToShortDateString()+" Adjustment: "+ProcedureCodes.GetStringProcCode(((Procedure)entry.Tag).CodeNum);
					}
				}
				else if(entryCharge.GetType()==typeof(PaySplit) && ((PaySplit)entryCharge.Tag).PayPlanNum!=0) {//Don't need this really
					procCodes=entryCharge.Date.ToShortDateString()+" PayPlan Split";
				}
				else if(entryCharge.GetType()==typeof(Adjustment)) {
					procCodes=entryCharge.Date.ToShortDateString()+" Adjustment";
				}
				else if(entryCharge.GetType()==typeof(ClaimProc)) {
					procCodes=entryCharge.Date.ToShortDateString()+" Claim Payment";
				}
				else {
					procCodes=entryCharge.Date.ToShortDateString()+" "+entryCharge.GetType().Name;
				}
				childRow.Cells.Add(procCodes);//ProcCodes
				childRow.Cells.Add(entryCharge.AmountOriginal.ToString("f"));//Amount Original
				childRow.Cells.Add(entryCharge.AmountStart.ToString("f"));//Amount Start
				childRow.Cells.Add(entryCharge.AmountEnd.ToString("f"));//Amount End
				childRow.DropDownParent=row;
				listRows.Add(childRow);
			}
			return listRows;
		}

		///<summary>Creates micro-allocations intelligently based on most to least matching criteria of selected charges.</summary>
		private void CreateTransfers(List<AccountEntry> listPosCharges,List<AccountEntry> listNegCharges,List<AccountEntry> listAccountEntries) {
			//No logic that manipulates these lists should happen before the regular transfer. If necessary (like fixing incorrect unearned)
			//they will need to be made as DBMs instead or wait for another logic overhaul. 
			CreatePayplanLoop(listPosCharges,listNegCharges,listAccountEntries);
			CreateTransferLoop(listPosCharges,listNegCharges,listAccountEntries);
		}

		/// <summary>Pre-transfer unearned so that we have correct data going into the logic for the regular transfers. This is an attempt to fix 
		/// incorrect unearned allocations (overallocations) that are allowed to exist within the db.</summary>
		public List<AccountEntry> CreateUnearnedLoop(List<AccountEntry> listPositiveCharges,long paymentNumCur,List<AccountEntry> listEntriesForPat) {
			List<AccountEntry> listPosUnearend=listPositiveCharges.FindAll(x => x.GetType()==typeof(PaySplit) && ((PaySplit)x.Tag).UnearnedType!=0);
			List<AccountEntry> listPosChargeAdditions=new List<AccountEntry>();
			foreach(AccountEntry charge in listPosUnearend) {
				if(charge.AmountOriginal < 0 && charge.AmountEnd > 0) {//started negative (as payments do) but now the payment 'owes' money i.e. overallocated
					listPosChargeAdditions.AddRange(TransferUnearnedHelper(charge,paymentNumCur,listEntriesForPat));
				}
			}	
			return listPosChargeAdditions;
		}

		private void CreatePayplanLoop(List<AccountEntry> listPosCharges,List<AccountEntry> listNegCharges,List<AccountEntry> listAccountEntries) {
			//0. Go through pos charges.  Find charges with payplannums and perform the below process with all things within the same payplan.
			//Find all unique payplan nums.
			Dictionary<long,List<AccountEntry>> dictPayPlanEntries=listPosCharges.GroupBy(x => (x.GetType()==typeof(PayPlanCharge) ? ((PayPlanCharge)x.Tag).PayPlanNum : x.GetType()==typeof(PaySplit) ? ((PaySplit)x.Tag).PayPlanNum : 0))
				.Where(x => x.Key!=0)
				.ToDictionary(x => x.Key,x => x.ToList());
			foreach(KeyValuePair<long,List<AccountEntry>> kvp in dictPayPlanEntries) {
				CreateTransferLoop(kvp.Value
					,listNegCharges.FindAll(x => (x.GetType()==typeof(PayPlanCharge) && ((PayPlanCharge)x.Tag).PayPlanNum==kvp.Key) || (x.GetType()==typeof(PaySplit) && ((PaySplit)x.Tag).PayPlanNum==kvp.Key))
					,listAccountEntries); 
			}
		}

		private void CreateTransferLoop(List<AccountEntry> listPosCharges,List<AccountEntry> listNegCharges,List<AccountEntry> listAccountEntries) {
			//Positive charges need to be paid off
			List<AccountEntry> listMatchingEntries=new List<AccountEntry>();
			bool hasInvalidSplits=false;
			//Eight separate loops going through the entire data set, each loop will have different requirements.
			//1. Prov/Pat/Clinic
			foreach(AccountEntry posCharge in listPosCharges) {
				if(posCharge.AmountEnd<=0) {
					continue;
				}
				foreach(AccountEntry negCharge in listNegCharges) {
					if(negCharge.AmountEnd>=0) {
						continue;
					}
					if(posCharge.ProvNum!=negCharge.ProvNum || posCharge.PatNum!=negCharge.PatNum || posCharge.ClinicNum!=negCharge.ClinicNum) {
						continue;
					}
					hasInvalidSplits|=CreateTransferHelper(posCharge,negCharge,listAccountEntries);
				}
			}
			//2. Prov/Pat
			foreach(AccountEntry posCharge in listPosCharges) {
				if(posCharge.AmountEnd<=0) {
					continue;
				}
				foreach(AccountEntry negCharge in listNegCharges) {
					if(negCharge.AmountEnd>=0) {
						continue;
					}
					if(posCharge.PatNum!=negCharge.PatNum || posCharge.ProvNum!=negCharge.ProvNum) {
						continue;
					}
					hasInvalidSplits|=CreateTransferHelper(posCharge,negCharge,listAccountEntries);
				}
			}
			//3. Prov/Clinic
			foreach(AccountEntry posCharge in listPosCharges) {
				if(posCharge.AmountEnd<=0) {
					continue;
				}
				foreach(AccountEntry negCharge in listNegCharges) {
					if(negCharge.AmountEnd>=0) {
						continue;
					}					
					if(posCharge.ProvNum!=negCharge.ProvNum || posCharge.ClinicNum!=negCharge.ClinicNum) {
						continue;
					}
					hasInvalidSplits|=CreateTransferHelper(posCharge,negCharge,listAccountEntries);
				}
			}
			//4. Pat/Clinic
			foreach(AccountEntry posCharge in listPosCharges) {
				if(posCharge.AmountEnd<=0) {
					continue;
				}
				foreach(AccountEntry negCharge in listNegCharges) {
					if(negCharge.AmountEnd>=0) {
						continue;
					}
					if(posCharge.PatNum!=negCharge.PatNum || posCharge.ClinicNum!=negCharge.ClinicNum) {
						continue;
					}
					hasInvalidSplits|=CreateTransferHelper(posCharge,negCharge,listAccountEntries);
				}
			}
			//5. Prov
			foreach(AccountEntry posCharge in listPosCharges) {
				if(posCharge.AmountEnd<=0) {
					continue;
				}
				foreach(AccountEntry negCharge in listNegCharges) {
					if(negCharge.AmountEnd>=0) {
						continue;
					}
					if(posCharge.ProvNum!=negCharge.ProvNum) {
						continue;
					}
					hasInvalidSplits|=CreateTransferHelper(posCharge,negCharge,listAccountEntries);
				}
			}
			//6. Pat
			foreach(AccountEntry posCharge in listPosCharges) {
				if(posCharge.AmountEnd<=0) {
					continue;
				}
				foreach(AccountEntry negCharge in listNegCharges) {
					if(negCharge.AmountEnd>=0) {
						continue;
					}
					if(posCharge.PatNum!=negCharge.PatNum) {
						continue;
					}
					hasInvalidSplits|=CreateTransferHelper(posCharge,negCharge,listAccountEntries);
				}
			}
			//7. Clinic
			foreach(AccountEntry posCharge in listPosCharges) {
				if(posCharge.AmountEnd<=0) {
					continue;
				}
				foreach(AccountEntry negCharge in listNegCharges) {
					if(negCharge.AmountEnd>=0) {
						continue;
					}
					if(posCharge.ClinicNum!=negCharge.ClinicNum) {
						continue;
					}
					hasInvalidSplits|=CreateTransferHelper(posCharge,negCharge,listAccountEntries);
				}
			}
			//8. Nothing
			foreach(AccountEntry posCharge in listPosCharges) {
				if(posCharge.AmountEnd<=0) {
					continue;
				}
				foreach(AccountEntry negCharge in listNegCharges) {
					if(negCharge.AmountEnd>=0) {
						continue;
					}
					hasInvalidSplits|=CreateTransferHelper(posCharge,negCharge,listAccountEntries);
				}
			}
			//9. Transfer all remaining excess to unearned (but keep the same pat/prov/clinic, unless they're on rigorous accounting in which case go to 0 prov)
			foreach(AccountEntry negCharge in listNegCharges) {
				if(negCharge.AmountEnd>=0 || !negCharge.GetType().In(typeof(PaySplit),typeof(Procedure),typeof(Adjustment))) {
					continue;
				}
				if(negCharge.GetType()==typeof(PaySplit) && ((PaySplit)negCharge.Tag).UnearnedType!=0) {//Only perform transfer with non-unearned splits.
					continue;
				}
				decimal amt=Math.Abs(negCharge.AmountEnd);
				PaySplit negSplit=new PaySplit();
				if(negCharge.GetType()==typeof(PaySplit)) {
					negSplit.ProcNum=((PaySplit)negCharge.Tag).ProcNum;
					negSplit.UnearnedType=((PaySplit)negCharge.Tag).UnearnedType;
					negSplit.FSplitNum=negCharge.PriKey;
				}
				else if(negCharge.GetType()==typeof(Procedure)) {
					if(((Procedure)negCharge.Tag).ProcFee < 0) {
						continue;//don't use money from negative procedures as a souce of income. 
					}
					negSplit.ProcNum=((Procedure)negCharge.Tag).ProcNum;
					negSplit.FSplitNum=0;
				}
				else if(negCharge.GetType()==typeof(Adjustment)) {
					negSplit.AdjNum=((Adjustment)negCharge.Tag).AdjNum;
					negSplit.FSplitNum=0;
				}
				//we may also need to handle claimspaybytotal and payplancharges in the future.
				negSplit.DatePay=DateTimeOD.Today;
				negSplit.ClinicNum=negCharge.ClinicNum;
				negSplit.PatNum=negCharge.PatNum;
				negSplit.PayPlanNum=0;
				negSplit.PayNum=_paymentCur.PayNum;
				negSplit.ProvNum=negCharge.ProvNum;
				negSplit.SplitAmt=0-(double)amt;
				PaySplit posSplit=new PaySplit();//the split that's going to unearned
				posSplit.DatePay=DateTimeOD.Today;
				posSplit.ClinicNum=negSplit.ClinicNum;
				posSplit.FSplitNum=0;
				posSplit.PatNum=negSplit.PatNum;
				posSplit.PayPlanNum=0;
				posSplit.PayNum=_paymentCur.PayNum;
				posSplit.ProcNum=0;
				posSplit.ProvNum=PrefC.GetBool(PrefName.AllowPrepayProvider) ? negSplit.ProvNum : 0 ;
				posSplit.AdjNum=0;
				posSplit.SplitAmt=(double)amt;
				posSplit.UnearnedType=negSplit.UnearnedType==0 ? PrefC.GetLong(PrefName.PrepaymentUnearnedType) : negSplit.UnearnedType;
				_listSplitsCur.AddRange(new[] { posSplit,negSplit });
				_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated(negSplit,posSplit));
				negCharge.SplitCollection.Add(negSplit);
				negCharge.AmountEnd+=amt;
			}
			if(hasInvalidSplits) {
				MsgBox.Show(this,"Due to Rigorous Accounting, one or more invalid transactions have been cancelled.  Please fix those manually.");
			}
			else if(_hasInvalidProcWithPayplan) {
				MsgBox.Show(this,"One or more over allocated paysplit was not able to be reversed.");
			}
		}

		///<summary>Creates and links paysplits with micro-allocations based on the charges passed in.  
		///Constructs the paysplits with all necessary information depending on the type of the charges.
		///Returns true if attempting to create invalid splits and rigorous accounting is enabled.</summary>
		private bool CreateTransferHelper(AccountEntry posCharge,AccountEntry negCharge,List<AccountEntry> listAccountEntriesForPat) {
			if(negCharge.GetType()==typeof(Procedure) && ((Procedure)negCharge.Tag).ProcFee < 0) {
				return false;//do not use negative procedures as sources of income. 
			}
			decimal amt=Math.Min(Math.Abs(posCharge.AmountEnd),Math.Abs(negCharge.AmountEnd));
			if(posCharge.GetType()==typeof(PayPlanCharge)) {//Allocate to payplancharges in a special way
				TransferPayPlanChargeHelper(posCharge,negCharge,listAccountEntriesForPat,amt);
			}
			else {
				PaySplit negSplit=new PaySplit();
				PaySplit posSplit=new PaySplit();
				if(posCharge.GetType()==typeof(PaySplit)) {
					//it's a negative paysplit - Likely used for a refund or something.
					//We need to use it as a source and transfer it to overpaid procedures or credit adjustments only.
					if(negCharge.GetType()==typeof(Adjustment) || negCharge.GetType()==typeof(Procedure)) {
						//Since the source of the money is a positive charge (negative split) we have to reverse the posSplit and negSplit associations.
						//In this particular case it is neg source -> pos split -> neg split -> destination proc/adj
						negSplit=new PaySplit();
						negSplit.DatePay=DateTimeOD.Today;
						negSplit.ClinicNum=negCharge.ClinicNum;
						negSplit.FSplitNum=0;//Can't set FSplitNum yet, the positive isn't inserted yet. 
						negSplit.PatNum=negCharge.PatNum;
						negSplit.PayPlanNum=0;
						negSplit.PayNum=_paymentCur.PayNum;
						negSplit.ProcNum=negCharge.GetType()==typeof(Procedure) ? negCharge.PriKey : 0;//Refund is headed to a procedure.
						negSplit.ProvNum=negCharge.ProvNum;
						negSplit.AdjNum=negCharge.GetType()==typeof(Adjustment) ? negCharge.PriKey : 0;//Refund is headed to an adjustment.
						negSplit.SplitAmt=0-(double)amt;
						negSplit.UnearnedType=0;
						posSplit=new PaySplit();
						posSplit.DatePay=DateTimeOD.Today;
						posSplit.ClinicNum=posCharge.ClinicNum;
						posSplit.FSplitNum=posCharge.PriKey;//FSplitNum is the refund splitnum.
						if(posCharge.PriKey==0) {
							//hasn't been inserted into the database yet, so make split association for it instead.
							_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated((PaySplit)posCharge.Tag,posSplit));
						}
						posSplit.PatNum=posCharge.PatNum;
						posSplit.PayPlanNum=0;
						posSplit.PayNum=_paymentCur.PayNum;
						posSplit.ProcNum=0;
						posSplit.ProvNum=posCharge.ProvNum;
						posSplit.AdjNum=0;
						posSplit.SplitAmt=(double)amt;
						posSplit.UnearnedType=((PaySplit)posCharge.Tag).UnearnedType;
						_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated(posSplit,negSplit));
						_listSplitsCur.AddRange(new[] { negSplit,posSplit });
						negCharge.SplitCollection.Add(negSplit);
						posCharge.SplitCollection.Add(posSplit);
						posCharge.AmountEnd-=amt;
						negCharge.AmountEnd+=amt;
					}
					return false;
				}
				else{
					posSplit=new PaySplit();
					posSplit.DatePay=DateTimeOD.Today;
					posSplit.ClinicNum=posCharge.ClinicNum;
					posSplit.FSplitNum=0;//Can't set FSplitNum just yet, the neg split isn't inserted so there is no FK yet.
					posSplit.PatNum=posCharge.PatNum;
					posSplit.PayPlanNum=0;
					posSplit.PayNum=_paymentCur.PayNum;
					posSplit.ProcNum=posCharge.GetType()==typeof(Procedure) ? posCharge.PriKey : posCharge.GetType()==typeof(PaySplit) ? ((PaySplit)posCharge.Tag).ProcNum : posCharge.GetType()==typeof(Adjustment) ? ((Adjustment)posCharge.Tag).ProcNum : posCharge.GetType()==typeof(ClaimProc) ? ((ClaimProc)posCharge.Tag).ProcNum : 0;//PosCharge may or may not be a proc.  If it is proc, use ProcNum.  If it is a paysplit, use the paysplit's procnum.
					posSplit.ProvNum=posCharge.ProvNum;
					posSplit.AdjNum=posCharge.GetType()==typeof(Adjustment) ? (((Adjustment)posCharge.Tag).ProcNum==0 ? ((Adjustment)posCharge.Tag).AdjNum : 0) : 0;
					posSplit.SplitAmt=(double)amt;
					//Unearned type will likely be 0, but if we make a positive split to ProvNum=0, use default unearned type.
					//explicit because of old adjustments that didn't have to have a provider.
					if(posSplit.ProvNum==0 && posSplit.AdjNum==0 && posSplit.ProcNum==0) {
						posSplit.UnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);
					}
					else if(posCharge.GetType()==typeof(PaySplit)) {
						posSplit.UnearnedType=((PaySplit)posCharge.Tag).UnearnedType;
					}
					else {
						posSplit.UnearnedType=0;
					}
					negSplit=new PaySplit();
					negSplit.DatePay=DateTimeOD.Today;
					negSplit.ClinicNum=negCharge.ClinicNum;
					negSplit.FSplitNum=0;//Money may be coming from a paysplit 
					if(negCharge.GetType()==typeof(PaySplit)) {
						negSplit.FSplitNum=negCharge.PriKey;
						if(negCharge.PriKey==0) {
							//money is coming from a split that we created earlier, and hasn't been inserted into the database yet. Make a split association for it.
							_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated((PaySplit)negCharge.Tag,negSplit));
						}
					}
					negSplit.PatNum=negCharge.PatNum;
					negSplit.PayPlanNum=0;
					negSplit.PayNum=_paymentCur.PayNum;
					negSplit.ProcNum=negCharge.GetType()==typeof(PaySplit) ? ((PaySplit)negCharge.Tag).ProcNum : negCharge.GetType()==typeof(Procedure) ? negCharge.PriKey : negCharge.GetType()==typeof(Adjustment) ? ((Adjustment)negCharge.Tag).ProcNum : negCharge.GetType()==typeof(ClaimProc) ? ((ClaimProc)negCharge.Tag).ProcNum : 0;//Money may be coming from an overpaid procedure instead of paysplit.
					negSplit.ProvNum=negCharge.ProvNum;
					negSplit.AdjNum=negCharge.GetType()==typeof(Adjustment) ? (((Adjustment)negCharge.Tag).ProcNum==0 ? ((Adjustment)negCharge.Tag).AdjNum : 0) : 0;
					negSplit.SplitAmt=0-(double)amt;
					if(negCharge.GetType()==typeof(PaySplit)) {//If money is coming from paysplit, use its unearned type (if any)
						negSplit.UnearnedType=((PaySplit)negCharge.Tag).UnearnedType;
					}
					else {
						negSplit.UnearnedType=0;
					}
					if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
						//split being created needs to have a procedure with a provider, or an adjustment with a provider 
						//or unearned type (optionally w/ provider) BUT if unearned, cannot have a procedure or adjustment. 
						if((Math.Sign(posSplit.ProcNum)!=Math.Sign(posSplit.ProvNum) && Math.Sign(posSplit.AdjNum)!=Math.Sign(posSplit.ProvNum))
							|| (posSplit.UnearnedType==0 && posSplit.ProvNum==0)
							|| (negSplit.UnearnedType==0 && negSplit.ProvNum==0))
						{
							return true;
						}
					}
					if(!PrefC.GetBool(PrefName.AllowPrepayProvider)) {//this pref used to only be available for enforce fully, but now is for all types.
						if((posSplit.UnearnedType!=0 && posSplit.ProvNum!=0) || (negSplit.UnearnedType!=0 && negSplit.ProvNum!=0)) {
							return true;
						}
					}
					_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated(negSplit,posSplit));
					_listSplitsCur.AddRange(new List<PaySplit>() {posSplit, negSplit});
					negCharge.SplitCollection.Add(negSplit);
					posCharge.SplitCollection.Add(posSplit);
					posCharge.AmountEnd-=amt;
					negCharge.AmountEnd+=amt;
				}
			}
			return false;
		}

		private void TransferPayPlanChargeHelper(AccountEntry posCharge,AccountEntry negCharge,List<AccountEntry> listAccountEntriesForPat,decimal amt) {
			negCharge.AmountEnd+=amt;
			PaySplit negSplit=new PaySplit();
			negSplit.DatePay=DateTimeOD.Today;
			negSplit.ClinicNum=negCharge.ClinicNum;
			negSplit.FSplitNum=0;
			if(negCharge.GetType()==typeof(PaySplit)) {
				negSplit.FSplitNum=negCharge.PriKey;
				if(negCharge.PriKey==0) {//hasn't been inserted into the database yet.
					_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated((PaySplit)negCharge.Tag,negSplit));
				}
			}
			negSplit.PatNum=negCharge.PatNum;
			negSplit.PayPlanNum=negCharge.GetType()==typeof(PaySplit) ? ((PaySplit)negCharge.Tag).PayPlanNum : 0;
			negSplit.PayNum=_paymentCur.PayNum;
			if(negCharge.GetType()==typeof(PaySplit)) {
				negSplit.ProcNum=((PaySplit)negCharge.Tag).ProcNum;
			}
			else if(negCharge.GetType()==typeof(Adjustment)) {
				negSplit.ProcNum=((Adjustment)negCharge.Tag).ProcNum;
			}
			else if(negCharge.GetType()==typeof(ClaimProc)) {
				negSplit.ProcNum=((ClaimProc)negCharge.Tag).ProcNum;
			}
			else {
				negSplit.ProcNum=0;
			}
			negSplit.ProvNum=negCharge.ProvNum;
			negSplit.AdjNum=negCharge.GetType()==typeof(Adjustment) ? negCharge.PriKey : 0;//Money may be coming from an adjustment.
			negSplit.SplitAmt=0-(double)amt;
			//If money is coming from paysplit, use its unearned type (if any)
			negSplit.UnearnedType=negCharge.GetType()==typeof(PaySplit) ? ((PaySplit)negCharge.Tag).UnearnedType : 0;
			negCharge.SplitCollection.Add(negSplit);
			decimal amount;
			List<PayPlanCharge> listCredits=_results.ListPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Credit 
				&& x.PayPlanNum==((PayPlanCharge)posCharge.Tag).PayPlanNum);
			List<PaySplit> listSplitsForPayPlan=PaySplits.CreateSplitForPayPlan(_paymentCur.PayNum,(double)amt,posCharge,listCredits
				,listAccountEntriesForPat,amt,false,out amount);
			if(listSplitsForPayPlan.Count>0) {
				foreach(PaySplit split in listSplitsForPayPlan) {
					split.FSplitNum=0;
					split.PayNum=_paymentCur.PayNum;
					posCharge.SplitCollection.Add(split);
					_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated(negSplit,split));
					_listSplitsCur.Add(split);
				}
				_listSplitsCur.Add(negSplit);
			}
		}

		/// <summary> This is prepayment allocation reversal. Because of this, items may need to be moved back to the list of positive charges. 
		/// The returned list will be any charges that need to be added back to the list of positive charges. </summary>
		private List<AccountEntry> TransferUnearnedHelper(AccountEntry positiveCharge,long paymentNumCur,List<AccountEntry> listEntriesForPat) {
			//Special case where unearned has been over allocated. 
			//Negative charge (split) masquarading around as a positve split becasuse it was overallocated. 
			_hasInvalidProcWithPayplan=false;
			List<AccountEntry> listPositiveChargeAdditions=new List<AccountEntry>();
			decimal amountOverallocated=positiveCharge.AmountEnd;
			PaySplit negSplit=new PaySplit();
			PaySplit posSplit=new PaySplit();
			//get what is attached to this original pre-payment
			List<PaySplit> listAttachedToOverallocated=PaySplits.GetSplitsForPrepay(new List<PaySplit>{((PaySplit)positiveCharge.Tag)})
				.OrderByDescending(x => x.DatePay)
				.ToList();//verify this order is correct. We want the most recent first. 
			List<PaySplit> listAttachedElseWhere=PaySplits.GetAllocatedElseWhere(positiveCharge.PriKey);
			foreach(PaySplit offset in listAttachedToOverallocated) {
				if(amountOverallocated.IsEqual(0)) {
					break;
				}
				//attempt to find the most recent split that was allocated from the offset
				List<PaySplit> listAttachedtoOffset=listAttachedElseWhere.FindAll(x => x.FSplitNum==offset.SplitNum)
					.OrderByDescending(x => x.DatePay)
					.ToList();
				AccountEntry productionEntry=null;
				foreach(PaySplit allocated in listAttachedtoOffset) {
					if(amountOverallocated.IsEqual(0)) {
						break;
					}
					if(allocated.ProcNum!=0 && allocated.PayPlanNum==0) {
						//find charge from list to money can be correctly taken away from the production that previously had been attached to overallocation
						productionEntry=listEntriesForPat.FirstOrDefault(x => x.PriKey==allocated.ProcNum);
					}
					else if(allocated.AdjNum!=0) {
						productionEntry=listEntriesForPat.FirstOrDefault(x => x.PriKey==allocated.AdjNum);
					}
					else {
						//if it was not previously explicitly attached to a procedure (that was not on a payment plan) or a positive adjustment, we do
						//not want to handle it at this point in time. 
						_hasInvalidProcWithPayplan=true;
						return listPositiveChargeAdditions;
					}
					//reverse the split up to the amount overallocated
					decimal reverseAmt=Math.Min(amountOverallocated,(decimal)allocated.SplitAmt);
					//make a positive split to put back to unallocated. Reverse offset. 
					posSplit.DatePay=DateTimeOD.Today;
					posSplit.ClinicNum=allocated.ClinicNum;
					posSplit.FSplitNum=offset.FSplitNum;//Split should be directly attached to the original prepayment positivecharge.PriKey
					posSplit.PatNum=allocated.PatNum;
					posSplit.PayPlanNum=0;
					posSplit.PayNum=paymentNumCur;
					posSplit.ProcNum=0;
					posSplit.ProvNum=positiveCharge.ProvNum;
					posSplit.AdjNum=0;
					posSplit.SplitAmt=(double)reverseAmt;
					posSplit.UnearnedType=offset.UnearnedType;
					//make a negative split to take away from this allocated split (likely taking away from a procedure or other production) Reverse allocation
					negSplit.DatePay=DateTimeOD.Today;
					negSplit.ClinicNum=positiveCharge.ClinicNum;
					negSplit.FSplitNum=0;//Split should be directly attached to the reverse offset we just made. Will be 0 for now since it has not yet been inserted.
					negSplit.PatNum=allocated.PatNum;
					negSplit.PayPlanNum=0;
					negSplit.PayNum=paymentNumCur;
					negSplit.ProcNum=allocated.ProcNum;
					negSplit.ProvNum=allocated.ProvNum;
					negSplit.AdjNum=allocated.AdjNum;
					negSplit.SplitAmt=-(double)reverseAmt;
					negSplit.UnearnedType=0;
					amountOverallocated-=reverseAmt;
					positiveCharge.AmountEnd-=reverseAmt;
					_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated(posSplit,negSplit));
					_listSplitsCur.AddRange(new[] { posSplit,negSplit });
					_listPaySplitsCreatedFromUnearned.Add(posSplit);
					_listPaySplitsCreatedFromUnearned.Add(negSplit);
					positiveCharge.SplitCollection.Add(posSplit);
					if(productionEntry!=null) {
						productionEntry.AmountEnd+=reverseAmt;
						listPositiveChargeAdditions.Add(productionEntry);
					}
				}
			}
			return listPositiveChargeAdditions;
		}

		///<summary>Takes unallocated money (splits that do not have procNum,adjNum,fSplitNum,payplanNum, or unearned type) and moves them to unearned 
		///through a transfer that balances to 0. 
		///In detail, loop through all splits. We are treating each split on the account as a parent split. 
		///If the split has children we will look at the childrens values to see if they equate to the parent's value.
		///If the children equate to the parent, we do not need to do anything. We will look at the children splits again when they come up as a 'parent'
		///If The chidren do not equate to the parent, an additional transfer need to be made to unearned to make them equate
		///If the split does not have children, we will look to see if it needs to be transferred to unearned (if it's unallocated) or not. </summary>
		private List<AccountEntry> TransferUnallocatedSplitToUnearned(List<PaySplit> listSplitsAll,long payNum) {
			List<AccountEntry> listModifiedEntries=new List<AccountEntry>();//to hold new entries for what we just created
			//TODO future job: Eliminate the need for query passed in. Make it optional to have account entries to implicit/explicit linking.
			//List<PaySplit> listSplitsAll=listAccountCharges.FindAll(x => x.GetType()==typeof(PaySplit)).Select(x => (PaySplit)x.Tag).ToList();							
			foreach(PaySplit parentSplit in listSplitsAll) {
				List<PaySplit> listChildrenSplits=listSplitsAll.FindAll(x => x.FSplitNum==parentSplit.SplitNum && x.FSplitNum!=0);
				if(listChildrenSplits.IsNullOrEmpty()) {
					//no children, just evaluate if parent is unallocated.
					if(parentSplit.ProcNum!=0 || parentSplit.AdjNum!=0 || parentSplit.PayPlanNum!=0 || parentSplit.UnearnedType!=0) {
						continue;//split has been allocated, move along.
					}
					listModifiedEntries.AddRange(TransferUnallocatedSplitHelper(parentSplit,payNum));//split is unallocated, transfer to unearned. 
				}
				else {
					//children exist for this parent split, evaluate their values in comparison.
					double sumChildren=listChildrenSplits.Sum(x => x.SplitAmt);
					double sumParentChildren=parentSplit.SplitAmt + sumChildren;
					if(sumParentChildren.IsZero() || 
						(parentSplit.SplitAmt.IsGreaterThan(0) && parentSplit.UnearnedType!=0 && Math.Abs(sumChildren)<parentSplit.SplitAmt)) 
					{
						continue;//either the children equate, or this is an unearned split that hasn't been fully allocated yet and that is fine.
					}
					if(Math.Abs(parentSplit.SplitAmt) > Math.Abs(sumChildren)) {
						listModifiedEntries.AddRange(TransferUnallocatedSplitHelper(parentSplit,payNum,sumParentChildren));//under allocated parent
					}
					else {
						listModifiedEntries.AddRange(TransferUnallocatedSplitHelper(parentSplit,payNum,sumParentChildren*-1));//over allocated parent
					}
				}
			}
			return listModifiedEntries;
		}

		private  List<AccountEntry> TransferUnallocatedSplitHelper(PaySplit splitToTransfer,long payNum,double splitAmtOverride=0) {
			List<AccountEntry> listModifiedEntries=new List<AccountEntry>();//to hold an return new entries for what we just created.
			//make an offsetting split that is the opposite sign as the split to transfer. 
			PaySplit offsetSplit=new PaySplit();
			offsetSplit.DatePay=DateTime.Today;
			offsetSplit.ClinicNum=splitToTransfer.ClinicNum;
			offsetSplit.FSplitNum=splitToTransfer.SplitNum;
			if(splitToTransfer.SplitNum==0) {
				_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated(splitToTransfer,offsetSplit));
			}
			offsetSplit.PatNum=splitToTransfer.PatNum;
			offsetSplit.PayPlanNum=splitToTransfer.PayPlanNum;//should usually be 0. Only not when production had been overallocated.
			offsetSplit.PayNum=payNum;
			offsetSplit.ProcNum=splitToTransfer.ProcNum;//should usually be 0
			offsetSplit.ProvNum=splitToTransfer.ProvNum;
			offsetSplit.AdjNum=splitToTransfer.AdjNum;//should ususally be 0
			offsetSplit.SplitAmt=splitAmtOverride==0?(splitToTransfer.SplitAmt*-1):(splitAmtOverride*-1);//should be for the opposite sign as the passed in split.
			offsetSplit.UnearnedType=splitToTransfer.UnearnedType;//this should stay in the case when we're correct unbalanced unearned specifically.
			PaySplit unearnedSplit=new PaySplit();
			unearnedSplit.DatePay=DateTime.Today;
			unearnedSplit.ClinicNum=splitToTransfer.ClinicNum;
			unearnedSplit.FSplitNum=0;//0 because offset split hasn't been inserted into the database yet. Make split associated for this.
			unearnedSplit.PatNum=splitToTransfer.PatNum;
			unearnedSplit.PayPlanNum=0;
			unearnedSplit.PayNum=payNum;
			unearnedSplit.ProvNum=0;
			if(PrefC.GetBool(PrefName.AllowPrepayProvider)==true) {
				unearnedSplit.ProvNum=splitToTransfer.ProvNum;
			}
			unearnedSplit.AdjNum=0;
			unearnedSplit.SplitAmt=offsetSplit.SplitAmt*-1;//should be the opposite sign as the offset split.
			unearnedSplit.UnearnedType=offsetSplit.UnearnedType==0?PrefC.GetLong(PrefName.PrepaymentUnearnedType):offsetSplit.UnearnedType;
			_listSplitsCur.AddRange(new List<PaySplit> {offsetSplit,unearnedSplit});
			_listPaySplitsCreatedFromUnearned.AddRange(new List<PaySplit> {offsetSplit,unearnedSplit});
			_listSplitsAssociated.Add(new PaySplits.PaySplitAssociated(offsetSplit,unearnedSplit));
			//make account entries for these new items, they may need to be used in the regular transfers down the line.
			AccountEntry offsetEntry=new AccountEntry(offsetSplit);
			offsetEntry.AmountStart=0;
			offsetEntry.AmountEnd=0;
			listModifiedEntries.Add(offsetEntry);
			listModifiedEntries.Add(new AccountEntry(unearnedSplit));
			return listModifiedEntries;
		}

		private void AdjustAccountEntryAmtEnds(List<AccountEntry> listAccountEntries,List<PaySplit> listSplits) {
			foreach(AccountEntry charge in listAccountEntries){
				if(charge.GetType()!=typeof(PaySplit)) {
					continue;
				}
				List<long> listFKeys=listSplits.FindAll(x => x.FSplitNum!=0).Select(x => x.FSplitNum).ToList();
				if(charge.PriKey.In(listFKeys)) {//has a child in the list of splits we created from balancing
					List<PaySplit> listSplitsForCharge=listSplits.FindAll(x => x.FSplitNum==charge.PriKey);//find all the children of this charge.
					charge.AmountEnd-=(decimal)listSplitsForCharge.Sum(x => x.SplitAmt);//modify parent since the created children took away value.
					charge.SplitCollection.AddRange(listSplitsForCharge);
				}
			}
		}

		///<summary>Deletes selected paysplits from the grid and attributes amounts back to where they originated from.
		///This will return a list of payment plan charges that were affected. This is so that splits can be correctly re-attributed to the payplancharge
		///when the user edits the paysplit. There should only ever be one payplancharge in that list, since the user can only edit one split at a time.</summary>
		private List<long> DeleteSelected() {
			//we need to return the payplancharge that the paysplit was associated to so that this paysplit can be correctly re-attributed to that charge.
			List<long> listPayPlanChargeNum=new List<long>();
			List<PaySplit> listSplits=gridSplits.SelectedIndices.Select(x => (PaySplit)gridSplits.Rows[x].Tag).ToList();
			foreach(PaySplit paySplit in listSplits) {
				//Find split associations so we can remove both when one is deleted
				PaySplits.PaySplitAssociated associated=_listSplitsAssociated.Find(x => x.PaySplitLinked==paySplit || x.PaySplitOrig==paySplit);
				for(int j = 0;j<_results.ListAccountCharges.Count;j++) {
					AccountEntry charge=_results.ListAccountCharges[j];
					decimal chargeAmtNew=charge.AmountEnd;
					if(charge.SplitCollection.Contains(associated.PaySplitLinked)) {
						chargeAmtNew+=(decimal)associated.PaySplitLinked.SplitAmt;
					}
					else if(charge.SplitCollection.Contains(associated.PaySplitOrig)) {
						chargeAmtNew+=(decimal)associated.PaySplitOrig.SplitAmt;
					}
					else {
						continue;
					}
					if(Math.Abs(chargeAmtNew)>Math.Abs(charge.AmountStart)) {//Trying to delete an overpayment, just increase charge's amount to the max.
						charge.AmountEnd=charge.AmountStart;
					}
					else {
						charge.AmountEnd=chargeAmtNew;//Give the money back to the charge so it will display.
					}
					charge.SplitCollection.Remove(paySplit);
				}
				_listSplitsCur.Remove(associated.PaySplitLinked);
				_listSplitsCur.Remove(associated.PaySplitOrig);
			}
			_listSplitsAssociated.RemoveAll(x => listSplits.Any(y => y==x.PaySplitLinked || y==x.PaySplitOrig));
			FillGridSplits();
			return listPayPlanChargeNum;
		}

		///<summary>When a paysplit is selected this method highlights all charges associated with it.</summary>
		private void gridSplits_CellClick(object sender,ODGridClickEventArgs e) {
			gridCharges.SetSelected(false);
			foreach(PaySplit paySplit in gridSplits.SelectedIndices.Select(x => (PaySplit)gridSplits.Rows[x].Tag)) {
				for(int j=0;j<gridCharges.RowsFiltered.Count;j++) {
					if(((AccountEntry)gridCharges.RowsFiltered[j].Tag)?.SplitCollection?.Contains(paySplit)??false) {
						gridCharges.SetSelected(j,true);
						break;
					}
				}
			}
		}

		///<summary>When a charge is selected this method highlights all paysplits associated with it.</summary>
		private void gridCharges_CellClick(object sender,ODGridClickEventArgs e) {
			//Charge total text set here
			List<Tuple<long,long,long>> listAddedParents=new List<Tuple<long,long,long>>();//Prov/Clinic/Pat
			gridSplits.SetSelected(false);
			decimal chargeTotal=0;
			foreach(ODGridRow row in gridCharges.SelectedGridRows) {
				AccountEntry accountEntry=(AccountEntry)row.Tag;
				if(accountEntry.Tag==null) {//Parent row
					chargeTotal+=PIn.Decimal(row.Cells[row.Cells.Count-1].Text);
					listAddedParents.Add(Tuple.Create(accountEntry.ProvNum,accountEntry.ClinicNum,accountEntry.PatNum));
					continue;
				}
				else {
					for(int j=0;j<gridSplits.Rows.Count;j++) {
						PaySplit paySplit=(PaySplit)gridSplits.Rows[j].Tag;
						if(accountEntry.SplitCollection.Contains(paySplit)) {
							gridSplits.SetSelected(j,true);
						}
						if(accountEntry.GetType()==typeof(PayPlanCharge) && paySplit.PayPlanNum==((PayPlanCharge)accountEntry.Tag).PayPlanNum) {
							gridSplits.SetSelected(j,true);
						}
					}
					if(!listAddedParents.Exists(x => x.Item1==accountEntry.ProvNum && x.Item2==accountEntry.ClinicNum && x.Item3==accountEntry.PatNum)) {//In case a parent AND child are selected, don't add child amounts if parent was added already
						chargeTotal+=accountEntry.AmountEnd;
					}
				}
			}
			textChargeTotal.Text=chargeTotal.ToString("F");
		}

		///<summary>Creates paysplits for selected charges if there is enough payment left.</summary>
		private void butTransfer_Click(object sender,EventArgs e) {	
			if(gridCharges.SelectedGridRows.Count==0) {//Nothing selected, transfer everything.
				gridCharges.SetSelected(true);
			}
			//Make list of positive charges
			List<AccountEntry> listPosCharges=gridCharges.SelectedGridRows.Select(x => (AccountEntry)x.Tag).Where(x => x.AmountEnd>0).ToList();
			//Make list of negative charges
			List<AccountEntry> listNegCharges=gridCharges.SelectedGridRows.Select(x => (AccountEntry)x.Tag).Where(x => x.AmountEnd<0).ToList();
			//We need to detect if parent rows are selected (row.DropDownParent==null).   If it is, we need to find all child rows of that row
			//We need to add the child rows into the list of charges even if they aren't explicitly selected
			foreach(ODGridRow row in gridCharges.SelectedGridRows) {
				if(row.DropDownParent!=null) {
					continue;
				}
				AccountEntry parentEntry=(AccountEntry)row.Tag;
				//The user has a parent row selected - Make sure all child rows are added to appropriate lists (if they aren't in there already)
				foreach(ODGridRow row2 in gridCharges.Rows) {
					AccountEntry childEntry=(AccountEntry)row2.Tag;
					if(parentEntry.AccountEntryNum==childEntry.AccountEntryNum) {
						continue;//Don't add parent row into anything.
					}
					if(!listPosCharges.Contains(childEntry) && !listNegCharges.Contains(childEntry) && childEntry.ClinicNum==parentEntry.ClinicNum 
						&& childEntry.PatNum==parentEntry.PatNum && childEntry.ProvNum==parentEntry.ProvNum && childEntry.AmountEnd!=0)
					{
						if(childEntry.AmountEnd>0) {
							listPosCharges.Add(childEntry);
						}
						else {
							listNegCharges.Add(childEntry);
						}
					}
				}
			}
			List<long> listPatNumsForCharges=listPosCharges.Select(x => x.PatNum).Distinct().ToList();
			//Get account entries so if any procs are attached to payment plans, their attached procedures can be attached to the new split from the transfer. 
			List<AccountEntry> listEntriesForPats=_results.ListAccountCharges.FindAll(x => x.PatNum.In(listPatNumsForCharges));
			listPosCharges=listPosCharges.OrderBy(x => x.Date).ToList();
			listNegCharges=listNegCharges.OrderBy(x => x.Date).ToList();
			CreateTransfers(listPosCharges,listNegCharges,listEntriesForPats);
			FillGridSplits();//Fills charge grid too.
		}

		///<summary>Deletes all paysplits.</summary>
		private void butClearAll_Click(object sender,EventArgs e) {
			gridSplits.SetSelected(true);
			DeleteSelected();
		}

		///<summary>Deletes selected paysplits.</summary>
		private void butDeleteSplit_Click(object sender,EventArgs e) {
			DeleteSelected();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PaymentCreate)) {
				return;
			}
			double splitTotal=_listSplitsCur.Select(x => x.SplitAmt).Sum();
			if(splitTotal != 0) { //income transfer
				MsgBox.Show(this,"Income transfers must have a split total of 0.");
				return;
			}
			_listSplitsCur.RemoveAll(x => x.SplitAmt==0);//We don't want any zero splits.  They were there just for display purposes.
			foreach(PaySplit split in _listSplitsCur) {
				PaySplits.Insert(split);
			}
			foreach(PaySplits.PaySplitAssociated split in _listSplitsAssociated) {
				//Update the FSplitNum after inserts are made. 
				if(split.PaySplitLinked!=null && split.PaySplitOrig!=null) {
					PaySplits.UpdateFSplitNum(split.PaySplitOrig.SplitNum,split.PaySplitLinked.SplitNum);
				}
			}
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,_paymentCur.PatNum,Patients.GetLim(_paymentCur.PatNum).GetNameLF()
				+", "+_paymentCur.PayAmt.ToString("c"));
			string strErrorMsg=Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(_patCur.PatNum,_listSplitsCur);
			if(!string.IsNullOrEmpty(strErrorMsg)) {
				MessageBox.Show(strErrorMsg);
			}
			DialogResult=DialogResult.OK;
		}

		private void FormIncomeTransferManage_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult!=DialogResult.OK && _unallocatedPayNum!=0) {
				//user is canceling out of the window and an unallocated transfer was made.
				ODException.SwallowAnyException(() => {
					Payments.Delete(_unallocatedPayNum);
					SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,_patCur.PatNum,$"Automatic transfer deleted for payNum: {_unallocatedPayNum}.");
				});
			}
		}
	}
}