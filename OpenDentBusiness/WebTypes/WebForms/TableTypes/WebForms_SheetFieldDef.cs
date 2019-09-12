using System.Drawing;
using System.Xml.Serialization;
using OpenDentBusiness;
using System.Windows.Forms;
//using OpenDentalWebCore;
using System;

namespace OpenDentBusiness.WebTypes.WebForms {
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,HasBatchWriteMethods=true,CrudLocationOverride=@"..\..\..\OpenDentBusiness\WebTypes\WebForms\Crud",
		NamespaceOverride="OpenDentBusiness.WebTypes.WebForms.Crud",CrudExcludePrefC=true)]
	public class WebForms_SheetFieldDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long WebSheetFieldDefID;
		///<summary>FK to webforms_sheetdef.WebSheetDefID</summary>
		public long WebSheetDefID;
		///<summary></summary>
		public SheetFieldType FieldType;
		///<summary></summary>
		public string FieldName;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string FieldValue;
		///<summary></summary>
		public float FontSize;
		///<summary></summary>
		public string FontName;
		///<summary></summary>
		public bool FontIsBold;
		///<summary></summary>
		public int XPos;
		///<summary></summary>
		public int YPos;
		///<summary></summary>
		public int Width;
		///<summary></summary>
		public int Height;
		///<summary></summary>
		public GrowthBehaviorEnum GrowthBehavior;
		///<summary></summary>
		public string RadioButtonValue;
		///<summary></summary>
		public string RadioButtonGroup;
		///<summary></summary>
		public bool IsRequired;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ImageData;
		///<summary></summary>
		public int TabOrder;
		///<summary></summary>
		public string ReportableName;
		///<summary></summary>
		public HorizontalAlignment TextAlign;
		///<summary></summary>
		[XmlIgnore]
		public Color ItemColor;
		///<summary>Tab stop order for all fields of a mobile sheet. One-based.  Only mobile fields can have values other than 0.
		///If all SheetFieldDefs for a given SheetField are 0 then assume that this sheet has no mobile-specific view.</summary>
		public int TabOrderMobile;
		///<summary>Each input field for a mobile will need a corresponding UI label. This is what the user sees as the label describing what this input is for. EG "First Name:, Last Name:, Address, etc."</summary>
		public string UiLabelMobile;
		///<summary>Human readable label that will be displayed for radio button item in mobile mode. 
		///Cannot use UiLabelMobile for this purpose as it is already dedicated to the radio group header that groups radio button items together.</summary>
		public string UiLabelMobileRadioButton;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ItemColor",typeof(int))]
		public int ColorOverrideXml {
			get {
				return ItemColor.ToArgb();
			}
			set {
				ItemColor=Color.FromArgb(value);
			}
		}
		
    public WebForms_SheetFieldDef(){

		}
		
    public WebForms_SheetFieldDef Copy(){
			return (WebForms_SheetFieldDef)this.MemberwiseClone();
		}
	}
}