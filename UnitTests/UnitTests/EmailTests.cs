﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

		///<summary>Ensures the body of an email can be decoded properly when the raw text is given in various formats, as would be seen when sent from
		///different email clients.  Includes html with multi-byte characters to verify these characters are decoded correctly.</summary>
		[TestMethod]
		public void EmailMessages_DecodeBodyText_VariousFormats() {
			List<Tuple<string,bool,string>> listTestSubjects=new List<Tuple<string,bool,string>>() {
				//Raw,Expected
				//Godaddy with =3D typed in the email
				new Tuple<string,bool,string>(@"<html><body><span style=3D""font-family:Verdana; color:#000000; font-size:10pt;""><div><span style=3D"""">test =3D3D more text =3D</span></div></span></body></html>"
					,false,@"<html><body><span style=""font-family:Verdana; color:#000000; font-size:10pt;""><div><span style="""">test =3D more text =</span></div></span></body></html>"),
				//Godaddy with various non-ascii characters.
				new Tuple<string,bool,string>(@"<html><body><span style=3D""font-family:Verdana; color:#000000; font-size:10pt;""><div class=3D""gs"" style=3D""""><div class=3D"""" style=3D""""><div id=3D"":n1"" class=3D""ii gt"" style=3D""""><div id=3D"":n0"" class=3D""a3s aXjCH "" style=3D""""><div dir=3D""ltr"" style=3D"""">chuck nu=C2=A4 =C3=82 =C3=80 =C2=A2<div class=3D""yj6qo"" style=3D""""></div><div class=3D""adL"" style=3D""""><br style=3D""""></div></div><div class=3D""adL"" style=3D""""></div></div></div><div class=3D""hi"" style=3D""""></div></div></div></span></body></html>"
					,false,@"<html><body><span style=""font-family:Verdana; color:#000000; font-size:10pt;""><div class=""gs"" style=""""><div class="""" style=""""><div id="":n1"" class=""ii gt"" style=""""><div id="":n0"" class=""a3s aXjCH "" style=""""><div dir=""ltr"" style="""">chuck nu¤ Â À ¢<div class=""yj6qo"" style=""""></div><div class=""adL"" style=""""><br style=""""></div></div><div class=""adL"" style=""""></div></div></div><div class=""hi"" style=""""></div></div></div></span></body></html>"),
				//Gmail base64
				new Tuple<string,bool,string>(@"<div dir=""ltr"">chuck nu¤ Â À ¢<br></div>",true
					,@"<div dir=""ltr"">chuck nu¤ Â À ¢<br></div>"),
				//Gmail non-base64 with =3D typed in email and a new line at the end.
				new Tuple<string,bool,string>(@"=3D and other things 
",false,@"= and other things 
"),
				//Gmail non-base64 with multiple lines and plain text
				//Gmail non-base64 with =3D typed in email and a new line at the end.
				new Tuple<string,bool,string>(@"plain text
multiple lines

",false,@"plain text
multiple lines

"),
			};
			Encoding encoding=Encoding.GetEncoding("utf-8");
			foreach(Tuple<string,bool,string> test in listTestSubjects) {
				Assert.AreEqual(test.Item3,EmailMessages.DecodeBodyText("=",test.Item1,test.Item2,encoding));
			}
		}

		///<summary>Ensures email subject lines with various formatting and special characters (ascii vs non-ascii) are interpreted correctly.</summary>
		[TestMethod]
		public void EmailMessages_ProcessMimeSubject_VariousFormats() {
			List<Tuple<string,string>> listTestSubjects=new List<Tuple<string,string>>() {
				//Raw,Expected
				new Tuple<string,string>("=?UTF-8?B?RndkOiDCoiDDhiAxMjM0NSDDpiDDvyBzb21lIGFzY2lpIGNoYXJzIMOCIMOD?=","Fwd: ¢ Æ 12345 æ ÿ some ascii chars Â Ã"),
				new Tuple<string,string>("=?UTF-8?Q?nu=C2=A4=20=C3=82=20=C3=80=20=C2=A2?=","nu¤ Â À ¢"),
				new Tuple<string,string>("[FWD: test =?UTF-8?Q?=3D=33D=20more=20text=20=3D=5D?=","[FWD: test =3D more text =]"),
				new Tuple<string,string>("regular old plain text subject line","regular old plain text subject line"),
				new Tuple<string,string>("",""),
			};
			foreach(Tuple<string,string> test in listTestSubjects) {
				Assert.AreEqual(test.Item2,EmailMessages.ProcessMimeSubject(test.Item1));
			}
		}
	}
}
