using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class CarrierT {
		public static Carrier CreateCarrier(string suffix,params TrustedEtransTypes[] arrayTrustedEtrans){
			Carrier carrier=new Carrier();
			carrier.CarrierName="Carrier"+suffix;
			if(arrayTrustedEtrans!=null && arrayTrustedEtrans.Length>0) {
				carrier.TrustedEtransFlags=(TrustedEtransTypes)arrayTrustedEtrans.ToList().Sum(x => (int)x);
			}
			Carriers.Insert(carrier);
			return carrier;
		}




	}
}
