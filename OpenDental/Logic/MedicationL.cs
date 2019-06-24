using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public class MedicationL {

		///<summary>Throws Exception. Downloads default medications list from OpenDental.com; returns filename of temp file.</summary>
		public static string DownloadDefaultMedicationsFile() {
			string tempFile=PrefC.GetRandomTempFile(".tmp");
			using(WebClient client=new WebClient()) {
				client.DownloadFile("http://www.opendental.com/medications/DefaultMedications.txt",tempFile);
			}
			return tempFile;
		}

		///<summary>Inserts any new medications in listNewMeds, as well as updating any existing medications in listExistingMeds in conflict with 
		///the corresponding new medication.</summary>
		public static int ImportMedications(List<ODTuple<Medication,string>> listImportMeds,List<Medication> listMedsExisting) {
			int countImportedMedications=0;
			foreach(ODTuple<Medication,string> medGenPair in listImportMeds) {//Loop through new medications/given generic name pairs.
				//Find any duplicate existing medications with the new medication
				if(IsDuplicateMed(medGenPair,listMedsExisting)) {
					continue;//medNew already exists, skip it.
				}
				InsertNewMed(medGenPair,listMedsExisting);
				countImportedMedications++;
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0
				,Lans.g("Medications","Imported")+" "+POut.Int(countImportedMedications)+" "+Lans.g("Medications","medications.")
			);
			return countImportedMedications;
		}

		///<summary>Determines if med is a duplicate of another Medication in listMedsExisting.
		///Given medGenNamePair is a medication that we are checking and the given generic name if set.
		///A duplicate is defined as MedName is equal, GenericName is equal, RxCui is equal and either Notes is equal or not defined.
		///A new medication with all properties being equal to an existing medication except with a blank Notes property is considered to be a 
		///duplicate, as it is likely the existing Medication is simply a user edited version of the same Medication.</summary>
		private static bool IsDuplicateMed(ODTuple<Medication,string> medGenNamePair,List<Medication> listMedsExisting) {
			Medication med=medGenNamePair.Item1;
			string genericName=medGenNamePair.Item2;
			bool isNoteChecked=true;
			//If everything is identical, except med.Notes is blank while x.Notes is not blank, we consider this to be a duplicate.
			if(string.IsNullOrEmpty(med.Notes)) {
				isNoteChecked=false;
			}
			return listMedsExisting.Any(
				x => x.MedName.Trim().ToLower()==med.MedName.Trim().ToLower() 
				&& Medications.GetGenericName(x.GenericNum).Trim().ToLower()==genericName.Trim().ToLower() 
				&& x.RxCui==med.RxCui
				&& (isNoteChecked ? (x.Notes.Trim().ToLower()==med.Notes.Trim().ToLower()) : true)
			);
		}

		///<summary>Inserts the given medNew.
		///Given medGennamePair is a medication that we are checking and the given generic name if set.
		///ListMedsExisting is used to identify the GenericNum for medNew.</summary>
		private static void InsertNewMed(ODTuple<Medication,string> medGenNamePair,List<Medication> listMedsExisting) {
			Medication medNew=medGenNamePair.Item1;
			string genericName=medGenNamePair.Item2;
			long genNum=listMedsExisting.FirstOrDefault(x => x.MedName==genericName)?.MedicationNum??0;
			if(genNum!=0) {//Found a match.
				medNew.GenericNum=genNum;
			}
			Medications.Insert(medNew);//Assigns new primary key.
			if(genNum==0) {//Found no match initially, assume given medication is the generic.
				medNew.GenericNum=medNew.MedicationNum;
				Medications.Update(medNew);
			}
			listMedsExisting.Add(medNew);//Keep in memory list and database in sync.
		}
		
		///<summary>Throws Exception.  Exports all medications to the passed in filename. Throws Exceptions.</summary>
		public static int ExportMedications(string filename,List<Medication> listMedications) {
			StringBuilder strBldrOutput=new StringBuilder();
			foreach(Medication med in listMedications) {//Loop through medications.
				/* Do not use POut here, as this will try to escape special characters, this is for writing to a file, not the db.
				 * Importing uses PIn, so we don't have to worry about these special characters causing problems on import.
				 * Using POut causes actual duplicates to not be detected on import. In other words, if you import a file of 10 medications, make no changes,
				 * export, then import this newly exported file, any medications with special characters will not register as a duplicate and you will end up
				 * with more than 10 medications in your list, some of them being duplicates except with \' instead of ' (as an example special character).
				 * */
				strBldrOutput.AppendLine(med.MedName+'\t'+Medications.GetGenericName(med.GenericNum)+'\t'+med.Notes+'\t'+med.RxCui);
			}
			File.WriteAllText(filename,strBldrOutput.ToString());//Allow Exception to trickle up.
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,
				Lans.g("Medications","Exported")+" "+POut.Int(listMedications.Count)+" "+Lans.g("Medications","medications to:")+" "+filename
			);
			return listMedications.Count;
		}

		///<summary>Throws exception.  Reads tab delimited medication information from given filename.
		///Returns the list of new medications with all generic medications before brand medications.
		///File required to be formatted such that each row contain: MedName\tGenericName\tNotes\tRxCui
		///</summary>
		public static List<ODTuple<Medication,string>> GetMedicationsFromFile(string filename,bool isTempFile=false) {
			List<ODTuple<Medication,string>> listMedsNew=new List<ODTuple<Medication,string>>();
			if(string.IsNullOrEmpty(filename)) {
				return listMedsNew;	
			}
			string medicationData=File.ReadAllText(filename);
			if(isTempFile) {
				File.Delete(filename);
			}
			List<string[]> listMedLines=SplitLines(medicationData);
			foreach(string[] medLine in listMedLines) {
				if(medLine.Length!=4) {
					throw new ODException(Lan.g("Medications","Invalid formatting detected in file."));
				}
				Medication medication=new Medication();
				medication.MedName=PIn.String(medLine[0]).Trim();//MedName
				string genericName=PIn.String(medLine[1]).Trim();//GenericName, not a field in Medication.cs but used for matching.
				medication.Notes=PIn.String(medLine[2]).Trim();//Notes
				medication.RxCui=PIn.Long(medLine[3]);//RxCui
				listMedsNew.Add(new ODTuple<Medication, string>(medication,genericName));
			}
			return SortMedGenericsFirst(listMedsNew);
		}

		///<summary>Returns a list of string arrays for the provided data.
		///Lines are determined by new line characters and tabs between fields.</summary>
		private static List<string[]> SplitLines(string data) {
			List<string[]> listLines=new List<string[]>();
			if(data==null) {
				return listLines;
			}
			if(data.Contains("\r\n")){
				data=data.Replace("\r\n","\n");
			}
			foreach(string line in data.Split('\n')) {//Remove any non Medication lines.
				string[] fields=line.Split('\t');
				if(fields.Length<1 || string.IsNullOrEmpty(fields[0])) {//Skip blank lines, blank MedicationName.
					continue;
				}
				listLines.Add(fields);
			}
			return listLines;
		}

		///<summary>Custom sorting so that generic medications are above branded medications.
		///Given list elements are a ODTuple of a medication and the given generic name if set.</summary>
		private static List<ODTuple<Medication,string>> SortMedGenericsFirst(List<ODTuple<Medication,string>> listMedLines) {
			List<ODTuple<Medication,string>> listMedGeneric=new List<ODTuple<Medication,string>>();
			List<ODTuple<Medication,string>> listMedBranded=new List<ODTuple<Medication,string>>();
			foreach(ODTuple<Medication,string> pair in listMedLines) {
				Medication med=pair.Item1;
				string genericName=pair.Item2;
				if(med.MedName.ToLower().In(genericName.ToLower(),"")) {//Generic if names directly match, or assume generic if no genericName provided.
					listMedGeneric.Add(pair);
				}
				else {//Branded
					listMedBranded.Add(pair);
				}
			}
			listMedGeneric.AddRange(listMedBranded);
			return listMedGeneric;
		}
	}

}