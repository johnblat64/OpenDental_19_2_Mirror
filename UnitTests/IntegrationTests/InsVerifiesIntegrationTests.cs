using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness.Eclaims;
using UnitTestsCore;
using OpenDentBusiness;
using System.Reflection;

namespace UnitTests.IntegrationTests {
	[TestClass]
	public class InsVerifiesIntegrationTests {
		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		[TestMethod]
		public void InsVerifies_Fake271Response_ShouldPassValidation() {
			AppointmentT.ClearAppointmentTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			x270Controller.FakeResponseOverride271="ISA*00*          *00*          *30*330989922      *29*AA0989922      *030606*0936*U*00401*000013966*0*T*:~GS*HB*330989922*AA0989922*20030606*0936*13966*X*004010X092~ST*271*0001~BHT*0022*11*ASX012145WEB*20030606*0936~HL*1**20*1~NM1*PR*2*ACMEINC*****PI*12345~HL*2*1*21*1~NM1*1P*1*PROVLAST*PROVFIRST****SV*5558006~HL*3*2*22*0~TRN*2*100*1330989922~NM1*IL*1*SMITH*JOHN*B***MI*123456789~REF*6P*XYZ123*GROUPNAME~REF*18*2484568*TEST PLAN NAME~N3*29 FREMONT ST*~N4*PEACE*NY*10023~DMG*D8*19570515*M~DTP*307*RD8*19910712-19920525~EB*1*FAM*30~SE*17*0001~GE*1*13966~IEA*1*000013966~";
			long provNum=ProviderT.CreateProvider("prov1","terry","smith",ssn:"123456789",isUsingTIN:true,nationalProvID:"0123456789");
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,provNum);
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum,birthDate:DateTime.Today);
			//Create clearinghouse, set required fields for benefit request validation, and set as default.
			Clearinghouse ch=ClearinghouseT.CreateClearinghouse("ClaimConnect",commBridge:EclaimsCommBridge.ClaimConnect,eFormat:ElectronicClaimFormat.x837D_5010_dental,isa05:"ZZ",
				isa07:"30",isa08:"BCBSGA",isa15:"P");
			Prefs.UpdateLong(PrefName.ClearinghouseDefaultEligibility,ch.ClearinghouseNum);
			Prefs.RefreshCache();
			Clearinghouses.Insert(ch);
			//Create carrier and set required fields for benefit request validation
			Carrier carrier=CarrierT.CreateCarrier(suffix,"123 boring st","boring","OR","97306","1234",arrayTrustedEtrans:TrustedEtransTypes.RealTimeEligibility);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum,groupNum:"XYZ123");//Group number should be >3 characters so it passes insverify validation
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,pat.PatNum,insSub.InsSubNum);
			Operatory op=OperatoryT.CreateOperatory();
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today,op.OperatoryNum,provNum);
			List<InsVerify> listInsVerifies=InsVerifies.TryBatchPatInsVerify();
			//Should only be one patient in the list and they should have passed validation.
			Assert.IsTrue(listInsVerifies.FirstOrDefault().BatchVerifyState==BatchInsVerifyState.Success);
		}
	}
}
