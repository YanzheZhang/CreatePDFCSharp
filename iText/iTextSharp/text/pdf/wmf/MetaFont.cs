using System;

/*
 * $Id: MetaFont.cs,v 1.1.1.1 2003/02/04 02:58:46 geraldhenson Exp $
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

namespace iTextSharp.text.pdf.wmf {
	public class MetaFont : MetaObject {
		static string[] fontNames = {
										"Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique",
										"Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique",
										"Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic",
										"Symbol", "ZapfDingbats"};

		internal const int MARKER_BOLD = 1;
		internal const int MARKER_ITALIC = 2;
		internal const int MARKER_COURIER = 0;
		internal const int MARKER_HELVETICA = 4;
		internal const int MARKER_TIMES = 8;
		internal const int MARKER_SYMBOL = 12;

		internal const int DEFAULT_PITCH = 0;
		internal const int FIXED_PITCH = 1;
		internal const int VARIABLE_PITCH = 2;
		internal const int FF_DONTCARE = 0;
		internal const int FF_ROMAN = 1;
		internal const int FF_SWISS = 2;
		internal const int FF_MODERN = 3;
		internal const int FF_SCRIPT = 4;
		internal const int FF_DECORATIVE = 5;
		internal const int BOLDTHRESHOLD = 600;    
		internal const int nameSize = 32;
		internal const int ETO_OPAQUE = 2;
		internal const int ETO_CLIPPED = 4;

		int height;
		float angle;
		int bold;
		int italic;
		bool underline;
		bool strikeout;
		int charset;
		int pitchAndFamily;
		string faceName;
		BaseFont font = null;

		public MetaFont() {
			type = META_FONT;
		}

		public void init(InputMeta meta) {
			height = Math.Abs(meta.readShort());
			meta.skip(2);
			angle = (float)(meta.readShort() / 1800.0 * Math.PI);
			meta.skip(2);
			bold = (meta.readShort() >= BOLDTHRESHOLD ? MARKER_BOLD : 0);
			italic = (meta.readByte() != 0 ? MARKER_ITALIC : 0);
			underline = (meta.readByte() != 0);
			strikeout = (meta.readByte() != 0);
			charset = meta.readByte();
			meta.skip(3);
			pitchAndFamily = meta.readByte();
			byte[] name = new byte[nameSize];
			int k;
			for (k = 0; k < nameSize; ++k) {
				int c = meta.readByte();
				if (c == 0) {
					break;
				}
				name[k] = (byte)c;
			}
			try {
				faceName = System.Text.Encoding.GetEncoding("windows-1252").GetString(name, 0, k);
			}
			catch (Exception e) {
				e.GetType();
				faceName = System.Text.ASCIIEncoding.ASCII.GetString(name, 0, k);
			}
			faceName = faceName.ToLower();
		}
    
		public BaseFont Font {
			get {
				if (font != null)
					return font;
				string fontName;
				if (faceName.IndexOf("courier") != -1 || faceName.IndexOf("terminal") != -1
					|| faceName.IndexOf("fixedsys") != -1) {
					fontName = fontNames[MARKER_COURIER + italic + bold];
				}
				else if (faceName.IndexOf("ms sans serif") != -1 || faceName.IndexOf("arial") != -1
					|| faceName.IndexOf("system") != -1) {
					fontName = fontNames[MARKER_HELVETICA + italic + bold];
				}
				else if (faceName.IndexOf("arial black") != -1) {
					fontName = fontNames[MARKER_HELVETICA + italic + MARKER_BOLD];
				}
				else if (faceName.IndexOf("times") != -1 || faceName.IndexOf("ms serif") != -1
					|| faceName.IndexOf("roman") != -1) {
					fontName = fontNames[MARKER_TIMES + italic + bold];
				}
				else if (faceName.IndexOf("symbol") != -1) {
					fontName = fontNames[MARKER_SYMBOL];
				}
				else {
					int pitch = pitchAndFamily & 3;
					int family = (pitchAndFamily >> 4) & 7;
					switch (family) {
						case FF_MODERN:
							fontName = fontNames[MARKER_COURIER + italic + bold];
							break;
						case FF_ROMAN:
							fontName = fontNames[MARKER_TIMES + italic + bold];
							break;
						case FF_SWISS:
						case FF_SCRIPT:
						case FF_DECORATIVE:
							fontName = fontNames[MARKER_HELVETICA + italic + bold];
							break;
						default: {
							switch (pitch) {
								case FIXED_PITCH:
									fontName = fontNames[MARKER_COURIER + italic + bold];
									break;
								default:
									fontName = fontNames[MARKER_HELVETICA + italic + bold];
									break;
							}
							break;
						}
					}
				}
				try {
					font = BaseFont.createFont(fontName, "windows-1252", false);
				}
				catch (Exception e) {
					throw e;
				}
        
				return font;
			}
		}
    
		public float Angle {
			get {
				return angle;
			}
		}
    
		public bool isUnderline() {
			return underline;
		}
    
		public bool isStrikeout() {
			return strikeout;
		}
    
		public float getFontSize(MetaState state) {
			return Math.Abs(state.transformY(height) - state.transformY(0)) * 0.86f;
		}
	}
}
