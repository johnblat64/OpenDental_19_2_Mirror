﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness {
	public class PaymentEdit {

		///<summary>Gets most all the data needed to load FormPayment.</summary>
		public static LoadData GetLoadData(Patient patCur,Payment paymentCur,List<long> listPatNumsFamily,bool isNew,bool isIncomeTxfr) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<LoadData>(MethodBase.GetCurrentMethod(),patCur,paymentCur,listPatNumsFamily,isNew,isIncomeTxfr);
			}
			LoadData data=new LoadData();
			data.SuperFam=new Family(Patients.GetBySuperFamily(patCur.SuperFamily));
			data.ListCreditCards=CreditCards.Refresh(patCur.PatNum);
			data.XWebResponse=XWebResponses.GetOneByPaymentNum(paymentCur.PayNum);
			data.TableBalances=Patients.GetPaymentStartingBalances(patCur.Guarantor,paymentCur.PayNum);
			data.TableBalances.TableName="TableBalances";
			data.ListSplits=PaySplits.GetForPayment(paymentCur.PayNum);
			data.ListPaySplitAllocations=PaySplits.GetSplitsForPrepay(data.ListSplits);
			if(!isNew) {
				data.Transaction=Transactions.GetAttachedToPayment(paymentCur.PayNum);
			}
			data.ListValidPayPlans=PayPlans.GetValidPlansNoIns(patCur.PatNum);
			//If this payment contains splits for other superfamily members, add the superfamily members to listPatNumsFamily (which we use later to generate charges)
			if(data.SuperFam.ListPats.Length>1) {//Always has current patient.
				foreach(PaySplit split in data.ListSplits) {
					if(!listPatNumsFamily.Contains(split.PatNum) && data.SuperFam.ListPats.Select(x => x.PatNum).Contains(split.PatNum)) {
						listPatNumsFamily.AddRange(data.SuperFam.ListPats.Select(x => x.PatNum));
						break;//This means that it is a historical payment and has splits for family members that are outside the current family.  Add the family members, no need to continue
					}
				}
			}
			//if there were no payment plans found where this patient is the guarantor, find payment plans in the family.
			if(data.ListValidPayPlans.Count == 0) {
				//Do not include insurance payment plans.
				data.ListValidPayPlans=PayPlans.GetForPats(listPatNumsFamily,patCur.PatNum)
					.Where(x => !x.IsClosed)
					.Where(x => x.PlanNum==0).ToList();
			}
			data.ListAssociatedPatients=Patients.GetAssociatedPatients(patCur.PatNum);
			data.ListPrePaysForPayment=PaySplits.GetSplitsLinked(data.ListSplits);
			data.ListProcsForSplits=Procedures.GetManyProc(data.ListSplits.Select(x => x.ProcNum).ToList(),false);
			data.ConstructChargesData=GetConstructChargesData(listPatNumsFamily,patCur.PatNum,data.ListSplits,paymentCur.PayNum,isIncomeTxfr);
			return data;
		}

		/// <summary>Performs load functionality. Fills dictPatients with PatNums and Patients of patients that are associated to patCurNum.
		///Gets all patients that are in the same family or super family as patCurNum and that patCurNum is the payplan guarantor of.
		///Pass in load data to keep current objects instead of refreshing.</summary>
		public static InitData Init(List<Patient> listPats,Family fam,Family superFam,Payment payCur,List<PaySplit> listSplitsCur
			,List<AccountEntry> listEntriesLoading,long patCurNum,Dictionary<long,Patient> dictPatients=null,bool isIncomeTxfr=false,bool isPatPrefer=false
			,LoadData loadData=null,bool doAutoSplit=true) 
		{
			//No remoting role check; no call to db
			InitData initData=new InitData();
			initData.DictPats=dictPatients;
			//get patients who have this patient's guarantor as their payplan's guarantor
			List<Patient> listPatients=listPats;
			listPatients.AddRange(fam.ListPats);
			if(superFam.ListPats!=null) {
				listPatients.AddRange(superFam.ListPats);
			}
			//Add patients with paysplits on this payment
			listPatients.AddRange(Patients.GetLimForPats(listSplitsCur.Select(x => x.PatNum).Where(x => listPatients.All(y => y.PatNum!=x)).ToList()));
			if(initData.DictPats==null) {
				initData.DictPats=new Dictionary<long,Patient>();
			}
			listPatients=listPatients.DistinctBy(x => x.PatNum).ToList();
			foreach(Patient pat in listPatients) {
				initData.DictPats[pat.PatNum]=pat;
			}
			//This logic will ensure that regardless of if it's a new, or old payment any created paysplits that haven't been saved, 
			//such as if splits were made in this window then the window was closed and then reopened, will persist.
			initData.SplitTotal=0;
			for(int i=0;i<listSplitsCur.Count;i++) {
				initData.SplitTotal+=(decimal)listSplitsCur[i].SplitAmt;
			}
			payCur.PayAmt=Math.Round(payCur.PayAmt-(double)initData.SplitTotal,3);
			initData.AutoSplitData=AutoSplitForPayment(listPatients.Select(x => x.PatNum).ToList(),patCurNum,listSplitsCur,payCur,listEntriesLoading
				,isIncomeTxfr,isPatPrefer,loadData,doAutoSplit);
			listSplitsCur.AddRange(initData.AutoSplitData.ListAutoSplits);
			initData.AutoSplitData.ListSplitsCur=listSplitsCur;//handle getting the full list here for init instead of getting the auto splits and adding splits.
			if(listPatients.Select(x => x.PatNum).ToList().Any(x => !initData.DictPats.ContainsKey(x))) {
				Patients.GetLimForPats(listPatients.Select(x => x.PatNum).ToList()
					.FindAll(x => !initData.DictPats.ContainsKey(x)))
					.ForEach(x => initData.DictPats[x.PatNum]=x);
			}
			return initData;
		}

		#region constructing and linking charges and credits
		///<summary>Gets the data needed to construct a list of charges on FormPayment.</summary>
		public static ConstructChargesData GetConstructChargesData(List<long> listPatNums,long patNum,List<PaySplit> listSplitsCur,long payCurNum
			,bool isIncomeTransfer) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ConstructChargesData>(MethodBase.GetCurrentMethod(),listPatNums,patNum,listSplitsCur,payCurNum,isIncomeTransfer);
			}
			ConstructChargesData data=new ConstructChargesData();
			data.ListAdjustments=Adjustments.GetAdjustForPats(listPatNums);
			data.ListPaySplits=PaySplits.GetForPats(listPatNums);//Might contain payplan payments.
			//will also contain TP procs if pref is set to ON
			data.ListProcsCompleted=Procedures.GetCompleteForPats(listPatNums);
			if(isIncomeTransfer) {
				//do not show pre-payment splits that are attached to TP procedures, they should not be transferred since they are already reserved money
				//only TP unearned should have an unearned type and a procNum. 
				data.ListPaySplits.RemoveAll(x => x.UnearnedType!=0 && x.ProcNum!=0 && !x.ProcNum.In(data.ListProcsCompleted.Select(y => y.ProcNum)));
			}
			if(PrefC.IsPrePayAllowedForTpProcs && !isIncomeTransfer) {
				data.ListProcsCompleted.AddRange(Procedures.GetTpForPats(listPatNums));
			}
			//Look for splits that are in the database yet have been deleted from the pay splits grid.
			for(int i=data.ListPaySplits.Count-1;i>=0;i--) {
				bool isFound=listSplitsCur.Any(x => x.SplitNum==data.ListPaySplits[i].SplitNum);
				if(!isFound && data.ListPaySplits[i].PayNum==payCurNum) {
					//If we have a split that's not found in the passed-in list of splits for the payment
					//and the split we got from the DB is on this payment, remove it because the user must have deleted the split from the payment window.
					//The payment window won't update the DB with the change until it's closed.
					data.ListPaySplits.RemoveAt(i);
				}
			}
			if(!isIncomeTransfer) {
				//sum ins pay as totals by claims to include the value for the ones that have already been transferred and taken care of. 
				data.ListInsPayAsTotal=ClaimProcs.GetOutstandingClaimPayByTotal(listPatNums);
			}
			data.ListPayPlans=PayPlans.GetForPats(listPatNums,patNum);//Used to figure out how much we need to pay off procs with, also contains ins payplans
			data.ListPayPlanCharges=new List<PayPlanCharge>();
			data.ListPaySplitsPayPlan=new List<PaySplit>();
			if(data.ListPayPlans.Count>0) {
				//get list where payplan guar is not in the fam)
				data.ListPaySplitsPayPlan=PaySplits.GetForPayPlans(data.ListPayPlans.Select(x => x.PayPlanNum).ToList());					
				data.ListPayPlanCharges=PayPlanCharges.GetDueForPayPlans(data.ListPayPlans,listPatNums);//Does not get charges for the future.
				data.ListPaySplits.AddRange(data.ListPaySplitsPayPlan
					.Where(x => !data.ListPaySplits.Any(y => y.SplitNum==x.SplitNum)).ToList());
			}
			data.ListClaimProcsFiltered=ClaimProcs.Refresh(listPatNums).Where(x => x.ProcNum!=0).ToList();//does not contain PayAsTotals
			return data;
		}

		/// <summary>Helper method that does the entire original auto split for payment. Gets the charges, and links the credits.</summary>
		public static ConstructResults ConstructAndLinkChargeCredits(List<long> listPatNums,long patCurNum,List<PaySplit> listSplitsCur,Payment payCur
			,List<AccountEntry> listEntriesLoading,bool isIncomeTxfr=false,bool isPreferCurPat=false,LoadData loadData=null) 
		{
			//No remoting role check; no call to db
			ConstructResults retVal=new ConstructResults();
			retVal.Payment=payCur;
			#region Get data
			//Get the lists of items we'll be using to calculate with.
			ConstructChargesData constructChargesData=loadData?.ConstructChargesData
				??GetConstructChargesData(listPatNums,patCurNum,listSplitsCur,retVal.Payment.PayNum,isIncomeTxfr);
			List<Procedure> listProcs=constructChargesData.ListProcsCompleted;//filled from db. List of completed procs for patient(s).
			List<Adjustment> listAdjustments=constructChargesData.ListAdjustments;
			List<PayAsTotal> listInsPayAsTotals=constructChargesData.ListInsPayAsTotal;//Claimprocs paid as total, might contain ins payplan payments.
			//Used to figure out how much we need to pay off procs with, also contains insurance payplans.
			List<PayPlan> listPayPlans=constructChargesData.ListPayPlans;
			List<PayPlanCharge> listPayPlanCharges=constructChargesData.ListPayPlanCharges;
			List<ClaimProc> listClaimProcsFiltered=constructChargesData.ListClaimProcsFiltered;
			#endregion
			#region Construct List of Charges
			ConstructListChargesResult constructChargeResult=ConstructListCharges(listPayPlanCharges,listPayPlans,listAdjustments,listProcs
				,constructChargesData.ListPaySplits,listInsPayAsTotals,isIncomeTxfr);
			retVal.ListAccountCharges=constructChargeResult.ListAccountEntries;
			retVal.ListPayPlanCharges=constructChargeResult.ListPayPlanCharges;
			retVal.ListAccountCharges.Sort(AccountEntrySort);
			#endregion
			#region Explicitly Link Credits
			SplitCollection listSplitsCurrentAndHistoric=new SplitCollection();
			//This ordering is necessary so parents come before their children when explicitly linking credits.
			listSplitsCurrentAndHistoric.AddRange(constructChargesData.ListPaySplits.OrderBy(x => x.DatePay).ThenBy(x => x.FSplitNum).ToList());
			listSplitsCurrentAndHistoric.AddRange(listSplitsCur.Where(x => x.SplitNum==0).Select(y => y.Copy()).ToList());
			retVal.ListAccountCharges=ExplicitlyLinkCredits(retVal,listSplitsCurrentAndHistoric,listClaimProcsFiltered
				,listAdjustments.Where(x => x.ProcNum!=0).ToList(),listPayPlans,isIncomeTxfr);
			#endregion
			#region Implicitly Link Credits
			if(!isIncomeTxfr) {//If this payment is an income transfer, do NOT use unallocated income to pay off charges.
				PayResults implicitResult=ImplicitlyLinkCredits(constructChargesData.ListPaySplits,listAdjustments,listInsPayAsTotals
					,retVal.ListAccountCharges,listSplitsCur,listEntriesLoading,retVal.Payment,patCurNum,isPreferCurPat);
				retVal.ListAccountCharges=implicitResult.ListAccountCharges;
				retVal.ListSplitsCur=implicitResult.ListSplitsCur;
				retVal.Payment=implicitResult.Payment;
			}
			#endregion Implicitly Link Credits
			return retVal;
		}

		public static ConstructListChargesResult ConstructListCharges(List<PayPlanCharge> listPayPlanCharges,List<PayPlan>listPayPlans
			,List<Adjustment>listAdjustments,List<Procedure>listProcs,List<PaySplit>listPaySplits,List<PayAsTotal>listInsPayAsTotal,bool hasPayTypeNone) 
		{
			//No remoting role check; no call to db
			ConstructListChargesResult retVal=new ConstructListChargesResult();
			List<AccountEntry> listCharges=new List<AccountEntry>();
			List<PayPlanCharge> listChargesToRemove=new List<PayPlanCharge>();
			foreach(PayPlanCharge ppc in listPayPlanCharges) {
				PayPlan pp=listPayPlans.Find(x => x.PayPlanNum==ppc.PayPlanNum);
				if(pp.IsClosed && PrefC.IsPayPlanVersion2) {
					listChargesToRemove.Add(ppc);//payment plan is no longer active, we will pay any remaining balance to the procedure directly. 
					continue;
				}
				if(ppc.ChargeType==PayPlanChargeType.Debit && pp.PlanNum==0 && ppc.Principal.IsGreaterThan(0)) {
					listCharges.Add(new AccountEntry(ppc));//only make account charges for positive debits on patient plans.
				}
			}
			retVal.ListPayPlanCharges=listPayPlanCharges.Except(listChargesToRemove).ToList();
			for(int i=0;i<listAdjustments.Count;i++) {
				if((listAdjustments[i].ProcNum==0 && listAdjustments[i].AdjAmt>0) || hasPayTypeNone) {//If there is no procnum on the adjustment, add it to charges if it's charge adjustment, or if it's income transfer mode add them all regardless.
					listCharges.Add(new AccountEntry(listAdjustments[i]));
				}
			}
			for(int i=0;i<listProcs.Count;i++) {
				listCharges.Add(new AccountEntry(listProcs[i]));
			}
			if(hasPayTypeNone) {
				for(int i=listPaySplits.Count-1;i>=0;i--) {
					listCharges.Add(new AccountEntry(listPaySplits[i]));//In Income Transfer mode, add all paysplits to the buckets.  The income transfers made previously should balance out any adjustments/inspaytotals that were transferred previously.
				}
				foreach(PayAsTotal totalPmt in listInsPayAsTotal) {//Ins pay totals need to be added to the sum total for income transfers
					listCharges.Add(new AccountEntry(totalPmt));
				}
			}
			retVal.ListAccountEntries=listCharges;
			return retVal;
		}

		public static List<AccountEntry> ExplicitlyLinkCredits(ConstructResults dataSet,SplitCollection listSplitsCurrentAndHistoric
			,List<ClaimProc> listClaimProcs,List<Adjustment> listAdjusts,List<PayPlan> listPayPlans,bool isIncomeTransfer=false) 
		{
			List<AccountEntry> listAccountCharges=dataSet.ListAccountCharges;
			List<PayPlanCharge> listPayPlanCharges=dataSet.ListPayPlanCharges;
			long payNum=dataSet.Payment.PayNum;
			//No remoting role check; no call to db
			//Explicitly link any of the credits to their corresponding charges if a link can be made. (ie. PaySplit.ProcNum to a Procedure.ProcNum)
			//Any payment plan credits for procedures should get applied to that procedure and removed from the credit total bucket.
			foreach(AccountEntry chargeCur in listAccountCharges) {
				if(chargeCur.Tag.GetType() == typeof(Procedure)) {
					List<Adjustment> listAdjustmentsForProc=new List<Adjustment>();//list of adjustments that will be directly applied to the procedure
					if(isIncomeTransfer) {//Only apply adjustments that have matching pat/prov/clinic values to the procedure.
						listAdjustmentsForProc=listAdjusts.FindAll(x => x.ProcNum==chargeCur.PriKey && x.ProvNum==chargeCur.ProvNum && x.PatNum==chargeCur.PatNum 
							&& x.ClinicNum==chargeCur.ClinicNum);
					}
					else {//Not transfer. Use adjustments to directly modify procedure value regardless
						listAdjustmentsForProc=listAdjusts.FindAll(x => x.ProcNum==chargeCur.PriKey);
					}
					chargeCur.AmountStart=ClaimProcs.GetPatPortion((Procedure)chargeCur.Tag,listClaimProcs,listAdjustmentsForProc);
					chargeCur.AmountEnd=chargeCur.AmountStart;
					decimal sumCreditsForProc=0;
					if(isIncomeTransfer) {
						List<long> listValidAdjNums=listAdjustmentsForProc.Select(x => x.AdjNum).ToList();
						foreach(AccountEntry adjEntry in listAccountCharges.FindAll(x => x.GetType()==typeof(Adjustment) && x.PriKey.In(listValidAdjNums))) {
							//remove value from the adjustment entries since they were just applied to the procedure in GetPatPortion()
							adjEntry.AmountStart=0;
							adjEntry.AmountEnd=0;
						}
						sumCreditsForProc=(decimal)listPayPlanCharges
						.Where(x => x.ChargeType==PayPlanChargeType.Credit && x.ProcNum==chargeCur.PriKey && x.ProvNum==chargeCur.ProvNum 
							&& x.PatNum==chargeCur.PatNum && x.ClinicNum==chargeCur.ClinicNum)
						.Sum(x => x.Principal);
					}
					else {
						sumCreditsForProc=(decimal)listPayPlanCharges
						.Where(x => x.ChargeType==PayPlanChargeType.Credit && x.ProcNum==chargeCur.PriKey)
						.Sum(x => x.Principal);
					}
					//attaching more credits than the procedure is worth will apply that credit to account charges below (after Explicitly Link Credits region end)
					chargeCur.AmountOriginal-=sumCreditsForProc;
					chargeCur.AmountStart-=sumCreditsForProc;
					chargeCur.AmountEnd-=sumCreditsForProc;
				}
				//Link payplan adjustments to their payplan charge (linked by date) to reduce the amount on the charge
				if(chargeCur.Tag.GetType()==typeof(PayPlanCharge)) {
					PayPlan payplan=listPayPlans.Find(x => x.PayPlanNum==((PayPlanCharge)chargeCur.Tag).PayPlanNum);
					//find all negative adjustments for charge.
					List<PayPlanCharge> listNegatives=listPayPlanCharges.FindAll(x => x.ChargeDate==((PayPlanCharge)chargeCur.Tag).ChargeDate
							&& x.PayPlanNum==payplan.PayPlanNum && x.IsDebitAdjustment);
					if(listNegatives.Count>0) {
						decimal sumAdjustmentsForCharge=Math.Abs((decimal)listNegatives.Sum(x => x.Principal));//stored as negative debit
						//modify the principal amt to apply the adjustment to the charge
						//users can modify adj to be more than the corresponding charge. We would not expect accuracy, but safeguard so it does not break stuff
						chargeCur.AmountStart-=Math.Min(sumAdjustmentsForCharge,(decimal)((PayPlanCharge)chargeCur.Tag).Principal);
						chargeCur.AmountEnd-=sumAdjustmentsForCharge;
					}
				}
			}
			//Procedures First
			//Existing Payments on the payment plan
			foreach(PaySplit splitCur in listSplitsCurrentAndHistoric) {
				foreach(AccountEntry chargeCur in listAccountCharges) {
					if(splitCur.SplitAmt==0) {//necessary for payplancharges to split and track the correct amount
						break;
					}
					//We want to explicitly pay the procedure if the payment plan is closed and on version 2, but don't if it is open or on version 1,3.
					bool doAttachSplitForPayPlan=listPayPlans.Any(x => x.PayPlanNum==splitCur.PayPlanNum && x.IsClosed) && PrefC.IsPayPlanVersion2;
					//======== NOTE: Any explicitly linked paysplit needs to be used on what it's attached to in its entirety (even if it's overpaid). =========  
					//Overpayments are taken care of later.
					//if the account entry is a procedure and the split is attached to it, then explicitly apply the payment to the procedure.
					if(chargeCur.Tag.GetType() == typeof(Procedure) && splitCur.ProcNum==chargeCur.PriKey && (splitCur.PayPlanNum==0 || doAttachSplitForPayPlan)
						&& (!isIncomeTransfer
							|| (isIncomeTransfer && chargeCur.ProvNum==splitCur.ProvNum && chargeCur.PatNum==splitCur.PatNum && chargeCur.ClinicNum==splitCur.ClinicNum))) 
					{
						decimal amtStart=(decimal)splitCur.SplitAmt;//Overpayment on procedures is handled later
						if(splitCur.PayNum!=payNum) {
							chargeCur.AmountStart-=amtStart;
						}
						PaySplit splitForCollection=splitCur.Copy();//take copy so we can get amtPaid without overwriting.
						splitForCollection.AccountEntryAmtPaid=(decimal)amtStart;
						chargeCur.AmountEnd-=amtStart;
						chargeCur.SplitCollection.Add(splitForCollection);
						splitCur.SplitAmt-=(double)amtStart;
						AccountEntry acctEntry=listAccountCharges.Find(x => x.GetType()==typeof(PaySplit) && x.SplitCollection.Contains(splitCur));
						if(acctEntry==null) {
							break;
						}
						acctEntry.AmountOriginal+=amtStart;
						acctEntry.AmountStart+=amtStart;
						acctEntry.AmountEnd+=amtStart;
						break;
					}
					//else if the account entry is a payplancharge, then explicitly apply it to that payplancharge
					else if(chargeCur.Tag.GetType()==typeof(PayPlanCharge)
						&& splitCur.PayPlanNum==((PayPlanCharge)chargeCur.Tag).PayPlanNum
						&& ((PayPlanCharge)chargeCur.Tag).ChargeType==PayPlanChargeType.Debit
						&& chargeCur.AmountEnd>0
						&& (!isIncomeTransfer
							|| (isIncomeTransfer && chargeCur.ProvNum==splitCur.ProvNum && (chargeCur.PatNum==splitCur.PatNum || ((PayPlanCharge)chargeCur.Tag).Guarantor==splitCur.PatNum) && chargeCur.ClinicNum==splitCur.ClinicNum))) 
					{  //If this payplancharge is for the pat or guarantor of a payplan, and the split is for either one, use the split to pay off payplan.
						double amt=Math.Min(splitCur.SplitAmt,(double)chargeCur.AmountEnd);
						if(splitCur.PayNum!=payNum) {
							chargeCur.AmountStart-=(decimal)amt;
						}
						PaySplit splitForCollection=splitCur.Copy();//take copy so we can get amtPaid without overwriting (for autosplitting payplancharges)
						splitForCollection.AccountEntryAmtPaid=(decimal)amt;
						chargeCur.AmountOriginal-=(decimal)amt;
						chargeCur.AmountEnd-=(decimal)amt;
						splitCur.SplitAmt-=amt;
						//add a copy of the paysplit to the charge so that we can keep track AccountEntryAmtPaid.
						chargeCur.SplitCollection.Add(splitForCollection);
						//also add to the linked procedure's split collection so we can keep track of which procedure to link to depending on amount paid. 
						listAccountCharges.Where(x => x.GetType()==typeof(Procedure) && x.PriKey==splitForCollection.ProcNum)
							.ForEach(x => x.SplitCollection.Add(splitForCollection));
						AccountEntry negEntry=listAccountCharges.Find(x => x.GetType()==typeof(PaySplit) && x.PriKey==splitCur.SplitNum);
						if(negEntry!=null) {
							negEntry.AmountOriginal=0;
							negEntry.AmountStart=0;
							negEntry.AmountEnd=0;
						}
					}
					else if(chargeCur.Tag.GetType()==typeof(Adjustment)
						&& (splitCur.AdjNum==chargeCur.PriKey || (splitCur.ProcNum!=0 && splitCur.ProcNum==((Adjustment)chargeCur.Tag).ProcNum))
						&& (splitCur.PayPlanNum==0 || doAttachSplitForPayPlan)
						&& chargeCur.ProvNum==splitCur.ProvNum && chargeCur.PatNum==splitCur.PatNum && chargeCur.ClinicNum==splitCur.ClinicNum) 
					{
						decimal amtStart=(decimal)splitCur.SplitAmt;
						if(splitCur.PayNum!=payNum) {
							chargeCur.AmountStart-=amtStart;
						}
						PaySplit splitForCollection=splitCur.Copy();//take copy so we can get amtPaid without overwriting.
						splitForCollection.AccountEntryAmtPaid=(decimal)amtStart;
						chargeCur.AmountEnd-=amtStart;
						chargeCur.SplitCollection.Add(splitForCollection);
						splitCur.SplitAmt-=(double)amtStart;
						AccountEntry acctEntry=listAccountCharges.Find(x => x.GetType()==typeof(PaySplit) && x.SplitCollection.Contains(splitCur));
						if(acctEntry==null) {
							break;
						}
						acctEntry.AmountOriginal+=amtStart;
						acctEntry.AmountStart+=amtStart;
						acctEntry.AmountEnd+=amtStart;
						break;
					}
					else if(chargeCur.Tag.GetType()==typeof(PaySplit)//PaySplits will only ever be in the charge list if it's in income transfer mode.
						&& chargeCur.AmountEnd!=0 && chargeCur.PriKey==splitCur.FSplitNum //fsplit num is correctly linked
						&& (chargeCur.ProvNum==splitCur.ProvNum || chargeCur.ProvNum==0 || splitCur.ProvNum==0)//matching provs, except for unearned provider(0)
						&& chargeCur.PatNum==splitCur.PatNum && chargeCur.ClinicNum==splitCur.ClinicNum //matching pat/prov/clinics
						&& splitCur.ProcNum==0 && splitCur.AdjNum==0 && splitCur.PayPlanNum==0) //split is unattached
					{
						if(splitCur.PayNum!=payNum) {
							chargeCur.AmountStart-=(decimal)splitCur.SplitAmt;//splits counteracting prepay are negative
						}
						PaySplit splitForCollection=splitCur.Copy();
						splitForCollection.AccountEntryAmtPaid=(decimal)splitCur.SplitAmt;
						chargeCur.AmountEnd-=(decimal)splitCur.SplitAmt;
						chargeCur.SplitCollection.Add(splitForCollection);
						List<AccountEntry> listPaySplitAccountEntries=listAccountCharges.FindAll(x => x.GetType()==typeof(PaySplit));
						AccountEntry splitCurEntry=listPaySplitAccountEntries.FirstOrDefault(x => x.PriKey==splitCur.SplitNum);//split entry
						if(splitCurEntry!=null) {
							splitCurEntry.AmountStart=0;//final allocations are likely to a procedure which gets handled elsewhere, don't double count them. 
							splitCurEntry.AmountEnd=0;
						}
						break;
					}
				}
				//Do not subtract amount from split or parent split here since we are looping through them and may be modifying split value that hasn't
				//been evaluated yet. If this code absolutely needs to be added back, make to only have it execute when it is not an income transfer.
			}
			listAccountCharges=LinkUnearnedForPatProvClinicUnearnedType(listAccountCharges);
			if(isIncomeTransfer) {
				//Apply payments to pat/prov/clinic buckets. This is similar to "implicit linking" for paysplits, which does not happen for transfers
				List<AccountEntry> listSplitEntries=listAccountCharges.Where(x => x.GetType()==typeof(PaySplit) && !x.AmountEnd.IsEqual(0)).ToList();
				foreach(PaySplit splitCur in listSplitsCurrentAndHistoric.Where(x => x.SplitNum.In(listSplitEntries.Select(y => y.PriKey).ToList()))) {
					foreach(AccountEntry chargeCur in listSplitEntries) {
						AccountEntry splitEntry=listSplitEntries.FirstOrDefault(x => x.PriKey==splitCur.SplitNum);
						if(splitEntry==null || splitEntry.AmountEnd.IsEqual(0)) {
							break;
						}
						//if splitCur.ProcNum==0 NEEDS to be added back in the future, there is an additional case that will need to be handled elsewhere.  B13471.
						if(!chargeCur.AmountEnd.IsEqual(0) && chargeCur.PriKey!=splitCur.SplitNum
							&& (chargeCur.ProvNum==splitCur.ProvNum || chargeCur.ProvNum==0 || splitCur.ProvNum==0)//matching provs, except unearned provider (0)
							&& chargeCur.PatNum==splitCur.PatNum && chargeCur.ClinicNum==splitCur.ClinicNum) 
						{
							if(Math.Sign(Math.Round(chargeCur.AmountEnd,3))==Math.Sign(Math.Round(splitEntry.AmountEnd,3))) {
								continue;//do not apply splits to other splits when they are of the same sign, we want them to stay separated, not get combined.
							}
							decimal amount=Math.Min(Math.Abs(splitEntry.AmountEnd),Math.Abs(chargeCur.AmountEnd));
							decimal splitAmount=amount;
							if(chargeCur.AmountEnd<0) {//negative charge, positive splitEntry
								amount=amount*-1;
							}
							if(splitCur.SplitAmt<0) {
								splitAmount=amount*-1;
							}
							if(splitCur.PayNum!=payNum) {
								chargeCur.AmountStart-=amount;
							}
							PaySplit splitForCollection=splitCur.Copy();
							splitForCollection.AccountEntryAmtPaid=splitAmount;
							chargeCur.AmountEnd-=amount;
							chargeCur.SplitCollection.Add(splitForCollection);
							splitEntry.AmountStart+=amount;
							splitEntry.AmountEnd+=amount;
						}
					}
				}
			}
			return listAccountCharges;
		}

		///<summary>This method applies positive and negative unearned to each other so the sum is correct before making any transfers.
		///This method assumes that explicit linking has been done before it is called so the unallocated splits no longer have any value (if any)
		///Without this method when a procedure, postive unearned, and negative unearned are on an account the postive would be applied to the procedure,
		///leaving a negative on the account. We want the positive and negative to cancel each other out before any transfers are made.</summary>
		public static List<AccountEntry> LinkUnearnedForPatProvClinicUnearnedType(List<AccountEntry> listAccountCharges) {
			List<AccountEntry> listUnearned=listAccountCharges.FindAll(x => x.GetType()==typeof(PaySplit) && ((PaySplit)x.Tag).UnearnedType!=0);
			List<AccountEntry> listPositiveUnearned=listUnearned.FindAll(x => x.AmountEnd > 0);//negative paysplit (like from refunds)
			List<AccountEntry> listNegativeUnearned=listUnearned.FindAll(x => x.AmountEnd < 0);//regular positve paysplits
			foreach(AccountEntry positiveEntry in listPositiveUnearned) {
				foreach(AccountEntry negativeEntry in listNegativeUnearned) {
					if(positiveEntry.AmountEnd.IsEqual(0)) {
						continue;//no more money to apply.
					}
					if(positiveEntry.ProvNum!=negativeEntry.ProvNum 
						|| positiveEntry.PatNum!=negativeEntry.PatNum 
						|| positiveEntry.ClinicNum!=negativeEntry.ClinicNum	
						|| ((PaySplit)positiveEntry.Tag).UnearnedType!=((PaySplit)negativeEntry.Tag).UnearnedType) 
					{
						continue;
					}
					decimal amount=Math.Min(Math.Abs(positiveEntry.AmountEnd),Math.Abs(negativeEntry.AmountEnd));
					positiveEntry.AmountStart-=amount;
					positiveEntry.AmountEnd-=amount;
					negativeEntry.AmountEnd+=amount;
					negativeEntry.AmountStart+=amount;
				}
			}
			return listAccountCharges;
		}

		public static PayResults ImplicitlyLinkCredits(List<PaySplit> listPaySplits,List<Adjustment> listAdjustments
			,List<PayAsTotal> listInsPayAsTotal,List<AccountEntry> listAccountCharges,List<PaySplit> listSplitsCur,List<AccountEntry> listAcctEntriesLoading
			,Payment payCur,long patNum,bool isPatPrefer) 
		{
			//No remoting role check; no call to db
			//use negative paysplits to counteract inspaytotals/paysplits/adjustments for same prov/pat/clinic-Accounts for income transfers and prevents using thetransferred money.
			PayResults implicitCredits=new PayResults();
			//only get splits that have not yet been explicitly allocated, not attached to procs or payplan.
			List<PaySplit> listSplitsCopied=new List<PaySplit>(listPaySplits.Where(x => x.ProcNum==0 && x.PayPlanNum==0).Select(x => x.Copy()).ToList());
			List<Adjustment> listNegAdjustCopied=new List<Adjustment>(listAdjustments.Where(x => x.ProcNum==0 && x.AdjAmt<0).Select(x => x.Clone()).ToList());
			//use negative paysplits to counteract inspaytotals/paysplits/adjustments for same prov/pat/clinic.  
			//Accounts for income transfers and prevents using the transferred money.
			List<PaySplit> listNegSplits=listSplitsCopied.FindAll(x => x.SplitAmt<0);
			List<long> listHiddenUnearnedDefNums=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType)
				.FindAll(x => !string.IsNullOrEmpty(x.ItemValue))//If ItemValue is not blank, it means "do not show on account"
				.Select(x => x.DefNum).ToList();
			foreach(PaySplit negSplit in listNegSplits) {
				if(negSplit.UnearnedType.In(listHiddenUnearnedDefNums)) {
					continue;
				}
				List<PaySplit> posSplits=listSplitsCopied.FindAll(x => x.SplitAmt>0 && x.PatNum==negSplit.PatNum && x.ProvNum==negSplit.ProvNum &&x.ClinicNum==negSplit.ClinicNum);
				foreach(PaySplit posSplit in posSplits) {
					if(posSplit.UnearnedType.In(listHiddenUnearnedDefNums)) {
						//splits with hidden unearned types are not allowed to be used for implicit linking or income transfers.
						continue;
					}
					if(negSplit.SplitAmt==0) {
						break;
					}
					if(posSplit.SplitAmt==0) {
						continue;
					}
					double amt=Math.Min(Math.Abs(negSplit.SplitAmt),posSplit.SplitAmt);
					negSplit.SplitAmt+=amt;
					posSplit.SplitAmt-=amt;
				}
				List<PayAsTotal> payByTotals=listInsPayAsTotal.FindAll(x => x.PatNum==negSplit.PatNum && x.ProvNum==negSplit.ProvNum 
					&& x.ClinicNum==negSplit.ClinicNum &&x.SummedInsPayAmt!=0);
				foreach(PayAsTotal claimProc in payByTotals) {
					if(negSplit.SplitAmt==0) {
						break;
					}
					if(claimProc.SummedInsPayAmt==0 && claimProc.SummedWriteOff==0) {
						continue;
					}
					double amtPay=Math.Min(Math.Abs(negSplit.SplitAmt),claimProc.SummedInsPayAmt);
					negSplit.SplitAmt+=amtPay;
					claimProc.SummedInsPayAmt-=amtPay;
					double amtWriteOff=Math.Min(Math.Abs(negSplit.SplitAmt),claimProc.SummedWriteOff);
					negSplit.SplitAmt+=amtWriteOff;
					claimProc.SummedWriteOff-=amtWriteOff;
				}
				List<Adjustment> listAdjusts=listNegAdjustCopied.FindAll(x => x.PatNum==negSplit.PatNum && x.ProvNum==negSplit.ProvNum 
					&& x.ClinicNum==negSplit.ClinicNum);
				foreach(Adjustment negAdjust in listAdjusts) {
					if(negSplit.SplitAmt==0) {
						break;
					}
					if(negAdjust.AdjAmt==0) {
						continue;
					}
					double amt=Math.Min(Math.Abs(negSplit.SplitAmt),Math.Abs(negAdjust.AdjAmt));
					//The split is always negative here.
					negSplit.SplitAmt+=amt;
					negAdjust.AdjAmt+=amt;
				}
			}
			if(isPatPrefer) {
				//Sort current pat charges so they are more likely to be "unpaid" by implicit linking which leads to them being paid by this payment.
				listAccountCharges=listAccountCharges.OrderByDescending(x => x.PatNum!=patNum).ThenBy(x => x.Date).ToList();
			}
			else if(PrefC.GetInt(PrefName.AutoSplitLogic)==(int)AutoSplitPreference.Adjustments) {
				//Sort by Adjustments, followed by Procedures. Need to sort by type, then sort by date so get earliest adjs followed by earliest procs
				listAccountCharges=listAccountCharges.OrderBy(x => x.GetType()!=typeof(Adjustment)).ThenBy(x => x.Date).ToList(); 
			}
			if(listAcctEntriesLoading.Count>0) {//User has specific procs/payplancharges/adjustments selected prior to entering the Payment window.  
				//They wish these be paid by this payment specifically.
				//To accomplish this, we need to auto-split to the selected procedures prior to implicit linking.
				foreach(AccountEntry entry in listAcctEntriesLoading) {
					if(payCur.PayAmt<=0) {
						break;//Will be empty
					}
					AccountEntry charge=listAccountCharges.Find(x => x.PriKey==entry.PriKey);
					if(charge==null) {
						continue;//likely only for the event of selecting payplan line items when the plan is closed and on version 2. 
					}
					PaySplit split=new PaySplit();
					double amt=Math.Min((double)charge.AmountEnd,payCur.PayAmt);
					payCur.PayAmt=Math.Round(payCur.PayAmt-amt,3);
					if(charge.GetType()==typeof(PayPlanCharge)) {
						//handles payment plan splits in a special way. Continue to the next entry after we add to the list. 
						PayPlanCharge ppCharge=(PayPlanCharge)charge.Tag;
						decimal payAmtOut;
						List<PaySplit> listPayPlanSplitsToAdd=PaySplits.CreateSplitForPayPlan(payCur.PayNum,amt,charge
							,PayPlanCharges.GetChargesForPayPlanChargeType(ppCharge.PayPlanNum,PayPlanChargeType.Credit),listAccountCharges
							,0,true,out payAmtOut);
						listSplitsCur.AddRange(listPayPlanSplitsToAdd);
						//payCur.PayAmt=(double)payAmtOut;
						charge.SplitCollection.AddRange(listPayPlanSplitsToAdd);
						continue;
					}
					else if(charge.GetType()==typeof(Procedure)) {
						split.ProcNum=charge.PriKey;
					}
					else if(charge.GetType()==typeof(Adjustment) && ((Adjustment)charge.Tag).ProcNum==0) {
						//should already be verified to have no procedure and positive amount
						split.AdjNum=charge.PriKey;
					}
					if(PrefC.HasClinicsEnabled) {//Clinics
						split.ClinicNum=charge.ClinicNum;
					}
					split.SplitAmt=amt;
					charge.AmountEnd-=(decimal)amt;
					split.DatePay=DateTime.Today;
					split.PatNum=charge.PatNum;
					split.ProvNum=charge.ProvNum;
					split.PayNum=payCur.PayNum;
					charge.SplitCollection.Add(split);
					listSplitsCur.Add(split);
				}
			}
			Enumerable.Range(1,8).ForEach(x => PayPatientBalances(ref listInsPayAsTotal,ref listSplitsCopied,ref listNegAdjustCopied,x,ref listAccountCharges));
			if(isPatPrefer) {
				//Sort current pat charges so they are more likely to be paid by this payment.
				listAccountCharges=listAccountCharges.OrderBy(x => x.PatNum!=patNum).ThenBy(x => x.Date).ToList();
			}
			implicitCredits.ListAccountCharges=listAccountCharges;
			implicitCredits.ListSplitsCur=listSplitsCur;
			implicitCredits.Payment=payCur;
			return implicitCredits;
		}

		///<summary>This function takes a list of a patient's credits and implicitly links them to the patient's charges.  Lists are passed in by reference
		///because the lists are used multiple times (they keep track of if a PaySplit has been fully used, for example).  Make sure to pass in copies
		///if you don't want original data overwritten.  Phase 1 matches on Prov, Clinic, and Patient.  Phase 2 matches on Prov and Patient.  Phase 3 matches only on Patient.
		///The purpose is to implicitly link a patient's credits to their charges as accurately as possible.</summary>
		public static void PayPatientBalances(ref List<PayAsTotal> listInsPayAsTotal,ref List<PaySplit> listPaySplits,ref List<Adjustment> listAdjustments
			,int phase,ref List<AccountEntry> listAccountCharges) 
		{
			//No remoting role check; no call to db
			List<long> listHiddenUnearnedDefNums=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType)
				.FindAll(x => !string.IsNullOrEmpty(x.ItemValue))//If ItemValue is not blank, it means "do not show on account"
				.Select(x => x.DefNum).ToList();
			foreach(PayAsTotal payAsTotal in listInsPayAsTotal) {//Use claim payments by total to pay off procedures for that specific patient.
				if(payAsTotal.SummedInsPayAmt==0 && payAsTotal.SummedWriteOff==0) {
					continue;
				}
				//Find all charges that have this claimproc's patient, provider, or clinic, and use the claimproc to pay them off.
				List<AccountEntry> listEntriesForPayAsTotal=new List<AccountEntry>();
				switch (phase) {
					case 1:
						listEntriesForPayAsTotal=listAccountCharges.FindAll(x => x.ProvNum==payAsTotal.ProvNum && x.PatNum==payAsTotal.PatNum && x.ClinicNum==payAsTotal.ClinicNum);
						break;
					case 2:
						listEntriesForPayAsTotal=listAccountCharges.FindAll(x => x.ProvNum==payAsTotal.ProvNum && x.PatNum==payAsTotal.PatNum);
						break;
					case 3:
						listEntriesForPayAsTotal=listAccountCharges.FindAll(x => x.ProvNum==payAsTotal.ProvNum && x.ClinicNum==payAsTotal.ClinicNum);
						break;
					case 4:
						listEntriesForPayAsTotal=listAccountCharges.FindAll(x => x.PatNum==payAsTotal.PatNum && x.ClinicNum==payAsTotal.ClinicNum);
						break;
					case 5:
						listEntriesForPayAsTotal=listAccountCharges.FindAll(x => x.ProvNum==payAsTotal.ProvNum);
						break;
					case 6:
						listEntriesForPayAsTotal=listAccountCharges.FindAll(x => x.PatNum==payAsTotal.PatNum);
						break;
					case 7:
						listEntriesForPayAsTotal=listAccountCharges.FindAll(x => x.ClinicNum==payAsTotal.ClinicNum);
						break;
					case 8:
						listEntriesForPayAsTotal=listAccountCharges;//Any unpaid charge.
						break;
				}
				foreach(AccountEntry accountEntry in listEntriesForPayAsTotal) {
					if(payAsTotal.SummedInsPayAmt==0 && payAsTotal.SummedWriteOff==0) {
						break;
					}
					if(accountEntry.AmountEnd==0) {
						continue;
					}
					if(accountEntry.GetType()==typeof(PayPlanCharge)) {//B12401
						continue;
					}
					double amt=Math.Min((double)accountEntry.AmountEnd,payAsTotal.SummedInsPayAmt);
					accountEntry.AmountStart-=(decimal)amt;
					accountEntry.AmountEnd-=(decimal)amt;
					payAsTotal.SummedInsPayAmt-=amt;
					double amtWriteOff=Math.Min((double)accountEntry.AmountEnd,payAsTotal.SummedWriteOff);
					accountEntry.AmountStart-=(decimal)amtWriteOff;
					accountEntry.AmountEnd-=(decimal)amtWriteOff;
					payAsTotal.SummedWriteOff-=amtWriteOff;
				}
			}
			foreach(PaySplit split in listPaySplits) {//Use unattached positive paysplits to pay off patient's procedures as accurately as possible.
				if(split.UnearnedType.In(listHiddenUnearnedDefNums)) {
					continue;//Splits that do not show on the account are not valid to use for implicit linking
				}
				if(split.ProcNum!=0) {
					continue;//Don't use explicitly linked paysplits.
				}
				if(split.SplitAmt<=0) {
					continue;
				}
				//Find all charges that have this split's patient, provider, or clinic, and use the split to pay them off.
				List<AccountEntry> listEntriesForSplit=new List<AccountEntry>();
				switch (phase) {
					case 1:
						listEntriesForSplit=listAccountCharges.FindAll(x => x.ProvNum==split.ProvNum && x.PatNum==split.PatNum && x.ClinicNum==split.ClinicNum);
						break;
					case 2:
						listEntriesForSplit=listAccountCharges.FindAll(x => x.ProvNum==split.ProvNum && x.PatNum==split.PatNum);
						break;
					case 3:
						listEntriesForSplit=listAccountCharges.FindAll(x => x.ProvNum==split.ProvNum && x.ClinicNum==split.ClinicNum);
						break;
					case 4:
						listEntriesForSplit=listAccountCharges.FindAll(x => x.PatNum==split.PatNum && x.ClinicNum==split.ClinicNum);
						break;
					case 5:
						listEntriesForSplit=listAccountCharges.FindAll(x => x.ProvNum==split.ProvNum);
						break;
					case 6:
						listEntriesForSplit=listAccountCharges.FindAll(x => x.PatNum==split.PatNum);
						break;
					case 7:
						listEntriesForSplit=listAccountCharges.FindAll(x => x.ClinicNum==split.ClinicNum);
						break;
					case 8:
						listEntriesForSplit=listAccountCharges;//Any unpaid charge.
						break;
				}
				foreach(AccountEntry accountEntry in listEntriesForSplit) {
					if(split.SplitAmt==0) {
						break;//Split's amount has been used by previous charges.
					}
					if(accountEntry.AmountEnd==0) {
						continue;
					}
					if(accountEntry.GetType()==typeof(PayPlanCharge)) {//B12401
						continue;
					}
					if(accountEntry.GetType()==typeof(Procedure) && ((Procedure)accountEntry.Tag).ProcStatus==ProcStat.TP) {
						continue;//we do not implicitly link to TP procedures
					}
					double amt=Math.Min((double)accountEntry.AmountEnd,split.SplitAmt);
					accountEntry.AmountStart-=(decimal)amt;
					accountEntry.AmountEnd-=(decimal)amt;
					accountEntry.SplitCollection.Add(split);
					split.SplitAmt-=amt;
				}
			}
			foreach(Adjustment adjustment in listAdjustments) {//Use unattached credit adjustments to pay off patient's procedures as accurately as possible.
				if(adjustment.ProcNum!=0) {
					continue;//Don't use explicitly linked adjustments.
				}
				if(adjustment.AdjAmt>=0) {
					continue;//Don't use charge adjustments.
				}
				//Find all charges that have this adjustment's patient, provider, or clinic, and use the adjustment to pay them off.
				List<AccountEntry> listEntriesForAdjust=new List<AccountEntry>();
				switch (phase) {
					case 1:
						listEntriesForAdjust=listAccountCharges.FindAll(x => x.ProvNum==adjustment.ProvNum && x.PatNum==adjustment.PatNum && x.ClinicNum==adjustment.ClinicNum);
						break;
					case 2:
						listEntriesForAdjust=listAccountCharges.FindAll(x => x.ProvNum==adjustment.ProvNum && x.PatNum==adjustment.PatNum);
						break;
					case 3:
						listEntriesForAdjust=listAccountCharges.FindAll(x => x.ProvNum==adjustment.ProvNum && x.ClinicNum==adjustment.ClinicNum);
						break;
					case 4:
						listEntriesForAdjust=listAccountCharges.FindAll(x => x.PatNum==adjustment.PatNum && x.ClinicNum==adjustment.ClinicNum);
						break;
					case 5:
						listEntriesForAdjust=listAccountCharges.FindAll(x => x.ProvNum==adjustment.ProvNum);
						break;
					case 6:
						listEntriesForAdjust=listAccountCharges.FindAll(x => x.PatNum==adjustment.PatNum);
						break;
					case 7:
						listEntriesForAdjust=listAccountCharges.FindAll(x => x.ClinicNum==adjustment.ClinicNum);
						break;
					case 8:
						listEntriesForAdjust=listAccountCharges;//Any unpaid charge.
						break;
				}
				foreach(AccountEntry accountEntry in listEntriesForAdjust) {
					if(adjustment.AdjAmt==0) {
						break;//Adjustment's amount has been used by previous charges.
					}
					if(accountEntry.AmountEnd==0) {
						continue;
					}
					if(accountEntry.GetType()==typeof(PayPlanCharge)) {//B12401
						continue;
					}
					double amt=Math.Min((double)accountEntry.AmountEnd,Math.Abs(adjustment.AdjAmt));//Credit adjustments are negative.
					accountEntry.AmountStart-=(decimal)amt;
					accountEntry.AmountEnd-=(decimal)amt;
					adjustment.AdjAmt+=amt;//Credit ajustments are negative.
				}
			}
		}
		#endregion

		///<summary>This function takes a list of a patient's prepayments and first explicitly links prepayment then implicitly links them to the existing prepayments.</summary>
		public static List<PaySplits.PaySplitAssociated> AllocateUnearned(List<AccountEntry> listAccountEntries,ref List<PaySplit> listPaySplit,Payment payCur
			,double unearnedAmt,Family fam) 
		{
			//No remoting role check; Method that uses ref parameters.
			List<PaySplits.PaySplitAssociated> retVal=new List<PaySplits.PaySplitAssociated>();
			if(unearnedAmt.IsLessThan(0) || listAccountEntries==null) {
				return retVal;
			}
			//don't allocate prepayments that are attached to TP procedures or that are flagged as hidden on account. 
			List<PaySplit> listFamPrePaySplits=PaySplits.GetPrepayForFam(fam,doExcludeTpPrepay:true);
			//Manipulate the original prepayments SplitAmt by subtracting counteracting SplitAmt.
			foreach(PaySplit prePaySplit in listFamPrePaySplits.FindAll(x => x.SplitAmt.IsGreaterThan(0))) {
				List<PaySplit> listSplitsForPrePay=PaySplits.GetSplitsForPrepay(new List<PaySplit>() { prePaySplit });//Find all splits for the pre-payment.
				foreach(PaySplit splitForPrePay in listSplitsForPrePay) {
					prePaySplit.SplitAmt+=splitForPrePay.SplitAmt;//Sum the amount of the pre-payment that's used. (balancing splits are negative usually)
				}
			}
			//We will try our best to automatically link any prepayments that don't have an explicit counteracting paysplit.
			//After all prepayments have been "linked", manipulate unearnedAmt accordingly.
			ImplicitlyLinkPrepayments(ref listFamPrePaySplits,ref unearnedAmt);
			//Get all claimprocs and adjustments for the proc;
			List<Procedure> listProcs=PaymentEdit.GetProcsForAccountEntries(listAccountEntries);
			List<ClaimProc> listClaimProcs=ClaimProcs.GetForProcs(listProcs.Select(x => x.ProcNum).Distinct().ToList());
			List<Adjustment> listAdjusts=Adjustments.GetForProcs(listProcs.Select(x => x.ProcNum).Distinct().ToList());
			foreach(Procedure proc in listProcs) {//For each proc see how much of the Unearned we use per proc
				if(unearnedAmt<=0) {
					break;
				}
				//Calculate the amount remaining on the procedure so we can know how much of the remaining pre-payment amount we can use.
				proc.ProcFee-=PaySplits.GetPaySplitsFromProc(proc.ProcNum).Sum(x => x.SplitAmt);//Figure out how much is due on this proc.
				double patPortion=(double)ClaimProcs.GetPatPortion(proc,listClaimProcs,listAdjusts);
				foreach(PaySplit prePaySplit in listFamPrePaySplits) {
					//First we need to decide how much of each pre-payment split we can use per proc.
					if(patPortion<=0) {//Proc has been paid for, go to next proc
						break;
					}
					if(prePaySplit.SplitAmt<=0) {//Split has been used, go to next split
						continue;
					}
					if(patPortion<=0) {
						break;
					}
					decimal splitTotal=0;
					if(splitTotal<(decimal)prePaySplit.SplitAmt) {//If the sum indicates there's pre-payment amount left over, let's use it.
						double amtToUse=0;
						if(prePaySplit.SplitAmt<patPortion) {
							amtToUse=prePaySplit.SplitAmt;
						}
						else {
							amtToUse=patPortion;
						}
						unearnedAmt-=amtToUse;//Reflect the new unearned amount available for future proc use.
						PaySplit splitNeg=new PaySplit();
						splitNeg.PatNum=prePaySplit.PatNum;
						splitNeg.PayNum=payCur.PayNum;
						splitNeg.FSplitNum=prePaySplit.SplitNum;
						splitNeg.ClinicNum=prePaySplit.ClinicNum;
						splitNeg.ProvNum=prePaySplit.ProvNum;
						splitNeg.SplitAmt=0-amtToUse;
						splitNeg.UnearnedType=prePaySplit.UnearnedType;
						splitNeg.DatePay=DateTime.Now;
						listPaySplit.Add(splitNeg);
						//Make a different paysplit attached to proc and prov they want to use it for.
						PaySplit splitPos=new PaySplit();
						splitPos.PatNum=proc.PatNum;//Use procedure's pat to allocate to that patient's account even when viewing entire family.
						splitPos.PayNum=payCur.PayNum;
						splitPos.FSplitNum=0;//The association will be done on form closing.
						splitPos.ProvNum=proc.ProvNum;
						splitPos.ClinicNum=proc.ClinicNum;
						splitPos.SplitAmt=amtToUse;
						splitPos.DatePay=DateTime.Now;
						splitPos.ProcNum=proc.ProcNum;
						listPaySplit.Add(splitPos);
						//link negSplit to posSplit. 
						retVal.Add(new PaySplits.PaySplitAssociated(splitNeg,splitPos));
						//link original prepayment to neg split.
						PaySplit paySplitPrePayOrig=PaySplits.GetOne(prePaySplit.SplitNum);
						retVal.Add(new PaySplits.PaySplitAssociated(paySplitPrePayOrig,splitNeg));
						prePaySplit.SplitAmt-=amtToUse;
						patPortion-=amtToUse;
					}
				}
			}
			return retVal;
		}

		///<summary>This function takes a list of a patient's prepayments and implicitly links them to unlinked paysplits.  Lists are passed in by reference
		///because the lists are used multiple times.</summary>
		private static void ImplicitlyLinkPrepayments(ref List<PaySplit> listFamPrePaySplits, ref double unearnedAmt) {
			//No remoting role check; private method that uses ref parameters.
			List<PaySplit> listPosPrePay=listFamPrePaySplits.FindAll(x => x.SplitAmt.IsGreaterThan(0));
			List<PaySplit> listNegPrePay=listFamPrePaySplits.FindAll(x => x.SplitAmt.IsLessThan(0));
			//Logic check PatNum - match, ProvNum - match, ClinicNum - match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumMatch:true,isClinicNumMatch:true);
			//Logic check PatNum - match, ProvNum - match, ClinicNum - zero
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumMatch:true,isClinicNumZero:true);
			//Logic check PatNum - match, ProvNum - match, ClinicNum - non zero & non match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumMatch:true,isClinicNonZeroNonMatch:true);
			//Logic check PatNum - match, ProvNum - zero, ClinicNum - match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumZero:true,isClinicNumMatch:true);
			//Logic check PatNum - match, ProvNum - zero, ClinicNum - zero
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumZero:true,isClinicNumZero:true);
			//Logic check PatNum - match, ProvNum - zero, ClinicNum - non zero & non match 
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumZero:true,isClinicNonZeroNonMatch:true);
			//Logic check PatNum - match, ProvNum - non zero & non match, ClinicNum - match 
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isPatMatch:true,isProvNonZeroNonMatch:true,isClinicNumMatch:true);
			//Logic check PatNum - match, ProvNum - non zero & non match, ClinicNum - zero
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isPatMatch:true,isProvNonZeroNonMatch:true,isClinicNumZero:true);
			//Logic check PatNum - match, ProvNum - non zero & non match, ClinicNum - non zero & non match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isPatMatch:true,isProvNonZeroNonMatch:true,isClinicNonZeroNonMatch:true);
			//Logic check PatNum - other family members, ProvNum - match, ClinicNum - match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumMatch:true,isClinicNumMatch:true);
			//Logic check PatNum - other family members, ProvNum - match, ClinicNum - zero
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumMatch:true,isClinicNumZero:true);
			//Logic check PatNum - other family members, ProvNum - match, ClinicNum - non zero & non match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumMatch:true,isClinicNonZeroNonMatch:true);
			//Logic check PatNum - other family members, ProvNum - zero, ClinicNum - match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumZero:true,isClinicNumMatch:true);
			//Logic check PatNum - other family members, ProvNum - zero, ClinicNum - zero
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumZero:true,isClinicNumZero:false);
			//Logic checkPatNum - other family members, ProvNum - zero, ClinicNum - non zero & non match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumZero:true,isClinicNonZeroNonMatch:true);
			//Logic checkPatNum - other family members, ProvNum - non zero & non match, ClinicNum - match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isFamMatch:true,isProvNonZeroNonMatch:true,isClinicNumMatch:true);
			//Logic check PatNum - other family members, ProvNum - non zero & non match, ClinicNum - zero
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isFamMatch:true,isProvNonZeroNonMatch:true,isClinicNumZero:true);
			//Logic check PatNum - other family members, ProvNum - non zero & non match, ClinicNum - non zero & non match
			unearnedAmt=ImplicitlyLinkPrepaymentsHelper(listPosPrePay,listNegPrePay,unearnedAmt,isFamMatch:true,isProvNonZeroNonMatch:true,isClinicNonZeroNonMatch:true);
		}

		/// <summary> Helpler method to allocate unearned implicitly. Returns amount remaining to be allocated after implicitly linking.</summary>
		public static double ImplicitlyLinkPrepaymentsHelper(List<PaySplit> listPosPrePay,List<PaySplit> listNegPrePay,double unearnedAmt,bool isPatMatch=false
			,bool isProvNumMatch=false,bool isClinicNumMatch=false,bool isFamMatch=false,bool isProvNumZero=false,bool isClinicNumZero=false
			,bool isProvNonZeroNonMatch=false,bool isClinicNonZeroNonMatch=false) 
		{
			//No remoting role check; no call to db.
			if(unearnedAmt.IsLessThan(0) || unearnedAmt.IsZero()){
				return 0;
			}
			//Manipulate the amounts (never letting them go into the negative) and then use whatever is left to represent the unearned amount.
			foreach(PaySplit posSplit in listPosPrePay) {
				if(posSplit.SplitAmt.IsEqual(0)) {
					continue;
				}
				//Find all negative paysplits with the filters
				List<PaySplit> filteredNegSplit=listNegPrePay
					.Where(x => !isPatMatch               || x.PatNum==posSplit.PatNum)
					.Where(x => !isFamMatch               || x.PatNum!=posSplit.PatNum)
					.Where(x => !isProvNumMatch           || x.ProvNum==posSplit.ProvNum)
					.Where(x => !isProvNumZero            || x.ProvNum==0)
					.Where(x => !isProvNonZeroNonMatch    || (x.ProvNum!=0 && x.ProvNum!=posSplit.ProvNum))
					.Where(x => !isClinicNumMatch         || x.ClinicNum==posSplit.ClinicNum)
					.Where(x => !isClinicNumZero          || x.ClinicNum==0)
					.Where(x => !isClinicNonZeroNonMatch  || (x.ClinicNum!=0 && x.ClinicNum!=posSplit.ClinicNum))
					.ToList();
				foreach(PaySplit negSplit in filteredNegSplit) {
					if(negSplit.SplitAmt.IsEqual(0)) {
						continue;
					}
					//Deduct split amount from the positive split for the amount of the negative split,or the postive split depending on which is smaller according to the absolute value. 
					double amt=Math.Min(posSplit.SplitAmt,Math.Abs(negSplit.SplitAmt));
					negSplit.SplitAmt+=amt;
					posSplit.SplitAmt-=amt;
				}
			}
			return listPosPrePay.Sum(x => x.SplitAmt);
		}

		/// <summary>Makes a payment from a passed in list of charges.</summary>
		public static PayResults MakePayment(List<List<AccountEntry>> listSelectedCharges,List<PaySplit> listSplitsCur,bool isShowAll,Payment payCur
			,int groupIdx,bool hasPayTypeNone,decimal textAmount,List<AccountEntry> listAllCharges) 
		{
			//No remoting role check; no call to db.
			PayResults splitData=null;
			if(hasPayTypeNone) {//Make simple splits, negative and positive, to no procedures, to even out the account balances.
				splitData=new PayResults { Payment=payCur };
				for(int i=0;i<listSelectedCharges.Count;i++) {
					PayResults createdSplit=new PayResults();
					PaySplit paySplit=new PaySplit();
					if(groupIdx==1) {//Grouped by provider
						//listCharges should all have same provider and patient.
						paySplit.ClinicNum=Clinics.ClinicNum;//Use current clinic
					}
					else {//Grouped by provider and clinic
						//listCharges should all have same provider and patient and clinic.
						paySplit.ClinicNum=listSelectedCharges[i][0].ClinicNum;
					}
					paySplit.DatePay=DateTime.Today;
					paySplit.PatNum=listSelectedCharges[i][0].PatNum;
					paySplit.PayNum=payCur.PayNum;
					paySplit.ProvNum=listSelectedCharges[i][0].ProvNum;
					paySplit.SplitAmt=(double)listSelectedCharges[i].Sum(x => x.AmountEnd);
					if(listSelectedCharges[i].Any(x => x.GetType()==typeof(PaySplit))) {
						paySplit.UnearnedType=((PaySplit)listSelectedCharges[i].First(x => x.GetType()==typeof(PaySplit)).Tag).UnearnedType;
						foreach(AccountEntry entry in listSelectedCharges[i]) {
							if(entry.GetType()==typeof(PaySplit)) {
								PaySplit split=(PaySplit)entry.Tag;
								if(split.UnearnedType!=paySplit.UnearnedType) {
									paySplit.UnearnedType=Defs.GetDef(DefCat.PaySplitUnearnedType,PrefC.GetLong(PrefName.PrepaymentUnearnedType)).DefNum;
									break;
								}
							}
						}
					}
					listSelectedCharges[i][0].SplitCollection.Add(paySplit);
					splitData.ListSplitsCur.Add(paySplit);
					listSelectedCharges[i].ForEach(x => x.AmountEnd=0);
				}
				splitData.ListAccountCharges.AddRange(listAllCharges);
				splitData.ListSplitsCur.AddRange(listSplitsCur);
				return splitData;
			}
			bool isPayAmtZeroUponEntering=(payCur.PayAmt==0);
			for(int i=0;i<listSelectedCharges.Count;i++) {
				if(payCur.PayAmt==0 && !isPayAmtZeroUponEntering) {
					break;
				}
				if(groupIdx>0) {//group by clinic and/or provider
					foreach(AccountEntry charge in listSelectedCharges[i]) {
						if(charge.AmountEnd==0 && !isShowAll) {
							continue;
						}
						decimal splitAmt=(isPayAmtZeroUponEntering ? charge.AmountEnd : (decimal)payCur.PayAmt);
						splitData=CreatePaySplit(charge.AccountEntryNum,splitAmt,payCur,textAmount,listSplitsCur,listAllCharges);
						listSplitsCur=splitData.ListSplitsCur;
						listAllCharges=splitData.ListAccountCharges;
						payCur=splitData.Payment;
					}
				}
				else {//group by none
					AccountEntry charge=listSelectedCharges[i].First();//Each item in listSelectedCharges should have exactly one AccountEntry.
					if(charge.AmountEnd==0 && !isShowAll) {
						continue;
					}
					decimal splitAmt=(isPayAmtZeroUponEntering ? charge.AmountEnd : (decimal)payCur.PayAmt);
					splitData=CreatePaySplit(charge.AccountEntryNum,splitAmt,payCur,textAmount,listSplitsCur,listAllCharges);
					listSplitsCur=splitData.ListSplitsCur;
					listAllCharges=splitData.ListAccountCharges;
					payCur=splitData.Payment;						
				}
			}
			return splitData??new PayResults { ListAccountCharges=listAllCharges, ListSplitsCur=listSplitsCur, Payment=payCur };
		}

		public static PayResults CreatePaySplit(long accountEntryNum,decimal payAmt,Payment payCur,decimal textAmount,List<PaySplit> listSplitsCur
			,List<AccountEntry> listCharges,bool isManual=false) 
		{
			//No remoting role check; no call to db.
			PayResults createdSplit=new PayResults();
			AccountEntry charge=listCharges.FirstOrDefault(x => x.AccountEntryNum==accountEntryNum);//get charge from passed list so it can be modified.
			createdSplit.ListSplitsCur=listSplitsCur;
			createdSplit.ListAccountCharges=listCharges;
			createdSplit.Payment=payCur;
			PaySplit split=new PaySplit();
			split.DatePay=DateTime.Today;
			if(charge.GetType()==typeof(Procedure)) {//Row selected is a Procedure.
				split.ProcNum=charge.PriKey;
				if(((Procedure)charge.Tag).ProcStatus==ProcStat.TP) {
					split.UnearnedType=PrefC.GetLong(PrefName.TpUnearnedType);
				}
			}
			else if(charge.GetType()==typeof(PayPlanCharge)) {//Row selected is a PayPlanCharge.
				PayPlanCharge ppChargeCur=(PayPlanCharge)charge.Tag;
				decimal payAmtCur;
				createdSplit.ListSplitsCur.AddRange(
					PaySplits.CreateSplitForPayPlan(payCur.PayNum,payCur.PayAmt,charge
					,PayPlanCharges.GetChargesForPayPlanChargeType(ppChargeCur.PayPlanNum,PayPlanChargeType.Credit),listCharges,payAmt,false,out payAmtCur));
				payCur.PayAmt=(double)payAmtCur;
				return createdSplit;
			}
			else if(charge.Tag.GetType()==typeof(Adjustment)) {//Row selected is an Adjustment.
				split.AdjNum=charge.PriKey;
			}
			else {//PaySplits and overpayment refunds.
				//Do nothing, nothing to link.
			}
			if(isManual) {
				split.SplitAmt=(double)payAmt;
				charge.AmountEnd-=payAmt;
			}
			else { 
				decimal chargeAmt=charge.AmountEnd;
				if(Math.Abs(chargeAmt)<Math.Abs(payAmt) || textAmount==0) {//Full payment of charge
					split.SplitAmt=(double)chargeAmt;
					charge.AmountEnd=0;//Reflect payment in underlying datastructure
				}
				else {//Partial payment of charge
					charge.AmountEnd-=payAmt;
					split.SplitAmt=(double)payAmt;
				}
			}
			if(PrefC.HasClinicsEnabled) {
				split.ClinicNum=charge.ClinicNum;
			}
			payCur.PayAmt-=split.SplitAmt;
			split.ProvNum=charge.ProvNum;
			split.PatNum=charge.PatNum;
			split.PayNum=payCur.PayNum;
			charge.SplitCollection.Add(split);
			createdSplit.ListSplitsCur.Add(split);
			createdSplit.Payment=payCur;
			return createdSplit;
		}

		///<summary>Checks if the amtEntered will result in the AccountEntry.Tag procedure to be overpaid. Returns false if AccountEntry.Tag is not a procedure.</summary>
		public static bool IsProcOverPaid(decimal amtEntered,AccountEntry accountEntry) {
			if(accountEntry.GetType()!=typeof(Procedure)) {
				return false;//AccountEntry is not a procedure. Return false.
			}
			//Only look for explicitly linked paysplits when calculating amount remaining. 
			decimal amtOverpay=accountEntry.AmountOriginal-(decimal)accountEntry.SplitCollection.Sum(x=>x.SplitAmt)-amtEntered;
			return amtOverpay.IsLessThanZero();
		}

		///<summary>Finds and updates matching charge's amount end and adds the current paySplit to the split collection for the charge.
		///Updates the underlying data structures when a manual split is created or edited.</summary>
		public static void UpdateForManualSplit(PaySplit paySplit,List<AccountEntry> listAccountCharges,long payPlanChargeNum=0,long prePaymentNum=0,bool isAllCharges=false) {
			//No remoting role check; no call to db
			//Find the charge row for this new split.
			//Locate a charge to apply the credit to, if a reasonable match exists.
			for(int i=0;i<listAccountCharges.Count;i++) {
				AccountEntry charge=listAccountCharges[i];
				if(charge.AmountEnd==0 && !isAllCharges) {
					continue;
				}
				bool isMatchFound=false;
				//New Split is for this proc (but not attached to a payplan)
				if(charge.GetType()==typeof(Procedure) && charge.PriKey==paySplit.ProcNum && paySplit.PayPlanNum==0) {
					isMatchFound=true;
				}
				else if(charge.GetType()==typeof(Adjustment) //New split is for this adjust
						&& paySplit.ProcNum==0 && paySplit.PayPlanNum==0 //Both being 0 is the only way we can tell it's for an Adj
						&& paySplit.DatePay==charge.Date
						&& paySplit.ProvNum==charge.ProvNum
						&& paySplit.PatNum==charge.PatNum
						&& paySplit.ClinicNum==charge.ClinicNum)
				{
					isMatchFound=true;
				}
				else if(charge.GetType()==typeof(PayPlanCharge) && charge.PriKey==payPlanChargeNum) {//split is for this payplancharge
					isMatchFound=true;
				}
				else if(charge.GetType()==typeof(PaySplit) && (charge.PriKey==prePaymentNum || charge.PriKey==paySplit.FSplitNum)) {//prepayment
					isMatchFound=true;
				}
				if(isMatchFound) {
					charge.AmountEnd=charge.AmountEnd-(decimal)paySplit.SplitAmt;
					PaySplit splitForCollection=paySplit.Copy();
					splitForCollection.AccountEntryAmtPaid=(decimal)paySplit.SplitAmt;
					charge.SplitCollection.Add(splitForCollection);
				}
				//If none of these, it's unattached to the best of our knowledge.
			}
		}

		/// <summary>Leave loadData blank for doRefreshData to be true and get a new copy of the objects.</summary>
		public static AutoSplit AutoSplitForPayment(List<long> listPatNums,long patCurNum,List<PaySplit> listSplitsCur,Payment payCur,
			List<AccountEntry> listEntriesLoading,bool isIncomeTxfr,bool isPatPrefer,LoadData loadData,bool doAutoSplit=true) 
		{
			ConstructResults constructResults=ConstructAndLinkChargeCredits(listPatNums,patCurNum,listSplitsCur,payCur
				,listEntriesLoading,isIncomeTxfr,isPatPrefer,loadData);
			AutoSplit autoSplit=AutoSplitForPayment(constructResults,doAutoSplit);
			return autoSplit;
		}

		public static AutoSplit AutoSplitForPayment(ConstructResults constructResults,bool doAutoSplit=true) 
		{
			AutoSplit autoSplitData=new AutoSplit();
			List<PayPlanCharge> listPayPlanCharges=constructResults.ListPayPlanCharges;
			autoSplitData.ListAccountCharges=constructResults.ListAccountCharges;
			autoSplitData.ListSplitsCur=constructResults.ListSplitsCur;
			autoSplitData.Payment=constructResults.Payment;
			//Create Auto-splits for the current payment to any remaining non-zero charges FIFO by date.
			if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.DontEnforce) {
				return autoSplitData;
			}
			if(!doAutoSplit) {
				return autoSplitData;
			}
			#region Auto-Split Current Payment
			double payAmt=autoSplitData.Payment.PayAmt;//keep track of the money that can be allocated for this payment.
			long payNum=autoSplitData.Payment.PayNum;
			//At this point we have a list of procs, positive adjustments, and payplancharges that require payment if the Amount>0.   
			//Create and associate new paysplits to their respective charge items.
			PaySplit split;
			for(int i=0;i<autoSplitData.ListAccountCharges.Count;i++) {
				if(payAmt==0) {
					break;
				}
				AccountEntry charge=autoSplitData.ListAccountCharges[i];
				if(Math.Round(charge.AmountEnd,3)<=0) {
					continue;//Skip charges which are already paid or negative.
				}
				if(payAmt<0 && Math.Round(charge.AmountEnd,3)>0) {//If they're different signs, don't make any guesses.  
					//This can happen if the user has less available than there are current splits for.
					//Remaining credits will always be all of one sign.
					return autoSplitData;//Will be empty
				}
				if(charge.GetType()==typeof(Procedure) && ((Procedure)charge.Tag).ProcStatus==ProcStat.TP) {
					continue;//do not auto split to TP procs. 
				}
				split=new PaySplit();
				split.IsNew=true;
				if(charge.GetType()==typeof(PayPlanCharge)) { //payments are allocated differently for payment plan charges
					//it's an autosplit, so pass in 0 for payAmt
					PayPlanVersions payPlanVer=(PayPlanVersions)PrefC.GetInt(PrefName.PayPlansVersion);
					if(payPlanVer!=PayPlanVersions.AgeCreditsAndDebits
						|| (payPlanVer==PayPlanVersions.AgeCreditsAndDebits && !PayPlans.GetOne(((PayPlanCharge)charge.Tag).PayPlanNum).IsClosed))
					{ 
						decimal payAmtCur;
						autoSplitData.ListAutoSplits.AddRange(PaySplits.CreateSplitForPayPlan(payNum,payAmt,charge,
							listPayPlanCharges.Where(x => x.ChargeType==PayPlanChargeType.Credit && x.PayPlanNum==((PayPlanCharge)charge.Tag).PayPlanNum).ToList(),
							autoSplitData.ListAccountCharges,0,true,out payAmtCur));
						payAmt=(double)payAmtCur;
					}
					continue;
				}
				else {
					if((double)Math.Abs(charge.AmountEnd)<Math.Abs(payAmt)) {//charge has "less" than the payment, use partial payment.
						split.SplitAmt=(double)charge.AmountEnd;
						payAmt=Math.Round(payAmt-(double)charge.AmountEnd,3);
						charge.AmountEnd=0;
					}
					else {//Use full payment
						split.SplitAmt=payAmt;
						charge.AmountEnd-=(decimal)payAmt;
						payAmt=0;
					}
				}
				split.DatePay=autoSplitData.Payment.PayDate;
				split.PatNum=charge.PatNum;
				split.ProvNum=charge.ProvNum;
				if(PrefC.HasClinicsEnabled) {
					split.ClinicNum=charge.ClinicNum;
				}
				if(charge.GetType()==typeof(Procedure)) {
					split.ProcNum=charge.PriKey;
				}
				else if(charge.GetType()==typeof(PayPlanCharge)) {
					split.PayPlanNum=((PayPlanCharge)charge.Tag).PayPlanNum;
				}
				else if(charge.GetType()==typeof(Adjustment)) {
					split.AdjNum=((Adjustment)charge.Tag).AdjNum;
				}
				split.PayNum=autoSplitData.Payment.PayNum;
				charge.SplitCollection.Add(split);
				autoSplitData.ListAutoSplits.Add(split);
			}
			if(autoSplitData.ListAutoSplits.Count==0 && autoSplitData.ListSplitsCur.Count==0 && payAmt!=0) {//Ensure at least 1 autosplit if payAmt was entered
				split=new PaySplit();
				split.SplitAmt=payAmt;
				payAmt=0;
				split.DatePay=autoSplitData.Payment.PayDate;
				split.PatNum=autoSplitData.Payment.PatNum;
				if(PrefC.IsODHQ) {
					split.ProvNum=7;//Jordan's ProvNum
				}
				else { 
					split.ProvNum=0;
				}
				split.UnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);//Use default unallocated type
				if(PrefC.HasClinicsEnabled) {
					split.ClinicNum=autoSplitData.Payment.ClinicNum;
				}
				split.PayNum=autoSplitData.Payment.PayNum;
				autoSplitData.ListAutoSplits.Add(split);
			}
			if(payAmt != 0) {//Create an unallocated split if there is any remaining payment amount.
				split=new PaySplit();
				split.SplitAmt=payAmt;
				payAmt=0;
				split.DatePay=autoSplitData.Payment.PayDate;
				split.PatNum=autoSplitData.Payment.PatNum;
				if(PrefC.IsODHQ) {
					split.ProvNum=7;//Jordan's ProvNum
				}
				else { 
					split.ProvNum=0;
				}
				split.UnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);//Use default unallocated type
				if(PrefC.HasClinicsEnabled) {
					split.ClinicNum=autoSplitData.Payment.ClinicNum;
				}
				split.PayNum=autoSplitData.Payment.PayNum;
				autoSplitData.ListAutoSplits.Add(split);
			}
			#endregion Auto-Split Current Payment
			autoSplitData.Payment.PayAmt=payAmt;
			return autoSplitData;
		}

		///<summary>Sorts similar to account module (AccountModules.cs, AccountLineComparer).Groups procedures together, then Adjustments, then everything else.</summary>
		private static int AccountEntrySort(AccountEntry x,AccountEntry y) {
			if(x.Date==y.Date) {
				if(x.GetType()==typeof(Procedure) && y.GetType()!=typeof(Procedure)) {
					return -1;
				}
				if(x.GetType()!=typeof(Procedure) && y.GetType()==typeof(Procedure)) {
					return 1;
				}
				if(x.GetType()==typeof(Adjustment) && y.GetType()!=typeof(Adjustment)) {
					return -1;
				}
				if(x.GetType()!=typeof(Adjustment) && y.GetType()==typeof(Adjustment)) {
					return 1;
				}
			}
			return x.Date.CompareTo(y.Date);
		}

		public static List<Procedure> GetProcsForAccountEntries(List<AccountEntry> listAccountEntry) {
			List<Procedure> listProcs=new List<Procedure>();
			List<AccountEntry> listAccountEntryProcs=listAccountEntry.FindAll(x => x.GetType()==typeof(Procedure));
			foreach(AccountEntry entry in listAccountEntryProcs) {
				listProcs.Add((Procedure)entry.Tag);
			}
			return listProcs;
		}

		public static List<AccountEntry> CreateAccountEntries(List<Procedure> listProcs) {
			//No remoting role check; no call to db
			List<AccountEntry> listAccountEntries=new List<AccountEntry>();
			foreach(Procedure proc in listProcs) {
				listAccountEntries.Add(new AccountEntry(proc));
			}
			return listAccountEntries;
		}

		#region Data Classes
		///<summary>The data needed to load FormPayment.</summary>
		[Serializable]
		public class LoadData {
			public Family SuperFam;
			public List<CreditCard> ListCreditCards;
			public XWebResponse XWebResponse;
			[XmlIgnore]
			public DataTable TableBalances;
			public List<PaySplit> ListSplits;//current list of splits associated to this payment
			public List<PaySplit> ListPaySplitAllocations;
			public Transaction Transaction;
			public List<PayPlan> ListValidPayPlans;
			public List<Patient> ListAssociatedPatients;
			public List<PaySplit> ListPrePaysForPayment;
			public ConstructChargesData ConstructChargesData;
			public List<Procedure> ListProcsForSplits;

			[XmlElement(nameof(TableBalances))]
			public string TableBalancesXml {
				get {
					if(TableBalances==null) {
						return null;
					}
					return XmlConverter.TableToXml(TableBalances);
				}
				set {
					if(value==null) {
						TableBalances=null;
						return;
					}
					TableBalances=XmlConverter.XmlToTable(value);
				}
			}
		}

		///<summary>The data needed to construct a list of charges for FormPayment.</summary>
		[Serializable]
		public class ConstructChargesData {
			///<summary>List from the db, completed for pat. Not list of pre-selected procs from acct. Will also contain TP procs if pref is set to ON.</summary>
			public List<Procedure> ListProcsCompleted=new List<Procedure>();
			public List<Adjustment> ListAdjustments=new List<Adjustment>();
			///<summary>Current list of all splits from database</summary>
			public List<PaySplit> ListPaySplits=new List<PaySplit>();
			///<summary>Stores the summed outstanding ins pay as totals (amounts and write offs) for the list of patnums</summary>
			public List<PayAsTotal> ListInsPayAsTotal=new List<PayAsTotal>();
			public List<PayPlan> ListPayPlans=new List<PayPlan>();
			public List<PaySplit> ListPaySplitsPayPlan=new List<PaySplit>();
			public List<PayPlanCharge> ListPayPlanCharges=new List<PayPlanCharge>();
			///<summary>Stores the list of claimprocs (not ins pay as totals) for the list of pat nums</summary>
			public List<ClaimProc> ListClaimProcsFiltered=new List<ClaimProc>();
		}

		///<summary>Data retrieved upon initialization. AutpSplit stores data retireved from going through list of charges, linking,and autosplitting.</summary>
		[Serializable]
		public class InitData {
			public AutoSplit AutoSplitData;
			public Dictionary<long,Patient> DictPats=new Dictionary<long,Patient>();
			public decimal SplitTotal;
		}

		/// <summary>Data resulting after making a payment.</summary>
		[Serializable]
		public class PayResults {
			public List<PaySplit> ListSplitsCur=new List<PaySplit>();
			public Payment Payment;
			public List<AccountEntry> ListAccountCharges=new List<AccountEntry>();
		}

		/// <summary>Data results after constructing list of charges and linking credits to them.</summary>
		[Serializable]
		public class ConstructResults {
			public List<PaySplit> ListSplitsCur=new List<PaySplit>();
			public Payment Payment;
			public List<AccountEntry> ListAccountCharges=new List<AccountEntry>();
			public List<PayPlanCharge> ListPayPlanCharges=new List<PayPlanCharge>();
		}
		
		/// <summary>Data after autosplitting. ListAutoSplits is separate from ListSplitsCur./// </summary>
		[Serializable]
		public class AutoSplit {
			public List<PaySplit> ListAutoSplits=new List<PaySplit>();
			public Payment Payment;
			public List<AccountEntry> ListAccountCharges=new List<AccountEntry>();
			public List<PaySplit> ListSplitsCur=new List<PaySplit>();
		}

		/// <summary> Data results after getting a just a list of charges.</summary>
		[Serializable]
		public class ConstructListChargesResult {
			public List<AccountEntry> ListAccountEntries=new List<AccountEntry>();
			public List<PayPlanCharge> ListPayPlanCharges=new List<PayPlanCharge>();
		}
		#endregion
	}
}
