using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class RpHiddenPaySplits {
		public static DataTable GetReportData(List<long> listProvNums,List<long> listUnearnedTypeDefNums,List<long> listClinicNums) {
			//todo lindsay: don't include clinic column if not enabled
			string command=$@"SELECT paysplit.DatePay,
												CONCAT(patient.LName,', ',patient.FName),provider.Abbr,clinic.Abbr,procedurecode.ProcCode,
												procedurecode.Descript,paysplit.SplitAmt
												FROM paysplit
												INNER JOIN definition ON definition.DefNum=paysplit.UnearnedType AND definition.ItemValue!=''
												INNER JOIN patient ON patient.PatNum=paysplit.PatNum
												LEFT JOIN procedurelog ON procedurelog.ProcNum=paysplit.ProcNum
												LEFT JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum
												LEFT JOIN provider ON provider.ProvNum=paysplit.ProvNum
												LEFT JOIN clinic ON clinic.ClinicNum=paysplit.ClinicNum
												WHERE paysplit.ProvNum IN ({string.Join(",",listProvNums)})
												AND paysplit.UnearnedType IN ({string.Join(",",listUnearnedTypeDefNums)}) ";
												if(listClinicNums.Count>0) {
													command+=$"AND paysplit.ClinicNum IN ({string.Join(",",listClinicNums)}) ";
												}
												command+="ORDER BY paysplit.DatePay,patient.LName,patient.FName";
			return Db.GetTable(command);
		}
	}
}
