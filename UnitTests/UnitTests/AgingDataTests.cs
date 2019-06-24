using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.AgingData_Tests {
	[TestClass]
	public class AgingDataTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		///<summary>Make sure that patients show up in the billing list (aging list) when there has been no statement.</summary>
		[TestMethod]
		public void AgingData_GetAgingData_PayPlanBillInAdvanceDays_NoStatement() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create a payment plan where the first charge date is in the future.
			DateTime dateTimePayPlanCharge=DateTime.Today.AddDays(5);
			DateTime dateStatement=DateTime.Today;
			PayPlanT.CreatePayPlan(patient.PatNum,1000,500,dateTimePayPlanCharge,provNum);
			//Make sure that the preference PayPlansBillInAdvanceDays is set to a day range that encompasses the first payment plan charge date.
			PrefT.UpdateLong(PrefName.PayPlansBillInAdvanceDays,10);
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,false,false,false,new List<long>());
			//Assert that the patient has been returned due to owing money on a payment plan that falls within the "bill in advance days" preference.
			Assert.IsTrue(dictPatAgingData.ContainsKey(patient.PatNum),"No aging data was returned for the patient.");
			//Assert all pertinent PatAgingData for this unit test.
			Assert.IsNotNull(dictPatAgingData[patient.PatNum].ListPatAgingTransactions);
			PatAgingTransaction patAgingTransaction=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.PayPlanCharge);
			Assert.IsNotNull(patAgingTransaction);
			Assert.AreEqual(dateTimePayPlanCharge,patAgingTransaction.DateLastTrans);
			//Do NOT insert a statement.  This should cause the patient to be included in the PatAging list returned.
			//StatementT.CreateStatement(patient.PatNum,mode_:StatementMode.Mail,isSent:true,dateSent:dateStatement);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTrans=new SerializableDictionary<long, List<PatAgingTransaction>>();
			foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
				dictPatAgingTrans[kvp.Key]=kvp.Value.ListPatAgingTransactions;
			}
			//Assert that GetDateLastTrans() returns the date we are expecting.
			Assert.AreEqual(DateTime.MinValue,AgingData.GetDateLastTrans(dictPatAgingTrans[patient.PatNum],dateStatement).Date);
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today.AddMonths(-1),new List<long>(),false,false,0,false,false,new List<long>(),false,false,
				new List<long>(),new List<long>(),dictPatAgingTrans);
			//Assert that the patient will not get a new statement due to the statement that was created above.
			Assert.IsTrue(listPatAging.Any(x => x.PatNum==patient.PatNum),"The patient was supposed to be present in the AgingList.");
		}

		///<summary>Make sure that patients do not show up in the billing list (aging list) when there has been a statement within
		///the PayPlansBillInAdvanceDays date range (assume the statement within the date range encompasses the payment plan charge).</summary>
		[TestMethod]
		public void AgingData_GetAgingData_PayPlanBillInAdvanceDays_WithStatement() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create a payment plan where the first charge date is in the future.
			DateTime dateTimePayPlanCharge=DateTime.Today.AddDays(5);
			DateTime dateStatement=DateTime.Today;
			PayPlanT.CreatePayPlan(patient.PatNum,1000,500,dateTimePayPlanCharge,provNum);
			//Make sure that the preference PayPlansBillInAdvanceDays is set to a day range that encompasses the first payment plan charge date.
			PrefT.UpdateLong(PrefName.PayPlansBillInAdvanceDays,10);
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,false,false,false,new List<long>());
			//Assert that the patient has been returned due to owing money on a payment plan that falls within the "bill in advance days" preference.
			Assert.IsTrue(dictPatAgingData.ContainsKey(patient.PatNum),"No aging data was returned for the patient.");
			//Assert all pertinent PatAgingData for this unit test.
			Assert.IsNotNull(dictPatAgingData[patient.PatNum].ListPatAgingTransactions);
			PatAgingTransaction patAgingTransaction=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.PayPlanCharge);
			Assert.IsNotNull(patAgingTransaction);
			Assert.AreEqual(dateTimePayPlanCharge,patAgingTransaction.DateLastTrans);
			//Insert a statement that was sent today.  This should cause the patient to be excluded from the PatAging list returned.
			StatementT.CreateStatement(patient.PatNum,mode_:StatementMode.Mail,isSent:true,dateSent:dateStatement);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTrans=new SerializableDictionary<long, List<PatAgingTransaction>>();
			foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
				dictPatAgingTrans[kvp.Key]=kvp.Value.ListPatAgingTransactions;
			}
			//Assert that GetDateLastTrans() returns the date we are expecting.
			Assert.AreEqual(DateTime.MinValue,AgingData.GetDateLastTrans(dictPatAgingTrans[patient.PatNum],dateStatement).Date);
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today.AddMonths(-1),new List<long>(),false,false,0,false,false,new List<long>(),false,false,
				new List<long>(),new List<long>(),dictPatAgingTrans);
			//Assert that the patient will not get a new statement due to the statement that was created above.
			Assert.IsFalse(listPatAging.Any(x => x.PatNum==patient.PatNum),"The patient was not supposed to be present in the AgingList.");
		}

		///<summary>Make sure that patients show up in the billing list (aging list) when there has been a statement within the PayPlansBillInAdvanceDays
		///date range (they should cancel each other out) BUT there is a completed procedure (or claimproc) AFTER the statement.</summary>
		[TestMethod]
		public void AgingData_GetAgingData_PayPlanBillInAdvanceDays_WithPendingProc() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			long provNum=ProviderT.CreateProvider(suffix);
			DateTime datePayPlan=DateTime.Today.AddDays(5);
			DateTime datePayPlanCreate=DateTime.Today.AddMonths(-1);//Payment Plan was created a month ago.
			DateTime dateProc=DateTime.Today;
			DateTime dateStatement=DateTime.Today.AddDays(-5);
			//Create a payment plan where the first charge date in the future.
			PayPlanT.CreatePayPlan(patient.PatNum,1000,500,datePayPlan,provNum);
			//Create a completed procedure that was completed today, before the first payplan charge date.
			ProcedureT.CreateProcedure(patient,"D1100",ProcStat.C,"",5,dateProc);
			//Insert a statement that was sent during the "bill in advance days" for the payment plan charge above.
			StatementT.CreateStatement(patient.PatNum,mode_:StatementMode.Mail,isSent:true,dateSent:dateStatement);
			//Make sure that the preference PayPlansBillInAdvanceDays is set to a day range that encompasses the first payment plan charge date.
			PrefT.UpdateLong(PrefName.PayPlansBillInAdvanceDays,10);
			//This scenario is exploiting the fact that the statement created 5 days ago was technically created for the payment plan (in advance).
			//Because of this fact, the patient shouldn't show up in the billing list until something new happens after the statement date.
			//The procedure that was completed today should cause the patient to show up in the billing list (something new happened).
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,false,false,false,new List<long>());
			//Assert that the patient has been returned due to the completed procedure.
			Assert.IsTrue(dictPatAgingData.ContainsKey(patient.PatNum),"No aging data was returned for the patient.");
			//Assert all pertinent PatAgingData for this unit test.
			Assert.IsNotNull(dictPatAgingData[patient.PatNum].ListPatAgingTransactions);
			PatAgingTransaction patAgingTransactionPP=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.PayPlanCharge);
			//Act like the payment plan was created a month ago.
			patAgingTransactionPP.SecDateTEntryTrans=datePayPlanCreate;
			PatAgingTransaction patAgingTransactionProc=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.Procedure);
			Assert.IsNotNull(patAgingTransactionPP);
			Assert.IsNotNull(patAgingTransactionProc);
			Assert.AreEqual(datePayPlan,patAgingTransactionPP.DateLastTrans);
			Assert.AreEqual(dateProc,patAgingTransactionProc.DateLastTrans);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTrans=new SerializableDictionary<long,List<PatAgingTransaction>>();
			foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
				dictPatAgingTrans[kvp.Key]=kvp.Value.ListPatAgingTransactions;
			}
			//The last transaction date should be the procedure date and not the pay plan charge date (even though pay plan is later).
			Assert.AreEqual(dateProc,AgingData.GetDateLastTrans(dictPatAgingTrans[patient.PatNum],dateStatement).Date);
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today.AddMonths(-1),new List<long>(),false,false,0,false,false,new List<long>(),
				false,false,new List<long>(),new List<long>(),dictPatAgingTrans);
			//Assert that the patient has been flagged to get a new statement due to procedure that was completed above.
			Assert.IsTrue(listPatAging.Any(x => x.PatNum==patient.PatNum),"The expected patient was not present in the AgingList.");
		}

		///<summary>Make sure that patients show up in the billing list (aging list) when there has been a statement within the PayPlansBillInAdvanceDays
		///date range (they should cancel each other out) BUT there is a completed procedure (or claimproc) AFTER the statement.</summary>
		[TestMethod]
		public void AgingData_GetAgingData_PayPlanBillInAdvanceDays_WithNewPayPlan() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			long provNum=ProviderT.CreateProvider(suffix);
			DateTime datePayPlanCharge=DateTime.Today.AddDays(5);
			DateTime datePayPlanCreate=DateTime.Today;//The payment plan that we are about to create will automatically have this date as the SecTDateEntry
			DateTime dateProc=DateTime.Today.AddDays(-1);
			DateTime dateStatement=DateTime.Today.AddDays(-1);
			//Create a payment plan where the first charge date in the future.
			PayPlanT.CreatePayPlan(patient.PatNum,1000,500,datePayPlanCharge,provNum);
			//Create a completed procedure that was completed yesterday, before the first payplan charge date AND before the payment plan creation date.
			ProcedureT.CreateProcedure(patient,"D1100",ProcStat.C,"",5,dateProc);
			//Insert a statement that was sent during the "bill in advance days" for the payment plan charge AND before the payment plan creation date.
			StatementT.CreateStatement(patient.PatNum,mode_:StatementMode.Mail,isSent:true,dateSent:dateStatement);
			//Make sure that the preference PayPlansBillInAdvanceDays is set to a day range that encompasses the first payment plan charge date.
			PrefT.UpdateLong(PrefName.PayPlansBillInAdvanceDays,10);
			//This scenario is exploiting the fact that the statement created yesterday was NOT technically created for the payment plan (in advance).
			//Because of this fact, the patient should show up in the billing list because something new has happened after the statement date.
			//The new payment plan should not be associated to the previous statement due to the SecTDateEntry.
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,false,false,false,new List<long>());
			//Assert that the patient has been returned due to owing money on the payment plan that was created.
			Assert.IsTrue(dictPatAgingData.ContainsKey(patient.PatNum),"No aging data was returned for the patient.");
			Assert.IsNotNull(dictPatAgingData[patient.PatNum].ListPatAgingTransactions);
			PatAgingTransaction patAgingTransactionPP=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.PayPlanCharge);
			PatAgingTransaction patAgingTransactionProc=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.Procedure);
			Assert.IsNotNull(patAgingTransactionPP);
			Assert.IsNotNull(patAgingTransactionProc);
			Assert.AreEqual(datePayPlanCharge,patAgingTransactionPP.DateLastTrans);
			Assert.AreEqual(dateProc,patAgingTransactionProc.DateLastTrans);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTrans=new SerializableDictionary<long,List<PatAgingTransaction>>();
			foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
				dictPatAgingTrans[kvp.Key]=kvp.Value.ListPatAgingTransactions;
			}
			//The last transaction date should be the charge date of the pay plan charge which indicates that the statement doesn't apply
			//to the payment plan because the payment plan was created AFTER the statement that just so happens to fall within the "bill in advance days".
			Assert.AreEqual(datePayPlanCharge,AgingData.GetDateLastTrans(dictPatAgingTrans[patient.PatNum],dateStatement).Date);
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today.AddMonths(-1),new List<long>(),false,false,0,false,false,new List<long>(),
				false,false,new List<long>(),new List<long>(),dictPatAgingTrans);
			Assert.IsTrue(listPatAging.Any(x => x.PatNum==patient.PatNum),"The expected patient was not present in the AgingList.");
		}

	}
}
