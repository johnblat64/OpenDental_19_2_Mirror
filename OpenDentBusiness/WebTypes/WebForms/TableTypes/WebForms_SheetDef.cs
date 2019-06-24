using System;
using OpenDentBusiness;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace OpenDentBusiness.WebTypes.WebForms {
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudLocationOverride=@"..\..\..\OpenDentBusiness\WebTypes\WebForms\Crud",NamespaceOverride="OpenDentBusiness.WebTypes.WebForms.Crud",CrudExcludePrefC=true)]
	public class WebForms_SheetDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long WebSheetDefID;
		///<summary>FK to customers.patient.PatNum.</summary>
		public long DentalOfficeID;
		///<summary></summary>
		public string Description;
		///<summary></summary>
		public SheetFieldType SheetType;
		///<summary></summary>
		public float FontSize;
		///<summary></summary>
		public string FontName;
		///<summary></summary>
		public int Width;
		///<summary></summary>
		public int Height;
		///<summary></summary>
		public bool IsLandscape;
		///<summary>FK to sheetdef.SheetDefNum</summary>
		public long SheetDefNum;
		///<Summary></Summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public List<WebForms_SheetFieldDef> SheetFieldDefs;
		///<summary>If true then this Sheet has been designed for mobile and will be displayed as a mobile-friendly WebForm.</summary>
		public bool HasMobileLayout;

		public WebForms_SheetDef(){

		}
		
    public WebForms_SheetDef Copy(){
			return (WebForms_SheetDef)this.MemberwiseClone();
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("SheetFieldDefs",typeof(WebForms_SheetFieldDef[]))]
		public WebForms_SheetFieldDef[] SheetFieldDefsXml {
			get {
				if(SheetFieldDefs==null) {
					return new WebForms_SheetFieldDef[0];
				}
				return SheetFieldDefs.ToArray();
			}
			set {
				SheetFieldDefs=new List<WebForms_SheetFieldDef>();
				for(int i=0;i<value.Length;i++) {
					SheetFieldDefs.Add(value[i]);
				}
			}
		}
	}
}