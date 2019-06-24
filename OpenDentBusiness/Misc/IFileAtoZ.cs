using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public interface IFileAtoZ {
		string ReadAllText(string fileName);
		void WriteAllText(string fileName,string textForFile);
		List<string> GetFilesInDirectory(string folder);
		string CombinePaths(params string[] paths);
		string AppendSuffix(string filePath,string suffix);
		bool Exists(string filePath);
		void Copy(string sourceFileName,string destinationFileName,FileAtoZSourceDestination sourceDestination,string uploadMessage="Copying file...",
			bool isFolder=false,bool doOverwrite=false);
		void Delete(string fileName);
		bool DirectoryExists(string folderName);
		Image GetImage(string imagePath);
	}
}
