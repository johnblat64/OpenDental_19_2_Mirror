﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ClaimProcT {

		public static ClaimProc AddInsUsedAdjustment(long patNum,long planNum,double insPayAmt,long subNum,double dedApplied){
			ClaimProc cp=ClaimProcs.CreateInsPlanAdjustment(patNum,planNum,subNum);
			cp.InsPayAmt=insPayAmt;
			cp.DedApplied=dedApplied;
			ClaimProcs.Insert(cp);
			return cp;
		}

		public static void AddInsPaid(long patNum,long planNum,long procNum,double amtPaid,long subNum,double dedApplied,double writeOff) {
			AddInsPaid(patNum,planNum,procNum,amtPaid,subNum,dedApplied,writeOff,DateTime.Today);
		}

		public static void AddInsPaidAsTotal(long patNum,long planNum,long provNum,double amtPaid,long subNum,double dedApplied,double writeOff) {
			ClaimProc cp=new ClaimProc();
			cp.PatNum=patNum;
			cp.PlanNum=planNum;
			cp.ProvNum=provNum;
			cp.InsSubNum=subNum;
			cp.InsPayAmt=amtPaid;
			cp.DedApplied=dedApplied;
			cp.WriteOff=writeOff;
			cp.Status=ClaimProcStatus.Received;
			cp.DateCP=DateTime.Today;
			cp.ProcDate=DateTime.Today;
			ClaimProcs.Insert(cp);
		}

		///<summary>This tells the calculating logic that insurance paid on a procedure.  It avoids the creation of an actual claim.</summary>
		public static void AddInsPaid(long patNum,long planNum,long procNum,double amtPaid,long subNum,double dedApplied,double writeOff,DateTime procDate) {
			ClaimProc cp=new ClaimProc();
			cp.ProcNum=procNum;
			cp.PatNum=patNum;
			cp.PlanNum=planNum;
			cp.InsSubNum=subNum;
			cp.InsPayAmt=amtPaid;
			cp.DedApplied=dedApplied;
			cp.WriteOff=writeOff;
			cp.Status=ClaimProcStatus.Received;
			cp.DateCP=DateTime.Today;
			cp.ProcDate=procDate;
			ClaimProcs.Insert(cp);
		}

		public static ClaimProc CreateClaimProc(long patNum,long procNum,long planNum,long insSubNum,DateTime procDate,double copayOverride,
			double allowedOverride,int percentOverride,ClaimProcStatus cps=ClaimProcStatus.NotReceived,int insPayAmt=0)
		{
			ClaimProc cp=new ClaimProc();
			cp.PatNum=patNum;
			cp.ProcNum=procNum;
			cp.PlanNum=planNum;
			cp.ProcDate=procDate;
			cp.InsSubNum=insSubNum;
			cp.CopayOverride=copayOverride;
			cp.AllowedOverride=allowedOverride;
			cp.PercentOverride=percentOverride;
			cp.InsEstTotalOverride=-1;
			cp.WriteOffEstOverride=-1;
			cp.PaidOtherInsOverride=-1;
			cp.Status=cps;
			cp.InsPayAmt=insPayAmt;
			ClaimProcs.Insert(cp);
			return cp;
		}

		///<summary>Deletes everything from the claimproc table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearClaimProcTable() {
			string command="DELETE FROM claimproc";
			DataCore.NonQ(command);
		}


	}
}
