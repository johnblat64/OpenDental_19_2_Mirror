using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Diagnostics;
using System.Net;

namespace CodeBase {
	public class ODFileUtils {

		[DllImport("kernel32.dll",SetLastError=true,CharSet=CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,out ulong lpFreebytesAvailable,out ulong lpTotalNumberOfBytes,out ulong lpTotalNumberOfFreeBytes);

		///<summary>This is a class scope variable in order to ensure that the random value is only seeded once for each time OD is launched.
		///Otherwise, if instantiated more often, then the same random numbers are generated over and over again.</summary>
		private static Random _rand=new Random();

		///<summary>Removes a trailing path separator from the given string if one exists.</summary>
		public static string RemoveTrailingSeparators(string path){
			while(path!=null && path.Length>0 && (path[path.Length-1]=='\\' || path[path.Length-1]=='/')) {
				path=path.Substring(0,path.Length-1);
			}
			return path;
		}

		public static string CombinePaths(string path1,string path2) {
			return CombinePaths(new string[] { path1,path2 });
		}

		public static string CombinePaths(string path1,string path2,char separator) {
			return CombinePaths(new string[] { path1,path2 },separator);
		}

		public static string CombinePaths(string path1,string path2,string path3) {
			return CombinePaths(new string[] { path1,path2,path3 });
		}

		public static string CombinePaths(string path1,string path2,string path3,char separator) {
			return CombinePaths(new string[] { path1,path2,path3 },separator);
		}

		public static string CombinePaths(string path1,string path2,string path3,string path4) {
			return CombinePaths(new string[] { path1,path2,path3,path4 });
		}

		public static string CombinePaths(string path1,string path2,string path3,string path4,char separator) {
			return CombinePaths(new string[] { path1,path2,path3,path4 },separator);
		}

		///<summary>OS independent path cominations. Ensures that each of the given path pieces are separated by the correct path separator for the current operating system. There is guaranteed not to be a trailing path separator at the end of the returned string (to accomodate file paths), unless the last specified path piece in the array is the empty string.</summary>
		public static string CombinePaths(string[] paths){
			string finalPath="";
			for(int i=0;i<paths.Length;i++){
				string path=RemoveTrailingSeparators(paths[i]);
				//Add an appropriate slash to divide the path peices, but do not use a trailing slash on the last piece.
				if(i<paths.Length-1){
					if(path!=null && path.Length>0){
						path=path+Path.DirectorySeparatorChar;
					}
				}
				finalPath=finalPath+path;
			}
			return finalPath;
		}

		///<summary>Ensures that each of the given path pieces are separated by the passed in separator character. 
		///There is guaranteed not to be a trailing path separator at the end of the returned string (to accomodate file paths), 
		///unless the last specified path piece in the array is the empty string.</summary>
		public static string CombinePaths(string[] paths,char separator) {
			return CombinePaths(paths).Replace(Path.DirectorySeparatorChar,separator);
		}

		///<summary>Reduces image size by changing it to Jpeg format and reducing image quality to 40%.</summary>
		public static string Compress(Bitmap image) {
			using(Bitmap bmp = new Bitmap(image))
			using(MemoryStream ms = new MemoryStream()) {
				ImageCodecInfo[] codecs=ImageCodecInfo.GetImageEncoders();
				ImageCodecInfo jpgEncoder=codecs.First(x => x.FormatID==ImageFormat.Jpeg.Guid);
				System.Drawing.Imaging.Encoder encoder=System.Drawing.Imaging.Encoder.Quality;
				EncoderParameters encoderParameters=new EncoderParameters(1);
				EncoderParameter encoderParameter=new EncoderParameter(encoder,40L);//Reduce quality to 40% of original
				encoderParameters.Param[0]=encoderParameter;
				bmp.Save(ms,jpgEncoder,encoderParameters);
				encoderParameters.Dispose();
				return Convert.ToBase64String(ms.ToArray());
			}
		}

		///<summary>This function takes a valid folder path.  Accepts UNC paths as well.  freeBytesAvail will contain the free space in bytes of the drive containing the folder.
		///It returns false if the function fails.</summary>
		public static bool GetDiskFreeSpace(string folder,out ulong freeBytesAvail) {
			freeBytesAvail=0;
			if(!folder.EndsWith("\\")) {
				folder+="\\";
			}
			ulong totBytes=0;
			ulong totFreeBytes=0;
			if(GetDiskFreeSpaceEx(folder,out freeBytesAvail,out totBytes,out totFreeBytes)) {
				return true;
			}
			else {
				return false;
			}
		}

		///<summary>Returns the directory in which the program executable rests. To get the full path for the program executable, use Applicaiton.ExecutablePath.</summary>
		public static string GetProgramDirectory(){
				int endPos=Application.ExecutablePath.LastIndexOf(Path.DirectorySeparatorChar);
				return Application.ExecutablePath.Substring(0,endPos+1);
		}

		///<summary>Creates a new randomly named file in the given directory path with the given extension and returns the full path to the new file.
		///The file name will include the local date and time down to the second.</summary>
		public static string CreateRandomFile(string dir,string ext,string prefix=""){
			if(ext.Length>0 && ext[0]!='.'){
				ext='.'+ext;
			}
			bool fileCreated=false;
			string filePath="";
			const string randChrs="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			do{
				string fileName=prefix;
				for(int i=0;i<6;i++){
					fileName+=randChrs[_rand.Next(0,randChrs.Length-1)];
				}
				fileName+=DateTime.Now.ToString("yyyyMMddhhmmss");
				filePath=CombinePaths(dir,fileName+ext);
				FileStream fs=null;
				try{
					fs=File.Create(filePath);
					fs.Dispose();
					fileCreated=true;
				}
				catch{
				}
			}while(!fileCreated);
			return filePath;
		}

