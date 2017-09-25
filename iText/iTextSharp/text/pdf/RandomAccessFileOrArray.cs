using System;
using System.IO;
using System.Text;

/*
 * $Id: RandomAccessFileOrArray.cs,v 1.1.1.1 2003/02/04 02:57:53 geraldhenson Exp $
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

	/** An implementation of a RandomAccessFile for input only
	 * that accepts a file or a byte array as data source.
	 *
	 * @author Paulo Soares (psoares@consiste.pt)
	 */
	public class RandomAccessFileOrArray {

		FileStream rf;
		string filename;
		byte[] arrayIn;
		int arrayInPtr;

		public RandomAccessFileOrArray(string filename) {
			this.filename = filename;
			rf = new FileStream(filename,FileMode.Open,FileAccess.Read);
		}
    
		public RandomAccessFileOrArray(byte[] arrayIn) {
			this.arrayIn = arrayIn;
		}

		public RandomAccessFileOrArray(RandomAccessFileOrArray file) {
			filename = file.filename;
			arrayIn = file.arrayIn;
		}
    
		public int read() {
			if (arrayIn == null)
				return rf.ReadByte();
			else {
				if (arrayInPtr >= arrayIn.Length)
					return -1;
				return arrayIn[arrayInPtr++] & 0xff;
			}
		}

		public int read(byte[] b, int off, int len) {
			if (arrayIn == null)
				return rf.Read(b, off, len);
			else {
				if (len == 0)
					return 0;
				if (arrayInPtr >= arrayIn.Length)
					return -1;
				if (arrayInPtr + len > arrayIn.Length)
					len = arrayIn.Length - arrayInPtr;
				Array.Copy(arrayIn, arrayInPtr, b, off, len);
				arrayInPtr += len;
				return len;
			}
		}

		public int read(byte[] b) {
			return read(b, 0, b.Length);
		}
    
		public void readFully(byte[] b) {
			readFully(b, 0, b.Length);
		}
    
		public void readFully(byte[] b, int off, int len) {
			int n = 0;
			do {
				int count = read(b, off + n, len - n);
				if (count < 0)
					throw new Exception("EOF");
				n += count;
			} while (n < len);
		}
    
		public int skipBytes(int n) {
			int pos;
			int len;
			int newpos;
        
			if (n <= 0) {
				return 0;
			}
			pos = this.FilePointer;
			len = this.Length;
			newpos = pos + n;
			if (newpos > len) {
				newpos = len;
			}
			seek(newpos);
        
			/* return the actual number of bytes skipped */
			return newpos - pos;
		}
    
		internal void reOpen() {
			if (filename != null) {
				close();
				rf = new FileStream(filename,FileMode.Open,FileAccess.Read);
			}
			else {
				arrayInPtr = 0;
			}
		}
    
		public void close() {
			if (rf != null) {
				rf.Close();
				rf = null;
			}
		}
    
		public int Length {
			get {
				if (arrayIn == null)
					return (int)rf.Length;
				else
					return arrayIn.Length;
			}
		}
    
		public void seek(int pos) {
			if (arrayIn == null)
				rf.Seek(pos, SeekOrigin.Begin);
			else
				arrayInPtr = pos;
		}
    
		public int FilePointer {
			get {
				if (arrayIn == null)
					return (int)rf.Position;
				else
					return arrayInPtr;
			}
		}
    
		public bool readBoolean() {
			int ch = this.read();
			if (ch < 0)
				throw new Exception("EOF");
			return (ch != 0);
		}
    
		public byte readByte() {
			int ch = this.read();
			if (ch < 0)
				throw new Exception("EOF");
			return (byte)(ch);
		}
    
		public int readUnsignedByte() {
			int ch = this.read();
			if (ch < 0)
				throw new Exception("EOF");
			return ch;
		}
    
		public short readShort() {
			int ch1 = this.read();
			int ch2 = this.read();
			if ((ch1 | ch2) < 0)
				throw new Exception("EOF");
			return (short)((ch1 << 8) + ch2);
		}
    
		public int readUnsignedShort() {
			int ch1 = this.read();
			int ch2 = this.read();
			if ((ch1 | ch2) < 0)
				throw new Exception("EOF");
			return (ch1 << 8) + ch2;
		}
    
		public char readChar() {
			int ch1 = this.read();
			int ch2 = this.read();
			if ((ch1 | ch2) < 0)
				throw new Exception("EOF");
			return (char)((ch1 << 8) + ch2);
		}
    
		public int readInt() {
			int ch1 = this.read();
			int ch2 = this.read();
			int ch3 = this.read();
			int ch4 = this.read();
			if ((ch1 | ch2 | ch3 | ch4) < 0)
				throw new Exception("EOF");
			return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + ch4);
		}
    
		public long readLong() {
			return ((long)(readInt()) << 32) + (readInt() & 0xFFFFFFFFL);
		}
    
		public float readfloat() {
			return (float)readInt();
		}
    
		public double readDouble() {
			return (double)readLong();
		}
    
		public string readLine() {
			StringBuilder input = new StringBuilder();
			int c = -1;
			bool eol = false;
        
			while (!eol) {
				switch (c = read()) {
					case -1:
					case '\n':
						eol = true;
						break;
					case '\r':
						eol = true;
						int cur = this.FilePointer;
						if ((read()) != '\n') {
							seek(cur);
						}
						break;
					default:
						input.Append((char)c);
						break;
				}
			}
        
			if ((c == -1) && (input.Length == 0)) {
				return null;
			}
			return input.ToString();
		}
    
		public string readUTF() {
			//return DataStream.readUTF(this);
			throw new Exception("not implemented");
		}
	}
}