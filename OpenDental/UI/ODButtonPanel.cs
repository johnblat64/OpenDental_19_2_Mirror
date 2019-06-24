using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental.UI {
	///<summary></summary>
	public delegate void ODButtonPanelEventHandler(object sender,ODButtonPanelEventArgs e);

	///<summary>Allows for a button panel that is customizable and generated from data in the DB.</summary>
	public partial class ODButtonPanel:UserControl {
		private int _rowHeight=18;
		private float _fontSize=8.5f;
		private Point _pointMouseClick;
		public List<ODPanelItem> Items;
		public bool IsUpdating;
		///<summary>The last item that was clicked on. Can be null.</summary>
		public ODPanelItem SelectedItem;
		///<summary>The last row that was clicked on. -1 if no row selected.</summary>
		public int SelectedRow; 

		#region EventHandlers
		///<summary></summary>
		[Category("Action"),Description("Occurs when an item is single clicked.")]
		public event ODButtonPanelEventHandler ItemClick=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when a button item is single clicked.")]
		public event ODButtonPanelEventHandler ItemClickBut=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when a label item is single clicked.")]
		public event ODButtonPanelEventHandler ItemClickLabel=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when a row is single clicked.")]
		public event ODButtonPanelEventHandler RowClick=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when an item is double clicked.")]
		public event ODButtonPanelEventHandler ItemDoubleClick=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when a button item is double clicked.")]
		public event ODButtonPanelEventHandler ItemDoubleClickBut=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when a label item is double clicked.")]
		public event ODButtonPanelEventHandler ItemDoubleClickLabel=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when a row is double clicked.")]
		public event ODButtonPanelEventHandler RowDoubleClick=null;
		#endregion

		public ODButtonPanel() {
			InitializeComponent();
			Items=new List<ODPanelItem>();
			SelectedRow=-1;
			//might need parent form to doubel buffer.
			DoubleBuffered=true;
		}

		///<summary></summary>
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
		}

		///<summary></summary>
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			Invalidate();
		}

		#region Computations
		///<summary>Computes the position of each column and the overall width.  Called from endUpdate and also from OnPaint.</summary>
		private void ComputeWidthsAndLocations(Graphics g) {
			Items.Sort(ODPanelItem.SortYX);
			for(int i=0;i<Items.Count;i++) {
				Items[i].CalculateWidth(g,_fontSize);
				Items[i].Location.Y=Items[i].YPos*_rowHeight;
				//Then set xPos using previous cell's position and width:
				if(i>0 && Items[i].YPos==Items[i-1].YPos) {//Previous item was on the same row. First item on each row is 0
					Items[i].Location.X=Items[i-1].Location.X+Items[i-1].ItemWidth;
				}
			}
		}

		///<summary>Returns the panel item clicked on. Returns null if no item found.</summary>
		public ODPanelItem PointToItem(Point loc) {
			ODPanelItem retVal=null;
			retVal=Items.Find(item =>
				item.YPos==(loc.Y/_rowHeight) //item is on the clicked row
				&& item.Location.X<loc.X //point is to to the right of the left side
				&& item.Location.X+item.ItemWidth>loc.X); //point is the the left of the right side
			return retVal;
		}
		#endregion Painting

		#region Painting
		///<summary></summary>
		protected override void OnPaintBackground(PaintEventArgs pea) {
			//base.OnPaintBackground (pea);
			//don't paint background.  This reduces flickering.
		}

		///<summary>Runs any time the control is invalidated.</summary>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			if(IsUpdating) {
				return;
			}
			if(Width<1 || Height<1) {
				return;
			}
			try {
				ComputeWidthsAndLocations(e.Graphics);
				DrawBackG(e.Graphics);
				DrawItems(e.Graphics);
				DrawOutline(e.Graphics);
			}
			catch(Exception ex) {
				//We had one customer who was receiving overflow exceptions because the ClientRetangle provided by the system was invalid,
				//due to a graphics device hardware state change when loading the Dexis client application via our Dexis bridge.
				//If we receive an invalid ClientRectangle, then we will simply not draw the button for a frame or two until the system has initialized.
				//A couple of frames later the system should return to normal operation and we will be able to draw the button again.
				ex.DoNothing();
			}
		}

		///<summary>Draws a solid gray background.</summary>
		private void DrawBackG(Graphics g) {
			g.FillRectangle(ODColorTheme.ButtonPanelBackgroundShadowBrush,
				0,0,
				Width,Height);//Creates a shadow on top and left of control.
			g.FillRectangle(ODColorTheme.ButtonPanelBackgroundBrush,
				1,1,
				Width-1,Height-1);
		}

		private void DrawItems(Graphics g) {
			for(int i=0;i<Items.Count;i++) {
				switch(Items[i].ItemType) {
					case ODPanelItemType.Button:
						DrawItemBut(g,Items[i]);
						break;
					case ODPanelItemType.Label:
						DrawItemLabel(g,Items[i]);
						break;
				}
			}
		}

		private void DrawItemLabel(Graphics g,ODPanelItem item) {
			RectangleF itemRect=new RectangleF(
				item.Location.X,item.Location.Y,
				item.ItemWidth,_rowHeight);
			g.FillRectangle(ODColorTheme.ButtonPanelLabelBackgroundBrush,itemRect);
			g.DrawString(item.Text,Font,ODColorTheme.ButtonPanelLabelBrush,itemRect,
				new StringFormat { Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center });
		}

		private void DrawItemBut(Graphics g,ODPanelItem item) {
			//Copied and pasted from OpenDental.UI.Button
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer,true);
			Rectangle recOutline=new Rectangle(item.Location.X,item.Location.Y,item.ItemWidth-1,_rowHeight-1);
			float radius=4;
			DrawItemButBackground(g,recOutline,radius);
			float diagonalLength=(float)Math.Sqrt(recOutline.Height*recOutline.Height+recOutline.Width*recOutline.Width);
			GraphicsHelper.DrawRoundedRectangle(g,ODColorTheme.ButBorderPen,recOutline,radius);
			DrawItemButTextAndImage(g,item);
			GraphicsHelper.DrawReflection(g,recOutline,radius);
		}

			#region Button Drawing Helper Functions

			private void DrawItemButBackground(Graphics g,RectangleF rect,float radius) {
				if(radius<0) {
					radius=0;
				}
				g.SmoothingMode=SmoothingMode.HighQuality;
				//sin(45)=.85. But experimentally, .7 works much better.
				//1/.85=1.18 But experimentally, 1.37 works better. What gives?
				//top
				g.FillRectangle(ODColorTheme.ButMainBrush,rect.Left+radius,rect.Top,rect.Width-(radius*2),radius);
				//UR
				//2 pies of 45 each.
				g.FillPie(ODColorTheme.ButMainBrush,rect.Right-(radius*2),rect.Top,radius*2,radius*2,270,45);
				LinearGradientBrush brush=new LinearGradientBrush(new PointF(rect.Right-(radius/2f)-.5f,rect.Top+(radius/2f)-.5f),
					new PointF(rect.Right,rect.Top+radius),
					ODColorTheme.ButMainBrush.Color,ODColorTheme.ButDarkestBrush.Color);
				g.FillPie(brush,rect.Right-(radius*2),rect.Top,radius*2,radius*2,315,45);
				brush.Dispose();
				//right
				brush=new LinearGradientBrush(new PointF(rect.Right-radius,rect.Top+radius),
					new PointF(rect.Right,rect.Top+radius),
					ODColorTheme.ButMainBrush.Color,ODColorTheme.ButDarkestBrush.Color);
				g.FillRectangle(brush,rect.Right-radius,rect.Top+radius-.5f,radius,rect.Height-(radius*2)+1f);
				brush.Dispose();
				//LR
				g.FillPie(ODColorTheme.ButDarkestBrush,rect.Right-(radius*2),rect.Bottom-(radius*2),radius*2,radius*2,0,90);
				brush=new LinearGradientBrush(new PointF(rect.Right-radius,rect.Bottom-radius),
					new PointF(rect.Right-(radius*.5f)+.5f,rect.Bottom-(radius*.5f)+.5f),
					ODColorTheme.ButMainBrush.Color,ODColorTheme.ButDarkestBrush.Color);
				g.FillPolygon(brush,new PointF[] {
					new PointF(rect.Right-radius,rect.Bottom-radius),
					new PointF(rect.Right,rect.Bottom-radius),
					new PointF(rect.Right-radius,rect.Bottom)});
				brush.Dispose();
				//bottom
				brush=new LinearGradientBrush(new PointF(rect.Left+radius,rect.Bottom-radius),
					new PointF(rect.Left+radius,rect.Bottom),
					ODColorTheme.ButMainBrush.Color,ODColorTheme.ButDarkestBrush.Color);
				g.FillRectangle(brush,rect.Left+radius-.5f,rect.Bottom-radius,rect.Width-(radius*2)+1f,radius);
				brush.Dispose();
				//LL
				//2 pies of 45 each.
				brush=new LinearGradientBrush(new PointF(rect.Left+(radius/2f),rect.Bottom-(radius/2f)),
					new PointF(rect.Left+radius,rect.Bottom),
					ODColorTheme.ButMainBrush.Color,ODColorTheme.ButDarkestBrush.Color);
				g.FillPie(brush,rect.Left,rect.Bottom-(radius*2),radius*2,radius*2,90,45);
				brush.Dispose();
				g.FillPie(ODColorTheme.ButMainBrush,rect.Left,rect.Bottom-(radius*2),radius*2,radius*2,135,45);
				//left
				g.FillRectangle(ODColorTheme.ButMainBrush,rect.Left,rect.Top+radius,radius,rect.Height-(radius*2));
				//UL
				g.FillPie(
					ODColorTheme.ButMainBrush,rect.Left,rect.Top,radius*2,radius*2,180,90);
				//center
				GraphicsPath path=new GraphicsPath();
				path.AddEllipse(rect.Left-rect.Width/8f,rect.Top-rect.Height/2f,rect.Width,rect.Height*3f/2f);
				PathGradientBrush pathBrush=new PathGradientBrush(path);
				pathBrush.CenterColor=ODColorTheme.ButCenterColor;
				pathBrush.SurroundColors=new Color[] { Color.FromArgb(0,255,255,255) };
				g.FillRectangle(ODColorTheme.ButMainBrush,
					rect.Left+radius-.5f,rect.Top+radius-.5f,
					rect.Width-(radius*2)+1f,rect.Height-(radius*2)+1f);
				g.FillRectangle(
					pathBrush,
					rect.Left+radius-.5f,rect.Top+radius-.5f,
					rect.Width-(radius*2)+1f,rect.Height-(radius*2)+1f);
				pathBrush.Dispose();
				//highlights
				brush=new LinearGradientBrush(new PointF(rect.Left+radius,rect.Top),
					new PointF(rect.Left+radius+rect.Width*2f/3f,rect.Top),
					ODColorTheme.ButLightestBrush.Color,ODColorTheme.ButMainBrush.Color);
				g.FillRectangle(brush,rect.Left+radius,rect.Y+radius*3f/8f,rect.Width/2f,radius/4f);
				brush.Dispose();
				path=new GraphicsPath();
				path.AddLine(rect.Left+radius,rect.Top+radius*3/8,rect.Left+radius,rect.Top+radius*5/8);
				path.AddArc(new RectangleF(rect.Left+radius*5/8,rect.Top+radius*5/8,radius*3/4,radius*3/4),270,-90);
				path.AddArc(new RectangleF(rect.Left+radius*3/8,rect.Top+radius*7/8,radius*1/4,radius*1/4),0,180);
				path.AddArc(new RectangleF(rect.Left+radius*3/8,rect.Top+radius*3/8,radius*5/4,radius*5/4),180,90);
				//g.DrawPath(Pens.Red,path);
				g.FillPath(ODColorTheme.ButLightestBrush,path);
			}

			///<summary>Draws the text and image</summary>
			private void DrawItemButTextAndImage(Graphics g,ODPanelItem item) {
				g.SmoothingMode=SmoothingMode.HighQuality;
				StringFormat sf=DrawItemButGetStringFormat(ContentAlignment.MiddleCenter);
				RectangleF recGlow1;
				RectangleF recText;
				recGlow1=new RectangleF(item.Location.X+.5f,item.Location.Y+.5f,item.ItemWidth,_rowHeight);
				recText=new RectangleF(item.Location.X,item.Location.Y,item.ItemWidth,_rowHeight);
				g.DrawString(item.Text,this.Font,ODColorTheme.ButGlowBrush,recGlow1,sf);
				using(SolidBrush brushText=new SolidBrush(ForeColor)) {
					g.DrawString(item.Text,this.Font,brushText,recText,sf);
				}
				sf.Dispose();
			}

			private StringFormat DrawItemButGetStringFormat(ContentAlignment contentAlignment) {
				if(!Enum.IsDefined(typeof(ContentAlignment),(int)contentAlignment))
					throw new System.ComponentModel.InvalidEnumArgumentException(
						"contentAlignment",(int)contentAlignment,typeof(ContentAlignment));
				StringFormat stringFormat = new StringFormat();
				switch(contentAlignment) {
					case ContentAlignment.MiddleCenter:
						stringFormat.LineAlignment = StringAlignment.Center;
						stringFormat.Alignment = StringAlignment.Center;
						break;
					case ContentAlignment.MiddleLeft:
						stringFormat.LineAlignment = StringAlignment.Center;
						stringFormat.Alignment = StringAlignment.Near;
						break;
					case ContentAlignment.MiddleRight:
						stringFormat.LineAlignment = StringAlignment.Center;
						stringFormat.Alignment = StringAlignment.Far;
						break;
					case ContentAlignment.TopCenter:
						stringFormat.LineAlignment = StringAlignment.Near;
						stringFormat.Alignment = StringAlignment.Center;
						break;
					case ContentAlignment.TopLeft:
						stringFormat.LineAlignment = StringAlignment.Near;
						stringFormat.Alignment = StringAlignment.Near;
						break;
					case ContentAlignment.TopRight:
						stringFormat.LineAlignment = StringAlignment.Near;
						stringFormat.Alignment = StringAlignment.Far;
						break;
					case ContentAlignment.BottomCenter:
						stringFormat.LineAlignment = StringAlignment.Far;
						stringFormat.Alignment = StringAlignment.Center;
						break;
					case ContentAlignment.BottomLeft:
						stringFormat.LineAlignment = StringAlignment.Far;
						stringFormat.Alignment = StringAlignment.Near;
						break;
					case ContentAlignment.BottomRight:
						stringFormat.LineAlignment = StringAlignment.Far;
						stringFormat.Alignment = StringAlignment.Far;
						break;
				}
				return stringFormat;
			}
			#endregion Button Drawing Helper Functions

		///<summary>Draws outline around entire control.</summary>
		private void DrawOutline(Graphics g) {
			g.DrawRectangle(ODColorTheme.ButtonPanelOutlinePen,0,0,Width-1,Height-1);
		}
		#endregion

		#region Clicking

		#region Single Click
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			SelectedItem=PointToItem(_pointMouseClick);
			SelectedRow=_pointMouseClick.Y/_rowHeight;
			if(SelectedItem!=null) {
				//OnClickItem(SelectedItem);
				switch(SelectedItem.ItemType) {
					case ODPanelItemType.Button:
						OnClickButton(SelectedItem);
						break;
					//case ODPanelItemType.Label:
					//	OnClickLabel(SelectedItem);
					//	break;
				}
			}
			//OnRowClick(SelectedRow);
		}

		private void OnClickButton(ODPanelItem itemClick) {
			ODButtonPanelEventArgs pArgs=new ODButtonPanelEventArgs(SelectedItem,SelectedRow,MouseButtons.Left);
			if(ItemClickBut!=null) {
				ItemClickBut(this,pArgs);
			}
		}
		#endregion Single Click

		#region Double Click
		///<summary></summary>
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			SelectedItem=PointToItem(_pointMouseClick);
			SelectedRow=_pointMouseClick.Y/_rowHeight;
			//if(SelectedItem!=null) {
			//	OnDoubleClickItem(SelectedItem);
			//	switch(SelectedItem.ItemType) {
			//		case ODPanelItemType.Button:
			//			OnDoubleClickButton(SelectedItem);
			//			break;
			//		case ODPanelItemType.Label:
			//			OnDoubleClickLabel(SelectedItem);
			//			break;
			//	}
			//}
			OnRowDoubleClick(_pointMouseClick.Y/_rowHeight);
		}

		private void OnRowDoubleClick(int p) {
			ODButtonPanelEventArgs pArgs=new ODButtonPanelEventArgs(SelectedItem,SelectedRow,MouseButtons.Left);
			if(RowDoubleClick!=null) {
				RowDoubleClick(this,pArgs);
			}
		}
		#endregion Double Click

		#endregion Clicking

		#region MouseEvents

		///<summary></summary>
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			_pointMouseClick=new Point(e.X,e.Y);
			//ODPanelItem item=PointToItem(_pointMouseClick);
			//int row=e.Y/_rowHeight;
		}

		#endregion MouseEvents

		#region BeginEndUpdate
		///<summary>Call this before adding any rows.  You would typically call Rows.Clear after this.</summary>
		public void BeginUpdate() {
			IsUpdating=true;
		}

		///<summary>Must be called after adding rows.  This computes the columns, computes the rows, lays out the scrollbars, clears SelectedIndices, and invalidates.</summary>
		public void EndUpdate() {
			using(Graphics g=this.CreateGraphics()) {
				ComputeWidthsAndLocations(g);
			}
			IsUpdating=false;
			Invalidate();
		}
		#endregion BeginEndUpdate

	}

	///<summary></summary>
	public class ODButtonPanelEventArgs {
		private ODPanelItem item;
		private int row;
		private MouseButtons button;

		///<summary></summary>
		public ODButtonPanelEventArgs(ODPanelItem item,int row,MouseButtons button) {
			this.item=item;
			this.row=row;
			this.button=button;
		}

		///<summary>Can be null.</summary>
		public ODPanelItem Item {
			get {
				return item;
			}
		}

		///<summary></summary>
		public int Row {
			get {
				return row;
			}
		}

		///<summary>Gets which mouse button was pressed.</summary>
		public MouseButtons Button {
			get {
				return button;
			}
		}

	}

}
