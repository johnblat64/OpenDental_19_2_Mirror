using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class InsSubT {
		///<summary></summary>
		public static InsSub CreateInsSub(long subscriberNum,long planNum,string subscriberID="1234") {
			InsSub sub=new InsSub();
			sub.Subscriber=subscriberNum;
			sub.PlanNum=planNum;
			sub.SubscriberID=subscriberID;
			InsSubs.Insert(sub);
			return sub;
		}

		public static List<InsSub> GetInsSubs(Patient pat) {
			Family fam=Patients.GetFamily(pat.PatNum);
			return InsSubs.RefreshForFam(fam);
		}

		public static InsSub GetSubForPriSecMed(PriSecMed priSecMed,List<PatPlan> listPatPlans,List<InsPlan> listPlans,List<InsSub> listSubs) {
			long subNum=PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(priSecMed,listPatPlans,listPlans,listSubs));
			return InsSubs.GetSub(subNum,listSubs);
		}

	}
}
