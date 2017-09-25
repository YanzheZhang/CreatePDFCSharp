using System;
using System.Drawing;

/*
 * $Id: PdfTemplate.cs,v 1.1.1.1 2003/02/04 02:57:46 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 Paulo Soares
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
 *
 * The Original Code is 'iText, a free JAVA-PDF library'.
 *
 * The Initial Developer of the Original Code is Bruno Lowagie. Portions created by
 * the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000, 2001, 2002 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), in which case the
 * provisions of LGPL are applicable instead of those above.  If you wish to
 * allow use of your version of this file only under the terms of the LGPL
 * License and not to allow others to use your version of this file under
 * the MPL, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the LGPL.
 * If you do not delete the provisions above, a recipient may use your version
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE.
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.pdf {

/**
 * Implements the form XObject.
 */

public class PdfTemplate : PdfContentByte {
    public static int TYPE_TEMPLATE = 1;
    public static int TYPE_IMPORTED = 2;
    public static int TYPE_PATTERN = 3;
    protected int type;
    /** The indirect reference to this template */
    protected PdfIndirectReference thisReference;
    
    /** The fonts used by this template */
    protected PdfFontDictionary fontDictionary;
    
    /** The images and other templates used by this template */
    protected PdfXObjectDictionary xObjectDictionary;
    
    protected PdfColorDictionary colorDictionary;
    
    protected PdfPatternDictionary patternDictionary;
    
    protected PdfShadingDictionary shadingDictionary;
    
    /** The bounding box of this template */
    protected Rectangle bBox = new Rectangle(0, 0);
    
    protected PdfArray matrix;
    
    /**
     *Creates a <CODE>PdfTemplate</CODE>.
     */
    
    protected PdfTemplate() : base(null) {
        type = TYPE_TEMPLATE;
    }
    
    /**
     * Creates new PdfTemplate
     *
     * @param wr the <CODE>PdfWriter</CODE>
     */
    
    internal PdfTemplate(PdfWriter wr) : base(wr) {
        type = TYPE_TEMPLATE;
        fontDictionary = new PdfFontDictionary();
        xObjectDictionary = new PdfXObjectDictionary();
        colorDictionary = new PdfColorDictionary();
        patternDictionary = new PdfPatternDictionary();
        shadingDictionary = new PdfShadingDictionary();
        thisReference = writer.PdfIndirectReference;
    }
    
    /**
     * Gets the bounding width of this template.
     *
     * @return width the bounding width
     */
    public float Width {
		get {
			return bBox.Width;
		}

		set {
			bBox.Left = 0;
			bBox.Right = value;
		}
    }
    
    /**
     * Gets the bounding heigth of this template.
     *
     * @return heigth the bounding height
     */
    
    public float Height {
		get {
			return bBox.Height;
		}

		set {
			bBox.Bottom = 0;
			bBox.Top = value;
		}
    }
    
    public Rectangle BoundingBox {
		get {
			return bBox;
		}
		set {
			this.bBox = value;
		}
	}
    
    public void setMatrix(float a, float b, float c, float d, float e, float f) {
		matrix = new PdfArray();
		matrix.Add(new PdfNumber(a));
		matrix.Add(new PdfNumber(b));
		matrix.Add(new PdfNumber(c));
		matrix.Add(new PdfNumber(d));
		matrix.Add(new PdfNumber(e));
		matrix.Add(new PdfNumber(f));
	}

	internal PdfArray Matrix {
		get {
			return matrix;
		}
	}
    
    /**
     * Gets the indirect reference to this template.
     *
     * @return the indirect reference to this template
     */
    
    internal PdfIndirectReference IndirectReference {
		get {
			return thisReference;
		}
    }
    
    /**
     * Adds a template to this template.
     *
     * @param template the template
     * @param a an element of the transformation matrix
     * @param b an element of the transformation matrix
     * @param c an element of the transformation matrix
     * @param d an element of the transformation matrix
     * @param e an element of the transformation matrix
     * @param f an element of the transformation matrix
     */
    
    public override void addTemplate(PdfTemplate template, float a, float b, float c, float d, float e, float f) {
        checkNoPattern(template);
        PdfName name = writer.addDirectTemplateSimple(template);
        content.Append("q ");
        content.Append(a).Append(' ');
        content.Append(b).Append(' ');
        content.Append(c).Append(' ');
        content.Append(d).Append(' ');
        content.Append(e).Append(' ');
        content.Append(f).Append(" cm ");
        content.Append(name.ToString()).Append(" Do Q").Append_i(separator);
        xObjectDictionary.put(name, template.IndirectReference);
    }
    
