using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI{
	//Jordan is the only one who should edit this file.
	///<summary>For some internal combo boxes, this is the part that comes up as the "list" to pick from.  It's a Form in order to allow more powerful and longer lists that are larger than the containing form.  It can handle thousands of entries instead of just 100.</summary>
	public partial class FormComboPicker : Form{
		#region Fields - Private
		private List<string> _listStrings;
		private bool _isMultiSelect=false;
		private Point _pointInitialUR;
		///<summary>This is the height of the dummy combobox at the top: 21</summary>
		private int _heightCombo=21;
		///<summary>True if we're still at _pointInitialUR.  False, if we had to shift because we hit the bottom of the screen, etc.  Then, we don't want to draw the "combobox" at the top.</summary>
		private bool _isOriginalLocation;
		#endregion Fields - Private

		#region Constructor
		public FormComboPicker(){
			InitializeComponent();
		}
		#endregion Constructor

		#region Events
		private void FormComboPicker_Deactivate(object sender, EventArgs e){
			//user clicked outside this dropdown "form"
			Close();
		}

		private void FormComboPicker_KeyDown(object sender, KeyEventArgs e){
			//Enter was doing nothing by default.
			if(e.KeyCode!=Keys.Enter){
				return;
			}
			Close();
		}

		///<summary></summary>
		private void FormComboPicker_Load(object sender, EventArgs e){
			if(ListStrings.Count==0){
				//DialogResult=DialogResult.OK;
				Close();
				return;
			}
			//before form is shown, so shouldn't flicker
			Rectangle rectScreenBounds=Screen.GetWorkingArea(this);
			listBoxMain.Top=_heightCombo+1;
			//listBoxMain ItemHeight=13, height=4+(13*items)
			int maxItems=(rectScreenBounds.Height/13);//-_heightCombo-2)/13;//auto rounds down. Combobox won't show
			if(ListStrings.Count>maxItems){//more items than will fit on the entire vertical screen
				//not using integral height so that, at max height, we can use entire space.
				this.Height=rectScreenBounds.Height;
				this.Top=rectScreenBounds.Height-this.Height;
				listBoxMain.Top=1;
				listBoxMain.Height=Height-2;//-listBoxMain.Top-1;//scroll should show, but that's still untested.
				_isOriginalLocation=false;
			}
			else{
				listBoxMain.Height=ListStrings.Count*13+4;
				if(PointInitialUR.Y+listBoxMain.Bottom+1>rectScreenBounds.Height){
					//bump it up
					this.Height=listBoxMain.Height+2;
					this.Top=rectScreenBounds.Height-this.Height;
					listBoxMain.Top=1;
					_isOriginalLocation=false;
				}
				else{
					this.Top=PointInitialUR.Y;
					this.Height=listBoxMain.Bottom+1;
					_isOriginalLocation=true;
				}
			}
			//listbox will be one pixel smaller on all 4 sides in order to show extra blue border.
			listBoxMain.Left=1;
			listBoxMain.Width=this.Width-listBoxMain.Left-2;
			this.Left=PointInitialUR.X-this.Width;
		}

		private void FormComboPicker_MouseDown(object sender, MouseEventArgs e){
			//this doesn't fire if clicking on the listbox; only if clicking above it.
			//DialogResult=DialogResult.OK;
			Close();
		}
		
		///<summary></summary>
		private void FormComboPicker_Paint(object sender, PaintEventArgs e){
			//the top portion is painted to look exactly like the combobox and down arrow that are underneath.
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			SolidBrush solidBrushBack=new SolidBrush(Color.FromArgb(229,241,251));//light blue helps us see that there is a dropdown
			Pen penArrow=new Pen(Color.FromArgb(20,20,20),1.5f);
			Pen penBlueOutline=new Pen(Color.FromArgb(0,120,215));
			//But we only want to draw that if this window is still at its default location.
			if(_isOriginalLocation){
				Rectangle rectangleCombo=new Rectangle();
				rectangleCombo.X=0;
				rectangleCombo.Y=0;
				rectangleCombo.Width=this.Width-rectangleCombo.X-1;
				rectangleCombo.Height=21-1;//the minus one is so it's not touching the edge of the control and hiding the drawing
				g.FillRectangle(solidBrushBack,rectangleCombo);
				g.DrawRectangle(penBlueOutline,rectangleCombo);//.X,rectangleCombo.Y,rectangleCombo);
				//the down arrow, starting at the left
				g.DrawLine(penArrow,Width-13,9,Width-9.5f,12);
				g.DrawLine(penArrow,Width-9.5f,12,Width-6,9);
				RectangleF rectangleFString=new RectangleF();
				rectangleFString.X=rectangleCombo.X+2;
				rectangleFString.Y=rectangleCombo.Y+4;
				rectangleFString.Width=rectangleCombo.Width-2;
				rectangleFString.Height=rectangleCombo.Height-4;
				int widthMax=rectangleCombo.Width-15;
				g.DrawString(GetDisplayText(widthMax),this.Font,Brushes.Black,rectangleFString);
			}
			//draw blue rectangle around listbox, which doesn't have its own rectangle
			Rectangle rectangleOutline=new Rectangle();
			rectangleOutline.X=0;
			rectangleOutline.Y=listBoxMain.Top-1;
			rectangleOutline.Width=this.Width-1;
			rectangleOutline.Height=listBoxMain.Height+2;
			g.DrawRectangle(penBlueOutline,rectangleOutline);
			solidBrushBack.Dispose();
			penArrow.Dispose();
			penBlueOutline.Dispose();
		}

		private void ListBoxMain_Click(object sender, EventArgs e){
			if(!IsMultiSelect){
				//DialogResult=DialogResult.OK;
				Close();
			}
			Invalidate();
		}
		#endregion Events

		#region Properties
		///<summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsMultiSelect{
			get{
				return _isMultiSelect;
			}
			set{
				if(_isMultiSelect==value){
					return;
				}
				_isMultiSelect=value;
				if(_isMultiSelect){
					listBoxMain.SelectionMode=SelectionMode.MultiExtended;
				}
				else{
					listBoxMain.SelectionMode=SelectionMode.One;
				}
			}
		}

		///<summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<string> ListStrings{
			get{
				return _listStrings;//used internally
			}
			set{
				_listStrings=value;
				listBoxMain.Items.Clear();
				foreach(string str in _listStrings){
					listBoxMain.Items.Add(str);
				}
			}
		}

		[Category("OD")]
		[Description("The initial point where the UR corner of this picker window should start, in Screen coordinate.  It might grow up from here if it runs out of room below. It might also rarely need to expand right.")]
		[DefaultValue(false)]
		public Point PointInitialUR{
			get{
				return _pointInitialUR;
			}
			set{
				_pointInitialUR=value;
			}
		}

		///<summary>Only used when IsMultiSelect=false;</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex{
			get{
				return listBoxMain.SelectedIndex;
			}
			set{
				if(listBoxMain.SelectedIndex==value){
					return;
				}
				listBoxMain.SelectedIndex=value;
			}
		}

		///<summary>Only used when IsMultiSelect=true;</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<int> SelectedIndices{
			get{
				return listBoxMain.SelectedIndices.Cast<int>().ToList();
			}
			set{
				listBoxMain.SelectedIndices.Clear();
				for(int i=0;i<value.Count;i++){
					listBoxMain.SetSelected(value[i],true);
				}
			}
		}
		#endregion Properties

		#region Methods - Private
		///<summary>If multiple items are selected, we string them together with commas.  But if the string is wider than widthMax, we instead show "Multiple".</summary>
		private string GetDisplayText(int widthMax){
			if(listBoxMain.SelectedIndices.Count==0){
				return "";
			}
			if(listBoxMain.SelectedIndices.Contains(0) && _listStrings[0]=="All"){//This isn't a very good test, but it is just display text.  CLINIC_NUM_ALL){
				return "All";
			}
			string str="";
			for(int i=0;i<listBoxMain.SelectedIndices.Count;i++){
				if(i>0){
					str+=",";
				}
				str+=_listStrings[listBoxMain.SelectedIndices[i]];
			}
			if(listBoxMain.SelectedIndices.Count>1){
				if(TextRenderer.MeasureText(str,this.Font).Width>widthMax){
					return "Multiple";
				}
			}
			return str;
		}
		#endregion Methods - Private
		
		#region Events - Closing
		private void ButOK_Click(object sender, EventArgs e){
			Close();
		}





		#endregion Events - Closing

		
	}
}
