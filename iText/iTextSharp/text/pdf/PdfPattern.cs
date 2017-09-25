using System;

namespace iTextSharp.text.pdf {

	/**
	 * A <CODE>PdfPattern</CODE> defines a ColorSpace
	 *
	 * @see		PdfStream
	 */

	public class PdfPattern : PdfStream {
    
		internal PdfPattern(PdfPatternPainter painter) : base() {
			PdfNumber one = new PdfNumber(1);
			PdfArray matrix = painter.Matrix;
			if ( matrix != null ) {
				put(PdfName.MATRIX, matrix);
			}
			put(PdfName.TYPE, PdfName.PATTERN);
			put(PdfName.BBOX, new PdfRectangle(painter.BoundingBox));
			put(PdfName.RESOURCES, painter.Resources);
			put(PdfName.TILINGTYPE, one);
			put(PdfName.PATTERNTYPE, one);
			if (painter.isStencil())
				put(PdfName.PAINTTYPE, new PdfNumber(2));
			else
				put(PdfName.PAINTTYPE, one);
			put(PdfName.XSTEP, new PdfNumber(painter.XStep));
			put(PdfName.YSTEP, new PdfNumber(painter.YStep));
			bytes = painter.toPdf(null);
			put(PdfName.LENGTH, new PdfNumber(bytes.Length));
			try {
				flateCompress();
			} catch (Exception e) {
				throw e;
			}
		}
	}
}