    /**
     * Adds an <CODE>Image</CODE> to this template. The positioning of the <CODE>Image</CODE>
     * is done with the transformation matrix. To position an <CODE>image</CODE> at (x,y)
     * use addImage(image, image_width, 0, 0, image_height, x, y).
     * @param image the <CODE>Image</CODE> object
     * @param a an element of the transformation matrix
     * @param b an element of the transformation matrix
     * @param c an element of the transformation matrix
     * @param d an element of the transformation matrix
     * @param e an element of the transformation matrix
     * @param f an element of the transformation matrix
     * @throws DocumentException on error
     */
    
    public override void addImage(Image image, float a, float b, float c, float d, float e, float f) {
        try {
            PdfName name;
            if (image.isImgTemplate()) {
                name = pdf.addDirectImageSimple(image);
                PdfTemplate template = image.TemplateData;
                float w = template.Width;
                float h = template.Height;
                addTemplate(template, a / w, b / w, c / h, d / h, e, f);
            }
            else {
                Image maskImage = image.ImageMask;
                if (maskImage != null) {
                    PdfName mname = pdf.addDirectImageSimple(maskImage);
                    xObjectDictionary.put(mname, writer.getImageReference(mname));
                }
                name = pdf.addDirectImageSimple(image);
                content.Append("q ");
                content.Append(a).Append(' ');
                content.Append(b).Append(' ');
                content.Append(c).Append(' ');
                content.Append(d).Append(' ');
                content.Append(e).Append(' ');
                content.Append(f).Append(" cm ");
                content.Append(name.ToString()).Append(" Do Q").Append_i(separator);
            }
            if (!image.isImgTemplate())
                xObjectDictionary.put(name, writer.getImageReference(name));
        }
        catch (Exception ee) {
            throw new DocumentException(ee.Message);
        }
    }
    
    public override void setColorFill(PdfSpotColor sp, float tint) {
        state.colorDetails = writer.addSimple(sp);
        colorDictionary.put(state.colorDetails.ColorName, state.colorDetails.IndirectReference);
        content.Append(state.colorDetails.ColorName.toPdf(null)).Append(" cs ").Append(tint).Append(" scn").Append_i(separator);
    }
    
    public override void setColorStroke(PdfSpotColor sp, float tint) {
        state.colorDetails = writer.addSimple(sp);
        colorDictionary.put(state.colorDetails.ColorName, state.colorDetails.IndirectReference);
        content.Append(state.colorDetails.ColorName.toPdf(null)).Append(" CS ").Append(tint).Append(" SCN").Append_i(separator);
    }
    
    public virtual PdfPatternPainter PatternFill {
		set {
			if (value.isStencil()) {
				setPatternFill(value, value.DefaultColor);
				return;
			}
			checkWriter();
			PdfName name = writer.addSimplePattern(value);
			patternDictionary.put(name, value.IndirectReference);
			content.Append(PdfName.PATTERN.toPdf(null)).Append(" cs ").Append(name.toPdf(null)).Append(" scn").Append_i(separator);
		}
    }
    
    public virtual PdfPatternPainter PatternStroke {
		set {
			if (value.isStencil()) {
				setPatternStroke(value, value.DefaultColor);
				return;
			}
			checkWriter();
			PdfName name = writer.addSimplePattern(value);
			patternDictionary.put(name, value.IndirectReference);
			content.Append(PdfName.PATTERN.toPdf(null)).Append(" CS ").Append(name.toPdf(null)).Append(" SCN").Append_i(separator);
		}
    }

    public override void setPatternFill(PdfPatternPainter p, Color color, float tint) {
        checkWriter();
        if (!p.isStencil())
            throw new RuntimeException("An uncolored pattern was expected.");
        PdfName name = writer.addSimplePattern(p);
        patternDictionary.put(name, p.IndirectReference);
        ColorDetails csDetail = writer.addSimplePatternColorspace(color);
        colorDictionary.put(csDetail.ColorName, csDetail.IndirectReference);
        content.Append(csDetail.ColorName.toPdf(null)).Append(" cs").Append_i(separator);
        outputColorNumbers(color, tint);
        content.Append(' ').Append(name.toPdf(null)).Append(" scn").Append_i(separator);
    }
    
