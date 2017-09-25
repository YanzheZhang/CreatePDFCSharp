using System;
using System.IO;
using System.Text;
using System.Collections;
using System.util;

using iTextSharp.text;

/*
 * $Id: TrueTypeFont.cs,v 1.2 2003/08/22 16:18:12 geraldhenson Exp $
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

	/** Reads a Truetype font
	 *
	 * @author Paulo Soares (psoares@consiste.pt)
	 */
	internal class TrueTypeFont : BaseFont {

		/** The code pages possible for a True Type font.
		 */    
		internal static string[] codePages = {
										"1252 Latin 1",
										"1250 Latin 2: Eastern Europe",
										"1251 Cyrillic",
										"1253 Greek",
										"1254 Turkish",
										"1255 Hebrew",
										"1256 Arabic",
										"1257 Windows Baltic",
										"1258 Vietnamese",
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										"874 Thai",
										"932 JIS/Japan",
										"936 Chinese: Simplified chars--PRC and Singapore",
										"949 Korean Wansung",
										"950 Chinese: Traditional chars--Taiwan and Hong Kong",
										"1361 Korean Johab",
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										"Macintosh Character Set (US Roman)",
										"OEM Character Set",
										"Symbol Character Set",
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										null,
										"869 IBM Greek",
										"866 MS-DOS Russian",
										"865 MS-DOS Nordic",
										"864 Arabic",
										"863 MS-DOS Canadian French",
										"862 Hebrew",
										"861 MS-DOS Icelandic",
										"860 MS-DOS Portuguese",
										"857 IBM Turkish",
										"855 IBM Cyrillic; primarily Russian",
										"852 Latin 2",
										"775 MS-DOS Baltic",
										"737 Greek; former 437 G",
										"708 Arabic; ASMO 708",
										"850 WE/Latin 1",
										"437 US"};
 
		protected bool justNames = false;
		/** Contains the location of the several tables. The key is the name of
		 * the table and the value is an <CODE>int[2]</CODE> where position 0
		 * is the offset from the start of the file and position 1 is the length
		 * of the table.
		 */
		protected Hashmap tables;
		/** The file in use.
		 */
		protected RandomAccessFileOrArray rf;
		/** The file name.
		 */
		protected string fileName;
    
		protected bool cff = false;
    
		protected int cffOffset;
    
		protected int cffLength;
    
		/** The offset from the start of the file to the table directory.
		 * It is 0 for TTF and may vary for TTC depending on the chosen font.
		 */    
		protected int directoryOffset;
		/** The index for the TTC font. It is an empty <CODE>string</CODE> for a
		 * TTF file.
		 */    
		protected string ttcIndex;
		/** The style modifier */
		protected string style = "";
		/** The content of table 'head'.
		 */
		protected FontHeader head = new FontHeader();
		/** The content of table 'hhea'.
		 */
		protected HorizontalHeader hhea = new HorizontalHeader();
		/** The content of table 'OS/2'.
		 */
		protected WindowsMetrics os_2 = new WindowsMetrics();
		/** The width of the glyphs. This is essentially the content of table
		 * 'hmtx' normalized to 1000 units.
		 */
		protected int[] GlyphWidths;
		/** The map containing the code information for the table 'cmap', encoding 1.0.
		 * The key is the code and the value is an <CODE>int[2]</CODE> where position 0
		 * is the glyph number and position 1 is the glyph width normalized to 1000
		 * units.
		 */
		protected Hashmap cmap10;
		/** The map containing the code information for the table 'cmap', encoding 3.1
		 * in Unicode.
		 * <P>
		 * The key is the code and the value is an <CODE>int</CODE>[2] where position 0
		 * is the glyph number and position 1 is the glyph width normalized to 1000
		 * units.
		 */
		protected Hashmap cmap31;
		/** The map containig the kerning information. It represents the content of
		 * table 'kern'. The key is an <CODE>int</CODE> where the top 16 bits
		 * are the Unicode for the first character and the lower 16 bits are the
		 * Unicode for the second character. The value is the amount of kerning in
		 * normalized 1000 units as an <CODE>int</CODE>. This value is usually negative.
		 */
		protected Hashmap kerning;
		/**
		 * The font name.
		 * This name is usually extracted from the table 'name' with
		 * the 'Name ID' 6.
		 */
		protected string fontName;
    
		/** The full name of the font
		 */    
		protected string[][] fullName;

		/** The family name of the font
		 */    
		protected string[][] familyName;
		/** The italic angle. It is usually extracted from the 'post' table or in it's
		 * absence with the code:
		 * <P>
		 * <PRE>
		 * -Math.atan2(hhea.caretSlopeRun, hhea.caretSlopeRise) * 180 / Math.PI
		 * </PRE>
		 */
		protected double italicAngle;
		/** <CODE>true</CODE> if all the glyphs have the same width.
		 */
		protected bool isFixedPitch = false;
    
		/** The components of table 'head'.
		 */
		protected class FontHeader {
			/** A variable. */
			internal int flags;
			/** A variable. */
			internal int unitsPerEm;
			/** A variable. */
			internal short xMin;
			/** A variable. */
			internal short yMin;
			/** A variable. */
			internal short xMax;
			/** A variable. */
			internal short yMax;
			/** A variable. */
			internal int macStyle;
		}
    
		/** The components of table 'hhea'.
		 */
		protected class HorizontalHeader {
			/** A variable. */
			internal short Ascender;
			/** A variable. */
			internal short Descender;
			/** A variable. */
			internal short LineGap;
			/** A variable. */
			internal int advanceWidthMax;
			/** A variable. */
			internal short minLeftSideBearing;
			/** A variable. */
			internal short minRightSideBearing;
			/** A variable. */
			internal short xMaxExtent;
			/** A variable. */
			internal short caretSlopeRise;
			/** A variable. */
			internal short caretSlopeRun;
			/** A variable. */
			internal int numberOfHMetrics;
		}
    
		/** The components of table 'OS/2'.
		 */
		protected class WindowsMetrics {
			/** A variable. */
			internal short xAvgCharWidth;
			/** A variable. */
			internal int usWeightClass;
			/** A variable. */
			internal int usWidthClass;
			/** A variable. */
			internal short fsType;
			/** A variable. */
			internal short ySubscriptXSize;
			/** A variable. */
			internal short ySubscriptYSize;
			/** A variable. */
			internal short ySubscriptXOffset;
			/** A variable. */
			internal short ySubscriptYOffset;
			/** A variable. */
			internal short ySuperscriptXSize;
			/** A variable. */
			internal short ySuperscriptYSize;
			/** A variable. */
			internal short ySuperscriptXOffset;
			/** A variable. */
			internal short ySuperscriptYOffset;
			/** A variable. */
			internal short yStrikeoutSize;
			/** A variable. */
			internal short yStrikeoutPosition;
			/** A variable. */
			internal short sFamilyClass;
			/** A variable. */
			internal byte[] panose = new byte[10];
			/** A variable. */
			internal byte[] achVendID = new byte[4];
			/** A variable. */
			internal int fsSelection;
			/** A variable. */
			internal int usFirstCharIndex;
			/** A variable. */
			internal int usLastCharIndex;
			/** A variable. */
			internal short sTypoAscender;
			/** A variable. */
			internal short sTypoDescender;
			/** A variable. */
			internal short sTypoLineGap;
			/** A variable. */
			internal int usWinAscent;
			/** A variable. */
			internal int usWinDescent;
			/** A variable. */
			internal int ulCodePageRange1;
			/** A variable. */
			internal int ulCodePageRange2;
			/** A variable. */
			internal int sCapHeight;
		}
    
		/** This constructor is present to allow extending the class.
		 */
		protected TrueTypeFont() {
		}
    
		internal TrueTypeFont(string ttFile, string enc, bool emb, byte[] ttfAfm) : this(ttFile, enc, emb, ttfAfm, false) {}
    
		/** Creates a new TrueType font.
		 * @param ttFile the location of the font on file. The file must end in '.ttf' or
		 * '.ttc' but can have modifiers after the name
		 * @param enc the encoding to be applied to this font
		 * @param emb true if the font is to be embedded in the PDF
		 * @param ttfAfm the font as a <CODE>byte</CODE> array
		 * @throws DocumentException the font is invalid
		 * @throws IOException the font file could not be read
		 */
		internal TrueTypeFont(string ttFile, string enc, bool emb, byte[] ttfAfm, bool justNames) {
			this.justNames = justNames;
			string nameBase = getBaseName(ttFile);
			string ttcName = getTTCName(nameBase);
			if (nameBase.Length < ttFile.Length) {
				style = ttFile.Substring(nameBase.Length);
			}
			encoding = enc;
			embedded = emb;
			fileName = ttcName;
			FontType = FONT_TYPE_TT;
			ttcIndex = "";
			if (ttcName.Length < nameBase.Length)
				ttcIndex = nameBase.Substring(ttcName.Length + 1);
			if (fileName.ToLower().EndsWith(".ttf") || fileName.ToLower().EndsWith(".otf") || fileName.ToLower().EndsWith(".ttc")) {
				process(ttfAfm);
			}
			else
				throw new DocumentException(fileName + style + " is not a TTF, OTF or TTC font file.");
			try {
				System.Text.Encoding.GetEncoding(enc).GetBytes(" ");
				createEncoding();
			}
			catch (Exception e) {
				throw e;
			}
		}
    
		/** Gets the name from a composed TTC file name.
		 * If I have for input "myfont.ttc,2" the return will
		 * be "myfont.ttc".
		 * @param name the full name
		 * @return the simple file name
		 */    
		protected static string getTTCName(string name) {
			int idx = name.ToLower().IndexOf(".ttc,");
			if (idx < 0)
				return name;
			else
				return name.Substring(0, idx + 4);
		}
    
    
		/**
		 * Reads the tables 'head', 'hhea', 'OS/2' and 'post' filling several variables.
		 * @throws DocumentException the font is invalid
		 * @throws IOException the font file could not be read
		 */
		internal void fillTables() {
			int[] table_location;
			table_location = (int[])tables["head"];
			if (table_location == null)
				throw new DocumentException("Table 'head' does not exist in " + fileName + style);
			rf.seek(table_location[0] + 16);
			head.flags = rf.readUnsignedShort();
			head.unitsPerEm = rf.readUnsignedShort();
			rf.skipBytes(16);
			head.xMin = rf.readShort();
			head.yMin = rf.readShort();
			head.xMax = rf.readShort();
			head.yMax = rf.readShort();
			head.macStyle = rf.readUnsignedShort();
        
			table_location = (int[])tables["hhea"];
			if (table_location == null)
				throw new DocumentException("Table 'hhea' does not exist " + fileName + style);
			rf.seek(table_location[0] + 4);
			hhea.Ascender = rf.readShort();
			hhea.Descender = rf.readShort();
			hhea.LineGap = rf.readShort();
			hhea.advanceWidthMax = rf.readUnsignedShort();
			hhea.minLeftSideBearing = rf.readShort();
			hhea.minRightSideBearing = rf.readShort();
			hhea.xMaxExtent = rf.readShort();
			hhea.caretSlopeRise = rf.readShort();
			hhea.caretSlopeRun = rf.readShort();
			rf.skipBytes(12);
			hhea.numberOfHMetrics = rf.readUnsignedShort();
        
			table_location = (int[])tables["OS/2"];
			if (table_location == null)
				throw new DocumentException("Table 'OS/2' does not exist in " + fileName + style);
			rf.seek(table_location[0]);
			int version = rf.readUnsignedShort();
			os_2.xAvgCharWidth = rf.readShort();
			os_2.usWeightClass = rf.readUnsignedShort();
			os_2.usWidthClass = rf.readUnsignedShort();
			os_2.fsType = rf.readShort();
			os_2.ySubscriptXSize = rf.readShort();
			os_2.ySubscriptYSize = rf.readShort();
			os_2.ySubscriptXOffset = rf.readShort();
			os_2.ySubscriptYOffset = rf.readShort();
			os_2.ySuperscriptXSize = rf.readShort();
			os_2.ySuperscriptYSize = rf.readShort();
			os_2.ySuperscriptXOffset = rf.readShort();
			os_2.ySuperscriptYOffset = rf.readShort();
			os_2.yStrikeoutSize = rf.readShort();
			os_2.yStrikeoutPosition = rf.readShort();
			os_2.sFamilyClass = rf.readShort();
			rf.readFully(os_2.panose);
			rf.skipBytes(16);
			rf.readFully(os_2.achVendID);
			os_2.fsSelection = rf.readUnsignedShort();
			os_2.usFirstCharIndex = rf.readUnsignedShort();
			os_2.usLastCharIndex = rf.readUnsignedShort();
			os_2.sTypoAscender = rf.readShort();
			os_2.sTypoDescender = rf.readShort();
			os_2.sTypoLineGap = rf.readShort();
			os_2.usWinAscent = rf.readUnsignedShort();
			os_2.usWinDescent = rf.readUnsignedShort();
			os_2.ulCodePageRange1 = 0;
			os_2.ulCodePageRange2 = 0;
			if (version > 0) {
				os_2.ulCodePageRange1 = rf.readInt();
				os_2.ulCodePageRange2 = rf.readInt();
			}
			if (version > 1) {
				rf.skipBytes(2);
				os_2.sCapHeight = rf.readShort();
			}
			else
				os_2.sCapHeight = (int)(0.7 * head.unitsPerEm);
        
			table_location = (int[])tables["post"];
			if (table_location == null) {
				italicAngle = -Math.Atan2(hhea.caretSlopeRun, hhea.caretSlopeRise) * 180 / Math.PI;
				return;
			}
			rf.seek(table_location[0] + 4);
			short mantissa = rf.readShort();
			int fraction = rf.readUnsignedShort();
			italicAngle = (double)mantissa + (double)fraction / 16384.0;
			rf.skipBytes(4);
			isFixedPitch = rf.readInt() != 0;
		}
    
		/**
		 * Gets the Postscript font name.
		 * @throws DocumentException the font is invalid
		 * @throws IOException the font file could not be read
		 * @return the Postscript font name
		 */
		internal string BaseFont {
			get {
				int[] table_location;
				table_location = (int[])tables["name"];
				if (table_location == null)
					throw new DocumentException("Table 'name' does not exist in " + fileName + style);
				rf.seek(table_location[0] + 2);
				int numRecords = rf.readUnsignedShort();
				int startOfStorage = rf.readUnsignedShort();
				for (int k = 0; k < numRecords; ++k) {
					int platformID = rf.readUnsignedShort();
					int platformEncodingID = rf.readUnsignedShort();
					int languageID = rf.readUnsignedShort();
					int nameID = rf.readUnsignedShort();
					int length = rf.readUnsignedShort();
					int offset = rf.readUnsignedShort();
					if (nameID == 6) {
						rf.seek(table_location[0] + startOfStorage + offset);
						if (platformID == 0 || platformID == 3)
							return readUnicodestring(length);
						else
							return readStandardstring(length);
					}
				}
				FileInfo file = new FileInfo(fileName);
				return file.Name.Replace(' ', '-');
			}
		}
    
		/** Extracts the names of the font in all the languages available.
		 * @param id the name id to retrieve
		 * @throws DocumentException on error
		 * @throws IOException on error
		 */    
		internal string[][] getNames(int id) {
			int[] table_location;
			table_location = (int[])tables["name"];
			if (table_location == null)
				throw new DocumentException("Table 'name' does not exist in " + fileName + style);
			rf.seek(table_location[0] + 2);
			int numRecords = rf.readUnsignedShort();
			int startOfStorage = rf.readUnsignedShort();
			ArrayList names = new ArrayList();
			for (int k = 0; k < numRecords; ++k) {
				int platformID = rf.readUnsignedShort();
				int platformEncodingID = rf.readUnsignedShort();
				int languageID = rf.readUnsignedShort();
				int nameID = rf.readUnsignedShort();
				int length = rf.readUnsignedShort();
				int offset = rf.readUnsignedShort();
				if (nameID == id) {
					int pos = rf.FilePointer;
					rf.seek(table_location[0] + startOfStorage + offset);
					string name;
					if (platformID == 0 || platformID == 3 || (platformID == 2 && platformEncodingID == 1)){
						name = readUnicodestring(length);
					}
					else {
						name = readStandardstring(length);
					}
					names.Add(new string[]{platformID.ToString(),
											  platformEncodingID.ToString(), languageID.ToString(), name});
					rf.seek(pos);
				}
			}
			string[][] thisName = new string[names.Count][];
			for (int k = 0; k < names.Count; ++k)
				thisName[k] = (string[])names[k];
			return thisName;
		}
    
		internal void checkCff() {
			int[] table_location;
			table_location = (int[])tables["CFF "];
			if (table_location != null) {
				cff = true;
				cffOffset = table_location[0];
				cffLength = table_location[1];
			}
		}

		/** Reads the font data.
		 * @param ttfAfm the font as a <CODE>byte</CODE> array, possibly <CODE>null</CODE>
		 * @throws DocumentException the font is invalid
		 * @throws IOException the font file could not be read
		 */
		internal void process(byte[] ttfAfm) {
			tables = new Hashmap();
        
			try {
				if (ttfAfm == null)
					rf = new RandomAccessFileOrArray(fileName);
				else
					rf = new RandomAccessFileOrArray(ttfAfm);
				if (ttcIndex.Length > 0) {
					int dirIdx = int.Parse(ttcIndex);
					if (dirIdx < 0)
						throw new DocumentException("The font index for " + fileName + " must be positive.");
					string mainTag = readStandardstring(4);
					if (!mainTag.Equals("ttcf"))
						throw new DocumentException(fileName + " is not a valid TTC file.");
					rf.skipBytes(4);
					int dirCount = rf.readInt();
					if (dirIdx >= dirCount)
						throw new DocumentException("The font index for " + fileName + " must be between 0 and " + (dirCount - 1) + ". It was " + dirIdx + ".");
					rf.skipBytes(dirIdx * 4);
					directoryOffset = rf.readInt();
				}
				rf.seek(directoryOffset);
				int ttId = rf.readInt();
				if (ttId != 0x00010000 && ttId != 0x4F54544F)
					throw new DocumentException(fileName + " is not a valid TTF or OTF file.");
				int num_tables = rf.readUnsignedShort();
				rf.skipBytes(6);
				for (int k = 0; k < num_tables; ++k) {
					string tag = readStandardstring(4);
					rf.skipBytes(4);
					int[] table_location = new int[2];
					table_location[0] = rf.readInt();
					table_location[1] = rf.readInt();
					tables.Add(tag, table_location);
				}
				checkCff();
				fontName = BaseFont;
				fullName = getNames(4); //full name
				familyName = getNames(1); //family name
				if (!justNames) {
					fillTables();
					readGlyphWidths();
					readCMaps();
					readKerning();
				}
			}
			finally {
				if (rf != null) {
					rf.close();
					if (!embedded)
						rf = null;
				}
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
				return System.Text.Encoding.GetEncoding(WINANSI).GetString(buf);
			}
			catch (Exception e) {
				throw e;
			}
		}
    
		/** Reads a Unicode <CODE>string</CODE> from the font file. Each character is
		 *  represented by two bytes.
		 * @param length the length of bytes to read. The <CODE>string</CODE> will have <CODE>length</CODE>/2
		 * characters
		 * @return the <CODE>string</CODE> read
		 * @throws IOException the font file could not be read
		 */
		protected string readUnicodestring(int length) {
			StringBuilder buf = new StringBuilder();
			length /= 2;
			for (int k = 0; k < length; ++k) {
				buf.Append(rf.readChar());
			}
			return buf.ToString();
		}
    
		/** Reads the glyphs widths. The widths are extracted from the table 'hmtx'.
		 *  The glyphs are normalized to 1000 units.
		 * @throws DocumentException the font is invalid
		 * @throws IOException the font file could not be read
		 */
		protected void readGlyphWidths() {
			int[] table_location;
			table_location = (int[])tables["hmtx"];
			if (table_location == null)
				throw new DocumentException("Table 'hmtx' does not exist in " + fileName + style);
			rf.seek(table_location[0]);
			GlyphWidths = new int[hhea.numberOfHMetrics];
			for (int k = 0; k < hhea.numberOfHMetrics; ++k) {
				GlyphWidths[k] = (rf.readUnsignedShort() * 1000) / head.unitsPerEm;
				rf.readUnsignedShort();
			}
		}
    
		/** Gets a glyph width.
		 * @param glyph the glyph to get the width of
		 * @return the width of the glyph in normalized 1000 units
		 */
		public int getGlyphWidth(int glyph) {
			if (glyph >= GlyphWidths.Length)
				glyph = GlyphWidths.Length - 1;
			return GlyphWidths[glyph];
		}
    
		/** Reads the several maps from the table 'cmap'. The maps of interest are 1.0 for symbolic
		 *  fonts and 3.1 for all others. A symbolic font is defined as having the map 3.0.
		 * @throws DocumentException the font is invalid
		 * @throws IOException the font file could not be read
		 */
		internal void readCMaps() {
			int[] table_location;
			table_location = (int[])tables["cmap"];
			if (table_location == null)
				throw new DocumentException("Table 'cmap' does not exist in " + fileName + style);
			rf.seek(table_location[0]);
			rf.skipBytes(2);
			int num_tables = rf.readUnsignedShort();
			fontSpecific = false;
			int map10 = 0;
			int map31 = 0;
			for (int k = 0; k < num_tables; ++k) {
				int platId = rf.readUnsignedShort();
				int platSpecId = rf.readUnsignedShort();
				int offset = rf.readInt();
				if (platId == 3 && platSpecId == 0) {
					fontSpecific = true;
				}
				else if (platId == 3 && platSpecId == 1) {
					map31 = offset;
				}
				if (platId == 1 && platSpecId == 0) {
					map10 = offset;
				}
			}
			if (map10 > 0) {
				rf.seek(table_location[0] + map10);
				int format = rf.readUnsignedShort();
				switch (format) {
					case 0:
						cmap10 = readFormat0();
						break;
					case 4:
						cmap10 = readFormat4();
						break;
					case 6:
						cmap10 = readFormat6();
						break;
				}
			}
			if (map31 > 0) {
				rf.seek(table_location[0] + map31);
				int format = rf.readUnsignedShort();
				if (format == 4) {
					cmap31 = readFormat4();
				}
			}
		}
    
		/** The information in the maps of the table 'cmap' is coded in several formats.
		 *  Format 0 is the Apple standard character to glyph index mapping table.
		 * @return a <CODE>Hashtable</CODE> representing this map
		 * @throws IOException the font file could not be read
		 */
		internal Hashmap readFormat0() {
			Hashmap h = new Hashmap();
			rf.skipBytes(4);
			for (int k = 0; k < 256; ++k) {
				int[] r = new int[2];
				r[0] = rf.readUnsignedByte();
				r[1] = getGlyphWidth(r[0]);
				h.Add(k, r);
			}
			return h;
		}
    
		/** The information in the maps of the table 'cmap' is coded in several formats.
		 *  Format 4 is the Microsoft standard character to glyph index mapping table.
		 * @return a <CODE>Hashtable</CODE> representing this map
		 * @throws IOException the font file could not be read
		 */
		internal Hashmap readFormat4() {
			Hashmap h = new Hashmap();
			int table_lenght = rf.readUnsignedShort();
			rf.skipBytes(2);
			int segCount = rf.readUnsignedShort() / 2;
			rf.skipBytes(6);
			int[] endCount = new int[segCount];
			for (int k = 0; k < segCount; ++k) {
				endCount[k] = rf.readUnsignedShort();
			}
			rf.skipBytes(2);
			int[] startCount = new int[segCount];
			for (int k = 0; k < segCount; ++k) {
				startCount[k] = rf.readUnsignedShort();
			}
			int[] idDelta = new int[segCount];
			for (int k = 0; k < segCount; ++k) {
				idDelta[k] = rf.readUnsignedShort();
			}
			int[] idRO = new int[segCount];
			for (int k = 0; k < segCount; ++k) {
				idRO[k] = rf.readUnsignedShort();
			}
			int[] glyphId = new int[table_lenght / 2 - 8 - segCount * 4];
			for (int k = 0; k < glyphId.Length; ++k) {
				glyphId[k] = rf.readUnsignedShort();
			}
			for (int k = 0; k < segCount; ++k) {
				int glyph;
				for (int j = startCount[k]; j <= endCount[k] && j != 0xFFFF; ++j) {
					if (idRO[k] == 0) {
						glyph = (j + idDelta[k]) & 0xFFFF;
					}
					else {
						int idx = k + idRO[k] / 2 - segCount + j - startCount[k];
						glyph = (glyphId[idx] + idDelta[k]) & 0xFFFF;
					}
					int[] r = new int[2];
					r[0] = glyph;
					r[1] = getGlyphWidth(r[0]);
					h.Add(j, r);
				}
			}
			return h;
		}
    
		/** The information in the maps of the table 'cmap' is coded in several formats.
		 *  Format 6 is a trimmed table mapping. It is similar to format 0 but can have
		 *  less than 256 entries.
		 * @return a <CODE>Hashtable</CODE> representing this map
		 * @throws IOException the font file could not be read
		 */
		internal Hashmap readFormat6() {
			Hashmap h = new Hashmap();
			rf.skipBytes(4);
			int start_code = rf.readUnsignedShort();
			int code_count = rf.readUnsignedShort();
			for (int k = 0; k < code_count; ++k) {
				int[] r = new int[2];
				r[0] = rf.readUnsignedShort();
				r[1] = getGlyphWidth(r[0]);
				h.Add(k + start_code, r);
			}
			return h;
		}
    
		/** Reads the kerning information from the 'kern' table.
		 * @throws IOException the font file could not be read
		 */
		internal void readKerning() {
			int[] table_location;
			table_location = (int[])tables["kern"];
			if (table_location == null)
				return;
			rf.seek(table_location[0] + 2);
			int nTables = rf.readUnsignedShort();
			kerning = new Hashmap();
			int checkpoint = table_location[0] + 4;
			int length = 0;
			for (int k = 0; k < nTables; ++k) {
				checkpoint += length;
				rf.seek(checkpoint);
				rf.skipBytes(2);
				length = rf.readUnsignedShort();
				int coverage = rf.readUnsignedShort();
				if ((coverage & 0xfff7) == 0x0001) {
					int nPairs = rf.readUnsignedShort();
					rf.skipBytes(6);
					for (int j = 0; j < nPairs; ++j) {
						int pair = rf.readInt();
						int value = ((int)rf.readShort() * 1000) / head.unitsPerEm;
						kerning.Add(pair, value);
					}
				}
			}
		}
    
		/** Gets the kerning between two Unicode chars.
		 * @param char1 the first char
		 * @param char2 the second char
		 * @return the kerning to be applied
		 */
		public override int getKerning(char char1, char char2) {
			int[] metrics = getMetricsTT(char1);
			if (metrics == null)
				return 0;
			int c1 = metrics[0];
			metrics = getMetricsTT(char2);
			if (metrics == null)
				return 0;
			int c2 = metrics[0];
			object v = kerning[c1 << 16 + c2];
			if (v == null)
				return 0;
			else
				return (int)v;
		}
    
		/** Gets the width from the font according to the unicode char <CODE>c</CODE>.
		 * If the <CODE>name</CODE> is null it's a symbolic font.
		 * @param c the unicode char
		 * @param name the glyph name
		 * @return the width of the char
		 */
		protected override int getRawWidth(int c, string name) {
			Hashmap map = null;
			if (name == null)
				map = cmap10;
			else
				map = cmap31;
			if (map == null)
				return 0;
			int[] metric = (int[])map[c];
			if (metric == null)
				return 0;
			return metric[1];
		}
    
		/** Generates the font descriptor for this font.
		 * @return the PdfDictionary containing the font descriptor or <CODE>null</CODE>
		 * @param subsetPrefix the subset prefix
		 * @param fontStream the indirect reference to a PdfStream containing the font or <CODE>null</CODE>
		 * @throws DocumentException if there is an error
		 */
		protected PdfDictionary getFontDescriptor(PdfIndirectReference fontStream, string subsetPrefix) {
			PdfDictionary dic = new PdfDictionary(new PdfName("FontDescriptor"));
			dic.put(new PdfName("Ascent"), new PdfNumber((int)os_2.sTypoAscender * 1000 / head.unitsPerEm));
			dic.put(new PdfName("CapHeight"), new PdfNumber((int)os_2.sCapHeight * 1000 / head.unitsPerEm));
			dic.put(new PdfName("Descent"), new PdfNumber((int)os_2.sTypoDescender * 1000 / head.unitsPerEm));
			dic.put(new PdfName("FontBBox"), new PdfRectangle(
				(int)head.xMin * 1000 / head.unitsPerEm,
				(int)head.yMin * 1000 / head.unitsPerEm,
				(int)head.xMax * 1000 / head.unitsPerEm,
				(int)head.yMax * 1000 / head.unitsPerEm));
			dic.put(new PdfName("FontName"), new PdfName(subsetPrefix + fontName + style));
			dic.put(new PdfName("ItalicAngle"), new PdfNumber(italicAngle));
			dic.put(new PdfName("StemV"), new PdfNumber(80));
			if (fontStream != null) {
				if (cff)
					dic.put(new PdfName("FontFile3"), fontStream);
				else
					dic.put(new PdfName("FontFile2"), fontStream);
			}
			int flags = 0;
			if (isFixedPitch)
				flags |= 1;
			flags |= fontSpecific ? 4 : 32;
			if ((head.macStyle & 2) != 0)
				flags |= 64;
			if ((head.macStyle & 1) != 0)
				flags |= 262144;
			dic.put(new PdfName("Flags"), new PdfNumber(flags));
        
			return dic;
		}
    
		/** Generates the font dictionary for this font.
		 * @return the PdfDictionary containing the font dictionary
		 * @param subsetPrefix the subset prefx
		 * @param firstChar the first valid character
		 * @param lastChar the last valid character
		 * @param shortTag a 256 bytes long <CODE>byte</CODE> array where each unused byte is represented by 0
		 * @param fontDescriptor the indirect reference to a PdfDictionary containing the font descriptor or <CODE>null</CODE>
		 * @throws DocumentException if there is an error
		 */
		protected PdfDictionary getFontBaseType(PdfIndirectReference fontDescriptor, string subsetPrefix, int firstChar, int lastChar, byte[] shortTag) {
			PdfDictionary dic = new PdfDictionary(PdfName.FONT);
			if (cff)
				dic.put(PdfName.SUBTYPE, new PdfName("Type1"));
			else
				dic.put(PdfName.SUBTYPE, new PdfName("TrueType"));
			dic.put(PdfName.BASEFONT, new PdfName(subsetPrefix + fontName + style));
			if (!fontSpecific) {
				for (int k = firstChar; k <= lastChar; ++k) {
					if (!differences[k].Equals(notdef)) {
						firstChar = k;
						break;
					}
				}
				if (encoding.Equals("windows-1252") || encoding.Equals("MacRoman"))
					dic.put(PdfName.ENCODING, encoding.Equals("windows-1252") ? PdfName.WIN_ANSI_ENCODING : PdfName.MAC_ROMAN_ENCODING);
				else {
					PdfDictionary enc = new PdfDictionary(new PdfName("Encoding"));
					PdfArray dif = new PdfArray();
					bool gap = true;                
					for (int k = firstChar; k <= lastChar; ++k) {
						if (shortTag[k] != 0) {
							if (gap) {
								dif.Add(new PdfNumber(k));
								gap = false;
							}
							dif.Add(new PdfName(differences[k]));
						}
						else
							gap = true;
					}
					enc.put(new PdfName("Differences"), dif);
					dic.put(PdfName.ENCODING, enc);
				}
			}
			dic.put(new PdfName("FirstChar"), new PdfNumber(firstChar));
			dic.put(new PdfName("LastChar"), new PdfNumber(lastChar));
			PdfArray wd = new PdfArray();
			for (int k = firstChar; k <= lastChar; ++k) {
				if (shortTag[k] == 0)
					wd.Add(new PdfNumber(0));
				else
					wd.Add(new PdfNumber(widths[k]));
			}
			dic.put(new PdfName("Widths"), wd);
			if (fontDescriptor != null)
				dic.put(new PdfName("FontDescriptor"), fontDescriptor);
			return dic;
		}
    
		/** Outputs to the writer the font dictionaries and streams.
		 * @param writer the writer for this document
		 * @param ref the font indirect reference
		 * @param params several parameters that depend on the font type
		 * @throws IOException on error
		 * @throws DocumentException error in generating the object
		 */
		internal override void writeFont(PdfWriter writer, PdfIndirectReference piref, Object[] parms) {
			int firstChar = (int)parms[0];
			int lastChar = (int)parms[1];
			byte[] shortTag = (byte[])parms[2];
			if (!subset) {
				firstChar = 0;
				lastChar = shortTag.Length - 1;
				for (int k = 0; k < shortTag.Length; ++k)
					shortTag[k] = 1;
			}
			PdfIndirectReference ind_font = null;
			PdfObject pobj = null;
			PdfIndirectObject obj = null;
			string subsetPrefix = "";
			if (embedded) {
				if (cff) {
					byte[] b = new byte[cffLength];
					try {
						rf.reOpen();
						rf.seek(cffOffset);
						rf.readFully(b);
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
					pobj = new StreamFont(b, "Type1C");
					obj = writer.addToBody(pobj);
					ind_font = obj.IndirectReference;
				}
				else {
					subsetPrefix = createSubsetPrefix();
					Hashmap glyphs = new Hashmap();
					for (int k = firstChar; k <= lastChar; ++k) {
						if (shortTag[k] != 0) {
							int[] metrics;
							if (fontSpecific)
								metrics = getMetricsTT(k);
							else
								metrics = getMetricsTT(unicodeDifferences[k]);
							if (metrics != null)
								glyphs.Add(metrics[0], null);
						}
					}
					TrueTypeFontSubSet sb = new TrueTypeFontSubSet(fileName, rf, glyphs, directoryOffset, true);
					byte[] b = sb.process();
					int[] lengths = new int[]{b.Length};
					pobj = new StreamFont(b, lengths);
					obj = writer.addToBody(pobj);
					ind_font = obj.IndirectReference;
				}
			}
			pobj = getFontDescriptor(ind_font, subsetPrefix);
			if (pobj != null){
				obj = writer.addToBody(pobj);
				ind_font = obj.IndirectReference;
			}
			pobj = getFontBaseType(ind_font, subsetPrefix, firstChar, lastChar, shortTag);
			writer.addToBody(pobj, piref);
		}
    
		/** Gets the font parameter identified by <CODE>key</CODE>. Valid values
		 * for <CODE>key</CODE> are <CODE>ASCENT</CODE>, <CODE>CAPHEIGHT</CODE>, <CODE>DESCENT</CODE>
		 * and <CODE>ITALICANGLE</CODE>.
		 * @param key the parameter to be extracted
		 * @param fontSize the font size in points
		 * @return the parameter in points
		 */    
		public override float getFontDescriptor(int key, float fontSize) {
			switch (key) {
				case ASCENT:
					return (float)os_2.sTypoAscender * fontSize / (float)head.unitsPerEm;
				case CAPHEIGHT:
					return (float)os_2.sCapHeight * fontSize / (float)head.unitsPerEm;
				case DESCENT:
					return (float)os_2.sTypoDescender * fontSize / (float)head.unitsPerEm;
				case ITALICANGLE:
					return (float)italicAngle;
				case BBOXLLX:
					return fontSize * (int)head.xMin / head.unitsPerEm;
				case BBOXLLY:
					return fontSize * (int)head.yMin / head.unitsPerEm;
				case BBOXURX:
					return fontSize * (int)head.xMax / head.unitsPerEm;
				case BBOXURY:
					return fontSize * (int)head.yMax / head.unitsPerEm;
				case AWT_ASCENT:
					return fontSize * (int)hhea.Ascender / head.unitsPerEm;
				case AWT_DESCENT:
					return fontSize * (int)hhea.Descender / head.unitsPerEm;
				case AWT_LEADING:
					return fontSize * (int)hhea.LineGap / head.unitsPerEm;
				case AWT_MAXADVANCE:
					return fontSize * (int)hhea.advanceWidthMax / head.unitsPerEm;
			}
			return 0;
		}
    
		/** Gets the glyph index and metrics for a character.
		 * @param c the character
		 * @return an <CODE>int</CODE> array with {glyph index, width}
		 */    
		public int[] getMetricsTT(int c) {
			if (!fontSpecific && cmap31 != null) 
				return (int[])cmap31[c];
			if (fontSpecific && cmap10 != null) 
				return (int[])cmap10[c];
			return null;
		}

		/** Gets the postscript font name.
		 * @return the postscript font name
		 */
		public override string PostscriptFontName {
			get {
				return fontName;
			}
		}

		/** Gets the code pages supported by the font.
		 * @return the code pages supported by the font
		 */
		public override string[] CodePagesSupported {
			get {
				long cp = (((long)os_2.ulCodePageRange2) << 32) + ((long)os_2.ulCodePageRange1 & 0xffffffffL);
				int count = 0;
				long bit = 1;
				for (int k = 0; k < 64; ++k) {
					if ((cp & bit) != 0 && codePages[k] != null)
						++count;
					bit <<= 1;
				}
				string[] ret = new string[count];
				count = 0;
				bit = 1;
				for (int k = 0; k < 64; ++k) {
					if ((cp & bit) != 0 && codePages[k] != null)
						ret[count++] = codePages[k];
					bit <<= 1;
				}
				return ret;
			}
		}
    
		/** Gets the full name of the font. If it is a True Type font
		 * each array element will have {Platform ID, Platform Encoding ID,
		 * Language ID, font name}. The interpretation of this values can be
		 * found in the Open Type specification, chapter 2, in the 'name' table.<br>
		 * For the other fonts the array has a single element with {"", "", "",
		 * font name}.
		 * @return the full name of the font
		 */
		public override string[][] FullFontName {
			get {
				return fullName;
			}
		}
    
		/** Gets the family name of the font. If it is a True Type font
		 * each array element will have {Platform ID, Platform Encoding ID,
		 * Language ID, font name}. The interpretation of this values can be
		 * found in the Open Type specification, chapter 2, in the 'name' table.<br>
		 * For the other fonts the array has a single element with {"", "", "",
		 * font name}.
		 * @return the family name of the font
		 */
		public override string[][] FamilyFontName {
			get {
				return familyName;
			}
		}
    
	}
}