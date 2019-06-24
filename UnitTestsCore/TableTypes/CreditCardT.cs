using System;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class CreditCardT {
		public static CreditCard CreateCard(long patNum,double chargeAmt,DateTime dateStart,long payPlanNum,string authorizedProcs="",
			ChargeFrequencyType frequencyType=ChargeFrequencyType.FixedDayOfMonth,DayOfWeekFrequency dayOfWeekFrequency=DayOfWeekFrequency.Every,
			DayOfWeek dayOfWeek=DayOfWeek.Friday,string daysOfMonth="",bool canChargeWhenZeroBal=false)
		{
			CreditCard card=new CreditCard();
			card.PatNum=patNum;
			card.ChargeAmt=chargeAmt;
			card.DateStart=dateStart;
			card.PayPlanNum=payPlanNum;
			card.CCExpiration=DateTime.Today.AddYears(3);
			card.CCNumberMasked="XXXXXXXXXXXXX1234";
			card.Procedures=authorizedProcs;
			if(frequencyType==ChargeFrequencyType.FixedDayOfMonth) {
				card.ChargeFrequency=POut.Int((int)ChargeFrequencyType.FixedDayOfMonth)
					+"|"+(daysOfMonth=="" ? dateStart.Day.ToString() : daysOfMonth);
			}
			else if(frequencyType==ChargeFrequencyType.FixedWeekDay) {
				card.ChargeFrequency=POut.Int((int)ChargeFrequencyType.FixedWeekDay)
					+"|"+POut.Int((int)dayOfWeekFrequency)+"|"+POut.Int((int)dayOfWeek);
			}
			card.CanChargeWhenNoBal=canChargeWhenZeroBal;
			CreditCards.Insert(card);
			return card;
		}

	}
}
