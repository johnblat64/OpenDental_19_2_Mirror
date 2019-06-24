using System;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ProcedureCodeT {

		///<summary>Returns a procedure code object that utilizes the procCode passed in.
		///Either returns the pre-existing code from the cache or creates a new one.</summary>
		public static ProcedureCode CreateProcCode(string procCode,bool isCanadianLab=false) {
			AddIfNotPresent(procCode,isCanadianLab);
			return ProcedureCodes.GetOne(procCode);
		}
		
		public static void Update(ProcedureCode procCode) {
			ProcedureCodes.Update(procCode);
			ProcedureCodes.RefreshCache();
		}

		/// <summary>Returns true if a procedureCode was added.</summary>
		public static bool AddIfNotPresent(string procCode,bool isCanadianLab=false) {
			if(!ProcedureCodes.GetContainsKey(procCode)) {
				ProcedureCodes.Insert(new ProcedureCode {
					ProcCode=procCode,
					IsCanadianLab=isCanadianLab,
				});
				ProcedureCodes.RefreshCache();
				return true;
			}
			return false;
		}

	}
}
