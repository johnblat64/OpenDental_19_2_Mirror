using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebForms;
using UnitTestsCore;

namespace UnitTests.WebForms_Sheets_Tests {
	[TestClass]
	public class WebForms_SheetsTests:TestBase {

		///<summary>Tests to see that the webforms matching function is working correctly. Tries multiple combinations to prove it is working.</summary>
		[TestMethod]
		public void WebForms_Sheets_FindSheetsForPat_Matching() {
			WebForms_Sheet sheetNoPhones=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,1),"bob@bobby.com",new List<string>());
			WebForms_Sheet sheetNoPhonesMatching=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,1),"bob@bobby.com",
				new List<string>());
			WebForms_Sheet sheetCloseMatch=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,2),"bob@bobby.com",new List<string>());
			WebForms_Sheet sheetPhoneNumber=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,2),"bob@bobby.com",
				new List<string> { "5555555555" });
			WebForms_Sheet sheetPhoneNumberMatching=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,2),"bob@bobby.com",
				new List<string> { "5555555555" });
			WebForms_Sheet sheetTooManyPhones=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,2),"bob@bobby.com",
				new List<string> { "5555555555","4444444444" });
			WebForms_Sheet sheetDifferentName=WebForms_SheetT.CreateWebFormSheet("Bobby","Bobby",new DateTime(2018,1,1),"bob@bobby.com",
				new List<string>());
			//Exact Match.
			Assert.AreEqual(1,WebForms_Sheets.FindSheetsForPat(sheetNoPhones,new List<WebForms_Sheet> { sheetNoPhonesMatching },"en-us").Count);
			//Close except different birthday.
			Assert.AreEqual(0,WebForms_Sheets.FindSheetsForPat(sheetNoPhones,new List<WebForms_Sheet> { sheetCloseMatch },"en-us").Count);
			//Exact Match.
			Assert.AreEqual(1,WebForms_Sheets.FindSheetsForPat(sheetPhoneNumber,new List<WebForms_Sheet> { sheetPhoneNumberMatching },"en-us").Count);
			//One form has more phone numbers.
			Assert.AreEqual(0,WebForms_Sheets.FindSheetsForPat(sheetPhoneNumber,new List<WebForms_Sheet> { sheetTooManyPhones },"en-us").Count);
			//No match. Different name.
			Assert.AreEqual(0,WebForms_Sheets.FindSheetsForPat(sheetTooManyPhones,new List<WebForms_Sheet> { sheetDifferentName },"en-us").Count);
			//A sheet will always match on itself.
			Assert.AreEqual(1,WebForms_Sheets.FindSheetsForPat(sheetDifferentName,new List<WebForms_Sheet> { sheetDifferentName },"en-us").Count);
		}

	}
}