    public override void setPatternStroke(PdfPatternPainter p, Color color, float tint) {
        checkWriter();
        if (!p.isStencil())
            throw new RuntimeException("An uncolored pattern was expected.");
        PdfName name = writer.addSimplePattern(p);
        patternDictionary.put(name, p.IndirectReference);
        ColorDetails csDetail = writer.addSimplePatternColorspace(color);
        colorDictionary.put(csDetail.ColorName, csDetail.IndirectReference);
        content.Append(csDetail.ColorName.toPdf(null)).Append(" CS").Append_i(separator);
        outputColorNumbers(color, tint);
        content.Append(' ').Append(name.toPdf(null)).Append(" SCN").Append_i(separator);
    }

    public override void paintShading(PdfShading shading) {
        writer.addSimpleShading(shading);
        shadingDictionary.put(shading.ShadingName, shading.ShadingReference);
        content.Append(shading.ShadingName.toPdf(null)).Append(" sh").Append_i(separator);
        ColorDetails details = shading.ColorDetails;
        if (details != null)
            colorDictionary.put(details.ColorName, details.IndirectReference);
    }
    
    public override PdfShadingPattern ShadingFill {
		set {
			writer.addSimpleShadingPattern(value);
			patternDictionary.put(value.PatternName, value.PatternReference);
			content.Append(PdfName.PATTERN.toPdf(null)).Append(" cs ").Append(value.PatternName.toPdf(null)).Append(" scn").Append_i(separator);
			ColorDetails details = value.ColorDetails;
			if (details != null)
				colorDictionary.put(details.ColorName, details.IndirectReference);
		}
    }

    public override PdfShadingPattern ShadingStroke {
		set {
			writer.addSimpleShadingPattern(value);
			patternDictionary.put(value.PatternName, value.PatternReference);
			content.Append(PdfName.PATTERN.toPdf(null)).Append(" CS ").Append(value.PatternName.toPdf(null)).Append(" SCN").Append_i(separator);
			ColorDetails details = value.ColorDetails;
			if (details != null)
				colorDictionary.put(details.ColorName, details.IndirectReference);
		}
    }
    
    public void beginVariableText() {
        content.Append("/Tx BMC ");
    }
    
    public void endVariableText() {
        content.Append("EMC ");
    }
    
    /**
     * Constructs the resources used by this template.
     *
     * @return the resources used by this template
     */
    
    internal virtual PdfObject Resources {
		get {
			PdfResources resources = new PdfResources();
			int procset = PdfProcSet.PDF;
			if (fontDictionary.containsFont()) {
				resources.Add(fontDictionary);
				procset |= PdfProcSet.TEXT;
			}
			if (xObjectDictionary.containsXObject()) {
				resources.Add(xObjectDictionary);
				procset |= PdfProcSet.IMAGEC;
			}
			if (colorDictionary.containsColorSpace())
				resources.Add(colorDictionary);
			if (patternDictionary.containsPattern())
				resources.Add(patternDictionary);
			if (shadingDictionary.containsShading())
				resources.Add(shadingDictionary);
			resources.Add(new PdfProcSet(procset));
			return resources;
		}
    }
    
    /**
     * Gets the stream representing this template.
     *
     * @return the stream representing this template
     */
    
    internal virtual PdfStream FormXObject {
		get {
			return new PdfFormXObject(this);
		}
    }
    
    /**
     * Set the font and the size for the subsequent text writing.
     *
     * @param bf the font
     * @param size the font size in points
     */
    
    public override void setFontAndSize(BaseFont bf, float size) {
        state.size = size;
        state.fontDetails = writer.addSimple(bf);
        PdfName name = state.fontDetails.FontName;
        content.Append(name.toPdf(null)).Append(' ').Append(size).Append(" Tf").Append_i(separator);
        fontDictionary.put(name, state.fontDetails.IndirectReference);
    }
    
    /**
     * Gets a duplicate of this <CODE>PdfTemplate</CODE>. All
     * the members are copied by reference but the buffer stays different.
     * @return a copy of this <CODE>PdfTemplate</CODE>
     */
    
    public override PdfContentByte Duplicate {
		get {
			PdfTemplate tpl = new PdfTemplate();
			tpl.writer = writer;
			tpl.pdf = pdf;
			tpl.thisReference = thisReference;
			tpl.fontDictionary = fontDictionary;
			tpl.xObjectDictionary = xObjectDictionary;
			tpl.colorDictionary = colorDictionary;
			tpl.patternDictionary = patternDictionary;
			tpl.shadingDictionary = shadingDictionary;
			tpl.bBox = new Rectangle(bBox);
			if (matrix != null) {
				tpl.matrix = new PdfArray(matrix);
			}
			return tpl;
		}
    }
    
    public int Type {
		get {
			return type;
		}
    }
}
}