		///<summary>Throws exceptions when there are permission issues.  Creates a new randomly named subdirectory inside the given directory path and returns the full path to the new subfolder.</summary>
		public static string CreateRandomFolder(string dir) {
			bool isFolderCreated=false;
			string folderPath="";
			const string randChrs="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			do {
				string subDirName="";
				for(int i=0;i<6;i++) {
					subDirName+=randChrs[_rand.Next(0,randChrs.Length-1)];
				}
				subDirName+=DateTime.Now.ToString("yyyyMMddhhmmss");
				folderPath=CombinePaths(dir,subDirName);
				if(!Directory.Exists(folderPath)) {
					Directory.CreateDirectory(folderPath);
					isFolderCreated=true;
				}
			} while(!isFolderCreated);
			return folderPath;
		}

		///<summary>Appends the suffix at the end of the file name but before the extension.</summary>
		public static string AppendSuffix(string filePath,string suffix) {
			string ext=Path.GetExtension(filePath);
			return CombinePaths(Path.GetDirectoryName(filePath),Path.GetFileNameWithoutExtension(filePath)+suffix+ext);
		}

		///<summary>Removes invalid characters from the passed in file name.</summary>
		public static string CleanFileName(string fileName) {
			return string.Join("_",fileName.Split(Path.GetInvalidFileNameChars()));
		}

		private static string ProcessUrlHelper(string exePath="",string extraArgs="",string extraFilePath="",string extraFileData="") {
			List<string> listParamStrs=new List<string>();
			listParamStrs.Add(string.IsNullOrWhiteSpace(exePath) ? "" : $"exePath=\"{WebUtility.UrlEncode(exePath)}\"");
			listParamStrs.Add(string.IsNullOrWhiteSpace(extraArgs) ? "" : $"extraArgs=\"{WebUtility.UrlEncode(extraArgs)}\"");
			listParamStrs.Add(string.IsNullOrWhiteSpace(extraFilePath) ? "" : $"extraFilePath={WebUtility.UrlEncode(extraFilePath)}");
			listParamStrs.Add(string.IsNullOrWhiteSpace(extraFileData) ? "" : $"extraFileData=\"{WebUtility.UrlEncode(extraFileData)}\"");
			listParamStrs.RemoveAll(x => string.IsNullOrWhiteSpace(x));
			return "odcloud:"+string.Join("&",listParamStrs);
		}

		///<summary>Start the given process.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client to start the process locally.</summary>
		public static void ProcessStart(Process process) {
			if(ODBuild.IsWeb()) {
				//We will only use the FileName and Arguments from the process's StartInfo.  Only non-web builds utilize the entire process.
				ProcessStart(process.StartInfo.FileName,process.StartInfo.Arguments);
			}
			else {
				process.Start();
			}
		}
		
		///<summary>Start a new process with the given path and arguments.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client to start the process locally.</summary>
		public static Process ProcessStart(string path,string commandLineArgs="") {
			if(ODBuild.IsWeb()) {
				string urlScheme=ProcessUrlHelper(path,commandLineArgs);
				return Process.Start(urlScheme);
			}
			return Process.Start(path,commandLineArgs);
		}
		
		///<summary>Write the given text to the given file.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client for File IO.</summary>
		public static void WriteAllText(string filePath,string text) {
			if(ODBuild.IsWeb()) {
				//Construct the odcloud client url scheme for writing to a file (save file path/save file data) and then call process.start
				string urlScheme=ProcessUrlHelper(extraFilePath:filePath,extraFileData:text);
				Process.Start(urlScheme);//This will open a new browser tab with the above URL and trigger odcloud client to open
			}
			else {
				File.WriteAllText(filePath,text);
			}
		}
		
		///<summary>Write the given text to the given file, then start a new process with the given path.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client for File IO and to start the process locally.</summary>
		public static Process WriteAllTextThenStart(string filePath,string fileText,string processPath) {
			return WriteAllTextThenStart(filePath,fileText,processPath,"");
		}
		
		///<summary>Write the given text to the given file, then start a new process with the given path.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client for File IO and to start the process locally.</summary>
		public static Process WriteAllTextThenStart(string filePath,string fileText,string processPath,string commandLineArgs) {
			if(ODBuild.IsWeb()) {
				string urlScheme=ProcessUrlHelper(processPath,commandLineArgs,filePath,fileText);
				return Process.Start(urlScheme);
			}
			else {
				File.WriteAllText(filePath,fileText);
				return Process.Start(processPath,commandLineArgs);
			}
		}
		
		///<summary>Write the given text to the given file, then start a new process with the given path.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client for File IO and to start the process locally.</summary>
		public static Process WriteAllTextThenStart(string filePath,string fileText,Encoding encoding,string processPath,string commandLineArgs) {
			if(ODBuild.IsWeb()) {
				//Purposefully omit encoding.  This can be an enhancement if needed.
				string urlScheme=ProcessUrlHelper(processPath,commandLineArgs,filePath,fileText);
				return Process.Start(urlScheme);
			}
			else {
				File.WriteAllText(filePath,fileText,encoding);
				return Process.Start(processPath,commandLineArgs);
			}
			
		}
	}
}
