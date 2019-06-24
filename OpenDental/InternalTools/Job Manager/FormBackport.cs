using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using static OpenDentBusiness.Backports;

namespace OpenDental {
	public partial class FormBackport:ODForm {
		///<summary>Stores all versions information. There is one for each BackportVersion.</summary>
		private List<ODVersion> _listVersions=new List<ODVersion>();
		///<summary>The path in the textPath when the data is refreshed.</summary>
		private string _pathOnRefresh;
		///<summary>The path in the textIgnoreList when the data is refreshed.</summary>
		private string _ignoreListNameOnRefresh;
		///<summary>This holds the current file changes.</summary>
		private List<ODFileChanges> _listFileChanges;
		///<summary>This holds the previously selected rows in the grid. This allows the user to select files and continue to double click to view 
		///svn differences without losing the selection.</summary>
		private int[] _arrayPreviousSelected;
		///<summary>This stores all the previously backported files and records the results of the backporting process.</summary>
		private List<ODBackportResult> _listBackportResults=new List<ODBackportResult>();
		///<summary>The current project on refresh. Provides support for SharedProjects.</summary>
		private BackportProject _currentProject=BackportProjects.Unknown;

		///<summary>Returns a list of versions that are currently checked to be backported to.</summary>
		private List<BackportVersion> _listVersionsToBackport {
			get {
				List<BackportVersion> listVersionsToBackport=new List<BackportVersion>();
				if(checkBeta.Checked) {
					listVersionsToBackport.Add(BackportVersion.Beta);
				}
				if(checkStable.Checked) {
					listVersionsToBackport.Add(BackportVersion.Stable);
				}
				if(checkPrevStable.Checked) {
					listVersionsToBackport.Add(BackportVersion.PreviousStable);
				}
				return listVersionsToBackport;
			}
		}

		public FormBackport() {
			InitializeComponent();
			#region Initialize Versions
			ODVersion odVersion=new ODVersion();
			string versions=VersionReleases.GetLastReleases(3);
			List<string> listVersions=versions.Split(';').Select(x => x.Substring(0,4)).ToList();
			odVersion.BackportVersion=BackportVersion.PreviousStable;
			odVersion.RawVersion=listVersions[0];
			odVersion.Version=new Version(listVersions[0]);
			_listVersions.Add(odVersion);
			odVersion=new ODVersion();
			odVersion.BackportVersion=BackportVersion.Stable;
			odVersion.RawVersion=listVersions[1];
			odVersion.Version=new Version(listVersions[1]);
			_listVersions.Add(odVersion);
			odVersion=new ODVersion();
			odVersion.BackportVersion=BackportVersion.Beta;
			odVersion.RawVersion=listVersions[2];
			odVersion.Version=new Version(listVersions[2]);
			_listVersions.Add(odVersion);
			checkBeta.Text+=listVersions[2];
			checkStable.Text+=listVersions[1];
			checkPrevStable.Text+=listVersions[0];
			butCompileBeta.Text+=listVersions[2];
			butCompileStable.Text+=listVersions[1];
			butCompilePrevStable.Text+=listVersions[0];
			#endregion Initialize Versions
			FillGrid();
		}

