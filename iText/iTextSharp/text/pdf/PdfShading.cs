using System;
using System.Drawing;

/*
 * Copyright 2002 Paulo Soares
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

	/** Implements the shading dictionary (or stream).
	 *
	 * @author Paulo Soares (psoares@consiste.pt)
	 */
	public class PdfShading {

		protected PdfDictionary shading;
    
		protected PdfWriter writer;
    
		protected int shadingType;
    
		protected ColorDetails colorDetails;
    
		protected PdfName shadingName;
    
		protected PdfIndirectReference shadingReference;
    
		/** Holds value of property bBox. */
		protected float[] bBox;
    
		/** Holds value of property antiAlias. */
		protected bool antiAlias = false;
    
		/** Creates new PdfShading */
		protected PdfShading(PdfWriter writer) {
			this.writer = writer;
		}
    
		protected void setColorSpace(Color color) {
			int type = ExtendedColor.getType(color);
			PdfObject colorSpace = null;
			switch (type) {
				case ExtendedColor.TYPE_GRAY: {
					colorSpace = PdfName.DEVICEGRAY;
					break;
				}
				case ExtendedColor.TYPE_CMYK: {
					colorSpace = PdfName.DEVICECMYK;
					break;
				}
				case ExtendedColor.TYPE_SEPARATION: {
					SpotColor spot = (SpotColor)color;
					colorDetails = writer.addSimple(spot.getPdfSpotColor());
					colorSpace = colorDetails.IndirectReference;
					break;
				}
				case ExtendedColor.TYPE_PATTERN:
				case ExtendedColor.TYPE_SHADING: {
					throwColorSpaceError();
					break;
				}
				default:
					colorSpace = PdfName.DEVICERGB;
					break;
			}
			shading.put(PdfName.COLORSPACE, colorSpace);
		}
    
		public static void throwColorSpaceError() {
			throw new IllegalArgumentException("A tiling or shading pattern cannot be used as a color space in a shading pattern");
		}
    
		public static void checkCompatibleColors(Color c1, Color c2) {
			int type1 = ExtendedColor.getType(c1);
			int type2 = ExtendedColor.getType(c2);
			if (type1 != type2)
				throw new IllegalArgumentException("Both colors must be of the same type.");
			if (type1 == ExtendedColor.TYPE_SEPARATION && ((SpotColor)c1).getPdfSpotColor() != ((SpotColor)c2).getPdfSpotColor())
				throw new IllegalArgumentException("The spot color must be the same, only the tint can vary.");
			if (type1 == ExtendedColor.TYPE_PATTERN || type1 == ExtendedColor.TYPE_SHADING)
				throwColorSpaceError();
		}
    
		public static float[] getColorArray(Color color) {
			int type = ExtendedColor.getType(color);
			switch (type) {
				case ExtendedColor.TYPE_GRAY: {
					return new float[]{((GrayColor)color).Gray};
				}
				case ExtendedColor.TYPE_CMYK: {
					CMYKColor cmyk = (CMYKColor)color;
					return new float[]{cmyk.Cyan, cmyk.Magenta, cmyk.Yellow, cmyk.Black};
				}
				case ExtendedColor.TYPE_SEPARATION: {
					return new float[]{((SpotColor)color).Tint};
				}
				case ExtendedColor.TYPE_RGB: {
					return new float[]{color.R / 255f, color.G / 255f, color.B / 255f};
				}
			}
			throwColorSpaceError();
			return null;
		}

		public static PdfShading type1(PdfWriter writer, Color colorSpace, float[] domain, float[] tMatrix, PdfFunction function) {
			PdfShading sp = new PdfShading(writer);
			sp.shading = new PdfDictionary();
			sp.shadingType = 1;
			sp.shading.put(PdfName.SHADINGTYPE, new PdfNumber(sp.shadingType));
			sp.setColorSpace(colorSpace);
			if (domain != null)
				sp.shading.put(PdfName.DOMAIN, new PdfArray(domain));
			if (tMatrix != null)
				sp.shading.put(PdfName.MATRIX, new PdfArray(tMatrix));
			sp.shading.put(PdfName.FUNCTION, function.Reference);
			return sp;
		}
    
		public static PdfShading type2(PdfWriter writer, Color colorSpace, float[] coords, float[] domain, PdfFunction function, bool[] extend) {
			PdfShading sp = new PdfShading(writer);
			sp.shading = new PdfDictionary();
			sp.shadingType = 2;
			sp.shading.put(PdfName.SHADINGTYPE, new PdfNumber(sp.shadingType));
			sp.setColorSpace(colorSpace);
			sp.shading.put(PdfName.COORDS, new PdfArray(coords));
			if (domain != null)
				sp.shading.put(PdfName.DOMAIN, new PdfArray(domain));
			sp.shading.put(PdfName.FUNCTION, function.Reference);
			if (extend != null && (extend[0] || extend[1])) {
				PdfArray array = new PdfArray(extend[0] ? PdfBoolean.PDFTRUE : PdfBoolean.PDFFALSE);
				array.Add(extend[1] ? PdfBoolean.PDFTRUE : PdfBoolean.PDFFALSE);
				sp.shading.put(PdfName.EXTEND, array);
			}
			return sp;
		}

		public static PdfShading type3(PdfWriter writer, Color colorSpace, float[] coords, float[] domain, PdfFunction function, bool[] extend) {
			PdfShading sp = type2(writer, colorSpace, coords, domain, function, extend);
			sp.shadingType = 3;
			sp.shading.put(PdfName.SHADINGTYPE, new PdfNumber(sp.shadingType));
			return sp;
		}
    
		public static PdfShading simpleAxial(PdfWriter writer, float x0, float y0, float x1, float y1, Color startColor, Color endColor, bool extendStart, bool extendEnd) {
			checkCompatibleColors(startColor, endColor);
			PdfFunction function = PdfFunction.type2(writer, new float[]{0, 1}, null, getColorArray(startColor),
				getColorArray(endColor), 1);
			return type2(writer, startColor, new float[]{x0, y0, x1, y1}, null, function, new bool[]{extendStart, extendEnd});
		}
    
		public static PdfShading simpleAxial(PdfWriter writer, float x0, float y0, float x1, float y1, Color startColor, Color endColor) {
			return simpleAxial(writer, x0, y0, x1, y1, startColor, endColor, true, true);
		}
    
		public static PdfShading simpleRadial(PdfWriter writer, float x0, float y0, float r0, float x1, float y1, float r1, Color startColor, Color endColor, bool extendStart, bool extendEnd) {
			checkCompatibleColors(startColor, endColor);
			PdfFunction function = PdfFunction.type2(writer, new float[]{0, 1}, null, getColorArray(startColor),
				getColorArray(endColor), 1);
			return type3(writer, startColor, new float[]{x0, y0, r0, x1, y1, r1}, null, function, new bool[]{extendStart, extendEnd});
		}

		public static PdfShading simpleRadial(PdfWriter writer, float x0, float y0, float r0, float x1, float y1, float r1, Color startColor, Color endColor) {
			return simpleRadial(writer, x0, y0, r0, x1, y1, r1, startColor, endColor, true, true);
		}

		internal PdfName ShadingName {
			get {
				return shadingName;
			}
		}
    
		internal PdfIndirectReference ShadingReference {
			get {
				if (shadingReference == null)
					shadingReference = writer.PdfIndirectReference;
				return shadingReference;
			}
		}
    
		internal int Name {
			set {
				shadingName = new PdfName("Sh" + value);
			}
		}
    
		internal void addToBody() {
			if (bBox != null)
				shading.put(PdfName.BBOX, new PdfArray(bBox));
			if (antiAlias)
				shading.put(PdfName.ANTIALIAS, PdfBoolean.PDFTRUE);
			writer.addToBody(shading, this.ShadingReference);
		}
    
		internal PdfWriter Writer {
			get {
				return writer;
			}
		}
    
		internal ColorDetails ColorDetails {
			get {
				return colorDetails;
			}
		}
    
		public float[] BBox {
			get {
				return bBox;
			}

			set {
				if (value.Length != 4)
					throw new IllegalArgumentException("BBox must be a 4 element array.");
				this.bBox = value;
			}
		}
    
		public bool isAntiAlias() {
			return antiAlias;
		}
    
		public bool AntiAlias {
			set {
				this.antiAlias = antiAlias;
			}
		}
    
	}
}