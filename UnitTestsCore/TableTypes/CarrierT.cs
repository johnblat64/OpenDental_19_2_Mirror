﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class CarrierT {
		public static Carrier CreateCarrier(string suffix,string address="",string city="",string state="",string zip="",string electID="",params TrustedEtransTypes[] arrayTrustedEtrans){
			Carrier carrier=new Carrier();
			carrier.CarrierName="Carrier"+suffix;
			carrier.Address=address;
			carrier.City=city;
			carrier.State=state;
			carrier.Zip=zip;
			carrier.ElectID=electID;
			if(arrayTrustedEtrans!=null && arrayTrustedEtrans.Length>0) {
				carrier.TrustedEtransFlags=(TrustedEtransTypes)arrayTrustedEtrans.ToList().Sum(x => (int)x);
			}
			Carriers.Insert(carrier);
			return carrier;
		}




	}
}
