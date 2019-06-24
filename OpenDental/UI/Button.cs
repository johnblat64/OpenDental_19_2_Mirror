using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using CodeBase;

namespace OpenDental.UI {
	///<summary>A custom button with rounded corners and theme support.</summary>
	public class Button : System.Windows.Forms.Button {
		private ControlState _butState=ControlState.Normal;
		private bool _bCanClick=false;
		private Point _adjustImageLocation;
		private bool _isAutoSize=true;
		///<summary>Gets overridden depending on the theme.</summary>
		private float _cornerRadius=4;

		///<summary>Initializes a new instance of the Button class.</summary>
		public Button() {
			//This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
		}

		#region Component Designer generated code
		///<summary>Required method for Designer support - do not modify the contents of this method with the code editor.</summary>
		private void InitializeComponent() {
			this.SuspendLayout();
			this.ResumeLayout(false);
		}
		#endregion

		#region Properties
		///<summary>Getter will always return Rectangle and the setter is not implemented.  Just for compatibility</summary>
		public enumType.BtnShape BtnShape {
			get {
				return enumType.BtnShape.Rectangle;
			}
			set {
			}
		}

		///<summary>Getter will always return Silver and the setter is not implemented.  Just for compatibility</summary>
		public enumType.XPStyle BtnStyle {
			get {
				return enumType.XPStyle.Silver;
			}
			set {
			}
		}

		///<summary>Getter will return the base FlatStyle and the setter will force the style to Standard.</summary>
		public new FlatStyle FlatStyle {
			get {
				return base.FlatStyle;
			}
			set {//if user tries to change, they can't
				base.FlatStyle=FlatStyle.Standard;
			}
		}

		///<summary>Set to true if this button should automatically resize depending on the text displayed.</summary>
		public bool Autosize {
			get {
				return _isAutoSize;
			}
			set {
				_isAutoSize=value;
			}
		}

		///<summary></summary>
		public Point AdjustImageLocation {
			get {
				return _adjustImageLocation;
			}
			set { 
				_adjustImageLocation=value;
				this.Invalidate();
			}
		}

		///<summary>Typically 4. This property is rarely used and will get overridden for themes other than Original and Blue.</summary>
		///Now we have a bunch of lines in designer files all over, and changing now would change a bunch of files.
		public float CornerRadius {
			get {
				return _cornerRadius;
			}
			set {
				_cornerRadius=value;
				this.Invalidate();
			}
		}
		#endregion Properties

