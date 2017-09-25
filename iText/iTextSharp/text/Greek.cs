using System;

/*
 * $Id: Greek.cs,v 1.2 2003/03/19 17:32:27 geraldhenson Exp $
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
	/// This class contains the symbols that correspond with Greek letters.
	/// </summary>
	/// <remarks>
	/// When you construct a Phrase (or a derived object) using a string,
	/// this string can contain Greek Symbols. These are characters with an int value
	/// between 913 and 937 (except 930) and between 945 and 969. With this class the value of the
	/// corresponding character of the Font Symbol, can be retrieved.
	/// </remarks>
	/// <seealso cref="T:iTextSharp.text.Phrase"/>
	public class Greek {
    
		/// <summary>
		/// Returns the first occurrence of a Greek symbol in a string.
		/// </summary>
		/// <param name="str">a string</param>
		/// <returns>an index of -1 if no Greek symbol was found</returns>
		public static int index(string str) {
			int length = str.Length;
			for (int i = 0; i < length; i++) {
				if (getCorrespondingSymbol(str[i]) != ' ') {
					return i;
				}
			}
			return -1;
		}
    
		/// <summary>
		/// Gets a chunk with a symbol character.
		/// </summary>
		/// <param name="c">the original ASCII-char</param>
		/// <param name="font"></param>
		/// <returns></returns>
		public static Chunk get(char c, Font font) {
			char greek = Greek.getCorrespondingSymbol(c);
			if (greek == ' ') {
				return new Chunk(new string(c, 1), font);
			}
			Font symbol = new Font(Font.SYMBOL, font.Size, font.Style, font.Color);
			string s = new string(greek, 1);
			return new Chunk(s, symbol);
		}
    
		/// <summary>
		/// Looks for the corresponding symbol in the font Symbol.
		/// </summary>
		/// <param name="c">the original ASCII-char</param>
		/// <returns>the corresponding symbol in font Symbol</returns>
		public static char getCorrespondingSymbol(char c) {
			switch(c) {
				case (char)913:
					return 'A'; // ALFA
				case (char)914:
					return 'B'; // BETA
				case (char)915:
					return 'G'; // GAMMA
				case (char)916:
					return 'D'; // DELTA
				case (char)917:
					return 'E'; // EPSILON
				case (char)918:
					return 'Z'; // ZETA
				case (char)919:
					return 'H'; // ETA
				case (char)920:
					return 'Q'; // THETA
				case (char)921:
					return 'I'; // IOTA
				case (char)922:
					return 'K'; // KAPPA
				case (char)923:
					return 'L'; // LAMBDA
				case (char)924:
					return 'M'; // MU
				case (char)925:
					return 'N'; // NU
				case (char)926:
					return 'X'; // XI
				case (char)927:
					return 'O'; // OMICRON
				case (char)928:
					return 'P'; // PI
				case (char)929:
					return 'R'; // RHO
				case (char)931:
					return 'S'; // SIGMA
				case (char)932:
					return 'T'; // TAU
				case (char)933:
					return 'U'; // UPSILON
				case (char)934:
					return 'J'; // PHI
				case (char)935:
					return 'C'; // CHI
				case (char)936:
					return 'Y'; // PSI
				case (char)937:
					return 'W'; // OMEGA
				case (char)945:
					return 'a'; // alfa
				case (char)946:
					return 'b'; // beta
				case (char)947:
					return 'g'; // gamma
				case (char)948:
					return 'd'; // delta
				case (char)949:
					return 'e'; // epsilon
				case (char)950:
					return 'z'; // zeta
				case (char)951:
					return 'h'; // eta
				case (char)952:
					return 'q'; // theta
				case (char)953:
					return 'i'; // iota
				case (char)954:
					return 'k'; // kappa
				case (char)955:
					return 'l'; // lambda
				case (char)956:
					return 'm'; // mu
				case (char)957:
					return 'n'; // nu
				case (char)958:
					return 'x'; // xi
				case (char)959:
					return 'o'; // omicron
				case (char)960:
					return 'p'; // pi
				case (char)961:
					return 'r'; // rho
				case (char)962:
					return 's'; // sigma
				case (char)963:
					return 's'; // sigma
				case (char)964:
					return 't'; // tau
				case (char)965:
					return 'u'; // upsilon
				case (char)966:
					return 'j'; // phi
				case (char)967:
					return 'c'; // chi
				case (char)968:
					return 'y'; // psi
				case (char)969:
					return 'w'; // omega
				default:
					return ' ';
			}
		}
	}
}
