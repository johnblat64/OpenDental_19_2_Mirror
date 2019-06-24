using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PayConnectResponseWebs{
		#region Get Methods
		
		///<summary>Gets one PayConnectResponseWeb from the db.</summary>
		public static PayConnectResponseWeb GetOne(long payConnectResponseWebNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<PayConnectResponseWeb>(MethodBase.GetCurrentMethod(),payConnectResponseWebNum);
			}
			return Crud.PayConnectResponseWebCrud.SelectOne(payConnectResponseWebNum);
		}
		#endregion Get Methods
		#region Modification Methods
		#region Insert
		///<summary></summary>
		public static long Insert(PayConnectResponseWeb payConnectResponseWeb){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				payConnectResponseWeb.PayConnectResponseWebNum=Meth.GetLong(MethodBase.GetCurrentMethod(),payConnectResponseWeb);
				return payConnectResponseWeb.PayConnectResponseWebNum;
			}
			return Crud.PayConnectResponseWebCrud.Insert(payConnectResponseWeb);
		}
		#endregion Insert
		#region Update
		///<summary></summary>
		public static void Update(PayConnectResponseWeb payConnectResponseWeb){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payConnectResponseWeb);
				return;
			}
			Crud.PayConnectResponseWebCrud.Update(payConnectResponseWeb);
		}
		#endregion Update
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		



	}
}