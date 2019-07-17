using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace OpenDentBusiness {
	public partial class ConvertDatabases {

		private static void To18_3_1() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimTrackingStatusExcludesNone','0')";
			Db.NonQ(command);			
			//command="ALTER TABLE sheetfield ADD TabOrderMobile int NOT NULL"; //Moved and combined into AlterLargeTable call below
			//Db.NonQ(command);
			//command="ALTER TABLE sheetfield ADD UiLabelMobile varchar(255) NOT NULL";//Moved and combined into AlterLargeTable call below
			//Db.NonQ(command);
			//Adding three columns to sheetfield at once using AlterLargeTable() for performance reasons.
			//No translation in convert script.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1 - Adding TabOrderMobile,UiLabelMobile,UiLabelMobileRadioButton to sheetfield.");
			LargeTableHelper.AlterLargeTable("sheetfield","SheetFieldNum",new List<Tuple<string,string>> { Tuple.Create("TabOrderMobile","int NOT NULL"),
				Tuple.Create("UiLabelMobile","varchar(255) NOT NULL"),Tuple.Create("UiLabelMobileRadioButton","varchar(255) NOT NULL") });
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1");//No translation in convert script.
			command="ALTER TABLE sheetfielddef ADD TabOrderMobile int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE sheetfielddef ADD UiLabelMobile varchar(255) NOT NULL";
			Db.NonQ(command);
			//Add edit archived patient permission to everyone
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",165)";//165 is ArchivedPatientEdit
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1 - Adding credit card frequency");//No translation in convert script.
			command="ALTER TABLE creditcard ADD ChargeFrequency varchar(150) NOT NULL";
			Db.NonQ(command);
			command="UPDATE creditcard SET ChargeFrequency=CONCAT('0"//0 for ChargeFrequencyType.FixedDayOfMonth
				+"|',DAY(DateStart)) WHERE YEAR(DateStart) > 1880";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1");//No translation in convert script.
			command="ALTER TABLE insfilingcode ADD GroupType bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE insfilingcode ADD INDEX (GroupType)";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('OpenDentalServiceHeartbeat','0001-01-01 00:00:00')";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			string alertCategoryNum=Db.GetScalar(command);
			command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES('"+alertCategoryNum+"','20')";//20 for alerttype OpenDentalServiceDown
			Db.NonQ(command);
			command="ALTER TABLE rxdef ADD PatientInstruction text NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE rxpat ADD PatientInstruction text NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultRxInstructions','0')";
			Db.NonQ(command);
			//Insert Midway Dental Supply bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
			   +") VALUES("
			   +"'Midway', "
			   +"'Midway Dental', "
			   +"'0', "
			   +"'"+POut.String(@"http://www.midwaydental.com/")+"', "
			   +"'"+"', "//No command line args
			   +"'')";
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
			   +"VALUES ("
			   +"'"+POut.Long(programNum)+"', "
			   +"'7', "//ToolBarsAvail.MainToolbar
			   +"'Midway Dental')";
			Db.NonQ(command);
			//end Midway Dental Supply bridge
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsPayNoWriteoffMoreThanProc','1')";//InsPayNoWriteoffMoreThanProc-default to true
			Db.NonQ(command);
			#region Sales Tax
			//Insert new columns for sales tax properties
			command="ALTER TABLE adjustment ADD TaxTransID bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE adjustment ADD INDEX (TaxTransID)";
			Db.NonQ(command);
			//command="ALTER TABLE procedurelog ADD TaxAmt double NOT NULL"; //Moved to AlterLargeTable call below.
			//Db.NonQ(command); 
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1 - Adding TaxAmt to procedurelog.");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("procedurelog","ProcNum",new List<Tuple<string,string>> { Tuple.Create("TaxAmt","double NOT NULL") });
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1");//No translation in convert script.
			command="ALTER TABLE procedurecode ADD TaxCode varchar(16) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD TaxAmt double NOT NULL";
			Db.NonQ(command);
			//Create the Avalara Program Bridge
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'AvaTax', " 
				+"'AvaTax from Avalara.com', " 
				+"'0', " 
				+"'', " 
				+"'', "//leave blank if none 
				+"'')"; 
			programNum=Db.NonQ(command,true);
			long defNum=0;
			//Check to see if they already have a 'Sales Tax' Adjustment Type.  If they do, use that definition, otherwise insert a new definition.
			command="SELECT DefNum FROM definition WHERE Category=1 "//DefCat.AdjTypes
				+"AND LOWER(ItemName)='sales tax'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0) {
				defNum=PIn.Long(table.Rows[0][0].ToString());
			}
			if(defNum==0) {//We didn't find a definition named 'Sales Tax'.
				//Insert new status for 'Sales Tax'
				command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=1";
				int maxOrder=Db.GetInt(command);
				command="INSERT INTO definition (Category,ItemOrder,ItemName) VALUES (1,"+POut.Int(maxOrder)+",'Sales Tax')";
				defNum=Db.NonQ(command,true);
			}
			else {
				//Now set the def to not hidden (in case they already had a sales tax def that was hidden)
				command="UPDATE definition SET IsHidden=0 WHERE DefNum="+POut.Long(defNum);
				Db.NonQ(command);
			}
			//Avalara Program Property - SalesTaxAdjustmentType
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Sales Tax Adjustment Type', " 
				 +"'"+POut.Long(defNum)+"')"; 
			Db.NonQ(command);
			//Avalara Program Property - SalesTaxStates
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Taxable States', " 
				 +"'')"; 
			Db.NonQ(command);
			//Avalara Program Property - Username
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Username', " 
				 +"'')";  
			Db.NonQ(command);
			//Avalara Program Property - Password
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Password', " 
				 +"'')";
			Db.NonQ(command);
			//Avalara Program Property - CompanyCode
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Company Code', " 
				 +"'')"; 
			Db.NonQ(command); 
			//Avalara Program Property - Environment
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Test (T) or Production (P)', " 
				 +"'P')"; 
			Db.NonQ(command); 
			//Avalara Program Property - Log Level
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Log Level', " //0 - error, 1- information, 2- verbose 
				 +"'0')"; 
			Db.NonQ(command); 
			//Avalara Program Property - PrepayProcCodes
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Prepay Proc Codes', " //comma-separate list of proccodes we will allow users to pre-pay for
				 +"'')"; 
			Db.NonQ(command);
			//Avalara Program Property -  DiscountedProcCodes
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'Discount Proc Codes', " //comma-separate list of proccodes we give users discounts on when they prepay.
				 +"'')";
			Db.NonQ(command);
			#endregion Sales Tax
			command="ALTER TABLE substitutionlink ADD SubstitutionCode VARCHAR(25) NOT NULL,ADD SubstOnlyIf int NOT NULL";
			Db.NonQ(command);
			//Set all current substitutionlink.SubstOnlyIf to SubstitutionCondition.Never
			//The rows currently indicate substitutions for codes/conditions that will be ignored for the PlanNum.
			//Setting them to Never maintains this functionality
			command="UPDATE substitutionlink SET SubstOnlyIf=3";//SubstitutionCondition.Never;
			Db.NonQ(command);
			#region HQ Only
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				command="DROP TABLE IF EXISTS jobnotification";
				Db.NonQ(command);
				command=@"CREATE TABLE jobnotification (
					JobNotificationNum BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
					JobNum BIGINT NOT NULL,
					UserNum BIGINT NOT NULL,
					Changes TINYINT(4) NOT NULL,
					INDEX(JobNum),
					INDEX(UserNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			#endregion
			//command="ALTER TABLE sheetfield ADD UiLabelMobileRadioButton varchar(255) NOT NULL"; //Moved to AlterLargeTable call above.
			//Db.NonQ(command);
			command="ALTER TABLE sheetfielddef ADD UiLabelMobileRadioButton varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE sheet ADD HasMobileLayout tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE sheetdef ADD HasMobileLayout tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PromptForSecondaryClaim','1')";//default to true.
			Db.NonQ(command);
			command="ALTER TABLE repeatcharge ADD ChargeAmtAlt double NOT NULL";
			Db.NonQ(command);
		}

		private static void To18_3_2() {
			string command;
			command="UPDATE repeatcharge SET ChargeAmtAlt=-1";//This indicates that they have not started using Zipwhip yet.
			Db.NonQ(command);
		}

		private static void To18_3_4() {
			string command;
			command="ALTER TABLE claimform ADD Width int NOT NULL";
			Db.NonQ(command);
			command="UPDATE claimform SET Width=850";
			Db.NonQ(command);
			command="ALTER TABLE claimform ADD Height int NOT NULL";
			Db.NonQ(command);
			command="UPDATE claimform SET Height=1100";
			Db.NonQ(command);
		}

		private static void To18_3_8() {
			string command;
			command="SELECT COUNT(*) FROM program WHERE ProgName='PandaPeriodAdvanced'";
			//We may have already added this bridge in 18.2.29
			if(Db.GetCount(command)=="0") {
				//Update current PandaPerio description to Panda Perio (simple)
				command="UPDATE program SET ProgDesc='Panda Perio (simple) from www.pandaperio.com' WHERE ProgName='PandaPerio'";
				Db.NonQ(command);
				//Insert PandaPeriodAdvanced bridge----------------------------------------------------------------- 
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					 +"'PandaPeriodAdvanced', "
					 +"'Panda Perio (advanced) from www.pandaperio.com', "
					 +"'0', "
					 +"'"+POut.String(@"C:\Program Files (x86)\Panda Perio\Panda.exe")+"', "
					 +"'', "//leave blank if none 
					 +"'')";
				long programNum=Db.NonQ(command,true);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					 +") VALUES("
					 +"'"+POut.Long(programNum)+"', "
					 +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					 +"'0')";
				Db.NonQ(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					 +"VALUES ("
					 +"'"+POut.Long(programNum)+"', "
					 +"'2', "//ToolBarsAvail.ChartModule 
					 +"'PandaPeriodAdvanced')";
				Db.NonQ(command);
				//end PandaPeriodAdvanced bridge 
			}
		}

		private static void To18_3_9() {
			string command;
			//Add Avalara Program Property for Tax Exempt Pat Field
			command="SELECT ProgramNum FROM program WHERE ProgName='AvaTax'";
			long programNum=Db.GetLong(command);
			command="SELECT COUNT(*) FROM programproperty WHERE ProgramNum='"+POut.Long(programNum)+"' AND PropertyDesc='Tax Exempt Pat Field Def'";
			string taxExemptProperty=Db.GetCount(command);
			if(taxExemptProperty=="0") {
				//Avalara Program Property - SalesTaxAdjustmentType
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
						+") VALUES(" 
						+"'"+POut.Long(programNum)+"', " 
						+"'Tax Exempt Pat Field Def', " 
						+"'')"; 
				Db.NonQ(command);
			}
		}

		private static void To18_3_15() {
			string command;
			command="ALTER TABLE emailtemplate ADD IsHtml tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PrintStatementsAlphabetically','0')";
			Db.NonQ(command);
		}

		private static void To18_3_18() {
			string command;
			//Some of the default Web Sched Notify templates had '[ApptDate]' where they should have had '[ApptTime]'.
			foreach(string prefName in new List<string> { "WebSchedVerifyRecallText","WebSchedVerifyNewPatText","WebSchedVerifyASAPText" }) {
				command="SELECT ValueString FROM preference WHERE PrefName='"+POut.String(prefName)+"'";
				string curValue=Db.GetScalar(command);
				if(curValue=="Appointment scheduled for [FName] on [ApptDate] [ApptDate] at [OfficeName], [OfficeAddress]") {
					string newValue="Appointment scheduled for [FName] on [ApptDate] [ApptTime] at [OfficeName], [OfficeAddress]";
					command="UPDATE preference SET ValueString='"+POut.String(newValue)+"' WHERE PrefName='"+POut.String(prefName)+"'";
					Db.NonQ(command);
				}
			}
			//Remove supplyorders with supplynum=0 and update supplyorder totals
			command="SELECT DISTINCT SupplyOrderNum FROM supplyorderitem WHERE SupplyNum=0";
			List<long> supplyOrderNums=Db.GetListLong(command);
			for(int i=0;i<supplyOrderNums.Count;i++) {
				command="UPDATE supplyorder SET AmountTotal="
					+"(SELECT SUM(Qty*Price) FROM supplyorderitem WHERE SupplyOrderNum="+POut.Long(supplyOrderNums[i])+" AND SupplyNum!=0) "
					+"WHERE SupplyOrderNum="+POut.Long(supplyOrderNums[i]);
				Db.NonQ(command);
			}
			command="DELETE FROM supplyorderitem WHERE SupplyNum=0";
			Db.NonQ(command);
		}

		private static void To18_3_19() {
			string command;
			command="SELECT ProgramNum,Enabled FROM program WHERE ProgName='eRx'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0 && PIn.Bool(table.Rows[0]["Enabled"].ToString())) {
				long programErx=PIn.Long(table.Rows[0]["ProgramNum"].ToString());
				command=$"SELECT PropertyValue FROM programproperty WHERE PropertyDesc='eRx Option' AND ProgramNum={programErx}";
				//0 is the enum value of Legacy, 1 is the enum value of DoseSpot
				bool isNewCrop=PIn.Int(Db.GetScalar(command))==0;
				if(isNewCrop) {//Only update rows if the office has eRx enabled and is using NewCrop.
					command="UPDATE provider SET IsErxEnabled=2 WHERE IsErxEnabled=1";
					Db.NonQ(command);
				}
			}
			//check databasemaintenance for TransactionsWithFutureDates, insert if not there and set IsOld to True or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='TransactionsWithFutureDates'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				command="INSERT INTO databasemaintenance (MethodName,IsOld) VALUES ('TransactionsWithFutureDates',1)";
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'TransactionsWithFutureDates'";
			}
			Db.NonQ(command);
			//Add Avalara Program Property for TaxCodeOverrides
			command="SELECT ProgramNum FROM program WHERE ProgName='AvaTax'";
			long programNum=Db.GetLong(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Tax Code Overrides', " 
				+"'')"; 
			Db.NonQ(command);
		}

		private static void To18_3_21() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('SaveDXCAttachments','1')";
			Db.NonQ(command);
		}

		private static void To18_3_22() {
			string command;
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				//Change TimeEstimate to TimeEstimateDevelopment
				command="ALTER TABLE job CHANGE COLUMN TimeEstimate TimeEstimateDevelopment bigint(20) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job ADD TimeEstimateConcept bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job ADD INDEX (TimeEstimateConcept)";
				Db.NonQ(command);
				command="ALTER TABLE job ADD TimeEstimateWriteup bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job ADD INDEX (TimeEstimateWriteup)";
				Db.NonQ(command);
				command="ALTER TABLE job ADD TimeEstimateReview bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job ADD INDEX (TimeEstimateReview)";
				Db.NonQ(command);
			}
		}

		private static void To18_3_23() {
			string command;
			command="ALTER TABLE confirmationrequest ADD DoNotResend tinyint NOT NULL";
			Db.NonQ(command);
		}

		private static void To18_3_24() {
			string command;
			command="UPDATE displayfield SET InternalName='Country' WHERE InternalName='Contry' AND Category=16;";
			Db.NonQ(command);
		}

		private static void To18_3_26() {
			string command;
			FixB11013();
			//Moving codes to the Obsolete category that were deleted in CDT 2019.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				//Move deprecated codes to the Obsolete procedure code category.
				//Make sure the procedure code category exists before moving the procedure codes.
				string procCatDescript="Obsolete";
				long defNum=0;
				command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
				DataTable dtDef=Db.GetTable(command);
				if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
					command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
					int countCats=PIn.Int(Db.GetCount(command));
					command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D1515",
					"D1525",
					"D5281",
					"D9940"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
		}

		private static void To18_3_30() {
			string command;
			command=@"UPDATE preference SET ValueString='https://www.patientviewer.com:49997/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx' 
				WHERE PrefName='WebServiceHQServerURL' AND ValueString='http://www.patientviewer.com:49999/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx'";
			Db.NonQ(command);
		}

		private static void To18_3_49() {
			string command;
			command="UPDATE preference SET ValueString='https://opendentalsoft.com:1943/WebServiceCustomerUpdates/Service1.asmx' "
				+"WHERE PrefName='UpdateServerAddress' AND ValueString='http://opendentalsoft.com:1942/WebServiceCustomerUpdates/Service1.asmx'";
			Db.NonQ(command);
		}

		private static void To18_4_1() {
			string command;
			DataTable table;
			command="ALTER TABLE rxpat ADD ClinicNum bigint NOT NULL,ADD INDEX (ClinicNum)";
			Db.NonQ(command);
			//Set rxpat's ClinicNum to default patient's ClinicNum
			command=@"UPDATE rxpat
				INNER JOIN patient ON rxpat.PatNum=patient.PatNum
				SET rxpat.ClinicNum=patient.ClinicNum";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS pharmclinic";
			Db.NonQ(command);
			command=@"CREATE TABLE pharmclinic (
					PharmClinicNum bigint NOT NULL auto_increment PRIMARY KEY,
					PharmacyNum bigint NOT NULL,
					ClinicNum bigint NOT NULL,
					INDEX(PharmacyNum),
					INDEX(ClinicNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//Fill table with current combinations of ClinicNums and PharmacyNums that appear in the rxpat table
			command=@"INSERT INTO pharmclinic (PharmacyNum,ClinicNum)
				SELECT DISTINCT PharmacyNum,ClinicNum FROM rxpat WHERE PharmacyNum != 0";
			Db.NonQ(command);
			//Convert appt reminder rules' plain text email templates into HTML templates.
			command=@"SELECT ApptReminderRuleNum,TemplateEmail,TemplateEmailAggShared,TemplateEmailAggPerAppt,TypeCur
				FROM apptreminderrule";
			table=Db.GetTable(command);
			command="SELECT ValueString FROM preference WHERE PrefName='EmailDisclaimerIsOn'";
			bool isEmailDisclaimerOn=(Db.GetScalar(command)=="1");
			Func<string,bool,string> fConvertReminderRule=new Func<string, bool, string>((template,includeDisclaimer) =>
				template
					.Replace(">","&>")
					.Replace("<","&<")
					+(includeDisclaimer && isEmailDisclaimerOn ? "\r\n\r\n\r\n[EmailDisclaimer]" : "")
			);
			foreach(DataRow row in table.Rows) {
				string templateEmail=fConvertReminderRule(PIn.String(row["TemplateEmail"].ToString()),true);
				if(PIn.Int(row["TypeCur"].ToString())==1) {//Confirmation
					templateEmail=templateEmail.Replace("[ConfirmURL]","<a href=\"[ConfirmURL]\">[ConfirmURL]</a>");
				}
				if(PIn.Int(row["TypeCur"].ToString())==3) {//Patient Portal Invites
					templateEmail=templateEmail.Replace("[PatientPortalURL]","<a href=\"[PatientPortalURL]\">[PatientPortalURL]</a>");
				}
				string templateEmailAggShared=fConvertReminderRule(PIn.String(row["TemplateEmailAggShared"].ToString()),true);
				if(PIn.Int(row["TypeCur"].ToString())==1) {//Confirmation
					templateEmailAggShared=templateEmailAggShared.Replace("[ConfirmURL]","<a href=\"[ConfirmURL]\">[ConfirmURL]</a>");
				}
				if(PIn.Int(row["TypeCur"].ToString())==3) {//Patient Portal Invites
					templateEmailAggShared=templateEmailAggShared.Replace("[PatientPortalURL]","<a href=\"[PatientPortalURL]\">[PatientPortalURL]</a>");
				}
				string templateEmailAggPerAppt=fConvertReminderRule(PIn.String(row["TemplateEmailAggPerAppt"].ToString()),false);
				command=@"UPDATE apptreminderrule SET 
					TemplateEmail='"+POut.String(templateEmail)+@"',
					templateEmailAggShared='"+POut.String(templateEmailAggShared)+@"',
					TemplateEmailAggPerAppt='"+POut.String(templateEmailAggPerAppt)+@"'
					WHERE ApptReminderRuleNum="+POut.Long(PIn.Long(row["ApptReminderRuleNum"].ToString()));
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraIncludeWOPercCoPay','1')";
			Db.NonQ(command);
			command="ALTER TABLE tsitranslog ADD ClinicNum bigint NOT NULL,ADD INDEX (ClinicNum)";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReplaceExistingBlockout','0')";
			Db.NonQ(command);
			command="ALTER TABLE etrans835attach ADD DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ArchiveKey','')";
			Db.NonQ(command);		
			#region DentalXChange Patient Credit Score Bridge
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'DXCPatientCreditScore', " 
				+"'DentalXChange Patient Credit Score from register.dentalxchange.com', "
				+"'0', " 
				+"'', "//Takes to web portal. No local executable
				+"'', "
				+"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
			   +") VALUES("
			   +"'"+POut.Long(programNum)+"', "
			   +"'Disable Advertising', "
			   +"'0')";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
			   +"VALUES ("
			   +"'"+POut.Long(programNum)+"', "
			   +"'0', "//ToolBarsAvail.AccoutModule
			   +"'DXC Patient Credit Score')";
			Db.NonQ(command);
			#endregion
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsEstRecalcReceived','0')";
			Db.NonQ(command);	
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.4.1 - Adding index to appointment.Priority.");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("appointment","AptNum",null,
				new List<Tuple<string,string>> { Tuple.Create("Priority","") });//no need to send index name. only adds index if not exists
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.4.1");//No translation in convert script.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				 groupNum=PIn.Long(row["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",169)";  //169 is InsuranceVerification
				 Db.NonQ(command);
			}
			//Add columns to confirmationrequest
			command="ALTER TABLE confirmationrequest ADD SmsSentOk tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE confirmationrequest ADD EmailSentOk tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('UnscheduledListNoRecalls','1')";//True by default.
			Db.NonQ(command);	
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EnterpriseApptList','0')";
			Db.NonQ(command);
			//Insurance History preferences
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistExamCodes',ValueString FROM preference WHERE PrefName='InsBenExamCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistProphyCodes',ValueString FROM preference WHERE PrefName='InsBenProphyCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistBWCodes',ValueString FROM preference WHERE PrefName='InsBenBWCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPanoCodes',ValueString FROM preference WHERE PrefName='InsBenPanoCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioURCodes',ValueString FROM preference WHERE PrefName='InsBenSRPCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioULCodes',ValueString FROM preference WHERE PrefName='InsBenSRPCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioLRCodes',ValueString FROM preference WHERE PrefName='InsBenSRPCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioLLCodes',ValueString FROM preference WHERE PrefName='InsBenSRPCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioMaintCodes',ValueString FROM preference WHERE PrefName='InsBenPerioMaintCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistDebridementCodes',ValueString FROM preference WHERE PrefName='InsBenFullDebridementCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptAutoRefreshRange','4')";//Default to 4
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesPayType','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsPlanUseUcrFeeForExclusions','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsPlanExclusionsMarkDoNotBillIns','0')";
			Db.NonQ(command);
			command="ALTER TABLE insplan ADD ExclusionFeeRule tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE procbutton ADD IsMultiVisit tinyint NOT NULL";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS procmultivisit";
			Db.NonQ(command);
			command=@"CREATE TABLE procmultivisit (
				ProcMultiVisitNum bigint NOT NULL auto_increment PRIMARY KEY,
				GroupProcMultiVisitNum bigint NOT NULL,
				ProcNum bigint NOT NULL,
				ProcStatus tinyint NOT NULL,
				IsInProcess tinyint NOT NULL,
				INDEX(GroupProcMultiVisitNum),
				INDEX(ProcNum),
				INDEX(IsInProcess)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//Create an Enterprise Setup Window
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeatureEnterprise','0')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RepeatingChargesAutomated','0')";//False by default
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RepeatingChargesAutomatedTime','2018-01-01 08:00:00')";//8:00am by default(date portion not used).
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RepeatingChargesRunAging','1')";//True by default
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RepeatingChargesLastDateTime','0001-01-01 00:00:00')";//Initialize to MinDate.
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RecurringChargesBeginDateTime','')";//Initialize to empty.
			Db.NonQ(command);
			command="ALTER TABLE automation ADD PatStatus tinyint NOT NULL";
			Db.NonQ(command);
		}//End of 18_4_1() method

		private static void To18_4_16() {
			string command;
			//Add Avalara Program Property for Sales Tax Return Adjustment Type
			command="SELECT ProgramNum FROM program WHERE ProgName='AvaTax'";
			long programNum=Db.GetLong(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Sales Tax Return Adjustment Type', " 
				+"'')"; 
			Db.NonQ(command);
			//Add Avalara Program Property for Tax Lock Date
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Tax Lock Date', " 
				+"'')"; 
			Db.NonQ(command);
		}//End of 18_4_16() method
	
		private static void To18_4_17() {
			string command;
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistExamCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistProphyCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistBWCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPanoCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioURCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioULCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioLRCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioLLCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioMaintCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistDebridementCodes'";
			Db.NonQ(command);
		}//End of 18_4_17() method

		private static void To18_4_22() {
			string command;
			command="UPDATE preference SET ValueString='https://opendentalsoft.com:1943/WebServiceCustomerUpdates/Service1.asmx' "
				+"WHERE PrefName='UpdateServerAddress' AND ValueString='http://opendentalsoft.com:1942/WebServiceCustomerUpdates/Service1.asmx'";
			Db.NonQ(command);
		}

		private static void To18_4_29() {
			string command;
			command="ALTER TABLE tsitranslog ADD AggTransLogNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE tsitranslog ADD INDEX (AggTransLogNum)";
			Db.NonQ(command);
		}

		private static void To19_1_1() {
			string command;
			//Oryx bridge-------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				+") VALUES("
				+"'Oryx', "
				+"'Oryx from oryxdentalsoftware.com', "
				+"'0', "
				+"'', "//Opens website. No local executable.
				+"'', "
				+"'')";
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'Client URL', "
				 +"'')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'Disable Advertising', "
				 +"'0')";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				 +"VALUES ("
				 +"'"+POut.Long(programNum)+"', "
				 +"'2', "//ToolBarsAvail.ChartModule
				 +"'Oryx')";
			Db.NonQ(command);
			//End Oryx bridge---------------------------------------------
			command="INSERT INTO preference(PrefName,ValueString) VALUES('FeesUseCache','0')";
			Db.NonQ(command);
			command="ALTER TABLE providerclinic ADD StateLicense varchar(50) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE providerclinic ADD StateRxID varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE providerclinic ADD StateWhereLicensed varchar(15) NOT NULL";
			Db.NonQ(command);
			command="SELECT ProvNum FROM providerclinic WHERE ClinicNum=0 GROUP BY ProvNum";
			List<long> listDefaultProvClinicsProvNums=Db.GetListLong(command);
			command="SELECT ProvNum,DEANum,StateLicense,StateRxID,StateWhereLicensed FROM provider";
			Dictionary<long,DataRow> dictProvs=Db.GetTable(command).Select().ToDictionary(x=> PIn.Long(x["ProvNum"].ToString()));
			foreach(KeyValuePair<long,DataRow> kvp in dictProvs) {
				string stateRxId=kvp.Value["StateRxID"].ToString();
				string stateLicense=kvp.Value["StateLicense"].ToString();
				string stateWhereLicensed=kvp.Value["StateWhereLicensed"].ToString();
				if(kvp.Key.In(listDefaultProvClinicsProvNums)) {
					//A default providerClinic already exist for this provider. Update the providerclinic row for this provider with ClinicNum=0
					command="UPDATE providerclinic SET StateLicense='"+POut.String(stateLicense)+"', "
						+"StateRxID='"+POut.String(stateRxId)+"', "
						+"StateWhereLicensed='"+POut.String(stateWhereLicensed)+"' "
						+"WHERE ProvNum="+POut.Long(kvp.Key)+" AND ClinicNum=0";
				}
				else {
					//Providerclinic doesn't exist for this provider. Add a new default providerclinic for ClinicNum=0
					command="INSERT INTO providerclinic (ProvNum,ClinicNum,DEANum,StateLicense,StateRxID,StateWhereLicensed) "
						+"VALUES ("+POut.Long(kvp.Key)+","//ProvNum
							+"0,"//ClinicNum
							+"'"+POut.String(kvp.Value["DEANum"].ToString())+"',"
							+"'"+POut.String(stateLicense)+"',"
							+"'"+POut.String(stateRxId)+"',"
							+"'"+POut.String(stateWhereLicensed)+"')";
				}
				Db.NonQ(command);
			}
			command="SELECT ProgramNum FROM program WHERE ProgName='PayConnect'";
			long payConnectProgNum=PIn.Long(Db.GetScalar(command));
			command="SELECT DISTINCT ClinicNum FROM programproperty WHERE ProgramNum="+POut.Long(payConnectProgNum);
			List<long> listClinicNums=Db.GetListLong(command);
			foreach(long clinicNum in listClinicNums) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum"
					 +") VALUES("
					 +"'"+POut.Long(payConnectProgNum)+"', "
					 +"'Patient Portal Payments Token', "
					 +"'',"
					 +"'',"
					 +POut.Long(clinicNum)+")";
				Db.NonQ(command);
			}
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 2"; //monthly
			long itemorder = Db.GetLong(command)+1; //get the next available ItemOrder for the Monthly Category to put this new report last.
			command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) "
				 +"VALUES('ODProcOverpaid',"+POut.Long(itemorder)+",'Procedures Overpaid',2,0)";
			long reportNumNew=Db.NonQ(command,getInsertID:true);
			long reportNum=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODProcsNotBilled'");
			List<long> listGroupNums=Db.GetListLong("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportNum));
			foreach(long groupNumCur in listGroupNums) {
				command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
					 +"VALUES('0001-01-01',0,"+POut.Long(groupNumCur)+",22,"+POut.Long(reportNumNew)+")";
				Db.NonQ(command);
			}
			//Add preference WebSchedRecallDoubleBooking, default to 1 which will preserve old behavior by preventing double booking.
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedRecallDoubleBooking','1')";
			Db.NonQ(command);
			//Add preference WebSchedNewPatApptDoubleBooking, default to 1 which will preserve old behavior by preventing double booking.
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedNewPatApptDoubleBooking','1')";
			Db.NonQ(command);
			//Add user column to alertItem table
			command="ALTER TABLE alertitem ADD UserNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE alertitem ADD INDEX (UserNum)";
			Db.NonQ(command);
			long itemOrder=Db.GetLong("SELECT ItemOrder FROM displayreport WHERE InternalName='ODProviderPayrollSummary'")+1;
			command="UPDATE displayreport SET ItemOrder="+POut.Long(itemOrder)+" WHERE InternalName='ODProviderPayrollSummary'";
			Db.NonQ(command);//Move down provider payroll summary
			itemOrder=Db.GetLong("SELECT ItemOrder FROM displayreport WHERE InternalName='ODProviderPayrollDetailed'")+1;
			command="UPDATE displayreport SET ItemOrder="+POut.Long(itemOrder)+" WHERE InternalName='ODProviderPayrollDetailed'";
			Db.NonQ(command);//Move down provider payroll detailed
			itemOrder=Db.GetLong("SELECT displayreport.ItemOrder FROM displayreport WHERE displayreport.InternalName='ODMoreOptions'")+1;
			command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden,IsVisibleInSubMenu) "
					+"VALUES('ODMonthlyProductionGoal',"+POut.Long(itemOrder)+",'Monthly Prod Inc Goal',0,0,0)";//Insert Monthly Production Goal in the new space we just made after More Options
			Db.NonQ(command);
			long moreOptionsFKey=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODMoreOptions'");
			DataTable userGroupTable=Db.GetTable("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(moreOptionsFKey));
			reportNum=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODMonthlyProductionGoal'");
			foreach(DataRow row in userGroupTable.Rows) {
					command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES('0001-01-01',0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"+POut.Long(reportNum)+")";
					Db.NonQ(command);
			}
			//Create the Reactivatione Table
			command="DROP TABLE IF EXISTS reactivation";
				Db.NonQ(command);
				command=@"CREATE TABLE reactivation (
					ReactivationNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNum bigint NOT NULL,
					ReactivationStatus bigint NOT NULL,
					ReactivationNote text NOT NULL,
					DoNotContact tinyint NOT NULL,
					INDEX(PatNum),
					INDEX(ReactivationStatus)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			//Add Reactivation Preferences
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeatureReactivations','0')";
			Db.NonQ(command);
			//Add preference ReactivationContactInterval
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationContactInterval','0')";
			Db.NonQ(command);
			//Add preference ReactivationCountContactMax
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationCountContactMax','0')";
			Db.NonQ(command);
			//Add preference ReactivationDaysPast
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationDaysPast','1095')";
			Db.NonQ(command);
			//Add preference ReactivationEmailFamMsg
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationEmailFamMsg','')";
			Db.NonQ(command);
			//Add preference ReactivationEmailMessage
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationEmailMessage','')";
			Db.NonQ(command);
			//Add preference ReactivationEmailSubject
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationEmailSubject','')";
			Db.NonQ(command);
			//Add preference ReactivationGroupByFamily
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationGroupByFamily','0')";
			Db.NonQ(command);
			//Add preference ReactivationPostcardFamMsg
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationPostcardFamMsg','')";
			Db.NonQ(command);
			//Add preference ReactivationPostcardMessage
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationPostcardMessage','')";
			Db.NonQ(command);
			//Add preference ReactivationPostcardsPerSheet
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationPostcardsPerSheet','3')";
			Db.NonQ(command);
			//Add preference ReactivationStatusEmailed
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationStatusEmailed','0')";
			Db.NonQ(command);
			//Add preference ReactivationStatusEmailedTexted
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationStatusEmailedTexted','0')";
			Db.NonQ(command);
			//Add preference ReactivationStatusMailed
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationStatusMailed','0')";
			Db.NonQ(command);
			//Add preference ReactivationStatusTexted
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationStatusTexted','0')";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS mobileappdevice";
			Db.NonQ(command);
			command=@"CREATE TABLE mobileappdevice (
					MobileAppDeviceNum bigint NOT NULL auto_increment PRIMARY KEY,
					ClinicNum bigint NOT NULL,
					DeviceName varchar(255) NOT NULL,
					UniqueID varchar(255) NOT NULL,
					IsAllowed tinyint NOT NULL,
					LastAttempt datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					LastLogin datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					INDEX(ClinicNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DatabaseMode','0')";//Default to 'Normal'
			Db.NonQ(command);
			command="ALTER TABLE dunning ADD IsSuperFamily tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE registrationkey ADD HasEarlyAccess tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('Ins834DropExistingPatPlans','0')";//Default to false
			Db.NonQ(command);
			LargeTableHelper.AlterLargeTable("treatplan","TreatPlanNum",new List<Tuple<string,string>> {
				Tuple.Create("SignaturePractice","text NOT NULL"),
				Tuple.Create("DateTSigned","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
				Tuple.Create("DateTPracticeSigned","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
				Tuple.Create("SignatureText","varchar(255) NOT NULL"),
				Tuple.Create("SignaturePracticeText","varchar(255) NOT NULL")
			});
			command="DROP TABLE IF EXISTS providercliniclink";
			Db.NonQ(command);
			command=@"CREATE TABLE providercliniclink (
				ProviderClinicLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
				ProvNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				INDEX(ProvNum),
				INDEX(ClinicNum)
			) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//PaySimple ACH Payment Type
			command="SELECT ProgramNum FROM program WHERE ProgName='PaySimple'";
			programNum=Db.GetLong(command);
			command=$@"SELECT ClinicNum,PropertyValue FROM programproperty 
				WHERE ProgramNum={POut.Long(programNum)} AND PropertyDesc='PaySimple Payment Type'";
			DataTable table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ClinicNum
					) VALUES(
					{POut.Long(programNum)},
					'PaySimple Payment Type ACH',
					'{POut.String(PIn.String(row["PropertyValue"].ToString()))}',
					{POut.Long(PIn.Long(row["ClinicNum"].ToString()))})";
				Db.NonQ(command);
			}
			//Rename 'PaySimple Payment Type' program property to make it clear it's only for credit cards.
			command=$@"UPDATE programproperty SET PropertyDesc='PaySimple Payment Type CC' 
				WHERE ProgramNum={POut.Long(programNum)} AND PropertyDesc='PaySimple Payment Type'";
			Db.NonQ(command);
			//Rename 'RecurringChargesPayType' preference to make it clear it's only for credit cards.
			command=$@"UPDATE preference SET PrefName='RecurringChargesPayTypeCC' WHERE PrefName='RecurringChargesPayType'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ThemeSetByUser','0')";
			Db.NonQ(command);
			//Transworld Adjustment
			command="SELECT ProgramNum FROM program WHERE ProgName='Transworld'";
			long tsiProgNum=Db.GetLong(command);
			command="SELECT DISTINCT ClinicNum FROM programproperty WHERE ProgramNum="+POut.Long(tsiProgNum);//copied from different example, but found
			//that if program is not yet enabled, but using clinics, properties will not get inserted for all clinics if you decide to enable feature later. 
			List<long> listProgClinicNums=Db.GetListLong(command);
			for(int i=0;i < listProgClinicNums.Count;i++) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("+POut.Long(tsiProgNum)+",'SyncExcludePosAdjType','0','',"+POut.Long(listProgClinicNums[i])+"),"
					+"("+POut.Long(tsiProgNum)+",'SyncExcludeNegAdjType','0','',"+POut.Long(listProgClinicNums[i])+")";
				Db.NonQ(command);
			}
			command="ALTER TABLE sheetdef ADD UserNum bigint NOT NULL";//New column UserNum will default to 0.
			Db.NonQ(command);
			command="ALTER TABLE sheetdef ADD INDEX (UserNum)";
			Db.NonQ(command);
			command="ALTER TABLE sheetfielddef ADD LayoutMode tinyint NOT NULL";//New column SheetTypeMode will default to 0 (SheetTypeMode.Default)
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SendEmailsInDiffProcess','0')";//Default to 'False'
			Db.NonQ(command);
			command="ALTER TABLE tasklist ADD GlobalTaskFilterType tinyint NOT NULL";
			Db.NonQ(command);
			command="UPDATE tasklist SET GlobalTaskFilterType=1";//GlobalTaskFilterType.Default
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TasksGlobalFilterType','0')";//Require offices to turn this on.
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS payconnectresponseweb";
			Db.NonQ(command);
			command=@"CREATE TABLE payconnectresponseweb (
				PayConnectResponseWebNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				PayNum bigint NOT NULL,
				AccountToken varchar(255) NOT NULL,
				PaymentToken varchar(255) NOT NULL,
				ProcessingStatus tinyint NOT NULL,
				DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimePending datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeCompleted datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeExpired datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeLastError datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				LastResponseStr text NOT NULL,
				INDEX(PatNum),
				INDEX(PayNum)
			) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('GlobalUpdateWriteOffLastClinicCompleted','')";
			Db.NonQ(command);
			command="ALTER TABLE paysplit ADD AdjNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE paysplit ADD INDEX (AdjNum)";
			Db.NonQ(command);
		}//End of 19_1_1() method

		private static void To19_1_3() {
			string command;
			command="ALTER TABLE repeatcharge ADD UnearnedTypes varchar(4000) NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_1_4() {
			string command;
			command="ALTER TABLE payconnectresponseweb ADD CCSource tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD Amount double NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD PayNote varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb MODIFY ProcessingStatus VARCHAR(255) NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_1_6() {
			string command;
			command="ALTER TABLE sheetdef DROP COLUMN UserNum";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ChartDefaultLayoutSheetDefNum','0')";
			Db.NonQ(command);
		}

		private static void To19_1_7() {
			string command;
			command="ALTER TABLE laboratory ADD IsHidden tinyint NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_1_9() {
			string command;
			command="UPDATE displayreport SET Description='Monthly Production Goal' WHERE InternalName='ODMonthlyProductionGoal'";
			Db.NonQ(command);
			DataTable table;
			//Add permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				 groupNum=PIn.Long(row["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,NewerDays,PermType) "
						+"VALUES("+POut.Long(groupNum)+",1,174)";//174 - NewClaimsProcNotBilled, default lock days to 1 day.
				 Db.NonQ(command);
			}
		}

		private static void To19_1_21() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('CanadaCreatePpoLabEst','1')";//Enabled by default.
			Db.NonQ(command);
		}

		private static void To19_1_23() {
			string command;			
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientSelectSearchMinChars','1')";//1 char by default, to maintain current behavior
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientSelectSearchPauseMs','1')";//1 ms by default, to maintain current behavior
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientSelectSearchWithEmptyParams','0')";//0 for Unknown (YN enum), to maintain current behavior
			Db.NonQ(command);
		}

		private static void To19_1_25() {
			string command;
			command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES (1,21)";//eservices, WebMailRecieved
			Db.NonQ(command);
		}

		private static void To19_1_31() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EnterpriseNoneApptViewDefaultDisabled','0')";//Default to false
			Db.NonQ(command);
		}

		private static void To19_1_34() {
			string command;
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedRecallApptSearchAfterDays','')";//Default to no days
			Db.NonQ(command);
		}

		private static void To19_2_1() {
			string command;
			DataTable table;
			//TSI - ArManager Excluded default preferences - The values are set to the unsent tab defaults
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedExcludeBadAddresses',ValueString FROM preference WHERE PrefName='ArManagerExcludeBadAddresses'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedExcludeIfUnsentProcs',ValueString FROM preference WHERE PrefName='ArManagerExcludeIfUnsentProcs'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedExcludeInsPending',ValueString FROM preference WHERE PrefName='ArManagerExcludeInsPending'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedDaysSinceLastPay',ValueString FROM preference WHERE PrefName='ArManagerUnsentDaysSinceLastPay'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedMinBal',ValueString FROM preference WHERE PrefName='ArManagerUnsentMinBal'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedAgeOfAccount',ValueString FROM preference WHERE PrefName='ArManagerUnsentAgeOfAccount'";
			Db.NonQ(command);
			//Add the new AccountingCashPaymentType preference
			string payTypeDescript="Cash";
			long defNum=0;
			command=$"SELECT DefNum,IsHidden FROM definition WHERE Category=10 AND ItemName='{POut.String(payTypeDescript)}'";//10 is DefCat.PaymentTypes
			table=Db.GetTable(command);
			if(table.Rows.Count==0) { //The cash payment type does not exist, add it
				command="SELECT COUNT(*) FROM definition WHERE Category=10";//10 is DefCat.PaymentTypes
				int countCats=PIn.Int(Db.GetCount(command));
				command=$"INSERT INTO definition (Category,ItemName,ItemOrder) VALUES (10,'{POut.String(payTypeDescript)}',{POut.Int(countCats)})";//10 is DefCat.PaymentTypes
				defNum=Db.NonQ(command,true);
			}
			else { //The cash payment type already exists, get the existing DefNum
				defNum=PIn.Long(table.Rows[0]["DefNum"].ToString());
				if(PIn.Bool(table.Rows[0]["IsHidden"].ToString())) {
					//Unhide if Cash payment type is hidden.
					command=$"UPDATE definition SET IsHidden=0 WHERE DefNum={POut.Long(defNum)} AND Category=10";
					Db.NonQ(command);
				}
			}
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('AccountingCashPaymentType','{POut.Long(defNum)}')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptModuleUses2019Overhaul','0')";
			Db.NonQ(command);
			command="ALTER TABLE userod ADD MobileWebPin varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE userod ADD MobileWebPinFailedAttempts tinyint unsigned NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD INDEX (ProvNum)";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD DateTP date NOT NULL DEFAULT '0001-01-01'";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD ClinicNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD INDEX (ClinicNum)";
			Db.NonQ(command);
			command="ALTER TABLE carrier ADD TrustedEtransFlags tinyint NOT NULL";
			Db.NonQ(command);
			//Convert plain text email templates into HTML templates.
			command=@"SELECT PrefName,ValueString FROM preference 
				WHERE PrefName IN('WebSchedMessage','WebSchedMessage2','WebSchedMessage3','WebSchedAggregatedEmailBody','WebSchedVerifyRecallEmailBody',
					'WebSchedVerifyNewPatEmailBody','WebSchedVerifyASAPEmailBody','WebSchedAsapEmailTemplate','PatientPortalNotifyBody')";
			table=Db.GetTable(command);
			command="SELECT ValueString FROM preference WHERE PrefName='EmailDisclaimerIsOn'";
			bool isEmailDisclaimerOn=(Db.GetScalar(command)=="1");
			string ConvertEmailTemplate(string template) {
				return template
					.Replace(">","&>")
					.Replace("<","&<")
					+(isEmailDisclaimerOn ? "\r\n\r\n\r\n[EmailDisclaimer]" : "");
			}
			foreach(DataRow row in table.Rows) {
				string prefName=PIn.String(row["PrefName"].ToString());
				string valueString=ConvertEmailTemplate(PIn.String(row["ValueString"].ToString()))
					.Replace("[OfficePhone]","<a href=\"tel:[OfficePhone]\">[OfficePhone]</a>");
				switch(prefName) {
					case "WebSchedMessage":
					case "WebSchedMessage2":
					case "WebSchedMessage3":
					case "PatientPortalNotifyBody":
						valueString=valueString.Replace("[URL]","<a href=\"[URL]\">[URL]</a>");
						break;
					case "WebSchedAsapEmailTemplate":
						valueString=valueString.Replace("[AsapURL]","<a href=\"[AsapURL]\">[AsapURL]</a>");
						break;
				}
				command=$"UPDATE preference SET ValueString='{POut.String(valueString)}' WHERE PrefName='{POut.String(prefName)}'";
				Db.NonQ(command);
			}
			command=@"SELECT ClinicNum,PrefName,ValueString FROM clinicpref 
				WHERE PrefName IN('WebSchedVerifyRecallEmailBody','WebSchedVerifyNewPatEmailBody','WebSchedVerifyASAPEmailBody','WebSchedAsapEmailTemplate')";
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				long clinicNum=PIn.Long(row["ClinicNum"].ToString());
				string prefName=PIn.String(row["PrefName"].ToString());
				string valueString=ConvertEmailTemplate(PIn.String(row["ValueString"].ToString()))
					.Replace("[OfficePhone]","<a href=\"tel:[OfficePhone]\">[OfficePhone]</a>");
				switch(prefName) {
					case "WebSchedAsapEmailTemplate":
						valueString=valueString.Replace("[AsapURL]","<a href=\"[AsapURL]\">[AsapURL]</a>");
						break;
				}
				command=$@"UPDATE clinicpref SET ValueString='{POut.String(valueString)}' WHERE PrefName='{POut.String(prefName)}' 
					AND ClinicNum={POut.Long(clinicNum)}";
				Db.NonQ(command);
			}
			//Previously www.open-dent.com/updates/ redirected to www.opendental.com/updates/. Since we want to use HTTPS now, we will simply go directly
			//to www.opendental.com.
			command="UPDATE preference SET ValueString='https://www.opendental.com/updates/' WHERE PrefName='UpdateWebsitePath' "
				+"AND ValueString='http://www.open-dent.com/updates/'";
			Db.NonQ(command);
			command="ALTER TABLE payment ADD ExternalId varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payment ADD PaymentStatus tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ArchiveDoBackupFirst','1')";//Default to 'True'
			Db.NonQ(command);
			command="ALTER TABLE emailtemplate CHANGE IsHtml TemplateType tinyint(4) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE emailmessage CHANGE IsHtml HtmlType tinyint(4) NOT NULL";
			Db.NonQ(command);
			//eClipboad Prefs
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardCreateMissingFormsOnCheckIn','1')";//Default to 'True'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardAllowSelfCheckIn','1')";//Default to 'True'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardMessageComplete','You have successfully checked in. Please return this device to the front desk.')";//Default to 'False'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardPopupKioskOnCheckIn','0')";//Default to 'False'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardPresentAvailableFormsOnCheckIn','1')";//Default to 'True'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardAllowSelfPortraitOnCheckIn','0')";//Default to 'False'
			Db.NonQ(command);
			//Default to 'True' (A new clinic with no entry in ClinicPref will implicitly have EClipboardUseDefaults==true)
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardUseDefaults','1')";
			Db.NonQ(command);
			//Set the EClipboardUseDefaults pref to true for all clinics
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			if(listClinicNums.Count>0) {
				foreach(long clinicNum in listClinicNums) {
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'EClipboardUseDefaults',1)"; //Default to true
					Db.NonQ(command);
				}
			}
			//add EClipboardSheetDef
			command="DROP TABLE IF EXISTS eclipboardsheetdef";
			Db.NonQ(command);
			command=@"CREATE TABLE eclipboardsheetdef (
				EClipboardSheetDefNum bigint NOT NULL auto_increment PRIMARY KEY,
				SheetDefNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				ResubmitInterval bigint NOT NULL,
				ItemOrder int NOT NULL,
				INDEX(SheetDefNum),
				INDEX(ClinicNum),
				INDEX(ResubmitInterval)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//There was a spelling typo in the FieldName for the X835.TransactionHandlingDescript field for ERA sheet defs.
			//Sheet defs of type ERA are not saved to the database so the only thing that needs to be updated are custom sheet field defs.
			//This typo correction does not need to be backported because the misspelled word does not get printed.  It is only used in the sheet setup.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - ERA SheetFieldDefs");
			command="UPDATE sheetfielddef SET FieldName='TransHandlingDesc' WHERE FieldName='TransHandilingDesc'";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1");
			//Changes to mobileappdevice
			command="ALTER TABLE mobileappdevice ADD PatNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mobileappdevice ADD INDEX (PatNum)";
			Db.NonQ(command);
			command="ALTER TABLE mobileappdevice ADD LastCheckInActivity datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="SELECT DefNum FROM definition WHERE Category=38 AND ItemName='ServiceError'";//38 is DefCat.InsuranceVerificationStatus
			DataTable dtDef=Db.GetTable(command);
			int itemOrder;
			if(dtDef.Rows.Count==0) { //Def does not exist
				command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=38";//38 is DefCat.InsuranceVerificationStatus
				itemOrder=PIn.Int(Db.GetCount(command));
				command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
					+"VALUES (38"+",'ServiceError',"+POut.Int(itemOrder)+",'SE')";//38 is DefCat.InsuranceVerificationStatus
				Db.NonQ(command);
			}
			//Insert PDMP bridge-----------------------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'PDMP', " 
				+"'PDMP', " 
				+"'0', " 
				+"'', "//Opens website. No local executable.
				+"'', "//leave blank if none 
				+"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
			   +"VALUES ("
			   +"'"+POut.Long(programNum)+"', "
			   +"'7', "//ToolBarsAvail.MainToolbar
			   +"'PDMP')";
			Db.NonQ(command);
			//Insert Illinois PDMP program properties--------------------------------------------
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Illinois PDMP FacilityID', " 
				+"'')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Illinois PDMP Username', " 
				+"'')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Illinois PDMP Password', " 
				+"'')"; 
			Db.NonQ(command); 
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraShowControlIdFilter','0')";//Default to false
			Db.NonQ(command);
			//add ScheduledProcess			
			command="DROP TABLE IF EXISTS scheduledprocess";
			Db.NonQ(command);
			command=@"CREATE TABLE scheduledprocess (
					ScheduledProcessNum bigint NOT NULL auto_increment PRIMARY KEY,
					ScheduledAction varchar(50) NOT NULL,
					TimeToRun datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					FrequencyToRun varchar(50) NOT NULL,
					LastRanDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00'
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('EClipboardClinicsSignedUp','')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";//Setup
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",177)";  //FeatureRequestEdit
				Db.NonQ(command);
			}
			#region TsiTransLogs Excluded Adjustment Type
			command=@"SELECT programproperty.ClinicNum,
				MAX(CASE WHEN programproperty.PropertyDesc='SyncExcludePosAdjType' THEN programproperty.PropertyValue END) SyncExcludePosAdjType,
				MAX(CASE WHEN programproperty.PropertyDesc='SyncExcludeNegAdjType' THEN programproperty.PropertyValue END) SyncExcludeNegAdjType
				FROM programproperty
				INNER JOIN program ON programproperty.ProgramNum=program.ProgramNum AND program.ProgName='Transworld' AND program.Enabled
				WHERE programproperty.PropertyDesc IN('SyncExcludePosAdjType','SyncExcludeNegAdjType')
				GROUP BY programproperty.ClinicNum";
			//dictionary key=ClinicNum, value=string of comma delimited adj type longs SyncExcludePosAdjType and SyncExcludeNegAdjType, i.e. "3,2"
			Dictionary<long,string> dictClinExclAdjTypes=Db.GetTable(command).Select()
				.ToDictionary(x => PIn.Long(x["ClinicNum"].ToString()),
					x => POut.Long(PIn.Long(x["SyncExcludePosAdjType"].ToString()))+","+POut.Long(PIn.Long(x["SyncExcludeNegAdjType"].ToString())));
			if(dictClinExclAdjTypes.Count>0) {
				if(dictClinExclAdjTypes.ContainsKey(0)) {//if HQ clinic has props set, use them for any clinic with no props
					command="SELECT ClinicNum FROM clinic";
					Db.GetListLong(command)
						.FindAll(x => !dictClinExclAdjTypes.ContainsKey(x))
						.ForEach(x => dictClinExclAdjTypes[x]=dictClinExclAdjTypes[0]);
				}
				command=@"SELECT tsitranslog.TsiTransLogNum
					FROM tsitranslog
					INNER JOIN adjustment ON adjustment.AdjNum=tsitranslog.FKey
					WHERE tsitranslog.FKeyType=0 "//TsiFKeyType.Adjustment
					+"AND tsitranslog.TransType=-1 "//TsiTransType.None
					+$@"AND ({
						string.Join(" OR ",dictClinExclAdjTypes
							.GroupBy(x => x.Value,x => x.Key)
							.Select(x => $@"(tsitranslog.ClinicNum IN ({string.Join(",",x.Select(y => POut.Long(y)))}) AND adjustment.AdjType IN ({x.Key}))"))
					})";
				List<long> listLogNumsToUpdate=Db.GetListLong(command);
				if(listLogNumsToUpdate.Count>0) {
					command="UPDATE tsitranslog SET TransType=10 "//TsiTransType.Excluded
						+$@"WHERE TsiTransLogNum IN({string.Join(",",listLogNumsToUpdate)})";
					Db.NonQ(command);
				}
			}
			#endregion TsiTransLogs Excluded Adjustment Type
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptPreventChangesToCompleted','0')";//Off by default
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraAllowTotalPayments','1')";//Default to enabled for backward compatibility
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS loginattempt";
			Db.NonQ(command);
			command=@"CREATE TABLE loginattempt (
					LoginAttemptNum bigint NOT NULL auto_increment PRIMARY KEY,
					UserName varchar(255) NOT NULL,
					LoginType tinyint NOT NULL,
					DateTFail datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					INDEX(UserName(10))
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT ValueString FROM preference WHERE PrefName='AccountShowTrojanExpressCollect'";
			if(PIn.Bool(Db.GetScalar(command))) {//They had it enabled, enable the Trojan program link
				command="UPDATE program SET Enabled=1 WHERE ProgName='Trojan'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString='0' WHERE PrefName='AccountShowTrojanExpressCollect'";
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesAllowedWhenNoPatBal','1')";//Default to 'True'
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - Adding CanChargeWhenNoBal to creditcard");
			command="ALTER TABLE creditcard ADD CanChargeWhenNoBal tinyint NOT NULL";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - claim table structure");
			command=@"ALTER TABLE claim ADD DateIllnessInjuryPreg date NOT NULL DEFAULT '0001-01-01',
				ADD DateIllnessInjuryPregQualifier tinyint NOT NULL,
				ADD DateOther date NOT NULL DEFAULT '0001-01-01',
				ADD DateOtherQualifier tinyint NOT NULL,
				ADD IsOutsideLab tinyint NOT NULL,
				ADD ResubmissionCode tinyint NOT NULL";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1");
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PrePayAllowedForTpProcs','0')";//default to Off. 
			Db.NonQ(command);
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=29";//29 is PaySplitUnearnedType
			int order=PIn.Int(Db.GetCount(command));
			command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
				+"VALUES (29,'Treat Plan Pre-Payment',"+POut.Int(order)+",'X')";//29 is PaySplitUnearnedType
			defNum=Db.NonQ(command,true);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TpUnearnedType','"+POut.Long(defNum)+"')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TpPrePayIsNonRefundable','0')";
			Db.NonQ(command);
			command="SELECT MAX(displayreport.ItemOrder)+1 FROM displayreport WHERE displayreport.Category=3";//DisplayReportCategory.Lists
			itemOrder=Db.GetInt(command);
			command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES ('ODHiddenPaySplits',"+POut.Int(itemOrder)
				+",'Hidden Payment Splits',3,0)";//3 is DisplayReportCategory.Lists
			long reportNumNew=Db.NonQ(command,getInsertID:true);
			long reportNum=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODPayments'");
			List<long> listGroupNums=Db.GetListLong("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportNum));
			foreach(long groupNumCur in listGroupNums) {
				command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
				+"VALUES('0001-01-01',0,"+POut.Long(groupNumCur)+",22,"+POut.Long(reportNumNew)+")";
				Db.NonQ(command);
			}
			To19_2_1_LargeTableScripts();//IMPORTANT: Leave the large table scripts at the end of To19_2_0.
		}//End of 19_2_1() method

		private static void To19_2_1_LargeTableScripts() {
			string command;
			//We want it to be the last thing done so that if it fails for a large customer and we have to manually finish the update script we will only
			//have to verify that these large table scripts have run.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - claimproc table structure");//No translation in convert script.
			//If adding an index with the same name as an existing index, but with different column(s), the existing index will be dropped before adding the
			//new one.  If an index exists that has the same column(s) as an index being added, the index won't be added again regardless of the index names
			LargeTableHelper.AlterLargeTable("claimproc","ClaimProcNum",null,
				new List<Tuple<string,string>> { Tuple.Create("ClaimNum,ClaimPaymentNum,InsPayAmt,DateCP","indexOutClaimCovering"),
					Tuple.Create("Status,PatNum,DateCP,PayPlanNum,InsPayAmt,WriteOff,InsPayEst,ProcDate,ProcNum","indexAgingCovering") });
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - securitylog table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("securitylog","SecurityLogNum",null,
				new List<Tuple<string, string>> { Tuple.Create("PermType","") });//no need to send index name. only adds index if not exists
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - benefit table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("benefit","BenefitNum",new List<Tuple<string,string>> { Tuple.Create("SecDateTEntry","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
				Tuple.Create("SecDateTEdit","timestamp") },new List<Tuple<string,string>> { Tuple.Create("SecDateTEntry",""),Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - insverify table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("insverify","InsVerifyNum",new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","timestamp") },
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - insverifyhist table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("insverifyhist","InsVerifyHistNum",new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","timestamp") },
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - patientnote table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("patientnote","PatNum",new List<Tuple<string,string>> { Tuple.Create("SecDateTEntry","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
				Tuple.Create("SecDateTEdit","timestamp") },new List<Tuple<string,string>> { Tuple.Create("SecDateTEntry",""),Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - task table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("task","TaskNum",new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","timestamp") },
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - taskhist table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("taskhist","TaskHistNum",new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","timestamp") },
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - procedurelog table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("procedurelog","ProcNum",new List<Tuple<string,string>>{ Tuple.Create("Urgency","tinyint NOT NULL") });
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1");//No translation in convert script.
			command="UPDATE benefit SET SecDateTEdit = NOW()";
			Db.NonQ(command);
			command="UPDATE insverify SET SecDateTEdit = NOW()";
			Db.NonQ(command);
			command="UPDATE insverifyhist SET SecDateTEdit = NOW()";
			Db.NonQ(command);
			command="UPDATE patientnote SET SecDateTEdit = NOW()";
			Db.NonQ(command);
			command="UPDATE task SET SecDateTEdit = NOW()";
			Db.NonQ(command);
			command="UPDATE taskhist SET SecDateTEdit = NOW()";
			Db.NonQ(command);
		}

		private static void To19_2_2() {
			string command;
			DataTable table;
			//XDR locationID for headquarters, LocationIDs for other clinics will be generated through the UI
			command="SELECT ProgramNum FROM program WHERE ProgName='XDR'";
			long programNum=Db.GetLong(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue) VALUES(
				'{POut.Long(programNum)}', 
				'Location ID', 
				'')";
			Db.NonQ(command);
			command="SELECT ValueString FROM preference WHERE PrefName='RigorousAccounting'";
			int intValue=Db.GetInt(command);//Rigorous accounting has three selection options Don't Enforce(2), AutoSplitOnly(1), EnforceFully(0)
			if(intValue==1 || intValue==2) {//if 1 or 2, prepayments to providers were always allowed. 
				command="UPDATE preference SET ValueString='1' WHERE PrefName='AllowPrepayProvider'";
				Db.NonQ(command);
			}
			command="ALTER TABLE claim MODIFY DateIllnessInjuryPregQualifier smallint NOT NULL,MODIFY DateOtherQualifier smallint NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_2_3() {
			string command;
			DataTable table;
			#region HQ Only
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				//Add new columns
				command="ALTER TABLE job ADD DateTimeConceptApproval datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
				command="ALTER TABLE job ADD DateTimeJobApproval datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
				command="ALTER TABLE job ADD DateTimeImplemented datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
				//Use JobLog rows to update correct info
				command=@"UPDATE job j 
									INNER JOIN(
										SELECT jl.JobNum,MAX(jl.DateTimeEntry) SetDate
										FROM JobLog jl
										WHERE jl.Description LIKE '%Concept approved.%'
										GROUP BY jl.JobNum) AS jl2 ON j.JobNum=jl2.JobNum
									SET DateTimeConceptApproval=jl2.SetDate";
				Db.NonQ(command);
				command=@"UPDATE job j 
									INNER JOIN (
										SELECT jl.JobNum,MAX(jl.DateTimeEntry) SetDate
										FROM JobLog jl
										WHERE jl.Description LIKE '%Job approved.%' OR jl.Description LIKE '%Changes approved.%'
										GROUP BY jl.JobNum) AS jl2 ON j.JobNum=jl2.JobNum
									SET DateTimeJobApproval=jl2.SetDate";
				Db.NonQ(command);
				command=@"UPDATE job j 
									INNER JOIN (
										SELECT jl.JobNum,MAX(jl.DateTimeEntry) SetDate
										FROM JobLog jl
										WHERE jl.Description LIKE '%Job implemented.%' OR jl.Description LIKE '%to Complete.%'
										GROUP BY jl.JobNum) AS jl2 ON j.JobNum=jl2.JobNum
									SET DateTimeImplemented=jl2.SetDate";
				Db.NonQ(command);
			}
			#endregion
			//Change the type of sheetfielddef.UiLabelMobile from varchar to text
			LargeTableHelper.AlterLargeTable("sheetfielddef","SheetFieldDefNum",new List<Tuple<string, string>>() { 
				Tuple.Create("UiLabelMobile","text NOT NULL"),
				Tuple.Create("UiLabelMobileRadioButton","text NOT NULL")
			});
			LargeTableHelper.AlterLargeTable("sheetfield","SheetFieldNum",new List<Tuple<string, string>>() { 
				Tuple.Create("UiLabelMobile","text NOT NULL"),
				Tuple.Create("UiLabelMobileRadioButton","text NOT NULL")
			});
		}

		private static void To19_2_4() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailAlertMaxConsecutiveFails','3')";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			long alertCategoryNum=Db.GetLong(command);
			//22 for alerttype EconnectorEmailTooManySendFails
			command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+",'22')";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices' AND IsHQCategory=1";
			alertCategoryNum=Db.GetLong(command);
			//22 for alerttype EconnectorEmailTooManySendFails
			command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+",'22')";
			Db.NonQ(command);
			command="ALTER TABLE insverify ADD INDEX (DateTimeEntry)";
			Db.NonQ(command);
			command="ALTER TABLE task ADD INDEX (DateTimeOriginal)";
			Db.NonQ(command);
			command="ALTER TABLE taskhist ADD INDEX (DateTStamp)";
			Db.NonQ(command);
		}

		private static void To19_2_7() {
			long chartDefaultDefNum=PIn.Long(Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='ChartDefaultLayoutSheetDefNum'"),false);
			Db.NonQ("DELETE FROM userodpref WHERE FKeyType=18 AND Fkey="+POut.Long(chartDefaultDefNum));//18 => DynamicChartLayout
		}

		private static void To19_2_8() {
			string command="INSERT INTO preference(PrefName,ValueString) VALUES('OpenDentalServiceComputerName','')";
			Db.NonQ(command);
			command="UPDATE claim SET CorrectionType=1 WHERE ResubmissionCode=7 AND CorrectionType=0";//CorrectionType 1 and ResubmissionCode 7 are 'Replacement'
			Db.NonQ(command);
			command="UPDATE claim SET CorrectionType=2 WHERE ResubmissionCode=8 AND CorrectionType=0";//CorrectionType 2 and ResubmissionCode 8 are 'Void'
			Db.NonQ(command);
			command="UPDATE claimformitem SET FieldName='CorrectionType' WHERE FieldName='ResubmissionCode'";
			Db.NonQ(command);
			command="ALTER TABLE claim DROP COLUMN ResubmissionCode";
			Db.NonQ(command);
		}

	}
}