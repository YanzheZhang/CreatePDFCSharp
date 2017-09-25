using System;
using System.IO;
using System.Text;

/*
 * $Id: PRTokeniser.cs,v 1.1.1.1 2003/02/04 02:57:53 geraldhenson Exp $
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

	/**
	 *
	 * @author  Paulo Soares (psoares@consiste.pt)
	 */
	public class PRTokeniser {
    
		public const int TK_NUMBER = 1;
		public const int TK_STRING = 2;
		public const int TK_NAME = 3;
		public const int TK_COMMENT = 4;
		public const int TK_START_ARRAY = 5;
		public const int TK_END_ARRAY = 6;
		public const int TK_START_DIC = 7;
		public const int TK_END_DIC = 8;
		public const int TK_REF = 9;
		public const int TK_OTHER = 10;
    
		internal static string EMPTY = "";

    
		protected RandomAccessFileOrArray file;
		protected int type;
		protected string stringValue;
		protected int reference;
		protected int generation;

		public PRTokeniser(string filename) {
			file = new RandomAccessFileOrArray(filename);
		}

		public PRTokeniser(byte[] pdfIn) {
			file = new RandomAccessFileOrArray(pdfIn);
		}
    
		public void seek(int pos) {
			file.seek(pos);
		}
    
		public int FilePointer {
			get {
				return file.FilePointer;
			}
		}

		public void close() {
			file.close();
		}
    
		public int Length {
			get {
				return file.Length;
			}
		}

		public int read() {
			return file.read();
		}
    
		public RandomAccessFileOrArray SafeFile {
			get {
				return new RandomAccessFileOrArray(file);
			}
		}
    
		public string readstring(int size) {
			StringBuilder buf = new StringBuilder();
			int ch;
			while ((size--) > 0) {
				ch = file.read();
				if (ch == -1)
					break;
				buf.Append((char)ch);
			}
			return buf.ToString();
		}

		public static bool isWhitespace(int ch) {
			return (ch == 0 || ch == 9 || ch == 10 || ch == 12 || ch == 13 || ch == 32);
		}
    
		public static bool isDelimiter(int ch) {
			return (ch == '(' || ch == ')' || ch == '<' || ch == '>' || ch == '[' || ch == ']' || ch == '/' || ch == '%');
		}

		public int TokenType {
			get {
				return type;
			}
		}
    
		public string StringValue {
			get {
				return stringValue;
			}
		}
    
		public int Reference {
			get {
				return reference;
			}
		}
    
		public int Generation {
			get {
				return generation;
			}
		}
    
		public void backOnePosition() {
			file.seek(file.FilePointer - 1);
		}
    
		internal void throwError(string error) {
			throw new IOException(error + " at file pointer " + file.FilePointer);
		}
    
		public void checkPdfHeader() {
			string str = readstring(7);
			if (!str.Equals("%PDF-1."))
				throw new IOException("PDF header signature not found.");
		}
    
		public int Startxref {
			get {
				int size = Math.Min(1024, file.Length);
				int pos = file.Length - size;
				file.seek(pos);
				string str = readstring(1024);
				int idx = str.LastIndexOf("startxref");
				if (idx < 0)
					throw new IOException("PDF startxref not found.");
				return pos + idx;
			}
		}

		public static int getHex(int v) {
			if (v >= '0' && v <= '9')
				return v - '0';
			if (v >= 'A' && v <= 'F')
				return v - 'A' + 10;
			if (v >= 'a' && v <= 'f')
				return v - 'a' + 10;
			return -1;
		}
    
		public void nextValidToken() {
			int level = 0;
			string n1 = null;
			string n2 = null;
			int ptr = 0;
			while (nextToken()) {
				if (type == TK_COMMENT)
					continue;
				switch (level) {
					case 0: {
						if (type != TK_NUMBER)
							return;
						ptr = file.FilePointer;
						n1 = stringValue;
						++level;
						break;
					}
					case 1: {
						if (type != TK_NUMBER) {
							file.seek(ptr);
							type = TK_NUMBER;
							stringValue = n1;
							return;
						}
						n2 = stringValue;
						++level;
						break;
					}
					default: {
						if (type != TK_OTHER || !stringValue.Equals("R")) {
							file.seek(ptr);
							type = TK_NUMBER;
							stringValue = n1;
							return;
						}
						type = TK_REF;
						reference = int.Parse(n1);
						generation = int.Parse(n2);
						return;
					}
				}
			}
			throwError("Unexpected end of file");
		}
    
		public bool nextToken() {
			StringBuilder outBuf = null;
			stringValue = EMPTY;
			int ch = 0;
			do {
				ch = file.read();
			} while (ch != -1 && isWhitespace(ch));
			if (ch == -1)
				return false;
			switch (ch) {
				case '[':
					type = TK_START_ARRAY;
					break;
				case ']':
					type = TK_END_ARRAY;
					break;
				case '/': {
					outBuf = new StringBuilder();
					type = TK_NAME;
					while (true) {
						ch = file.read();
						if (ch == -1 || isDelimiter(ch) || isWhitespace(ch))
							break;
						if (ch == '#') {
							ch = (getHex(file.read()) << 4) + getHex(file.read());
						}
						outBuf.Append((char)ch);
					}
					backOnePosition();
					break;
				}
				case '>':
					ch = file.read();
					if (ch != '>')
						throwError("'>' not expected");
					type = TK_END_DIC;
					break;
				case '<': {
					int v1 = file.read();
					if (v1 == '<') {
						type = TK_START_DIC;
						break;
					}
					outBuf = new StringBuilder();
					type = TK_STRING;
					int v2 = 0;
					while (true) {
						while (isWhitespace(v1))
							v1 = file.read();
						if (v1 == '>')
							break;
						v1 = getHex(v1);
						if (v1 < 0)
							break;
						v2 = file.read();
						while (isWhitespace(v2))
							v2 = file.read();
						if (v2 == '>') {
							ch = v1 << 4;
							outBuf.Append((char)ch);
							break;
						}
						v2 = getHex(v2);
						if (v2 < 0)
							break;
						ch = (v1 << 4) + v2;
						outBuf.Append((char)ch);
						v1 = file.read();
					}
					if (v1 < 0 || v2 < 0)
						throwError("Error reading string");
					break;
				}
				case '%':
					type = TK_COMMENT;
					do {
						ch = file.read();
					} while (ch != -1 && ch != 'r' && ch != 'n');
					break;
				case '(': {
					outBuf = new StringBuilder();
					type = TK_STRING;
					int nesting = 0;
					while (true) {
						ch = file.read();
						if (ch == -1)
							break;
						if (ch == '(') {
							++nesting;
						}
						else if (ch == ')') {
							--nesting;
						}
						else if (ch == '\\') {
							bool lineBreak = false;
							ch = file.read();
							switch (ch) {
								case 'n':
									ch = '\n';
									break;
								case 'r':
									ch = '\r';
									break;
								case 't':
									ch = '\t';
									break;
								case 'b':
									ch = '\b';
									break;
								case 'f':
									ch = '\f';
									break;
								case '(':
								case ')':
								case '\\':
									break;
								case '\r':
									lineBreak = true;
									ch = file.read();
									if (ch != '\n')
										backOnePosition();
									break;
								case '\n':
									lineBreak = true;
									break;
								default: {
									if (ch < '0' || ch > '7') {
										break;
									}
									int octal = ch - '0';
									ch = file.read();
									if (ch < '0' || ch > '7') {
										ch = octal;
										backOnePosition();
										break;
									}
									octal = (octal << 3) + ch - '0';
									ch = file.read();
									if (ch < '0' || ch > '7') {
										ch = octal;
										backOnePosition();
										break;
									}
									octal = (octal << 3) + ch - '0';
									ch = octal & 0xff;
									break;
								}
							}
							if (lineBreak)
								continue;
							if (ch < 0)
								break;
						}
						else if (ch == '\r') {
							ch = file.read();
							if (ch < 0)
								break;
							if (ch != '\n') {
								ch = '\n';
								backOnePosition();
							}
						}
						if (nesting == -1)
							break;
						outBuf.Append((char)ch);
					}
					if (ch == -1)
						throwError("Error reading string");
					break;
				}
				default: {
					outBuf = new StringBuilder();
					if (ch == '-' || ch == '+' || ch == '.' || (ch >= '0' && ch <= '9')) {
						type = TK_NUMBER;
						do {
							outBuf.Append((char)ch);
							ch = file.read();
						} while (ch != -1 && ((ch >= '0' && ch <= '9') || ch == '.'));
					}
					else {
						type = TK_OTHER;
						do {
							outBuf.Append((char)ch);
							ch = file.read();
						} while (ch != -1 && !isDelimiter(ch) && !isWhitespace(ch));
					}
					backOnePosition();
					break;
				}
			}
			if (outBuf != null)
				stringValue = outBuf.ToString();
			return true;
		}
    
		public int IntValue {
			get {
				return int.Parse(stringValue);
			}
		}
	}
}