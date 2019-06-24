﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class DashPatPicture:ODPictureBox,IDashWidgetField {
		private Bitmap _patPicture;
		private Document _docPatPicture;

		public DashPatPicture() {
			InitializeComponent();
		}

		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(!IsNecessaryDataAvailable(data)) {
				return;
			}
			if(_docPatPicture==null) {
				return;
			}
			if(sheetField!=null && data.ListDocuments.Any(x => x.DocNum==_docPatPicture.DocNum)) {
				Bitmap patPicture=ImageHelper.GetThumbnail(data.BitmapImagesModule,Math.Min(sheetField.Width,sheetField.Height));
				_patPicture?.Dispose();
				_patPicture=patPicture;
			}
		}

		private bool IsNecessaryDataAvailable(PatientDashboardDataEventArgs data) {
			if(data.Pat==null || data.ListDocuments==null || data.BitmapImagesModule==null) { 
				return false;//Necessary data not found in PatientDashboardDataEventArgs.  Make no change.
			}
			return true;
		}

		public void RefreshData(Patient pat,SheetField sheetField) {
			if(pat==null || 
				PrefC.AtoZfolderUsed==DataStorageType.InDatabase)//Do not use patient image when A to Z folders are disabled.
			{
				return;
			}
			try{
				long newDocNum=PIn.Long(sheetField.FieldValue);
				if(_docPatPicture==null || newDocNum!=_docPatPicture.DocNum) {
					_docPatPicture=Documents.GetByNum(newDocNum,true);
					Bitmap fullImage=ImageHelper.GetFullImage(_docPatPicture,ImageStore.GetPatientFolder(pat,ImageStore.GetPreferredAtoZpath()));
					Bitmap patPicture=ImageHelper.GetThumbnail(fullImage,Math.Min(sheetField.Width,sheetField.Height));
					_patPicture?.Dispose();
					_patPicture=patPicture;
					fullImage.Dispose();
				}
			}
			catch(Exception e){
				e.DoNothing();
				_patPicture?.Dispose();
				_patPicture=null;//Something went wrong retrieving the image.  Default to "Patient Picture Unavailable".
			}
		}

		public void RefreshView() {
			Image=_patPicture;
			HasBorder=true;
			TextNullImage="Patient Picture Unavailable";
		}
	}
}
