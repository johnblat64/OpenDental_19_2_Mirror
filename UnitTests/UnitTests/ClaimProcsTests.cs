using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.ClaimProcs_Tests {
	[TestClass]
	public class ClaimProcsTests:TestBase {

		const double _ucrFee=50;
		const double _fee=25;
		const int _coveragePercent=50;
		const double _patPortionFromFee=_fee*((double)_coveragePercent/100.0);
		const double _blankFee=-1;

		#region FixedBenefits

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffFalse_PPO60_FixedBenefitFeeBlank() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,false);
			double procFee=100;
			double ppoFee=60;
			double fixedBenefitFee=-1;//blank
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(60,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(-1,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(40,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffTrue_PPO60_FixedBenefitFeeBlank() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,true);
			double procFee=100;
			double ppoFee=60;
			double fixedBenefitFee=-1;//blank
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(60,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(40,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffFalse_PPO60_FixedBenefitFeeZero() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,false);
			double procFee=100;
			double ppoFee=60;
			double fixedBenefitFee=0;
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(60,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(40,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffTrue_PPO60_FixedBenefitFeeZero() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,true);
			double procFee=100;
			double ppoFee=60;
			double fixedBenefitFee=0;
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(60,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(40,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffFalse_PPOBlank_FixedBenefitFeeBlank() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,false);
			double procFee=100;
			double ppoFee=-1;//blank
			double fixedBenefitFee=-1;//blank
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(100,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(-1,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffTrue_PPOBlank_FixedBenefitFeeBlank() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,true);
			double procFee=100;
			double ppoFee=-1;//blank
			double fixedBenefitFee=-1;//blank
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(100,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		///<summary>Creates a procedure and computes estimates for a patient with a fixed benefit PPO plan.</summary>
		private void ComputeBaseEstFixedBenefits(string suffix,double procFee,double ppoFee,double fixedBenefitFee,double copayOverride,double allowedOverride,int percentOverride
			,double paidOtherInsTot,double paidOtherInsBase,double writeOffOtherIns,Action<BenefitsAssertItem> assertAct)
		{
			Patient pat=PatientT.CreatePatient(suffix);
			string procStr="D0150";
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ProcedureCode procCode=ProcedureCodes.GetProcCode(procStr);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			long catPercFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Category % "+suffix);
			long fixedBenefitFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.FixedBenefit,"Fixed Benefit "+suffix);
			if(ppoFee>-1) {
				FeeT.CreateFee(ppoFeeSchedNum,procCode.CodeNum,ppoFee);
			}
			if(fixedBenefitFee>-1) {
				FeeT.CreateFee(fixedBenefitFeeSchedNum,procCode.CodeNum,fixedBenefitFee);
			}
			InsuranceT.AddInsurance(pat,suffix,"p",ppoFeeSchedNum,copayFeeSchedNum: fixedBenefitFeeSchedNum);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,copayOverride,allowedOverride,percentOverride);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,priPlan,priPatPlan.PatPlanNum,listBens,histList,loopList,listPatPlans,paidOtherInsTot
				,paidOtherInsBase,pat.Age,writeOffOtherIns,listPlans,listSubs,listSubLinks,false,null);
			assertAct(new BenefitsAssertItem() {
				Procedure=proc,
				PrimaryClaimProc=priClaimProc,
			});
		}

		#endregion

		#region PPO
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_BlankFee_PPO() {
			AssertExclusions("p",feeSchedFee: _blankFee,hasExclusion: false,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion: _ucrFee*((double)_coveragePercent/100.0));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_ZeroFee_PPO() {
			AssertExclusions("p",feeSchedFee: 0,hasExclusion: false,eProcFee: _ucrFee,eWriteOff: _ucrFee,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_NormalFee_PPO() {
			AssertExclusions("p",feeSchedFee: _fee,hasExclusion: false,eProcFee: _ucrFee,eWriteOff: _ucrFee-_fee,ePatPortion: _patPortionFromFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_PPO() {
			AssertExclusions("p",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion: _ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_PPO() {
			AssertExclusions("p",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: _ucrFee,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_PPO() {
			AssertExclusions("p",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: _ucrFee-_fee,ePatPortion: _ucrFee-(_ucrFee-_fee));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_PPO_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("p",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_PPO_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("p",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_PPO_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("p",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}
		#endregion

		#region Medicaid
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_BlankFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: _blankFee,hasExclusion: false,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_ZeroFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: 0,hasExclusion: false,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_NormalFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: _fee,hasExclusion: false,eProcFee: _fee,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: 0,hasExclusion: true,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: _fee,hasExclusion: true,eProcFee: _fee,eWriteOff: -1,ePatPortion: _fee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_Medicaid_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("f",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_Medicaid_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("f",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_Medicaid_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("f",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}
		#endregion

		#region Capitation
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_BlankFee_Capitation() {
			AssertExclusions("c",feeSchedFee: _blankFee,hasExclusion: false,eProcFee: 0,eWriteOff: 0,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_ZeroFee_Capitation() {
			AssertExclusions("c",feeSchedFee: 0,hasExclusion: false,eProcFee: 0,eWriteOff: 0,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_NormalFee_Capitation() {
			AssertExclusions("c",feeSchedFee: _fee,hasExclusion: false,eProcFee: _fee,eWriteOff: _fee,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_Capitation() {
			AssertExclusions("c",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: 0,eWriteOff: 0,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_Capitation() {
			AssertExclusions("c",feeSchedFee: 0,hasExclusion: true,eProcFee: 0,eWriteOff: 0,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_Capitation() {
			AssertExclusions("c",feeSchedFee: _fee,hasExclusion: true,eProcFee: _fee,eWriteOff: _fee,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_Capitation_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("c",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_Capitation_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("c",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_Capitation_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("c",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		///<summary>We have a DBM named ClaimProcDateNotMatchCapComplete which fixes this issue if present in historic data.</summary>
		public void ClaimProcs_ComputeEstimates_Capitation() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,"Medicaid","c");
			DateTime dateToday=DateTime.Today;//Store the value so it does not change when used in muiltiple places below (in case run at midnight).
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.TP,"",50,dateToday.AddMonths(-1));
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			Assert.AreEqual(listClaimProcs[0].Status,ClaimProcStatus.CapEstimate);//Must be CapEstimate for TP procedures.
			Assert.AreEqual(listClaimProcs[0].ProcDate,proc.ProcDate);//ProcDate must be synchronized.
			Assert.AreEqual(listClaimProcs[0].DateCP,proc.ProcDate);//DateCP (Payment Date) starts as the ProcDate for TP procs and is updated when completed.
			Procedure procOld=proc.Copy();
			proc.ProcStatus=ProcStat.C;
			proc.ProcDate=dateToday;//When we set procedures complete anywhere in the program, we also set the ProcDate to today.
			Procedures.Update(proc,procOld);
			Procedures.ComputeEstimates(proc,pat.PatNum,listClaimProcs,false,
				insInfo.ListInsPlans,insInfo.ListPatPlans,insInfo.ListBenefits,pat.Age,insInfo.ListInsSubs);
			List<ClaimProc> listCompClaimProcs=ClaimProcs.RefreshForProc(proc.ProcNum);
			Assert.AreEqual(listCompClaimProcs[0].Status,ClaimProcStatus.CapComplete);//Must be CapComplete for complete procedures.
			Assert.AreEqual(listCompClaimProcs[0].ProcDate,dateToday);//ProcDate must be set to today's date when completed.
			Assert.AreEqual(listCompClaimProcs[0].DateCP,dateToday);//DateCP (Payment Date) must be set to today's date when completed.
		}

		#endregion

		#region CatPercent
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_BlankFee_CatPercent() {
			AssertExclusions("",feeSchedFee: _blankFee,hasExclusion: false,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_ZeroFee_CatPercent() {
			AssertExclusions("",feeSchedFee: 0,hasExclusion: false,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_NormalFee_CatPercent() {
			AssertExclusions("",feeSchedFee: _fee,hasExclusion: false,eProcFee: _fee,eWriteOff: -1,ePatPortion: _patPortionFromFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_CatPercent() {
			AssertExclusions("",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_CatPercent() {
			AssertExclusions("",feeSchedFee: 0,hasExclusion: true,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_CatPercent() {
			AssertExclusions("",feeSchedFee: _fee,hasExclusion: true,eProcFee: _fee,eWriteOff: -1,ePatPortion: _fee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_CatPercent_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_CatPercent_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_CatPercent_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}
		#endregion

		#region NoBillins Preference
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NoBillIns() {
			PrefT.UpdateBool(PrefName.InsPlanExclusionsMarkDoNotBillIns,true);
			AssertExclusions("",feeSchedFee: _fee,hasExclusion: true,eProcFee: _fee,eWriteOff: -1,ePatPortion: _fee,eNoBillIns: true);
		}
		#endregion

		#region InsEstRecalcReceived
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_InsEstRecalcReceived_True() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			const int coveragePercent=_coveragePercent;
			const double ucrFee=_ucrFee;
			string procStr="D0145";
			string planType="";//percentage
			double feeSchedFee=_fee;
			double eBaseEst=feeSchedFee*(coveragePercent/100d);//Expected BaseEst
			ProcedureCode procCode=ProcedureCodes.GetProcCode(procStr);
			#region Provider Ucr Fee Setup
			long provFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			long provNum=ProviderT.CreateProvider($"Provider {suffix}",feeSchedNum:provFeeSchedNum);
			FeeT.CreateFee(provFeeSchedNum,procCode.CodeNum,ucrFee);
			#endregion
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum);
			#region Fee Schedule Setup
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			if(feeSchedFee>-1) {
				FeeT.CreateFee(feeSchedNum,procCode.CodeNum,feeSchedFee);
			}
			#endregion
			#region InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			double procFee=Procedures.GetProcFee(pat,listPatPlans,listSubs,listPlans,procCode.CodeNum,pat.PriProv,0,"");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Received);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			PrefT.UpdateBool(PrefName.InsEstRecalcReceived,true);//Set InsEstRecalcReceived preference to true.
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,priPlan,priPatPlan.PatPlanNum,listBens,histList,loopList,listPatPlans,0
				,0,pat.Age,0,listPlans,listSubs,listSubLinks,false,null);
			Assert.AreEqual(eBaseEst,priClaimProc.BaseEst);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_InsEstRecalcReceived_False() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			const int coveragePercent=_coveragePercent;
			const double ucrFee=_ucrFee;
			string procStr="D0145";
			string planType="";//percentage
			double feeSchedFee=_fee;
			double eBaseEst=0;//Expected BaseEst will not be recalculated for Received ClaimProc, therefore, will stay 0.
			ProcedureCode procCode=ProcedureCodes.GetProcCode(procStr);
			#region Provider Ucr Fee Setup
			long provFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			long provNum=ProviderT.CreateProvider($"Provider {suffix}",feeSchedNum:provFeeSchedNum);
			FeeT.CreateFee(provFeeSchedNum,procCode.CodeNum,ucrFee);
			#endregion
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum);
			#region Fee Schedule Setup
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			if(feeSchedFee>-1) {
				FeeT.CreateFee(feeSchedNum,procCode.CodeNum,feeSchedFee);
			}
			#endregion
			#region InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			double procFee=Procedures.GetProcFee(pat,listPatPlans,listSubs,listPlans,procCode.CodeNum,pat.PriProv,0,"");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Received);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			PrefT.UpdateBool(PrefName.InsEstRecalcReceived,false);//Set InsEstRecalcReceived preference to false.
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,priPlan,priPatPlan.PatPlanNum,listBens,histList,loopList,listPatPlans,0
				,0,pat.Age,0,listPlans,listSubs,listSubLinks,false,null);
			Assert.AreEqual(eBaseEst,priClaimProc.BaseEst);
		}
		#endregion InsEstRecalcReceived

		#region Secondary PPO

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_PPOSecondaryWriteoffs() {
			PrefT.UpdateBool(PrefName.InsPPOsecWriteoffs,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			int coveragePercentPri=70;
			int coveragePercentSec=80;
			int procFee=100;
			int secAllowed=80;
			string procStr="D0145";
			ProcedureCode procCode=ProcedureCodes.GetProcCode(procStr);
			Patient pat=PatientT.CreatePatient(suffix);
			long feeSchedNumPri=FeeSchedT.CreateFeeSched(FeeScheduleType.OutNetwork,"FeeSchedPri "+suffix);
			long feeSchedNumSec=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSchedSec "+suffix);
			FeeT.CreateFee(feeSchedNumSec,procCode.CodeNum,secAllowed);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,"Pri "+suffix,"");
			ins.AddInsurance(pat,"Sec "+suffix,"p",feeSchedNumSec,2,false,cobRule: EnumCobRule.Standard);
			ins.AddBenefit(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,coveragePercentPri));
			ins.AddBenefit(BenefitT.CreateCategoryPercent(ins.SecInsPlan.PlanNum,EbenefitCategory.Diagnostic,coveragePercentSec));
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.PriInsPlan.PlanNum,ins.PriInsSub.InsSubNum,DateTime.Today,
				-1,-1,-1,ClaimProcStatus.Estimate);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,ins.PriInsPlan,ins.PriPatPlan.PatPlanNum,ins.ListBenefits,histList,loopList,ins.ListPatPlans,0
				,0,pat.Age,0,ins.ListInsPlans,ins.ListInsSubs,ins.ListSubLinks,false,null);
			Assert.AreEqual(70,priClaimProc.InsPayEst);
			Assert.AreEqual(-1,priClaimProc.WriteOffEst);
			ClaimProc secClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.SecInsPlan.PlanNum,ins.SecInsSub.InsSubNum,DateTime.Today,
				-1,-1,-1,ClaimProcStatus.Estimate);
			ClaimProcs.ComputeBaseEst(secClaimProc,proc,ins.SecInsPlan,ins.SecPatPlan.PatPlanNum,ins.ListBenefits,histList,loopList,ins.ListPatPlans,
				priClaimProc.InsPayEst,priClaimProc.InsPayEst,pat.Age,0,ins.ListInsPlans,ins.ListInsSubs,ins.ListSubLinks,false,null);
			Assert.AreEqual(30,secClaimProc.InsPayEst);
			Assert.AreEqual(0,secClaimProc.WriteOffEst);
		}

		#endregion Secondary PPO

		///<summary>This unit test is the first one that looks at the values showing in the claimproc window.
		///This catches situations where the only "bug" is just a display issue in that window.
		///Validates the values in the claimproc window when opened from the Chart module.</summary>
		[TestMethod]
		public void ClaimProcs_FormClaimProc_TextBoxValuesFromChartModule() {
			string suffix="28";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,1300);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,25);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - crown
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.TP,"1",800);//Tooth 1
			ProcedureT.SetPriority(proc1,0);//Priority 1
			//proc2 - crown
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.TP,"9",800);//Tooth 9
			ProcedureT.SetPriority(proc2,1);//Priority 2
			//Lists:
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			//Mimick the TP module estimate calculations when the TP module is loaded. We expect the user to refresh the TP module to calculate insurance estimates for all other areas of the program.
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//Save changes in the list to the database, just like the TP module does when loaded. Then the values can be referenced elsewhere in the program instead of recalculating.
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Validate the estimates within the Edit Claim Proc window are correct when opened from inside of the Chart module by passing in a null histlist and a null looplist just like the Chart module would.
			List<ClaimProcHist> histListNull=null;
			List<ClaimProcHist> loopListNull=null;
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP1=new FormClaimProc(claimProc1,proc1,fam,pat,planList,histListNull,ref loopListNull,patPlans,false,subList);
			formCP1.Initialize();
			Assert.AreEqual("25.00",formCP1.GetTextValue("textDedEst"));
			Assert.AreEqual("412.50",formCP1.GetTextValue("textPatPortion1"));
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP2=new FormClaimProc(claimProc2,proc2,fam,pat,planList,histListNull,ref loopListNull,patPlans,false,subList);
			formCP2.Initialize();
			Assert.AreEqual("0.00",formCP2.GetTextValue("textDedEst"));
			Assert.AreEqual("400.00",formCP2.GetTextValue("textPatPortion1"));
		}

		///<summary>Validates the values in the claimproc window when opened from the Claim Edit window.</summary>
		[TestMethod]
		public void ClaimProcs_FormClaimProc_TextBoxValuesFromClaimEditWindow() {
			string suffix="29";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,1300);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,25);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - crown
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.C,"1",800);//Tooth 1
			ProcedureT.SetPriority(proc1,0);//Priority 1
			//proc2 - crown
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.C,"9",800);//Tooth 9
			ProcedureT.SetPriority(proc2,1);//Priority 2
			//Lists:
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,ProcList,benefitList,subList);//Creates the claim in the same manner as the account module, including estimates.
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Validate the estimates as they would appear inside of the Claim Proc Edit window when opened from inside of the Edit Claim window by passing in the null histlist and null looplist that the Claim Edit window would send in.
			List<ClaimProcHist> histList=null;
			List<ClaimProcHist> loopList=null;
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP1=new FormClaimProc(claimProc1,proc1,fam,pat,planList,histList,ref loopList,patPlans,false,subList);
			formCP1.IsInClaim=true;
			formCP1.Initialize();
			Assert.AreEqual("25.00",formCP1.GetTextValue("textDedEst"));
			Assert.AreEqual("412.50",formCP1.GetTextValue("textPatPortion1"));
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP2=new FormClaimProc(claimProc2,proc2,fam,pat,planList,histList,ref loopList,patPlans,false,subList);
			formCP2.IsInClaim=true;
			formCP2.Initialize();
			Assert.AreEqual("0.00",formCP2.GetTextValue("textDedEst"));
			Assert.AreEqual("400.00",formCP2.GetTextValue("textPatPortion1"));
		}

		///<summary>Validates the values in the claimproc window when opened from the TP module.</summary>
		[TestMethod]
		public void ClaimProcs_FormClaimProc_TextBoxValuesFromTreatPlanModule() {
			string suffix="30";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,1300);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,25);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - crown
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.TP,"1",800);//Tooth 1
			ProcedureT.SetPriority(proc1,0);//Priority 1
			//proc2 - crown
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.TP,"9",800);//Tooth 9
			ProcedureT.SetPriority(proc2,1);//Priority 2
			//Lists:
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			//Mimick the TP module estimate calculations when the TP module is loaded.
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//Save changes in the list to the database, just like the TP module does when loaded.
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Validate the estimates within the Edit Claim Proc window are correct when opened from inside of the TP module by passing in same histlist and loop list that the TP module would.
			histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);//The history list is fetched when the TP module is loaded and is passed in the same for all claimprocs.
			loopList=new List<ClaimProcHist>();//Always empty for the first claimproc.
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP1=new FormClaimProc(claimProc1,proc1,fam,pat,planList,histList,ref loopList,patPlans,false,subList);
			formCP1.Initialize();
			Assert.AreEqual("25.00",formCP1.GetTextValue("textDedEst"));
			Assert.AreEqual("412.50",formCP1.GetTextValue("textPatPortion1"));
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);//The history list is fetched when the TP module is loaded and is passed in the same for all claimprocs.
			loopList=new List<ClaimProcHist>();
			loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,proc1.ProcNum,proc1.CodeNum));
			FormClaimProc formCP2=new FormClaimProc(claimProc2,proc2,fam,pat,planList,histList,ref loopList,patPlans,false,subList);
			formCP2.Initialize();
			Assert.AreEqual("0.00",formCP2.GetTextValue("textDedEst"));
			Assert.AreEqual("400.00",formCP2.GetTextValue("textPatPortion1"));
		}

		/// <summary>
		/// Sets up a plan based on the passed in plan type/fee schedule fee/if the proc should be an exclusion.
		/// eProcFee/eWriteOff/ePatPortion are the expected values of the assertions
		/// </summary>
		private void AssertExclusions(string planType,double feeSchedFee,bool hasExclusion,double eProcFee,double eWriteOff,double ePatPortion,bool eNoBillIns=false) {
			string suffix=MethodBase.GetCurrentMethod().Name;
			const int coveragePercent=50;
			const int ucrFee=50;
			string procStr="D0145";
			ProcedureCode procCode=ProcedureCodes.GetProcCode(procStr);
			#region Provider Ucr Fee Setup
			long provFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			long provNum=ProviderT.CreateProvider($"Provider Exclusion {suffix}",feeSchedNum:provFeeSchedNum);
			FeeT.CreateFee(provFeeSchedNum,procCode.CodeNum,ucrFee);
			#endregion
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum);
			#region Fee Schedule Setup
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			if(feeSchedFee>-1) {
				FeeT.CreateFee(feeSchedNum,procCode.CodeNum,feeSchedFee);
			}
			#endregion
			#region InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			if(hasExclusion) {
				BenefitT.CreateExclusion(priPlan.PlanNum,procCode.CodeNum);
			}
			#endregion
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			double procFee=Procedures.GetProcFee(pat,listPatPlans,listSubs,listPlans,procCode.CodeNum,pat.PriProv,0,"");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ClaimProcStatus cps=ClaimProcStatus.NotReceived;
			if(planType=="c") {
				cps=ClaimProcStatus.CapEstimate;
			}
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,cps);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,priPlan,priPatPlan.PatPlanNum,listBens,histList,loopList,listPatPlans,0
				,0,pat.Age,0,listPlans,listSubs,listSubLinks,false,null);
			#region Assert
			BenefitsAssertItem assertItem=new BenefitsAssertItem();
			assertItem.PrimaryClaimProc=priClaimProc;
			assertItem.Procedure=proc;
			Assert.AreEqual(eProcFee,assertItem.Procedure.ProcFee,"ProcFee calculation");
			Assert.AreEqual(eWriteOff,assertItem.PrimaryClaimProc.WriteOffEst,"WriteOffEst calculation");
			double patPort=(double)ClaimProcs.GetPatPortion(assertItem.Procedure,new List<ClaimProc>() { assertItem.PrimaryClaimProc });
			Assert.AreEqual(ePatPortion,patPort,"PatPortion calculation");
			Assert.AreEqual(eNoBillIns,assertItem.PrimaryClaimProc.NoBillIns,"NoBillIns");
			#endregion
		}

		private class BenefitsAssertItem {
			public Procedure Procedure;
			public ClaimProc PrimaryClaimProc;

			public BenefitsAssertItem() { }

		}
	}
}
