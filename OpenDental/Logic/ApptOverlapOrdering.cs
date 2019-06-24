using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OpenDental {
	///<summary>Stores appointments that are overlapping and the order they should be displayed in.</summary>
	public class ApptOverlapOrdering {
		///<summary>Contains a list of sets of apointments. These lists of appointments are stored in priority order with the first appointment 
		///being the highest priority. One List&lt;Appointment&gt; represents a single order of overlapping appointments. As there can be many orders
		///to keep track of, there is a List of these. Every AptNum can only be in ONE order.</summary>
		private List<List<AppointmentLite>> _listAppointments=new List<List<AppointmentLite>>();
		///<summary>Used to cycle the appointment in the cycle appointments logic. Should be the time on ContrAppt that is clicked on.</summary>
		private TimeSpan _timeLastClickedOn;

		///<summary>Checks to see if an appoinment is an overlapping appointment.</summary>
		public bool IsOverlappingAppt(long aptNum) {
			if(_listAppointments.Any(x => x.Any(y => y.AptNum==aptNum))) {//is there any appointment in any order set
				return true;
			}
			return false;//none
		}

		///<summary>Adds a list of appoinments as a set and puts them in order based on the DateTStamp. The listAptNums should be
		///the AptNums found from Appointments.GetOverlappingAppts.</summary>
		private void AddOverlappingAppts(List<long> listAptNums) {
			//only database call is when adding a new set of overlapping appointment
			List<AppointmentLite> listAppointments=Appointments.GetMultApts(listAptNums).OrderByDescending(x => x.DateTStamp)
				.Select(x => new AppointmentLite(x)).ToList();
			_listAppointments.Add(listAppointments);
		}

		///<summary>Gets the appoinment that was selected in case it is part of a special order.
		///The selected aptNum is returned if it was previously selected. Otherwise, it returns the appointment on top.</summary>
		public long GetSelectedApptNum(long aptNum,TimeSpan timeClicked) {
			_timeLastClickedOn=timeClicked;
			List<AppointmentLite> listOrders=GetOrderByApptNum(aptNum);//Gets all appointments in order
			//Gets all appointments that fall in the timeframe
			listOrders=listOrders.FindAll(x => _timeLastClickedOn.Between(x.AptDateTime.TimeOfDay,
				x.AptEndTime.TimeOfDay,isUpperBoundInclusive: false));
			//if the last SelectedAptNum is in this group, the user clicked on that appointment again. Leave as SelectedAptNum.
			if(listOrders.Any(x => x.AptNum==ContrApptSingle.SelectedAptNum)) {
				return ContrApptSingle.SelectedAptNum;
			}
			else {//The highest priority one should be selected.
				if(listOrders.Count==0) {
					return aptNum;
				}
				return listOrders.First().AptNum;
			}
		}

		///<summary>Cycles the overlapping appointments. The aptNum passed in is the appointment that was clicked on.</summary>
		public void CycleOverlappingAppts(long aptNum) {
			//Gets all appointments in order
			List<AppointmentLite> listApptOrders=GetOrderByApptNum(aptNum);
			//Gets all appointments in order set that lie within the clicked on time
			List<AppointmentLite> listAppointmentsOnTimeClicked=listApptOrders.FindAll(x => _timeLastClickedOn.Between(x.AptDateTime.TimeOfDay,
					  x.AptEndTime.TimeOfDay,isUpperBoundInclusive: false));
			//Lowest Priority appt where clicked gets added as highest priority
			AppointmentLite lowestPriorityAppt=listAppointmentsOnTimeClicked.Last();
			listApptOrders.Remove(lowestPriorityAppt);
			listApptOrders.Insert(0,lowestPriorityAppt);
		}

		///<summary>Returns a list of aptNums that are in a set. They are sorted from first drawn to last draw (descending priority).</summary>
		public List<long> GetApptNumsInSet(long aptNum) {
			List<AppointmentLite> listApptsReversed=new List<AppointmentLite>(GetOrderByApptNum(aptNum));
			listApptsReversed.Reverse();
			return listApptsReversed.Select(x => x.AptNum).ToList();//Descending order
		}

		///<summary>Finds appointments that are overlapping. Adds them to the list if they are not already in a list or if the appointment times
		///have been changed.</summary>
		public void UpdateApptOrder(DataTable dtAppointments) {
			//Get all overlapping appointments
			List<List<long>> listApptsOrders=Appointments.GetOverlappingAppts(dtAppointments);
			foreach(List<long> listAptGroup in listApptsOrders) {
				if(DoesGroupAlreadyExist(listAptGroup)) {//if they all exist in an order, check for changes
					List<AppointmentLite> listApptsStored=GetOrderByApptNum(listAptGroup[0]).OrderBy(x => x.AptNum).ToList();
					List<Appointment> listApptsCur=Appointments.GetMultApts(listAptGroup).OrderBy(x => x.AptNum).ToList();
					for(int i=0;i<listApptsStored.Count;i++) {
						if(listApptsStored[i].AptDateTime!=listApptsCur[i].AptDateTime
							|| listApptsStored[i].AptEndTime!=listApptsCur[i].EndTime
							|| listApptsStored[i].Op!=listApptsStored[i].Op) 
						{
							_listAppointments.Remove(GetOrderByApptNum(listAptGroup[0]));
							AddOverlappingAppts(listAptGroup);
							break;
						}
					}
				}
				else if(listAptGroup.Any(x => IsOverlappingAppt(x))) {//at least one of the aptNums is in another group
					foreach(long aptNum in listAptGroup) {//remove all old orders
						if(IsOverlappingAppt(aptNum)) {
							_listAppointments.Remove(GetOrderByApptNum(aptNum));
						}
					}
					AddOverlappingAppts(listAptGroup);
				}
				else {//None exist in an order, add them
					AddOverlappingAppts(listAptGroup);
				}
			}
			//Ran only if there are appointment orders that could be invalid. More efficient to skip otherwise
			if(!IsOverlapOrderingEmpty()) {
				RemoveInvalidOrders(listApptsOrders,dtAppointments);
			}
		}

		///<summary>Returns true if there are no appointment overlap orders stored.</summary>
		public bool IsOverlapOrderingEmpty() {
			return _listAppointments.Count==0;
		}

		///<summary>Removes any stale orders. This is when someone moves an overlapping appointment and it is no longer overlapping. 
		///listApptsOrder may have a count of 0, so we must remove the old orders that are no longer valid. 
		///Will only remove those in dtAppointments, but no longer overlapping.</summary>
		private void RemoveInvalidOrders(List<List<long>> listApptsOrders,DataTable dtAppointments) {
			foreach(DataRow row in dtAppointments.Rows) {
				long aptNum=PIn.Long(row["AptNum"].ToString());
				if(IsOverlappingAppt(aptNum) && !listApptsOrders.Any(x => x.Contains(aptNum))) {
					_listAppointments.Remove(GetOrderByApptNum(aptNum));
				}
			}
		}

		///<summary>Gets the times that an appointment is overlapping with others.</summary>
		public List<DateRange> GetOverlapTimes(long aptNum) {
			List<AppointmentLite> listOverlapAppointments=GetOrderByApptNum(aptNum);
			if(listOverlapAppointments==null || listOverlapAppointments.Count==0) {
				return new List<DateRange>();
			}
			List<DateRange> listOverlapTimes=new List<DateRange>();
			AppointmentLite apt=listOverlapAppointments.Find(x => x.AptNum==aptNum);
			foreach(AppointmentLite aptCompare in listOverlapAppointments) {
				if(apt.AptNum!=aptCompare.AptNum 
					&& MiscUtils.DoSlotsOverlap(apt.AptDateTime,apt.AptEndTime,aptCompare.AptDateTime,aptCompare.AptEndTime)) 
				{
					DateTime beginOverlap=ODMathLib.Max(apt.AptDateTime,aptCompare.AptDateTime);
					DateTime endOverlap=ODMathLib.Min(apt.AptEndTime,aptCompare.AptEndTime);
					listOverlapTimes.Add(new DateRange(beginOverlap,endOverlap));
				}
			}
			return listOverlapTimes;
		}

		///<summary>Checks to see if a list of AptNums already exist as a group.</summary>
		private bool DoesGroupAlreadyExist(List<long> listAptNums) {
			foreach(List<AppointmentLite> listAppts in _listAppointments) {
				listAptNums.Sort();
				List<long> listSortedAppts=listAppts.Select(x => x.AptNum).OrderBy(x => x).ToList();
				if(listAptNums.SequenceEqual(listSortedAppts)) {
					return true;
				}
			}
			return false;
		}

		///<summary>Gets an order set by a single appointment.</summary>
		private List<AppointmentLite> GetOrderByApptNum(long aptNum) {
			return _listAppointments.Find(x => x.Any(y => y.AptNum==aptNum));
		}

		///<summary>Stores some fields from the appointment class. Used to prevent storing too much data in ContrApptSheet.</summary>
		private class AppointmentLite {
			public long AptNum;
			public long Op;
			public DateTime AptDateTime;
			public DateTime AptEndTime;

			public AppointmentLite(Appointment apt) {
				AptNum=apt.AptNum;
				Op=apt.Op;
				AptDateTime=apt.AptDateTime;
				AptEndTime=apt.EndTime;
			}
		}
	}
}
