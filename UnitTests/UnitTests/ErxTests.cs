using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Erx_Tests {
	[TestClass]
	public class ErxTests:TestBase {
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

		[TestMethod]
		public void DoseSpot_IsPhoneNumberValid() {
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("(959)-230-4007"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("9592304007"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("959-230-4007"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("(959)230-4007"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("(959)230-4007X1234"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("(959)230-4007x1234"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-(959)230-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-959-230-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-(959)230-4007X1234"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-(959)230-4007x1234"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-(959)230-40071"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("(959)230t-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("(555)230-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("(959)222-2222"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("(059)230-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("(159)230-4007"));
		}

	}
}