		///<summary>Fills the grid.</summary>
		private void FillGrid() {
			//Remove previously selected 
			_arrayPreviousSelected=null;
			//FillGrid
			gridMain.BeginUpdate();
			if(gridMain.Columns.Count==0) {
				gridMain.Columns.Clear();
				ODGridColumn col;
				col=new ODGridColumn("Type",75);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(_listVersions.Find(x => x.BackportVersion==BackportVersion.Beta).RawVersion,75);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(_listVersions.Find(x => x.BackportVersion==BackportVersion.Stable).RawVersion,75);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(_listVersions.Find(x => x.BackportVersion==BackportVersion.PreviousStable).RawVersion,75);
				gridMain.Columns.Add(col);
				col=new ODGridColumn("File Name",200);
				gridMain.Columns.Add(col);
				col=new ODGridColumn("File Path",50);
				gridMain.Columns.Add(col);
			}
			gridMain.Rows.Clear();
			ODGridRow row;
			if(_listFileChanges!=null) {
				foreach(ODFileChanges changedFile in _listFileChanges) {
					row=new ODGridRow();
					row.Cells.Add(Enum.GetName(changedFile.ModificationType.GetType(),changedFile.ModificationType));
					row.Cells.Add(GetStatusCell(BackportVersion.Beta,changedFile));
					row.Cells.Add(GetStatusCell(BackportVersion.Stable,changedFile));
					row.Cells.Add(GetStatusCell(BackportVersion.PreviousStable,changedFile));
					row.Cells.Add(changedFile.FileName);
					//Only paste path after head/
					row.Cells.Add(changedFile.FilePathHead.Substring(changedFile.FilePathHead.IndexOf("head")+4
						,changedFile.FilePathHead.Length-changedFile.FilePathHead.IndexOf("head")-4));
					row.Tag=changedFile;//Tag used to open changes in TortoiseSVN
					gridMain.Rows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		///<summary>Refreshes the data and changes the UI accordingly. Called from FillGrid when dataRefresh is true.</summary>
		private void RefreshData() {
			List<ODThread> listThreadsRunning=ODThread.GetThreadsByGroupName("FormBackport_Refresh");
			//Quit all threads that are still running. The user may have changed the path. This way the grid will not be filled with false information.
			listThreadsRunning.ForEach(x => x.QuitAsync());
			//Store path
			_pathOnRefresh=textPath.Text;
			_ignoreListNameOnRefresh=textIgnoreList.Text;
			_currentProject=BackportProjects.Unknown;
			//Clear Rows
			gridMain.BeginUpdate();
			gridMain.Rows.Clear();
			gridMain.EndUpdate();
			if(!Directory.Exists(textPath.Text)) {
				MessageBox.Show("The directory does not exist.");
				return;
			}
			else {
				if(_pathOnRefresh.Contains("OPEN DENTAL SUBVERSION")) {//look for open dental first as its naming scheme does not match the rest.
					_currentProject=BackportProjects.OpenDental;
				}
				else {
					for(int i=0;i<Enum.GetNames(typeof(ProjectName)).Length;i++) {
						if(_pathOnRefresh.Contains(Enum.GetNames(typeof(ProjectName))[i])){
							_currentProject=BackportProjects.ListProjects.Find(x => x.Name==((ProjectName)i));
							break;
						}
					}
				}
			}
			if(_currentProject==BackportProjects.Unknown) {
				MessageBox.Show("Could not find the correct project.");
				return;
			}
			Cursor=Cursors.AppStarting;
			//Refresh Data
			string pathOnRefresh=_pathOnRefresh;
			string ignoreListNameOnRefresh=_ignoreListNameOnRefresh;
			BackportProject currentProject=_currentProject.Copy();
			ODThread odThread=new ODThread((o) => {
				List<ODFileChanges> listFileChanges=GetListOfFiles(pathOnRefresh,
					_listVersions.Find(x => x.BackportVersion==BackportVersion.Beta).Version,
					_listVersions.Find(x => x.BackportVersion==BackportVersion.Stable).Version,
					_listVersions.Find(x => x.BackportVersion==BackportVersion.PreviousStable).Version,
					ignoreListNameOnRefresh,currentProject);
				this.InvokeIfNotDisposed(() => {//If window quit, this action will not run and the thread will die.
					if(o.HasQuit) {//If the user refreshed the path and this was marked to quit.
						return;
					}
					Cursor=Cursors.Default;
					_listFileChanges=listFileChanges;
					labelCurProj.Text="Current Project: "+Enum.GetName(_currentProject.Name.GetType(),_currentProject.Name);
					FillGrid();
				});
			});
			odThread.AddExceptionHandler(ex => {
				this.InvokeIfNotDisposed(() => {//If there's an exception after the form is closed, swallow and do not do anything.
					FriendlyException.Show("Error refreshing data.",ex);
				});
			});
			odThread.GroupName="FormBackport_Refresh";
			odThread.Name="FormBackport_Refresh"+DateTime.Now.Millisecond;
			odThread.Start();
		}

		///<summary>Returns an ODGridCell formatted to show the status of the backport.</summary>
		private ODGridCell GetStatusCell(BackportVersion backportVersion,ODFileChanges changedFile) {
			ODGridCell cell=new ODGridCell();
			ODBackportResult backportResult=_listBackportResults.Find(x => x.FilePathHead==changedFile.FilePathHead);
			if(backportResult!=null) {
				ResultType result=backportResult.GetResult(backportVersion);
				int numFailed=backportResult.GetFailedChanges(backportVersion).Count;
				cell.Text=result.ToString();
				switch(result) {
					case ResultType.Ok:
						cell.ColorText=Color.DarkGreen;
						break;
					case ResultType.Partial:
						cell.ColorText=Color.YellowGreen;
						cell.Text+=" ("+(changedFile.ListLineChanges.Count-numFailed).ToString()+"/"+changedFile.ListLineChanges.Count+")";
						break;
					case ResultType.Failed:
						cell.ColorText=Color.DarkRed;
						break;
					case ResultType.None:
					default:
						cell.ColorText=Color.Black;
						cell.Text="";
						break;
				}
			}
			return cell;
		}

		private void butBackport_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MessageBox.Show("Please select at least one file to backport first.");
				return;
			}
			//Get only selected files and store in local copy
			List<ODFileChanges> listFileChanges=_listFileChanges.Where(x => gridMain.SelectedTags<ODFileChanges>()
				.Any(y => x.FilePathHead==y.FilePathHead)).ToList();
			//Check only backporting Modified files
			if(listFileChanges.Any(x => x.ModificationType!=FileModificationType.Modified)) {
				MessageBox.Show("Only \"Modified\" files can be backported.");
				return;
			}
			//Make sure it is a known project
			if(_currentProject.Name==ProjectName.Unknown) {
				string currentProjects="";
				for(int i=0;i<Enum.GetNames(typeof(ProjectName)).Length;i++) {
					if((ProjectName)i==ProjectName.Unknown){
						continue;
					}
					if(i > 1) {
						currentProjects+=", ";
					}
					if(i==Enum.GetNames(typeof(ProjectName)).Length-1) {
						currentProjects+="and ";
					}
					currentProjects+=Enum.GetNames(typeof(ProjectName))[i];
				}
				MessageBox.Show("Unknown project. The currently supported projects are "+currentProjects+".\r\n\r\nNote: The opendental folder "
					+"should be called \"OPEN DENTAL SUBVERSION\"");
				return;
			}
			//Checks that the files are editable before beginning
			if(AreValidFiles(listFileChanges)) {
				//Begin the backportting process
				BeginBackport(listFileChanges);
			}
		}

		///<summary>Checks to make sure the files are able to be open. Returns true if they are and false if they are unavailable.</summary>
		private bool AreValidFiles(List<ODFileChanges> listFileChanges) {
			List<string> listErrors=new List<string>();
			string message="";
			foreach(ODFileChanges change in listFileChanges) {
				foreach(BackportVersion versionToCheck in _listVersionsToBackport) {
					try {
						using(FileStream stream=File.Open(change.DictVersionFilePath[versionToCheck],FileMode.Open)) {
							stream.Close();//The file could open, simply close it.
						}
					}
					catch {
						string error=change.FileName+" in "+_listVersions.Find(x => x.BackportVersion==versionToCheck).RawVersion;
						listErrors.Add(error);
					}
				}
			}
			if(listErrors.Count==0) {
				return true;
			}
			else if(listErrors.Count==1) {
				message="Could not open "+listErrors[0]+". It may be in use or may not exist.";
			}
			else if(listErrors.Count==2){
				message="Could not open "+listErrors[0]+" or "+listErrors[1]+". They may be in use or may not exist.";
			}
			else {//format correctly
				message="Could not open ";
				for(int i=0;i<listErrors.Count;i++) {
					if(i > 0) {
						message+=", ";
					}
					if(i==listErrors.Count-1) {
						message+="and ";
					}
					message+=listErrors[i];
				}
				message+=". They may be in use or may not exist.";
			}
			MessageBox.Show(message);
			return false;
		}

		///<summary>Begins the backporting process. All information in the FileChanges should be filled at this point.</summary>
		public void BeginBackport(List<ODFileChanges> listFileChanges) {
			foreach(BackportVersion version in _listVersionsToBackport) {
				BackportFiles(version,listFileChanges);
			}
			if(!_listVersionsToBackport.IsNullOrEmpty()) {
				FillGrid();
			}
		}

		///<summary>This backports the files that have not been backported yet.</summary>
		///<param name="backportVersion">The version that is being backported.</param>
		///<param name="listFileChanges">A list of the files that will be backported.</param>
		public void BackportFiles(BackportVersion backportVersion,List<ODFileChanges> listFileChanges) {
			foreach(ODFileChanges file in listFileChanges) {
				ODBackportResult backportResult=new ODBackportResult();
				backportResult.FilePathHead=file.FilePathHead;
				if(_listBackportResults.Any(x => x.FilePathHead==file.FilePathHead)) {
					backportResult=_listBackportResults.Find(x => x.FilePathHead==file.FilePathHead);
				}
				ResultType resultType=backportResult.GetResult(backportVersion);
				if(resultType!=ResultType.None) {//Skip already backported files
					continue;
				}
				resultType=ModifyFile(file,backportVersion,backportResult);
				backportResult.UpdateResult(backportVersion,resultType);
				if(!_listBackportResults.Any(x => x.FilePathHead==file.FilePathHead)) {//If not added yet, add here.
					_listBackportResults.Add(backportResult);
				}
			}
		}

		///<summary>Compiles the given project by opening it in Visual Studio and automatically beginning the build.</summary>
		private void Compile(BackportVersion backportVersion) {
			if(_currentProject.Name==ProjectName.Unknown) {
				return;
			}
			Version versionCur=_listVersions.Find(x => x.BackportVersion==backportVersion).Version;
			//Default path to VS 2019.
			string vsPath=@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.exe";
			if(!File.Exists(vsPath)) {
				//VS 2019 does not exist. Check VS 2015.
				vsPath=@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe";
				if(!File.Exists(vsPath)) {
					MessageBox.Show("Cannot find Visual Studio 2019 or 2015.");
					return;
				}
			}
			string command=$"\"{vsPath}\""
				+"/Run \"";
			string path=_pathOnRefresh+"\\";
			//add the versioned folder
			if(_currentProject.PatternMajor==MajorMinorPattern.MajorDotMinor) {
				path+=Enum.GetName(_currentProject.Name.GetType(),_currentProject.Name)+versionCur.Major+"."+versionCur.Minor+"\\";
			}
			else if(_currentProject.PatternMajor==MajorMinorPattern._Major_Minor) {
				path+=Enum.GetName(_currentProject.Name.GetType(),_currentProject.Name)+"_"+versionCur.Major+"_"+versionCur.Minor+"\\";
			}
			//add the path to the .sln (or .csproj in some cases). These are hardcoded as the patterns are all over the place.
			switch(_currentProject.Name) {
				case ProjectName.CDT:
					path+="xCDT.sln";
					break;
				case ProjectName.EHR:
					path+="EHR\\xEHR.csproj";
					break;
				case ProjectName.opendental:
					path+="OpenDental"+"_"+versionCur.Major+"_"+versionCur.Minor+".sln";
					break;
				case ProjectName.OpenDentalHelp:
					path+="OpenDentalHelp\\OpenDentalHelp.csproj";
					break;
				case ProjectName.ODCrypt:
				case ProjectName.OpenDentalService:
				case ProjectName.OpenDentalWebApps:
				case ProjectName.PhoneTrackingServer:
				default:
					path+=Enum.GetName(_currentProject.Name.GetType(),_currentProject.Name)+"_"+versionCur.Major+"_"+versionCur.Minor+".sln";
					break;
			}
			if(!File.Exists(path)) {
				MessageBox.Show("File does not exist.\r\n"+path);
				return;
			}
			Cursor=Cursors.WaitCursor;
			RunWindowsCommand(command+path+"\"",false);
			Cursor=Cursors.Default;
		}

		///<summary>Brings up the tortoise svn window for the currently selected path.</summary>
		private void Commit() {
			if(_currentProject==BackportProjects.Unknown || !Directory.Exists(_pathOnRefresh)) {
				return;
			}
			string command="TortoiseProc.exe /command:commit /path:\""+_pathOnRefresh+"\" ";
			Cursor=Cursors.WaitCursor;
			RunWindowsCommand(command,false);
			Cursor=Cursors.Default;
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshData();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_arrayPreviousSelected!=null) {
				gridMain.SetSelected(false);
				gridMain.SetSelected(_arrayPreviousSelected,true);
			}
			string path=((ODFileChanges)gridMain.Rows[e.Row].Tag).FilePathHead;
			string command="TortoiseProc.exe /command:diff /path:\""+path+"\"";
			RunWindowsCommand(command,false);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			_arrayPreviousSelected=gridMain.SelectedIndices;
		}

		private void butCompileBeta_Click(object sender,EventArgs e) {
			Compile(BackportVersion.Beta);
		}

		private void butCompileStable_Click(object sender,EventArgs e) {
			Compile(BackportVersion.Stable);
		}

		private void butCompilePrevStable_Click(object sender,EventArgs e) {
			Compile(BackportVersion.PreviousStable);
		}

		private void butCommit_Click(object sender,EventArgs e) {
			Commit();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}
	}
}