		#region Overridden Event Handlers
		///<summary></summary>
		protected override void OnClick(EventArgs ea) {
			this.Capture=false;
			_bCanClick=false;
			if(this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition))) {
				_butState=ControlState.Hover;
			}
			else {
				_butState=ControlState.Normal;
			}
			this.Invalidate();
			base.OnClick(ea);
		}

		///<summary></summary>
		protected override void OnMouseEnter(EventArgs ea) {
			base.OnMouseEnter(ea);
			_butState=ControlState.Hover;
			this.Invalidate();
		}

		///<summary></summary>
		protected override void OnMouseDown(MouseEventArgs mea) {
			base.OnMouseDown(mea);
			if(mea.Button==MouseButtons.Left) {
				_bCanClick=true;
				_butState=ControlState.Pressed;
				this.Invalidate();
			}
		}

		///<summary></summary>
		protected override void OnMouseMove(MouseEventArgs mea) {
			base.OnMouseMove(mea);
			if(ClientRectangle.Contains(mea.X, mea.Y)) {
				if(_butState==ControlState.Hover && this.Capture && !_bCanClick) {
					_bCanClick=true;
					_butState=ControlState.Pressed;
					this.Invalidate();
				}
			}
			else {
				if(_butState==ControlState.Pressed) {
					_bCanClick=false;
					_butState=ControlState.Hover;
					this.Invalidate();
				}
			}
		}

		///<summary></summary>
		protected override void OnMouseLeave(EventArgs ea) {
			base.OnMouseLeave(ea);
			_butState=ControlState.Normal;
			this.Invalidate();
		}

		///<summary></summary>
		protected override void OnEnabledChanged(EventArgs ea) {
			base.OnEnabledChanged(ea);
			_butState=ControlState.Normal;
			this.Invalidate();
		}

		///<summary></summary>
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			try {
				if(_isAutoSize&&Text!=""&&!IsDisposed) {
					int buffer=6;
					int textWidth=0;
					using(Graphics g = this.CreateGraphics()) {
						textWidth=(int)g.MeasureString(Text,Font).Width;
					}
					int oldWidth=Width;
					if(this.Image==null) {
						if(Width<textWidth+buffer) {
							Width=textWidth+buffer;
						}
					}
					else {
						if(Width<textWidth+Image.Size.Width+buffer) {
							Width=textWidth+Image.Size.Width+buffer;
						}
					}
					if((Anchor&AnchorStyles.Right)==AnchorStyles.Right) {
						//to be perfect, it would exclude if anchored left also
						//this works even if no change in width
						Left+=oldWidth-Width;
					}
				}
			}
			catch(Exception ex) {
				//This method may fail if window handle has not been created yet. 
				//Can happen if invoked from a separate thread due to a race condition.
				//Nothing we can do about it and not worth crashing the program so swallow it.
				ex.DoNothing();
			}
		}
		#endregion Overridden Event Handlers

		#region Painting
		///<summary></summary>
		protected override void OnPaint(PaintEventArgs p) {
			this.OnPaintBackground(p);
			try {
				Graphics g=p.Graphics;
				RectangleF recOutline=new RectangleF(0,0,ClientRectangle.Width-1,ClientRectangle.Height-1);
				_cornerRadius=(ODColorTheme.ButCornerRadiusOverride==-1)?_cornerRadius:ODColorTheme.ButCornerRadiusOverride;
				float radius=_cornerRadius;
				#region DrawBackground
				switch(_butState) {
					case ControlState.Normal:
						if(Enabled) {
							radius=_cornerRadius;
							if(Focused || IsDefault) {
								DrawBackground(g,recOutline,radius,ODColorTheme.ButDefaultDarkBrush,ODColorTheme.ButMainBrush,ODColorTheme.ButLightestBrush);
							}
							else {
								DrawBackground(g,recOutline,radius,ODColorTheme.ButDarkestBrush,ODColorTheme.ButMainBrush,ODColorTheme.ButLightestBrush);
							}
						}
						else {
							radius=_cornerRadius;
						}
						break;
					case ControlState.Hover:
						radius=_cornerRadius;
						DrawBackground(g,recOutline,radius,ODColorTheme.ButHoverDarkBrush,ODColorTheme.ButHoverMainBrush,ODColorTheme.ButHoverLightBrush);
						break;
					case ControlState.Pressed:
						if(radius > 3) {
							radius=_cornerRadius-3;
						}
						DrawBackground(g,recOutline,radius,ODColorTheme.ButPressedDarkestBrush,ODColorTheme.ButPressedMainBrush,ODColorTheme.ButPressedLightestBrush);
						break;
				}
				#endregion
				GraphicsHelper.DrawRoundedRectangle(g,ODColorTheme.ButBorderPen,recOutline,radius);
				DrawTextAndImage(g);
				GraphicsHelper.DrawReflection(g,recOutline,radius);
			}
			catch(Exception ex) {
				//We had one customer who was receiving overflow exceptions because the ClientRetangle provided by the system was invalid,
				//due to a graphics device hardware state change when loading the Dexis client application via our Dexis bridge.
				//If we receive an invalid ClientRectangle, then we will simply not draw the button for a frame or two until the system has initialized.
				//A couple of frames later the system should return to normal operation and we will be able to draw the button again.
				ex.DoNothing();
			}
		}

		///<summary>Draws button background for different button states (normal, pressed, hover).</summary>
		private void DrawBackground(Graphics g,RectangleF rect,float radius,SolidBrush darkBrush,SolidBrush mainBrush,SolidBrush lightBrush) {
			if(radius < 0) {
				radius=0;
			}
			LinearGradientBrush brush;
			g.SmoothingMode=SmoothingMode.HighQuality;
			//sin(45)=.85. But experimentally, .7 works much better.
			//1/.85=1.18 But experimentally, 1.37 works better. What gives?
			//top
			g.FillRectangle(mainBrush,rect.Left+radius,rect.Top,rect.Width-(radius*2),radius);
			//UR
			//2 pies of 45 each.
			g.FillPie(mainBrush,rect.Right-(radius*2),rect.Top,radius*2,radius*2,270,45);
			brush=new LinearGradientBrush(new PointF(rect.Right-(radius/2f)-.5f,rect.Top+(radius/2f)-.5f),
				new PointF(rect.Right,rect.Top+radius),
				mainBrush.Color,darkBrush.Color);
			g.FillPie(brush,rect.Right-(radius*2),rect.Top,radius*2,radius*2,315,45);
			brush.Dispose();
			//right
			brush=new LinearGradientBrush(new PointF(rect.Right-radius,rect.Top+radius),
				new PointF(rect.Right,rect.Top+radius),mainBrush.Color,darkBrush.Color);
			g.FillRectangle(brush,rect.Right-radius,rect.Top+radius-.5f,radius,rect.Height-(radius*2)+1f);
			brush.Dispose();
			//LR
			g.FillPie(darkBrush,rect.Right-(radius*2),rect.Bottom-(radius*2),radius*2,radius*2,0,90);
			brush=new LinearGradientBrush(new PointF(rect.Right-radius,rect.Bottom-radius),
				new PointF(rect.Right-(radius*.5f)+.5f,rect.Bottom-(radius*.5f)+.5f),
				mainBrush.Color,darkBrush.Color);
			g.FillPolygon(brush,new PointF[] {
				new PointF(rect.Right-radius,rect.Bottom-radius),
				new PointF(rect.Right,rect.Bottom-radius),
				new PointF(rect.Right-radius,rect.Bottom)});
			brush.Dispose();
			//bottom
			brush=new LinearGradientBrush(new PointF(rect.Left+radius,rect.Bottom-radius),new PointF(rect.Left+radius,rect.Bottom),
				mainBrush.Color,darkBrush.Color);
			g.FillRectangle(brush,rect.Left+radius-.5f,rect.Bottom-radius,rect.Width-(radius*2)+1f,radius);
			brush.Dispose();
			//LL
			//2 pies of 45 each.
			brush=new LinearGradientBrush(new PointF(rect.Left+(radius/2f),rect.Bottom-(radius/2f)),
				new PointF(rect.Left+radius,rect.Bottom),
				mainBrush.Color,darkBrush.Color);
			g.FillPie(brush,rect.Left,rect.Bottom-(radius*2),radius*2,radius*2,90,45);
			brush.Dispose();
			g.FillPie(mainBrush,rect.Left,rect.Bottom-(radius*2),radius*2,radius*2,135,45);
			//left
			g.FillRectangle(mainBrush,rect.Left,rect.Top+radius,radius,rect.Height-(radius*2));
			//UL
			g.FillPie(//new SolidBrush(clrLight)
				mainBrush,rect.Left,rect.Top,radius*2,radius*2,180,90);
			//center
			GraphicsPath path=new GraphicsPath();
			path.AddEllipse(rect.Left-rect.Width/8f,rect.Top-rect.Height/2f,rect.Width,rect.Height*3f/2f);
			PathGradientBrush pathBrush=new PathGradientBrush(path);
			pathBrush.CenterColor=ODColorTheme.ButCenterColor;
			pathBrush.SurroundColors=new Color[] {Color.FromArgb(0,255,255,255)};//Transparent
			g.FillRectangle(mainBrush,rect.Left+radius-.5f,rect.Top+radius-.5f,rect.Width-(radius*2)+1f,rect.Height-(radius*2)+1f);
			g.FillRectangle(pathBrush,rect.Left+radius-.5f,rect.Top+radius-.5f,rect.Width-(radius*2)+1f,rect.Height-(radius*2)+1f);
			//highlights
			brush=new LinearGradientBrush(new PointF(rect.Left+radius,rect.Top),new PointF(rect.Left+radius+rect.Width*2f/3f,rect.Top),
				lightBrush.Color,mainBrush.Color);
			g.FillRectangle(brush,rect.Left+radius,rect.Y+radius*3f/8f,rect.Width/2f,radius/4f);
			brush.Dispose();
			path=new GraphicsPath();
			path.AddLine(rect.Left+radius,rect.Top+radius*3/8,rect.Left+radius,rect.Top+radius*5/8);
			path.AddArc(new RectangleF(rect.Left+radius*5/8,rect.Top+radius*5/8,radius*3/4,radius*3/4),270,-90);
			path.AddArc(new RectangleF(rect.Left+radius*3/8,rect.Top+radius*7/8,radius*1/4,radius*1/4),0,180);
			path.AddArc(new RectangleF(rect.Left+radius*3/8,rect.Top+radius*3/8,radius*5/4,radius*5/4),180,90);
			//g.DrawPath(Pens.Red,path);
			g.FillPath(lightBrush,path);
			path.Dispose();
		}

		///<summary>Draws the text and image</summary>
		private void DrawTextAndImage(Graphics g) {
			g.SmoothingMode=SmoothingMode.HighQuality;
			SolidBrush brushText;
			if(Enabled) {
				brushText=ODColorTheme.ButTextColor;
			}
			else {
				brushText=ODColorTheme.ButDisabledTextBrush;
			}
			StringFormat sf=GetStringFormat(this.TextAlign);
			if(ShowKeyboardCues) {
				sf.HotkeyPrefix=System.Drawing.Text.HotkeyPrefix.Show;
			}
			else {
				sf.HotkeyPrefix=System.Drawing.Text.HotkeyPrefix.Hide;
			}
			RectangleF recGlow1;
			if(this.Image!=null) {
				Rectangle recTxt=new Rectangle();
				Point ImagePoint= new Point(6, 4);
				switch(this.ImageAlign) {
					case ContentAlignment.MiddleLeft:
						ImagePoint.X=6;
						ImagePoint.Y=this.ClientRectangle.Height/2-Image.Height/2;
						recTxt.Width=this.ClientRectangle.Width-this.Image.Width;
						recTxt.Height=this.ClientRectangle.Height;
						recTxt.X=this.Image.Width;
						recTxt.Y=0;
						break;
					case ContentAlignment.MiddleRight:
						recTxt.Width=this.ClientRectangle.Width-this.Image.Width-8;
						recTxt.Height=this.ClientRectangle.Height;
						recTxt.X=0;
						recTxt.Y=0;
						ImagePoint.X=recTxt.Width;
						recTxt.Width+=this._adjustImageLocation.X;
						ImagePoint.Y=this.ClientRectangle.Height/2-Image.Height/2;
						break;
					case ContentAlignment.MiddleCenter:// no text in this alignment
						ImagePoint.X=(this.ClientRectangle.Width-this.Image.Width)/2;
						ImagePoint.Y=(this.ClientRectangle.Height-this.Image.Height)/2;
						recTxt.Width=0;
						recTxt.Height=0;
						recTxt.X=this.ClientRectangle.Width;
						recTxt.Y=this.ClientRectangle.Height;
						break;
				}
				ImagePoint.X+=_adjustImageLocation.X;
				ImagePoint.Y+=_adjustImageLocation.Y;
				if(this.Enabled) {
					g.DrawImage(this.Image,ImagePoint);
				}
				else {
					System.Windows.Forms.ControlPaint.DrawImageDisabled(g,this.Image,ImagePoint.X,ImagePoint.Y,BackColor);
				}
				recGlow1=new RectangleF(recTxt.X+.5f,recTxt.Y+.5f,recTxt.Width,recTxt.Height);
				if(this.ImageAlign!=ContentAlignment.MiddleCenter) {
					if(Enabled) {
						//first draw white text slightly off center
						g.DrawString(this.Text,this.Font,ODColorTheme.ButGlowBrush,recGlow1,sf);
					}
					//then, the black text
					g.DrawString(this.Text,this.Font,brushText,recTxt,sf);
				}
			}
			else {//image is null
				recGlow1=new RectangleF(ClientRectangle.X+.5f,ClientRectangle.Y+.5f,ClientRectangle.Width,ClientRectangle.Height);
				if(this.Enabled) {
					g.DrawString(this.Text,this.Font,ODColorTheme.ButGlowBrush,recGlow1,sf);
				}
				g.DrawString(this.Text,this.Font,brushText,this.ClientRectangle,sf);
			}
			sf.Dispose();
		}
		#endregion Painting

		private StringFormat GetStringFormat(ContentAlignment contentAlignment) {
			if(!Enum.IsDefined(typeof(ContentAlignment),(int)contentAlignment)) {
				throw new InvalidEnumArgumentException("contentAlignment",(int)contentAlignment,typeof(ContentAlignment));
			}
			StringFormat stringFormat=new StringFormat();
			switch(contentAlignment) {
				case ContentAlignment.MiddleCenter:
					stringFormat.LineAlignment=StringAlignment.Center;
					stringFormat.Alignment=StringAlignment.Center;
					break;
				case ContentAlignment.MiddleLeft:
					stringFormat.LineAlignment=StringAlignment.Center;
					stringFormat.Alignment=StringAlignment.Near;
					break;
				case ContentAlignment.MiddleRight:
					stringFormat.LineAlignment=StringAlignment.Center;
					stringFormat.Alignment=StringAlignment.Far;
					break;
				case ContentAlignment.TopCenter:
					stringFormat.LineAlignment=StringAlignment.Near;
					stringFormat.Alignment=StringAlignment.Center;
					break;
				case ContentAlignment.TopLeft:
					stringFormat.LineAlignment=StringAlignment.Near;
					stringFormat.Alignment=StringAlignment.Near;
					break;
				case ContentAlignment.TopRight:
					stringFormat.LineAlignment=StringAlignment.Near;
					stringFormat.Alignment=StringAlignment.Far;
					break;
				case ContentAlignment.BottomCenter:
					stringFormat.LineAlignment=StringAlignment.Far;
					stringFormat.Alignment=StringAlignment.Center;
					break;
				case ContentAlignment.BottomLeft:
					stringFormat.LineAlignment=StringAlignment.Far;
					stringFormat.Alignment=StringAlignment.Near;
					break;
				case ContentAlignment.BottomRight:
					stringFormat.LineAlignment=StringAlignment.Far;
					stringFormat.Alignment=StringAlignment.Far;
					break;
			}
			return stringFormat;
		}

		///<summary></summary>
		public enum ControlState {
			///<summary>button is in the normal state.</summary>
			Normal,
			///<summary>button is in the hover state.</summary>
			Hover,
			///<summary>button is in the pressed state.</summary>
			Pressed,
			///<summary>button is in the default state.</summary>
			Default,
			///<summary>button is in the disabled state.</summary>
			Disabled
		}

	}

	///<summary>Just for backward compatibility.</summary>
	public class enumType {
		///<summary></summary>
		public enum XPStyle {
			///<summary></summary>
			Default,
			///<summary></summary>
			Blue,
			///<summary></summary>
			OliveGreen,
			///<summary></summary>
			Silver
		}

		///<summary></summary>
		public enum BtnShape {
			///<summary></summary>
			Rectangle,
			///<summary></summary>
			Ellipse
		}
	}


}
