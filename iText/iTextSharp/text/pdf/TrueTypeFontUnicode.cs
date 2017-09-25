using System;
using System.IO;
using System.Text;
using System.Collections;
using System.util;

/*
 * $Id: TrueTypeFontUnicode.cs,v 1.1.1.1 2003/02/04 02:57:58 geraldhenson Exp $
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

	/** Represents a True Type font with Unicode encoding. All the character
	 * in the font can be used directly by using the encoding Identity-H or
	 * Identity-V. This is the only way to represent some character sets such
	 * as Thai.
	 * @author  Paulo Soares (psoares@consiste.pt)
	 */
	internal class TrueTypeFontUnicode : TrueTypeFont, IComparer {
    
		/** <CODE>true</CODE> if the encoding is vertical.
		 */    
		bool vertical = false;

		/** Creates a new TrueType font addressed by Unicode characters. The font
		 * will always be embedded.
		 * @param ttFile the location of the font on file. The file must end in '.ttf'.
		 * The modifiers after the name are ignored.
		 * @param enc the encoding to be applied to this font
		 * @param emb true if the font is to be embedded in the PDF
		 * @param ttfAfm the font as a <CODE>byte</CODE> array
		 * @throws DocumentException the font is invalid
		 * @throws IOException the font file could not be read
		 */
		internal TrueTypeFontUnicode(string ttFile, string enc, bool emb, byte[] ttfAfm) {
			string nameBase = getBaseName(ttFile);
			string ttcName = getTTCName(nameBase);
			if (nameBase.Length < ttFile.Length) {
				style = ttFile.Substring(nameBase.Length);
			}
			encoding = enc;
			embedded = emb;
			fileName = ttcName;
			ttcIndex = "";
			if (ttcName.Length < nameBase.Length)
				ttcIndex = nameBase.Substring(ttcName.Length + 1);
			FontType = FONT_TYPE_TTUNI;
			if ((fileName.ToLower().EndsWith(".ttf") || fileName.ToLower().EndsWith(".ttc")) && ((enc.Equals(IDENTITY_H) || enc.Equals(IDENTITY_V)) && emb)) {
				process(ttfAfm);
				if ((cmap31 == null && !fontSpecific) || (cmap10 == null && fontSpecific))
					throw new DocumentException(fileName + " " + style + " does not contain an usable cmap.");
				if (fontSpecific) {
					fontSpecific = false;
					string tempEncoding = encoding;
					encoding = PdfObject.ENCODING;
					try {
						createEncoding();
					}
					catch (Exception e) {
						throw new DocumentException(e.Message);
					}
					encoding = tempEncoding;
					fontSpecific = true;
				}
			}
			else
				throw new DocumentException(fileName + " " + style + " is not a TTF font file.");
			vertical = enc.EndsWith("V");
		}
    
		/**
		 * Gets the width of a <CODE>string</CODE> in normalized 1000 units.
		 * @param text the <CODE>string</CODE> to get the witdth of
		 * @return the width in normalized 1000 units
		 */
		public override int getWidth(string text) {
			if (vertical)
				return text.Length * 1000;
			int total = 0;
			if (fontSpecific) {
				byte[] b = PdfEncodings.convertToBytes(text, WINANSI);
				int len = b.Length;
				for (int k = 0; k < len; ++k)
					total += getRawWidth(b[k] & 0xff, null);
			}
			else {
				int len = text.Length;
				for (int k = 0; k < len; ++k)
					total += getRawWidth(text[k], encoding);
			}
			return total;
		}

		/** Creates a ToUnicode CMap to allow copy and paste from Acrobat.
		 * @param metrics metrics[0] contains the glyph index and metrics[2]
		 * contains the Unicode code
		 * @throws DocumentException on error
		 * @return the stream representing this CMap or <CODE>null</CODE>
		 */    
		private PdfStream getToUnicode(Object[] metrics) {
			if (metrics.Length == 0)
				return null;
			StringBuilder buf = new StringBuilder(
				"/CIDInit /ProcSet findresource begin\n" +
				"12 dict begin\n" +
				"begincmap\n" +
				"/CIDSystemInfo\n" +
				"<< /Registry (Adobe)\n" +
				"/Ordering (UCS)\n" +
				"/Supplement 0\n" +
				">> def\n" +
				"/CMapName /Adobe-Identity-UCS def\n" +
				"/CMapType 2 def\n" +
				"1 begincodespacerange\n" +
				toHex(((int[])metrics[0])[0]) + toHex(((int[])metrics[metrics.Length - 1])[0]) + "\n" +
				"endcodespacerange\n");
			int size = 0;
			for (int k = 0; k < metrics.Length; ++k) {
				if (size == 0) {
					if (k != 0) {
						buf.Append("endbfrange\n");
					}
					size = Math.Min(100, metrics.Length - k);
					buf.Append(size).Append(" beginbfrange\n");
				}
				--size;
				int[] metric = (int[])metrics[k];
				string fromTo = toHex(metric[0]);
				buf.Append(fromTo).Append(fromTo).Append(toHex(metric[2])).Append("\n");
			}
			buf.Append(
				"endbfrange\n" +
				"endcmap\n" +
				"CMapName currentdict /CMap defineresource pop\n" +
				"end end\n");
			string s = buf.ToString();
			PdfStream stream = new PdfStream(PdfEncodings.convertToBytes(s, null));
			stream.flateCompress();
			return stream;
		}
    
		/** Gets an hex string in the format "&lt;HHHH&gt;".
		 * @param n the number
		 * @return the hex string
		 */    
		internal static string toHex(int n) {
			string s = System.Convert.ToString(n, 16);
			return "<0000".Substring(0, 5 - s.Length) + s + ">";
		}
    
		/** Generates the CIDFontTyte2 dictionary.
		 * @param fontDescriptor the indirect reference to the font descriptor
		 * @param subsetPrefix the subset prefix
		 * @param metrics the horizontal width metrics
		 * @return a stream
		 */    
		private PdfDictionary getCIDFontType2(PdfIndirectReference fontDescriptor, string subsetPrefix, Object[] metrics) {
			PdfDictionary dic = new PdfDictionary(PdfName.FONT);
			dic.put(PdfName.SUBTYPE, new PdfName("CIDFontType2"));
			dic.put(PdfName.BASEFONT, new PdfName(subsetPrefix + fontName));
			dic.put(PdfName.FONTDESCRIPTOR, fontDescriptor);
			dic.put(new PdfName("CIDToGIDMap"),new PdfName("Identity"));
			PdfDictionary cdic = new PdfDictionary();
			cdic.put(PdfName.REGISTRY, new PdfString("Adobe"));
			cdic.put(PdfName.ORDERING, new PdfString("Identity"));
			cdic.put(PdfName.SUPPLEMENT, new PdfNumber(0));
			dic.put(new PdfName("CIDSystemInfo"), cdic);
			if (!vertical) {
				dic.put(new PdfName("DW"), new PdfNumber(1000));
				StringBuilder buf = new StringBuilder("[");
				int lastNumber = -10;
				bool firstTime = true;
				for (int k = 0; k < metrics.Length; ++k) {
					int[] metric = (int[])metrics[k];
					if (metric[1] == 1000)
						continue;
					int m = metric[0];
					if (m == lastNumber + 1) {
						buf.Append(" ").Append(metric[1]);
					}
					else {
						if (!firstTime) {
							buf.Append("]");
						}
						firstTime = false;
						buf.Append(m).Append("[").Append(metric[1]);
					}
					lastNumber = m;
				}
				if (buf.Length > 1) {
					buf.Append("]]");
					dic.put(PdfName.W, new PdfLiteral(buf.ToString()));
				}
			}
			return dic;
		}
    
		/** Generates the font dictionary.
		 * @param descendant the descendant dictionary
		 * @param subsetPrefix the subset prefix
		 * @param toUnicode the ToUnicode stream
		 * @return the stream
		 */    
		private PdfDictionary getFontBaseType(PdfIndirectReference descendant, string subsetPrefix, PdfIndirectReference toUnicode) {
			PdfDictionary dic = new PdfDictionary(PdfName.FONT);
			dic.put(PdfName.SUBTYPE, new PdfName("Type0"));
			dic.put(PdfName.BASEFONT, new PdfName(subsetPrefix + fontName));
			dic.put(PdfName.ENCODING, new PdfName(encoding));
			dic.put(new PdfName("DescendantFonts"), new PdfArray(descendant));
			if (toUnicode != null)
				dic.put(new PdfName("ToUnicode"), toUnicode);
			return dic;
		}

		/** The method used to sort the metrics array.
		 * @param o1 the first element
		 * @param o2 the second element
		 * @return the comparisation
		 */    
		public int Compare(Object o1, Object o2) {
			int m1 = ((int[])o1)[0];
			int m2 = ((int[])o2)[0];
			if (m1 < m2)
				return -1;
			if (m1 == m2)
				return 0;
			return 1;
		}

		/** Outputs to the writer the font dictionaries and streams.
		 * @param writer the writer for this document
		 * @param ref the font indirect reference
		 * @param parms several parameters that depend on the font type
		 * @throws IOException on error
		 * @throws DocumentException error in generating the object
		 */
		internal override void writeFont(PdfWriter writer, PdfIndirectReference piref, Object[] parms) {
			Hashmap longTag = (Hashmap)parms[0];
			ArrayList tmp = new ArrayList();
			foreach(object o in longTag.Values) {
				tmp.Add(o);
			}
			Object[] metrics = tmp.ToArray();
			Array.Sort(metrics, this);
			PdfIndirectReference ind_font = null;
			PdfObject pobj = null;
			PdfIndirectObject obj = null;
			TrueTypeFontSubSet sb = new TrueTypeFontSubSet(fileName, rf, longTag, directoryOffset, false);
			byte[] b = sb.process();
			int[] lengths = new int[]{b.Length};
			pobj = new StreamFont(b, lengths);
			obj = writer.addToBody(pobj);
			ind_font = obj.IndirectReference;
			string subsetPrefix = createSubsetPrefix();
			PdfDictionary dic = getFontDescriptor(ind_font, subsetPrefix);
			obj = writer.addToBody(dic);
			ind_font = obj.IndirectReference;

			pobj = getCIDFontType2(ind_font, subsetPrefix, metrics);
			obj = writer.addToBody(pobj);
			ind_font = obj.IndirectReference;

			pobj = getToUnicode(metrics);
			PdfIndirectReference toUnicodeRef = null;
			if (pobj != null) {
				obj = writer.addToBody(pobj);
				toUnicodeRef = obj.IndirectReference;
			}

			pobj = getFontBaseType(ind_font, subsetPrefix, toUnicodeRef);
			writer.addToBody(pobj, piref);
		}

		/** A forbidden operation. Will throw a null pointer exception.
		 * @param text the text
		 * @return always <CODE>null</CODE>
		 */    
		internal override byte[] convertToBytes(string text) {
			return null;
		}
	}
}