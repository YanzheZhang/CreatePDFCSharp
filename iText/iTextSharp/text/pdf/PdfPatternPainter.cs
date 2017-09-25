using System;
using System.Drawing;

using iTextSharp.text;

namespace iTextSharp.text.pdf {

	/**
	 * Implements the pattern.
	 */

	public class PdfPatternPainter : PdfTemplate {
    
		protected float xstep, ystep;
		protected bool stencil = false;
		protected Color defaultColor;
    
		/**
		 *Creates a <CODE>PdfPattern</CODE>.
		 */
    
		private PdfPatternPainter() : base(null) {
			type = TYPE_PATTERN;
		}
    
		/**
		 * Creates new PdfPattern
		 *
		 * @param wr the <CODE>PdfWriter</CODE>
		 */
    
		internal PdfPatternPainter(PdfWriter wr) : base(wr) {
			type = TYPE_PATTERN;
		}
    
		internal PdfPatternPainter(PdfWriter wr, Color defaultColor) : this(wr) {
			stencil = true;
			if (defaultColor == null)
				this.defaultColor = new Color(System.Drawing.Color.Gray);
			else
				this.defaultColor = defaultColor;
		}
    
		public float XStep {
			get {
				return this.xstep;
			}

			set {
				this.xstep = value;
			}
		}
    
		public float YStep {
			get {
				return this.ystep;
			}

			set {
				this.ystep = value;
			}
		}
    
		public bool isStencil() {
			return stencil;
		}
    
		public void setPatternMatrix(float a, float b, float c, float d, float e, float f) {
			setMatrix(a, b, c, d, e, f);
		}
		/**
		 * Gets the stream representing this pattern
		 *
		 * @return the stream representing this pattern
		 */
    
		internal PdfPattern Pattern {
			get {
				return new PdfPattern(this);
			}
		}
    
		/**
		 * Gets a duplicate of this <CODE>PdfPatternPainter</CODE>. All
		 * the members are copied by reference but the buffer stays different.
		 * @return a copy of this <CODE>PdfPatternPainter</CODE>
		 */
    
		public override PdfContentByte Duplicate {
			get {
				PdfPatternPainter tpl = new PdfPatternPainter();
				tpl.writer = writer;
				tpl.pdf = pdf;
				tpl.thisReference = thisReference;
				tpl.fontDictionary = fontDictionary;
				tpl.xObjectDictionary = xObjectDictionary;
				tpl.colorDictionary = colorDictionary;
				tpl.patternDictionary = patternDictionary;
				tpl.bBox = new Rectangle(bBox);
				tpl.xstep = xstep;
				tpl.ystep = ystep;
				tpl.matrix = matrix;
				tpl.stencil = stencil;
				tpl.defaultColor = defaultColor;
				return tpl;
			}
		}
    
		public Color DefaultColor {
			get {
				return defaultColor;
			}
		}
    
		public override float GrayFill {
			set {
				checkNoColor();
				base.GrayFill = value;
			}
		}
    
		public override void resetGrayFill() {
			checkNoColor();
			base.resetGrayFill();
		}
    
		public override float GrayStroke {
			set {
				checkNoColor();
				base.GrayStroke = value;
			}
		}
    
		public override void resetGrayStroke() {
			checkNoColor();
			base.resetGrayStroke();
		}
    
		public override void setRGBColorFillF(float red, float green, float blue) {
			checkNoColor();
			base.setRGBColorFillF(red, green, blue);
		}
    
		public override void resetRGBColorFill() {
			checkNoColor();
			base.resetRGBColorFill();
		}
    
		public override void setRGBColorStrokeF(float red, float green, float blue) {
			checkNoColor();
			base.setRGBColorStrokeF(red, green, blue);
		}
    
		public override void resetRGBColorStroke() {
			checkNoColor();
			base.resetRGBColorStroke();
		}
    
		public override void setCMYKColorFillF(float cyan, float magenta, float yellow, float black) {
			checkNoColor();
			base.setCMYKColorFillF(cyan, magenta, yellow, black);
		}
    
		public override void resetCMYKColorFill() {
			checkNoColor();
			base.resetCMYKColorFill();
		}
    
		public override void setCMYKColorStrokeF(float cyan, float magenta, float yellow, float black) {
			checkNoColor();
			base.setCMYKColorStrokeF(cyan, magenta, yellow, black);
		}
    
		public override void resetCMYKColorStroke() {
			checkNoColor();
			base.resetCMYKColorStroke();
		}
    
		public override void addImage(Image image, float a, float b, float c, float d, float e, float f) {
			if (stencil && !image.isMask())
				checkNoColor();
			base.addImage(image, a, b, c, d, e, f);
		}
    
		public override void setCMYKColorFill(int cyan, int magenta, int yellow, int black) {
			checkNoColor();
			base.setCMYKColorFill(cyan, magenta, yellow, black);
		}
    
		public override void setCMYKColorStroke(int cyan, int magenta, int yellow, int black) {
			checkNoColor();
			base.setCMYKColorStroke(cyan, magenta, yellow, black);
		}
    
		public override void setRGBColorFill(int red, int green, int blue) {
			checkNoColor();
			base.setRGBColorFill(red, green, blue);
		}
    
    
		public override void setRGBColorStroke(int red, int green, int blue) {
			checkNoColor();
			base.setRGBColorStroke(red, green, blue);
		}
    
		public override Color ColorStroke {
			set {
				checkNoColor();
				base.ColorStroke = value;
			}
		}
    
		public override Color ColorFill {
			set {
				checkNoColor();
				base.ColorFill = value;
			}
		}
    
		public override void setColorFill(PdfSpotColor sp, float tint) {
			checkNoColor();
			base.setColorFill(sp, tint);
		}
    
		public override void setColorStroke(PdfSpotColor sp, float tint) {
			checkNoColor();
			base.setColorStroke(sp, tint);
		}
    
		public override PdfPatternPainter PatternFill {
			set {
				checkNoColor();
				base.setPatternFill(value);
			}
		}
    
		public override void setPatternFill(PdfPatternPainter p, Color color, float tint) {
			checkNoColor();
			base.setPatternFill(p, color, tint);
		}
    
		public override void setPatternStroke(PdfPatternPainter p, Color color, float tint) {
			checkNoColor();
			base.setPatternStroke(p, color, tint);
		}
    
		public override PdfPatternPainter PatternStroke {
			set {
				checkNoColor();
				base.setPatternStroke(value);
			}
		}
    
		internal void checkNoColor() {
			if (stencil)
				throw new RuntimeException("Colors are not allowed in uncolored tile patterns.");
		}
	}
}