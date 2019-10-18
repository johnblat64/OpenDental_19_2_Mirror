using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestsCore;
using OpenDentBusiness;
using System.Reflection;
using CodeBase;

namespace UnitTests.Payments_Tests {
	[TestClass]
	public class PaymentsTests:TestBase {
		#region Prepayment Test
		private List<PaySplit> _listFamPrePaySplits;
		private List<PaySplit> _listPosPrePay;
		private List<PaySplit> _listNegPrePay;
		#endregion

		[ClassInitialize]
		public static void SetUp(TestContext context) {

		}

		#region Payment Tests - Payment Edit
		///<summary>Make sure auto splits go to each procedure for proper amounts.</summary>
		[TestMethod]
		public void PaymentEdit_AutoSplitForPayment_SplitForPaymentLessThanTotalofProcs() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",40);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",40);
			Payment pay=PaymentT.MakePaymentNoSplits(pat.PatNum,50);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,pay,new List<long> { pat.PatNum },true,false);
			PaymentEdit.ConstructResults chargeResult=PaymentEdit.ConstructAndLinkChargeCredits(new List<long> {pat.PatNum },pat.PatNum
				,loadData.ConstructChargesData.ListPaySplits,pay,new List<AccountEntry>());
			PaymentEdit.AutoSplit autoSplit=PaymentEdit.AutoSplitForPayment(chargeResult);
			Assert.AreEqual(2,autoSplit.ListAutoSplits.Count);
			Assert.AreEqual(1,autoSplit.ListAutoSplits.Count(x => x.SplitAmt.IsEqual(40)));
			Assert.AreEqual(1,autoSplit.ListAutoSplits.Count(x => x.SplitAmt.IsEqual(10)));
			Assert.AreEqual(2,autoSplit.ListAutoSplits.Count(x => x.UnearnedType==0));
		}

		///<summary>Make sure there are no negative auto splits created for an overpaid procedure.</summary>
		[TestMethod]
		public void PaymentEdit_AutoSplitForPayment_NoNegativeAutoSplits() {
			long provNumA=ProviderT.CreateProvider("provA");
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",70);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",20);
			//make an overpayment for one of the procedures so it spills over.
			DateTime payDate=DateTime.Today;
			Payment pay=PaymentT.MakePayment(pat.PatNum,71,payDate,procNum:proc1.ProcNum);//pre-existing payment
			//attempt to make another payment. Auto splits should not suggest a negative split.
			Payment newPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,2,payDate,isNew:true,
				payType:Defs.GetDefsForCategory(DefCat.PaymentTypes,true)[0].DefNum);//current payment we're trying to make
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,newPayment,new List<long>() {pat.PatNum },true,false);
			PaymentEdit.ConstructChargesData chargeData=PaymentEdit.GetConstructChargesData(new List<long> {pat.PatNum },pat.PatNum,
				PaySplits.GetForPayment(pay.PayNum),pay.PayNum,false);
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(new List<long> {pat.PatNum },pat.PatNum
				,chargeData.ListPaySplits,newPayment,new List<AccountEntry>());
			PaymentEdit.AutoSplit autoSplits=PaymentEdit.AutoSplitForPayment(constructResults);
			Assert.AreEqual(0,autoSplits.ListAutoSplits.FindAll(x => x.SplitAmt<0).Count);//assert no negative auto splits were made.
			Assert.AreEqual(0,autoSplits.ListSplitsCur.FindAll(x => x.SplitAmt<0).Count);//auto splits not catching everything
			Assert.AreEqual(0,autoSplits.ListAutoSplits.Count(x => x.UnearnedType!=0));
		}

		///<summary>Make sure auto splits go to payment plan charges for proper amounts and aren't marked as unearned.</summary>
		[TestMethod]
		public void PaymentEdit_AutoSplitForPayment_NoExtraZeroSplitsForPayPlanCharges() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",75);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",75);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",75);
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,75,DateTime.Today.AddMonths(-4),0,new List<Procedure>() {proc1,proc2,proc3 });
			Payment pay=PaymentT.MakePaymentNoSplits(pat.PatNum,100,DateTime.Today,true,1);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,pay,new List<long>() {pat.PatNum},true,false);
			PaymentEdit.ConstructChargesData chargeData=PaymentEdit.GetConstructChargesData(new List<long>() {pat.PatNum},pat.PatNum
				,PaySplits.GetForPayment(pay.PayNum),pay.PayNum,false);
			PaymentEdit.ConstructResults results=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>() {pat.PatNum},pat.PatNum,chargeData.ListPaySplits
				,pay,new List<AccountEntry>());
			PaymentEdit.AutoSplit autoSplits=PaymentEdit.AutoSplitForPayment(results);
			//only two auto splits should exist. 1 covering the first whole payplancharge,and a second partial.
			Assert.AreEqual(2,autoSplits.ListAutoSplits.Count);
			Assert.AreEqual(1,autoSplits.ListAutoSplits.Count(x => x.SplitAmt.IsEqual(75)));
			Assert.AreEqual(1,autoSplits.ListAutoSplits.Count(x => x.SplitAmt.IsEqual(25)));
			Assert.AreEqual(2,autoSplits.ListAutoSplits.Count(x => x.UnearnedType==0));
		}

		///<summary>Make sure procedures are implicitly paid by unattached payment, and that a later procedure is paid by auto splits.</summary>
		[TestMethod]
		public void PaymentEdit_AutoSplitForPayments_ProceduresAndUnattachedPayments() {
			//Note, timing matters for this test. If all procedures are for today, the asserts will not be true.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			//make past procedures and pay them off with an unattached payment. Ok for prov, but do not link the procs.
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",55,DateTime.Today.AddDays(-2));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",65,DateTime.Today.AddDays(-2));
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",75,DateTime.Today.AddDays(-2));
			Payment unattachedPayment=PaymentT.MakePayment(pat.PatNum,195,DateTime.Today.AddDays(-1));//no other fields because unattached.
			//make new procedure
			Procedure newProc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today);
			Payment newPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,100);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,newPayment,new List<long>() {pat.PatNum},true,false);
			PaymentEdit.ConstructChargesData chargeData=PaymentEdit.GetConstructChargesData(new List<long>() {pat.PatNum},pat.PatNum
				,PaySplits.GetForPayment(newPayment.PayNum),newPayment.PayNum,false);
			PaymentEdit.ConstructResults results=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>() {pat.PatNum},pat.PatNum,chargeData.ListPaySplits
				,newPayment,new List<AccountEntry>());
			PaymentEdit.AutoSplit autoSplits=PaymentEdit.AutoSplitForPayment(results);
			Assert.AreEqual(1,autoSplits.ListAutoSplits.Count); //should only be one procedure that still needs to be paid off. 
			Assert.AreEqual(1,autoSplits.ListAutoSplits.Count(x => x.SplitAmt.IsEqual(100)));
			Assert.AreEqual(1,autoSplits.ListAutoSplits.Count(x => x.UnearnedType==0));
			Assert.AreEqual(1,autoSplits.ListAccountCharges.Count(x => x.AmountStart.IsGreaterThanZero()));
		}

		///<summary>Make sure auto split will be for procedure even when payment plan payment covers more than payment plan charge (but less than proc total)</summary>
		[TestMethod]
		public void PaymentEdit_AutoSplitForPayment_NoUnallocatedSplitsWhenPayPlanPaymentCoversMoreThanOneChargeInAPayment() {
			//Full split amount was being added to both charge's split collections during Explicit Linking
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("Prov"+MethodBase.GetCurrentMethod().Name);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",100,DateTime.Today.AddMonths(-2),provNum:provNum);
			//Payment Plan For The Procedure
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,25,DateTime.Today.AddMonths(-1),provNum,new List<Procedure> {proc});
			//Initial payment - important that it covers more than the first charge
			Payment firstPayment=PaymentT.MakePayment(pat.PatNum,31,DateTime.Today.AddDays(-1),payplan.PayPlanNum,provNum,proc.ProcNum,1);
			//Go to make another payment - 19 should be the current amount remaining on the payment plan. (25+25) - 31 = 19 
			Payment curPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,19,DateTime.Today,true,payType:1);//1 because not income txfr.
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,curPayment,new List<long>() {pat.PatNum },true,false);
			PaymentEdit.ConstructChargesData chargeData=PaymentEdit.GetConstructChargesData(new List<long>() {pat.PatNum },pat.PatNum,
				loadData.ConstructChargesData.ListPaySplits,curPayment.PayNum,false);
			PaymentEdit.ConstructResults results=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>() { pat.PatNum },pat.PatNum,chargeData.ListPaySplits,
				curPayment,new List<AccountEntry>());
			PaymentEdit.AutoSplit autoSplits=PaymentEdit.AutoSplitForPayment(results);
			Assert.AreEqual(1,autoSplits.ListAutoSplits.Count);
			Assert.AreEqual(1,autoSplits.ListAutoSplits.Count(x => x.SplitAmt.IsEqual(19)));
			Assert.AreEqual(1,autoSplits.ListAutoSplits.Count(x => x.UnearnedType==0));//make sure it's not unallocated.
		}
		
		///<summary>Make sure auto splits that are created are in the correct number and order (earliest proc paid first).</summary>
		[TestMethod]
		public void PaymentEdit_Init_CorrectlyOrderedAutoSplits() {//Legacy_TestFortyOne
			string suffix="41";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Procedure procedure1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",50,DateTime.Now.AddDays(-1));
			Procedure procedure2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",40,DateTime.Now.AddDays(-2));
			Procedure procedure3=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Now.AddDays(-3));
			Payment payment=PaymentT.MakePaymentNoSplits(patNum,150);
			Family famForPat=Patients.GetFamily(patNum);
			List<Patient> listFamForPat=famForPat.ListPats.ToList();
			PaymentEdit.InitData init=PaymentEdit.Init(listFamForPat,famForPat,new Family { },payment,new List<PaySplit>(),new List<AccountEntry>(),patNum);
			//Auto Splits will be in opposite order from least recent to most recent.
			Assert.AreEqual(3,init.AutoSplitData.ListAutoSplits.Count);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[0].SplitAmt!=60 || init.AutoSplitData.ListAutoSplits[0].ProcNum!=procedure3.ProcNum);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[1].SplitAmt!=40 || init.AutoSplitData.ListAutoSplits[1].ProcNum!=procedure2.ProcNum);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[2].SplitAmt!=50 || init.AutoSplitData.ListAutoSplits[2].ProcNum!=procedure1.ProcNum);
		}

		///<summary>Make sure auto splits are created in correct number and order with an existing payment already present.</summary>
		[TestMethod]
		public void PaymentEdit_Init_CorrectlyOrderedAutoSplitsWithExistingPayment() {//Legacy_TestFortyTwo
			string suffix="42";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Procedure procedure1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",40,DateTime.Now.AddDays(-1));
			Procedure procedure2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",60,DateTime.Now.AddDays(-2));
			Procedure procedure3=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",80,DateTime.Now.AddDays(-3));
			Payment payment1=PaymentT.MakePayment(patNum,110,DateTime.Now.AddDays(-2));
			Payment payment2=PaymentT.MakePaymentNoSplits(patNum,80,DateTime.Today);
			Family famForPat=Patients.GetFamily(patNum);
			List<Patient> listFamForPat=famForPat.ListPats.ToList();
			PaymentEdit.InitData init=PaymentEdit.Init(listFamForPat,famForPat,new Family { },payment2,new List<PaySplit>(),new List<AccountEntry>(),patNum);
			//Auto Splits will be in opposite order from least recent to most recent.
			//ListSplitsCur should contain three paysplits, one for procedure1 for 40, and one for procedure2 for 30,
			//and an unallocated split for 10 with the remainder on the payment (40+30+10=80).
			Assert.AreEqual(3,init.AutoSplitData.ListAutoSplits.Count);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[0].SplitAmt!=30 || init.AutoSplitData.ListAutoSplits[0].ProcNum!=procedure2.ProcNum);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[1].SplitAmt!=40 || init.AutoSplitData.ListAutoSplits[1].ProcNum!=procedure1.ProcNum);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[2].SplitAmt!=10 || init.AutoSplitData.ListAutoSplits[2].ProcNum!=0);
		}

		///<summary>Make sure if existing procedures are overpaid with an unallocated payment that an additional payment doesn't autosplit to the procs.</summary>
		[TestMethod]
		public void PaymentEdit_Init_AutoSplitOverAllocation() {//Legacy_TestFortyThree
			string suffix="43";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Procedure procedure1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",40,DateTime.Now.AddDays(-1));
			Procedure procedure2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",60,DateTime.Now.AddDays(-2));
			Procedure procedure3=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",80,DateTime.Now.AddDays(-3));
			Payment payment1=PaymentT.MakePayment(patNum,200,DateTime.Now.AddDays(-2));
			Payment payment2=PaymentT.MakePaymentNoSplits(patNum,50,DateTime.Today);
			Family famForPat=Patients.GetFamily(patNum);
			List<Patient> listFamForPat=famForPat.ListPats.ToList();
			PaymentEdit.InitData init=PaymentEdit.Init(listFamForPat,famForPat,new Family { },payment2,new List<PaySplit>(),new List<AccountEntry>(),patNum);
			//Auto Splits will be in opposite order from least recent to most recent.
			//ListSplitsCur should contain one paysplit worth 50 and not attached to any procedures.
			Assert.AreEqual(1,init.AutoSplitData.ListAutoSplits.Count);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[0].SplitAmt!=50 || init.AutoSplitData.ListAutoSplits[0].ProcNum!=0);
		}

		///<summary>Make sure if a payment is created fo rnegative amount that it makes no auto splits.</summary>
		[TestMethod]
		public void PaymentEdit_Init_AutoSplitForPaymentNegativePaymentAmount() {//Legacy_TestFortyFour
			string suffix="44";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Procedure procedure1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",40,DateTime.Now.AddDays(-1));
			Payment payment=PaymentT.MakePaymentNoSplits(patNum,-50,DateTime.Today);
			Family famForPat=Patients.GetFamily(patNum);
			List<Patient> listFamForPat=famForPat.ListPats.ToList();
			PaymentEdit.InitData init=PaymentEdit.Init(listFamForPat,famForPat,new Family { },payment,new List<PaySplit>(),new List<AccountEntry>(),patNum);
			//Auto Splits will be in opposite order from least recent to most recent.
			//ListSplitsCur should contain no paysplits since it doesn't make sense to create negative payments when there are outstanding charges.
			Assert.AreEqual(0,init.AutoSplitData.ListAutoSplits.Count);
		}

		///<summary>Make sure auto splits take into account unallocated adjustment, an overpayment on a procedure, 
		///and are attributed correctly to the remaining procedure with an unallocated split for the rest.</summary>
		[TestMethod]
		public void PaymentEdit_Init_AutoSplitWithAdjustmentAndExistingPayment() {//Legacy_TestFortyFive
			string suffix="45";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Procedure procedure1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",40,DateTime.Now.AddDays(-1));
			Procedure procedure2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",60,DateTime.Now.AddDays(-2));
			Procedure procedure3=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",80,DateTime.Now.AddDays(-3));
			Adjustment adjustment=AdjustmentT.MakeAdjustment(patNum,-40,procDate:DateTime.Now.AddDays(-2));
			Payment payment=PaymentT.MakePayment(patNum,100,DateTime.Now.AddDays(-2),procNum:procedure3.ProcNum);
			Payment payment2=PaymentT.MakePaymentNoSplits(patNum,50,DateTime.Today);
			Family famForPat=Patients.GetFamily(patNum);
			List<Patient> listFamForPat=famForPat.ListPats.ToList();
			PaymentEdit.InitData init=PaymentEdit.Init(listFamForPat,famForPat,new Family { },payment2,new List<PaySplit>(),new List<AccountEntry>(),patNum);
			//Auto Splits will be in opposite order from least recent to most recent.
			//ListSplitsCur should contain two paysplits, 40 attached to the D1110 and another for the remainder of 10, not attached to any procedure.
			Assert.AreEqual(2,init.AutoSplitData.ListAutoSplits.Count);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[0].SplitAmt!=40 || init.AutoSplitData.ListAutoSplits[0].ProcNum!=procedure1.ProcNum);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[1].SplitAmt!=10 || init.AutoSplitData.ListAutoSplits[1].ProcNum!=0);
		}

		///<summary>Make sure if there is a negative procedure and a negative payment amount that a new payment goes fully to unallocated.</summary>
		[TestMethod]
		public void PaymentEdit_Init_AutoSplitForPaymentNegativePaymentAmountNegProcedure() {//Legacy_TestFortySix
			string suffix="46";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Procedure procedure1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",-40,DateTime.Now.AddDays(-1));
			Payment payment=PaymentT.MakePaymentNoSplits(patNum,-50,DateTime.Today);
			Family famForPat=Patients.GetFamily(patNum);
			List<Patient> listFamForPat=famForPat.ListPats.ToList();
			PaymentEdit.InitData init=PaymentEdit.Init(listFamForPat,famForPat,new Family { },payment,new List<PaySplit>(),new List<AccountEntry>(),patNum);
			//Auto Splits will be in opposite order from least recent to most recent.
			//ListSplitsCur should contain one paysplit for the amount passed in that is unallocated.
			Assert.AreEqual(1,init.AutoSplitData.ListAutoSplits.Count);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[0].SplitAmt!=-50 || init.AutoSplitData.ListAutoSplits[0].ProcNum!=0);
		}

		///<summary>Make sure auto split suggestions go to the correct patients, for the correct amounts.</summary>
		[TestMethod]
		public void PaymentEdit_Init_AutoSplitProcedureGuarantor() {//Legacy_TestFortySeven
			string suffix="47";
			Patient pat=PatientT.CreatePatient(suffix);
			Patient patOld=PatientT.CreatePatient(suffix+"fam");
			Patient pat2=patOld.Copy();
			long patNum=pat.PatNum;
			pat2.Guarantor=patNum;
			Patients.Update(pat2,patOld);
			long patNum2=pat2.PatNum;
			Procedure procedure1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",50,DateTime.Now.AddDays(-1));
			Procedure procedure2=ProcedureT.CreateProcedure(pat2,"D0120",ProcStat.C,"",40,DateTime.Now.AddDays(-2));
			Procedure procedure3=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Now.AddDays(-3));
			Payment payment=PaymentT.MakePaymentNoSplits(patNum,150,DateTime.Today);
			Family famForPat=Patients.GetFamily(patNum);
			List<Patient> listFamForPat=famForPat.ListPats.ToList();
			PaymentEdit.InitData init=PaymentEdit.Init(listFamForPat,famForPat,new Family { },payment,new List<PaySplit>(),new List<AccountEntry>(),patNum);
			//Auto Splits will be in opposite order from least recent to most recent.
			Assert.AreEqual(3,init.AutoSplitData.ListAutoSplits.Count);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[0].SplitAmt!=60 || init.AutoSplitData.ListAutoSplits[0].ProcNum!=procedure3.ProcNum 
				|| init.AutoSplitData.ListAutoSplits[0].PatNum!=patNum);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[1].SplitAmt!=40 || init.AutoSplitData.ListAutoSplits[1].ProcNum!=procedure2.ProcNum 
				|| init.AutoSplitData.ListAutoSplits[1].PatNum!=patNum2);
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[2].SplitAmt!=50 || init.AutoSplitData.ListAutoSplits[2].ProcNum!=procedure1.ProcNum 
				|| init.AutoSplitData.ListAutoSplits[2].PatNum!=patNum);
		}

		///<summary>Make sure auto split suggestions take into account claim payments on procedures.</summary>
		[TestMethod]
		public void PaymentEdit_Init_AutoSplitWithClaimPayments() {//Legacy_TestFortyEight
			string suffix="48";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			InsPlan insPlan=InsPlanT.CreateInsPlan(CarrierT.CreateCarrier(suffix).CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patNum,insPlan.PlanNum);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,patNum,insSub.InsSubNum);
			Procedure procedure1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",50,DateTime.Now.AddDays(-1));
			Procedure procedure2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",40,DateTime.Now.AddDays(-2));
			Procedure procedure3=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Now.AddDays(-3));
			ClaimProcT.AddInsPaid(patNum,insPlan.PlanNum,procedure1.ProcNum,20,insSub.InsSubNum,0,0);
			ClaimProcT.AddInsPaid(patNum,insPlan.PlanNum,procedure2.ProcNum,5,insSub.InsSubNum,5,0);
			ClaimProcT.AddInsPaid(patNum,insPlan.PlanNum,procedure3.ProcNum,20,insSub.InsSubNum,0,10);
			Payment payment=PaymentT.MakePaymentNoSplits(patNum,150,DateTime.Today);
			Family famForPat=Patients.GetFamily(patNum);
			List<Patient> listFamForPat=famForPat.ListPats.ToList();
			PaymentEdit.InitData init=PaymentEdit.Init(listFamForPat,famForPat,new Family { },payment,new List<PaySplit>(),new List<AccountEntry>(),patNum);
			//Auto Splits will be in opposite order from least recent to most recent.
			//ListSplitsCur should contain four splits, 30, 35, and 30, then one unallocated for the remainder of the payment 55.
			Assert.AreEqual(4,init.AutoSplitData.ListAutoSplits.Count);
			//First auto split not not 30, not not procedure 3, and not not patNum
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[0].SplitAmt!=30 
				|| init.AutoSplitData.ListAutoSplits[0].ProcNum!=procedure3.ProcNum
				|| init.AutoSplitData.ListAutoSplits[0].PatNum!=patNum);
			//Second auto split not not 35, not not procedure 2, and not not patNum
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[1].SplitAmt!=35 
				|| init.AutoSplitData.ListAutoSplits[1].ProcNum!=procedure2.ProcNum
				|| init.AutoSplitData.ListAutoSplits[1].PatNum!=patNum);
			//Third auto split not not 30, not not procedure 1, and not not patNum
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[2].SplitAmt!=30 
				|| init.AutoSplitData.ListAutoSplits[2].ProcNum!=procedure1.ProcNum
				|| init.AutoSplitData.ListAutoSplits[2].PatNum!=patNum);
			//Fourth auto split not not 55, and not not procNum of 0
			Assert.IsFalse(init.AutoSplitData.ListAutoSplits[3].SplitAmt!=55
				|| init.AutoSplitData.ListAutoSplits[3].ProcNum!=0);
		}

		///<summary>Make sure that with a positive adjustment and the preference set to not prefer adjustments in FIFO logic that the first procedure is 
		///worth 55 at the end of auto splitting (was originally 75, and payment is for 20)</summary>
		[TestMethod]
		public void PaymentEdit_Init_FIFOWithPosAdjustment() {
			PrefT.UpdateInt(PrefName.AutoSplitLogic,(int)AutoSplitPreference.FIFO);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov1");
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",75,DateTime.Today.AddMonths(-1),provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",135,DateTime.Today.AddMonths(-1).AddDays(1),provNum:provNum);
			Adjustment adjustment=AdjustmentT.MakeAdjustment(pat.PatNum,20,DateTime.Today.AddDays(-15),provNum:provNum);
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,20);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,payCur,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },payCur
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum);
			//Verify the logic pays starts to pay off the first procedure
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.Count(x => x.Tag.GetType()==typeof(Procedure) && x.AmountEnd==55));
		}

		///<summary>Make sure that with a positive adjustment and the preference set to prefer adjustments in FIFO logic that the first item paid is 
		///an adjustment and that the adjustment (worth 20) is fully paid (by payment worth 20)</summary>
		[TestMethod]
		public void PaymentEdit_Init_AdjustmentPreferWithPosAdjustment() {
			PrefT.UpdateInt(PrefName.AutoSplitLogic,(int)AutoSplitPreference.Adjustments);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov1");
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",75,DateTime.Today.AddMonths(-1),provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",135,DateTime.Today.AddMonths(-1).AddDays(1),provNum:provNum);
			Adjustment adjustment=AdjustmentT.MakeAdjustment(pat.PatNum,20,DateTime.Today.AddDays(-15),provNum:provNum);
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,20);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,payCur,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },payCur
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum);
			//Verify the logic chooses to pay off the adjustment first
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.Count(x => x.Tag.GetType()==typeof(Adjustment) && x.AmountEnd==0));
		}

		///<summary>Make sure that if there are two procs and a payplan made with no procs attached that has a start date 4 months ago, 
		///the payment logic returns 6 owed charges, two of which are the procs and 4 of which are payplan charges.</summary>
		[TestMethod]
		public void PaymentEdit_Init_PayPlanChargesWithUnattachedCredits() {
			//new payplan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",135,DateTime.Today.AddMonths(-4));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Today.AddMonths(-4));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),0,totalAmt:195);
			//Go to make a payment for the charges due
			Payment pay=PaymentT.MakePaymentNoSplits(pat.PatNum,60,DateTime.Today);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,pay,new List<long> {pat.PatNum},true,false);
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(new List<long> {pat.PatNum },pat.PatNum
				,loadData.ConstructChargesData.ListPaySplits,pay,new List<AccountEntry>());
			Assert.AreEqual(6,constructResults.ListAccountCharges.Count);//2 procedures and 4 months of charges since unattached credits.
			Assert.AreEqual(2,constructResults.ListAccountCharges.Count(x => x.Tag.GetType()==typeof(Procedure)));
			Assert.AreEqual(4,constructResults.ListAccountCharges.Count(x => x.Tag.GetType()==typeof(PayPlanCharge)));
		}

		///<summary>Make sure that if there are two procs both attached to a payplan that has a start date of 4 months ago, that the payment logic
		///returns 4 owed charges, all four of which are payplan charges.</summary>
		[TestMethod]
		public void PaymentEdit_Init_PayPlanChargesWithAttachedCredits() {
			//new payplan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",135,DateTime.Today.AddMonths(-4));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Today.AddMonths(-4));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),0,new List<Procedure>() {proc1,proc2});
			//Go to make a payment for the charges that are due
			Payment pay=PaymentT.MakePaymentNoSplits(pat.PatNum,60,DateTime.Today);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,pay,new List<long>{pat.PatNum},true,false);
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(new List<long> {pat.PatNum },pat.PatNum
				,loadData.ConstructChargesData.ListPaySplits,pay,new List<AccountEntry>());
			Assert.AreEqual(4,constructResults.ListAccountCharges.FindAll(x => x.AmountStart>0).Count);//Procs shouldn't show - only the 4 pay plan charges
			Assert.AreEqual(4,constructResults.ListAccountCharges.Count(x => x.Tag.GetType()==typeof(PayPlanCharge)));
		}

		///<summary>Make sure that if there are two procedures, a payment plan that has unattached credits (started 4 months ago), and two payments on the payment plan,
		///that our payment logic returns 4 charges that owe money, there are two charges that are procedures, and 4 charges that are payplan charges.</summary>
		[TestMethod]
		public void PaymentEdit_Init_ChargesWithUnattachedPayPlanCreditsWithPreviousPayments() {
			//new payplan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long prov=ProviderT.CreateProvider("ProvA");
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",135,DateTime.Today.AddMonths(-4),provNum:prov);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Today.AddMonths(-4).AddDays(1),provNum:prov);
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),prov,totalAmt:195);//totalAmt since unattached credits
			//Make initial payments.
			PaymentT.MakePayment(pat.PatNum,30,DateTime.Today.AddMonths(-2),payplan.PayPlanNum,prov,0,1);
			PaymentT.MakePayment(pat.PatNum,30,DateTime.Today.AddMonths(-1),payplan.PayPlanNum,prov,0,1);
			//Go to make another payment. 2 pay plan charges should have been "removed" (amtStart to 0) from being paid. 
			Payment pay=PaymentT.MakePaymentNoSplits(pat.PatNum,30,DateTime.Today);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,pay,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },pay
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum);
			//2 procs and 2 pp charges
			Assert.AreEqual(4,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.AmountStart>0).Count);
			Assert.AreEqual(2,initData.AutoSplitData.ListAccountCharges.Count(x => x.Tag.GetType()==typeof(Procedure)));
			Assert.AreEqual(4,initData.AutoSplitData.ListAccountCharges.Count(x => x.Tag.GetType()==typeof(PayPlanCharge)));
		}

		///<summary>Make sure that a new payment goes to a procedure instead of a payplan due to the payplan being paid.</summary>
		[TestMethod]
		public void PaymentEdit_Init_PayPlansWithAttachedCreditsAndUnattachedProcedureOnPaySplits() {
			//Test create for instance when implicit linking was taking payment plan splits into account twice. In this test scenario it would lead
			//to the resulting procedure having a starting amount of 25 upon coming into the payment window when it should have been 75, the full amount.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50);
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,50,DateTime.Today.AddMonths(-1),0,new List<Procedure>() {proc1 });
			Payment paymentForPayPlan=PaymentT.MakePayment(pat.PatNum,50,DateTime.Today,payplan.PayPlanNum);//specifically testing when not attached to proc
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",75);
			Payment paymentForNewProc=PaymentT.MakePaymentNoSplits(pat.PatNum,75,DateTime.Today,true,1);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,paymentForNewProc,new List<long> {pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },paymentForNewProc
				,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum);
			//Verify the starting amount of the new procedure is correct and an autosplit exists for only the procedure, no unallocated.
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.AmountStart==75).Count);
			Assert.AreEqual(1,initData.AutoSplitData.ListAutoSplits.Count);
		}

		///<summary>Make sure if an adjustment is attached to a procedure that it affects the procedure's amount.</summary>
		[TestMethod]
		public void PaymentEdit_Init_AttachedAdjustment() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov1");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",135,DateTime.Today.AddMonths(-1).AddDays(1),provNum:provNum);
			Adjustment adjustment=AdjustmentT.MakeAdjustment(pat.PatNum,20,DateTime.Today.AddDays(-15),provNum:provNum,procNum:proc.ProcNum);
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,20);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,payCur,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },payCur
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum);
			//Verify there is only one charge (the procedure's charge + the adjustment for the amount original)
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.Count);
			Assert.AreEqual(typeof(Procedure),initData.AutoSplitData.ListAccountCharges[0].Tag.GetType());
			Assert.AreEqual(135,initData.AutoSplitData.ListAccountCharges[0].AmountOriginal);
		}

		///<summary>Make sure that adjustments attached to a procedure affects its amount and unallocated splits reduce the procedure's end amount to 0.</summary>
		[TestMethod]
		public void PaymentEdit_Init_UnattachedPaymentsAndAttachedAdjustments() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov1");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",135,DateTime.Today.AddMonths(-1).AddDays(1),provNum:provNum);
			Adjustment adjustment=AdjustmentT.MakeAdjustment(pat.PatNum,20,DateTime.Today.AddDays(-15),provNum:provNum,procNum:proc.ProcNum);
			Payment existingPayment1=PaymentT.MakePayment(pat.PatNum,35,DateTime.Today.AddDays(-1));//no prov or proc because it's unattached.
			Payment existingPayment2=PaymentT.MakePayment(pat.PatNum,20,DateTime.Today.AddDays(-1));
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,100);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,payCur,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },payCur
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum);
			//Verify there is only one charge (the procedure's charge + the adjustment for the amount original)
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.Count);
			Assert.AreEqual(typeof(Procedure),initData.AutoSplitData.ListAccountCharges[0].Tag.GetType());
			Assert.AreEqual(0,initData.AutoSplitData.ListAccountCharges[0].AmountEnd);
		}

		///<summary>Make sure that if a procedure isn't paid explicitly that income transfer mode displays all things separately (procedures and paysplit).</summary>
		[TestMethod]
		public void PaymentEdit_Init_IncomeTransferWhenIncomeIncorrectlyAllocatedToOneProvider() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum1=ProviderT.CreateProvider("prov1");
			long provNum2=ProviderT.CreateProvider("prov2");
			Procedure procByProv1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",100,DateTime.Today.AddMonths(-1),provNum:provNum1);
			Procedure procByProv2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",50,DateTime.Today.AddMonths(-1),provNum:provNum2);
			Payment payAllForOneProv=PaymentT.MakePayment(pat.PatNum,150,DateTime.Today.AddDays(-1),provNum:provNum1,payType:1);//make entire payment to prov1
			//make an income transfer and see if it catches the over and underallocations
			Payment incomeTransfer=PaymentT.MakePaymentNoSplits(pat.PatNum,0,isNew:true);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,incomeTransfer,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },incomeTransfer
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum,isIncomeTxfr:true);
			//Assert there the appropriate amounts going to the correct providers.
			Assert.AreEqual(2,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.Tag.GetType()==typeof(Procedure)).Count);
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.Tag.GetType()==typeof(PaySplit)).Count);
			//FormPayment.FillGridCharges will sum up the charges depending on what grouping is selected. Here we are just going to validate the output of
			//PaymentEdit.Init is as it should be.
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.AmountStart==-150 && x.ProvNum==provNum1).Count);
		}

		///<summary>Make sure in income transfer mode that if nothing is explicitly attached, all charges/credits show in the list.</summary>
		[TestMethod]
		public void PaymentEdit_Init_IncomeTransferWhenAdjustmentsIncorrectlyAllocatedToOneProvider() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum1=ProviderT.CreateProvider("prov1");
			long provNum2=ProviderT.CreateProvider("prov2");
			Adjustment procForProv1=AdjustmentT.MakeAdjustment(pat.PatNum,100,DateTime.Today.AddMonths(-1),provNum:provNum1);
			Adjustment procForProv2=AdjustmentT.MakeAdjustment(pat.PatNum,150,DateTime.Today.AddMonths(-1),provNum:provNum2);
			Payment payAllForOneProv=PaymentT.MakePayment(pat.PatNum,250,DateTime.Today.AddDays(-1),provNum:provNum1,payType:1);//make entire payment to prov1
			//make an income transfer and see if it catches the over and underallocations
			Payment incomeTransfer=PaymentT.MakePaymentNoSplits(pat.PatNum,0,isNew:true);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,incomeTransfer,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },incomeTransfer
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum,isIncomeTxfr:true);
			//Assert there the appropriate amounts going to the correct providers.
			Assert.AreEqual(2,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.Tag.GetType()==typeof(Adjustment)).Count);
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.Tag.GetType()==typeof(PaySplit)).Count);
			//FormPayment.FillGridCharges will sum up the charges depending on what grouping is selected. Here we are just going to validate the output of
			//PaymentEdit.Init is as it should be.
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.AmountStart==-250 && x.ProvNum==provNum1).Count);
		}

		[TestMethod]
		public void PaymentEdit_Init_IncomeTransferPreDateTransfer() {
			//Create procedure for date X, create payment (unallocated) for date X.  
			//Create transfer to transfer from unallocated to procedure, but on date X-Y, where Y is some time.  
			//There should be no charges for transfer.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum1=ProviderT.CreateProvider("prov1");
			Procedure procByProv1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",100,DateTime.Today.AddMonths(-1),provNum:provNum1);
			//Create an income transfer to transfer from the total payment to the proc, but make it for prior to proc/payment date
			PaySplit prePay=PaySplitT.CreatePrepayment(pat.PatNum,100,DateTime.Today.AddMonths(-1),provNum1,0);
			Payment incomeTransfer=PaymentT.MakePaymentNoSplits(pat.PatNum,0,isNew:true,payDate:DateTime.Today.AddMonths(-2));
			PaySplit negSplit=PaySplitT.CreateSplit(0,pat.PatNum,incomeTransfer.PayNum,0,DateTime.Today.AddMonths(-2),0,provNum1,-100,0,prePay.SplitNum);//negative transfer
			PaySplitT.CreateSplit(0,pat.PatNum,incomeTransfer.PayNum,0,DateTime.Today.AddMonths(-2),procByProv1.ProcNum,procByProv1.ProvNum,100,0,negSplit.SplitNum);//Positive allocation
			Payment incomeTransfer2=PaymentT.MakePaymentNoSplits(pat.PatNum,0,isNew:true,payDate:DateTime.Today);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,incomeTransfer2,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },incomeTransfer2
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum,isIncomeTxfr:true,loadData:loadData);
			//Everything should be paid off.
			Assert.AreEqual(0,initData.AutoSplitData.ListAutoSplits.Count);
		}

		///<summary>Make sure that payment logic takes into account base units.</summary>
		[TestMethod]
		public void PaymentEdit_Init_ProcedureWithBaseUnits() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure procWithBaseUnits=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",100,baseUnits:1);//1 proc fee is $100, so total should be $200.
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,200);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,payCur,new List<long> {pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },payCur,
				loadData.ListSplits,new List<AccountEntry> {new AccountEntry(procWithBaseUnits) },pat.PatNum);
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.AmountOriginal==200 && x.AmountStart==200 && x.AmountEnd==0).Count);
		}

		///<summary>Make sure that two procedures on a payment plan, with payments attached to the plan, that there are two charges requiring payment still.</summary>
		[TestMethod]
		public void PaymentEdit_ConstructAndLinkChargeCredits_ChargesWithAttachedPayPlanCreditsWithPreviousPayments() {
			//new payplan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",135,DateTime.Today.AddMonths(-4));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Today.AddMonths(-4));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),0,new List<Procedure>() {proc1,proc2});
			//Procedures's amount start should now be 0 from being attached. Make initial payments.
			PaymentT.MakePayment(pat.PatNum,30,DateTime.Today.AddMonths(-2),payplan.PayPlanNum,procNum:proc1.ProcNum);
			PaymentT.MakePayment(pat.PatNum,30,DateTime.Today.AddMonths(-1),payplan.PayPlanNum,procNum:proc1.ProcNum);
			//2 pay plan charges should have been removed from being paid. Make a new payment. 
			Payment pay=PaymentT.MakePaymentNoSplits(pat.PatNum,30,DateTime.Today,isNew:true,payType:1);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,pay,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },pay
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum);
			//should only see 2 pay plan charges that have not been paid, along with 2 pay plan charges that have been paid. 
			Assert.AreEqual(2,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.AmountStart>0).Count);
			Assert.AreEqual(4,initData.AutoSplitData.ListAccountCharges.Count(x => x.Tag.GetType()==typeof(PayPlanCharge)));
		}

		///<summary>Make sure that if there is a positive adjustment (and only that) that auto split logic will make a split to it.</summary>
		[TestMethod]
		public void PaymentEdit_MakePayment_Adjustment() {
			//equivalent of being in the payment window and then hitting the 'pay' button to move a charge over from outstanding to current list of splits.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Adjustment adjustment=AdjustmentT.MakeAdjustment(pat.PatNum,100,DateTime.Today.AddDays(-1));
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,100,DateTime.Today,true,1);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,payment,new List<long> {pat.PatNum },true,false);
			PaymentEdit.ConstructChargesData chargeData=PaymentEdit.GetConstructChargesData(new List<long> {pat.PatNum },pat.PatNum,
				PaySplits.GetForPayment(payment.PayNum),payment.PayNum,false);
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(new List<long> {pat.PatNum },pat.PatNum
				,chargeData.ListPaySplits,payment,new List<AccountEntry>());
			List<List<AccountEntry>> listListAE=new List<List<AccountEntry>>();
			listListAE.Add(constructResults.ListAccountCharges);
			PaymentEdit.PayResults results=PaymentEdit.MakePayment(listListAE,PaySplits.GetForPayment(payment.PayNum),false,payment,0,false,100,
				constructResults.ListAccountCharges);
			Assert.AreEqual(0,results.ListAccountCharges.FindAll(x => x.AmountEnd>0).Count);//no more splits should need to be paid off.
			Assert.AreEqual(1,results.ListSplitsCur.FindAll(x => x.SplitAmt==100).Count);//Adjustment should now be paid in full. 
		}

		///<summary>Make sure that if there is a 100 prepayment and a procedure for 50, that allocating unearned allocates properly and unearned amount total
		///is correct.</summary>
		[TestMethod]
		public void PaymentEdit_AllocateUnearned_LinkToOriginalPrepayment() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			//create prepayment of $100
			long provNum=ProviderT.CreateProvider("SG");
			Clinic clinic1=ClinicT.CreateClinic("Clinic1");
			Family fam=Patients.GetFamily(pat.PatNum);
			//create original prepayment.
			PaySplit prePay=PaySplitT.CreatePrepayment(pat.PatNum,100,DateTime.Today.AddDays(-1),provNum,clinic1.ClinicNum);
			//complete a procedure
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",50,provNum:provNum);
			//Setup to run the PaymentEdit.AllocateUnearned
			List<PaySplit> listPaySplits=new List<PaySplit>();
			List<PaySplit> listFamPrePaySplits=PaySplits.GetPrepayForFam(fam);
			//Unearned amount should be $100.
			double unearnedAmt=(double)PaySplits.GetUnearnedForFam(fam,listFamPrePaySplits);
			Assert.AreEqual(100,unearnedAmt);
			//Create the payment we will use to allocate some of the $100 prepayment.
			Payment pay=PaymentT.MakePaymentForPrepayment(pat,clinic1);
			//Run the AllocateUnearned method. This a list of paysplitAssociated.
			//The ref list of paysplits should also have the new paysplits that are associated to the original prepayment.
			List<PaySplits.PaySplitAssociated> listPaySplitAssociated=PaymentEdit.AllocateUnearned(new List<AccountEntry> { new AccountEntry(proc1) },ref listPaySplits,pay,unearnedAmt,fam);
			//Insert the paysplits and link the prepayment paysplits. This is similar to what is done when a user creates a payment from FormPayment.cs.
			PaySplitT.InsertPrepaymentSplits(listPaySplits,listPaySplitAssociated);
			//The ref list of paysplits should have the correct allocated prepayment amount. 
			Assert.AreEqual(-50,listPaySplits.Where(x => x.UnearnedType!=0).Sum(x => x.SplitAmt));
			//Create new procedure
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,provNum:provNum);
			//Now do what we just did again for a new procedure. The unallocated amount should be $50.
			listFamPrePaySplits=PaySplits.GetPrepayForFam(fam);
			unearnedAmt=(double)PaySplits.GetUnearnedForFam(fam,listFamPrePaySplits);
			Assert.AreEqual(50,unearnedAmt);
			List<PaySplit> listPaySplitsUnearned=new List<PaySplit>();
			pay=PaymentT.MakePaymentForPrepayment(pat,clinic1);
			List<PaySplits.PaySplitAssociated> retVal=PaymentEdit.AllocateUnearned(new List<AccountEntry> { new AccountEntry(proc2) },ref listPaySplitsUnearned,pay,unearnedAmt,fam);
			Assert.AreEqual(2,retVal.Count);
			Assert.AreEqual(-50,listPaySplitsUnearned.Where(x => x.UnearnedType!=0).Sum(x => x.SplitAmt));
		}

		///<summary>Make sure that when a prepayment is allocated incorrectly (not linked) that it will allocate the correct partial amount later.</summary>
		[TestMethod]
		public void PaymentEdit_AllocateUnearned_NoLinkToOriginalPrepayment() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			//create prepayment of $100
			long provNum=ProviderT.CreateProvider("SG");
			Clinic clinic1=ClinicT.CreateClinic("Clinic1");
			//create original prepayment.
			PaySplit prePay=PaySplitT.CreatePrepayment(pat.PatNum,100,DateTime.Today.AddDays(-1),provNum,clinic1.ClinicNum);
			//complete a procedure
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",50,provNum:provNum);
			//Manually allocate prepayment without linking to the original prepayment.
			//We want to do it manually so we don't link this to the orginal prepayment correctly.
			//Not linking correctly will test that the AllocateUnearned method is implicitly linking prepayments correctly.
			PaySplitT.CreatePaySplitsForPrepayment(proc1,50,prov: provNum,clinic: clinic1);
			//Create new procedure
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,provNum:provNum);
			//test the PaymentEdit.AllocateUnearned() method.
			Family fam=Patients.GetFamily(pat.PatNum);
			List<PaySplit> listFamPrePaySplits=PaySplits.GetPrepayForFam(fam);
			//Should be $100
			double unearnedAmt=(double)PaySplits.GetUnearnedForFam(fam,listFamPrePaySplits);
			Assert.AreEqual(100,unearnedAmt);
			List<PaySplit> listPaySplitsUnearned=new List<PaySplit>();
			Payment pay=PaymentT.MakePaymentForPrepayment(pat,clinic1);
			List<PaySplits.PaySplitAssociated> retVal=PaymentEdit.AllocateUnearned(new List<AccountEntry> { new AccountEntry(proc2) },ref listPaySplitsUnearned,pay,unearnedAmt,fam);
			Assert.AreEqual(2,retVal.Count);
			//After running the AllocateUnearned, we should implicitly link the incorrect prepayment made when we call CreatePaySplitsForPrepayment above.
			Assert.AreEqual(-50,listPaySplitsUnearned.Where(x => x.UnearnedType!=0).Sum(x => x.SplitAmt));
		}

		///<summary>Various tests for allocating unearned under different circumstances (linked and unlinked splits, matching or mismatching pat/prov/clinic)</summary>
		[TestMethod]
		public void PaymentEdit_ImplicitlyLinkPrepaymentsHelper() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Patient patFam=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Patient patFamOld=patFam.Copy();
			patFam.Guarantor=pat.PatNum;
			Patients.Update(patFam,patFamOld);
			//create prepayment of $100
			long provNum=ProviderT.CreateProvider("SG");
			Clinic clinic1=ClinicT.CreateClinic("Clinic1");
			//create original prepayment.
			PaySplit prePay=PaySplitT.CreatePrepayment(pat.PatNum,100,DateTime.Today.AddDays(-1),provNum,clinic1.ClinicNum);
			//complete a procedure
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",50,provNum:provNum);
			//Manually allocate prepayment without linking to the original prepayment.
			List<PaySplit> listPaySplits=PaySplitT.CreatePaySplitsForPrepayment(proc1,50,prov:provNum,clinic:clinic1);
			ResetPrepayments(pat);
			long nonMatch=100000;
			//test the PaymentEdit.AllocateUnearned() method.
			double unearnedAmt=(double)PaySplits.GetUnearnedForFam(Patients.GetFamily(pat.PatNum),_listFamPrePaySplits);
			//Logic check PatNum - match, ProvNum - match, ClinicNum - match
			double retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumMatch:true,isClinicNumMatch:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - match, ProvNum - match, ClinicNum - zero
			ResetPrepayments(pat);
			//update the clinicnum to 0
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,pat.PatNum,provNum,0,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumMatch:true,isClinicNumZero:true);
			Assert.AreEqual(50,retVal);
			//previous Test one should be $100
			ResetPrepayments(pat);
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumMatch:true,isClinicNumMatch:true);
			Assert.AreEqual(100,retVal);
			//Logic check PatNum - match, ProvNum - match, ClinicNum - non zero & non match
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,pat.PatNum,provNum,100000,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch: true,isProvNumMatch: true,isClinicNonZeroNonMatch: true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - match, ProvNum - zero, ClinicNum - match
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,pat.PatNum,0,clinic1.ClinicNum,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch: true,isProvNumZero:true,isClinicNumMatch:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - match, ProvNum - zero, ClinicNum - zero
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,pat.PatNum,0,0,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumZero:true,isClinicNumZero:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - match, ProvNum - zero, ClinicNum - non zero & non match 
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,pat.PatNum,0,nonMatch,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumZero:true,isClinicNonZeroNonMatch:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - match, ProvNum - non zero & non match, ClinicNum - match 
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,pat.PatNum,nonMatch,clinic1.ClinicNum,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch:true,isProvNonZeroNonMatch:true,isClinicNumMatch:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - match, ProvNum - non zero & non match, ClinicNum - zero
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,pat.PatNum,nonMatch,0,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch:true,isProvNonZeroNonMatch:true,isClinicNumZero:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - match, ProvNum - non zero & non match, ClinicNum - non zero & non match
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,pat.PatNum,nonMatch,nonMatch,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch:true,isProvNonZeroNonMatch:true,isClinicNonZeroNonMatch:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - other family members, ProvNum - match, ClinicNum - match
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,patFam.PatNum,provNum,clinic1.ClinicNum,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumMatch:true,isClinicNumMatch:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - other family members, ProvNum - match, ClinicNum - zero
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,patFam.PatNum,provNum,0,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumMatch:true,isClinicNumZero:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - other family members, ProvNum - match, ClinicNum - non zero & non match
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,patFam.PatNum,provNum,nonMatch,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumMatch:true,isClinicNonZeroNonMatch:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - other family members, ProvNum - zero, ClinicNum - match
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,patFam.PatNum,0,clinic1.ClinicNum,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumZero:true,isClinicNumMatch:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - other family members, ProvNum - zero, ClinicNum - zero
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,patFam.PatNum,0,0,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumZero:true,isClinicNumZero:true);
			Assert.AreEqual(50,retVal);
			//Logic checkPatNum - other family members, ProvNum - zero, ClinicNum - non zero & non match
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,patFam.PatNum,0,nonMatch,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isFamMatch:true,isProvNumZero:true,isClinicNonZeroNonMatch:true);
			Assert.AreEqual(50,retVal);
			//Old test from above
			ResetPrepayments(pat);
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isPatMatch:true,isProvNumMatch:true,isClinicNumMatch:true);
			Assert.AreEqual(100,retVal);
			//Logic checkPatNum - other family members, ProvNum - non zero & non match, ClinicNum - match
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,patFam.PatNum,nonMatch,clinic1.ClinicNum,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isFamMatch:true,isProvNonZeroNonMatch:true,isClinicNumMatch:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - other family members, ProvNum - non zero & non match, ClinicNum - zero
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,patFam.PatNum,nonMatch,0,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isFamMatch:true,isProvNonZeroNonMatch:true,isClinicNumZero:true);
			Assert.AreEqual(50,retVal);
			//Logic check PatNum - other family members, ProvNum - non zero & non match, ClinicNum - non zero & non match
			ResetPrepayments(pat);
			_listFamPrePaySplits.ForEach(x => UpdatePaySplitHelper(x,patFam.PatNum,nonMatch,nonMatch,_listNegPrePay.First().PayNum));
			retVal=PaymentEdit.ImplicitlyLinkPrepaymentsHelper(_listPosPrePay,_listNegPrePay,unearnedAmt,isFamMatch:true,isProvNonZeroNonMatch:true,isClinicNonZeroNonMatch:true);
			Assert.AreEqual(50,retVal);
		}

		///<summary>Make sure that when a payment plan is closed out but partially paid that procedures still calculate as owing correct amount</summary>
		[TestMethod]
		public void PaymentEdit_ExplicitlyLinkCredits_ShowCorrectAmountNeedingPaymentWhenPaymentPlanV2IsClosedAndPartiallyPaid() {
			//Explicitly Link Credits is the method that will contain the method being tested, but test will call Init to run through the whole gambit.
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",100,DateTime.Today.AddMonths(-4));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),0,new List<Procedure>() {proc});
			PaymentT.MakePayment(pat.PatNum,30,DateTime.Today.AddMonths(-2),payplan.PayPlanNum,procNum:proc.ProcNum);
			List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			listCharges.Add(PayPlanEdit.CloseOutPatPayPlan(listCharges,payplan,DateTime.Today));
			listCharges.RemoveAll(x => x.ChargeDate > DateTime.Today);
			payplan.IsClosed=true;
			PayPlans.Update(payplan);
			PayPlanCharges.Sync(listCharges,payplan.PayPlanNum);
			Payment currentPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,60,DateTime.Today,true,1);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,currentPayment,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },currentPayment
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum);
			//only one procedure should come up as needing payment
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.AmountStart>0 && x.GetType()==typeof(Procedure)).Count);
			//procedure amount start should be reduced by the amount of the previous payments for the procedure. 
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.AmountStart==70).Count);
		}

		///<summary>Make sure that if a payment plan has a payment plan adjustment that the procedure owes the correct amount.</summary>
		[TestMethod]
		public void PaymentEdit_Init_ShowCorrectAmountNeedingPaymentOnChargeWhenPaymentPlanAdjustmentsExistForThePayPlanCharge() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",100,DateTime.Today.AddMonths(-3));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,100,DateTime.Today.AddMonths(-3),0,new List<Procedure> {proc1 });
			List<PayPlanCharge> listChargesAndCredits=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			listChargesAndCredits=PayPlanEdit.CreatePayPlanAdjustments(-60,listChargesAndCredits,0);//create a 60 adjustment for the $100 charge.
			PayPlans.Update(payplan);
			PayPlanCharges.Sync(listChargesAndCredits,payplan.PayPlanNum);
			Payment paymentCur=PaymentT.MakePaymentNoSplits(pat.PatNum,40,DateTime.Today,true);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat,paymentCur,new List<long>{pat.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat.PatNum),new Family { },paymentCur
					,loadData.ListSplits,new List<AccountEntry>(),pat.PatNum);
			//the charge that will show in oustanding charges when the user goes to make a payment should show that only $40 (100 proc - 60 adj) is due
			Assert.AreEqual(1,initData.AutoSplitData.ListAccountCharges.FindAll(x => x.GetType()==typeof(PayPlanCharge) 
				&& x.AmountOriginal==100 && x.AmountStart==40).Count);

		}

		///<summary>Make sure if a procedure has been paid by an incorrect provider and a negative unallocated split was made to counteract that payment that
		///the procedure shows as owing money still.</summary>
		[TestMethod]
		public void PaymentEdit_UnlinkedIncomeXferShowsProcOwingMoney() {
			//Make a procedure for Provider A
			//Make a payment on that procedure (in full) for Provider B
			//"Correct" the mistake by creating a negative split for Provider B, but unattached
			//When making another payment it should show the procedure as owing money for Provider A (instead of owing none)
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provA=ProviderT.CreateProvider("ProvA");
			long provB=ProviderT.CreateProvider("ProvB");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",50,provNum:provA);
			PaySplit wrongSplit=PaySplitT.CreateSplit(proc.ClinicNum,pat.PatNum,0,0,proc.ProcDate,proc.ProcNum,provB,50,0);
			PaySplit incorrectXferSplit=PaySplitT.CreateSplit(proc.ClinicNum,pat.PatNum,0,0,proc.ProcDate,0,provB,-50,0);
			//In income xfer mode it'll show that there is a proc owing money.
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,0);
			PaymentEdit.ConstructResults results=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>(),
				payCur,new List<AccountEntry>(),true,false);
			//Make sure that the logic creates 3 charges - One for the procedure (original, start, and end are 50) 
			//One for the wrongSplit paid to the procedure but wrong provider (original, start, and end are -50)
			//One for the incorrectXferSplit that the user created to "Correct" the wrong split.(original, start, and end are 50). This charge gets created
			//because when we implicitly link credits we assume that the "wrongSplit" was already allocated when we explicitly linked the procedures. 
			//i.e the wrong split is associated to the proc but with the wrong provider. The logic does not associate the split to the procedure. When we try to 
			//implicitly link incorrectXferSplit with the wrongSplit, we can't since wrongSplit has an attached procedure.
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(Procedure) && x.AmountOriginal==50 && x.AmountStart==50 && x.AmountEnd==50).Count);
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(PaySplit) && x.AmountEnd==50).Count);
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(PaySplit) && x.AmountEnd==-50).Count);
			//The user needs to either unattach the paysplit on the procedure, attach the negative split on the procedure, or delete both splits.
		}

		///<summary>In income transfer mode, if there are negative unallocated splits, they should be implicitly used by other sources of income (and not show as valid xfer sources).</summary>
		[TestMethod]
		public void PaymentEdit_ConstructLinkChargeCredits_NegSplitsImplicitlyUsedByPosSources() {
			//Make a payment for Provider A for -50 (a charge)
			//Make an unattached claimproc for Provider A for 50
			//Perform an income transfer - It should show both the claimproc and the paysplit valued as 0 (they counteract each other)
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provA=ProviderT.CreateProvider("ProvA");
			Carrier carrier=CarrierT.CreateCarrier("ABC");
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,plan.PlanNum,provA,50,insSub.InsSubNum,0,0);
			PaySplit wrongSplit=PaySplitT.CreateSplit(0,pat.PatNum,0,0,DateTime.Today,0,provA,-50,0);
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,0);
			PaymentEdit.ConstructResults results=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>(),
				payCur,new List<AccountEntry>(),true,false);
			Assert.AreEqual(0,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(ClaimProc)).Count);
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.AmountEnd==50).Count);//PaySplits are opposite signed as account charges. 
		}

		///<summary>If there is an adjustment that needs paying, autosplit logic should attach the paysplit to the adjustment. (treat it like a proc)</summary>
		[TestMethod]
		public void PaymentEdit_PaymentAutoSplitToAdjustment() {
			//Make an unattached adjustment for 50 (a charge)
			//Make a new payment for 50
			//Perform a normal payment - There should be one split for 50 that has the adjustment's AdjNum
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("ProvA");
			Adjustment adjust1=AdjustmentT.MakeAdjustment(pat.PatNum,50,DateTime.Today,provNum:provNum);
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,50);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>(),
				payCur,new List<AccountEntry>(),false,false,new PaymentEdit.LoadData());
			Assert.AreEqual(1,results.ListAutoSplits.Count);
			Assert.AreEqual(results.ListAutoSplits[0].AdjNum,adjust1.AdjNum);
		}

		///<summary>Auto split logic should not use later payments to allocate to the same adjustment that's been allocated to already.</summary>
		[TestMethod]
		public void PaymentEdit_AutoSplitDoesntReuseAllocatedAdjustment() {
			//Make an unattached adjustment for 50 (a charge)
			//Make a payment for 50 to that adjustment
			//Create a procedure that comes after the adjustment for 25
			//Make a new payment for 50 and autosplit it.
			//The auto payment should have 2 splits - One for the procedure and one to unallocated, each for 25.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("ProvA");
			Adjustment adjust1=AdjustmentT.MakeAdjustment(pat.PatNum,50,DateTime.Today.AddDays(-3),provNum:provNum);
			Payment payOld=PaymentT.MakePaymentNoSplits(pat.PatNum,50,DateTime.Today.AddDays(-3));
			PaySplitT.CreateSplit(adjust1.ClinicNum,pat.PatNum,payOld.PayNum,0,DateTime.Today.AddDays(-3),0,provNum,50,0,adjust1.AdjNum);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",25);
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,50,DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>(),
				payCur,new List<AccountEntry>(),false,false,new PaymentEdit.LoadData());
			Assert.AreEqual(2,results.ListAutoSplits.Count);
			Assert.AreEqual(1,results.ListAutoSplits.FindAll(x => x.PatNum==proc.PatNum && x.ProvNum==proc.ProvNum && x.ClinicNum==proc.ClinicNum && x.ProcNum==proc.ProcNum && x.AdjNum==0 && x.UnearnedType==0 && x.SplitAmt==25).Count);
			Assert.AreEqual(1,results.ListAutoSplits.FindAll(x => x.PatNum==pat.PatNum && x.ProvNum==0 && x.ClinicNum==pat.ClinicNum && x.ProcNum==0 && x.AdjNum==0 && x.UnearnedType>0 && x.SplitAmt==25).Count);
		}
			
		///<summary>Adjustments explicitly attached to procedure should not be used to implicitly pay off anything.</summary>
		[TestMethod]
		public void PaymentEdit_AdjustmentsExplicitlyAllocatedArentUsedImplicitly() {
			//Make procedure worth 50
			//Make adjustment worth 70 that's on the first procedure
			//Make another procedure worth 20
			//Auto-split logic should show the second procedure worth 20 still and the first worth -20
			//Autosplits created using 50 payment should be 20 for the second procedure, and 30 with unearned>0
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("ProvA");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50);
			Adjustment adjust1=AdjustmentT.MakeAdjustment(pat.PatNum,-70,DateTime.Today.AddDays(-3),provNum:provNum,procNum:proc.ProcNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",20);
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,50,DateTime.Today.AddDays(-3));
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>(),
				payCur,new List<AccountEntry>(),false,false,new PaymentEdit.LoadData());
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.AmountEnd==-20).Count);//First proc with overpaid adjustment
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.AmountEnd==0).Count);//Second proc, paid fully by payment
			Assert.AreEqual(2,results.ListAutoSplits.Count);//Make sure there are only 2 splits created.
			Assert.AreEqual(1,results.ListAutoSplits.FindAll(x => x.SplitAmt==20 && x.ProcNum==proc2.ProcNum).Count);//One split for 20 for second proc
			Assert.AreEqual(1,results.ListAutoSplits.FindAll(x => x.SplitAmt==30 && x.ProcNum==0 && x.UnearnedType>0).Count);//One split for 30 to unearned
		}

		///<summary>Transferring income to an adjustment should no longer show that adjustment as having owed money.</summary>
		[TestMethod]
		public void PaymentEdit_IncomeTransferToAdjustment() {
			//Create charge adjustment for 50
			//Create payment for 50 that's unallocated
			//Create an income transfer that takes 50 from the unallocated and allocates it to the adjustment.
			//Create new procedure for 25 and backdate it to before the adjustment
			//Create new payment and perform income transfer logic.  
			//There should display only one charge owing any money - The backdated procedure for 25.  (It is no longer implicitly paid due to adjustment link)
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("ProvA");
			Adjustment adjust1=AdjustmentT.MakeAdjustment(pat.PatNum,50,DateTime.Today,provNum:provNum);
			Payment payOld=PaymentT.MakePaymentNoSplits(pat.PatNum,50,DateTime.Today);
			PaySplit prepaySplit=PaySplitT.CreateSplit(0,pat.PatNum,payOld.PayNum,0,DateTime.Today,0,0,50,20);//Some arbitrary unearned type number
			Payment payXfer=PaymentT.MakePaymentNoSplits(pat.PatNum,0,DateTime.Today);
			PaySplit negSplit=PaySplitT.CreateSplit(prepaySplit.ClinicNum,prepaySplit.PatNum,payXfer.PayNum,0,DateTime.Today,0,prepaySplit.ProvNum,-50,prepaySplit.UnearnedType,0,prepaySplit.SplitNum);//negative split taking 50 from prepay split
			PaySplit posSplit=PaySplitT.CreateSplit(adjust1.ClinicNum,adjust1.PatNum,payXfer.PayNum,0,DateTime.Today,0,adjust1.ProvNum,50,0,adjust1.AdjNum,negSplit.SplitNum);//positive split allocating 50 to adjustment
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",25,DateTime.Today.AddDays(-3));//pre-date procedure for before adjust.  In the old way, we'd implicitly use the income on this instead of adjustment.
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,0,DateTime.Today);
			PaymentEdit.ConstructResults results=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>(),
				payCur,new List<AccountEntry>(),true,false);
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(Procedure) && x.AmountEnd==25).Count);
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(Adjustment) && x.AmountEnd==0).Count);
		}

		[TestMethod]
		public void PaymentEdit_ExplicitlyLinkCredits_TransferFromWrongProviderSplit() {
			//To simulate a scenario where a procedure has been paid to the wrong provider and making an income transfer to correct it.
			//Problem was that after making the transfer, the original split and the offsetting split would still show in the window.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNumA=ProviderT.CreateProvider("ProvA");
			long provNumB=ProviderT.CreateProvider("ProvB");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",65,provNum:provNumA);
			Payment originalPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,65);
			Payments.Insert(originalPayment);
			PaySplit originalPaySplit=PaySplitT.CreateSplit(0,pat.PatNum,originalPayment.PayNum,0,proc.ProcDate,proc.ProcNum,provNumB,65,0);
			Payment transfer=PaymentT.MakePaymentNoSplits(pat.PatNum,0);
			PaymentEdit.ConstructAndLinkChargeCredits(new List<long>() {pat.PatNum },pat.PatNum,new List<PaySplit>(),
				transfer,new List<AccountEntry>(),true);
			//create income transfer manually
			PaySplit negOffsetSplit=PaySplitT.CreateSplit(0,pat.PatNum,transfer.PayNum,0,DateTime.Today,proc.ProcNum,provNumB,-65,0,0,originalPaySplit.SplitNum);
			PaySplit posAllocation=PaySplitT.CreateSplit(0,pat.PatNum,transfer.PayNum,0,DateTime.Today,proc.ProcNum,provNumA,65,0,0,negOffsetSplit.SplitNum);
			//Pretend like user is going to make another transfer so we can verify nothing would show up in the window
			Payment fakeTransfer=PaymentT.MakePaymentNoSplits(pat.PatNum,0);
			PaymentEdit.ConstructResults results=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>()
				,fakeTransfer,new List<AccountEntry>(),true);
			//ProvB should have two splits, a positive and a negative. 
			Assert.AreEqual(2,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(PaySplit) && x.ProvNum==provNumB).Count);
			//ProvA should have two positive charges. Original procedure and allocated split. 
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(PaySplit) && x.ProvNum==provNumA).Count);
			//there should be two positive splits total for the amount, and negative (note since they're charges right now their signs are opposite for splits)
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(PaySplit) && x.AmountOriginal==65 && x.AmountStart==0 
				&& x.AmountEnd==0).Count);
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(PaySplit) && x.AmountOriginal==-65 && x.AmountStart==0 
				&& x.AmountEnd==0).Count);//only 1 because one of them (the final allocating split) gets taken care of in Explicitly link and has OrigAmt==0
			Assert.AreEqual(1,results.ListAccountCharges.FindAll(x => x.GetType()==typeof(Procedure) && x.ProvNum==provNumA && x.AmountOriginal==65 
				&& x.AmountStart==0 && x.AmountEnd==0).Count);
			Assert.AreEqual(0,results.ListAccountCharges.Sum(x => x.AmountEnd));
		}

		[TestMethod]
		public void PaymentEdit_AutoSplitForPayment_TpProcsInPaymentWindowWhenDisplayCorrectlyAccordingToPref() {
			PrefT.UpdateInt(PrefName.PrePayAllowedForTpProcs,(int)YN.Yes);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov");
			Procedure treatPlanProc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",50,provNum:provNum);
			Payment currentPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,50,isNew:true);
			PaymentEdit.ConstructResults constructData=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>{pat.PatNum},pat.PatNum
				,new List<PaySplit>(),currentPayment,new List<AccountEntry>());
			PaymentEdit.AutoSplit autoSplitData=PaymentEdit.AutoSplitForPayment(constructData);
			Assert.AreEqual(1,autoSplitData.ListAccountCharges.Count);
			Assert.AreEqual(0,autoSplitData.ListAutoSplits.FindAll(x => x.ProcNum!=0).Count);//auto split should be for reg. unallocated, not the tp.
			//now do the same with the pref turned off. Account charge should not show in the list.
			PrefT.UpdateInt(PrefName.PrePayAllowedForTpProcs,(int)YN.No);
			constructData=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>{pat.PatNum},pat.PatNum,new List<PaySplit>(),currentPayment,
				new List<AccountEntry>());
			autoSplitData=PaymentEdit.AutoSplitForPayment(constructData);
			Assert.AreEqual(0,autoSplitData.ListAccountCharges.Count);
		}

		[TestMethod]
		public void Payments_CreateTransferForTpProcs_TransferTpPreallocationsToAllocatedMoneyOnceCompleted() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov");
			Procedure treatPlanProc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",50,provNum:provNum);
			long hiddenUnearnedType=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType).FirstOrDefault(x => x.ItemValue!="").DefNum;
			Payment paidTpPreAllocation=PaymentT.MakePayment(pat.PatNum,50,DateTime.Today.AddDays(-1),0,provNum,treatPlanProc.ProcNum,1,0,
				hiddenUnearnedType);
			ProcedureT.SetComplete(treatPlanProc,pat,new InsuranceInfo());
			//setting proceudre complete should have made a transfer taking the unearned on the proc, and making it allocated.
			List<PaySplit> listSplitsOnProc=PaySplits.GetPaySplitsFromProc(treatPlanProc.ProcNum);
			Assert.AreEqual(1,listSplitsOnProc.Count);//In the end only one split ends up being attached to the procedure.
			//check to make sure the original prepayment got the procedure disassociated from it. 
			Assert.AreEqual(0,PaySplits.GetForPayment(paidTpPreAllocation.PayNum).First().ProcNum);
			//the negative split (attached to original prepay) should also be the FSplitNum of the final allocating split that has the procedure.
			Assert.AreEqual(listSplitsOnProc.First().FSplitNum
				,PaySplits.GetSplitsForPrepay(PaySplits.GetForPayment(paidTpPreAllocation.PayNum)).First().SplitNum);
			Assert.AreEqual(50,listSplitsOnProc.Sum(x => x.SplitAmt));
		}

		[TestMethod]
		public void Payments_CreateTransferForTpProcs_TransferTpPreAllocationToBrokenProcedure() {
			PrefT.UpdateBool(PrefName.TpPrePayIsNonRefundable,true);
			PrefT.UpdateInt(PrefName.PrePayAllowedForTpProcs,(int)YN.Yes);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov");
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today,1,provNum);
			Procedure treatPlanProc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",50,provNum:provNum,aptNum:appt.AptNum);
			long hiddenUnearnedType=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType).FirstOrDefault(x => x.ItemValue!="").DefNum;
			Payment paidTpPreAllocation=PaymentT.MakePayment(pat.PatNum,50,DateTime.Today.AddDays(-1),0,provNum,treatPlanProc.ProcNum,1,0,
				hiddenUnearnedType);
			//D9986 == missed appointment
			ProcedureCode procCode=new ProcedureCode{CodeNum=treatPlanProc.CodeNum,ProcCode="D9986",NoBillIns=true,ProvNumDefault=provNum};
			Procedure brokenProcedure=AppointmentT.BreakAppointment(appt,pat,procCode,50);
			List<PaySplit> listSplitsForBrokenProc=PaySplits.GetPaySplitsFromProc(brokenProcedure.ProcNum);
			Assert.AreEqual(50,listSplitsForBrokenProc.Sum(x => x.SplitAmt));
			Assert.AreEqual(0,PaySplits.GetPaySplitsFromProc(treatPlanProc.ProcNum).Sum(x => x.SplitAmt));
		}

		[TestMethod]
		public void PaymentEdit_AutoSplitForPayment_HiddenPaysplitsAreNotUsedInImplicitLinking() {
			Def def=DefT.CreateDefinition(DefCat.PaySplitUnearnedType,"hiddenType","x");
			PrefT.UpdateInt(PrefName.PrePayAllowedForTpProcs,(int)YN.Yes);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov");
			//Make an unattached hidden payment that will be reserved for some treatment planned procedure down the line.
			Payment hiddenPayment=PaymentT.MakePayment(pat.PatNum,50,DateTime.Today.AddDays(-1),unearnedType:def.DefNum);
			//Make a completed procedure that we do not want linked to the payment we just made. 
			Procedure procedure=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"4",100);
			Payment currentPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,25,DateTime.Today);
			PaymentEdit.ConstructResults constructData=PaymentEdit.ConstructAndLinkChargeCredits(new List<long>{pat.PatNum},pat.PatNum
				,new List<PaySplit>(),currentPayment,new List<AccountEntry>());
			PaymentEdit.AutoSplit autoSplitData=PaymentEdit.AutoSplitForPayment(constructData);
			Assert.AreEqual(1,autoSplitData.ListAccountCharges.FindAll(x => x.AmountStart==100 && x.AmountEnd==75).Count);
			Assert.AreEqual(1,autoSplitData.ListAutoSplits.FindAll(x => x.SplitAmt==25).Count);//auto split should be for reg. unallocated, not the tp.
		}
		#endregion
		#region PayPlan Tests - PayPlanEdit
		[TestMethod]
		public void PayPlanEdit_CreatePayPlanAdjustments_PayPlanWithNegAdjustmentsForPlansWithUnattachedCredits() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",135,DateTime.Today.AddMonths(-3));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Today.AddMonths(-3));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),provNum:0,totalAmt:195);//totalAmt since unattached credits
			List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			double totalFutureNegAdjs=PayPlanT.GetTotalNegFutureAdjs(listCharges);
			List<PayPlanCharge> listChargesAndCredits=PayPlanEdit.CreatePayPlanAdjustments(-45,listCharges,totalFutureNegAdjs);
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal < 0).Count);//2 negative debits
			//Assert that the method only created adjustments for furthest out dates.
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit 
				&& x.ChargeDate.Month==DateTime.Today.AddMonths(2).Month).Count);//1 positive and 1 negative debit
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit 
				&& x.ChargeDate.Month==DateTime.Today.AddMonths(3).Month).Count);
		}

		[TestMethod]
		public void PayPlanEdit_CreatePayPlanAdjustments_PayPlanWithNegAdjustmentForPlansWithAttachedCredits() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",135,DateTime.Today.AddMonths(-3));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Today.AddMonths(-3));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),0,new List<Procedure> {proc1,proc2 });
			List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			double totalFutureNegAdjs=PayPlanT.GetTotalNegFutureAdjs(listCharges);
			List<PayPlanCharge> listChargesAndCredits=PayPlanEdit.CreatePayPlanAdjustments(-45,listCharges,totalFutureNegAdjs);
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal < 0).Count);//2 negative debits
			//Assert that the method only created adjustments for furthest out dates.
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit 
				&& x.ChargeDate.Month==DateTime.Today.AddMonths(2).Month).Count);//1 positive and 1 negative debit
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit 
				&& x.ChargeDate.Month==DateTime.Today.AddMonths(3).Month).Count);
		}

		[TestMethod]
		public void PayPlanEdit_CloseOutPatPayPlan_CloseOutPatPayPlanWithAdjustments() {
			DateTime today=new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.Today.Day,0,0,0,DateTimeKind.Unspecified);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",100,DateTime.Today.AddMonths(-3));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",92,DateTime.Today.AddMonths(-3));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),0,new List<Procedure> {proc1,proc2 });
			Payment payment=PaymentT.MakePayment(pat.PatNum,30,DateTime.Today.AddMonths(-2),payplan.PayPlanNum);//make a payment for the plan
			List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			double totalFutureNegAdjs=PayPlanT.GetTotalNegFutureAdjs(listCharges);
			List<PayPlanCharge> listChargesAndCredits=PayPlanEdit.CreatePayPlanAdjustments(-62,listCharges,totalFutureNegAdjs);//make adjustments for the plan.
			listChargesAndCredits.Add(PayPlanT.CreateNegativeCreditForAdj(pat.PatNum,payplan.PayPlanNum,-62));//add the tx credit for the adjustment
			//Balance should equal 100. $192 of completed tx - $30 payment + $-62 adjustment. 
			PayPlanCharge closeOutCharge=PayPlanEdit.CloseOutPatPayPlan(listChargesAndCredits,payplan,today);
			//List<PayPlanCharge> listFinalCharges=listChargesAndCredits.RemoveAll(x => x.ChargeDate > today.Date);
			listChargesAndCredits.RemoveAll(x => x.ChargeDate > DateTime.Today);
			listChargesAndCredits.Add(closeOutCharge);
			double debitsDue=120;//4 pay plan charges of $30 each that have come due
			double creditsOutstanding=130;//Original $192 - $62 adjustment
			Assert.AreEqual(creditsOutstanding - debitsDue,closeOutCharge.Principal);//close out charge should equal 10 (remaining debits - credits)
			//total balance should equal 100 (-30 to take out the payment that was made). 
			Assert.AreEqual(100,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit).Sum(x => x.Principal)-30); 
		}

		///<summary>If a payment plan is created with a guarantor outside the payment plan, and payments made by guarantor, don't show the payplan's procedures nor guarantor's payment as valid income transfer sources/destinations.</summary>
		[TestMethod]
		public void PayPlanEdit_IncomeXferWithPayPlanAndGuarantorPaymentsOutsideFamily() {
			DateTime today=new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.Today.Day,0,0,0,DateTimeKind.Unspecified);
			Patient pat1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Patient pat2=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"2");
			Procedure proc1=ProcedureT.CreateProcedure(pat1,"D1120",ProcStat.C,"",100,DateTime.Today.AddMonths(-3));
			Procedure proc2=ProcedureT.CreateProcedure(pat2,"D0220",ProcStat.C,"",92,DateTime.Today.AddMonths(-3));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat1.PatNum,100,DateTime.Today.AddMonths(-3),0,new List<Procedure> {proc1},guarantorNum:pat2.PatNum);
			Payment payment=PaymentT.MakePayment(pat2.PatNum,100,DateTime.Today.AddMonths(-2),payplan.PayPlanNum);//make a payment for the plan
			List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			PayPlanCharge closeOutCharge=PayPlanEdit.CloseOutPatPayPlan(listCharges,payplan,today);
			//When performing an income transfer, we should see only one charge - The charge for the guarantor that's unpaid for 92, even though the guarantor paid on the payplan.
			Payment incomeTransfer=PaymentT.MakePaymentNoSplits(pat2.PatNum,0,isNew:true);
			PaymentEdit.LoadData loadData=PaymentEdit.GetLoadData(pat2,incomeTransfer,new List<long>{pat2.PatNum},true,false);
			PaymentEdit.InitData initData=PaymentEdit.Init(loadData.ListAssociatedPatients,Patients.GetFamily(pat2.PatNum),new Family { },incomeTransfer
					,loadData.ListSplits,new List<AccountEntry>(),pat2.PatNum,isIncomeTxfr:true);
			Assert.AreEqual(initData.AutoSplitData.ListAccountCharges.FindAll(x => x.AmountOriginal!=0).Count,1);
			Assert.AreEqual(initData.AutoSplitData.ListAccountCharges.Find(x => x.AmountOriginal!=0).AmountOriginal,92);
		}

		[TestMethod]
		public void PaySplits_CreateSplitForPayPlan_CreateAttachedSplitsForMultipleAttachedCredits() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",100,DateTime.Today.AddMonths(-3));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today.AddMonths(-3));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,100,DateTime.Today.AddMonths(-3),0,new List<Procedure> {proc1,proc2 });
			Payment payment=PaymentT.MakePayment(pat.PatNum,100,DateTime.Today.AddMonths(-2),payplan.PayPlanNum,procNum:proc1.ProcNum);//make a payment for the plan
			Payment newPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,100,DateTime.Today);
			PaymentEdit.InitData init=PaymentEdit.Init(new List<Patient> {pat },Patients.GetFamily(pat.PatNum),Patients.GetFamily(pat.PatNum),newPayment
				,new List<PaySplit>(),new List<AccountEntry>(),pat.PatNum);
			Assert.AreEqual(proc2.ProcNum,init.AutoSplitData.ListAutoSplits[0].ProcNum);
		}

		[TestMethod]
		public void PayPlanEdit_GetLoadDataForPayPlanCredits_CreditsGetCorrectlyLinkedToCorrectPayPlan() {
			Patient patInFam=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"famMember");
			Patient patGuar=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"famGuar");
			Family fam=new Family(new List<Patient>() {patGuar,patInFam});
			Procedure procForFamMember=ProcedureT.CreateProcedure(patInFam,"D1120",ProcStat.C,"",100,DateTime.Today.AddMonths(-3));
			Procedure procForFamGuar=ProcedureT.CreateProcedure(patGuar,"D0220",ProcStat.C,"",80,DateTime.Today.AddMonths(-3));
			PayPlan payplanForFamMember=PayPlanT.CreatePayPlanWithCredits(patInFam.PatNum,30,DateTime.Today.AddMonths(-1),0
				,new List<Procedure> {procForFamMember},guarantorNum:patGuar.PatNum);
			PayPlan payplanForFamGuar=PayPlanT.CreatePayPlanWithCredits(patGuar.PatNum,30,DateTime.Today.AddMonths(-1),0,new List<Procedure> {procForFamGuar});
			PayPlanEdit.PayPlanCreditLoadData data=PayPlanEdit.GetLoadDataForPayPlanCredits(patGuar.PatNum,payplanForFamGuar.PayPlanNum);
			decimal amt=PayPlanEdit.GetAccountCredits(data.ListAdjustments,data.ListTempPaySplit,data.ListPayments,data.ListClaimProcs,data.ListPayPlanCharges);
			//list of credits is supposed to include credits for pat on differen't payplans for pat. Should not take family memeber's credits into account.
			Assert.AreEqual(0,amt);
		}

		[TestMethod]
		public void PaySplits_GetUnearnedForFam_ManuallyAllocatingUnearnedForFamily() {
			//Test scenario. A user has manually allocated unearned but did not attach the final linking split to the procedure pay split. 
			//Orginal prepayment is linked on the negative allocating split, but the postive allocating split does not have the negative split attached.
			//We need to check to make sure the unearned is calculating correctly, previosly is was double counting some things and not getting correct 
			//unearned amount. 
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=new Family(new List<Patient> {pat});
			Def unearned=DefT.CreateDefinition(DefCat.PaySplitUnearnedType,MethodBase.GetCurrentMethod().Name);
			//Create 2 separate prepayments (bug would only show when more than 1 was present)
			PaySplit prepay1=PaySplitT.CreatePrepayment(pat.PatNum,65,DateTime.Today.AddMonths(-3));
			PaySplit prepay2=PaySplitT.CreatePrepayment(pat.PatNum,5,DateTime.Today.AddMonths(-3));
			//make the procedures for the prepayments
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",65,DateTime.Today);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",5,DateTime.Today);
			//Manually allocate the prepayments to the procedures.
			Payment pay1=PaymentT.MakePaymentNoSplits(pat.PatNum,0,DateTime.Today);
			PaySplitT.CreateSplit(0,pat.PatNum,pay1.PayNum,0,prepay1.SecDateTEdit,0,0,-65,unearned.DefNum,prepay1.SplitNum);
			PaySplitT.CreateSplit(0,pat.PatNum,pay1.PayNum,0,proc1.ProcDate,proc1.ProcNum,proc1.ProvNum,65,0);
			Payment pay2=PaymentT.MakePaymentNoSplits(pat.PatNum,0,DateTime.Today);
			PaySplitT.CreateSplit(0,pat.PatNum,pay2.PayNum,0,prepay2.SecDateTEdit,0,0,-5,unearned.DefNum,prepay2.SplitNum);
			PaySplitT.CreateSplit(0,pat.PatNum,pay2.PayNum,0,proc2.ProcDate,proc2.ProcNum,proc2.ProvNum,5,0);
			decimal unearnedAmt=PaySplits.GetUnearnedForFam(fam);
			Assert.AreEqual(0,unearnedAmt);
		}
		#endregion
		#region Helper Methods
		private void UpdatePaySplitHelper(PaySplit ps,long patNum,long provNum,long clinicNum,long payNum) {
			if(payNum!=0 && payNum==ps.PayNum) {
				ps.PatNum=patNum;
				ps.ProvNum=provNum;
				ps.ClinicNum=clinicNum;
				PaySplits.Update(ps);
			}
		}

		private void ResetPrepayments(Patient pat) {
			Family fam=Patients.GetFamily(pat.PatNum);
			_listFamPrePaySplits=PaySplits.GetPrepayForFam(fam);
			_listPosPrePay=_listFamPrePaySplits.FindAll(x => x.SplitAmt.IsGreaterThan(0));
			_listNegPrePay=_listFamPrePaySplits.FindAll(x => x.SplitAmt.IsLessThan(0));
		}
		#endregion
	}
}
