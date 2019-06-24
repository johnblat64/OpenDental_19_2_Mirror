using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class SubstitutionLinkT {
		public static SubstitutionLink CreateSubstitutionLink(long codeNum,string subCode,SubstitutionCondition substOnlyIf,long planNum) {
			SubstitutionLink subLink=new SubstitutionLink();
			subLink.CodeNum=codeNum;
			subLink.SubstitutionCode=subCode;
			subLink.SubstOnlyIf=substOnlyIf;
			subLink.PlanNum=planNum;
			SubstitutionLinks.Insert(subLink);
			return subLink;
		}
	}
}
