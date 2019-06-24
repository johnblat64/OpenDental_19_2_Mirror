using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Ledgers_Tests {
	[TestClass]
	public class LedgersTests:TestBase {
		///<summary>Tests that ComputeAgingForPaysplitsAllocatedToDiffPats computes aging for other patients.</summary>
		[TestMethod]
		public void LedgersTests_ComputeAgingForPaysplitsAllocatedToDiffPats() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patDad=PatientT.CreatePatient(fName:"Father",suffix:suffix);//Father-guarantor
			Patient patChild=PatientT.CreatePatient(fName:"Child",suffix:suffix);//Child
			PatientT.SetGuarantor(patChild,patDad.PatNum);
			Procedure proc1=ProcedureT.CreateProcedure(patChild,"D1110",ProcStat.C,"",50,DateTime.Now.AddDays(-45));
			Procedure proc2=ProcedureT.CreateProcedure(patChild,"D0120",ProcStat.C,"",40,DateTime.Now.AddDays(-45));
			Ledgers.ComputeAging(patDad.PatNum,DateTime.Today);
			patDad=Patients.GetPat(patDad.PatNum);
			Assert.AreEqual(90,patDad.Bal_31_60);
			Patient patMom=PatientT.CreatePatient(fName:"Mom",suffix:suffix);//Mom is not associated to the father and childs account
			long patNum=patMom.PatNum;
			//complete procedures for patMom.
			Procedure proc3=ProcedureT.CreateProcedure(patMom,"D1110",ProcStat.C,"",50,DateTime.Now.AddDays(-1));
			Procedure proc4=ProcedureT.CreateProcedure(patMom,"D0120",ProcStat.C,"",40,DateTime.Now.AddDays(-1));
			//Compute aging. Check that the aging is correct before we continue
			Ledgers.ComputeAging(patNum,DateTime.Today);
			patMom=Patients.GetPat(patNum);
			Assert.AreEqual(90,patMom.Bal_0_30);
			//patMom will now make a payment for her procedures and patChilds
			Payment pay=PaymentT.MakePaymentNoSplits(patNum,100,clinicNum:proc3.ClinicNum);
			List<PaySplit> listPaySplits=new List<PaySplit>();
			listPaySplits.Add(PaySplitT.CreateSplit(proc3.ClinicNum,patNum,pay.PayNum,0,DateTime.Today,proc3.ProcNum,proc3.ProvNum,50,0));
			//Create a paysplit for patChild. PatChild is from a different family.
			listPaySplits.Add(PaySplitT.CreateSplit(proc1.ClinicNum,patChild.PatNum,pay.PayNum,0,DateTime.Today,proc1.ProcNum,proc1.ProvNum,50,0));
			//This should compute the aging for the 			
			string strErrorMsg=Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(patNum,listPaySplits);
			Assert.IsTrue(string.IsNullOrEmpty(strErrorMsg));
			patDad=Patients.GetPat(patDad.PatNum);
			Assert.AreEqual(40,patDad.Bal_31_60);
			//Compute patMom aging to verify that it is correct. 
			Ledgers.ComputeAging(patNum,DateTime.Today);
			patMom=Patients.GetPat(patNum);
			Assert.AreEqual(40,patMom.Bal_0_30);
		}
	}
}
