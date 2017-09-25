using System;
using System.IO;
using System.Collections;
using System.util;

using iTextSharp.text;

/*
 * $Id: TrueTypeFontSubSet.cs,v 1.1.1.1 2003/02/04 02:57:57 geraldhenson Exp $
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

	/** Subsets a True Type font by removing the unneeded glyphs from
	 * the font.
	 *
	 * @author  Paulo Soares (psoares@consiste.pt)
	 */
	internal class TrueTypeFontSubSet {
		internal static string[] tableNamesSimple = {"cvt ", "fpgm", "glyf", "head",
											   "hhea", "hmtx", "loca", "maxp", "prep"};
		internal static string[] tableNamesCmap = {"cmap", "cvt ", "fpgm", "glyf", "head",
											 "hhea", "hmtx", "loca", "maxp", "prep"};
		internal static int[] entrySelectors = {0,0,1,1,2,2,2,2,3,3,3,3,3,3,3,3,4,4,4,4,4};
		internal static int TABLE_CHECKSUM = 0;
		internal static int TABLE_OFFSET = 1;
		internal static int TABLE_LENGTH = 2;
		internal static int HEAD_LOCA_FORMAT_OFFSET = 51;

		internal static int ARG_1_AND_2_ARE_WORDS = 1;
		internal static int WE_HAVE_A_SCALE = 8;
		internal static int MORE_COMPONENTS = 32;
		internal static int WE_HAVE_AN_X_AND_Y_SCALE = 64;
		internal static int WE_HAVE_A_TWO_BY_TWO = 128;
    
    
		/** Contains the location of the several tables. The key is the name of
		 * the table and the value is an <CODE>int[3]</CODE> where position 0
		 * is the checksum, position 1 is the offset from the start of the file
		 * and position 2 is the length of the table.
		 */
		protected Hashmap tableDirectory;
		/** The file in use.
		 */
		protected RandomAccessFileOrArray rf;
		/** The file name.
		 */
		protected string fileName;
		protected bool includeCmap;
		protected bool locaShortTable;
		protected int[] locaTable;
		protected Hashmap glyphsUsed;
		protected ArrayList glyphsInList;
		protected int tableGlyphOffset;
		protected int[] newLocaTable;
		protected byte[] newLocaTableOut;
		protected byte[] newGlyfTable;
		protected int glyfTableRealSize;
		protected int locaTableRealSize;
		protected byte[] outFont;
		protected int fontPtr;
		protected int directoryOffset;

		/** Creates a new TrueTypeFontSubSet
		 * @param directoryOffset The offset from the start of the file to the table directory
		 * @param fileName the file name of the font
		 * @param glyphsUsed the glyphs used
		 * @param includeCmap <CODE>true</CODE> if the table cmap is to be included in the generated font
		 */
		internal TrueTypeFontSubSet(string fileName, RandomAccessFileOrArray rf, Hashmap glyphsUsed, int directoryOffset, bool includeCmap) {
			this.fileName = fileName;
			this.rf = rf;
			this.glyphsUsed = glyphsUsed;
			this.includeCmap = includeCmap;
			this.directoryOffset = directoryOffset;
			glyphsInList = new ArrayList(glyphsUsed.Keys);
		}
    
		/** Does the actual work of subsetting the font.
		 * @throws IOException on error
		 * @throws DocumentException on error
		 * @return the subset font
		 */    
		internal byte[] process() {
			try {
				rf.reOpen();
				createTableDirectory();
				readLoca();
				flatGlyphs();
				createNewGlyphTables();
				locaTobytes();
				assembleFont();
				return outFont;
			}
			finally {
				try {
					rf.close();
				}
				catch (Exception e) {
					e.GetType();
					// empty on purpose
				}
			}
		}
    
		protected void assembleFont() {
			int[] tableLocation;
			int fullFontSize = 0;
			string[] tableNames;
			if (includeCmap)
				tableNames = tableNamesCmap;
			else
				tableNames = tableNamesSimple;
			int tablesUsed = 2;
			int len = 0;
			for (int k = 0; k < tableNames.Length; ++k) {
				string name = tableNames[k];
				if (name.Equals("glyf") || name.Equals("loca"))
					continue;
				tableLocation = (int[])tableDirectory[name];
				if (tableLocation == null)
					continue;
				++tablesUsed;
				fullFontSize += (tableLocation[TABLE_LENGTH] + 3) & (~3);
			}
			fullFontSize += newLocaTableOut.Length;
			fullFontSize += newGlyfTable.Length;
			int iref = 16 * tablesUsed + 12;
			fullFontSize += iref;
			outFont = new byte[fullFontSize];
			fontPtr = 0;
			writeFontInt(0x00010000);
			writeFontShort(tablesUsed);
			int selector = entrySelectors[tablesUsed];
			writeFontShort((1 << selector) * 16);
			writeFontShort(selector);
			writeFontShort((tablesUsed - (1 << selector)) * 16);
			for (int k = 0; k < tableNames.Length; ++k) {
				string name = tableNames[k];
				tableLocation = (int[])tableDirectory[name];
				if (tableLocation == null)
					continue;
				writeFontstring(name);
				if (name.Equals("glyf")) {
					writeFontInt(calculateChecksum(newGlyfTable));
					len = glyfTableRealSize;
				}
				else if (name.Equals("loca")) {
					writeFontInt(calculateChecksum(newLocaTableOut));
					len = locaTableRealSize;
				}
				else {
					writeFontInt(tableLocation[TABLE_CHECKSUM]);
					len = tableLocation[TABLE_LENGTH];
				}
				writeFontInt(iref);
				writeFontInt(len);
				iref += (len + 3) & (~3);
			}
			for (int k = 0; k < tableNames.Length; ++k) {
				string name = tableNames[k];
				tableLocation = (int[])tableDirectory[name];
				if (tableLocation == null)
					continue;
				if (name.Equals("glyf")) {
					Array.Copy(newGlyfTable, 0, outFont, fontPtr, newGlyfTable.Length);
					fontPtr += newGlyfTable.Length;
					newGlyfTable = null;
				}
				else if (name.Equals("loca")) {
					Array.Copy(newLocaTableOut, 0, outFont, fontPtr, newLocaTableOut.Length);
					fontPtr += newLocaTableOut.Length;
					newLocaTableOut = null;
				}
				else {
					rf.seek(tableLocation[TABLE_OFFSET]);
					rf.readFully(outFont, fontPtr, tableLocation[TABLE_LENGTH]);
					fontPtr += (tableLocation[TABLE_LENGTH] + 3) & (~3);
				}
			}
		}
    
		protected void createTableDirectory() {
			tableDirectory = new Hashmap();
			rf.seek(directoryOffset);
			int id = rf.readInt();
			if (id != 0x00010000)
				throw new DocumentException(fileName + " is not a true type file.");
			int num_tables = rf.readUnsignedShort();
			rf.skipBytes(6);
			for (int k = 0; k < num_tables; ++k) {
				string tag = readStandardstring(4);
				int[] tableLocation = new int[3];
				tableLocation[TABLE_CHECKSUM] = rf.readInt();
				tableLocation[TABLE_OFFSET] = rf.readInt();
				tableLocation[TABLE_LENGTH] = rf.readInt();
				tableDirectory.Add(tag, tableLocation);
			}
		}
    
		protected void readLoca() {
			int[] tableLocation;
			tableLocation = (int[])tableDirectory["head"];
			if (tableLocation == null)
				throw new DocumentException("Table 'head' does not exist in " + fileName);
			rf.seek(tableLocation[TABLE_OFFSET] + HEAD_LOCA_FORMAT_OFFSET);
			locaShortTable = (rf.readUnsignedShort() == 0);
			tableLocation = (int[])tableDirectory["loca"];
			if (tableLocation == null)
				throw new DocumentException("Table 'loca' does not exist in " + fileName);
			rf.seek(tableLocation[TABLE_OFFSET]);
			if (locaShortTable) {
				int entries = tableLocation[TABLE_LENGTH] / 2;
				locaTable = new int[entries];
				for (int k = 0; k < entries; ++k)
					locaTable[k] = rf.readUnsignedShort() * 2;
			}
			else {
				int entries = tableLocation[TABLE_LENGTH] / 4;
				locaTable = new int[entries];
				for (int k = 0; k < entries; ++k)
					locaTable[k] = rf.readInt();
			}
		}
    
		protected void createNewGlyphTables() {
			newLocaTable = new int[locaTable.Length];
			int[] activeGlyphs = new int[glyphsInList.Count];
			for (int k = 0; k < activeGlyphs.Length; ++k)
				activeGlyphs[k] = (int)glyphsInList[k];
			Array.Sort(activeGlyphs);
			int glyfSize = 0;
			for (int k = 0; k < activeGlyphs.Length; ++k) {
				int glyph = activeGlyphs[k];
				glyfSize += locaTable[glyph + 1] - locaTable[glyph];
			}
			glyfTableRealSize = glyfSize;
			glyfSize = (glyfSize + 3) & (~3);
			newGlyfTable = new byte[glyfSize];
			int glyfPtr = 0;
			int listGlyf = 0;
			for (int k = 0; k < newLocaTable.Length; ++k) {
				newLocaTable[k] = glyfPtr;
				if (listGlyf < activeGlyphs.Length && activeGlyphs[listGlyf] == k) {
					++listGlyf;
					newLocaTable[k] = glyfPtr;
					int start = locaTable[k];
					int len = locaTable[k + 1] - start;
					if (len > 0) {
						rf.seek(tableGlyphOffset + start);
						rf.readFully(newGlyfTable, glyfPtr, len);
						glyfPtr += len;
					}
				}
			}
		}
    
		protected void locaTobytes() {
			if (locaShortTable)
				locaTableRealSize = newLocaTable.Length * 2;
			else
				locaTableRealSize = newLocaTable.Length * 4;
			newLocaTableOut = new byte[(locaTableRealSize + 3) & (~3)];
			outFont = newLocaTableOut;
			fontPtr = 0;
			for (int k = 0; k < newLocaTable.Length; ++k) {
				if (locaShortTable)
					writeFontShort(newLocaTable[k] / 2);
				else
					writeFontInt(newLocaTable[k]);
			}
        
		}
    
		protected void flatGlyphs() {
			int[] tableLocation;
			tableLocation = (int[])tableDirectory["glyf"];
			if (tableLocation == null)
				throw new DocumentException("Table 'glyf' does not exist in " + fileName);
			int glyph0 = 0;
			if (!glyphsUsed.ContainsKey(glyph0)) {
				glyphsUsed.Add(glyph0, null);
				glyphsInList.Add(glyph0);
			}
			tableGlyphOffset = tableLocation[TABLE_OFFSET];
			for (int k = 0; k < glyphsInList.Count; ++k) {
				int glyph = (int)glyphsInList[k];
				checkGlyphComposite(glyph);
			}
		}

		protected void checkGlyphComposite(int glyph) {
			int start = locaTable[glyph];
			if (start == locaTable[glyph + 1]) // no contour
				return;
			rf.seek(tableGlyphOffset + start);
			int numContours = rf.readShort();
			if (numContours >= 0)
				return;
			rf.skipBytes(8);
			for(;;) {
				int flags = rf.readUnsignedShort();
				int cGlyph = rf.readUnsignedShort();
				if (!glyphsUsed.ContainsKey(cGlyph)) {
					glyphsUsed.Add(cGlyph, null);
					glyphsInList.Add(cGlyph);
				}
				if ((flags & MORE_COMPONENTS) == 0)
					return;
				int skip;
				if ((flags & ARG_1_AND_2_ARE_WORDS) != 0)
					skip = 4;
				else
					skip = 2;
				if ((flags & WE_HAVE_A_SCALE) != 0)
					skip += 2;
				else if ((flags & WE_HAVE_AN_X_AND_Y_SCALE) != 0)
					skip += 4;
				if ((flags & WE_HAVE_A_TWO_BY_TWO) != 0)
					skip += 8;
				rf.skipBytes(skip);
			}
		}
    
		/** Reads a <CODE>string</CODE> from the font file as bytes using the Cp1252
		 *  encoding.
		 * @param length the length of bytes to read
		 * @return the <CODE>string</CODE> read
		 * @throws IOException the font file could not be read
		 */
		protected string readStandardstring(int length) {
			byte[] buf = new byte[length];
			rf.readFully(buf);
			try {
				return System.Text.Encoding.GetEncoding(BaseFont.WINANSI).GetString(buf);
			}
			catch (Exception e) {
				throw e;
			}
		}
    
		protected void writeFontShort(int n) {
			outFont[fontPtr++] = (byte)(n >> 8);
			outFont[fontPtr++] = (byte)(n);
		}

		protected void writeFontInt(int n) {
			outFont[fontPtr++] = (byte)(n >> 24);
			outFont[fontPtr++] = (byte)(n >> 16);
			outFont[fontPtr++] = (byte)(n >> 8);
			outFont[fontPtr++] = (byte)(n);
		}

		protected void writeFontstring(string s) {
			byte[] b = PdfEncodings.convertToBytes(s, BaseFont.WINANSI);
			Array.Copy(b, 0, outFont, fontPtr, b.Length);
			fontPtr += b.Length;
		}
    
		protected int calculateChecksum(byte[] b) {
			int len = b.Length / 4;
			int v0 = 0;
			int v1 = 0;
			int v2 = 0;
			int v3 = 0;
			int ptr = 0;
			for (int k = 0; k < len; ++k) {
				v3 += (int)b[ptr++] & 0xff;
				v2 += (int)b[ptr++] & 0xff;
				v1 += (int)b[ptr++] & 0xff;
				v0 += (int)b[ptr++] & 0xff;
			}
			return v0 + (v1 << 8) + (v2 << 16) + (v3 << 24);
		}
	}
}