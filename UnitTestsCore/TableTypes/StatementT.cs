using System;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class StatementT {

		public static Statement CreateStatement(long patNum,StatementMode mode_=StatementMode.InPerson,bool isSent=false,DateTime dateSent=default) {
			Statement statement=new Statement() {
				PatNum=patNum,
				Mode_=mode_,
				IsSent=isSent,
				DateSent=dateSent,
			};
			Statements.Insert(statement);
			return statement;
		}
	}
}
