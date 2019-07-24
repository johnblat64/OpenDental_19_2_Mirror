using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.UserQueries_Tests {
	[TestClass]
	public class UserQueriesTests:TestBase {
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		public static List<string> CreateList(params string[] list) {
			List<string> retVal=new List<string>();
			foreach(string str in list) {
				retVal.Add(str);
			}
			return retVal;
		}

		private static List<string> SplitQuery(string strQuery,string strSplit=";") {
			List<string> stmts = UserQueries.SplitQuery(strQuery,false,strSplit);
			UserQueries.TrimList(stmts);
			stmts.RemoveAll(x => string.IsNullOrEmpty(x));
			return stmts;
		}

		/// <summary>Asserts that UserQueries.SplitQuery() splits strQuery up into a list of strings that matches listExpectedStrings.</summary>
		/// <param name="strQuery">Provide a query that will be split up.</param>
		/// <param name="listExpectedStrings">Provide the list of strings that strQuery should get split up into.</param>
		private static void RunSplitQueryTest(string strQuery,List<string> listExpectedStrings) {
			List<string> listSplitQueries=SplitQuery(strQuery);
			foreach(string strSplit in listSplitQueries) {
				Assert.IsTrue(listExpectedStrings.Contains(strSplit));
			}
		}

		/// <summary>Assert.True UserQueries.ParseSetStatements() matches listExpectedStrings.</summary>
		/// <param name="strQuery">Provide a query that will be split up.</param>
		/// <param name="listExpectedStrings">Provide the list of strings that strQuery should get split up into.</param>
		private static void RunParseSetStatementsTest(string strQuery,List<string> listExpectedStrings) {
			List<string> listSplitQueries=UserQueries.ParseSetStatements(strQuery);
			foreach(string strSplit in listSplitQueries) {
				Assert.IsTrue(listExpectedStrings.Contains(strSplit));
			}
		}

		[TestMethod]
		public void UserQueries_SplitQuery_CaseStatement() {
			string strQuery=@"SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 ELSE 10 END);
				SELECT @Test;";
			RunSplitQueryTest(strQuery,CreateList("SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 ELSE 10 END)","SELECT @Test"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_CaseStatementMissingEnd() {
			string strQuery=@"SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 ELSE 10);
				SELECT @Test;";
			RunSplitQueryTest(strQuery,CreateList("SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 ELSE 10)","SELECT @Test"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_MoreThanOneSetStmt() {
			string strQuery=@"SET @FromDate='2012-12-03' , @ToDate='2016-12-03';
			SET @CarrierName='CSC Medicaid'; 
			SET @OperatoryName='%%';
			SELECT * FROM patient;";
			RunSplitQueryTest(strQuery,CreateList("SET @FromDate='2012-12-03' , @ToDate='2016-12-03'","SET @CarrierName='CSC Medicaid'",
				"SET @OperatoryName='%%'","SELECT * FROM patient"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_MultiCaseStatement() {
			string strQuery=@"SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1  CASE WHEN CURDATE()='2019-05-20' THEN 55 ELSE 10 END);
				SELECT @Test;";
			RunSplitQueryTest(strQuery,CreateList("SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1  CASE WHEN CURDATE()='2019-05-20' THEN 55 ELSE 10 END)","SELECT @Test"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_OneSetStmt() {
			string strQuery=@"SET @ExcAllergy='%None%'; 
				SELECT * FROM allergydef";
			RunSplitQueryTest(strQuery,CreateList("SET @ExcAllergy='%None%'","SELECT * FROM allergydef"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_SetStmtMissingQuote() {
			string strQuery=@"SET @FromDate='2012-12-03 , @ToDate=2016-12-03';
				SET @FromDate2='2012-12-03 , @ToDate2=2016-12-03;
				SET @CarrierName='CSC Medicaid'; 
				SET @OperatoryName='%%';
				SELECT * FROM patient;";
			RunParseSetStatementsTest(strQuery,CreateList("SET @FromDate='2012-12-03 , @ToDate=2016-12-03'","SET @CarrierName='CSC Medicaid'",
				"SET @OperatoryName='%%'","SELECT * FROM patient","SET @FromDate2='2012-12-03 , @ToDate2=2016-12-03"));
		}

	}
}
