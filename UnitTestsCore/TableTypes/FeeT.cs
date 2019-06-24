using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class FeeT {

		///<summary>Inserts a new fee, refreshes the cache and then returns FeeNum.</summary>
		public static long CreateFee(long feeSchedNum,long codeNum,double amount=0,long clinicNum=0,long provNum=0,bool hasCacheRefresh=true) {
			long feeNum=GetNewFee(feeSchedNum,codeNum,amount,clinicNum,provNum,hasCacheRefresh).FeeNum;
			return feeNum;
		}

		public static Fee GetNewFee(long feeSchedNum,long codeNum,double amount=0,long clinicNum=0,long provNum=0,bool hasCacheRefresh=true) {
			Fee fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum;
			fee.Amount=amount;
			fee.ClinicNum=clinicNum;
			fee.ProvNum=provNum;
			fee.FeeNum=Fees.Insert(fee);
			Fees.InvalidateFeeSchedule(fee.FeeSched);
			if(hasCacheRefresh && PrefC.FeesUseCache) {
				RefreshCache();
			}
			return fee;
		}

		public static void RefreshCache() {
			Fees.FillCache();
		}


	}
}
