using System;
using System.Collections;
using System.util;
using System.Drawing;

using iTextSharp.text.markup;
using iTextSharp.text.pdf;
using iTextSharp.text;

/*
 * $Id: FontFactory.cs,v 1.3 2003/03/26 01:00:36 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2002 by Bruno Lowagie.
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
	/// If you are using True Type fonts, you can declare the paths of the different ttf- and ttc-files
	/// to this static class first and then create fonts in your code using one of the static getFont-method
	/// without having to enter a path as parameter.
	/// </summary>
	public class FontFactory {
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string COURIER = BaseFont.COURIER;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string COURIER_BOLD = BaseFont.COURIER_BOLD;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string COURIER_OBLIQUE = BaseFont.COURIER_OBLIQUE;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string COURIER_BOLDOBLIQUE = BaseFont.COURIER_BOLDOBLIQUE;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string HELVETICA = BaseFont.HELVETICA;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string HELVETICA_BOLD = BaseFont.HELVETICA_BOLD;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string HELVETICA_OBLIQUE = BaseFont.HELVETICA_OBLIQUE;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string HELVETICA_BOLDOBLIQUE = BaseFont.HELVETICA_BOLDOBLIQUE;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string SYMBOL = BaseFont.SYMBOL;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string TIMES_NEW_ROMAN = "Times New Roman";
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string TIMES = "Times";
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string TIMES_ROMAN = BaseFont.TIMES_ROMAN;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string TIMES_BOLD = BaseFont.TIMES_BOLD;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string TIMES_ITALIC = BaseFont.TIMES_ITALIC;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string TIMES_BOLDITALIC = BaseFont.TIMES_BOLDITALIC;
    
		/// <summary> This is a possible value of a base 14 type 1 font </summary>
		public static string ZAPFDINGBATS = BaseFont.ZAPFDINGBATS;
    
		/// <summary> This is a map of postscriptfontnames of True Type fonts and the path of their ttf- or ttc-file. </summary>
		private static Properties trueTypeFonts = new Properties();
    
		/// <summary>
		/// Static Constructor
		/// </summary>
		static FontFactory() {
			trueTypeFonts.Add(COURIER, COURIER);
			trueTypeFonts.Add(COURIER_BOLD, COURIER_BOLD);
			trueTypeFonts.Add(COURIER_OBLIQUE, COURIER_OBLIQUE);
			trueTypeFonts.Add(COURIER_BOLDOBLIQUE, COURIER_BOLDOBLIQUE);
			trueTypeFonts.Add(HELVETICA, HELVETICA);
			trueTypeFonts.Add(HELVETICA_BOLD, HELVETICA_BOLD);
			trueTypeFonts.Add(HELVETICA_OBLIQUE, HELVETICA_OBLIQUE);
			trueTypeFonts.Add(HELVETICA_BOLDOBLIQUE, HELVETICA_BOLDOBLIQUE);
			trueTypeFonts.Add(SYMBOL, SYMBOL);
			trueTypeFonts.Add(TIMES_ROMAN, TIMES_ROMAN);
			trueTypeFonts.Add(TIMES_BOLD, TIMES_BOLD);
			trueTypeFonts.Add(TIMES_ITALIC, TIMES_ITALIC);
			trueTypeFonts.Add(TIMES_BOLDITALIC, TIMES_BOLDITALIC);
			trueTypeFonts.Add(ZAPFDINGBATS, ZAPFDINGBATS);

			ArrayList tmp;
			tmp = new ArrayList();
			tmp.Add(COURIER);
			tmp.Add(COURIER_BOLD);
			tmp.Add(COURIER_OBLIQUE);
			tmp.Add(COURIER_BOLDOBLIQUE);
			fontFamilies.Add(COURIER, tmp);
			tmp = new ArrayList();
			tmp.Add(HELVETICA);
			tmp.Add(HELVETICA_BOLD);
			tmp.Add(HELVETICA_OBLIQUE);
			tmp.Add(HELVETICA_BOLDOBLIQUE);
			fontFamilies.Add(HELVETICA, tmp);
			tmp = new ArrayList();
			tmp.Add(SYMBOL);
			fontFamilies.Add(SYMBOL, tmp);
			tmp = new ArrayList();
			tmp.Add(TIMES_ROMAN);
			tmp.Add(TIMES_BOLD);
			tmp.Add(TIMES_ITALIC);
			tmp.Add(TIMES_BOLDITALIC);
			fontFamilies.Add(TIMES_NEW_ROMAN, tmp);
			fontFamilies.Add(TIMES, tmp);
			tmp = new ArrayList();
			tmp.Add(ZAPFDINGBATS);
			fontFamilies.Add(ZAPFDINGBATS, tmp);
		}
    
		/// <summary> This is a map of fontfamilies. </summary>
		private static Hashmap fontFamilies = new Hashmap();
    
		/// <summary> This is the default encoding to use. </summary>
		public static string defaultEncoding = BaseFont.WINANSI;
    
		/// <summary> This is the default value of the <VAR>embedded</VAR> variable. </summary>
		public static bool defaultEmbedding = BaseFont.NOT_EMBEDDED;
    
		/// <summary> Creates new FontFactory </summary>
		private FontFactory() {
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="encoding">the encoding of the font</param>
		/// <param name="embedded">true if the font is to be embedded in the PDF</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		/// <param name="color">the Color of this font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, string encoding, bool embedded, float size, int style, Color color) {
			if (fontname == null) return new Font(Font.UNDEFINED, size, style, color);
			ArrayList tmp = (ArrayList) fontFamilies[fontname];
			if (tmp != null) {
				// some bugs were fixed here by Daniel Marczisovszky
				string lowercasefontname = fontname.ToLower();
				int s = style == Font.UNDEFINED ? Font.NORMAL : style;
				foreach(string f in tmp) {
					string lcf = f.ToLower();
					int fs = Font.NORMAL;
					if (lcf.ToLower().IndexOf("bold") != -1) fs |= Font.BOLD;
					if (lcf.ToLower().IndexOf("italic") != -1 || lcf.ToLower().IndexOf("oblique") != -1) fs |= Font.ITALIC;
					if ((s & Font.BOLDITALIC) == fs && lcf.IndexOf(lowercasefontname) != -1) {
						fontname = f;
						break;
					}
				}
			}
			BaseFont basefont = null;
			try {
				try {
					// the font is a type 1 font or CJK font
					basefont = BaseFont.createFont(fontname, encoding, embedded);
				}
				catch(DocumentException de) {
					de.GetType();
					// the font is a true type font or an unknown font
					fontname = trueTypeFonts[fontname];
					// the font is not registered as truetype font
					if (fontname == null) return new Font(Font.UNDEFINED, size, style, color);
					// the font is registered as truetype font
					basefont = BaseFont.createFont(fontname, encoding, embedded);
				}
			}
			catch(DocumentException de) {
				// this shouldn't happen
				throw de;
			}
			catch(System.IO.IOException ioe) {
				ioe.GetType();
				// the font is registered as a true type font, but the path was wrong
				return new Font(Font.UNDEFINED, size, style, color);
			}
			catch(Exception npe) {
				npe.GetType();
				// null was entered as fontname and/or encoding
				return new Font(Font.UNDEFINED, size, style, color);
			}
			return new Font(basefont, size, style, color);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="attributes">the attributes of a Font object</param>
		/// <returns>a Font object</returns>
		public static Font getFont(Properties attributes) {
			string fontname = null;
			string encoding = defaultEncoding;
			bool embedded = defaultEmbedding;
			float size = Font.UNDEFINED;
			int style = Font.NORMAL;
			Color color = null;
			string value = attributes.Remove(MarkupTags.STYLE);
			if (value != null && value.Length > 0) {
				Properties styleAttributes = MarkupParser.parseAttributes(value);
				if (styleAttributes.Count == 0) {
					attributes.Add(MarkupTags.STYLE, value);
				}
				else {
					fontname = (string)styleAttributes.Remove(MarkupTags.CSS_FONTFAMILY);
					if (fontname != null) {
						string tmp;
						while (fontname.IndexOf(",") != -1) {
							tmp = fontname.Substring(0, fontname.IndexOf(","));
							if (isRegistered(tmp)) {
								fontname = tmp;
							}
							else {
								fontname = fontname.Substring(fontname.IndexOf(",") + 1);
							}
						}
					}
					if ((value = (string)styleAttributes.Remove(MarkupTags.CSS_FONTSIZE)) != null) {
						size = MarkupParser.parseLength(value);
					}
					if ((value = (string)styleAttributes.Remove(MarkupTags.CSS_FONTWEIGHT)) != null) {
						style |= Font.getStyleValue(value);
					}
					if ((value = (string)styleAttributes.Remove(MarkupTags.CSS_FONTSTYLE)) != null) {
						style |= Font.getStyleValue(value);
					}
					if ((value = (string)styleAttributes.Remove(MarkupTags.CSS_COLOR)) != null) {
						color = MarkupParser.decodeColor(value);
					}
					attributes.AddAll(styleAttributes);
				}
			}
			if ((value = attributes.Remove(ElementTags.ENCODING)) != null) {
				encoding = value;
			}
			if ("false".Equals(attributes.Remove(ElementTags.EMBEDDED))) {
				embedded = false;
			}
			if ((value = attributes.Remove(ElementTags.FONT)) != null) {
				fontname = value;
			}
			if ((value = attributes.Remove(ElementTags.SIZE)) != null) {
				size = float.Parse(value);
			}
			if ((value = attributes.Remove(MarkupTags.STYLE)) != null) {
				style |= Font.getStyleValue(value);
			}
			if ((value = attributes.Remove(ElementTags.STYLE)) != null) {
				style |= Font.getStyleValue(value);
			}
			string r = attributes.Remove(ElementTags.RED);
			string g = attributes.Remove(ElementTags.GREEN);
			string b = attributes.Remove(ElementTags.BLUE);
			if (r != null || g != null || b != null) {
				int red = 0;
				int green = 0;
				int blue = 0;
				if (r != null) red = int.Parse(r);
				if (g != null) green = int.Parse(g);
				if (b != null) blue = int.Parse(b);
				color = new Color(red, green, blue);
			}
			else if ((value = attributes.Remove(ElementTags.COLOR)) != null) {
				color = MarkupParser.decodeColor(value);
			}
			if (fontname == null) {
				return getFont(null, encoding, embedded, size, style, color);
			}
			return getFont(fontname, encoding, embedded, size, style, color);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="encoding">the encoding of the font</param>
		/// <param name="embedded">true if the font is to be embedded in the PDF</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, string encoding, bool embedded, float size, int style) {
			return getFont(fontname, encoding, embedded, size, style, null);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="encoding">the encoding of the font</param>
		/// <param name="embedded">true if the font is to be embedded in the PDF</param>
		/// <param name="size">the size of this font</param>
		/// <returns></returns>
		public static Font getFont(string fontname, string encoding, bool embedded, float size) {
			return getFont(fontname, encoding, embedded, size, Font.UNDEFINED, null);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="encoding">the encoding of the font</param>
		/// <param name="embedded">true if the font is to be embedded in the PDF</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, string encoding, bool embedded) {
			return getFont(fontname, encoding, embedded, Font.UNDEFINED, Font.UNDEFINED, null);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="encoding">the encoding of the font</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		/// <param name="color">the Color of this font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, string encoding, float size, int style, Color color) {
			return getFont(fontname, encoding, defaultEmbedding, size, style, color);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="encoding">the encoding of the font</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, string encoding, float size, int style) {
			return getFont(fontname, encoding, defaultEmbedding, size, style, null);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="encoding">the encoding of the font</param>
		/// <param name="size">the size of this font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, string encoding, float size) {
			return getFont(fontname, encoding, defaultEmbedding, size, Font.UNDEFINED, null);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="encoding">the encoding of the font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, string encoding) {
			return getFont(fontname, encoding, defaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		/// <param name="color">the Color of this font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, float size, int style, Color color) {
			return getFont(fontname, defaultEncoding, defaultEmbedding, size, style, color);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="size">the size of this font</param>
		/// <param name="style">the style of this font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, float size, int style) {
			return getFont(fontname, defaultEncoding, defaultEmbedding, size, style, null);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <param name="size">the size of this font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname, float size) {
			return getFont(fontname, defaultEncoding, defaultEmbedding, size, Font.UNDEFINED, null);
		}
    
		/// <summary>
		/// Constructs a Font-object.
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <returns>a Font object</returns>
		public static Font getFont(string fontname) {
			return getFont(fontname, defaultEncoding, defaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);
		}

		public static void register(Properties attributes) {
			string path;
			string alias = null;

			path = attributes.Remove("path");
			alias = attributes.Remove("alias");

			register(path, alias);
		}
    
		/// <summary>
		/// Register a ttf- or a ttc-file.
		/// </summary>
		/// <param name="path">the path to a ttf- or ttc-file</param>
		public static void register(string path) {
			register(path, null);
		}
    
		/// <summary>
		/// Register a ttf- or a ttc-file and use an alias for the font contained in the ttf-file.
		/// </summary>
		/// <param name="path">the path to a ttf- or ttc-file</param>
		/// <param name="alias">the alias you want to use for the font</param>
		public static void register(string path, string alias) {
			try {
				if (path.ToLower().EndsWith(".ttf")) {
					BaseFont bf = BaseFont.createFont(path, BaseFont.WINANSI, false, false, null, null);
					trueTypeFonts.Add(bf.PostscriptFontName, path);
					if (alias != null) {
						trueTypeFonts.Add(alias, path);
					}
					string fullName = null;
					string familyName = null;
					string[][] names = bf.FullFontName;
					for (int i = 0; i < names.Length; i++) {
						if ("0".Equals(names[i][2])) {
							fullName = names[i][3];
							trueTypeFonts.Add(fullName, path);
							break;
						}
					}
					if (fullName != null) {
						names = bf.FamilyFontName;
						for (int i = 0; i < names.Length; i++) {
							if ("0".Equals(names[i][2])) {
								familyName = names[i][3];
								ArrayList tmp = (ArrayList) fontFamilies[familyName];
								if (tmp == null) {
									tmp = new ArrayList();
								}
								tmp.Add(fullName);
								fontFamilies.Add(familyName, tmp);
								break;
							}
						}
					}
				}
				else if (path.ToLower().EndsWith(".ttc")) {
					string[] names = BaseFont.enumerateTTCNames(path);
					for (int i = 0; i < names.Length; i++) {
						trueTypeFonts.Add(names[i], path + "," + (i + 1));
					}
					if (alias != null) {
						Console.Error.WriteLine("class FontFactory: You can't define an alias for a true type collection.");
					}
				}
			}
			catch(DocumentException de) {
				// this shouldn't happen
				throw de;
			}
			catch(System.IO.IOException ioe) {
				throw ioe;
			}
		}
    
		/// <summary>
		/// Gets a set of registered fontnames.
		/// </summary>
		/// <value>a set of registered fontnames</value>
		public static ICollection RegisteredFonts {
			get {
				return Chunk.getKeySet(trueTypeFonts);
			}
		}
    
		/// <summary>
		/// Gets a set of registered font families.
		/// </summary>
		/// <value>a set of registered font families</value>
		public static ICollection RegisteredFamilies {
			get {
				return Chunk.getKeySet(fontFamilies);
			}
		}
    
		/// <summary>
		/// Checks whether the given font is contained within the object
		/// </summary>
		/// <param name="fontname">the name of the font</param>
		/// <returns>true if font is contained within the object</returns>
		public static bool contains(string fontname) {
			return trueTypeFonts.ContainsKey(fontname);
		}
    
		/// <summary>
		/// Checks if a certain font is registered.
		/// </summary>
		/// <param name="fontname">the name of the font that has to be checked</param>
		/// <returns>true if the font is found</returns>
		public static bool isRegistered(string fontname) {
			foreach(string key in trueTypeFonts) {
				if (fontname.ToLower().Equals(trueTypeFonts[key])) {
					return true;
				}
			}
			return false;
		}
	}
}
