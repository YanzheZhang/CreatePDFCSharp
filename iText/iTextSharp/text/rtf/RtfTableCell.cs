using System;
using System.util;

using iTextSharp.text;

namespace iTextSharp.text.rtf {

	/// <summary>
	/// A Cell with extended style attributes
	/// </summary>
	public class RtfTableCell : Cell {
		/* Table border styles */
    
		/// <summary> Table border solid </summary>
		public const int BORDER_UNDEFINED = 0;
    
		/// <summary> Table border solid </summary>
		public const int BORDER_SINGLE = 1;
    
		/// <summary> Table border double thickness </summary>
		public const int BORDER_DOUBLE_THICK = 2;
    
		/// <summary> Table border shadowed </summary>
		public const int BORDER_SHADOWED = 3;
    
		/// <summary> Table border dotted </summary>
		public const int BORDER_DOTTED = 4;
    
		/// <summary> Table border dashed </summary>
		public const int BORDER_DASHED = 5;
    
		/// <summary> Table border hairline </summary>
		public const int BORDER_HAIRLINE = 6;
    
		/// <summary> Table border double line </summary>
		public const int BORDER_DOUBLE = 7;
    
		/// <summary> Table border dot dash line </summary>
		public const int BORDER_DOT_DASH = 8;
    
		/// <summary> Table border dot dot dash line </summary>
		public const int BORDER_DOT_DOT_DASH = 9;
    
		/// <summary> Table border triple line </summary>
		public const int BORDER_TRIPLE = 10;

		/// <summary> Table border line </summary>
		public const int BORDER_THICK_THIN = 11;
    
		/// <summary> Table border line </summary>
		public const int BORDER_THIN_THICK = 12;
    
		/// <summary> Table border line </summary>
		public const int BORDER_THIN_THICK_THIN = 13;
    
		/// <summary> Table border line </summary>
		public const int BORDER_THICK_THIN_MED = 14;
    
		/// <summary> Table border line </summary>
		public const int BORDER_THIN_THICK_MED = 15;
    
		/// <summary> Table border line </summary>
		public const int BORDER_THIN_THICK_THIN_MED = 16;
    
		/// <summary> Table border line </summary>
		public const int BORDER_THICK_THIN_LARGE = 17;
    
		/// <summary> Table border line </summary>
		public const int BORDER_THIN_THICK_LARGE = 18;
    
		/// <summary> Table border line </summary>
		public const int BORDER_THIN_THICK_THIN_LARGE = 19;
    
		/// <summary> Table border line </summary>
		public const int BORDER_WAVY = 20;
    
		/// <summary> Table border line </summary>
		public const int BORDER_DOUBLE_WAVY = 21;
    
		/// <summary> Table border line </summary>
		public const int BORDER_STRIPED = 22;
    
		/// <summary> Table border line </summary>
		public const int BORDER_EMBOSS = 23;
    
		/// <summary> Table border line </summary>
		public const int BORDER_ENGRAVE = 24;
    
		/* Instance variables */
		private float topBorderWidth;
		private float leftBorderWidth;
		private float rightBorderWidth;
		private float bottomBorderWidth;
		private int topBorderStyle = 1;
		private int leftBorderStyle = 1;
		private int rightBorderStyle = 1;
		private int bottomBorderStyle = 1;
    
		/// <summary>
		/// Constructs an empty Cell (for internal use only).
		/// </summary>
		/// <param name="dummy">a dummy value</param>
		public RtfTableCell(bool dummy) : base(dummy) {}
    
		/// <summary>
		/// Constructs a Cell with a certain Element.
		/// </summary>
		/// <remarks>
		/// if the element is a ListItem, Row or
		/// Cell, an exception will be thrown.
		/// </remarks>
		/// <param name="element">the element</param>
		public RtfTableCell(IElement element) : base(element) {}
    
		/// <summary>
		/// Constructs a Cell with a certain content.
		/// </summary>
		/// <remarks>
		/// The String will be converted into a Paragraph.
		/// </remarks>
		/// <param name="content">a string</param>
		public RtfTableCell(String content) : base(content) {}
    
		/// <summary>
		/// Returns a Cell that has been constructed taking in account
		/// the value of some attributes.
		/// </summary>
		/// <param name="attributes">Some attributes</param>
		public RtfTableCell(Properties attributes) : base(attributes) {}
    
