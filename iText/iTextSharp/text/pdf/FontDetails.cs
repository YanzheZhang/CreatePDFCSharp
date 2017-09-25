using System;
using System.Collections;
using System.util;

/*
 * $Id: FontDetails.cs,v 1.1.1.1 2003/02/04 02:56:52 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 by Paulo Soares.
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

	/** Each font in the document will have an instance of this class
	 * where the characters used will be represented.
	 *
	 * @author  Paulo Soares (psoares@consiste.pt)
	 */
	internal class FontDetails {
    
		/** The indirect reference to this font
		 */    
		PdfIndirectReference indirectReference;
		/** The font name that appears in the document body stream
		 */    
		PdfName fontName;
		/** The font
		 */    
		BaseFont baseFont;
		/** The font if its an instance of <CODE>TrueTypeFontUnicode</CODE>
		 */    
		TrueTypeFontUnicode ttu;

		CJKFont cjkFont;
		/** The array used with single byte encodings
		 */    
		byte[] shortTag;
		/** The map used with double byte encodings. The key is int(glyph) and the
		 * value is int[]{glyph, width, Unicode code}
		 */    
		Hashmap longTag;
    
		IntHashtable cjkTag;
		/** The font type
		 */    
		int fontType;
		/** <CODE>true</CODE> if the font is symbolic
		 */    
		bool symbolic;
		/** Indicates if all the glyphs and widths for that particular
		 * encoding should be included in the document.
		 */
		protected bool subset = true;
		/** Each font used in a document has an instance of this class.
		 * This class stores the characters used in the document and other
		 * specifics unique to the current working document.
		 * @param fontName the font name
		 * @param indirectReference the indirect reference to the font
		 * @param baseFont the <CODE>BaseFont</CODE>
		 */
		internal FontDetails(PdfName fontName, PdfIndirectReference indirectReference, BaseFont baseFont) {
			this.fontName = fontName;
			this.indirectReference = indirectReference;
			this.baseFont = baseFont;
			fontType = baseFont.FontType;
			switch (fontType) {
				case BaseFont.FONT_TYPE_T1:
				case BaseFont.FONT_TYPE_TT:
					shortTag = new byte[256];
					break;
				case BaseFont.FONT_TYPE_CJK:
					cjkTag = new IntHashtable();
					cjkFont = (CJKFont)baseFont;
					break;
				case BaseFont.FONT_TYPE_TTUNI:
					longTag = new Hashmap();
					ttu = (TrueTypeFontUnicode)baseFont;
					symbolic = baseFont.isFontSpecific();
					break;
			}
		}
    
		/** Gets the indirect reference to this font.
		 * @return the indirect reference to this font
		 */    
		internal PdfIndirectReference IndirectReference {
			get {
				return indirectReference;
			}
		}
    
		/** Gets the font name as it appears in the document body.
		 * @return the font name
		 */    
		internal PdfName FontName {
			get {
				return fontName;
			}
		}
    
		/** Gets the <CODE>BaseFont</CODE> of this font.
		 * @return the <CODE>BaseFont</CODE> of this font
		 */    
		internal BaseFont BaseFont {
			get {
				return baseFont;
			}
		}
    
		/** Converts the text into bytes to be placed in the document.
		 * The conversion is done according to the font and the encoding and the characters
		 * used are stored.
		 * @param text the text to convert
		 * @return the conversion
		 */    
		internal byte[] convertToBytes(string text) {
			byte[] b = null;
				switch (fontType) {
					case BaseFont.FONT_TYPE_T1:
					case BaseFont.FONT_TYPE_TT: {
						b = baseFont.convertToBytes(text);
						int len = b.Length;
						for (int k = 0; k < len; ++k)
							shortTag[((int)b[k]) & 0xff] = 1;
						break;
					}
					case BaseFont.FONT_TYPE_CJK: {
						int len = text.Length;
						for (int k = 0; k < len; ++k)
							cjkTag[cjkFont.getCidCode(text[k])] = 0;
						b = baseFont.convertToBytes(text);
						break;
					}
					case BaseFont.FONT_TYPE_TTUNI: {
						try {
							int len = text.Length;
							int[] metrics = null;
								char[] glyph = new char[len];
									int i = 0;
							if (symbolic) {
								b = PdfEncodings.convertToBytes(text, BaseFont.WINANSI);
								len = b.Length;
								for (int k = 0; k < len; ++k) {
									metrics = ttu.getMetricsTT(b[k] & 0xff);
									if (metrics == null)
										continue;
									longTag.Add(metrics[0], new int[]{metrics[0], metrics[1], ttu.getUnicodeDifferences(b[k] & 0xff)});
									glyph[i++] = (char)metrics[0];
								}
							}
							else {
								for (int k = 0; k < len; ++k) {
									char c = text[k];
									metrics = ttu.getMetricsTT(c);
									if (metrics == null)
										continue;
									int m0 = metrics[0];
									int gl = m0;
									if (!longTag.ContainsKey(gl))
										longTag.Add(gl, new int[]{m0, metrics[1], c});
									glyph[i++] = (char)m0;
								}
							}
							string s = new string(glyph, 0, i);
							b = System.Text.Encoding.GetEncoding(CJKFont.CJK_ENCODING).GetBytes(s);
						}
						catch (Exception e) {
							throw e;
						}
						break;
					}
				}
			return b;
		}
    
		/** Writes the font definition to the document.
		 * @param writer the <CODE>PdfWriter</CODE> of this document
		 */    
		internal void writeFont(PdfWriter writer) {
			try {
				switch (fontType) {
					case BaseFont.FONT_TYPE_T1:
					case BaseFont.FONT_TYPE_TT: {
						int firstChar;
						int lastChar;
						if (subset) {
							for (firstChar = 0; firstChar < 256; ++firstChar) {
								if (shortTag[firstChar] != 0)
									break;
							}
							for (lastChar = 255; lastChar >= firstChar; --lastChar) {
								if (shortTag[lastChar] != 0)
									break;
							}
							if (firstChar > 255) {
								firstChar = 255;
								lastChar = 255;
							}
						}
						else {
							for (int k = 0; k < shortTag.Length; ++k)
								shortTag[k] = 1;
							firstChar = 0;
							lastChar = shortTag.Length - 1;
						}
						baseFont.writeFont(writer, indirectReference, new Object[]{firstChar, lastChar, shortTag});
						break;
					}
					case BaseFont.FONT_TYPE_CJK:
						baseFont.writeFont(writer, indirectReference, new Object[]{cjkTag});
						break;
					case BaseFont.FONT_TYPE_TTUNI:
						baseFont.writeFont(writer, indirectReference, new Object[]{longTag});
						break;
				}
			}
			catch(Exception e) {
				throw e;
			}
		}
    
		/** Indicates if all the glyphs and widths for that particular
		 * encoding should be included in the document.
		 * @return <CODE>false</CODE> to include all the glyphs and widths.
		 */
		public bool isSubset() {
			return subset;
		}
    
		/** Indicates if all the glyphs and widths for that particular
		 * encoding should be included in the document. Set to <CODE>false</CODE>
		 * to include all.
		 * @param subset new value of property subset
		 */
		public bool Subset {
			set {
				this.subset = value;
			}
		}
	}
}