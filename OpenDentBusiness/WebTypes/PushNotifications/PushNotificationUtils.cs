using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness.WebTypes {
	public static class PushNotificationUtils {

		#region Check-In Module
		///<summary>Check-in the given patient on the given device. 
		///The device will simulate the patient entering their FName, LName, BDate then clicking Submit and then Confirming their identity.</summary>
		public static void CI_CheckinPatient(long patNum,long mobileAppDeviceNum) {
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				return;
			}
			//See CI_CheckinPatient for input details.
			SendPushBackground(PushType.CI_CheckinPatient,mobileAppDeviceNum,listTags: new List<string>() { pat.FName,pat.LName,pat.Birthdate.Ticks.ToString() });
		}

		///<summary>Add a sheet the the device check-in checklist.</summary>
		public static void CI_AddSheet(long patNum,long sheetNum) {
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				return;
			}
			List<MobileAppDevice> listMads=MobileAppDevices.GetAll(patNum);
			if(listMads.IsNullOrEmpty()) {
				return;
			}
			Sheet sheet=Sheets.GetOne(sheetNum);
			if(sheet==null) {
				return;
			}
			//See CI_AddSheet for input details.
			foreach(MobileAppDevice mad in listMads) {
				SendPushBackground(PushType.CI_AddSheet,mad.MobileAppDeviceNum,listPrimaryKeys: new List<long>() { pat.PatNum,sheet.SheetNum });
			}
		}

		///<summary>Remove a sheet from a device check-in checklist.</summary>
		public static void CI_RemoveSheet(long patNum,long sheetNum) {
			Patient pat=Patients.GetPat(patNum);
			//Inentionally not getting the actual Sheet from the db here. It may have already been deleted, now time to get it off the device.
			if(pat==null) {
				return;
			}
			List<MobileAppDevice> listMads=MobileAppDevices.GetAll(patNum);
			if(listMads.IsNullOrEmpty()) {
				return;
			}
			//See CI_RemoveSheet for input details.
			foreach(MobileAppDevice mad in listMads) {
				SendPushBackground(PushType.CI_RemoveSheet,mad.MobileAppDeviceNum,listPrimaryKeys: new List<long>() { pat.PatNum,sheetNum });
			}
		}

		///<summary>Take this device back to check-in. Any action being taken in an existing check-in session will be lost.</summary>
		public static void CI_GoToCheckin(long mobileAppDeviceNum) {
			//See CI_GoToCheckin for input details.
			SendPushBackground(PushType.CI_GoToCheckin,mobileAppDeviceNum);
		}

		///<summary>Occurs when the MobileAppDevice.IsAllowed changed for this device or this MobileAppDevice row is deleted (send isAllowed = false).</summary>
		public static void CI_IsAllowedChanged(long mobileAppDeviceNum,bool isAllowed) {
			//See CI_IsAllowedChanged for input details.
			SendPushBackground(PushType.CI_IsAllowedChanged,mobileAppDeviceNum,listTags: new List<string>() { POut.Bool(isAllowed) },runAsync:false);
		}

		///<summary>This push occurs when the eClipboard preferences for the given clinic change.</summary>
		public static void CI_NewEClipboardPrefs(long clinicNum) {
			//See CI_NewEClipboardPrefs for input details.
			SendPushBackground(PushType.CI_NewEClipboardPrefs,clinicNum:clinicNum,listTags:new List<string>() {
				POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardAllowSelfCheckIn,clinicNum)),
				ClinicPrefs.GetPrefValue(PrefName.EClipboardMessageComplete,clinicNum),
				POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicNum)),
				POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicNum)),
			});
		}

		#endregion

		#region MobileWeb Module

		#endregion

		#region Send Helpers
		///<summary>Sends a push notification as a background message to tell open applications to perform a specific task (as defined by the payload). 
		///The user does NOT need to interact with the app to process this action.
		///Background push is silent and generally the user does not care if it fails. 
		///Swallows errors and logs to PushNotifications Logger directory.  </summary>
		///<param name="pushType">The action for apps to take in the background.</param>
		///<param name="mobileAppDeviceNum">The MobileAppDeviceNum for the devices that the push notification will be sent to. 
		///Only use this when notification is intended for one and only one device. ClinicNum param will be ignored if MobileAppDeviceNum is specified.
		///In this case the ClinicNum belonging to this MobileAppDevice row will be used instead..</param>
		///<param name="clinicNum">The ClinicNum for the devices that the push notification will be sent to. 
		///If the office does not have clinics enabled, use clinicNum of 0.</param>
		///<param name="listPrimaryKeys">Varies depending on specific PushType(s).</param>
		///<param name="listTags">Varies depending on specific PushType(s).</param>
		///<param name="runAsync">Spawn thread and do not wait for result if true.</param>
		private static void SendPushBackground(PushType pushType,long mobileAppDeviceNum=0,long clinicNum=0,List<long> listPrimaryKeys=null,List<string> listTags=null,bool runAsync = true) {
			SendPush(pushType,false,mobileAppDeviceNum,clinicNum,listPrimaryKeys,listTags,runAsync:runAsync);
		}

		///<summary>Sends a push notification as an alert. Makes an alert on the device if possible. 
		///If the user clicks on the alert or the app is already open, will do the given action.
		///Swallows errors and logs to PushNotifications Logger directory.  </summary>
		///<param name="pushType">The action for apps to take in the background.</param>
		///<param name="alertTitle">The title of the alert that will appear in the notification center.</param>
		///<param name="alertMessage">The contents of the alert.</param>
		///<param name="mobileAppDeviceNum">The MobileAppDeviceNum for the devices that the push notification will be sent to. 
		///Only use this when notification is intended for one and only one device. ClinicNum param will be ignored if MobileAppDeviceNum is specified.
		///In this case the ClinicNum belonging to this MobileAppDevice row will be used instead..</param>
		///<param name="clinicNum">The ClinicNum for the devices that the push notification will be sent to. 
		///If the office does not have clinics enabled, use clinicNum of 0.</param>
		///<param name="listPrimaryKeys">Varies depending on specific PushType(s).</param>
		///<param name="listTags">Varies depending on specific PushType(s).</param>
		///<param name="runAsync">Spawn thread and do not wait for result if true.</param>
		private static void SendPushAlert(PushType pushType,string alertTitle,string alertMessage,long mobileAppDeviceNum=0,long clinicNum=0,
			List<long> listPrimaryKeys=null,List<string> listTags=null,bool runAsync = true) 
		{
			if(string.IsNullOrEmpty(alertTitle)||string.IsNullOrEmpty(alertMessage)) {
				throw new Exception("Alert title and message are required.");
			}
			SendPush(pushType,true,mobileAppDeviceNum,clinicNum,listPrimaryKeys,listTags,alertTitle,alertMessage,runAsync);
		}

		///<summary>Only call this method from SendPushBackground() or SendPushAlert(). See summary of those methods for documentation.
		///If runAsync==true then spawns a worker thread and makes the web call. This method is passive and returns void so no reason to wait for return.</summary>
		private static void SendPush(PushType pushType,bool isAlert,long mobileAppDeviceNum=0,long clinicNum=0,
			List<long> listPrimaryKeys=null,List<string> listTags=null,string alertTitle=null,string alertMessage=null,bool runAsync=true) 
		{
			void execute() {
				eServiceCode eService=eServiceCode.EClipboard;
				switch(pushType) {
					case PushType.CI_CheckinPatient:
					case PushType.CI_AddSheet:
					case PushType.CI_RemoveSheet:
					case PushType.CI_GoToCheckin:
					case PushType.CI_NewEClipboardPrefs:
					case PushType.CI_IsAllowedChanged:
						eService=eServiceCode.EClipboard;
						break;
					case PushType.None:
					default:
						throw new Exception("Unsupported PushType: "+pushType.ToString());
				}
				PushNotificationPayload payload=new PushNotificationPayload {
					IsAlert=isAlert,
					AlertMessage=alertMessage??"",
					AlertTitle=alertTitle??"",
					PushNotificationActionJSON=JsonConvert.SerializeObject( new PushNotificationAction() {
						TypeOfPush=pushType,
						ListPrimaryKeys=listPrimaryKeys??new List<long>(),
						ListTags=listTags??new List<string>()
					},
					typeof(PushNotificationAction),
					new JsonSerializerSettings())
				};
				if(mobileAppDeviceNum>0) { //Send to one exact device.
					MobileAppDevice mad=MobileAppDevices.GetOne(mobileAppDeviceNum);
					if(mad==null) {
						throw new Exception("MobileAppDeviceNum not found: "+mobileAppDeviceNum.ToString());
					}
					payload.DeviceId=mad.UniqueID;
					payload.ClinicNum=mad.ClinicNum;
				}
				else if(clinicNum>=0) { //Send to all devices for this clinic.
					payload.ClinicNum=clinicNum;
				}
				else {
					//Must either specify mobileAppDeviceNum or clinicNum. 
					//Failing to specify either would send to the entire RegKey. You're probably doing it wrong!
					throw new Exception("Programming error. Must specify either MobileAppDeviceNum or ClinicNum!");
				}
				//Validate that this clinic is signed up for eClipboard.
				if(!MobileAppDevices.IsClinicSignedUpForEClipboard(payload.ClinicNum)) {
					throw new Exception($"ClinicNum {payload.ClinicNum} is not signed up for eClipboard.");
				}
				string jsonPayload=JsonConvert.SerializeObject(payload,typeof(PushNotificationPayload),new JsonSerializerSettings());
				string result=WebServiceMainHQProxy.GetWebServiceMainHQInstance().SendPushNotification(PayloadHelper.CreatePayload("",eService),jsonPayload);
				if(result.ToLower()!="success") {
					throw new Exception(result);
				}
			}
			ODThread th=new ODThread(new ODThread.WorkerDelegate((o) => { execute(); }));
			th.AddExceptionHandler((e) => { Logger.WriteException(e,"PushNotifications"); });
			th.Name="SendPush_"+pushType.ToString()+"_ClinicNum_"+clinicNum.ToString();
			th.Start();
			if(!runAsync) { //Join back to main thread to cause this to be a blocking call.
				th.Join(Timeout.Infinite);
			}
		}		
		#endregion
	}
}