		/// <summary>
		/// Set all four borders to f width
		/// </summary>
		/// <value>the desired border width</value>
		public float BorderWidth {
			set {
				base.BorderWidth = value;
				topBorderWidth = value;
				leftBorderWidth = value;
				rightBorderWidth = value;
				bottomBorderWidth = value;
			}
		}
    
		/// <summary>
		/// Get/set the top border width
		/// </summary>
		/// <value>the top border width</value>
		public float TopBorderWidth {
			get {
				return topBorderWidth;
			}

			set {
				topBorderWidth = value;
			}
		}
    
		/// <summary>
		/// Get/set the left border width
		/// </summary>
		/// <value>the left border width</value>
		public float LeftBorderWidth {
			get {
				return leftBorderWidth;
			}

			set {
				leftBorderWidth = value;
			}
		}
    
		/// <summary>
		/// Get/set the right border width
		/// </summary>
		/// <value>the right border width</value>
		public float RightBorderWidth {
			get {
				return rightBorderWidth;
			}

			set {
				rightBorderWidth = value;
			}
		}
    
		/// <summary>
		/// Get/set the bottom border width
		/// </summary>
		/// <value>the bottom border width</value>
		public float BottomBorderWidth {
			get {
				return bottomBorderWidth;
			}

			set {
				bottomBorderWidth = value;
			}
		}
    
		/// <summary>
		/// Set all four borders to style defined by style
		/// </summary>
		public int BorderStyle {
			set {
				topBorderStyle = value;
				leftBorderStyle = value;
				rightBorderStyle = value;
				bottomBorderStyle = value;
			}
		}
    
		/// <summary>
		/// Get/set the top border style
		/// </summary>
		/// <value>the top border style</value>
		public int TopBorderStyle {
			get {
				return topBorderStyle;
			}

			set {
				topBorderStyle = value;
			}
		}
    
		/// <summary>
		/// Get/set the left border style
		/// </summary>
		/// <value>the left border style</value>
		public int LeftBorderStyle {
			get {
				return leftBorderStyle;
			}

			set {
				leftBorderStyle = value;
			}
		}
    
		/// <summary>
		/// Get/set the right border style
		/// </summary>
		/// <value>the right border style</value>
		public int RightBorderStyle {
			get {
				return rightBorderStyle;
			}

			set {
				rightBorderStyle = value;
			}
		}
    
		/// <summary>
		/// Get/set the bottom border style
		/// </summary>
		/// <value>the bottom border style</value>
		public int BottomBorderStyle {
			get {
				return bottomBorderStyle;
			}

			set {
				bottomBorderStyle = value;
			}
		}
    
		/// <summary>
		/// Get the RTF control word for style
		/// </summary>
		/// <param name="style">the style</param>
		/// <returns>RTF control word</returns>
		internal static byte[] getStyleControlWord(int style) {
			switch(style) {
				case BORDER_UNDEFINED				: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrs");
				case BORDER_SINGLE 					: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrs");
				case BORDER_DOUBLE_THICK	 		: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrth");
				case BORDER_SHADOWED 				: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrsh");
				case BORDER_DOTTED   				: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrdot");
				case BORDER_DASHED   				: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrdash");
				case BORDER_HAIRLINE   				: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrhair");
				case BORDER_DOUBLE 		  			: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrdb");
				case BORDER_DOT_DASH   				: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrdashd");
				case BORDER_DOT_DOT_DASH			: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrdashdd");
				case BORDER_TRIPLE					: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrtriple");
				case BORDER_THICK_THIN				: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrtnthsg");
				case BORDER_THIN_THICK				: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrthtnsg");
				case BORDER_THIN_THICK_THIN			: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrtnthtnsg");
				case BORDER_THICK_THIN_MED			: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrtnthmg");
				case BORDER_THIN_THICK_MED			: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrthtnmg");
				case BORDER_THIN_THICK_THIN_MED		: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrtnthtnmg");
				case BORDER_THICK_THIN_LARGE		: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrtnthlg");
				case BORDER_THIN_THICK_LARGE		: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrthtnlg");
				case BORDER_THIN_THICK_THIN_LARGE	: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrtnthtnlg");
				case BORDER_WAVY					: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrwavy");
				case BORDER_DOUBLE_WAVY				: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrwavydb");
				case BORDER_STRIPED					: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrdashdotstr");
				case BORDER_EMBOSS					: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdremboss");
				case BORDER_ENGRAVE					: return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrengrave");
			}
        
			return System.Text.ASCIIEncoding.ASCII.GetBytes("brdrs");
		}
	}
}