using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.FeeCache_Tests {
	[TestClass]
	public class FeeCacheTests:FeeTestBase {
		private const string _feeDeleteDuplicatesExpectedResult=@"Procedure codes with duplicate fee entries: 0
   Double click to see a break down.";

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			FeeTestSetup();
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			PrefT.UpdateBool(PrefName.FeesUseCache,true);
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		/// <summary>Test the FeeCache initializes with the expected standard fees.</summary>
		[TestMethod]
		public void FeeCache_Initialize() {
			FeeTestArgs args=CreateManyFees(1,1,1,MethodBase.GetCurrentMethod().Name);
			Clinics.ClinicNum=args.ListClinics.First().ClinicNum;
			FeeCache cache=new FeeCache();
			//If we are running multiple tests, it is possible that some additional fees with clinic 0 were created, and these would be loaded into the cache,
			//which is the correct behavior. To check this is right, first check that all of the expected fees are there
			List<Fee> listExpected=args.ListFees.OrderBy(x => x.FeeNum).ToList();
			List<Fee> listActual=cache.Where(x => x.FeeSched==_standardFeeSchedNum || args.ListFeeSchedNums.Contains(x.FeeSched)).OrderBy(x => x.FeeNum).ToList();
			Assert.IsTrue(AreListsSimilar(listExpected,listActual));
			//Second, we check to make sure that no additional clinics, beyond HQ and the current clinic were loaded into the cache.
			Assert.IsNull(cache.Where(x => x.ClinicNum!=0 && x.ClinicNum!=Clinics.ClinicNum).FirstOrDefault());
		}

		///<summary>Fill the cache with the fees from the HQ clinic and the currently selected Clinic.</summary>
		[TestMethod]
		public void FeeCache_FillCache() {
			FeeTestArgs args=CreateManyFees(1,1,1,MethodBase.GetCurrentMethod().Name);
			Clinics.ClinicNum=args.ListClinics.First().ClinicNum;
			List<Fee> listExpectedFees=args.ListFees.OrderBy(x => x.FeeNum).ToList();
			Fees.FillCache();
			FeeCache cacheCopy=Fees.GetCache();
			//It is possible another test created a fee with a different fee schednum and clinicNum=0. The fee cache *should* load these if they exist
			//but for the purpose of the test it is simpler to just exclude them here.
			List<Fee> listCachedFees=cacheCopy.Where(x => !(x.ClinicNum==0 && x.FeeSched!=_standardFeeSchedNum)).OrderBy(x => x.FeeNum).ToList();
			Assert.IsTrue(AreListsSimilar(listExpectedFees,listCachedFees));
		}

		/// <summary>Creates a number of fees, loads the fees into the cache, invalidates a fee schedule and makes a copy. Checks that the invalidated 
		/// fee schedule is refreshed and loaded into the copy.</summary>
		[TestMethod]
		public void FeeCache_GetCopy() {
			FeeTestArgs args=CreateManyFees(5,3,2,MethodBase.GetCurrentMethod().Name);
			FeeCache cache=new FeeCache();
			cache.GetFeesForClinics(args.ListClinics.Select(x => x.ClinicNum)); //load these clinics into memory
			long invalidatedFeeSched = args.ListFeeSchedNums.Last();
			Fee fee=args.ListFees.First(x => x.FeeSched==invalidatedFeeSched);
			//Before we invalidate the fee sched, make sure it's actually in our cache
			Assert.AreEqual(fee.Amount,cache.GetFee(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum).Amount); 
			cache.Invalidate(invalidatedFeeSched);
			FeeCache copiedCache = (FeeCache)cache.GetCopy();
			Assert.AreEqual(fee.Amount,copiedCache.GetFee(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum).Amount);
		}

		///<summary>Create a single fee and add to the local cache only.</summary>
		[TestMethod]
		public void FeeCache_AddFee() {
			FeeCache cache=new FeeCache();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name);
			Fee fee=new Fee()
			{
				FeeSched=feeSchedNum,
				ClinicNum=0,
				ProvNum=0,
				CodeNum=_listProcCodes[_rand.Next(_listProcCodes.Count-1)].CodeNum,
				Amount=_defaultFeeAmt
			};
			cache.Add(fee);
			Assert.IsNotNull(cache.GetFee(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum,true));
		}

		///<summary>Fill the cache, then get a fee from a clinic. Use to load test the time it takes to fill the cache with a new clinic.</summary>
		[TestMethod]
		public void FeeCache_AddClinic() {
			FeeTestArgs args=CreateManyFees(1,1,1,MethodBase.GetCurrentMethod().Name);
			FeeCache cache=new FeeCache();
			List<Fee> listExpectedFees=args.ListFees.Where(x => x.ClinicNum==args.ListClinics.Select(y => y.ClinicNum).Last()).OrderBy(x => x.FeeNum).ToList();
			List<Fee> listActualFees=cache.GetFeesForClinic(args.ListClinics.Select(y => y.ClinicNum).Last()).OrderBy(x => x.FeeNum).ToList();
			Assert.IsTrue(AreListsSimilar(listExpectedFees,listActualFees));
		}

		///<summary>Update a single fee in the local cache only.</summary>
		[TestMethod]
		public void FeeCache_UpdateFee() {
			Fee fee=CreateSingleFee(MethodBase.GetCurrentMethod().Name);
			fee.Amount=75;
			FeeCache cache=new FeeCache();
			cache.Update(fee);
			Assert.AreEqual(fee.Amount,cache.GetAmount0(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum));
			Fees.Delete(fee);
		}

		///<summary>Remove a single fee from the local cache only.</summary>
		[TestMethod]
		public void FeeCache_RemoveFee() {
			Fee fee=CreateSingleFee(MethodBase.GetCurrentMethod().Name);
			FeeCache cache=new FeeCache();
			Assert.IsTrue(cache.Remove(fee));
			Fees.Delete(fee);
		}

		///<summary>Creates several fees and randomly performs 100 inserts,updates,and deletes and checks they are saved correctly.
		///Implicitly tests Fees.UpdateFromCache.</summary>
		[TestMethod]
		public void FeeCache_Transaction() {
			FeeTestArgs args=CreateManyFees(2,6,2,MethodBase.GetCurrentMethod().Name);
			List<Tuple<Fee,long>> listChanges=new List<Tuple<Fee, long>>();
			List<Fee> listFeesToUse=args.ListFees.OrderBy(x => _rand.Next()).Take(100).ToList();
			FeeCache cache=new FeeCache();
			cache.BeginTransaction();
			foreach(Fee fee in listFeesToUse) {
				int flip=_rand.Next(2);
				Fee newFee=null;
				switch(flip) {
					case 0:
						newFee=new Fee()
						{
							FeeSched=args.EmptyFeeSchedNum,
							Amount=_defaultFeeAmt,
							CodeNum=_listProcCodes[_rand.Next(_listProcCodes.Count-1)].CodeNum,
							ClinicNum=args.ListClinics[_rand.Next(args.ListClinics.Count-1)].ClinicNum,
						};
						args.ListFees.Add(newFee);
						cache.Add(newFee);
						break;
					case 1:						
						fee.Amount=fee.Amount * _rand.NextDouble();
						cache.Update(fee);
						break;
					case 2:
						cache.Remove(fee);
						args.ListFees.Remove(fee);
						break;
				}
				listChanges.Add(Tuple.Create<Fee,long>(newFee??fee,flip));
			}
			cache.SaveToDb();
			foreach(Tuple<Fee,long> change in listChanges) {
				Fee fee=change.Item1;
				switch(change.Item2) {
					case 0:
						Assert.IsNotNull(cache.GetFee(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum,true));
						break;
					case 1:
						Assert.AreEqual(fee.Amount,cache.GetAmount0(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum),0.01);
						break;
					case 2:
						Assert.IsNull(cache.GetFee(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum,true));
						break;
				}
			}
		}

		///<summary>Cycle through getting clinic fees from the local cache. This should be used to confirm the queueClinicNums is working. 
		///It is not intended for load testing, or it may take a very long time.</summary>
		[TestMethod]
		public void FeeCache_CycleClinics() {
			FeeCache cache=new FeeCache();
			List<Fee> listFees=CreateManyFees(2,7,2,MethodBase.GetCurrentMethod().Name).ListFees;
			//Only check a random subset of elements in the created list in order to prevent this test from taking a long time
			List<Fee> listFeesToCheck=listFees.OrderBy(x => _rand.Next()).Take(500).ToList();
			foreach(Fee fee in listFeesToCheck) {
				Assert.IsNotNull(cache.GetFee(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum,true));
			}
		}

		/// <summary>Creates three fee schedules. Invalidate two fee schedules, confirm that getting the fees still returns the proper amount.</summary>
		[TestMethod]
		public void FeeCache_InvalidateFeeSchedules() {
			FeeTestArgs args=CreateManyFees(3,1,1,MethodBase.GetCurrentMethod().Name);
			FeeCache cache=new FeeCache(args.ListFees);
			Fee fee1=args.ListFees.First();
			Fee fee2=args.ListFees.First(x => x.FeeSched!=fee1.FeeSched);
			Fee fee3=args.ListFees.First(x => x.FeeSched!=fee2.FeeSched);
			cache.Invalidate(fee1.FeeSched);
			cache.Invalidate(fee2.FeeSched);
			Assert.AreEqual(fee1.Amount,cache.GetFee(fee1.CodeNum,fee1.FeeSched,fee1.ClinicNum,fee1.ProvNum).Amount);
			Assert.AreEqual(fee2.Amount,cache.GetFee(fee2.CodeNum,fee2.FeeSched,fee2.ClinicNum,fee2.ProvNum).Amount);
			Assert.AreEqual(fee3.Amount,cache.GetFee(fee3.CodeNum,fee3.FeeSched,fee3.ClinicNum,fee3.ProvNum).Amount);
		}

		///<summary>Searches for a fee where there are multiple potential matches.</summary>
		[TestMethod]
		public void FeeCache_GetFee_ManyPossibleMatches() {
			string name=MethodBase.GetCurrentMethod().Name;
			Fee expectedFee=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),true,true);
			CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),true,false,expectedFee.FeeSched,expectedFee.CodeNum,expectedFee.ClinicNum);
			CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),false,true,expectedFee.FeeSched,expectedFee.CodeNum,0,expectedFee.ProvNum);
			CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),false,false,expectedFee.FeeSched,expectedFee.CodeNum);
			Fee actualFee=new FeeCache().GetFee(expectedFee.CodeNum,expectedFee.FeeSched,expectedFee.ClinicNum,expectedFee.ProvNum);
			Assert.IsTrue(AreSimilar(expectedFee,actualFee));
		}

		///<summary>Searches for a fee for a provider and clinic but accepts the fee for the default clinic and no provider due to the fee schedule being
		///a global fee schedule.</summary>
		[TestMethod]
		public void FeeCache_GetFee_GlobalFeeSched() {
			string name=MethodBase.GetCurrentMethod().Name;
			Fee expectedFee=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),false,false,isGlobalFeeSched: true);
			Fee otherFee1=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),true,false,expectedFee.FeeSched,expectedFee.CodeNum);
			Fee otherFee2=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),false,true,expectedFee.FeeSched,expectedFee.CodeNum);
			CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),true,true,expectedFee.FeeSched,expectedFee.CodeNum);
			Fee actualFee=new FeeCache().GetFee(expectedFee.CodeNum,expectedFee.FeeSched,otherFee1.ClinicNum,otherFee2.ProvNum);
			Assert.IsTrue(AreSimilar(expectedFee,actualFee));
		}

		#region FeeSchedTools Tests

		///<summary>Creates a single fee for a given fee schedule, copies the feeschedule to an empty schedule and checks that the fee is the same
		///in both fee schedules.</summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_CopyFeeSched() {
			//Create two fee schedules; from and to
			long fromSched=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_FROM");
			long toSched=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_TO");
			//Create a single fee and associate it to the "from" fee schedule.
			long feeCodeNum=_listProcCodes[_rand.Next(_listProcCodes.Count-1)].CodeNum;
			FeeT.CreateFee(fromSched,feeCodeNum,_defaultFeeAmt*_rand.NextDouble());
			FeeCache cache=new FeeCache();
			FeeScheds.CopyFeeSchedule(cache,FeeScheds.GetFirst(x => x.FeeSchedNum==fromSched),0,0,FeeScheds.GetFirst(x => x.FeeSchedNum==toSched),null,0);
			cache.SaveToDb();
			//Get the two fees and check that they are the same.
			Fee fromFee=Fees.GetFee(feeCodeNum,fromSched,0,0);
			Fee toFee=Fees.GetFee(feeCodeNum,toSched,0,0);
			Assert.AreEqual(fromFee.Amount,toFee.Amount);
		}

		///<summary>Mimics two instances of Open Dental being open and copying a fee schedule into the same fee schedule from each instance.
		///No matter how many instances of Open Dental invoke the same fee schedule action they should never create duplicate fees.</summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_CopyFeeSched_Concurrency() {
			//Make sure there are no duplicate fees already present within the database.
			string dbmResult=DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Check);
			if(dbmResult.Trim()!=_feeDeleteDuplicatesExpectedResult) {
				DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Fix);
			}
			//Create two fee schedules; from and to
			long feeSchedNumFrom=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_FROM");
			long feeSchedNumTo=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_TO");
			//Create a single fee and associate it to the "from" fee schedule.
			FeeT.CreateFee(feeSchedNumFrom,_listProcCodes[_rand.Next(_listProcCodes.Count-1)].CodeNum,_defaultFeeAmt);
			//Create a helper action that will simply copy the "from" schedule into the "to" schedule for the given fee cache passed in.
			Action<FeeCache> actionCopyFromTo=new Action<FeeCache>((feeCache) => {
				FeeScheds.CopyFeeSchedule(feeCache,
					FeeScheds.GetFirst(x => x.FeeSchedNum==feeSchedNumFrom),
					0,
					0,
					FeeScheds.GetFirst(x => x.FeeSchedNum==feeSchedNumTo),
					null,
					0);
			});
			//Create two identical copies of the "fee cache" which mimics two separate instances of Open Dental running at the same time.
			FeeCache feeCache1=new FeeCache();
			FeeCache feeCache2=new FeeCache();
			//Mimic each user clicking the "Copy" button from within the Fee Tools window one right after the other (before they click OK).
			actionCopyFromTo(feeCache1);
			actionCopyFromTo(feeCache2);
			//Now act like the each instance of Open Dental just clicked the "OK" button which simply saves the cache changes to the db.
			feeCache1.SaveToDb();
			feeCache2.SaveToDb();
			//Make sure that there was NOT a duplicate fee inserted into the database.
			dbmResult=DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Check);
			Assert.AreEqual(dbmResult.Trim(),_feeDeleteDuplicatesExpectedResult,"Duplicate fees detected due to concurrent copying.");
		}

		///<summary>Users can copy one fee schedule to multiple clinics at once.  There was a problem with copying fee schedules to more than six clinics
		///at the same time due to running the copy fee schedule logic in parallel.  This unit test is designed to make sure that the bug fix of running
		///the copy fee schedule logic synchronously is preserved OR if parallel threads are reintroduced that they have fixed the bug.</summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_CopyFeeSched_Clinics() {
			//Make sure there are no duplicate fees already present within the database.
			string dbmResult=DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Check);
			if(dbmResult.Trim()!=_feeDeleteDuplicatesExpectedResult) {
				DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Fix);
			}
			//Make sure that there are more than six clinics
			ClinicT.ClearClinicTable();
			for(int i=0;i<10;i++) {
				ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"_"+i);
			}
			//Create two fee schedules; from and to
			long feeSchedNumFrom=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_FROM");
			long feeSchedNumTo=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_TO");
			//Create a fee for every single procedure code in the database and associate it to the "from" fee schedule.
			foreach(ProcedureCode code in _listProcCodes) {
				FeeT.CreateFee(feeSchedNumFrom,code.CodeNum,_rand.Next(5000),hasCacheRefresh:false);
			}
			FeeCache feeCache=new FeeCache();
			//Copy the "from" fee schedule into the "to" fee schedule and do it for at least seven clinics.
			FeeScheds.CopyFeeSchedule(feeCache,
					FeeScheds.GetFirst(x => x.FeeSchedNum==feeSchedNumFrom),
					0,
					0,
					FeeScheds.GetFirst(x => x.FeeSchedNum==feeSchedNumTo),
					Clinics.GetDeepCopy(true).Select(x => x.ClinicNum).ToList(),
					0);
			//Now act like the user just clicked the "OK" button which simply saves the cache changes to the db.
			feeCache.SaveToDb();
			//Make sure that there was NOT a duplicate fee inserted into the database.
			dbmResult=DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Check);
			Assert.AreEqual(dbmResult.Trim(),_feeDeleteDuplicatesExpectedResult,"Duplicate fees detected due to concurrent copying.");
		}

		///<summary>Test importing FeeSchedule by copying from one list to another.</summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_Import() {
			FeeTestArgs args=CreateManyFees(1,1,1,MethodBase.GetCurrentMethod().Name);
			FeeCache cache=new FeeCache();
			List<Fee> listImportedFees = cache.GetListFees(args.EmptyFeeSchedNum,0,0);
			foreach(Fee fee in cache.GetListFees(_standardFeeSchedNum,0,0)) {
				string codeText = _listProcCodes.Where(x => x.CodeNum==fee.CodeNum).Select(x => x.ProcCode).FirstOrDefault();
				listImportedFees=Fees.Import(codeText,fee.Amount,args.EmptyFeeSchedNum,fee.ClinicNum,fee.ProvNum,listImportedFees);
			}
			Fees.InsertMany(listImportedFees);
			Assert.IsTrue(DoAmountsMatch(listImportedFees,_standardFeeSchedNum,0,0));
		}

		///<summary>Creates and exports a fee schedule, then tries to import the fee schedule from the exported file then checks that the new fee 
		///schedule was imported correctly. If there are procedurecodes with an empty proccode we exclude these from the check, as we cannot look up
		///the correct code nums during the import (intended behavior). </summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_ImportExport() {
			FeeTestArgs feeArgs=CreateManyFees(1,1,1,MethodBase.GetCurrentMethod().Name);
			long exportedSched=feeArgs.ListFeeSchedNums[0];
			long importedSched=feeArgs.EmptyFeeSchedNum;
			long clinicNum=feeArgs.ListClinics[0].ClinicNum;
			string filename=MethodBase.GetCurrentMethod().Name;
			FeeScheds.ExportFeeSchedule(new FeeCache(),exportedSched,clinicNum,feeArgs.ListProvNums[0],filename);
			OpenDental.FeeL.ImportFees(importedSched,clinicNum,feeArgs.ListProvNums[0],filename,new System.Windows.Forms.Form(),false);
			FeeCache cache=new FeeCache();
			foreach(ProcedureCode procCode in _listProcCodes.Where(x => !string.IsNullOrWhiteSpace(x.ProcCode))) { //unable to import without a proccodes
				Fee expected=cache.GetFee(procCode.CodeNum,exportedSched,clinicNum,feeArgs.ListProvNums[0]);
				Fee actual=cache.GetFee(procCode.CodeNum,importedSched,clinicNum,feeArgs.ListProvNums[0]);
				Assert.AreEqual(expected.Amount,actual.Amount);
			}
		}

		///<summary>Import canada fees from a file.</summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_ImportCanada() {
			string canadianCodes=Properties.Resources.canadianprocedurecodes;
			//If we need to import these procedures codes, do so
			foreach(string line in canadianCodes.Split(new[] { "\r\n", "\r", "\n" },StringSplitOptions.None)) {
				string[] properties=line.Split('\t');
				if(properties.Count()!=10) {
					continue;
				}
				if(ProcedureCodes.GetCodeNum(properties[0])!=0) {
					continue;
				}
				ProcedureCode procCode=new ProcedureCode()
				{
					ProcCode=properties[0],
					Descript=properties[1],
					ProcTime=properties[8],
					AbbrDesc=properties[9],
				};
				ProcedureCodes.Insert(procCode);
			}
			//Now import the fees
			string feeData=Properties.Resources.BC_BCDA_2018_GPOOC;
			FeeSched feeSched=FeeSchedT.GetNewFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name,isGlobal:false);
			FeeCache feeCache=new FeeCache();
			int numImported;
			int numSkipped;
			List<Fee> listNewFees=FeeScheds.ImportCanadaFeeSchedule(feeCache,feeSched,feeData,0,0,out numImported,out numSkipped);
			feeCache.BeginTransaction();
			feeCache.RemoveFees(feeSched.FeeSchedNum,0,0);
			foreach(Fee fee in listNewFees) {
				feeCache.Add(fee);
			}
			feeCache.SaveToDb();
			Assert.IsTrue(DoAmountsMatch(listNewFees,feeSched.FeeSchedNum,0,0));
		}

		///<summary>Create and fill a fee schedule, then clear the fee schedule and make sure it is empty.</summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_ClearFeeSchedule() {
			FeeTestArgs feeArgs=CreateManyFees(1,1,1,MethodBase.GetCurrentMethod().Name);
			long feeSchedNum=feeArgs.ListFeeSchedNums[0];
			Assert.IsTrue(Fees.GetCountByFeeSchedNum(feeSchedNum) > 0);
			FeeCache cache=new FeeCache();
			cache.BeginTransaction();
			cache.RemoveFees(feeSchedNum,feeArgs.ListClinics[0].ClinicNum,feeArgs.ListProvNums[0]);
			cache.SaveToDb();
			cache.FillCacheIfNeeded();
			Assert.AreEqual(0,Fees.GetCountByFeeSchedNum(feeSchedNum));
		}

		///<summary>Create the standard fee schedule and increase by 5% to the nearest penny.</summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_Increase() {
			FeeTestArgs args = CreateManyFees(0,0,0,MethodBase.GetCurrentMethod().Name);
			int percent = 5;
			FeeCache cache = new FeeCache();
			List<Fee> listStandardFees = cache.GetListFees(_standardFeeSchedNum,0,0).OrderBy(x => x.FeeNum).ToList();
			List<Fee> listIncreasedFees = listStandardFees.Select(x => x.Copy()).ToList();
			listIncreasedFees=Fees.Increase(_standardFeeSchedNum,percent,2,listIncreasedFees,0,0).OrderBy(x => x.FeeNum).ToList();
			foreach(Fee fee in listIncreasedFees) {
				Fee expectedFee = cache.GetFee(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum);
				double amount = fee.Amount/(1+(double)percent/100);
				amount=Math.Round(amount,2);
				try {
					Assert.AreEqual(expectedFee.Amount,amount);
				}
				catch(Exception e) {
					throw new Exception("Failed for fee: " + expectedFee.FeeNum,e);
				}
			}
		}

		///<summary>There was a bug where an exception would throw when trying to increase a fee schedule for a specific fee whose best match was a fee
		///from a different clinic and provider combination.</summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_Increase_NonPerfectMatch() {
			//Make sure that there are more than six clinics
			ClinicT.ClearClinicTable();
			int clinicCount=2;
			for(int i=0;i<clinicCount;i++) {
				ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"_"+i);
			}
			//Create two providers.
			int providerCount=2;
			List<long> listProvNums=new List<long>();
			for(int i=0;i<providerCount;i++) {
				listProvNums.Add(ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"_"+i));
			}
			//Create a fee schedule that will be used by all clinics
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name,isGlobal:false);
			//Create a fee for every single procedure code in the database and associate it to the "from" fee schedule.
			foreach(ProcedureCode code in _listProcCodes) {
				FeeT.CreateFee(feeSchedNum,code.CodeNum,_rand.Next(1,5000),hasCacheRefresh:false);
			}
			int feeCountBefore=Fees.GetCountByFeeSchedNum(feeSchedNum);
			FeeCache feeCache=new FeeCache();
			feeCache.BeginTransaction();
			//Begin a transaction to update all of the fees for the two providers in all of the clinics.
			List<Action> listActions=new List<Action>();
			foreach(Clinic clinic in Clinics.GetDeepCopy(true)) {
				listActions.Add(() => {
					List<Fee> listFees=feeCache.GetListFees(feeSchedNum,0,0);
					foreach(long provNum in listProvNums) {
						Fees.Increase(feeSchedNum,10,2,listFees,clinic.ClinicNum,provNum);
					}
					feeCache.RemoveFees(feeSchedNum,clinic.ClinicNum,0);
					foreach(long provNum in listProvNums) {
						feeCache.RemoveFees(feeSchedNum,clinic.ClinicNum,provNum);
					}
					foreach(Fee fee in listFees) {
						feeCache.Add(fee);
					}
				});
			}
			//FeeCache can't handle parallel threads ATM but when it can the following line can be manipulated in order to increase unit test speed.
			ODThread.RunParallel(listActions,TimeSpan.FromMinutes(30),numThreads:1);
			feeCache.SaveToDb();
			int feeCountAfter=Fees.GetCountByFeeSchedNum(feeSchedNum);
			//Assert that both the Dent and the Hyg providers had new fee schedules created for every single clinic.
			int expectedCount=feeCountBefore + (feeCountBefore * clinicCount * providerCount);
			Assert.AreEqual(expectedCount,feeCountAfter);
		}

		///<summary>Attach some fees to procedures, change half the fees and call Global Update Fees.</summary>
		[TestMethod]
		public void FeeCache_FeeSchedTools_GlobalUpdateFees() {
			Prefs.UpdateBool(PrefName.MedicalFeeUsedForNewProcs,false);
			Prefs.RefreshCache();
			string suffix=MethodBase.GetCurrentMethod().Name;
			string procStr="D0120";
			string procStr2="D0145";
			double procFee=100;
			ProcedureCode procCode=ProcedureCodes.GetProcCode(procStr);
			ProcedureCode procCode2=ProcedureCodes.GetProcCode(procStr2);
			//Set up clinic, prov, pat
			Clinic clinic=ClinicT.CreateClinic(suffix);
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix,false);
			long provNum=ProviderT.CreateProvider(suffix,feeSchedNum:feeSchedNum);
			Fee fee=FeeT.GetNewFee(feeSchedNum,procCode.CodeNum,procFee,clinic.ClinicNum,provNum);
			Fee fee2=FeeT.GetNewFee(feeSchedNum,procCode2.CodeNum,procFee,clinic.ClinicNum,provNum);
			Patient pat=PatientT.CreatePatient(suffix,provNum,clinic.ClinicNum);
			//Chart a procedure for this proccode/pat as well as a different proccode
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",fee.Amount);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procStr2,ProcStat.TP,"",fee2.Amount);
			//Update the fee amount for only the D0120 code
			fee.Amount=50;
			Fees.Update(fee);
			//Now run global update fees
			long numUpdated=Procedures.GlobalUpdateFees(new FeeCache().GetFeesForClinics(new List<long>() { clinic.ClinicNum }),clinic.ClinicNum,clinic.Abbr);
			//Make sure we have the same number of updated fees, and fee amounts for both procs
			//Assert.AreEqual(1,numUpdated);
			proc=Procedures.GetOneProc(proc.ProcNum,false);
			proc2=Procedures.GetOneProc(proc2.ProcNum,false);
			Assert.AreEqual(fee.Amount,proc.ProcFee);
			Assert.AreEqual(fee2.Amount,proc2.ProcFee);
		}

		#endregion

	}
}
