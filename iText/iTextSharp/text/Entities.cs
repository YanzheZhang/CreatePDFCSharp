using System;
using System.Collections;
using System.util;

/*
 * $Id: Entities.cs,v 1.2 2003/03/12 20:10:14 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie.
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
	/// This class contains entities that can be used in an entity tag.
	/// </summary>
	public class Entities {
    
		/// <summary> This is a map that contains all possible id values of the entity tag. </summary>
		public static Hashmap map;
    
		/// <summary>
		/// Static Constructor
		/// </summary>
		static Entities() {
			map = new Hashmap();
			map.Add("169", 227);
			map.Add("172", 216);
			map.Add("174", 210);
			map.Add("177", 177);
			map.Add("215", 180);
			map.Add("247", 184);
			map.Add("8230", 188);
			map.Add("8242", 162);
			map.Add("8243", 178);
			map.Add("8260", 164);
			map.Add("8364", 240);
			map.Add("8465", 193);
			map.Add("8472", 195);
			map.Add("8476", 194);
			map.Add("8482", 212);
			map.Add("8501", 192);
			map.Add("8592", 172);
			map.Add("8593", 173);
			map.Add("8594", 174);
			map.Add("8595", 175);
			map.Add("8596", 171);
			map.Add("8629", 191);
			map.Add("8656", 220);
			map.Add("8657", 221);
			map.Add("8658", 222);
			map.Add("8659", 223);
			map.Add("8660", 219);
			map.Add("8704", 34);
			map.Add("8706", 182);
			map.Add("8707", 36);
			map.Add("8709", 198);
			map.Add("8711", 209);
			map.Add("8712", 206);
			map.Add("8713", 207);
			map.Add("8717", 39);
			map.Add("8719", 213);
			map.Add("8721", 229);
			map.Add("8722", 45);
			map.Add("8727", 42);
			map.Add("8729", 183);
			map.Add("8730", 214);
			map.Add("8733", 181);
			map.Add("8734", 165);
			map.Add("8736", 208);
			map.Add("8743", 217);
			map.Add("8744", 218);
			map.Add("8745", 199);
			map.Add("8746", 200);
			map.Add("8747", 242);
			map.Add("8756", 92);
			map.Add("8764", 126);
			map.Add("8773", 64);
			map.Add("8776", 187);
			map.Add("8800", 185);
			map.Add("8801", 186);
			map.Add("8804", 163);
			map.Add("8805", 179);
			map.Add("8834", 204);
			map.Add("8835", 201);
			map.Add("8836", 203);
			map.Add("8838", 205);
			map.Add("8839", 202);
			map.Add("8853", 197);
			map.Add("8855", 196);
			map.Add("8869", 94);
			map.Add("8901", 215);
			map.Add("8992", 243);
			map.Add("8993", 245);
			map.Add("9001", 225);
			map.Add("9002", 241);
			map.Add("913", 65);
			map.Add("914", 66);
			map.Add("915", 71);
			map.Add("916", 68);
			map.Add("917", 69);
			map.Add("918", 90);
			map.Add("919", 72);
			map.Add("920", 81);
			map.Add("921", 73);
			map.Add("922", 75);
			map.Add("923", 76);
			map.Add("924", 77);
			map.Add("925", 78);
			map.Add("926", 88);
			map.Add("927", 79);
			map.Add("928", 80);
			map.Add("929", 82);
			map.Add("931", 83);
			map.Add("932", 84);
			map.Add("933", 85);
			map.Add("934", 70);
			map.Add("935", 67);
			map.Add("936", 89);
			map.Add("937", 87);
			map.Add("945", 97);
			map.Add("946", 98);
			map.Add("947", 103);
			map.Add("948", 100);
			map.Add("949", 101);
			map.Add("950", 122);
			map.Add("951", 104);
			map.Add("952", 113);
			map.Add("953", 105);
			map.Add("954", 107);
			map.Add("955", 108);
			map.Add("956", 109);
			map.Add("957", 110);
			map.Add("958", 120);
			map.Add("959", 111);
			map.Add("960", 112);
			map.Add("961", 114);
			map.Add("962", 86);
			map.Add("963", 115);
			map.Add("964", 116);
			map.Add("965", 117);
			map.Add("966", 102);
			map.Add("967", 99);
			map.Add("9674", 224);
			map.Add("968", 121);
			map.Add("969", 119);
			map.Add("977", 74);
			map.Add("978", 161);
			map.Add("981", 106);
			map.Add("982", 118);
			map.Add("9824", 170);
			map.Add("9827", 167);
			map.Add("9829", 169);
			map.Add("9830", 168);
			map.Add("Alpha", 65);
			map.Add("Beta", 66);
			map.Add("Chi", 67);
			map.Add("Delta", 68);
			map.Add("Epsilon", 69);
			map.Add("Eta", 72);
			map.Add("Gamma", 71);
			map.Add("Iota", 73);
			map.Add("Kappa", 75);
			map.Add("Lambda", 76);
			map.Add("Mu", 77);
			map.Add("Nu", 78);
			map.Add("Omega", 87);
			map.Add("Omicron", 79);
			map.Add("Phi", 70);
			map.Add("Pi", 80);
			map.Add("Prime", 178);
			map.Add("Psi", 89);
			map.Add("Rho", 82);
			map.Add("Sigma", 83);
			map.Add("Tau", 84);
			map.Add("Theta", 81);
			map.Add("Upsilon", 85);
			map.Add("Xi", 88);
			map.Add("Zeta", 90);
			map.Add("alefsym", 192);
			map.Add("alpha", 97);
			map.Add("and", 217);
			map.Add("ang", 208);
			map.Add("asymp", 187);
			map.Add("beta", 98);
			map.Add("cap", 199);
			map.Add("chi", 99);
			map.Add("clubs", 167);
			map.Add("cong", 64);
			map.Add("copy", 211);
			map.Add("crarr", 191);
			map.Add("cup", 200);
			map.Add("dArr", 223);
			map.Add("darr", 175);
			map.Add("delta", 100);
			map.Add("diams", 168);
			map.Add("divide", 184);
			map.Add("empty", 198);
			map.Add("epsilon", 101);
			map.Add("equiv", 186);
			map.Add("eta", 104);
			map.Add("euro", 240);
			map.Add("exist", 36);
			map.Add("forall", 34);
			map.Add("frasl", 164);
			map.Add("gamma", 103);
			map.Add("ge", 179);
			map.Add("hArr", 219);
			map.Add("harr", 171);
			map.Add("hearts", 169);
			map.Add("hellip", 188);
			map.Add("horizontal arrow extender", 190);
			map.Add("image", 193);
			map.Add("infin", 165);
			map.Add("int", 242);
			map.Add("iota", 105);
			map.Add("isin", 206);
			map.Add("kappa", 107);
			map.Add("lArr", 220);
			map.Add("lambda", 108);
			map.Add("lang", 225);
			map.Add("large brace extender", 239);
			map.Add("large integral extender", 244);
			map.Add("large left brace (bottom)", 238);
			map.Add("large left brace (middle)", 237);
			map.Add("large left brace (top)", 236);
			map.Add("large left bracket (bottom)", 235);
			map.Add("large left bracket (extender)", 234);
			map.Add("large left bracket (top)", 233);
			map.Add("large left parenthesis (bottom)", 232);
			map.Add("large left parenthesis (extender)", 231);
			map.Add("large left parenthesis (top)", 230);
			map.Add("large right brace (bottom)", 254);
			map.Add("large right brace (middle)", 253);
			map.Add("large right brace (top)", 252);
			map.Add("large right bracket (bottom)", 251);
			map.Add("large right bracket (extender)", 250);
			map.Add("large right bracket (top)", 249);
			map.Add("large right parenthesis (bottom)", 248);
			map.Add("large right parenthesis (extender)", 247);
			map.Add("large right parenthesis (top)", 246);
			map.Add("larr", 172);
			map.Add("le", 163);
			map.Add("lowast", 42);
			map.Add("loz", 224);
			map.Add("minus", 45);
			map.Add("mu", 109);
			map.Add("nabla", 209);
			map.Add("ne", 185);
			map.Add("not", 216);
			map.Add("notin", 207);
			map.Add("nsub", 203);
			map.Add("nu", 110);
			map.Add("omega", 119);
			map.Add("omicron", 111);
			map.Add("oplus", 197);
			map.Add("or", 218);
			map.Add("otimes", 196);
			map.Add("part", 182);
			map.Add("perp", 94);
			map.Add("phi", 102);
			map.Add("pi", 112);
			map.Add("piv", 118);
			map.Add("plusmn", 177);
			map.Add("prime", 162);
			map.Add("prod", 213);
			map.Add("prop", 181);
			map.Add("psi", 121);
			map.Add("rArr", 222);
			map.Add("radic", 214);
			map.Add("radical extender", 96);
			map.Add("rang", 241);
			map.Add("rarr", 174);
			map.Add("real", 194);
			map.Add("reg", 210);
			map.Add("rho", 114);
			map.Add("sdot", 215);
			map.Add("sigma", 115);
			map.Add("sigmaf", 86);
			map.Add("sim", 126);
			map.Add("spades", 170);
			map.Add("sub", 204);
			map.Add("sube", 205);
			map.Add("sum", 229);
			map.Add("sup", 201);
			map.Add("supe", 202);
			map.Add("tau", 116);
			map.Add("there4", 92);
			map.Add("theta", 113);
			map.Add("thetasym", 74);
			map.Add("times", 180);
			map.Add("trade", 212);
			map.Add("uArr", 221);
			map.Add("uarr", 173);
			map.Add("upsih", 161);
			map.Add("upsilon", 117);
			map.Add("vertical arrow extender", 189);
			map.Add("weierp", 195);
			map.Add("xi", 120);
			map.Add("zeta", 122);
		}
    
		/// <summary>
		/// Gets a chunk with a symbol character.
		/// </summary>
		/// <param name="e">the original ASCII-char</param>
		/// <param name="font">a Font</param>
		/// <returns>a Chunk</returns>
		public static Chunk get(string e, Font font) {
			int s = getCorrespondingSymbol(e);
			if (s == -1) {
				try {
					return new Chunk(new string(new char[] {(char)int.Parse(e)}), font);
				}
				catch(Exception exception) {
					exception.GetType();
					return new Chunk(e, font);
				}
			}
			Font symbol = new Font(Font.SYMBOL, font.Size, font.Style, font.Color);
			return new Chunk(((char)s).ToString(), symbol);
		}
    
		/// <summary>
		/// Looks for the corresponding symbol in the font Symbol.
		/// </summary>
		/// <param name="c">the original ASCII-char</param>
		/// <returns>the corresponding symbol in font Symbol</returns>
		public static int getCorrespondingSymbol(string c) {
			object obj = map[c];
			if (obj == null) {
				return -1;
			}
				return (int)obj;
		}
    
		/// <summary>
		/// Checks if a given tag corresponds with this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public static bool isTag(string tag) {
			return ElementTags.ENTITY.Equals(tag);
		}
	}
}
