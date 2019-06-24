using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeBase{
  public class ODEnvironment{
		///<summary>Constant that represnets number of milliseconds that can elapse between a first click and a second click 
		///for OD to consider the mouse action a double-click.
		///Currently used in ODGrid when HasDistinctClickEvents is true.
		///Eventually we may want to add pref to allow the user to set this.</summary>
		public const int DoubleClickTime=200;
		///<summary>Reflects the state of the laptop or slate mode, 0 for Slate Mode and non-zero otherwise.
		///When this system metric changes, the system sends a broadcast message via WM_SETTINGCHANGE with "ConvertibleSlateMode" in the LPARAM.
		///Note that this system metric doesn't apply to desktop PCs. In that case, use GetAutoRotationState.
		///See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getsystemmetrics for details.
		///Not supported in Windows CE.</summary>
		private const int SM_CONVERTIBLESLATEMODE=0x2003;
		///<summary>Nonzero if the current operating system is the Windows XP Tablet PC edition or if the current operating system is Windows Vista or Windows 7
		///and the Tablet PC Input service is started; otherwise, 0. The SM_DIGITIZER setting indicates the type of digitizer input supported by a device 
		///running Windows 7 or Windows Server 2008 R2. For more information, see Remarks.
		///See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getsystemmetrics for details.
		///Not supported in Windows CE.</summary>
		private const int SM_TABLETPC=86;

		#region Tablet Mode
		[DllImport("user32.dll",SetLastError=true,CharSet=CharSet.Auto,EntryPoint="GetSystemMetrics")]
		///<summary>Retrieves the specified system metric or system configuration setting. Note that all dimensions retrieved by GetSystemMetrics are in 
		///pixels. The system metric or configuration setting to be retrieved. This parameter can be one of the following values. Note that all SM_CX* 
		///values are widths and all SM_CY* values are heights. Also note that all settings designed to return Boolean data represent TRUE as any nonzero 
		///value, and FALSE as a zero value. See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getsystemmetrics and
		///https://docs.microsoft.com/en-us/windows/desktop/tablet/determining-whether-a-pc-is-a-tablet-pc for more details.</summary>
		private static extern int GetSystemMetrics(int nIndex);

		///<summary>Indicates if the current device is running in Windows Tablet mode and does not have a physical keyboard attached.
		///Returns false if any errors occurred while trying to query system metrics for information so that calling entities default to Desktop mode.</summary>
		public static bool IsTabletMode {
			get {
				bool isTableMode=false;
				ODException.SwallowAnyException(() => {
					isTableMode=QueryTabletMode();
				});
				return isTableMode;
			}
		}

		///<summary>Indicates if the current device is a Windows Tablet PC. This means that all of the following are true:
		///There is an integrated digitizer, either pen or touch, on the system. The Tablet PC optional component is installed. This component contains 
		///features such as Tablet PC Input Panel and Windows Journal. The computer is licensed to use the optional component. Premium versions of 
		///Windows Vista—such as Windows Vista Home Premium, Windows Vista Small Business, Windows Vista Professional, Windows Vista Enterprise, and 
		///Windows Vista Ultimate—are licensed to use the optional component. Tablet PC Input Service is running. Tablet PC Input Service is a new 
		///service for Windows Vista that controls Tablet PC input.
		///See https://docs.microsoft.com/en-us/windows/desktop/tablet/determining-whether-a-pc-is-a-tablet-pc for details.</summary>
		private static bool IsTabletPC {
			get {
				return (GetSystemMetrics(SM_TABLETPC)!=0);
			}
		}

		///<summary>True if the current device is both a Tablet PC and is also in slate mode, the combination of which we will consider to mean the
		///device is currently running in Tablet Mode.  SM_CONVERTIBLESLATEMODE reflects the state of the laptop or slate mode, 0 for Slate Mode and 
		///non-zero otherwise. Note that the SM_CONVERTIBLESLATEMODE system metric doesn't apply to desktop PCs, and as such is only useful for 
		///determining if a device is a Tablet PC and functioning as such in conjunction with SM_TABLETPC. Slate mode may be thought of as indicating 
		///that the Tablet PC is currently detached from a hardware keyboard.  An example is a Windows Surface with a detachable keyboard.  When the 
		///keyboard is attached, though the device is a still a Tablet PC, it is functioning like a laptop, therefore Slate mode is off.  When the 
		///keyboard is detached, the device is functioning as a tablet, i.e. in slate mode.</summary>
		private static bool QueryTabletMode() {
			return IsTabletPC && (GetSystemMetrics(SM_CONVERTIBLESLATEMODE)==0);
		}
		#endregion

		//public static bool Is64BitOperatingSystem(){
		//  string arch="";
		//  try{
		//      arch=Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
		//  }catch{
		//      //May fail if the environemnt variable is not present on the target machine (i.e. Unix).
		//  }
		//  bool retVal=Regex.IsMatch(arch,".*64.*");
		//  return retVal; 
		//}

		///<summary>Will return true if the provided id matches the local computer name or a local IPv4 or IPv6 address. Will return false if id is 'localhost' or '127.0.0.1'. Returns false in all other cases.</summary>
		public static bool IdIsThisComputer(string id){
			id=id.ToLower();
			//Compare ID against the local host name.
			if(Environment.MachineName.ToLower()==id){
			  return true;
			}
			IPHostEntry iphostentry;
			try {
				iphostentry=Dns.GetHostEntry(Environment.MachineName);
			}
			catch {
				return false;
			}
			//Check against the local computer's IP addresses (does not include 127.0.0.1). Includes IPv4 and IPv6.
			foreach(IPAddress ipaddress in iphostentry.AddressList){
			  if(ipaddress.ToString()==id){
			    return true;
			  }
			}
			return false;
		}

		///<summary>Will return true if the provided servername matches the local computer name or a local IPv4 or IPv6 address.  Will return true if servername is 'localhost' or '127.0.0.1'.  Returns false in all other cases.</summary>
		public static bool IsRunningOnDbServer(string servername) {
			servername=servername.ToLower();
			//Compare servername against the local host name.  Also check if the servername is "localhost".
			if(Environment.MachineName.ToLower()==servername || servername=="localhost") {
				return true;
			}
			//Check to see if the servername is an ipaddress that is a loopback (127.XXX.XXX.XXX).  Catches failure in parsing.
			try {
				if(IPAddress.IsLoopback(IPAddress.Parse(servername))) {
					return true;
				}
			}
			catch { }	//not a valid IP address
			IPHostEntry iphostentry;
			try {
				iphostentry=Dns.GetHostEntry(Environment.MachineName);
			}
			catch {
				return false;
			}
			//Check against the local computer's IP addresses (does not include 127.0.0.1). Includes IPv4 and IPv6.
			foreach(IPAddress ipaddress in iphostentry.AddressList) {
				if(ipaddress.ToString()==servername) {
					return true;
				}
			}
			return false;
		}

		///<summary>Returns an IPv4 address for the local machine. Returns an empty string if one cannot be found.</summary>
		public static string GetLocalIPAddress() {
			IPHostEntry iphostentry;
			try {
				iphostentry=Dns.GetHostEntry(Environment.MachineName);
			}
			catch {
				return "";
			}
			foreach(IPAddress ip in iphostentry.AddressList) {
				if(ip.AddressFamily==AddressFamily.InterNetwork) {
					return ip.ToString();
				}
			}
			return "";
		}
	}
}
