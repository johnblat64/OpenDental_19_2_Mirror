using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestsCore;
using OpenDental;
using System.Reflection;

namespace UnitTests.Email_Tests {
	[TestClass]
	public class EmailTests:TestBase {
		[TestMethod]
		public void EmailMessages_FindAndReplacePostalAddressTag() {
			//Format disclaimer.
			PrefT.UpdateString(PrefName.EmailDisclaimerTemplate,"This email has been sent to you from:\r\n[PostalAddress].\r\n\r\nHow to unsubscribe:\r\nIf you no longer want to receive any email messages from us, simply reply to this email with the word \"unsubscribe\" in the subject line.");
			//Setup practice address.
			PrefT.UpdateString(PrefName.PracticeAddress,"Practice Address1 Here");
			PrefT.UpdateString(PrefName.PracticeAddress2,"3275 Marietta St SE");
			PrefT.UpdateString(PrefName.PracticeCity,"Salem");
			PrefT.UpdateString(PrefName.PracticeST,"OR");
			PrefT.UpdateString(PrefName.PracticeZip,"97317");
			//Setup clinic address.
			Clinic clinic=ClinicT.CreateClinic();
			clinic.Address="Clinic Address1 Here";
			Clinics.Update(clinic);
			Clinics.RefreshCache();
			//Turn feature off.
			PrefT.UpdateBool(PrefName.EmailDisclaimerIsOn,false);
			string emailBody="Hi, this is an email.\r\n\r\nRegards,\r\nEvery OD Engineer... ever.";
			string emailBodyWithDisclaimer=EmailMessages.FindAndReplacePostalAddressTag(emailBody,0);
			//Feature is off so no disclaimer added.
			Assert.AreEqual(emailBody,emailBodyWithDisclaimer);
			//Turn feature on.
			PrefT.UpdateBool(PrefName.EmailDisclaimerIsOn,true);
			//Turn clinics off.
			PrefT.UpdateBool(PrefName.EasyNoClinics,true);
			emailBodyWithDisclaimer=EmailMessages.FindAndReplacePostalAddressTag(emailBody,0);
			//Feature is on so disclaimer added (no clinic).
			Assert.AreNotEqual(emailBody,emailBodyWithDisclaimer);
			Assert.IsTrue(emailBodyWithDisclaimer.EndsWith("subject line."));
			Assert.IsTrue(emailBodyWithDisclaimer.Contains("Practice Address"));
			Assert.IsFalse(emailBodyWithDisclaimer.Contains("Clinic Address"));
			//Turn clinics on.
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);
			emailBodyWithDisclaimer=EmailMessages.FindAndReplacePostalAddressTag(emailBody,clinic.ClinicNum);
			//Feature is on so disclaimer added (with clinic).
			Assert.AreNotEqual(emailBody,emailBodyWithDisclaimer);
			Assert.IsTrue(emailBodyWithDisclaimer.EndsWith("subject line."));
			Assert.IsTrue(emailBodyWithDisclaimer.Contains("Clinic Address"));
			Assert.IsFalse(emailBodyWithDisclaimer.Contains("Practice Address"));
		}

		[TestMethod]
		public void EmailPreviewControl_ReplaceTemplateFields_AreAllReplacementStringsReplaced() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			ReferralT.CreateReferral(pat.PatNum);//LoadTemplate(...) will use this referral.
			RecallT.CreateRecall(pat.PatNum,0,DateTime.Now,new Interval());//LoadTemplate(...) will use this recall.
			string subject="";
			MessageReplaceType typesAll=(MessageReplaceType)Enum.GetValues(typeof(MessageReplaceType)).OfType<MessageReplaceType>().Sum(x => (int)x);
			foreach(FormMessageReplacements.ReplacementField field in FormMessageReplacements.GetReplacementFieldList(true,typesAll)) {
				if(field.IsSupported) {
					subject+=field.FieldName;
				}
			}
			subject=EmailPreviewControl.ReplaceTemplateFields(subject,pat,null,Clinics.GetClinic(pat.ClinicNum));
			Assert.IsFalse(subject.Any(x => x==']' || x=='['));
		}
	}
}
