using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness
{
	public class JobHelper {

		public static List<long> GetEngineerUserNums() {
			return new List<long>() {7,24,29,52,59,61,72,77,98,31,138,149,157,223,227,273,349,355,383,393};
		}

		public static List<Userod> GetEngineerUsers() {
			return Userods.GetUsers(GetEngineerUserNums());
		}
		
	}
}
