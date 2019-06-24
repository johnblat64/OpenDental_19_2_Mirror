using System;
using OpenDentBusiness;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace OpenDentBusiness.WebTypes.WebForms {
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudLocationOverride=@"..\..\..\OpenDentBusiness\WebTypes\WebForms\Crud",NamespaceOverride="OpenDentBusiness.WebTypes.WebForms.Crud",CrudExcludePrefC=true)]
	public class WebForms_Sheet:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SheetID;
		///<summary></summary>
		public long DentalOfficeID;
		///<summary></summary>
		public string Description;
		///<summary></summary>
		public SheetFieldType SheetType;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeSheet;
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
		///<summary></summary>
		public long ClinicNum;
		///<Summary></Summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public List<WebForms_SheetField> SheetFields;
		///<summary>If true then this Sheet has been designed for mobile and will be displayed as a mobile-friendly WebForm.</summary>
		public bool HasMobileLayout;

		public WebForms_Sheet(){

		}
		
    public WebForms_Sheet Copy(){
			return (WebForms_Sheet)this.MemberwiseClone();
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("SheetFields",typeof(WebForms_SheetField[]))]
		public WebForms_SheetField[] SheetFieldsXml {
			get {
				if(SheetFields==null) {
					return new WebForms_SheetField[0];
				}
				return SheetFields.ToArray();
			}
			set {
				SheetFields=new List<WebForms_SheetField>();
				for(int i=0;i<value.Length;i++) {
					SheetFields.Add(value[i]);
				}
			}
		}
	}
}