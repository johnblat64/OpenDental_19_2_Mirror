﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class PaymentT {
		public static Payment MakePayment(long patNum,double payAmt,DateTime payDate,long payPlanNum=0,long provNum=0,long procNum=0,long payType=0,
			long clinicNum=0,long unearnedType=0,bool isRecurringCharge=false,DateTime recurringChargeDate=default(DateTime),string externalId="") 
		{
			Payment payment=new Payment();
			payment.PatNum=patNum;
			payment.PayDate=payDate;
			payment.PayAmt=payAmt;
			payment.PayType=payType;
			payment.ClinicNum=clinicNum;
			payment.DateEntry=payDate;
			payment.IsRecurringCC=isRecurringCharge;
			payment.RecurringChargeDate=recurringChargeDate;
			payment.ExternalId=externalId;
			Payments.Insert(payment);
			PaySplit split=new PaySplit();
			split.PayNum=payment.PayNum;
			split.PatNum=payment.PatNum;
			split.DatePay=payDate;
			split.ClinicNum=payment.ClinicNum;
			split.PayPlanNum=payPlanNum;
			split.ProvNum=provNum;
			split.ProcNum=procNum;
			split.SplitAmt=payAmt;
			split.DateEntry=payDate;
			split.UnearnedType=unearnedType;
			PaySplits.Insert(split);
			return payment;
		}

		///<summary>Use this to test auto-split or income transfer logic.  This makes no splits, just the payment shell.</summary>
		public static Payment MakePaymentNoSplits(long patNum,double payAmt,DateTime payDate=default(DateTime),bool isNew=false,long payType=0
			,long clinicNum=0) 
		{//payType defaulted to non-income transfer
			if(payDate==default(DateTime)) {
				payDate=DateTime.Today;
			}
			Payment payment=new Payment();
			payment.PatNum=patNum;
			payment.PayDate=payDate;
			payment.PayAmt=payAmt;
			payment.IsNew=isNew;
			payment.ClinicNum=clinicNum;
			payment.PayType=payType;
			Payments.Insert(payment);
			return payment;
		}

		public static Payment MakePaymentForPrepayment(Patient pat,Clinic clinic) {
			Payment paymentCur=new Payment();
			paymentCur.PayDate=DateTime.Today;
			paymentCur.PatNum=pat.PatNum;
			paymentCur.ClinicNum=clinic.ClinicNum;
			paymentCur.DateEntry=DateTime.Today;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			if(listDefs.Count>0) {
				paymentCur.PayType=listDefs[0].DefNum;
			}
			paymentCur.PaymentSource=CreditCardSource.None;
			paymentCur.ProcessStatus=ProcessStat.OfficeProcessed;
			paymentCur.PayAmt=0;
			Payments.Insert(paymentCur);
			return paymentCur;
		}
	}
}
