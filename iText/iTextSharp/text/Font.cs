using System;
using System.Drawing;

using iTextSharp.text.pdf;
using iTextSharp.text.markup;

/*
 * $Id: Font.cs,v 1.2 2003/03/12 20:10:15 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text {
	/// <summary>
	/// Contains all the specifications of a font: fontfamily, size, style and color.
	/// </summary>
	/// <example>
	/// <code>
	/// Paragraph p = new Paragraph("This is a paragraph",
	///               <strong>new Font(Font.HELVETICA, 18, Font.BOLDITALIC, new Color(0, 0, 255))</strong>);
	/// </code>
	/// </example>
	public class Font : IComparable {
    
		// static membervariables for the different families
    
		/// <summary> a possible value of a font family. </summary>
		public const int COURIER = 0;
    
		/// <summary> a possible value of a font family. </summary>
		public const int HELVETICA = 1;
    
		/// <summary> a possible value of a font family. </summary>
		public const int TIMES_NEW_ROMAN = 2;
    
		/// <summary> a possible value of a font family. </summary>
		public const int SYMBOL = 3;
    
		/// <summary> a possible value of a font family. </summary>
		public const int ZAPFDINGBATS = 4;
    
		// static membervariables for the different styles
    
		/// <summary> this is a possible style. </summary>
		public const int NORMAL		= 0;
    
		/// <summary> this is a possible style. </summary>
		public const int BOLD		= 1;
    
		/// <summary> this is a possible style. </summary>
		public const int ITALIC		= 2;
    
		/// <summary> this is a possible style. </summary>
		public const int UNDERLINE	= 4;
    
		/// <summary> this is a possible style. </summary>
		public const int STRIKETHRU	= 8;
    
		/// <summary> this is a possible style. </summary>
		public const int BOLDITALIC	= BOLD | ITALIC;
    
		// static membervariables
    
		/// <summary> the value of an undefined attribute. </summary>
		public const int UNDEFINED = -1;
    
		/// <summary> the value of the default size. </summary>
		public const int DEFAULTSIZE = 12;
    
		// membervariables
    
		/// <summary> the value of the fontfamily. </summary>
		private int family = UNDEFINED;
    
		/// <summary> the value of the fontsize. </summary>
		private float size = UNDEFINED;
    
		/// <summary> the value of the style. </summary>
		private int style = UNDEFINED;
    
		/// <summary> the value of the color. </summary>
		private Color color;
    
		/// <summary> the external font </summary>
		private BaseFont baseFont = null;
    
		// constructors
    
		/// <summary>
		/// Constructs a Font.
		/// </summary>
		/// <param name="family">the family to which this font belongs</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		/// <param name="color">the Color of this font.</param>
		public Font(int family, float size, int style, Color color) {
			this.family = family;
			this.size = size;
			this.style = style;
			this.color = color;
		}
    
		/// <summary>
		/// Constructs a Font.
		/// </summary>
		/// <param name="bf">the external font</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		/// <param name="color">the Color of this font.</param>
		public Font(BaseFont bf, float size, int style, Color color) {
			this.baseFont = bf;
			this.size = size;
			this.style = style;
			this.color = color;
		}
    
		/// <summary>
		/// Constructs a Font.
		/// </summary>
		/// <param name="bf">the external font</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		public Font(BaseFont bf, float size, int style) : this(bf, size, style, null) {}
    
		/// <summary>
		/// Constructs a Font.
		/// </summary>
		/// <param name="bf">the external font</param>
		/// <param name="size">the size of this font</param>
		public Font(BaseFont bf, float size) : this(bf, size, UNDEFINED, null) {}
    
		/// <summary>
		/// Constructs a Font.
		/// </summary>
		/// <param name="bf">the external font</param>
		public Font(BaseFont bf) : this(bf, UNDEFINED, UNDEFINED, null) {}
    
		/// <summary>
		/// Constructs a Font.
		/// </summary>
		/// <param name="family">the family to which this font belongs</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		public Font(int family, float size, int style) : this(family, size, style, null) {}
    
		/// <summary>
		/// Constructs a Font.
		/// </summary>
		/// <param name="family">the family to which this font belongs</param>
		/// <param name="size">the size of this font</param>
		public Font(int family, float size) : this(family, size, UNDEFINED, null) {}
    
		/// <summary>
		/// Constructs a Font.
		/// </summary>
		/// <param name="family">the family to which this font belongs</param>
		public Font(int family) : this(family, UNDEFINED, UNDEFINED, null) {}
    
		/// <summary>
		/// Constructs a Font.
		/// </summary>
		/// <overloads>
		/// Has nine overloads.
		/// </overloads>
		public Font() : this(UNDEFINED, UNDEFINED, UNDEFINED, null) {}
    
		// implementation of the Comparable interface
    
		/// <summary>
		/// Compares this Font with another
		/// </summary>
		/// <param name="obj">the other Font</param>
		/// <returns>a value</returns>
		public int CompareTo(Object obj) {
			if (obj == null) {
				return -1;
			}
			Font font;
			try {
				font = (Font) obj;
				if (baseFont != null && !baseFont.Equals(font.BaseFont)) {
					return -2;
				}
				if (this.family != font.Family) {
					return 1;
				}
				if (this.size != font.Size) {
					return 2;
				}
				if (this.style != font.Style) {
					return 3;
				}
				if (this.color == null) {
					if (font.Color == null) {
						return 0;
					}
					return 4;
				}
				if (font.Color == null) {
					return 4;
				}
				if (((Color)this.color).Equals(font.Color)) {
					return 0;
				}
				return 4;
			}
			catch(Exception cce) {
				cce.GetType();
				return -3;
			}
		}
    
		// methods
    
		/// <summary>
		/// Translates a string-value of a certain family
		/// into the index that is used for this family in this class.
		/// </summary>
		/// <param name="family">A string representing a certain font-family</param>
		/// <returns>the corresponding index</returns>
		public static int getFamilyIndex(string family) {
			family = family.ToLower();
			if (family.ToLower().Equals(FontFactory.COURIER)) {
				return COURIER;
			}
			if (family.ToLower().Equals(FontFactory.HELVETICA)) {
				return HELVETICA;
			}
			if (family.ToLower().Equals(FontFactory.TIMES_ROMAN) || family.ToLower().Equals(FontFactory.TIMES_NEW_ROMAN)) {
				return TIMES_NEW_ROMAN;
			}
			if (family.ToLower().Equals(FontFactory.SYMBOL)) {
				return SYMBOL;
			}
			if (family.ToLower().Equals(FontFactory.ZAPFDINGBATS)) {
				return ZAPFDINGBATS;
			}
			return UNDEFINED;
		}
    
		/// <summary>
		/// Gets the familyname as a string.
		/// </summary>
		/// <value>the familyname</value>
		public string Familyname {
			get {
				string tmp = "unknown";
				switch(this.Family) {
					case Font.COURIER:
						return FontFactory.COURIER;
					case Font.HELVETICA:
						return FontFactory.HELVETICA;
					case Font.TIMES_NEW_ROMAN:
						return FontFactory.TIMES_NEW_ROMAN;
					case Font.SYMBOL:
						return FontFactory.SYMBOL;
					case Font.ZAPFDINGBATS:
						return FontFactory.ZAPFDINGBATS;
					default:
						if (baseFont != null) {
							string[][] names = baseFont.FamilyFontName;
							for (int i = 0; i < names.Length; i++) {
								if ("0".Equals(names[i][2])) {
									return names[i][3];
								}
								if ("1033".Equals(names[i][2])) {
									tmp = names[i][3];
								}
								if ("".Equals(names[i][2])) {
									tmp = names[i][3];
								}
							}
						}
						break;
				}
				return tmp;
			}
		}
    
		/// <summary>
		/// Translates a string-value of a certain style
		/// into the index value is used for this style in this class.
		/// </summary>
		/// <param name="style">a string</param>
		/// <returns>the corresponding value</returns>
		public static int getStyleValue(string style) {
			int s = 0;
			if (style.IndexOf(MarkupTags.CSS_NORMAL) != -1) {
				s |= NORMAL;
			}
			if (style.IndexOf(MarkupTags.CSS_BOLD) != -1) {
				s |= BOLD;
			}
			if (style.IndexOf(MarkupTags.CSS_ITALIC) != -1) {
				s |= ITALIC;
			}
			if (style.IndexOf(MarkupTags.CSS_OBLIQUE) != -1) {
				s |= ITALIC;
			}
			if (style.IndexOf(MarkupTags.CSS_UNDERLINE) != -1) {
				s |= UNDERLINE;
			}
			if (style.IndexOf(MarkupTags.CSS_LINETHROUGH) != -1) {
				s |= STRIKETHRU;
			}
			return s;
		}
    
    
		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="red">the red-value of the new color</param>
		/// <param name="green">the green-value of the new color</param>
		/// <param name="blue">the blue-value of the new color</param>
		public void setColor(int red, int green, int blue) {
			this.color = new Color(System.Drawing.Color.FromArgb(red, green, blue));
		}
    
		/// <summary>
		/// Gets the leading that can be used with this font.
		/// </summary>
		/// <param name="linespacing">a certain linespacing</param>
		/// <returns>the height of a line</returns>
		public float leading(float linespacing) {
			if (size == UNDEFINED) {
				return linespacing * DEFAULTSIZE;
			}
			return linespacing * size;
		}
    
		/// <summary>
		/// Checks if the properties of this font are undefined or null.
		/// <p/>
		/// If so, the standard should be used.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isStandardFont() {
			return (family == UNDEFINED
				&& size == UNDEFINED
				&& style == UNDEFINED
				&& color == null
				&& baseFont == null);
		}
    
		/// <summary>
		/// Replaces the attributes that are equal to null with
		/// the attributes of a given font.
		/// </summary>
		/// <param name="font">the font of a bigger element class</param>
		/// <returns>a Font</returns>
		public Font difference(Font font) {
			// size
			float dSize = font.size;
			if (dSize == UNDEFINED) {
				dSize = this.size;
			}
			// style
			int dStyle = UNDEFINED;
			int style1 = this.Style;
			int style2 = font.Style;
			if (style1 != UNDEFINED || style2 != UNDEFINED) {
				if (style1 == UNDEFINED) style1 = 0;
				if (style2 == UNDEFINED) style2 = 0;
				dStyle = style1 | style2;
			}
			// color
			object dColor = (Color)font.Color;
			if (dColor == null) {
				dColor = this.Color;
			}
			// family
			if (font.baseFont != null) {
				return new Font(font.BaseFont, dSize, dStyle, (Color)dColor);
			}
			if (font.Family != UNDEFINED) {
				return new Font(font.Family, dSize, dStyle, (Color)dColor);
			}
			if (this.baseFont != null) {
				if (dStyle == style1) {
					return new Font(this.BaseFont, dSize, dStyle, (Color)dColor);
				}
				else {
					return FontFactory.getFont(this.Familyname, dSize, dStyle, (Color)dColor);
				}
			}
			return new Font(this.Family, dSize, dStyle, (Color)dColor);
		}
    
		// methods to retrieve the membervariables
    
		/// <summary>
		/// Gets the family of this font.
		/// </summary>
		/// <value>the value of the family</value>
		public int Family {
			get {
				return family;
			}
		}
    
		/// <summary>
		/// Sets the family using a String ("Courier",
		/// "Helvetica", "Times New Roman", "Symbol" or "ZapfDingbats").
		/// </summary>
		/// <param name="family">A String representing a certain font-family.</param>
		public void setFamily(String family) {
			this.family = getFamilyIndex(family);
		}

		/// <summary>
		/// Get/set the size of this font.
		/// </summary>
		/// <value>the size of this font</value>
		public float Size {
			get {
				return size;
			}

			set {
				this.size = value;
			}
		}
    
		/// <summary>
		/// Gets the style of this font.
		/// </summary>
		/// <value>the style of this font</value>
		public int Style {
			get {
				return style;
			}
		}
    
		/// <summary>
		/// Sets the style using a String containing one of
		/// more of the following values: normal, bold, italic, underline, strike.
		/// </summary>
		/// <param name="style">A String representing a certain style.</param>
		public void setStyle(String style) {
			if (this.style == UNDEFINED) this.style = NORMAL;
			this.style |= getStyleValue(style);
		}

		/// <summary>
		/// checks if this font is Bold.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isBold() {
			if (style == UNDEFINED) {
				return false;
			}
			return (style &	BOLD) == BOLD;
		}
    
		/// <summary>
		/// checks if this font is Bold.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isItalic() {
			if (style == UNDEFINED) {
				return false;
			}
			return (style &	ITALIC) == ITALIC;
		}
    
		/// <summary>
		/// checks if this font is underlined.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isUnderlined() {
			if (style == UNDEFINED) {
				return false;
			}
			return (style &	UNDERLINE) == UNDERLINE;
		}
    
		/// <summary>
		/// checks if the style of this font is STRIKETHRU.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isStrikethru() {
			if (style == UNDEFINED) {
				return false;
			}
			return (style &	STRIKETHRU) == STRIKETHRU;
		}
    
		/// <summary>
		/// Get/set the color of this font.
		/// </summary>
		/// <value>the color of this font</value>
		public Color Color {
			get {
				if (this.color != null) {
					return (Color)color;
				} else {
					return null;
				}
			}

			set {
				this.color = value;
			}
		}
    
		/// <summary>
		/// Gets the BaseFont inside this object.
		/// </summary>
		/// <value>the BaseFont</value>
		public BaseFont BaseFont {
			get {
				return baseFont;
			}
		}
	}
}
