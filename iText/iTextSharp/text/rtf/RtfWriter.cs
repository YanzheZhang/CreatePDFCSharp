using System;
using System.Net;
using System.IO;
using System.Collections;
using System.Text;

using iTextSharp.text;

/**
 * $Id: RtfWriter.cs,v 1.4 2003/03/22 22:01:24 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 by Mark Hall
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
 * LGPL license (the “GNU LIBRARY GENERAL PUBLIC LICENSE”), in which case the
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

namespace iTextSharp.text.rtf {
	/// <summary>
	/// A DocWriter class for Rich Text Files (RTF).
	/// </summary>
	/// <remarks>
	/// A RtfWriter can be added as a DocListener
	/// to a certain Document by getting an instance.
	/// Every Element added to the original Document
	/// will be written to the Stream of this RtfWriter.
	/// </remarks>
	/// <example>
	/// <code>
	/// // creation of the document with a certain size and certain margins
	/// Document document = new Document(PageSize.A4, 50, 50, 50, 50);
	/// try {
	///		// this will write RTF to the Standard Stream
	///		<strong>RtfWriter.getInstance(document, System.out);</strong>
	///		// this will write Rtf to a file called text.rtf
	///		<strong>RtfWriter.getInstance(document, new FileStream("text.rtf"));</strong>
	///		// this will write Rtf to for instance the Stream of a HttpServletResponse-object
	///		<strong>RtfWriter.getInstance(document, response.getStream());</strong>
	///	}
	///	catch(Exception de) {
	///		Console.Error.WriteLine(de.Message);
	///	}
	///	// this will close the document and all the Streams listening to it
	///	<strong>document.close();</strong>
	///	</code>
	///	<P/>
	///	<strong>LIMITATIONS</strong><BR/>
	///	There are currently still a few limitations on what the RTF Writer can do:
	///	<UL>
	///		<LI>Only PNG / JPEG Images are supported.</LI>
	///		<LI>Rotating of Images is not supported.</LI>
	///		<LI>Nested Tables are not supported.</LI>
	///		<LI>The Leading is not supported.</LI>
	///		</UL>
	/// </example>
	public class RtfWriter : DocWriter, IDocListener {
		/*
		 * Static Constants
		 */
    
		/*
		 * General
		 */
    
		/// <summary> This is the escape character which introduces RTF tags. </summary>
		public static byte escape = (byte) '\\';

		/// <summary> This is another escape character which introduces RTF tags. </summary>
		private static byte[] extendedEscape  = System.Text.ASCIIEncoding.ASCII.GetBytes("\\*\\");

		/// <summary> This is the delimiter between RTF tags and normal text. </summary>
		internal static byte delimiter = (byte) ' ';

		/// <summary> This is another delimiter between RTF tags and normal text. </summary>
		private static byte commaDelimiter = (byte) ';';

		/// <summary> This is the character for beginning a new group. </summary>
		public static byte openGroup = (byte) '{';
    
		/// <summary> This is the character for closing a group. </summary>
		public static byte closeGroup = (byte) '}';
    
		/*
		 * RTF Information
		 */
    
		/// <summary> RTF begin and version. </summary>
		private static byte[] docBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("rtf1");
    
		/// <summary> RTF encoding. </summary>
		private static byte[] ansi  = System.Text.ASCIIEncoding.ASCII.GetBytes("ansi");
    
		/// <summary> RTF encoding codepage. </summary>
		private static byte[] ansiCodepage  = System.Text.ASCIIEncoding.ASCII.GetBytes("ansicpg");
    
		/*
		 *Font Data
		 */
    
		/// <summary> Begin the font table tag. </summary>
		private static byte[] fontTable  = System.Text.ASCIIEncoding.ASCII.GetBytes("fonttbl");
    
		/// <summary> Font number tag. </summary>
		protected static byte fontNumber = (byte) 'f';
    
		/// <summary> Font size tag. </summary>
		protected static byte[] fontSize  = System.Text.ASCIIEncoding.ASCII.GetBytes("fs");

		/// <summary> Font color tag. </summary>
		protected static byte[] fontColor  = System.Text.ASCIIEncoding.ASCII.GetBytes("cf");
    
		/// <summary> Modern font tag. </summary>
		private static byte[] fontModern  = System.Text.ASCIIEncoding.ASCII.GetBytes("fmodern");
    
		/// <summary> Swiss font tag. </summary>
		private static byte[] fontSwiss  = System.Text.ASCIIEncoding.ASCII.GetBytes("fswiss");
    
		/// <summary> Roman font tag. </summary>
		private static byte[] fontRoman  = System.Text.ASCIIEncoding.ASCII.GetBytes("froman");
    
		/// <summary> Tech font tag. </summary>
		private static byte[] fontTech  = System.Text.ASCIIEncoding.ASCII.GetBytes("ftech");
    
		/// <summary> Font charset tag. </summary>
		private static byte[] fontCharset  = System.Text.ASCIIEncoding.ASCII.GetBytes("fcharset");
    
		/// <summary> Font Courier tag. </summary>
		private static byte[] fontCourier  = System.Text.ASCIIEncoding.ASCII.GetBytes("Courier");
    
		/// <summary> Font Arial tag. </summary>
		private static byte[] fontArial  = System.Text.ASCIIEncoding.ASCII.GetBytes("Arial");
    
		/// <summary> Font Symbol tag. </summary>
		private static byte[] fontSymbol  = System.Text.ASCIIEncoding.ASCII.GetBytes("Symbol");
    
		/// <summary> Font Times New Roman tag. </summary>
		private static byte[] fontTimesNewRoman  = System.Text.ASCIIEncoding.ASCII.GetBytes("Times New Roman");
    
		/// <summary> Font Windings tag. </summary>
		private static byte[] fontWindings  = System.Text.ASCIIEncoding.ASCII.GetBytes("Windings");
    
		/// <summary> Default Font. </summary>
		private static byte[] defaultFont  = System.Text.ASCIIEncoding.ASCII.GetBytes("deff");
    
		/// <summary> First indent tag. </summary>
		private static byte[] firstIndent  = System.Text.ASCIIEncoding.ASCII.GetBytes("fi");
    
		/// <summary> List indent tag. </summary>
		private static byte[] listIndent  = System.Text.ASCIIEncoding.ASCII.GetBytes("li");
    
		/*
		 * Sections / Paragraphs
		 */
    
		/// <summary> Reset section defaults tag. </summary>
		private static byte[] sectionDefaults  = System.Text.ASCIIEncoding.ASCII.GetBytes("sectd");
    
		/// <summary> Begin new section tag. </summary>
		private static byte[] section  = System.Text.ASCIIEncoding.ASCII.GetBytes("sect");

		/// <summary> Reset paragraph defaults tag. </summary>
		public static byte[] paragraphDefaults  = System.Text.ASCIIEncoding.ASCII.GetBytes("pard");

		/// <summary> Begin new paragraph tag. </summary>
		public static byte[] paragraph  = System.Text.ASCIIEncoding.ASCII.GetBytes("par");
    
		/*
		 * Lists
		 */

		/// <summary> Begin the List Table </summary>
		private static byte[] listtableGroup  = System.Text.ASCIIEncoding.ASCII.GetBytes("listtable");
    
		/// <summary> Begin the List Override Table </summary>
		private static byte[] listoverridetableGroup  = System.Text.ASCIIEncoding.ASCII.GetBytes("listoverridetable");
    
		/// <summary> Begin a List definition </summary>
		private static byte[] listDefinition  = System.Text.ASCIIEncoding.ASCII.GetBytes("list");
    
		/// <summary> List Template ID </summary>
		private static byte[] listTemplateID  = System.Text.ASCIIEncoding.ASCII.GetBytes("listtemplateid");
    
		/// <summary> RTF Writer outputs hybrid lists </summary>
		private static byte[] hybridList  = System.Text.ASCIIEncoding.ASCII.GetBytes("hybrid");
    
		/// <summary> Current List level </summary>
		private static byte[] listLevelDefinition  = System.Text.ASCIIEncoding.ASCII.GetBytes("listlevel");
    
		/// <summary> Level numbering (old) </summary>
		private static byte[] listLevelTypeOld  = System.Text.ASCIIEncoding.ASCII.GetBytes("levelnfc");
    
		/// <summary> Level numbering (new) </summary>
		private static byte[] listLevelTypeNew  = System.Text.ASCIIEncoding.ASCII.GetBytes("levelnfcn");
    
		/// <summary> Level alignment (old) </summary>
		private static byte[] listLevelAlignOld  = System.Text.ASCIIEncoding.ASCII.GetBytes("leveljc");
    
		/// <summary> Level alignment (new) </summary>
		private static byte[] listLevelAlignNew  = System.Text.ASCIIEncoding.ASCII.GetBytes("leveljcn");
    
		/// <summary> Level starting number </summary>
		private static byte[] listLevelStartAt  = System.Text.ASCIIEncoding.ASCII.GetBytes("levelstartat");
    
		/// <summary> Level text group </summary>
		private static byte[] listLevelTextDefinition  = System.Text.ASCIIEncoding.ASCII.GetBytes("leveltext");
    
		/// <summary> Filler for Level Text Length </summary>
		private static byte[] listLevelTextLength  = System.Text.ASCIIEncoding.ASCII.GetBytes("\'0");
    
		/// <summary> Level Text Numbering Style </summary>
		private static byte[] listLevelTextStyleNumbers  = System.Text.ASCIIEncoding.ASCII.GetBytes("\'00.");
    
		/// <summary> Level Text Bullet Style </summary>
		private static byte[] listLevelTextStyleBullet  = System.Text.ASCIIEncoding.ASCII.GetBytes("u-3913 ?");
    
		/// <summary> Level Numbers Definition </summary>
		private static byte[] listLevelNumbersDefinition  = System.Text.ASCIIEncoding.ASCII.GetBytes("levelnumbers");
    
		/// <summary> Filler for Level Numbers </summary>
		private static byte[] listLevelNumbers  = System.Text.ASCIIEncoding.ASCII.GetBytes("\\'0");
    
		/// <summary> Tab Stop </summary>
		private static byte[] tabStop  = System.Text.ASCIIEncoding.ASCII.GetBytes("tx");
    
		/// <summary> Actual list begin </summary>
		private static byte[] listBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("ls");
    
		/// <summary> Current list level </summary>
		private static byte[] listCurrentLevel  = System.Text.ASCIIEncoding.ASCII.GetBytes("ilvl");
    
		/// <summary> List text group for older browsers </summary>
		private static byte[] listTextOld  = System.Text.ASCIIEncoding.ASCII.GetBytes("listtext");
    
		/// <summary> Tab </summary>
		private static byte[] tab  = System.Text.ASCIIEncoding.ASCII.GetBytes("tab");
    
		/// <summary> Old Bullet Style </summary>
		private static byte[] listBulletOld  = System.Text.ASCIIEncoding.ASCII.GetBytes("\'b7");
    
		/// <summary> Current List ID </summary>
		private static byte[] listID  = System.Text.ASCIIEncoding.ASCII.GetBytes("listid");

		/// <summary> List override </summary>
		private static byte[] listOverride  = System.Text.ASCIIEncoding.ASCII.GetBytes("listoverride");

		/// <summary> Number of overrides </summary>
		private static byte[] listOverrideCount  = System.Text.ASCIIEncoding.ASCII.GetBytes("listoverridecount");
    
		/*
		 * Text Style
		 */
    
		/// <summary> Bold tag. </summary>
		protected static byte bold = (byte) 'b';
    
		/// <summary> Italic tag. </summary>
		protected static byte italic = (byte) 'i';
    
		/// <summary> Underline tag. </summary>
		protected static byte[] underline  = System.Text.ASCIIEncoding.ASCII.GetBytes("ul");
    
		/// <summary> Strikethrough tag. </summary>
		protected static byte[] strikethrough  = System.Text.ASCIIEncoding.ASCII.GetBytes("strike");
    
		/// <summary> Text alignment left tag. </summary>
		public static byte[] alignLeft  = System.Text.ASCIIEncoding.ASCII.GetBytes("ql");

		/// <summary> Text alignment center tag. </summary>
		private static byte[] alignCenter  = System.Text.ASCIIEncoding.ASCII.GetBytes("qc");
    
		/// <summary> Text alignment right tag. </summary>
		private static byte[] alignRight  = System.Text.ASCIIEncoding.ASCII.GetBytes("qr");
    
		/// <summary> Text alignment justify tag. </summary>
		private static byte[] alignJustify  = System.Text.ASCIIEncoding.ASCII.GetBytes("qj");
    
		/*
		 * Colors
		 */
    
		/// <summary> Begin colour table tag. </summary>
		private static byte[] colorTable  = System.Text.ASCIIEncoding.ASCII.GetBytes("colortbl");

		/// <summary> Red value tag. </summary>
		private static byte[] colorRed  = System.Text.ASCIIEncoding.ASCII.GetBytes("red");

		/// <summary> Green value tag. </summary>
		private static byte[] colorGreen  = System.Text.ASCIIEncoding.ASCII.GetBytes("green");

		/// <summary> Blue value tag. </summary>
		private static byte[] colorBlue  = System.Text.ASCIIEncoding.ASCII.GetBytes("blue");
    
		/*
		 * Information Group
		 */
    
		/// <summary> Begin the info group tag.</summary>
		private static byte[] infoBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("info");

		/// <summary> Author tag. </summary>
		private static byte[] metaAuthor  = System.Text.ASCIIEncoding.ASCII.GetBytes("author");
    
		/// <summary> Subject tag. </summary>
		private static byte[] metaSubject  = System.Text.ASCIIEncoding.ASCII.GetBytes("subject");
    
		/// <summary> Keywords tag. </summary>
		private static byte[] metaKeywords  = System.Text.ASCIIEncoding.ASCII.GetBytes("keywords");

		/// <summary> Title tag. </summary>
		private static byte[] metaTitle  = System.Text.ASCIIEncoding.ASCII.GetBytes("title");
    
		/// <summary> Producer tag. </summary>
		private static byte[] metaProducer  = System.Text.ASCIIEncoding.ASCII.GetBytes("operator");

		/// <summary> Creation Date tag. </summary>
		private static byte[] metaCreationDate  = System.Text.ASCIIEncoding.ASCII.GetBytes("creationdate");

		/// <summary> Year tag. </summary>
		private static byte[] year  = System.Text.ASCIIEncoding.ASCII.GetBytes("yr");

		/// <summary> Month tag. </summary>
		private static byte[] month  = System.Text.ASCIIEncoding.ASCII.GetBytes("mo");

		/// <summary> Day tag. </summary>
		private static byte[] day  = System.Text.ASCIIEncoding.ASCII.GetBytes("dy");

		/// <summary> Hour tag. </summary>
		private static byte[] hour  = System.Text.ASCIIEncoding.ASCII.GetBytes("hr");

		/// <summary> Minute tag. </summary>
		private static byte[] minute  = System.Text.ASCIIEncoding.ASCII.GetBytes("min");

		/// <summary> Second tag. </summary>
		private static byte[] second  = System.Text.ASCIIEncoding.ASCII.GetBytes("sec");

		/// <summary> Start superscript. </summary>
		private static byte[] startSuper  = System.Text.ASCIIEncoding.ASCII.GetBytes("super");

		/// <summary> Start subscript. </summary>
		private static byte[] startSub  = System.Text.ASCIIEncoding.ASCII.GetBytes("sub");

		/// <summary> End super/sub script. </summary>
		private static byte[] endSuperSub  = System.Text.ASCIIEncoding.ASCII.GetBytes("nosupersub");

		/*
		 * Header / Footer
		 */

		/// <summary> Title Page tag </summary>
		private static byte[] titlePage  = System.Text.ASCIIEncoding.ASCII.GetBytes("titlepg");

		/// <summary> Facing pages tag </summary>
		private static byte[] facingPages  = System.Text.ASCIIEncoding.ASCII.GetBytes("facingp");

		/// <summary> Begin header group tag. </summary>
		private static byte[] headerBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("header");

		/// <summary> Begin footer group tag. </summary>
		private static byte[] footerBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("footer");

		// header footer 'left', 'right', 'first'
		private static byte[] headerlBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("headerl");

		private static byte[] footerlBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("footerl");

		private static byte[] headerrBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("headerr");

		private static byte[] footerrBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("footerr");

		private static byte[] headerfBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("headerf");

		private static byte[] footerfBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("footerf");

		/*
		 * Paper Properties
		 */

		/// <summary> Paper width tag. </summary>
		private static byte[] rtfPaperWidth  = System.Text.ASCIIEncoding.ASCII.GetBytes("paperw");

		/// <summary> Paper height tag. </summary>
		private static byte[] rtfPaperHeight  = System.Text.ASCIIEncoding.ASCII.GetBytes("paperh");

		/// <summary> Margin left tag. </summary>
		private static byte[] rtfMarginLeft  = System.Text.ASCIIEncoding.ASCII.GetBytes("margl");

		/// <summary> Margin right tag. </summary>
		private static byte[] rtfMarginRight  = System.Text.ASCIIEncoding.ASCII.GetBytes("margr");

		/// <summary> Margin top tag. </summary>
		private static byte[] rtfMarginTop  = System.Text.ASCIIEncoding.ASCII.GetBytes("margt");

		/// <summary> Margin bottom tag. </summary>
		private static byte[] rtfMarginBottom  = System.Text.ASCIIEncoding.ASCII.GetBytes("margb");

		/// <summary> New Page tag. </summary>
		private static byte[] newPage  = System.Text.ASCIIEncoding.ASCII.GetBytes("page");

		/// <summary> Document Landscape tag 1. </summary>
		private static byte[] landscapeTag1  = System.Text.ASCIIEncoding.ASCII.GetBytes("landscape");

		/// <summary> Document Landscape tag 2. </summary>
		private static byte[] landscapeTag2  = System.Text.ASCIIEncoding.ASCII.GetBytes("lndscpsxn");

		/*
		 * Annotations
		 */
    
		/// <summary> Annotation ID tag. </summary>
		private static byte[] annotationID  = System.Text.ASCIIEncoding.ASCII.GetBytes("atnid");
    
		/// <summary> Annotation Author tag. </summary>
		private static byte[] annotationAuthor  = System.Text.ASCIIEncoding.ASCII.GetBytes("atnauthor");
    
		/// <summary> Annotation text tag. </summary>
		private static byte[] annotation  = System.Text.ASCIIEncoding.ASCII.GetBytes("annotation");
    
		/*
		 * Images
		 */
    
		/// <summary> Begin the main Picture group tag </summary>
		private static byte[] pictureGroup  = System.Text.ASCIIEncoding.ASCII.GetBytes("shppict");

		/// <summary> Begin the picture tag </summary>
		private static byte[] picture  = System.Text.ASCIIEncoding.ASCII.GetBytes("pict");

		/// <summary> PNG Image </summary>
		private static byte[] picturePNG  = System.Text.ASCIIEncoding.ASCII.GetBytes("pngblip");

		/// <summary> JPEG Image </summary>
		private static byte[] pictureJPEG  = System.Text.ASCIIEncoding.ASCII.GetBytes("jpegblip");

		/// <summary> Picture width </summary>
		private static byte[] pictureWidth  = System.Text.ASCIIEncoding.ASCII.GetBytes("picw");

		/// <summary> Picture height </summary>
		private static byte[] pictureHeight  = System.Text.ASCIIEncoding.ASCII.GetBytes("pich");
    
		/// <summary> Picture scale horizontal percent </summary>
		private static byte[] pictureScaleX  = System.Text.ASCIIEncoding.ASCII.GetBytes("picscalex");

		/// <summary> Picture scale vertical percent </summary>
		private static byte[] pictureScaleY  = System.Text.ASCIIEncoding.ASCII.GetBytes("picscaley");
    
		/*
		 * Fields (for page numbering)
		 */
    
		/// <summary> Begin field tag </summary>
		internal static byte[] field  = System.Text.ASCIIEncoding.ASCII.GetBytes("field");
    
		/// <summary> Content fo the field </summary>
		internal static byte[] fieldContent  = System.Text.ASCIIEncoding.ASCII.GetBytes("fldinst");
    
		/// <summary> PAGE numbers </summary>
		internal static byte[] fieldPage  = System.Text.ASCIIEncoding.ASCII.GetBytes("PAGE");
    
		/// <summary> HYPERLINK field </summary>
		internal static byte[] fieldHyperlink  = System.Text.ASCIIEncoding.ASCII.GetBytes("HYPERLINK");
    
		/// <summary> Last page number (not used) </summary>
		internal static byte[] fieldDisplay  = System.Text.ASCIIEncoding.ASCII.GetBytes("fldrslt");

    
		/// <summary> Class variables </summary>
    
		/*
		 * Because of the way RTF works and the way itext works, the text has to be
		 * stored and is only written to the actual Stream at the end.
		 */
    
		/// <summary> This Vector contains all fonts used in the document. </summary>
		private ArrayList fontList = new ArrayList();
    
		/// <summary> This Vector contains all colours used in the document. </summary>
		private ArrayList colorList = new ArrayList();

		/// <summary> This MemoryStream contains the main body of the document. </summary>
		private MemoryStream content = null;
    
		/// <summary> This MemoryStream contains the information group. </summary>
		private MemoryStream info = null;
    
		/// <summary> This MemoryStream contains the list table. </summary>
		private MemoryStream listtable = null;
    
		/// <summary> This MemoryStream contains the list override table. </summary>
		private MemoryStream listoverride = null;
    
		/// <summary> Document header. </summary>
		private HeaderFooter header = null;
    
		/// <summary> Document footer. </summary>
		private HeaderFooter footer = null;
    
		/// <summary> Left margin. </summary>
		private int marginLeft = 1800;
    
		/// <summary> Right margin. </summary>
		private int marginRight = 1800;

		/// <summary> Top margin. </summary>
		private int marginTop = 1440;
    
		/// <summary> Bottom margin. </summary>
		private int marginBottom = 1440;

		/// <summary> Page width. </summary>
		private int pageWidth = 11906;

		/// <summary> Page height. </summary>
		private int pageHeight = 16838;
    
		/// <summary> Factor to use when converting. </summary>
		public static double twipsFactor = 20;//20.57140;
    
		/// <summary> Current list ID. </summary>
		private int currentListID = 1;
    
		/// <summary> List of current Lists. </summary>
		private ArrayList listIds = null;

		/// <summary> Current List Level. </summary>
		private int listLevel = 0;

		/// <summary> Current maximum List Level. </summary>
		private int maxListLevel = 0;

		/// <summary> Write a TOC </summary>
		private bool writeTOC = false;

		/// <summary> Special title page </summary>
		private bool hasTitlePage = false;

		/// <summary> Currently writing either Header or Footer </summary>
		private bool inHeaderFooter = false;

		/// <summary> Currently writing a Table </summary>
		private bool inTable = false;

		/// <summary> Landscape or Portrait Document </summary>
		private bool landscape = false;

		Random r = new Random();

		/// <summary> Protected Constructor </summary>

		/// <summary>
		/// Constructs a RtfWriter.
		/// </summary>
		/// <param name="doc">The Document that is to be written as RTF</param>
		/// <param name="os">The Stream the writer has to write to.</param>
		protected RtfWriter(Document doc, Stream os) : base(doc, os) {
			document.addDocListener(this);
			initDefaults();
		}

		/// <summary>
		/// Get/set the current setting of writeTOC
		/// </summary>
		/// <value>bool value indicating whether a TOC is being generated</value>
		public bool GeneratingTOCEntries {
			get {
				return writeTOC;
			}

			set {
				this.writeTOC = value;
			}
		}

		/// <summary>
		/// Get/set the current setting of hasTitlePage
		/// </summary>
		/// <value>bool value indicating whether the first page is a title page</value>
		public bool HasTitlePage {
			get {
				return hasTitlePage;
			}

			set {
				this.hasTitlePage = value;
			}
		}

		/// <summary>
		/// Get/set the current landscape setting
		/// </summary>
		/// <value>bool value indicating the current page format</value>
		public bool Landscape {
			get {
				return landscape;
			}

			set {
				this.landscape = value;
			}
		}


		/// <summary>
		/// Gets an instance of the RtfWriter.
		/// </summary>
		/// <param name="document">The Document that has to be written</param>
		/// <param name="os">The Stream the writer has to write to.</param>
		/// <returns>a new RtfWriter</returns>
		public static RtfWriter getInstance(Document document, Stream os) {
			return(new RtfWriter(document, os));
		}

		/// <summary>
		/// Signals that the Document has been opened and that
		/// Elements can be added.
		/// </summary>
		public override void Open() {
			base.Open();
		}
    
		/// <summary>
		/// Signals that the Document was closed and that no other
		/// Elements will be added.
		/// </summary>
		/// <remarks>
		/// The content of the font table, color table, information group, content, header, footer are merged into the final
		/// Stream
		/// </remarks>
		public override void Close() {
			writeDocument();
			base.Close();
		}
    
		/// <summary>
		/// Adds the footer to the bottom of the Document.
		/// </summary>
		/// <value>the footer</value>
		public HeaderFooter Footer {
			set {
				this.footer = value;
			}
		}
    
		/// <summary>
		/// Adds the header to the top of the Document.
		/// </summary>
		/// <value>the header</value>
		public HeaderFooter Header {
			set {
				this.header = value;
			}
		}
    
		/// <summary>
		/// Resets the footer.
		/// </summary>
		public void resetFooter() {
			this.Footer = null;
		}
    
		/// <summary>
		/// Resets the header.
		/// </summary>
		public void resetHeader() {
			this.Header =  null;
		}
    
		/// <summary>
		/// Tells the RtfWriter that a new page is to be begun.
		/// </summary>
		/// <returns>true if a new Page was begun.</returns>
		public bool NewPage() {
			try {
				content.WriteByte(escape);
				content.Write(newPage, 0, newPage.Length);
				content.WriteByte(escape);
				content.Write(paragraph, 0, paragraph.Length);
			}
			catch(IOException e) {
				return false;
			}
			return true;
		}
    
		/// <summary>
		/// Sets the page margins
		/// </summary>
		/// <param name="marginLeft">The left margin</param>
		/// <param name="marginRight">The right margin</param>
		/// <param name="marginTop">The top margin</param>
		/// <param name="marginBottom">The bottom margin</param>
		/// <returns>true if the page margins were set.</returns>
		public bool setMargins(float marginLeft, float marginRight, float marginTop, float marginBottom) {
			this.marginLeft = (int) (marginLeft * twipsFactor);
			this.marginRight = (int) (marginRight * twipsFactor);
			this.marginTop = (int) (marginTop * twipsFactor);
			this.marginBottom = (int) (marginBottom * twipsFactor);
			return true;
		}

		/// <summary>
		/// Sets the page size
		/// </summary>
		/// <param name="pageSize">A Rectangle specifying the page size</param>
		/// <returns>true if the page size was set</returns>
		public bool setPageSize(Rectangle pageSize) {
			if(!parseFormat(pageSize, false)) {
				pageWidth = (int) (pageSize.Width * twipsFactor);
				pageHeight = (int) (pageSize.Height * twipsFactor);
				landscape = pageWidth > pageHeight;
			}
			return true;
		}

		/// <summary>
		/// Write the table of contents.
		/// </summary>
		/// <param name="tocTitle">The title that will be displayed above the TOC</param>
		/// <param name="titleFont">The Font that will be used for the tocTitle</param>
		/// <param name="showTOCasEntry">Set this to true if you want the TOC to appear as an entry in the TOC</param>
		/// <param name="showTOCEntryFont">Use this Font to specify what Font to use when showTOCasEntry is true</param>
		/// <returns>true if the TOC was added.</returns>
		public bool WriteTOC(String tocTitle, Font titleFont, bool showTOCasEntry, Font showTOCEntryFont) {
			try {
				RtfTOC toc = new RtfTOC(tocTitle, titleFont);
				if(showTOCasEntry) { toc.AddTOCAsTOCEntry(tocTitle, showTOCEntryFont); }
				Add(new Paragraph(toc));
			}
			catch(DocumentException de) { return false; }
			return true;
		}
    
		/// <summary>
		/// Signals that an Element was added to the Document.
		/// </summary>
		/// <param name="element">the Element added</param>
		/// <returns>true if the element was added, false if not.</returns>
		public override bool Add(IElement element) {
			return addElement(element, content);
		}
    

		/// <summary> Private functions </summary>
    
		/// <summary>
		/// Adds an Element to the Document.
		/// </summary>
		/// <param name="element">the Element</param>
		/// <param name="str">the Stream</param>
		/// <returns>true if the element was added, false if not.</returns>
		internal bool addElement(IElement element, MemoryStream str) {
			try {
				switch(element.Type) {
					case Element.CHUNK        : writeChunk((Chunk) element, str);                break;
					case Element.PARAGRAPH    : writeParagraph((Paragraph) element, str);        break;
					case Element.ANCHOR       : writeAnchor((Anchor) element, str);              break;
					case Element.PHRASE       : writePhrase((Phrase) element, str);              break;
					case Element.CHAPTER      :
					case Element.SECTION      : writeSection((Section) element, str);            break;
					case Element.LIST         : writeList((iTextSharp.text.List) element, str); break;
					case Element.TABLE        : writeTable((Table) element, str);                  break;
					case Element.ANNOTATION   : writeAnnotation((Annotation) element, str);      break;
					case Element.PNG          :
					case Element.JPEG         : writeImage((Image) element, str);                break;
                
					case Element.AUTHOR       : writeMeta(metaAuthor, (Meta) element);       break;
					case Element.SUBJECT      : writeMeta(metaSubject, (Meta) element);      break;
					case Element.KEYWORDS     : writeMeta(metaKeywords, (Meta) element);     break;
					case Element.TITLE        : writeMeta(metaTitle, (Meta) element);        break;
					case Element.PRODUCER     : writeMeta(metaProducer, (Meta) element);     break;
					case Element.CREATIONDATE : writeMeta(metaCreationDate, (Meta) element); break;
				}
			}
			catch(IOException e) {
				return false;
			}
			return true;
		}

		/// <summary>
		/// Write the beginning of a new Section
		/// </summary>
		/// <param name="sectionElement">The Section be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writeSection(Section sectionElement, MemoryStream str) {
			if(sectionElement.Type == Element.CHAPTER) {
				str.WriteByte(escape);
				str.Write(sectionDefaults, 0, sectionDefaults.Length);
				writeSectionDefaults(str);
			}
			if(sectionElement.Title != null) {
				if(writeTOC) {
					StringBuilder title = new StringBuilder("");
					foreach (Chunk chunk in sectionElement.Title.Chunks) {
						title.Append(chunk.Content);
					}
					Add(new RtfTOCEntry(title.ToString(), sectionElement.Title.Font));
				}
				else {
					sectionElement.Title.process(this);
				}
				str.WriteByte(escape);
				str.Write(paragraph, 0, paragraph.Length);
			}
			sectionElement.process(this);
			if(sectionElement.Type == Element.CHAPTER) {
				str.WriteByte(escape);
				str.Write(section, 0, section.Length);
			}
			if(sectionElement.Type == Element.SECTION) {
				str.WriteByte(escape);
				str.Write(paragraph, 0, paragraph.Length);
			}
		}
    
		/// <summary>
		/// Write the beginning of a new Paragraph
		/// </summary>
		/// <param name="paragraphElement">The Paragraph to be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writeParagraph(Paragraph paragraphElement, MemoryStream str) {
			str.WriteByte(escape);
			str.Write(paragraphDefaults, 0, paragraphDefaults.Length);
			if(inTable) {
				str.WriteByte(escape);
				str.Write(RtfCell.cellInTable, 0, RtfCell.cellInTable.Length);
			}
			switch(paragraphElement.Alignment) {
				case Element.ALIGN_LEFT      : str.WriteByte(escape); str.Write(alignLeft, 0, alignLeft.Length); break;
				case Element.ALIGN_RIGHT     : str.WriteByte(escape); str.Write(alignRight, 0, alignRight.Length); break;
				case Element.ALIGN_CENTER    : str.WriteByte(escape); str.Write(alignCenter, 0, alignCenter.Length); break;
				case Element.ALIGN_JUSTIFIED : str.WriteByte(escape); str.Write(alignJustify, 0, alignJustify.Length); break;
			}
			str.WriteByte(escape);
			str.Write(listIndent, 0, listIndent.Length);
			writeInt(str, (int) (paragraphElement.IndentationLeft * twipsFactor));
			foreach (Chunk ch in paragraphElement.Chunks) {
				ch.Font = paragraphElement.Font.difference(ch.Font);
			}
			MemoryStream save = content;
			content = str;
			paragraphElement.process(this);
			content = save;
			if(!inTable) {
				str.WriteByte(escape);
				str.Write(paragraph, 0, paragraph.Length);
			}
		}

		/// <summary>
		/// Write a Phrase.
		/// </summary>
		/// <param name="phrase">The Phrase item to be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writePhrase(Phrase phrase, MemoryStream str) {
			str.WriteByte(escape);
			str.Write(paragraphDefaults, 0, paragraphDefaults.Length);
			if(inTable) {
				str.WriteByte(escape);
				str.Write(RtfCell.cellInTable, 0, RtfCell.cellInTable.Length);
			}
			foreach (Chunk ch in phrase.Chunks) {
				ch.Font = phrase.Font.difference(ch.Font);
			}
			MemoryStream save = content;
			content = str;
			phrase.process(this);
			content = save;
		}

		/// <summary>
		/// Write an Anchor. Anchors are treated like Phrases.
		/// </summary>
		/// <param name="anchor">The Chunk item to be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writeAnchor(Anchor anchor, MemoryStream str) {
			if (anchor.Url != null) {
				str.WriteByte(openGroup);
				str.WriteByte(escape);
				str.Write(field, 0, field.Length);
				str.WriteByte(openGroup);
				str.Write(extendedEscape, 0, extendedEscape.Length);
				str.Write(fieldContent, 0, fieldContent.Length);
				str.WriteByte(openGroup);
				str.Write(fieldHyperlink, 0, fieldHyperlink.Length);
				str.WriteByte(delimiter);
				str.Write(ASCIIEncoding.ASCII.GetBytes(anchor.Url.ToString()), 0, 
					ASCIIEncoding.ASCII.GetBytes(anchor.Url.ToString()).Length);
				str.WriteByte(closeGroup);
				str.WriteByte(closeGroup);
				str.WriteByte(openGroup);
				str.WriteByte(escape);
				str.Write(fieldDisplay, 0, fieldDisplay.Length);
				str.WriteByte(delimiter);
				writePhrase(anchor, str);
				str.WriteByte(closeGroup);
				str.WriteByte(closeGroup);
			}
			else {
				writePhrase(anchor, str);
			}
		}

		/// <summary>
		/// Write a Chunk and all its font properties.
		/// </summary>
		/// <param name="chunk">The Chunk item to be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writeChunk(Chunk chunk, MemoryStream str) {
			if (chunk is IRtfField) {
				((IRtfField)chunk).Write(this, str);
			} else {
				if(chunk.Image != null) {
					writeImage(chunk.Image, str);
				}
				else {
					writeInitialFontSignature(str, chunk);
					str.Write(ASCIIEncoding.ASCII.GetBytes(filterSpecialChar(chunk.Content)), 0, 
						ASCIIEncoding.ASCII.GetBytes(filterSpecialChar(chunk.Content)).Length);
					writeFinishingFontSignature(str, chunk);
				}
			}
		}


		internal void writeInitialFontSignature(Stream str, Chunk chunk) {
			Font font = chunk.Font;

			str.WriteByte(escape);
			str.WriteByte(fontNumber);
			if (!font.Familyname.ToLower().Equals("unknown")) {
				writeInt(str, addFont(font));
			} else {
				writeInt(str, 0);
			}
			str.WriteByte(escape);
			str.Write(fontSize, 0, fontSize.Length);
			if (font.Size > 0) {
				writeInt(str, (int)(font.Size * 2));
			} else {
				writeInt(str, 20);
			}
			str.WriteByte(escape);
			str.Write(fontColor, 0, fontColor.Length);
			writeInt(str, addColor(font.Color));
			if (font.isBold()) {
				str.WriteByte(escape);
				str.WriteByte(bold);
			}
			if (font.isItalic()) {
				str.WriteByte(escape);
				str.WriteByte(italic);
			}
			if (font.isUnderlined()) {
				str.WriteByte(escape);
				str.Write(underline, 0, underline.Length);
			}
			if (font.isStrikethru()) {
				str.WriteByte(escape);
				str.Write(strikethrough, 0, strikethrough.Length);
			}

			/*
			 * Superscript / Subscript added by Scott Dietrich (sdietrich@emlab.com)
			 */
			if (chunk.Attributes != null) {
				object tmp = chunk.Attributes[Chunk.SUBSUPSCRIPT];
				if (tmp != null) {
					float f = (float)tmp;
					if (f > 0) {
						str.WriteByte(escape);
						str.Write(startSuper, 0, startSuper.Length);
					}
					else if (f < 0) {
						str.WriteByte(escape);
						str.Write(startSub, 0, startSub.Length);
					}
				}
			}

			str.WriteByte(delimiter);
		}


		internal void writeFinishingFontSignature(Stream str, Chunk chunk) {
			Font font = chunk.Font;

			if (font.isBold()) {
				str.WriteByte(escape);
				str.WriteByte(bold);
				writeInt(str, 0);
			}
			if (font.isItalic()) {
				str.WriteByte(escape);
				str.WriteByte(italic);
				writeInt(str, 0);
			}
			if (font.isUnderlined()) {
				str.WriteByte(escape);
				str.Write(underline, 0, underline.Length);
				writeInt(str, 0);
			}
			if (font.isStrikethru()) {
				str.WriteByte(escape);
				str.Write(strikethrough, 0, strikethrough.Length);
				writeInt(str, 0);
			}

			/*
			 * Superscript / Subscript added by Scott Dietrich (sdietrich@emlab.com)
			 */
			if (chunk.Attributes != null) {
				object tmp = chunk.Attributes[Chunk.SUBSUPSCRIPT];
				if (tmp != null) {
					float f = (float)tmp;
					if (f != 0) {
						str.WriteByte(escape);
						str.Write(endSuperSub, 0, endSuperSub.Length);
					}
				}
			}
		}

		/// <summary>
		/// Write a ListItem
		/// </summary>
		/// <param name="listItem">The ListItem to be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writeListElement(ListItem listItem, MemoryStream str) {
			foreach (Chunk ch in listItem.Chunks) {
				addElement(ch, str);
			}
			str.WriteByte(escape);
			str.Write(paragraph, 0, paragraph.Length);
		}
    
		/// <summary>
		/// Write a List
		/// </summary>
		/// <param name="list">The List to be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writeList(iTextSharp.text.List list, MemoryStream str) {
			int type = 0;
			int align = 0;
			int fontNr = addFont(new Font(Font.SYMBOL, 10, Font.NORMAL, new Color(0, 0, 0)));
			if(!list.isNumbered()) type = 23;
			if(listLevel == 0) {
				maxListLevel = 0;
				listtable.WriteByte(openGroup);
				listtable.WriteByte(escape);
				listtable.Write(listDefinition, 0, listDefinition.Length);
				int i = RandomInt;
				listtable.WriteByte(escape);
				listtable.Write(listTemplateID, 0, listTemplateID.Length);
				writeInt(listtable, i);
				listtable.WriteByte(escape);
				listtable.Write(hybridList, 0, hybridList.Length);
				listtable.WriteByte((byte) '\n');
			}
			if(listLevel >= maxListLevel) {
				maxListLevel++;
				listtable.WriteByte(openGroup);
				listtable.WriteByte(escape);
				listtable.Write(listLevelDefinition, 0, listLevelDefinition.Length);
				listtable.WriteByte(escape);
				listtable.Write(listLevelTypeOld, 0, listLevelTypeOld.Length);
				writeInt(listtable, type);
				listtable.WriteByte(escape);
				listtable.Write(listLevelTypeNew, 0, listLevelTypeNew.Length);
				writeInt(listtable, type);
				listtable.WriteByte(escape);
				listtable.Write(listLevelAlignOld, 0, listLevelAlignOld.Length);
				writeInt(listtable, align);
				listtable.WriteByte(escape);
				listtable.Write(listLevelAlignNew, 0, listLevelAlignNew.Length);
				writeInt(listtable, align);
				listtable.WriteByte(escape);
				listtable.Write(listLevelStartAt, 0, listLevelStartAt.Length);
				writeInt(listtable, 1);
				listtable.WriteByte(openGroup);
				listtable.WriteByte(escape);
				listtable.Write(listLevelTextDefinition, 0, listLevelTextDefinition.Length);
				listtable.WriteByte(escape);
				listtable.Write(listLevelTextLength, 0, listLevelTextLength.Length);
				if(list.isNumbered()) { writeInt(listtable, 2); } else { writeInt(listtable, 1); }
				listtable.WriteByte(escape);
				if(list.isNumbered()) { listtable.Write(listLevelTextStyleNumbers, 0, listLevelTextStyleNumbers.Length); } else { listtable.Write(listLevelTextStyleBullet, 0, listLevelTextStyleBullet.Length); }
				listtable.WriteByte(commaDelimiter);
				listtable.WriteByte(closeGroup);
				listtable.WriteByte(openGroup);
				listtable.WriteByte(escape);
				listtable.Write(listLevelNumbersDefinition, 0, listLevelNumbersDefinition.Length);
				if(list.isNumbered()) { listtable.WriteByte(delimiter); listtable.Write(listLevelNumbers, 0, listLevelNumbers.Length); writeInt(listtable, listLevel + 1); }
				listtable.WriteByte(commaDelimiter);
				listtable.WriteByte(closeGroup);
				if(!list.isNumbered()) { listtable.WriteByte(escape); listtable.WriteByte(fontNumber); writeInt(listtable, fontNr); }
				listtable.WriteByte(escape);
				listtable.Write(firstIndent, 0, firstIndent.Length);
				writeInt(listtable, (int) (list.IndentationLeft * twipsFactor * -1));
				listtable.WriteByte(escape);
				listtable.Write(listIndent, 0, listIndent.Length);
				writeInt(listtable, (int) ((list.IndentationLeft + list.SymbolIndent) * twipsFactor));
				listtable.WriteByte(escape);
				listtable.Write(tabStop, 0, tabStop.Length);
				writeInt(listtable, (int) (list.SymbolIndent * twipsFactor));
				listtable.WriteByte(closeGroup);
				listtable.WriteByte((byte) '\n');
			}
			// Actual List Begin in Content
			str.WriteByte(escape);
			str.Write(paragraphDefaults, 0, paragraphDefaults.Length);
			str.WriteByte(escape);
			str.Write(alignLeft, 0, alignLeft.Length);
			str.WriteByte(escape);
			str.Write(firstIndent, 0, firstIndent.Length);
			writeInt(str, (int) (list.IndentationLeft * twipsFactor * -1));
			str.WriteByte(escape);
			str.Write(listIndent, 0, listIndent.Length);
			writeInt(str, (int) ((list.IndentationLeft + list.SymbolIndent) * twipsFactor));
			str.WriteByte(escape);
			str.Write(fontSize, 0, fontSize.Length);
			writeInt(str, 20);
			str.WriteByte(escape);
			str.Write(listBegin, 0, listBegin.Length);
			writeInt(str, currentListID);
			if(listLevel > 0) {
				str.WriteByte(escape);
				str.Write(listCurrentLevel, 0, listCurrentLevel.Length);
				writeInt(str, listLevel);
			}
			str.WriteByte(openGroup);
			int count = 1;
			foreach (IElement ele in list.Items) {
				IElement listElem = ele;
				if(listElem.Type == Element.CHUNK) { listElem = new ListItem((Chunk) listElem); }
				if(listElem.Type == Element.LISTITEM) {
					str.WriteByte(openGroup);
					str.WriteByte(escape);
					str.Write(listTextOld, 0, listTextOld.Length);
					str.WriteByte(escape);
					str.Write(paragraphDefaults, 0, paragraphDefaults.Length);
					str.WriteByte(escape);
					str.WriteByte(fontNumber);
					if(list.isNumbered()) { writeInt(str, addFont(new Font(Font.TIMES_NEW_ROMAN, Font.NORMAL, 10, new Color(0, 0, 0)))); } else { writeInt(str, fontNr); }
					str.WriteByte(escape);
					str.Write(firstIndent, 0, firstIndent.Length);
					writeInt(str, (int) (list.IndentationLeft * twipsFactor * -1));
					str.WriteByte(escape);
					str.Write(listIndent, 0, listIndent.Length);
					writeInt(str, (int) ((list.IndentationLeft + list.SymbolIndent) * twipsFactor));
					str.WriteByte(delimiter);
					if(list.isNumbered()) { writeInt(str, count); str.WriteByte(ASCIIEncoding.ASCII.GetBytes(".")[0]); } else { str.WriteByte(escape); str.Write(listBulletOld, 0, listBulletOld.Length); }
					str.WriteByte(escape);
					str.Write(tab, 0, tab.Length);
					str.WriteByte(closeGroup);
					writeListElement((ListItem) listElem, str);
					count++;
				}
				else if(listElem.Type == Element.LIST) {
					listLevel++;
					writeList((iTextSharp.text.List) listElem, str);
					listLevel--;
					str.WriteByte(escape);
					str.Write(paragraphDefaults, 0, paragraphDefaults.Length);
					str.WriteByte(escape);
					str.Write(alignLeft, 0, alignLeft.Length);
					str.WriteByte(escape);
					str.Write(firstIndent, 0, firstIndent.Length);
					writeInt(str, (int) (list.IndentationLeft * twipsFactor * -1));
					str.WriteByte(escape);
					str.Write(listIndent, 0, listIndent.Length);
					writeInt(str, (int) ((list.IndentationLeft + list.SymbolIndent) * twipsFactor));
					str.WriteByte(escape);
					str.Write(fontSize, 0, fontSize.Length);
					writeInt(str, 20);
					str.WriteByte(escape);
					str.Write(listBegin, 0, listBegin.Length);
					writeInt(str, currentListID);
					if(listLevel > 0) {
						str.WriteByte(escape);
						str.Write(listCurrentLevel, 0, listCurrentLevel.Length);
						writeInt(str, listLevel);
					}
				}
				str.WriteByte((byte)'\n');
			}
			str.WriteByte(closeGroup);
			if(listLevel == 0) {
				int i = RandomInt;
				listtable.WriteByte(escape);
				listtable.Write(listID, 0, listID.Length);
				writeInt(listtable, i);
				listtable.WriteByte(closeGroup);
				listtable.WriteByte((byte) '\n');
				listoverride.WriteByte(openGroup);
				listoverride.WriteByte(escape);
				listoverride.Write(listOverride, 0, listOverride.Length);
				listoverride.WriteByte(escape);
				listoverride.Write(listID, 0, listID.Length);
				writeInt(listoverride, i);
				listoverride.WriteByte(escape);
				listoverride.Write(listOverrideCount, 0, listOverrideCount.Length);
				writeInt(listoverride, 0);
				listoverride.WriteByte(escape);
				listoverride.Write(listBegin, 0, listBegin.Length);
				writeInt(listoverride, currentListID);
				currentListID++;
				listoverride.WriteByte(closeGroup);
				listoverride.WriteByte((byte) '\n');
			}
		}
    
		/// <summary>
		/// Write a Table.
		/// </summary>
		/// <param name="table">The table to be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writeTable(Table table, MemoryStream str) {
			inTable = true;
			table.complete();
			RtfTable rtfTable = new RtfTable(this);
			rtfTable.importTable(table, pageWidth);
			rtfTable.writeTable(str);
			inTable = false;
		}
    
		/// <summary>
		/// Write an Image.
		/// </summary>
		/// <param name="image">The image to be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writeImage(Image image, MemoryStream str) {
			if(!image.isPng() && !image.isJpeg()) throw new DocumentException("Only PNG and JPEG images are supported by the RTF Writer");
			switch(image.Alignment) {
				case Element.ALIGN_LEFT      : str.WriteByte(escape); str.Write(alignLeft, 0, alignLeft.Length); break;
				case Element.ALIGN_RIGHT     : str.WriteByte(escape); str.Write(alignRight, 0, alignRight.Length); break;
				case Element.ALIGN_CENTER    : str.WriteByte(escape); str.Write(alignCenter, 0, alignCenter.Length); break;
				case Element.ALIGN_JUSTIFIED : str.WriteByte(escape); str.Write(alignJustify, 0, alignJustify.Length); break;
			}
			str.WriteByte(openGroup);
			str.Write(extendedEscape, 0, extendedEscape.Length);
			str.Write(pictureGroup, 0, pictureGroup.Length);
			str.WriteByte(openGroup);
			str.WriteByte(escape);
			str.Write(picture, 0, picture.Length);
			str.WriteByte(escape);
			if(image.isPng()) str.Write(picturePNG, 0, picturePNG.Length);
			if(image.isJpeg())  str.Write(pictureJPEG, 0, pictureJPEG.Length);
			str.WriteByte(escape);
			str.Write(pictureWidth, 0, pictureWidth.Length);
			writeInt(str, (int) (image.PlainWidth * twipsFactor));
			str.WriteByte(escape);
			str.Write(pictureHeight, 0, pictureHeight.Length);
			writeInt(str, (int) (image.PlainHeight * twipsFactor));


			// For some reason this messes up the intended image size. It makes it too big. Weird
			//
			//        str.WriteByte(escape);
			//        str.Write(pictureIntendedWidth);
			//        writeInt(str, (int) (image.PlainWidth * twipsFactor));
			//        str.WriteByte(escape);
			//        str.Write(pictureIntendedHeight);
			//        writeInt(str, (int) (image.PlainHeight * twipsFactor));


			if(image.Width > 0) {
				str.WriteByte(escape);
				str.Write(pictureScaleX, 0, pictureScaleX.Length);
				writeInt(str, (int) (100 / image.Width * image.PlainWidth));
			}
			if(image.Height > 0) {
				str.WriteByte(escape);
				str.Write(pictureScaleY, 0, pictureScaleY.Length);
				writeInt(str, (int) (100 / image.Height * image.PlainHeight));
			}
			str.WriteByte(delimiter);			
			Stream imgIn;
			if (image.RawData == null) {
				WebRequest w = WebRequest.Create(image.Url);
				imgIn = w.GetResponse().GetResponseStream();
			} else {
				imgIn = new MemoryStream(image.RawData);
			}
			int buffer = -1;
			int count = 0;
			str.WriteByte((byte) '\n');
			while((buffer = imgIn.ReadByte()) != -1) {
				String helperStr = System.Convert.ToString(buffer, 16);
				if (helperStr.Length < 2) helperStr = "0" + helperStr;
				str.Write(ASCIIEncoding.ASCII.GetBytes(helperStr), 0,
					ASCIIEncoding.ASCII.GetBytes(helperStr).Length);
				count++;
				if(count == 64) { str.WriteByte((byte) '\n'); count = 0; }
			}
			str.WriteByte(closeGroup);
			str.WriteByte(closeGroup);
			str.WriteByte((byte) '\n');
		}
    
		/// <summary>
		/// Write an Annotation
		/// </summary>
		/// <param name="annotationElement">The Annotation to be written</param>
		/// <param name="str">The MemoryStream to write to</param>
		private void writeAnnotation(Annotation annotationElement, MemoryStream str) {
			int id = RandomInt;
			str.WriteByte(openGroup);
			str.Write(extendedEscape, 0, extendedEscape.Length);
			str.Write(annotationID, 0, annotationID.Length);
			str.WriteByte(delimiter);
			writeInt(str, id);
			str.WriteByte(closeGroup);
			str.WriteByte(openGroup);
			str.Write(extendedEscape, 0, extendedEscape.Length);
			str.Write(annotationAuthor, 0, annotationAuthor.Length);
			str.WriteByte(delimiter);
			str.Write(ASCIIEncoding.ASCII.GetBytes(annotationElement.Title), 0,
				ASCIIEncoding.ASCII.GetBytes(annotationElement.Title).Length);
			str.WriteByte(closeGroup);
			str.WriteByte(openGroup);
			str.Write(extendedEscape, 0, extendedEscape.Length);
			str.Write(annotation, 0, annotation.Length);
			str.WriteByte(escape);
			str.Write(paragraphDefaults, 0, paragraphDefaults.Length);
			str.WriteByte(delimiter);
			str.Write(ASCIIEncoding.ASCII.GetBytes(annotationElement.Content), 0,
				ASCIIEncoding.ASCII.GetBytes(annotationElement.Content).Length);
			str.WriteByte(closeGroup);
		}

		/// <summary>
		/// Add a Meta element. It is written to the Inforamtion Group
		/// and merged with the main MemoryStream when the
		/// Document is closed.
		/// </summary>
		/// <remarks>
		/// Currently only the Meta Elements Author, Subject, Keywords, Title, Producer and CreationDate are supported.
		/// </remarks>
		/// <param name="metaName">The type of Meta element to be added</param>
		/// <param name="meta">The Meta element to be added</param>
		private void writeMeta(byte[] metaName, Meta meta) {
			info.WriteByte(openGroup);
			try {
				info.WriteByte(escape);
				info.Write(metaName, 0, metaName.Length);
				info.WriteByte(delimiter);
				if(meta.Type == Element.CREATIONDATE) { 
					writeFormatedDateTime(meta.Content); 
				} else {
					info.Write(ASCIIEncoding.ASCII.GetBytes(meta.Content), 0,
						ASCIIEncoding.ASCII.GetBytes(meta.Content).Length); }
			}
			finally {
				info.WriteByte(closeGroup);
			}
		}
    
		/// <summary>
		/// Writes a date. The date is formated <strong>Year, Month, Day, Hour, Minute, Second</strong>
		/// </summary>
		/// <param name="date">The date to be written</param>
		private void writeFormatedDateTime(String date) {
			DateTime dt = DateTime.Parse(date);

			info.WriteByte(escape);
			info.Write(year, 0, year.Length);
			writeInt(info, dt.Year);
			info.WriteByte(escape);
			info.Write(month, 0, month.Length);
			writeInt(info, dt.Month);
			info.WriteByte(escape);
			info.Write(day, 0, day.Length);
			writeInt(info, dt.Day);
			info.WriteByte(escape);
			info.Write(hour, 0, hour.Length);
			writeInt(info, dt.Hour);
			info.WriteByte(escape);
			info.Write(minute, 0, minute.Length);
			writeInt(info, dt.Minute);
			info.WriteByte(escape);
			info.Write(second, 0, second.Length);
			writeInt(info, dt.Second);
		}
    
		/// <summary>
		/// Add a new Font to the list of fonts. If the Font
		/// already exists in the list of fonts, then it is not added again.
		/// </summary>
		/// <param name="newFont">The Font to be added</param>
		/// <returns>The index of the Font in the font list</returns>
		protected int addFont(Font newFont) {
			int fn = -1;

			for(int i = 0; i < fontList.Count; i++) {
				if(newFont.Familyname.Equals(((Font)fontList[i]).Familyname)) { fn = i; }
			}
			if(fn == -1) {
				fontList.Add(newFont);
				return fontList.Count - 1;
			}
			return fn;
		}
    
		/// <summary>
		/// Add a new Color to the list of colours. If the Color
		/// already exists in the list of colours, then it is not added again.
		/// </summary>
		/// <param name="newColor">The Color to be added</param>
		/// <returns>The index of the color in the colour list</returns>
		internal int addColor(Color newColor) {
			int cn = 0;
			if(newColor == null) { return cn; }
			cn = colorList.IndexOf(newColor);
			if(cn == -1) {
				colorList.Add(newColor);
				return colorList.Count - 1;
			}
			return cn;
		}
    
		/// <summary>
		/// Merge all the different Vectors and MemoryStreams
		/// to the MemoryStream
		/// </summary>
		/// <returns>true if all information was sucessfully written to the MemoryStream</returns>
		private bool writeDocument() {
			try {
				writeDocumentIntro();
				os.WriteByte((byte)'\n');
				writeFontList();
				os.WriteByte((byte)'\n');
				writeColorList();
				os.WriteByte((byte)'\n');
				writeList();
				os.WriteByte((byte)'\n');
				writeInfoGroup();
				os.WriteByte((byte)'\n');
				writeDocumentFormat();
				os.WriteByte((byte)'\n');
				MemoryStream hf = new MemoryStream();
				writeSectionDefaults(hf);
				hf.WriteTo(os);
				content.WriteTo(os);
				os.WriteByte(closeGroup);
				return true;
			}
			catch(IOException e) {
				Console.Error.WriteLine(e.Message);
				return false;
			}
        
		}
    
		/// <summary>
		/// Write the Rich Text file settings
		/// </summary>
		private void writeDocumentIntro() {
			os.WriteByte(openGroup);
			os.WriteByte(escape);
			os.Write(docBegin, 0, docBegin.Length);
			os.WriteByte(escape);
			os.Write(ansi, 0, ansi.Length);
			os.WriteByte(escape);
			os.Write(ansiCodepage, 0, ansiCodepage.Length);
			writeInt(os, 1252);
			os.WriteByte(escape);
			os.Write(defaultFont, 0, defaultFont.Length);
			writeInt(os, 0);
		}
    
		/// <summary>
		/// Write the font list to the MemoryStream
		/// </summary>
		private void writeFontList() {
			Font fnt;

			os.WriteByte(openGroup);
			os.WriteByte(escape);
			os.Write(fontTable, 0, fontTable.Length);
			for(int i = 0; i < fontList.Count; i++) {
				fnt = (Font) fontList[i];
				os.WriteByte(openGroup);
				os.WriteByte(escape);
				os.WriteByte(fontNumber);
				writeInt(os, i);
				os.WriteByte(escape);
				switch(Font.getFamilyIndex(fnt.Familyname)) {
					case Font.COURIER:
						os.Write(fontModern, 0, fontModern.Length);
						os.WriteByte(escape);
						os.Write(fontCharset, 0, fontCharset.Length);
						writeInt(os, 0);
						os.WriteByte(delimiter);
						os.Write(fontCourier, 0, fontCourier.Length);
						break;
					case Font.HELVETICA:
						os.Write(fontSwiss, 0, fontSwiss.Length);
						os.WriteByte(escape);
						os.Write(fontCharset, 0, fontCharset.Length);
						writeInt(os, 0);
						os.WriteByte(delimiter);
						os.Write(fontArial, 0, fontArial.Length);
						break;
					case Font.SYMBOL:
						os.Write(fontRoman, 0, fontRoman.Length);
						os.WriteByte(escape);
						os.Write(fontCharset, 0, fontCharset.Length);
						writeInt(os, 2);
						os.WriteByte(delimiter);
						os.Write(fontSymbol, 0, fontSymbol.Length);
						break;
					case Font.TIMES_NEW_ROMAN:
						os.Write(fontRoman, 0, fontRoman.Length);
						os.WriteByte(escape);
						os.Write(fontCharset, 0, fontCharset.Length);
						writeInt(os, 0);
						os.WriteByte(delimiter);
						os.Write(fontTimesNewRoman, 0, fontTimesNewRoman.Length);
						break;
					case Font.ZAPFDINGBATS:
						os.Write(fontTech, 0, fontTech.Length);
						os.WriteByte(escape);
						os.Write(fontCharset, 0, fontCharset.Length);
						writeInt(os, 0);
						os.WriteByte(delimiter);
						os.Write(fontWindings, 0, fontWindings.Length);
						break;
					default: os.Write(ASCIIEncoding.ASCII.GetBytes(fnt.Familyname), 0,
								 ASCIIEncoding.ASCII.GetBytes(fnt.Familyname).Length);
						os.WriteByte(escape);
						os.Write(fontCharset, 0, fontCharset.Length);
						writeInt(os, 0);
						os.WriteByte(delimiter);
						os.Write(ASCIIEncoding.ASCII.GetBytes(fnt.Familyname), 0, 
							ASCIIEncoding.ASCII.GetBytes(fnt.Familyname).Length);
						break;
				}
				os.WriteByte(commaDelimiter);
				os.WriteByte(closeGroup);
			}
			os.WriteByte(closeGroup);
		}

		/// <summary>
		/// Write the colour list to the MemoryStream
		/// </summary>
		private void writeColorList() {
			Color color = null;
        
			os.WriteByte(openGroup);
			os.WriteByte(escape);
			os.Write(colorTable, 0, colorTable.Length);
			for(int i = 0; i < colorList.Count; i++) {
				color = (Color) colorList[i];
				os.WriteByte(escape);
				os.Write(colorRed, 0, colorRed.Length);
				writeInt(os, color.R);
				os.WriteByte(escape);
				os.Write(colorGreen, 0, colorGreen.Length);
				writeInt(os, color.G);
				os.WriteByte(escape);
				os.Write(colorBlue, 0, colorBlue.Length);
				writeInt(os, color.B);
				os.WriteByte(commaDelimiter);
			}
			os.WriteByte(closeGroup);
		}
    
		/// <summary>
		/// Write the Information Group to the MemoryStream
		/// </summary>
		private void writeInfoGroup() {
			os.WriteByte(openGroup);
			os.WriteByte(escape);
			os.Write(infoBegin, 0, infoBegin.Length);
			info.WriteTo(os);
			os.WriteByte(closeGroup);
		}
    
		/// <summary>
		/// Write the listtable and listoverridetable to the MemoryStream
		/// </summary>
		private void writeList() {
			listtable.WriteByte(closeGroup);
			listoverride.WriteByte(closeGroup);
			listtable.WriteTo(os);
			os.WriteByte((byte)'\n');
			listoverride.WriteTo(os);
		}

		/// <summary>
		/// Write an integer
		/// </summary>
		/// <param name="str">The Stream to which the int value is to be written</param>
		/// <param name="i">The int value to be written</param>
		public static void writeInt(Stream str, int i) {
			str.Write(ASCIIEncoding.ASCII.GetBytes(i.ToString()), 0, 
				ASCIIEncoding.ASCII.GetBytes(i.ToString()).Length);
		}
    
		/// <summary>
		/// Get a random integer.
		/// </summary>
		/// <value>a <b>unique</b> random integer to be used with listids.</value>
		private int RandomInt {
			get {
				bool ok = false;
				int newInt = -1;
				int oldInt = -1;
				while(!ok) {
					newInt = (int) (r.NextDouble() * int.MaxValue);
					ok = true;
					for(int i = 0; i < listIds.Count; i++) {
						oldInt = (int) listIds[i];
						if(oldInt == newInt) { ok = true; }
					}
				}
				listIds.Add(newInt);
				return newInt;
			}
		}

		/// <summary>
		/// Write the current header and footer to a MemoryStream
		/// </summary>
		/// <param name="os">The MemoryStream to which the header and footer will be written.</param>
		public void writeHeadersFooters(MemoryStream os) {
			if (this.footer is RtfHeaderFooters) {
				RtfHeaderFooters rtfHf = (RtfHeaderFooters)this.footer;
				HeaderFooter hf = rtfHf.get(RtfHeaderFooters.ALL_PAGES);
				if (hf != null) {
					writeHeaderFooter(hf, footerBegin, os);
				}
				hf = rtfHf.get(RtfHeaderFooters.LEFT_PAGES);
				if (hf != null) {
					writeHeaderFooter(hf, footerlBegin, os);
				}
				hf = rtfHf.get(RtfHeaderFooters.RIGHT_PAGES);
				if (hf != null) {
					writeHeaderFooter(hf, footerrBegin, os);
				}
				hf = rtfHf.get(RtfHeaderFooters.FIRST_PAGE);
				if (hf != null) {
					writeHeaderFooter(hf, footerfBegin, os);
				}
			} else {
				writeHeaderFooter(this.footer, footerBegin, os);
			}
			if (this.header is RtfHeaderFooters) {
				RtfHeaderFooters rtfHf = (RtfHeaderFooters)this.header;
				HeaderFooter hf = rtfHf.get(RtfHeaderFooters.ALL_PAGES);
				if (hf != null) {
					writeHeaderFooter(hf, headerBegin, os);
				}
				hf = rtfHf.get(RtfHeaderFooters.LEFT_PAGES);
				if (hf != null) {
					writeHeaderFooter(hf, headerlBegin, os);
				}
				hf = rtfHf.get(RtfHeaderFooters.RIGHT_PAGES);
				if (hf != null) {
					writeHeaderFooter(hf, headerrBegin, os);
				}
				hf = rtfHf.get(RtfHeaderFooters.FIRST_PAGE);
				if (hf != null) {
					writeHeaderFooter(hf, headerfBegin, os);
				}
			} else {
				writeHeaderFooter(this.header, headerBegin, os);
			}
		}

		/// <summary>
		/// Write a HeaderFooter to a MemoryStream
		/// </summary>
		/// <param name="headerFooter">The HeaderFooter object to be written.</param>
		/// <param name="hfType">The type of header or footer to be added.</param>
		/// <param name="target">The MemoryStream to which the HeaderFooter will be written.</param>
		private void writeHeaderFooter(HeaderFooter headerFooter, byte[] hfType, MemoryStream target) {
			inHeaderFooter = true;
			try {
				target.WriteByte(openGroup);
				target.WriteByte(escape);
				target.Write(hfType, 0, hfType.Length);
				target.WriteByte(delimiter);
				if(headerFooter != null) {
					if (headerFooter is RtfHeaderFooter
						&& ((RtfHeaderFooter)headerFooter).Content != null) {
						this.addElement(((RtfHeaderFooter)headerFooter).Content, target);
					} else {
						if(headerFooter.Before != null) { this.addElement(headerFooter.Before, target); }
						if(headerFooter.isNumbered()) { this.addElement(new RtfPageNumber("", headerFooter.Before.Font), target); }
						if(headerFooter.After != null) { this.addElement(headerFooter.After, target); }
						//                    Element[] headerElements = new Element[3];
						/*                    java.util.List chunks = headerFooter.paragraph().getChunks();
										int headerCount = chunks.Size;
					//                    if(headerCount >= 1) { headerElements[0] = (Element) headerFooter.paragraph().getChunks().get(0); }
					//                    if(headerCount >= 2) { headerElements[1] = (Element) headerFooter.paragraph().getChunks().get(1); }
					//                    if(headerCount >= 3) { headerElements[2] = (Element) headerFooter.paragraph().getChunks().get(2); }
										if(headerCount >= 1) {
											Add((Element)chunks.get(0));
										}
										if (headerCount >= 2) {
											Element chunk = (Element)chunks.get(1);
											if (chunk.Type == Element.CHUNK) {
												try {
													Integer.parseInt(((Chunk)chunk).Content);
													content.WriteByte(openGroup);
													content.WriteByte(escape);
													content.write(field);
													content.WriteByte(openGroup);
													content.write(extendedEscape);
													content.write(fieldContent);
													content.WriteByte(openGroup);
													content.WriteByte(delimiter);
													content.write(fieldPage);
													content.WriteByte(delimiter);
													content.WriteByte(closeGroup);
													content.WriteByte(closeGroup);
													content.WriteByte(openGroup);
													content.WriteByte(escape);
													content.write(fieldDisplay);
													content.WriteByte(openGroup);
													content.WriteByte(closeGroup);
													content.WriteByte(closeGroup);
													content.WriteByte(closeGroup);
												}
												catch(NumberFormatException nfe)
												{
													Add(chunk);
												}
											} else {
												Add(chunk);
											}
					//                        if(headerElements[1].Type == Element.PHRASE) { writePhrase((Phrase) headerElements[1], content); }
										}
										if(headerCount >= 3) {
											Add((Element)chunks.get(2));
					//                        if(headerElements[2].Type == Element.CHUNK) { writeChunk((Chunk) headerElements[2], content); }
					//                        if(headerElements[2].Type == Element.PHRASE) { writePhrase((Phrase) headerElements[2], content); }
										}*/
					}
				}
				target.WriteByte(closeGroup);
			}
			catch(DocumentException e) {
				throw new IOException("DocumentException - "+e.Message);
			}
			inHeaderFooter = false;
		}

		/// <summary>
		/// Write the Document's Paper and Margin Size
		/// to the MemoryStream
		/// </summary>
		private void writeDocumentFormat() {
			os.WriteByte(escape);
			os.Write(rtfPaperWidth, 0, rtfPaperWidth.Length);
			writeInt(os, pageWidth);
			os.WriteByte(escape);
			os.Write(rtfPaperHeight, 0, rtfPaperHeight.Length);
			writeInt(os, pageHeight);
			os.WriteByte(escape);
			os.Write(rtfMarginLeft, 0, rtfMarginLeft.Length);
			writeInt(os, marginLeft);
			os.WriteByte(escape);
			os.Write(rtfMarginRight, 0, rtfMarginRight.Length);
			writeInt(os, marginRight);
			os.WriteByte(escape);
			os.Write(rtfMarginTop, 0, rtfMarginTop.Length);
			writeInt(os, marginTop);
			os.WriteByte(escape);
			os.Write(rtfMarginBottom, 0, rtfMarginBottom.Length);
			writeInt(os, marginBottom);
			//        os.WriteByte(closeGroup);
		}
    
		/// <summary>
		/// Initialise all helper classes.
		/// Clears alls lists, creates new MemoryStream's
		/// </summary>
		private void initDefaults() {
			fontList.Clear();
			colorList.Clear();
			info = new MemoryStream();
			content = new MemoryStream();
			listtable = new MemoryStream();
			listoverride = new MemoryStream();
			document.addProducer();
			document.addCreationDate();
			addFont(new Font(Font.TIMES_NEW_ROMAN, 10, Font.NORMAL));
			addColor(new Color(0, 0, 0));
			addColor(new Color(255, 255, 255));
			listIds = new ArrayList();
			try {
				listtable.WriteByte(openGroup);
				listtable.Write(extendedEscape, 0, extendedEscape.Length);
				listtable.Write(listtableGroup, 0, listtableGroup.Length);
				listtable.WriteByte((byte) '\n');
				listoverride.WriteByte(openGroup);
				listoverride.Write(extendedEscape, 0, extendedEscape.Length);
				listoverride.Write(listoverridetableGroup, 0, listoverridetableGroup.Length);
				listoverride.WriteByte((byte) '\n');
			}
			catch(IOException e) {
				Console.Error.WriteLine("InitDefaultsError" + e);
			}
		}

		/// <summary>
		/// Writes the default values for the current Section
		/// </summary>
		/// <param name="str">The MemoryStream to be written to</param>
		private void writeSectionDefaults(MemoryStream str) {
			if(header is RtfHeaderFooters || footer  is RtfHeaderFooters) {
				str.WriteByte(escape);
				str.Write(facingPages, 0, facingPages.Length);
			}
			if(hasTitlePage) {
				str.WriteByte(escape);
				str.Write(titlePage, 0, titlePage.Length);
			}
			writeHeadersFooters(str);
			if(landscape) {
				str.WriteByte(escape);
				str.Write(landscapeTag1, 0, landscapeTag1.Length);
				str.WriteByte(escape);
				str.Write(landscapeTag2, 0, landscapeTag2.Length);
			}
		}

		/// <summary>
		/// This method tries to fit the Rectangle pageSize to one of the predefined PageSize rectangles.
		/// If a match is found the pageWidth and pageHeight will be set according to values determined from files
		/// generated by MS Word2000 and OpenOffice 641. If no match is found the method will try to match the rotated
		/// Rectangle by calling itself with the parameter rotate set to true.
		/// </summary>
		/// <param name="pageSize"></param>
		/// <param name="rotate"></param>
		/// <returns></returns>
		private bool parseFormat(Rectangle pageSize, bool rotate) {
			if(rotate) { pageSize = pageSize.rotate(); }
			if(rectEquals(pageSize, PageSize.A3)) {
				pageWidth = 16837;
				pageHeight = 23811;
				landscape = rotate;
				return true;
			}
			if(rectEquals(pageSize, PageSize.A4)) {
				pageWidth = 11907;
				pageHeight = 16840;
				landscape = rotate;
				return true;
			}
			if(rectEquals(pageSize, PageSize.A5)) {
				pageWidth = 8391;
				pageHeight = 11907;
				landscape = rotate;
				return true;
			}
			if(rectEquals(pageSize, PageSize.A6)) {
				pageWidth = 5959;
				pageHeight = 8420;
				landscape = rotate;
				return true;
			}
			if(rectEquals(pageSize, PageSize.B4)) {
				pageWidth = 14570;
				pageHeight = 20636;
				landscape = rotate;
				return true;
			}
			if(rectEquals(pageSize, PageSize.B5)) {
				pageWidth = 10319;
				pageHeight = 14572;
				landscape = rotate;
				return true;
			}
			if(rectEquals(pageSize, PageSize.HALFLETTER)) {
				pageWidth = 7927;
				pageHeight = 12247;
				landscape = rotate;
				return true;
			}
			if(rectEquals(pageSize, PageSize.LETTER)) {
				pageWidth = 12242;
				pageHeight = 15842;
				landscape = rotate;
				return true;
			}
			if(rectEquals(pageSize, PageSize.LEGAL)) {
				pageWidth = 12252;
				pageHeight = 20163;
				landscape = rotate;
				return true;
			}
			if(!rotate && parseFormat(pageSize, true)) {
				int x = pageWidth;
				pageWidth = pageHeight;
				pageHeight = x;
				return true;
			}
			return false;
		}

		/// <summary>
		/// This method compares to Rectangles. They are considered equal if width and height are the same
		/// </summary>
		/// <param name="rect1"></param>
		/// <param name="rect2"></param>
		/// <returns></returns>
		private bool rectEquals(Rectangle rect1, Rectangle rect2) {
			return (rect1.Width == rect2.Width) && (rect1.Height == rect2.Height);
		}

		/// <summary>
		/// Returns whether we are currently writing a header or footer
		/// </summary>
		/// <returns>the value of HeaderFoote</returns>
		public bool writingHeaderFooter() {
			return inHeaderFooter;
		}

		/// <summary>
		/// Replaces special characters with their unicode values
		/// </summary>
		/// <param name="str">The original String</param>
		/// <returns>The converted String</returns>
		public static String filterSpecialChar(String str) {
			int length = str.Length;
			int z = (int)'z';
			StringBuilder ret = new StringBuilder(length);
			for(int i = 0; i < length; i++) {
				char ch = str[i];

				if(ch == '\\') {
					ret.Append("\\\\");
				}
				else if(ch == '\n') {
					ret.Append("\\par ");
				}
				else if(((int)ch) > z) {
					ret.Append("\\u").Append((long)ch).Append('G');
				}
				else {
					ret.Append(ch);
				}
			}
			return ret.ToString();
		}
	}
}