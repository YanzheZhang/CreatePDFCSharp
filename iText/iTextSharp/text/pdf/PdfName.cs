using System;
using System.Text;

/*
 * $Id: PdfName.cs,v 1.3 2003/04/04 20:38:28 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie
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
	 * <CODE>PdfName</CODE> is an object that can be used as a name in a PDF-file.
	 * <P>
	 * A name, like a string, is a sequence of characters. It must begin with a slash
	 * followed by a sequence of ASCII characters in the range 32 through 136 except
	 * %, (, ), [, ], &lt;, &gt;, {, }, / and #. Any character except 0x00 may be included
	 * in a name by writing its twocharacter hex code, preceded by #. The maximum number
	 * of characters in a name is 127.<BR>
	 * This object is described in the 'Portable Document Format Reference Manual version 1.3'
	 * section 4.5 (page 39-40).
	 * <P>
	 *
	 * @see		PdfObject
	 * @see		PdfDictionary
	 * @see		BadPdfFormatException
	 */

	public class PdfName : PdfObject, IComparable {
    
		// static membervariables (a variety of standard names used in PDF)
    
		/** This is a static PdfName */
		public static PdfName A = new PdfName("A");
    
		/** This is a static PdfName */
		public static PdfName AA = new PdfName("AA");
    
		/** This is a static PdfName */
		public static PdfName AC = new PdfName("AC");
    
		/** This is a static PdfName */
		public static PdfName ACROFORM = new PdfName("AcroForm");
    
		/** This is a static PdfName */
		public static PdfName ACTION = new PdfName("Action");
    
		/** This is a static PdfName */
		public static PdfName ALTERNATE = new PdfName("Alternate");
    
		/** This is a static PdfName */
		public static PdfName ANNOT = new PdfName("Annot");
    
		/** This is a static PdfName */
		public static PdfName ANTIALIAS = new PdfName("AntiAlias");
    
		/** This is a static PdfName */
		public static PdfName ANNOTS = new PdfName("Annots");
    
		/** This is a static PdfName */
		public static PdfName AP = new PdfName("AP");
    
		/** This is a static PdfName */
		public static PdfName AS = new PdfName("AS");
    
		/** This is a static PdfName */
		public static PdfName ASCII85DECODE = new PdfName("ASCII85Decode");
    
		/** This is a static PdfName */
		public static PdfName ASCIIHEXDECODE = new PdfName("ASCIIHexDecode");
    
		/** This is a static PdfName */
		public static PdfName AUTHOR = new PdfName("Author");
    
		/** This is a static PdfName */
		public static PdfName B = new PdfName("B");
    
		/** This is a static PdfName */
		public static PdfName BASEFONT = new PdfName("BaseFont");
    
		/** This is a static PdfName */
		public static PdfName BBOX = new PdfName("BBox");
    
		/** This is a static PdfName */
		public static PdfName BC = new PdfName("BC");
    
		/** This is a static PdfName */
		public static PdfName BG = new PdfName("BG");
    
		/** This is a static PdfName */
		public static PdfName BITSPERCOMPONENT = new PdfName("BitsPerComponent");
    
		/** This is a static PdfName */
		public static PdfName BITSPERSAMPLE = new PdfName("BitsPerSample");
    
		/** This is a static PdfName */
		public static PdfName BL = new PdfName("Bl");
    
		/** This is a static PdfName */
		public static PdfName BLACKIS1 = new PdfName("BlackIs1");
    
		/** This is a static PdfName */
		public static PdfName BORDER = new PdfName("Border");
    
		/** This is a static PdfName */
		public static PdfName BOUNDS = new PdfName("Bounds");
    
		/** This is a static PdfName */
		public static PdfName BS = new PdfName("BS");
    
		/** This is a static PdfName */
		public static PdfName BTN = new PdfName("Btn");
    
		/** This is a static PdfName */
		public static PdfName BYTERANGE = new PdfName("ByteRange");
    
		/** This is a static PdfName */
		public static PdfName C = new PdfName("C");
    
		/** This is a static PdfName */
		public static PdfName C0 = new PdfName("C0");
    
		/** This is a static PdfName */
		public static PdfName C1 = new PdfName("C1");
    
		/** This is a static PdfName */
		public static PdfName CA = new PdfName("CA");
    
		/** This is a static PdfName */
		public static PdfName CATALOG = new PdfName("Catalog");
    
		/** This is a static PdfName */
		public static PdfName CCITTFAXDECODE = new PdfName("CCITTFaxDecode");
    
		/** This is a static PdfName */
		public static PdfName CENTERWINDOW = new PdfName("CenterWindow");
    
		/** This is a static PdfName */
		public static PdfName CH = new PdfName("Ch");
    
		/** This is a static PdfName */
		public static PdfName CIRCLE = new PdfName("Circle");
    
		/** This is a static PdfName */
		public static PdfName CO = new PdfName("CO");
    
		/** This is a static PdfName */
		public static PdfName COLORS = new PdfName("Colors");
    
		/** This is a static PdfName */
		public static PdfName COLORSPACE = new PdfName("ColorSpace");
    
		/** This is a static PdfName */
		public static PdfName COLUMNS = new PdfName("Columns");
    
		/** This is a static PdfName */
		public static PdfName CONTENT = new PdfName("Content");
    
		/** This is a static PdfName */
		public static PdfName CONTENTS = new PdfName("Contents");
    
		/** This is a static PdfName */
		public static PdfName COORDS = new PdfName("Coords");
    
		/** This is a static PdfName */
		public static PdfName COUNT = new PdfName("Count");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName COURIER = new PdfName("Courier");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName COURIER_BOLD = new PdfName("Courier-Bold");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName COURIER_OBLIQUE = new PdfName("Courier-Oblique");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName COURIER_BOLDOBLIQUE = new PdfName("Courier-BoldOblique");
    
		/** This is a static PdfName */
		public static PdfName CREATIONDATE = new PdfName("CreationDate");
    
		/** This is a static PdfName */
		public static PdfName CREATOR = new PdfName("Creator");
    
		/** This is a static PdfName */
		public static PdfName CROPBOX = new PdfName("CropBox");
    
		/** This is a static PdfName */
		public static PdfName D = new PdfName("D");
    
		/** This is a static PdfName */
		public static PdfName DA = new PdfName("DA");
    
		/** This is a static PdfName */
		public static PdfName DC = new PdfName("DC");
    
		/** This is a static PdfName */
		public static PdfName DCTDECODE = new PdfName("DCTDecode");
    
		/** This is a static PdfName */
		public static PdfName DECODE = new PdfName("Decode");
    
		/** This is a static PdfName */
		public static PdfName DECODEPARMS = new PdfName("DecodeParms");
    
		/** This is a static PdfName */
		public static PdfName DEST = new PdfName("Dest");
    
		/** This is a static PdfName */
		public static PdfName DESTS = new PdfName("Dests");
    
		/** This is a static PdfName */
		public static PdfName DEVICEGRAY = new PdfName("DeviceGray");
    
		/** This is a static PdfName */
		public static PdfName DEVICERGB = new PdfName("DeviceRGB");
    
		/** This is a static PdfName */
		public static PdfName DEVICECMYK = new PdfName("DeviceCMYK");
    
		/** This is a static PdfName */
		public static PdfName DI = new PdfName("Di");

		/** This is a static PdfName */
		public static PdfName DIRECTION = new PdfName("Direction");
    
		/** This is a static PdfName */
		public static PdfName DM = new PdfName("Dm");
    
		/** This is a static PdfName */
		public static PdfName DOMAIN = new PdfName("Domain");
    
		/** This is a static PdfName */
		public static PdfName DP = new PdfName("DP");
    
		/** This is a static PdfName */
		public static PdfName DR = new PdfName("DR");
    
		/** This is a static PdfName */
		public static PdfName DS = new PdfName("DS");
    
		/** This is a static PdfName */
		public static PdfName DUR = new PdfName("Dur");
    
		/** This is a static PdfName */
		public static PdfName DV = new PdfName("DV");
    
		/** This is a static PdfName */
		public static PdfName E = new PdfName("E");
    
		/** This is a static PdfName */
		public static PdfName EARLYCHANGE = new PdfName("EarlyChange");
    
		/** This is a static PdfName */
		public static PdfName ENCODE = new PdfName("Encode");
    
		/** This is a static PdfName */
		public static PdfName ENCODEDBYTEALIGN = new PdfName("EncodedByteAlign");
    
		/** This is a static PdfName */
		public new static PdfName ENCODING = new PdfName("Encoding");
    
		/** This is a static PdfName */
		public static PdfName ENCRYPT = new PdfName("Encrypt");
    
		/** This is a static PdfName */
		public static PdfName ENDOFBLOCK = new PdfName("EndOfBlock");
    
		/** This is a static PdfName */
		public static PdfName ENDOFLINE = new PdfName("EndOfLine");
    
		/** This is a static PdfName */
		public static PdfName EXTEND = new PdfName("Extend");
    
		/** This is a static PdfName */
		public static PdfName EXTGSTATE = new PdfName("ExtGState");
    
		/** This is a static PdfName */
		public static PdfName F = new PdfName("F");
    
		/** This is a static PdfName */
		public static PdfName FDECODEPARMS = new PdfName("FDecodeParms");
    
		/** This is a static PdfName */
		public static PdfName FF = new PdfName("Ff");
    
		/** This is a static PdfName */
		public static PdfName FFILTER = new PdfName("FFilter");
    
		/** This is a static PdfName */
		public static PdfName FIELDS = new PdfName("Fields");
    
		/** This is a static PdfName */
		public static PdfName FILTER = new PdfName("Filter");
    
		/** This is a static PdfName */
		public static PdfName FIRST = new PdfName("First");
    
		/** This is a static PdfName */
		public static PdfName FIRSTCHAR = new PdfName("FirstChar");
    
		/** This is a static PdfName */
		public static PdfName FIRSTPAGE = new PdfName("FirstPage");
    
		/** This is a static PdfName */
		public static PdfName FIT = new PdfName("Fit");
    
		/** This is a static PdfName */
		public static PdfName FITH = new PdfName("FitH");
    
		/** This is a static PdfName */
		public static PdfName FITV = new PdfName("FitV");
    
		/** This is a static PdfName */
		public static PdfName FITR = new PdfName("FitR");
    
		/** This is a static PdfName */
		public static PdfName FITB = new PdfName("FitB");
    
		/** This is a static PdfName */
		public static PdfName FITBH = new PdfName("FitBH");
    
		/** This is a static PdfName */
		public static PdfName FITBV = new PdfName("FitBV");
    
		/** This is a static PdfName */
		public static PdfName FITWINDOW = new PdfName("FitWindow");
    
		/** This is a static PdfName */
		public static PdfName FLAGS = new PdfName("Flags");
    
		/** This is a static PdfName */
		public static PdfName FLATEDECODE = new PdfName("FlateDecode");
    
		/** This is a static PdfName */
		public static PdfName FO = new PdfName("Fo");
    
		/** This is a static PdfName */
		public static PdfName FONT = new PdfName("Font");
    
		/** This is a static PdfName */
		public static PdfName FONTDESCRIPTOR = new PdfName("FontDescriptor");
    
		/** This is a static PdfName */
		public static PdfName FORM = new PdfName("Form");
    
		/** This is a static PdfName */
		public static PdfName FORMTYPE = new PdfName("FormType");
    
		/** This is a static PdfName */
		public static PdfName FREETEXT = new PdfName("FreeText");
    
		/** This is a static PdfName */
		public static PdfName FS = new PdfName("FS");
    
		/** This is a static PdfName */
		public static PdfName FT = new PdfName("FT");
    
		/** This is a static PdfName */
		public static PdfName FULLSCREEN = new PdfName("FullScreen");
    
		/** This is a static PdfName */
		public static PdfName FUNCTION = new PdfName("Function");
    
		/** This is a static PdfName */
		public static PdfName FUNCTIONS = new PdfName("Functions");
    
		/** This is a static PdfName */
		public static PdfName FUNCTIONTYPE = new PdfName("FunctionType");
    
		/** This is a static PdfName of an attribute. */
		public static PdfName GOTO = new PdfName("GoTo");
    
		/** This is a static PdfName of an attribute. */
		public static PdfName GOTOR = new PdfName("GoToR");
    
		/** This is a static PdfName of an attribute. */
		public static PdfName H = new PdfName("H");
    
		/** This is a static PdfName of an attribute. */
		public static PdfName HEIGHT = new PdfName("Height");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName HELVETICA = new PdfName("Helvetica");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName HELVETICA_BOLD = new PdfName("Helvetica-Bold");
    
		/** This is a static PdfName PdfName of a base 14 type 1 font */
		public static PdfName HELVETICA_OBLIQUE = new PdfName("Helvetica-Oblique");
    
		/** This is a static PdfName PdfName of a base 14 type 1 font */
		public static PdfName HELVETICA_BOLDOBLIQUE = new PdfName("Helvetica-BoldOblique");
    
		/** This is a static PdfName */
		public static PdfName HID = new PdfName("Hid");
    
		/** This is a static PdfName */
		public static PdfName HIDE = new PdfName("Hide");
    
		/** This is a static PdfName */
		public static PdfName HIDEMENUBAR = new PdfName("HideMenubar");
    
		/** This is a static PdfName */
		public static PdfName HIDETOOLBAR = new PdfName("HideToolbar");
    
		/** This is a static PdfName */
		public static PdfName HIDEWINDOWUI = new PdfName("HideWindowUI");
    
		/** This is a static PdfName */
		public static PdfName HIGHLIGHT = new PdfName("Highlight");
    
		/** This is a static PdfName */
		public static PdfName I = new PdfName("I");
    
		/** This is a static PdfName */
		public static PdfName ICCBASED = new PdfName("ICCBased");
    
		/** This is a static PdfName */
		public static PdfName ID = new PdfName("ID");
    
		/** This is a static PdfName */
		public static PdfName IF = new PdfName("IF");
    
		/** This is a static PdfName */
		public static PdfName IMAGE = new PdfName("Image");
    
		/** This is a static PdfName */
		public static PdfName IMAGEB = new PdfName("ImageB");
    
		/** This is a static PdfName */
		public static PdfName IMAGEC = new PdfName("ImageC");
    
		/** This is a static PdfName */
		public static PdfName IMAGEI = new PdfName("ImageI");
    
		/** This is a static PdfName */
		public static PdfName IMAGEMASK = new PdfName("ImageMask");
    
		/** This is a static PdfName */
		public static PdfName INDEXED = new PdfName("Indexed");
    
		/** This is a static PdfName */
		public static PdfName INFO = new PdfName("Info");
    
		/** This is a static PdfName */
		public static PdfName INKLIST = new PdfName("InkList");
    
		/** This is a static PdfName */
		public static PdfName IMPORTDATA = new PdfName("ImportData");
    
		/** This is a static PdfName */
		public static PdfName INTERPOLATE = new PdfName("Interpolate");
    
		/** This is a static PdfName */
		public static PdfName ISMAP = new PdfName("IsMap");
    
		/** This is a static PdfName */
		public static PdfName IX = new PdfName("IX");
    
		/** This is a static PdfName */
		public static PdfName JAVASCRIPT = new PdfName("JavaScript");
    
		/** This is a static PdfName */
		public static PdfName JS = new PdfName("JS");
    
		/** This is a static PdfName */
		public static PdfName K = new PdfName("K");
    
		/** This is a static PdfName */
		public static PdfName KEYWORDS = new PdfName("Keywords");
    
		/** This is a static PdfName */
		public static PdfName KIDS = new PdfName("Kids");
    
		/** This is a static PdfName */
		public static PdfName L = new PdfName("L");
    
		/** This is a static PdfName */
		public static PdfName L2R = new PdfName("L2R");
    
		/** This is a static PdfName */
		public static PdfName LAST = new PdfName("Last");
    
		/** This is a static PdfName */
		public static PdfName LASTCHAR = new PdfName("LastChar");
    
		/** This is a static PdfName */
		public static PdfName LASTPAGE = new PdfName("LastPage");
    
		/** This is a static PdfName */
		public static PdfName LAUNCH = new PdfName("Launch");
    
		/** This is a static PdfName */
		public static PdfName LENGTH = new PdfName("Length");
    
		/** This is a static PdfName */
		public static PdfName LIMITS = new PdfName("Limits");
    
		/** This is a static PdfName */
		public static PdfName LINE = new PdfName("Line");
    
		/** This is a static PdfName */
		public static PdfName LINK = new PdfName("Link");
    
		/** This is a static PdfName */
		public static PdfName LOCATION = new PdfName("Location");
    
		/** This is a static PdfName */
		public static PdfName LZWDECODE = new PdfName("LZWDecode");
    
		/** This is a static PdfName */
		public static PdfName M = new PdfName("M");
    
		/** This is a static PdfName */
		public static PdfName MATRIX = new PdfName("Matrix");
    
		/** This is a static PdfName of an encoding */
		public static PdfName MAC_EXPERT_ENCODING = new PdfName("MacExpertEncoding");
    
		/** This is a static PdfName of an encoding */
		public static PdfName MAC_ROMAN_ENCODING = new PdfName("MacRomanEncoding");
    
		/** This is a static PdfName of an encoding */
		public static PdfName MASK = new PdfName("Mask");
    
		/** This is a static PdfName of an encoding */
		public static PdfName MAXLEN = new PdfName("MaxLen");
    
		/** This is a static PdfName */
		public static PdfName MEDIABOX = new PdfName("MediaBox");
    
		/** This is a static PdfName of an encoding */
		public static PdfName MK = new PdfName("MK");
    
		/** This is a static PdfName */
		public static PdfName MODDATE = new PdfName("ModDate");
    
		/** This is a static PdfName */
		public static PdfName N = new PdfName("N");
    
		/** This is a static PdfName */
		public new static PdfName NAME = new PdfName("Name");
    
		/** This is a static PdfName */
		public static PdfName NAMED = new PdfName("Named");
    
		/** This is a static PdfName */
		public static PdfName NAMES = new PdfName("Names");
    
		/** This is a static PdfName */
		public static PdfName NEXT = new PdfName("Next");
    
		/** This is a static PdfName */
		public static PdfName NEXTPAGE = new PdfName("NextPage");
    
		/** This is a static PdfName */
		public static PdfName NONFULLSCREENPAGEMODE = new PdfName("NonFullScreenPageMode");
    
		/** This is a static PdfName */
		public static PdfName O = new PdfName("O");
    
		/** This is a static PdfName */
		public static PdfName ONECOLUMN = new PdfName("OneColumn");
    
		/** This is a static PdfName */
		public static PdfName OPEN = new PdfName("Open");
    
		/** This is a static PdfName */
		public static PdfName OPENACTION = new PdfName("OpenAction");
    
		/** This is a static PdfName */
		public static PdfName OPT = new PdfName("Opt");
    
		/** This is a static PdfName */
		public static PdfName ORDER = new PdfName("Order");
    
		/** This is a static PdfName */
		public static PdfName ORDERING = new PdfName("Ordering");
    
		/** This is a static PdfName */
		public static PdfName OUTLINES = new PdfName("Outlines");
    
		/** This is a static PdfName */
		public static PdfName P = new PdfName("P");
    
		/** This is a static PdfName */
		public static PdfName PAGE = new PdfName("Page");
    
		/** This is a static PdfName */
		public static PdfName PAGELABELS = new PdfName("PageLabels");
    
		/** This is a static PdfName */
		public static PdfName PAGELAYOUT = new PdfName("PageLayout");
    
		/** This is a static PdfName */
		public static PdfName PAGEMODE = new PdfName("PageMode");
    
		/** This is a static PdfName */
		public static PdfName PAGES = new PdfName("Pages");
    
		/** This is a static PdfName */
		public static PdfName PAINTTYPE = new PdfName("PaintType");
    
		/** This is a static PdfName */
		public static PdfName PANOSE = new PdfName("Panose");
    
		/** This is a static PdfName */
		public static PdfName PARENT = new PdfName("Parent");
    
		/** This is a static PdfName */
		public static PdfName PATTERN = new PdfName("Pattern");
    
		/** This is a static PdfName */
		public static PdfName PATTERNTYPE = new PdfName("PatternType");
    
		/** This is a static PdfName */
		public static PdfName PDF = new PdfName("PDF");
    
		/** This is a static PdfName */
		public static PdfName POPUP = new PdfName("Popup");
    
		/** This is a static PdfName */
		public static PdfName PREDICTOR = new PdfName("Predictor");
    
		/** This is a static PdfName */
		public static PdfName PREV = new PdfName("Prev");
    
		/** This is a static PdfName */
		public static PdfName PREVPAGE = new PdfName("PrevPage");
    
		/** This is a static PdfName */
		public static PdfName PROCSET = new PdfName("ProcSet");
    
		/** This is a static PdfName */
		public static PdfName PRODUCER = new PdfName("Producer");
    
		/** This is a static PdfName */
		public static PdfName PROPERTIES = new PdfName("Properties");
    
		/** This is a static PdfName */
		public static PdfName Q = new PdfName("Q");
    
		/** This is a static PdfName */
		public static PdfName QUADPOINTS = new PdfName("QuadPoints");
    
		/** This is a static PdfName */
		public static PdfName R = new PdfName("R");
    
		/** This is a static PdfName */
		public static PdfName R2L = new PdfName("R2L");
    
		/** This is a static PdfName */
		public static PdfName RANGE = new PdfName("Range");
    
		/** This is a static PdfName */
		public static PdfName RC = new PdfName("RC");
    
		/** This is a static PdfName */
		public static PdfName REASON = new PdfName("Reason");
    
		/** This is a static PdfName */
		public static PdfName RECT = new PdfName("Rect");
    
		/** This is a static PdfName */
		public static PdfName REGISTRY = new PdfName("Registry");
    
		/** This is a static PdfName */
		public static PdfName RESETFORM = new PdfName("ResetForm");
    
		/** This is a static PdfName */
		public static PdfName RESOURCES = new PdfName("Resources");
    
		/** This is a static PdfName */
		public static PdfName RI = new PdfName("RI");
    
		/** This is a static PdfName */
		public static PdfName ROOT = new PdfName("Root");
    
		/** This is a static PdfName */
		public static PdfName ROTATE = new PdfName("Rotate");
    
		/** This is a static PdfName */
		public static PdfName ROWS = new PdfName("Rows");
    
		/** This is a static PdfName */
		public static PdfName RUNLENGTHDECODE = new PdfName("RunLengthDecode");
    
		/** This is a static PdfName */
		public static PdfName S = new PdfName("S");
    
		/** This is a static PdfName */
		public static PdfName SEPARATION = new PdfName("Separation");
    
		/** This is a static PdfName */
		public static PdfName SHADING = new PdfName("Shading");
    
		/** This is a static PdfName */
		public static PdfName SHADINGTYPE = new PdfName("ShadingType");
    
		/** This is a static PdfName */
		public static PdfName SIG = new PdfName("Sig");
    
		/** This is a static PdfName */
		public static PdfName SIGFLAGS = new PdfName("SigFlags");
    
		/** This is a static PdfName */
		public static PdfName SINGLEPAGE = new PdfName("SinglePage");
    
		/** This is a static PdfName */
		public static PdfName SIZE = new PdfName("Size");
    
		/** This is a static PdfName */
		public static PdfName SQUARE = new PdfName("Square");
    
		/** This is a static PdfName */
		public static PdfName STAMP = new PdfName("Stamp");
    
		/** This is a static PdfName */
		public static PdfName STANDARD = new PdfName("Standard");
    
		/** This is a static PdfName */
		public static PdfName STRIKEOUT = new PdfName("StrikeOut");
    
		/** This is a static PdfName */
		public static PdfName SUBFILTER = new PdfName("SubFilter");
    
		/** This is a static PdfName */
		public static PdfName SUBJECT = new PdfName("Subject");
    
		/** This is a static PdfName */
		public static PdfName SUBMITFORM = new PdfName("SubmitForm");
    
		/** This is a static PdfName */
		public static PdfName SUBTYPE = new PdfName("Subtype");
    
		/** This is a static PdfName */
		public static PdfName SUPPLEMENT = new PdfName("Supplement");
    
		/** This is a static PdfName */
		public static PdfName SW = new PdfName("SW");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName SYMBOL = new PdfName("Symbol");
    
		/** This is a static PdfName */
		public static PdfName T = new PdfName("T");
    
		/** This is a static PdfName */
		public static PdfName TEXT = new PdfName("Text");
    
		/** This is a static PdfName */
		public static PdfName THUMB = new PdfName("Thumb");
    
		/** This is a static PdfName */
		public static PdfName THREADS = new PdfName("Threads");
    
		/** This is a static PdfName */
		public static PdfName TI = new PdfName("TI");
    
		/** This is a static PdfName */
		public static PdfName TILINGTYPE = new PdfName("TilingType");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName TIMES_ROMAN = new PdfName("Times-Roman");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName TIMES_BOLD = new PdfName("Times-Bold");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName TIMES_ITALIC = new PdfName("Times-Italic");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName TIMES_BOLDITALIC = new PdfName("Times-BoldItalic");
    
		/** This is a static PdfName */
		public static PdfName TITLE = new PdfName("Title");
    
		/** This is a static PdfName */
		public static PdfName TM = new PdfName("TM");
    
		/** This is a static PdfName */
		public static PdfName TP = new PdfName("TP");
    
		/** This is a static PdfName */
		public static PdfName TRANS = new PdfName("Trans");
    
		/** This is a static PdfName */
		public static PdfName TU = new PdfName("TU");
    
		/** This is a static PdfName */
		public static PdfName TWOCOLUMNLEFT = new PdfName("TwoColumnLeft");
    
		/** This is a static PdfName */
		public static PdfName TWOCOLUMNRIGHT = new PdfName("TwoColumnRight");
    
		/** This is a static PdfName */
		public static PdfName TX = new PdfName("Tx");
    
		/** This is a static PdfName */
		public static PdfName TYPE = new PdfName("Type");
    
		/** This is a static PdfName */
		public static PdfName TYPE1 = new PdfName("Type1");
    
		/** This is a static PdfName of an attribute. */
		public static PdfName U = new PdfName("U");
    
		/** This is a static PdfName of an attribute. */
		public static PdfName UNDERLINE = new PdfName("Underline");
    
		/** This is a static PdfName */
		public static PdfName URI = new PdfName("URI");
    
		/** This is a static PdfName */
		public static PdfName URL = new PdfName("URL");
    
		/** This is a static PdfName */
		public static PdfName USENONE = new PdfName("UseNone");
    
		/** This is a static PdfName */
		public static PdfName USEOUTLINES = new PdfName("UseOutlines");
    
		/** This is a static PdfName */
		public static PdfName USETHUMBS = new PdfName("UseThumbs");
    
		/** This is a static PdfName */
		public static PdfName V = new PdfName("V");
    
		/** This is a static PdfName */
		public static PdfName VIEWERPREFERENCES = new PdfName("ViewerPreferences");
    
		/** This is a static PdfName of an attribute. */
		public static PdfName W = new PdfName("W");
    
		/** This is a static PdfName of an attribute. */
		public static PdfName WIDGET = new PdfName("Widget");
    
		/** This is a static PdfName of an attribute. */
		public static PdfName WIDTH = new PdfName("Width");
    
		/** This is a static PdfName */
		public static PdfName WIDTHS = new PdfName("Widths");
    
		/** This is a static PdfName of an encoding */
		public static PdfName WIN = new PdfName("Win");
    
		/** This is a static PdfName of an encoding */
		public static PdfName WIN_ANSI_ENCODING = new PdfName("WinAnsiEncoding");
    
		/** This is a static PdfName of an encoding */
		public static PdfName WP = new PdfName("WP");
    
		/** This is a static PdfName of an encoding */
		public static PdfName WS = new PdfName("WS");
    
		/** This is a static PdfName */
		public static PdfName X = new PdfName("X");
    
		/** This is a static PdfName */
		public static PdfName XOBJECT = new PdfName("XObject");
    
		/** This is a static PdfName */
		public static PdfName XSTEP = new PdfName("XStep");
    
		/** This is a static PdfName */
		public static PdfName XYZ = new PdfName("XYZ");
    
		/** This is a static PdfName */
		public static PdfName YSTEP = new PdfName("YStep");
    
		/** This is a static PdfName of a base 14 type 1 font */
		public static PdfName ZAPFDINGBATS = new PdfName("ZapfDingbats");
    
		private int hash = 0;
    
		// constructors
    
		/**
		 * Constructs a <CODE>PdfName</CODE>-object.
		 *
		 * @param		name		the new Name.
		 */
    
		public PdfName(string name) : base(PdfObject.NAME, name) {
			// The minimum number of characters in a name is 1, the maximum is 127 (the '/' not included)
			if (bytes.Length < 1 || bytes.Length > 127) {
				throw new IllegalArgumentException("The name is too long (" + bytes.Length + " characters).");
			}
			// The name has to be checked for illegal characters
			int length = name.Length;
			for (int i = 0; i < length; i++) {
				if (name[i] < 32 || name[i] > 255) {
					throw new IllegalArgumentException("Illegal character on position " + i + ".");
				}
			}
			// every special character has to be substituted
			StringBuilder pdfName = new StringBuilder("/");
			char character;
			// loop over all the characters
			for (int index = 0; index < length; index++) {
				character = name[index];
				// special characters are escaped (reference manual p.39)
				switch (character) {
					case ' ':
					case '%':
					case '(':
					case ')':
					case '<':
					case '>':
					case '[':
					case ']':
					case '{':
					case '}':
					case '/':
					case '#':
						pdfName.Append('#');
						pdfName.Append(System.Convert.ToString(character, 16));
						break;
					default:
						if (character > 126) {
							pdfName.Append('#');
							pdfName.Append(System.Convert.ToString(character, 16));
						}
						else
							pdfName.Append(character);
						break;
				}
			}
			this.Content = pdfName.ToString();
		}
    
		// methods
    
		/**
		 * Compares this object with the specified object for order.  Returns a
		 * negative int, zero, or a positive int as this object is less
		 * than, equal to, or greater than the specified object.<p>
		 *
		 *
		 * @param   object the Object to be compared.
		 * @return  a negative int, zero, or a positive int as this object
		 *		is less than, equal to, or greater than the specified object.
		 *
		 * @throws Exception if the specified object's type prevents it
		 *         from being compared to this Object.
		 */
		public int CompareTo(Object obj) {
			PdfName name = (PdfName) obj;
        
			byte[] myBytes = bytes;
			byte[] objBytes = name.bytes;
			int len = Math.Min(myBytes.Length, objBytes.Length);
			for(int i=0; i<len; i++) {
				if(myBytes[i] > objBytes[i])
					return 1;
            
				if(myBytes[i] < objBytes[i])
					return -1;
			}
			if (myBytes.Length < objBytes.Length)
				return -1;
			if (myBytes.Length > objBytes.Length)
				return 1;
			return 0;
		}
    
		/**
		 * Indicates whether some other object is "equal to" this one.
		 *
		 * @param   obj   the reference object with which to compare.
		 * @return  <code>true</code> if this object is the same as the obj
		 *          argument; <code>false</code> otherwise.
		 */
		public override bool Equals(Object obj) {
			return this.CompareTo(obj) == 0;
		}
    
		/**
		 * Returns a hash code value for the object. This method is
		 * supported for the benefit of hashtables such as those provided by
		 * <code>java.util.Hashtable</code>.
		 *
		 * @return  a hash code value for this object.
		 */
		public override int GetHashCode() {
			int h = hash;
			if (h == 0) {
				int ptr = 0;
				int len = bytes.Length;
            
				for (int i = 0; i < len; i++)
					h = 31*h + (bytes[ptr++] & 0xff);
				hash = h;
			}
			return h;
		}
    
		/** Decodes an escaped name in the form "/AB#20CD" into "AB CD".
		 * @param name the name to decode
		 * @return the decoded name
		 */
		public static string decodeName(string name) {
			StringBuilder buf = new StringBuilder();
			try {
				int len = name.Length;
				for (int k = 1; k < len; ++k) {
					char c = name[k];
					if (c == '#') {
						c = (char)((PRTokeniser.getHex(name[k + 1]) << 4) + PRTokeniser.getHex(name[k + 2]));
						k += 2;
					}
					buf.Append(c);
				}
			}
			catch (Exception e) {
				e.GetType();
				// empty on purpose
			}
			return buf.ToString();
		}
	}
}
