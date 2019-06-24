using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.InsVerify_Tests {
	[TestClass]
	public class InsVerifyTests:TestBase {
		
		private static long _patNumTrusted;
		private static long _patNumTrustedSecond;
		private static long _patNumNotTrusted;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			AppointmentT.ClearAppointmentTable();//Make sure any other appts do not show on insurance verification list.
			ClearinghouseT.ClearClearinghouseTable();//Must be cleared because we expect the 270 request to fail when a real request is attempted.
			_patNumTrusted=PatientT.CreatePatient().PatNum;
			CreateNewCarrierAndPatPlan(_patNumTrusted,true);
			AppointmentT.CreateAppointment(_patNumTrusted,DateTime.Now.AddDays(2),0,0);//Causes patient patplan to show in insurance verification list.
			_patNumTrustedSecond=PatientT.CreatePatient().PatNum;
			CreateNewCarrierAndPatPlan(_patNumTrustedSecond,true);
			AppointmentT.CreateAppointment(_patNumTrustedSecond,DateTime.Now.AddDays(3),0,0);//Causes patient patplan to show in insurance verification list.
			_patNumNotTrusted=PatientT.CreatePatient().PatNum;
			CreateNewCarrierAndPatPlan(_patNumNotTrusted,false);
			AppointmentT.CreateAppointment(_patNumNotTrusted,DateTime.Now.AddDays(4),0,0);//Causes patient patplan to show in insurance verification list.
		}

		[TestMethod]
		public void InsVerifies_VerifyDateTest() {
			List<List<int>> testCases=new List<List<int>>();
			#region testCase setup
			for(int i=0;i<14;i++) {
				testCases.Add(null);
				switch(i) {
					case 0:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 1:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 2:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 3:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 4:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 5:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 6:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 7:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 8:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 9:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 10:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 11:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 12:
						testCases[i]=new List<int> { 
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime,
							(int)eventType.NextScheduledVerifyDate
						};
						break;
					case 13:
						testCases[i]=new List<int> { 
							(int)eventType.DateLastVerified,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime,
							(int)eventType.NextScheduledVerifyDate
						};
						break;
					default:
						break;
				}
			}
			#endregion
			//This is a list of the possible dates times.
			//For each of the test cases the first items in the case list gets the first date, second gets the second etc...
			List<DateTime> dates=new List<DateTime> {
				new DateTime(2018,2,23,0,0,0),	//2-23-2018    0:00
				new DateTime(2018,2,23,1,0,0),	//2-23-2018		1:00
				new DateTime(2018,2,23,2,0,0),	//"       "		2:00
				new DateTime(2018,2,23,3,0,0),	//etc etc....
				new DateTime(2018,2,23,4,0,0),
				new DateTime(2018,2,23,5,0,0),
				new DateTime(2018,2,23,6,0,0)
			};
			//List of "Hours" we know are right. The hour corresponds to the index of the test case list that is correct.
			List<int> expectedResults=new List<int> {3,3,3,3,3,3,3,3,3,3,3,4,3,3};	//expectedResults[i] == results of passing test of testCases[i]
			//Loop through and verify test cases
			Console.WriteLine("See [[Insurance Verification - Hours Available for Verification]] for the visual diagram corresponding to the test number.");
			for(int i=0;i<testCases.Count;i++) { 
				List<int> items=testCases[i];
				DateTime daysForVerification=DateTime.MaxValue;	//Max value so they're out of the way
				DateTime appointmentDate=DateTime.MaxValue;
				DateTime needsVerifyDate=DateTime.MaxValue;
				DateTime lastVerifyDate=DateTime.MaxValue;
				DateTime benifitRenewalDate=DateTime.MaxValue;
				#region TimeCreation
				int currentDate=0;
				foreach(int index in items) {
					switch(index) {
						case (int)eventType.DateLastVerified:
							lastVerifyDate=dates[currentDate];
							currentDate++;
							break;
						case (int)eventType.NextScheduledVerifyDate:
							needsVerifyDate=dates[currentDate];
							currentDate++;
							break;
						case (int)eventType.DateTimeApptWasScheduled:
							appointmentDate=dates[currentDate];
							currentDate++;
							break;
						case (int)eventType.ApptSchedDaysMaxDaysForVerification:
							daysForVerification=dates[currentDate];
							currentDate++;
							break;
						case (int)eventType.DateTimeInsuranceRenewalNeeded:
							benifitRenewalDate=dates[currentDate];
							currentDate++;
							break;
						//These two aren't used in the calculation, so we just increment the datetime index
						case (int)eventType.AptDateTime:
							currentDate++;
							break;
						case (int)eventType.DateTimeVerified:
							currentDate++;
							break;
					}
				}
				#endregion
				DateTime resultDate=InsVerifies.VerifyDateCalulation(daysForVerification,appointmentDate,needsVerifyDate,benifitRenewalDate);
				Console.WriteLine((expectedResults[i]==resultDate.Hour ? "PASSED: " : "FAILED: ")+"Test "+(i+1));
				Assert.AreEqual(expectedResults[i],resultDate.Hour);//If the times are equal, then the result is correct.
			}
		}

		[TestMethod]
		public void InsVerifies_TryBatchPatInsVerify_NoRequestNeeded() {
			//No appointment created so no need for batch ins verify.
			//AppointmentT.ClearAppointmentTable();
			List<InsVerify> listInsVerify=InsVerifies.TryBatchPatInsVerify(true);
			Assert.IsTrue(listInsVerify.Count==3 
				&& listInsVerify[0].BatchVerifyState==BatchInsVerifyState.Success
				&& listInsVerify[1].BatchVerifyState==BatchInsVerifyState.Error
				&& listInsVerify[2].BatchVerifyState==BatchInsVerifyState.Skipped
			);
		}


		///<summary>Inserts all DB insurance rows needed for a patient with an appointment to show up on for insurance verificaiton.</summary>
		private static void CreateNewCarrierAndPatPlan(long patNum,bool isTrusted){
			TrustedEtransTypes[] arrayTrustedVals=null;
			if(isTrusted) { 
				arrayTrustedVals=new TrustedEtransTypes[] { TrustedEtransTypes.RealTimeEligibility };
			}
			Carrier carrier=CarrierT.CreateCarrier("BatchInsVerifyCarrier_"+POut.Long(patNum),arrayTrustedVals);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patNum,insPlan.PlanNum);
			PatPlanT.CreatePatPlan(1,patNum,insSub.InsSubNum);
		}

		private enum eventType {
			DateLastVerified,
			NextScheduledVerifyDate,
			DateTimeApptWasScheduled,
			AptDateTime,
			ApptSchedDaysMaxDaysForVerification,
			DateTimeInsuranceRenewalNeeded,
			DateTimeVerified
		}
	}
}
