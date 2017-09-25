using System;
using System.util;
using System.Collections;
using System.Drawing;
using System.Globalization;

/*
 * $Id: MarkupParser.cs,v 1.3 2003/03/25 01:16:01 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text.markup {
	/// <summary>
	/// This class contains several static methods that can be used to parse markup.
	/// </summary>
	public class MarkupParser {
    
		/// <summary> Creates new MarkupParser </summary>
		private MarkupParser() {
		}
    
		/// <summary>
		/// This method parses a string with attributes and returns a Properties object.
		/// </summary>
		/// <param name="str">a string of this form: 'key1="value1"; key2="value2";... keyN="valueN" '</param>
		/// <returns>a Properties object</returns>
		public static Properties parseAttributes(string str) {
			Properties result = new Properties();
			if (str == null) return result;
			StringTokenizer keyValuePairs = new StringTokenizer(str, ";");
			StringTokenizer keyValuePair;
			string key;
			string value;
			while (keyValuePairs.hasMoreTokens()) {
				keyValuePair = new StringTokenizer(keyValuePairs.nextToken(), ":");
				if (keyValuePair.hasMoreTokens()) key = keyValuePair.nextToken().Trim().Trim();
				else continue;
				if (keyValuePair.hasMoreTokens()) value = keyValuePair.nextToken().Trim();
				else continue;
				if (value.StartsWith("\"")) value = value.Substring(1);
				if (value.EndsWith("\"")) value = value.Substring(0, value.Length - 1);
				result.Add(key, value);
			}
			return result;
		}

		/// <summary>
		/// This method parses the value of 'font' attribute and returns a Properties object.
		/// </summary>
		/// <param name="str">a string of this form: 'style1 ... styleN size/leading font1 ... fontN'</param>
		/// <returns>a Properties object</returns>
		public static Properties parseFont(string str) {
			Properties result = new Properties();
			if (str == null) return result;
			int pos = 0;
			string value;
			str = str.Trim();
			while (str.Length > 0) {
				pos = str.IndexOf(" ", pos);
				if (pos == -1) {
					value = str;
					str = "";
				}
				else {
					value = str.Substring(0, pos);
					str = str.Substring(pos).Trim();
				}
				if (value.ToLower().Equals("bold")) {
					result.Add(MarkupTags.CSS_FONTWEIGHT, MarkupTags.CSS_BOLD);
					continue;
				}
				if (value.ToLower().Equals("italic")) {
					result.Add(MarkupTags.CSS_FONTSTYLE, MarkupTags.CSS_ITALIC);
					continue;
				}
				if (value.ToLower().Equals("oblique")) {
					result.Add(MarkupTags.CSS_FONTSTYLE, MarkupTags.CSS_OBLIQUE);
					continue;
				}
				float f;
				if ((f = parseLength(value)) > 0) {
					result.Add(MarkupTags.CSS_FONTSIZE, f.ToString() + "pt");
					int p = value.IndexOf("/");
					if (p > -1 && p < value.Length - 1) {
						result.Add(MarkupTags.CSS_LINEHEIGHT, value.Substring(p + 1) + "pt");
					}
				}
				if (value.EndsWith(",")) {
					value = value.Substring(0, value.Length - 1);
					if (FontFactory.contains(value)) {
						result.Add(MarkupTags.CSS_FONTFAMILY, value);
						return result;
					}
				}
				if ("".Equals(str) && FontFactory.contains(value)) {
					result.Add(MarkupTags.CSS_FONTFAMILY, value);
				}
			}
			return result;
		}
    
		/// <summary>
		/// Parses a length.
		/// </summary>
		/// <param name="str">a length in the form of an optional + or -, followed by a number and a unit.</param>
		/// <returns>a float</returns>
		public static float parseLength(string str) {
			int pos = 0;
			int length = str.Length;
			bool ok = true;
			while (ok && pos < length) {
				switch(str[pos]) {
					case '+':
					case '-':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case '.':
						pos++;
						break;
					default:
						ok = false;
						break;
				}
			}
			if (pos == 0) return 0f;
			if (pos == length) return float.Parse(str);
			float f = float.Parse(str.Substring(0, pos));
			str = str.Substring(pos);
			// inches
			if (str.StartsWith("in")) {
				return f * 72f;
			}
			// centimeters
			if (str.StartsWith("cm")) {
				return (f / 2.54f) * 72f;
			}
			// millimeters
			if (str.StartsWith("mm")) {
				return (f / 25.4f) * 72f;
			}
			// picas
			if (str.StartsWith("pc")) {
				return f * 12f;
			}
			// default: we assume the length was measured in points
			return f;
		}
    
		/// <summary>
		/// Converts a <CODE>Color</CODE> into a HTML representation of this <CODE>Color</CODE>.
		/// </summary>
		/// <param name="color">the <CODE>Color</CODE> that has to be converted.</param>
		/// <returns>the HTML representation of this <CODE>Color</CODE></returns>
		public static Color decodeColor(string color) {
			int red = 0;
			int green = 0;
			int blue = 0;
			try {
				red = int.Parse(color.Substring(1, 2), NumberStyles.HexNumber);
				green = int.Parse(color.Substring(3, 2), NumberStyles.HexNumber);
				blue = int.Parse(color.Substring(5), NumberStyles.HexNumber);
			}
			catch(Exception sioobe) {
				// empty on purpose
				sioobe.GetType();
			}
			return new Color(red, green, blue);
		}
	}
}
