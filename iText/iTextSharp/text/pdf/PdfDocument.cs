using System;
using System.IO;
using System.Drawing;
using System.Collections;

using System.util;

using iTextSharp.text;

/*
 * $Name:  $
 * $Id: PdfDocument.cs,v 1.4 2003/08/22 16:18:12 geraldhenson Exp $
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

namespace iTextSharp.text.pdf {

	/**
	 * <CODE>PdfDocument</CODE> is the class that is used by <CODE>PdfWriter</CODE>
	 * to translate a <CODE>Document</CODE> into a PDF with different pages.
	 * <P>
	 * A <CODE>PdfDocument</CODE> always listens to a <CODE>Document</CODE>
	 * and adds the Pdf representation of every <CODE>Element</CODE> that is
	 * added to the <CODE>Document</CODE>.
	 *
	 * @see		iTextSharp.text.Document
	 * @see		iTextSharp.text.DocListener
	 * @see		PdfWriter
	 */

	public class PdfDocument : Document, IDocListener {
    
		/**
		 * <CODE>PdfInfo</CODE> is the PDF InfoDictionary.
		 * <P>
		 * A document's trailer may contain a reference to an Info dictionary that provides information
		 * about the document. This optional dictionary may contain one or more keys, whose values
		 * should be strings.<BR>
		 * This object is described in the 'Portable Document Format Reference Manual version 1.3'
		 * section 6.10 (page 120-121)
		 */
    
		public class PdfInfo : PdfDictionary {
        
			// constructors
        
			/**
			 * Construct a <CODE>PdfInfo</CODE>-object.
			 */
        
			internal PdfInfo() : base() {
				addProducer();
				addCreationDate();
			}
        
			/**
			 * Constructs a <CODE>PdfInfo</CODE>-object.
			 *
			 * @param		author		name of the author of the document
			 * @param		title		title of the document
			 * @param		subject		subject of the document
			 */
        
			internal PdfInfo(string author, string title, string subject) : this() {
				addTitle(title);
				addSubject(subject);
				addAuthor(author);
			}
        
			/**
			 * Adds the title of the document.
			 *
			 * @param	title		the title of the document
			 */
        
			internal void addTitle(string title) {
				put(PdfName.TITLE, new PdfString(title, PdfObject.TEXT_UNICODE));
			}
        
			/**
			 * Adds the subject to the document.
			 *
			 * @param	subject		the subject of the document
			 */
        
			internal void addSubject(string subject) {
				put(PdfName.SUBJECT, new PdfString(subject, PdfObject.TEXT_UNICODE));
			}
        
			/**
			 * Adds some keywords to the document.
			 *
			 * @param	keywords		the keywords of the document
			 */
        
			internal void addKeywords(string keywords) {
				put(PdfName.KEYWORDS, new PdfString(keywords, PdfObject.TEXT_UNICODE));
			}
        
			/**
			 * Adds the name of the author to the document.
			 *
			 * @param	author		the name of the author
			 */
        
			internal void addAuthor(string author) {
				put(PdfName.AUTHOR, new PdfString(author, PdfObject.TEXT_UNICODE));
			}
        
			/**
			 * Adds the name of the creator to the document.
			 *
			 * @param	creator		the name of the creator
			 */
        
			internal void addCreator(string creator) {
				put(PdfName.CREATOR, new PdfString(creator, PdfObject.TEXT_UNICODE));
			}
        
			/**
			 * Adds the name of the producer to the document.
			 */
        
			internal void addProducer() {
				// This line may only be changed by Bruno Lowagie or Paulo Soares
				put(PdfName.PRODUCER, new PdfString(Document.Version));
				// Do not edit the line above!
			}
        
			/**
			 * Adds the date of creation to the document.
			 */
        
			internal void addCreationDate() {
				put(PdfName.CREATIONDATE, new PdfDate());
			}
		}
    
		/**
		 * <CODE>PdfCatalog</CODE> is the PDF Catalog-object.
		 * <P>
		 * The Catalog is a dictionary that is the root node of the document. It contains a reference
		 * to the tree of pages contained in the document, a reference to the tree of objects representing
		 * the document's outline, a reference to the document's article threads, and the list of named
		 * destinations. In addition, the Catalog indicates whether the document's outline or thumbnail
		 * page images should be displayed automatically when the document is viewed and wether some location
		 * other than the first page should be shown when the document is opened.<BR>
		 * In this class however, only the reference to the tree of pages is implemented.<BR>
		 * This object is described in the 'Portable Document Format Reference Manual version 1.3'
		 * section 6.2 (page 67-71)
		 */
    
		internal class PdfCatalog : PdfDictionary {
			PdfDocument parent;
        
			// constructors
        
			/**
			 * Constructs a <CODE>PdfCatalog</CODE>.
			 *
			 * @param		pages		an indirect reference to the root of the document's Pages tree.
			 */
        
			internal PdfCatalog(PdfDocument parent, PdfIndirectReference pages) : base(CATALOG) {
				this.parent = parent;
				put(PdfName.PAGES, pages);
			}
        
			/**
			 * Constructs a <CODE>PdfCatalog</CODE>.
			 *
			 * @param		pages		an indirect reference to the root of the document's Pages tree.
			 * @param		outlines	an indirect reference to the outline tree.
			 */
        
			internal PdfCatalog(PdfDocument parent, PdfIndirectReference pages, PdfIndirectReference outlines) : base(CATALOG) {
				this.parent = parent;
				put(PdfName.PAGES, pages);
				put(PdfName.PAGEMODE, PdfName.USEOUTLINES);
				put(PdfName.OUTLINES, outlines);
			}
        
			/**
			 * Adds the names of the named destinations to the catalog.
			 * @param localDestinations the local destinations
			 */
			internal void addNames(SortedMap localDestinations, ArrayList documentJavaScript, PdfWriter writer) {
				if (localDestinations.Count == 0 && documentJavaScript.Count == 0)
					return;
				try {
					PdfDictionary names = new PdfDictionary();
					if (localDestinations.Count > 0) {
						PdfArray ar = new PdfArray();
						foreach(string name in localDestinations.Keys) {
							Object[] obj = (Object[])localDestinations[name];
							PdfIndirectReference piref = (PdfIndirectReference)obj[1];
							ar.Add(new PdfString(name));
							ar.Add(piref);
						}
						PdfDictionary dests = new PdfDictionary();
						dests.put(PdfName.NAMES, ar);
						names.put(PdfName.DESTS, writer.addToBody(dests).IndirectReference);
					}
					if (documentJavaScript.Count > 0) {
						string[] s = new string[documentJavaScript.Count];
						for (int k = 0; k < s.Length; ++k)
							s[k] = int.Parse(k.ToString(), System.Globalization.NumberStyles.HexNumber).ToString();
						Array.Sort(s, new StringCompare());
						PdfArray ar = new PdfArray();
						for (int k = 0; k < s.Length; ++k) {
							ar.Add(new PdfString(s[k]));
							ar.Add((PdfIndirectReference)documentJavaScript[k]);
						}
						PdfDictionary js = new PdfDictionary();
						js.put(PdfName.NAMES, ar);
						names.put(PdfName.JAVASCRIPT, writer.addToBody(js).IndirectReference);
					}
					put(PdfName.NAMES, writer.addToBody(names).IndirectReference);
				}
				catch (IOException e) {
					throw e;
				}
			}
        
			/** Sets the viewer preferences as the sum of several constants.
			 * @param preferences the viewer preferences
			 * @see PdfWriter#setViewerPreferences
			 */
        
			internal int ViewerPreferences {
				set {
					if ((value & PdfWriter.PageLayoutSinglePage) != 0)
						put(PdfName.PAGELAYOUT, PdfName.SINGLEPAGE);
					else if ((value & PdfWriter.PageLayoutOneColumn) != 0)
						put(PdfName.PAGELAYOUT, PdfName.ONECOLUMN);
					else if ((value & PdfWriter.PageLayoutTwoColumnLeft) != 0)
						put(PdfName.PAGELAYOUT, PdfName.TWOCOLUMNLEFT);
					else if ((value & PdfWriter.PageLayoutTwoColumnRight) != 0)
						put(PdfName.PAGELAYOUT, PdfName.TWOCOLUMNRIGHT);
					if ((value & PdfWriter.PageModeUseNone) != 0)
						put(PdfName.PAGEMODE, PdfName.USENONE);
					else if ((value & PdfWriter.PageModeUseOutlines) != 0)
						put(PdfName.PAGEMODE, PdfName.USEOUTLINES);
					else if ((value & PdfWriter.PageModeUseThumbs) != 0)
						put(PdfName.PAGEMODE, PdfName.USETHUMBS);
					else if ((value & PdfWriter.PageModeFullScreen) != 0)
						put(PdfName.PAGEMODE, PdfName.FULLSCREEN);
					if ((value & PdfWriter.ViewerPreferencesMask) == 0)
						return;
					PdfDictionary vp = new PdfDictionary();
					if ((value & PdfWriter.HideToolbar) != 0)
						vp.put(PdfName.HIDETOOLBAR, PdfBoolean.PDFTRUE);
					if ((value & PdfWriter.HideMenubar) != 0)
						vp.put(PdfName.HIDEMENUBAR, PdfBoolean.PDFTRUE);
					if ((value & PdfWriter.HideWindowUI) != 0)
						vp.put(PdfName.HIDEWINDOWUI, PdfBoolean.PDFTRUE);
					if ((value & PdfWriter.FitWindow) != 0)
						vp.put(PdfName.FITWINDOW, PdfBoolean.PDFTRUE);
					if ((value & PdfWriter.CenterWindow) != 0)
						vp.put(PdfName.CENTERWINDOW, PdfBoolean.PDFTRUE);
					if ((value & PdfWriter.NonFullScreenPageModeUseNone) != 0)
						vp.put(PdfName.NONFULLSCREENPAGEMODE, PdfName.USENONE);
					else if ((value & PdfWriter.NonFullScreenPageModeUseOutlines) != 0)
						vp.put(PdfName.NONFULLSCREENPAGEMODE, PdfName.USEOUTLINES);
					else if ((value & PdfWriter.NonFullScreenPageModeUseThumbs) != 0)
						vp.put(PdfName.NONFULLSCREENPAGEMODE, PdfName.USETHUMBS);
					if ((value & PdfWriter.DirectionL2R) != 0)
						vp.put(PdfName.DIRECTION, PdfName.L2R);
					else if ((value & PdfWriter.DirectionR2L) != 0)
						vp.put(PdfName.DIRECTION, PdfName.R2L);
					put(PdfName.VIEWERPREFERENCES, vp);
				}
			}
        
			internal PdfAction OpenAction {
				set {
					put(PdfName.OPENACTION, value);
				}
			}
        
        
			/** Sets the document level additional actions.
			 * @param actions   dictionary of actions
			 */
			internal PdfDictionary AdditionalActions {
				set {
					try {
						put(PdfName.AA, parent.writer.addToBody(value).IndirectReference);
					} catch (Exception e) {
						throw e;
					}
				}
			}
        
        
			internal PdfPageLabels PageLabels {
				set {
					put(PdfName.PAGELABELS, value.Dictionary);
				}
			}
        
			internal PdfObject AcroForm {
				set {
					put(PdfName.ACROFORM, value);
				}
			}
		}
    
		// membervariables
    
		/** The characters to be applied the hanging ponctuation. */
		static string hangingPunctuation = ".,;:'";
    
		/** The <CODE>PdfWriter</CODE>. */
		internal PdfWriter writer;
    
		/** some meta information about the Document. */
		private PdfInfo info = new PdfInfo();
    
		/** Signals that OnOpenDocument should be called. */
		private bool firstPageEvent = true;
    
		/** Signals that onParagraph is valid. */
		private bool isParagraph = true;
    
		// Horizontal line
    
		/** The line that is currently being written. */
		private PdfLine line = null;
    
		/** This represents the current indentation of the PDF Elements on the left side. */
		private float indentLeft = 0;
    
		/** This represents the current indentation of the PDF Elements on the right side. */
		private float indentRight = 0;
    
		/** This represents the current indentation of the PDF Elements on the left side. */
		private float listIndentLeft = 0;
    
		/** This represents the current alignment of the PDF Elements. */
		private int alignment = Element.ALIGN_LEFT;
    
		// Vertical lines
    
		/** This is the PdfContentByte object, containing the text. */
		private PdfContentByte text;
    
		/** This is the PdfContentByte object, containing the borders and other Graphics. */
		private PdfContentByte graphics;
    
		/** The lines that are written until now. */
		private ArrayList lines = new ArrayList();
    
		/** This represents the leading of the lines. */
		private float leading = 0;
    
		/** This is the current height of the document. */
		private float currentHeight = 0;
    
		/** This represents the current indentation of the PDF Elements on the top side. */
		private float indentTop = 0;
    
		/** This represents the current indentation of the PDF Elements on the bottom side. */
		private float indentBottom = 0;
    
		/** This checks if the page is empty. */
		private bool pageEmpty = true;
    
		private int textEmptySize;
		// resources
    
		/** This is the size of the next page. */
		protected Rectangle nextPageSize = null;
    
		/** This is the size of the crop box of the current Page. */
		protected Rectangle thisCropSize = null;
    
		/** This is the size of the crop box that will be used in
		 * the next page. */
		protected Rectangle cropSize = null;
    
		/** This is the FontDictionary of the current Page. */
		protected PdfFontDictionary fontDictionary;
    
		/** This is the XObjectDictionary of the current Page. */
		protected PdfXObjectDictionary xObjectDictionary;
    
		/** This is the ColorSpaceDictionary of the current Page. */
		protected PdfColorDictionary colorDictionary;
    
		/** This is the PatternDictionary of the current Page. */
		protected PdfPatternDictionary patternDictionary;
    
		/** This is the ShadingDictionary of the current Page. */
		protected PdfShadingDictionary shadingDictionary;
		// images
    
		/** This is the list with all the images in the document. */
		private Hashmap images = new Hashmap();
    
		/** This is the image that could not be shown on a previous page. */
		private Image imageWait = null;
    
		/** This is the position where the image ends. */
		private float imageEnd = -1;
    
		/** This is the indentation caused by an image on the left. */
		private float imageIndentLeft = 0;
    
		/** This is the indentation caused by an image on the right. */
		private float imageIndentRight = 0;
    
		// annotations and outlines
    
		/** This is the array containing the references to the annotations. */
		private ArrayList annotations;
    
		/** This is an array containg references to some delayed annotations. */
		private ArrayList delayedAnnotations = new ArrayList();
    
		/** This is the AcroForm object. */
		PdfAcroForm acroForm;
    
		/** This is the root outline of the document. */
		private PdfOutline rootOutline;
    
		/** This is the current <CODE>PdfOutline</CODE> in the hierarchy of outlines. */
		private PdfOutline currentOutline;
    
		/** The current active <CODE>PdfAction</CODE> when processing an <CODE>Anchor</CODE>. */
		private PdfAction currentAction = null;
    
		/**
		 * Stores the destinations keyed by name. Value is
		 * <CODE>Object[]{PdfAction,PdfIndirectReference,PdfDestintion}</CODE>.
		 */
		private SortedMap localDestinations = new SortedMap(new StringCompare());
    
		private ArrayList documentJavaScript = new ArrayList();
    
		/** these are the viewerpreferences of the document */
		private int viewerPreferences = 0;
    
		private string openActionName;
		private PdfAction openActionAction;
		private PdfDictionary additionalActions;
		private PdfPageLabels pageLabels;
    
		//add by Jin-Hsia Yang
		private bool isNewpage = false;
		private bool isParagraphE = false;
		private float paraIndent = 0;
		//end add by Jin-Hsia Yang
    
		/** margin in x direction starting from the left. Will be valid in the next page */
		protected float nextMarginLeft;
    
		/** margin in x direction starting from the right. Will be valid in the next page */
		protected float nextMarginRight;
    
		/** margin in y direction starting from the top. Will be valid in the next page */
		protected float nextMarginTop;
    
		/** margin in y direction starting from the bottom. Will be valid in the next page */
		protected float nextMarginBottom;
    
		/** The duration of the page */
		protected int duration=-1; // negative values will indicate no duration
    
		/** The page transition */
		protected PdfTransition transition=null; 
    

		// constructors
    
		/**
		 * Constructs a new PDF document.
		 * @throws DocumentException on error
		 */
    
		public PdfDocument() : base() {
			addProducer();
			addCreationDate();
		}
    
		// listener methods
    
		/**
		 * Adds a <CODE>PdfWriter</CODE> to the <CODE>PdfDocument</CODE>.
		 *
		 * @param writer the <CODE>PdfWriter</CODE> that writes everything
		 *                     what is added to this document to an outputstream.
		 * @throws DocumentException on error
		 */
    
		public void addWriter(PdfWriter writer) {
			if (this.writer == null) {
				this.writer = writer;
				acroForm = new PdfAcroForm(writer);
				return;
			}
			throw new DocumentException("You can only add a writer to a PdfDocument once.");
		}
    
		/**
		 * Sets the pagesize.
		 *
		 * @param pageSize the new pagesize
		 * @return <CODE>true</CODE> if the page size was set
		 */
    
		public override bool setPageSize(Rectangle pageSize) {
			if (writer != null && writer.isPaused()) {
				return false;
			}
			nextPageSize = new Rectangle(pageSize);
			return true;
		}
    
		/**
		 * Changes the header of this document.
		 *
		 * @param header the new header
		 */
    
		public override HeaderFooter Header {
			set {
				if (writer != null && writer.isPaused()) {
					return;
				}
				base.Header = value;
			}
		}
    
		/**
		 * Resets the header of this document.
		 */
    
		public override void resetHeader() {
			if (writer != null && writer.isPaused()) {
				return;
			}
			base.resetHeader();
		}
    
		/**
		 * Changes the footer of this document.
		 *
		 * @param	footer		the new footer
		 */
    
		public override HeaderFooter Footer {
			set {
				if (writer != null && writer.isPaused()) {
					return;
				}
				base.Footer = value;
			}
		}
    
		/**
		 * Resets the footer of this document.
		 */
    
		public override void resetFooter() {
			if (writer != null && writer.isPaused()) {
				return;
			}
			base.resetFooter();
		}
    
		/**
		 * Sets the page number to 0.
		 */
    
		public override void resetPageCount() {
			if (writer != null && writer.isPaused()) {
				return;
			}
			base.resetPageCount();
		}
    
		/**
		 * Sets the page number.
		 *
		 * @param	pageN		the new page number
		 */
    
		public override int PageCount {
			set {
				if (writer != null && writer.isPaused()) {
					return;
				}
				base.PageCount = value;
			}
		}
    
		/**
		 * Sets the <CODE>Watermark</CODE>.
		 *
		 * @param watermark the watermark to add
		 * @return <CODE>true</CODE> if the element was added, <CODE>false</CODE> if not.
		 */
    
		public override bool Add(Watermark watermark) {
			if (writer != null && writer.isPaused()) {
				return false;
			}
			this.watermark = watermark;
			return true;
		}
    
		/**
		 * Removes the <CODE>Watermark</CODE>.
		 */
    
		public override void removeWatermark() {
			if (writer != null && writer.isPaused()) {
				return;
			}
			this.watermark = null;
		}
    
		/**
		 * Sets the margins.
		 *
		 * @param	marginLeft		the margin on the left
		 * @param	marginRight		the margin on the right
		 * @param	marginTop		the margin on the top
		 * @param	marginBottom	the margin on the bottom
		 * @return	a <CODE>boolean</CODE>
		 */
    
		public override bool setMargins(float marginLeft, float marginRight, float marginTop, float marginBottom) {
			if (writer != null && writer.isPaused()) {
				return false;
			}
			nextMarginLeft = marginLeft;
			nextMarginRight = marginRight;
			nextMarginTop = marginTop;
			nextMarginBottom = marginBottom;
			return true;
		}
    
		protected PdfArray rotateAnnotations() {
			PdfArray array = new PdfArray();
			int rotation = pageSize.Rotation % 360;
			int currentPage = writer.CurrentPageNumber;
			for (int k = 0; k < annotations.Count; ++k) {
				PdfAnnotation dic = (PdfAnnotation)annotations[k];
				int page = dic.PlaceInPage;
				if (page > currentPage) {
					delayedAnnotations.Add(dic);
					continue;
				}
				if (dic.isForm()) {
					if (!dic.isUsed()) {
						Hashmap templates = dic.Templates;
						if (templates != null)
							acroForm.addFieldTemplates(templates);
					}
					PdfFormField field = (PdfFormField)dic;
					if (field.Parent == null)
						acroForm.addDocumentField(field.IndirectReference);
				}
				if (dic.isAnnotation()) {
					array.Add(dic.IndirectReference);
					if (!dic.isUsed()) {
						PdfRectangle rect = (PdfRectangle)dic.get(PdfName.RECT);
						switch (rotation) {
							case 90:
								dic.put(PdfName.RECT, new PdfRectangle(
									pageSize.Top - rect.Bottom,
									rect.Left,
									pageSize.Top - rect.Top,
									rect.Right));
								break;
							case 180:
								dic.put(PdfName.RECT, new PdfRectangle(
									pageSize.Right - rect.Left,
									pageSize.Top - rect.Bottom,
									pageSize.Right - rect.Right,
									pageSize.Top - rect.Top));
								break;
							case 270:
								dic.put(PdfName.RECT, new PdfRectangle(
									rect.Bottom,
									pageSize.Right - rect.Left,
									rect.Top,
									pageSize.Right - rect.Right));
								break;
						}
					}
				}
				if (!dic.isUsed()) {
					dic.Used = true;
					try {
						writer.addToBody(dic, dic.IndirectReference);
					}
					catch (IOException e) {
						throw e;
					}
				}
			}
			return array;
		}
    
		protected PdfDictionary codeTransition(PdfTransition transition) {
			PdfDictionary trans = new PdfDictionary(PdfName.TRANS);
			switch (transition.Type) {
				case PdfTransition.SPLITVOUT:
					trans.put(PdfName.S,new PdfName("Split"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DM,new PdfName("V"));
					trans.put(PdfName.M,new PdfName("O"));
					break;
				case PdfTransition.SPLITHOUT:
					trans.put(PdfName.S,new PdfName("Split"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DM,new PdfName("H"));
					trans.put(PdfName.M,new PdfName("O"));
					break;
				case PdfTransition.SPLITVIN:
					trans.put(PdfName.S,new PdfName("Split"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DM,new PdfName("V"));
					trans.put(PdfName.M,new PdfName("I"));
					break;
				case PdfTransition.SPLITHIN:
					trans.put(PdfName.S,new PdfName("Split"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DM,new PdfName("H"));
					trans.put(PdfName.M,new PdfName("I"));
					break;
				case PdfTransition.BLINDV:
					trans.put(PdfName.S,new PdfName("Blinds"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DM,new PdfName("V"));
					break;
				case PdfTransition.BLINDH:
					trans.put(PdfName.S,new PdfName("Blinds"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DM,new PdfName("H"));
					break;
				case PdfTransition.INBOX:
					trans.put(PdfName.S,new PdfName("Box"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.M,new PdfName("I"));
					break;
				case PdfTransition.OUTBOX:
					trans.put(PdfName.S,new PdfName("Box"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.M,new PdfName("O"));
					break;
				case PdfTransition.LRWIPE:
					trans.put(PdfName.S,new PdfName("Wipe"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DI,new PdfNumber(0));
					break;
				case PdfTransition.RLWIPE:
					trans.put(PdfName.S,new PdfName("Wipe"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DI,new PdfNumber(180));
					break;
				case PdfTransition.BTWIPE:
					trans.put(PdfName.S,new PdfName("Wipe"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DI,new PdfNumber(90));
					break;
				case PdfTransition.TBWIPE:
					trans.put(PdfName.S,new PdfName("Wipe"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DI,new PdfNumber(270));
					break;
				case PdfTransition.DISSOLVE:
					trans.put(PdfName.S,new PdfName("Dissolve"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					break;
				case PdfTransition.LRGLITTER:
					trans.put(PdfName.S,new PdfName("Glitter"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DI,new PdfNumber(0));
					break;
				case PdfTransition.TBGLITTER:
					trans.put(PdfName.S,new PdfName("Glitter"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DI,new PdfNumber(270));
					break;
				case PdfTransition.DGLITTER:
					trans.put(PdfName.S,new PdfName("Glitter"));
					trans.put(PdfName.D,new PdfNumber(transition.Duration));
					trans.put(PdfName.DI,new PdfNumber(315));
					break;
			}
			return trans;
		}
    
		/**
		 * Makes a new page and sends it to the <CODE>PdfWriter</CODE>.
		 *
		 * @return a <CODE>boolean</CODE>
		 * @throws DocumentException on error
		 */
    
		public override bool newPage() {
			//add by Jin-Hsia Yang
			isNewpage = true;
			//end add by Jin-Hsia Yang
			if (writer.DirectContent.Size == 0 && writer.DirectContentUnder.Size == 0 && (pageEmpty || (writer != null && writer.isPaused()))) {
				return false;
			}
			IPdfPageEvent pageEvent = writer.PageEvent;
			if (pageEvent != null)
				pageEvent.onEndPage(writer, this);
        
			//Added to inform any listeners that we are moving to a new page (added by David Freels)
			base.newPage();
        
			// we flush the arraylist with recently written lines
			flushLines();
			// we assemble the resources of this pages
			PdfResources resources = new PdfResources();
			int procset = PdfProcSet.PDF;
			if (fontDictionary.containsFont()) {
				resources.Add(fontDictionary);
				procset |= PdfProcSet.TEXT;
			}
			if (xObjectDictionary.containsXObject()) {
				resources.Add(xObjectDictionary);
				procset |= PdfProcSet.IMAGEC;
			}
			resources.Add(new PdfProcSet(procset));
			if (colorDictionary.containsColorSpace())
				resources.Add(colorDictionary);
			if (patternDictionary.containsPattern())
				resources.Add(patternDictionary);
			if (shadingDictionary.containsShading())
				resources.Add(shadingDictionary);
			// we make a new page and add it to the document
			PdfPage page;
			int rotation = pageSize.Rotation;
			if (rotation == 0)
				page = new PdfPage(new PdfRectangle(pageSize, rotation), thisCropSize, resources);
			else
				page = new PdfPage(new PdfRectangle(pageSize, rotation), thisCropSize, resources, new PdfNumber(rotation));
			// we add the transitions
			if (this.transition!=null) {
				page.put(PdfName.TRANS,codeTransition(this.transition));
			}
			if (this.duration>0) {
				page.put(PdfName.DUR,new PdfNumber(this.duration));
			}
			// we add the annotations
			if (annotations.Count > 0) {
				PdfArray array = rotateAnnotations();
				if (array.Size != 0)
					page.put(PdfName.ANNOTS, array);
			}
			if (!open || close) {
				throw new PdfException("The document isn't open.");
			}
			if (text.Size > textEmptySize)
				text.endText();
			else
				text = null;
			PdfIndirectReference pageReference = writer.Add(page, new PdfContents(writer.DirectContentUnder, graphics, text, writer.DirectContent, pageSize));
			// we initialize the new page
			initPage();
        
			//add by Jin-Hsia Yang
			isNewpage = false;
			//end add by Jin-Hsia Yang
        
			return true;
		}
    
		// methods to open and close a document
    
		/**
		 * Opens the document.
		 * <P>
		 * You have to open the document before you can begin to add content
		 * to the body of the document.
		 */
    
		public override void Open() {
			if (!open) {
				base.Open();
				writer.Open();
				rootOutline = new PdfOutline(writer);
				currentOutline = rootOutline;
			}
			try {
				initPage();
			}
			catch(DocumentException de) {
				throw de;
			}
		}
    
		internal void outlineTree(PdfOutline outline) {
			outline.IndirectReference = writer.PdfIndirectReference;
			if (outline.Parent != null)
				outline.put(PdfName.PARENT, outline.Parent.IndirectReference);
			ArrayList kids = outline.Kids;
			int size = kids.Count;
			for (int k = 0; k < size; ++k)
				outlineTree((PdfOutline)kids[k]);
			for (int k = 0; k < size; ++k) {
				if (k > 0)
					((PdfOutline)kids[k]).put(PdfName.PREV, ((PdfOutline)kids[k - 1]).IndirectReference);
				if (k < size - 1)
					((PdfOutline)kids[k]).put(PdfName.NEXT, ((PdfOutline)kids[k + 1]).IndirectReference);
			}
			if (size > 0) {
				outline.put(PdfName.FIRST, ((PdfOutline)kids[0]).IndirectReference);
				outline.put(PdfName.LAST, ((PdfOutline)kids[size - 1]).IndirectReference);
			}
			for (int k = 0; k < size; ++k) {
				PdfOutline kid = (PdfOutline)kids[k];
				writer.addToBody(kid, kid.IndirectReference);
			}
		}
    
		internal void writeOutlines() {
			if (rootOutline.Kids.Count == 0)
				return;
			outlineTree(rootOutline);
			writer.addToBody(rootOutline, rootOutline.IndirectReference);
		}
    
		internal void traverseOutlineCount(PdfOutline outline) {
			ArrayList kids = outline.Kids;
			PdfOutline parent = outline.Parent;
			if (kids.Count == 0) {
				if (parent != null) {
					parent.Count = parent.Count + 1;
				}
			}
			else {
				for (int k = 0; k < kids.Count; ++k) {
					traverseOutlineCount((PdfOutline)kids[k]);
				}
				if (parent != null) {
					if (outline.isOpen()) {
						parent.Count = outline.Count + parent.Count + 1;
					}
					else {
						parent.Count = parent.Count + 1;
						outline.Count = -outline.Count;
					}
				}
			}
		}
    
		internal void calculateOutlineCount() {
			if (rootOutline.Kids.Count == 0)
				return;
			traverseOutlineCount(rootOutline);
		}
		/**
		 * Closes the document.
		 * <B>
		 * Once all the content has been written in the body, you have to close
		 * the body. After that nothing can be written to the body anymore.
		 */
    
		public override void Close() {
			if (close) {
				return;
			}
			try {
				newPage();
				if (imageWait != null) newPage();
				if (annotations.Count > 0)
					throw new Exception(annotations.Count + " annotations had invalid placement pages.");
				IPdfPageEvent pageEvent = writer.PageEvent;
				if (pageEvent != null)
					pageEvent.onCloseDocument(writer, this);
				base.Close();
            
				writer.addLocalDestinations(localDestinations);
				calculateOutlineCount();
				writeOutlines();
			}
			catch(Exception e) {
				throw e;
			}
        
			writer.Close();
		}
    
		/** Adds a font to the current page.
		 * @param name the name of the font
		 * @param ref the indirect reference to this font
		 */
		public void addFont(PdfName name, PdfIndirectReference piref) {
			fontDictionary.put(name, piref);
		}
    
		public void addColor(PdfName name, PdfIndirectReference piref) {
			colorDictionary.put(name, piref);
		}
    
		public PdfName addPatternToPage(PdfPatternPainter painter) {
			PdfName name = writer.addSimplePattern(painter);
			patternDictionary.put(name, painter.IndirectReference);
			return name;
		}
    
		public void addShadingPatternToPage(PdfShadingPattern shading) {
			writer.addSimpleShadingPattern(shading);
			patternDictionary.put(shading.PatternName, shading.PatternReference);
		}
    
		public void addShadingToPage(PdfShading shading) {
			writer.addSimpleShading(shading);
			shadingDictionary.put(shading.ShadingName, shading.ShadingReference);
		}
    
		/** Adds a <CODE>PdfPTable</CODE> to the document.
		 * @param ptable the <CODE>PdfPTable</CODE> to be added to the document.
		 * @param xWidth the width the <CODE>PdfPTable</CODE> occupies in the page
		 * @throws DocumentException on error
		 */
    
		internal void addPTable(PdfPTable ptable, float xWidth) {
			if (ptable.HeaderRows >= ptable.Size)
				return;
			bool skipHeader = ptable.SkipFirstHeader;
			float headerHeight = ptable.HeaderHeight;
			float bottom = this.IndentBottom();
			float baseY = this.IndentTop() - currentHeight;
			float currentY = baseY;
			int startRow = ptable.HeaderRows;
			int currentRow = startRow;
			PdfContentByte[] cv = null;
			float eventY = 0;
			int eventRow = 0;
			int eventHeader = 0;
			float[] absoluteWidths = ptable.AbsoluteWidths;
			IPdfPTableEvent tevent = ptable.TableEvent;
			ptable.TableEvent = null;
			float[] heights = new float[ptable.Size];
			int heightsIdx = 0;
			for (currentRow = startRow; currentRow < ptable.Size; ++currentRow) {
				if (currentRow == startRow && currentY - ptable.getRowHeight(currentRow) - headerHeight < bottom) {
					if (currentHeight == 0)
						++startRow;
					else {
						newPage();
						startRow = currentRow;
						--currentRow;
						bottom = IndentBottom();
						baseY = IndentTop() - currentHeight;
						currentY = baseY;
						skipHeader = false;
					}
					continue;
				}
				if (currentY - ptable.getRowHeight(currentRow) < bottom) {
					if (cv != null) {
						if (tevent != null) {
							float[] finalHeights = new float[heightsIdx + 1];
							finalHeights[0] = eventY;
							for (int k = 0; k < heightsIdx; ++k)
								finalHeights[k + 1] = finalHeights[k] - heights[k];
							tevent.tableLayout(ptable, ptable.getEventWidths(xWidth, eventRow, eventRow + heightsIdx - eventHeader, true), finalHeights, eventHeader, eventRow, cv);
						}
						PdfPTable.endWritingRows(cv);
						cv = null;
					}
					newPage();
					startRow = currentRow;
					--currentRow;
					bottom = IndentBottom();
					baseY = IndentTop() - currentHeight;
					currentY = baseY;
				}
				else {
					if (cv == null) {
						cv = PdfPTable.beginWritingRows(writer.DirectContent);
						if (tevent != null && !skipHeader) {
							heightsIdx = 0;
							eventHeader = ptable.HeaderRows;
							for (int k = 0; k < eventHeader; ++k)
								heights[heightsIdx++] = ptable.getRowHeight(k);
							eventY = currentY;
							eventRow = currentRow;
						}
						if (!skipHeader)
							currentY = ptable.writeSelectedRows(0, ptable.HeaderRows, xWidth, currentY, cv);
						else
							skipHeader = false;
					}
					if (tevent != null) {
						heights[heightsIdx++] = ptable.getRowHeight(currentRow);
					}
					currentY = ptable.writeSelectedRows(currentRow, currentRow + 1, xWidth, currentY, cv);
				}
			}
			if (cv != null) {
				if (tevent != null) {
					float[] finalHeights = new float[heightsIdx + 1];
					finalHeights[0] = eventY;
					for (int k = 0; k < heightsIdx; ++k)
						finalHeights[k + 1] = finalHeights[k] - heights[k];
					tevent.tableLayout(ptable, ptable.getEventWidths(xWidth, eventRow, eventRow + heightsIdx - eventHeader, true), finalHeights, eventHeader, eventRow, cv);
				}
				PdfPTable.endWritingRows(cv);
				text.moveText(0, currentY - baseY);
				currentHeight = IndentTop() - currentY;
			}
			ptable.TableEvent = tevent;
        
		}
    
		/**
		 * Signals that an <CODE>Element</CODE> was added to the <CODE>Document</CODE>.
		 *
		 * @param element the element to add
		 * @return <CODE>true</CODE> if the element was added, <CODE>false</CODE> if not.
		 * @throws DocumentException when a document isn't open yet, or has been closed
		 */
    
		public override bool Add(IElement element) {
			if (writer != null && writer.isPaused()) {
				return false;
			}
			try {
            
				switch(element.Type) {
                
						// Information (headers)
					case Element.HEADER:
						// other headers than those below are not supported
						return false;
					case Element.TITLE:
						info.addTitle(((Meta)element).Content);
						break;
					case Element.SUBJECT:
						info.addSubject(((Meta)element).Content);
						break;
					case Element.KEYWORDS:
						info.addKeywords(((Meta)element).Content);
						break;
					case Element.AUTHOR:
						info.addAuthor(((Meta)element).Content);
						break;
					case Element.CREATOR:
						info.addCreator(((Meta)element).Content);
						break;
					case Element.PRODUCER:
						// you can not change the name of the producer
						info.addProducer();
						break;
					case Element.CREATIONDATE:
						// you can not set the creation date, only reset it
						info.addCreationDate();
						break;
                    
						// content (text)
					case Element.CHUNK: {
						// if there isn't a current line available, we make one
						if (line == null) {
							carriageReturn();
						}
                    
						// we cast the element to a chunk
						PdfChunk chunk = new PdfChunk((Chunk) element, currentAction); {
																						   // we try to add the chunk to the line, until we succeed
																						   PdfChunk overflow;
																						   while ((overflow = line.Add(chunk)) != null) {
																							   carriageReturn();
																							   chunk = overflow;
																						   }
																					   }
						pageEmpty = false;
						if (chunk.isAttribute(Chunk.NEWPAGE)) {
							newPage();
						}
						break;
					}
					case Element.ANCHOR: {
						Anchor anchor = (Anchor) element;
						string url = anchor.Reference;
						leading = anchor.Leading;
						if (url != null) {
							currentAction = new PdfAction(url);
						}
                    
						// we process the element
						element.process(this);
						currentAction = null;
						break;
					}
					case Element.ANNOTATION: {
						if (line == null) {
							carriageReturn();
						}
						Annotation annot = (Annotation) element;
						switch(annot.AnnotationType) {
							case Annotation.URL_NET:
								annotations.Add(new PdfAnnotation(writer, annot.Llx(), annot.Lly(), annot.Urx(), annot.Ury(), new PdfAction((Uri) annot.Attributes[Annotation.URL])));
								break;
							case Annotation.URL_AS_STRING:
								annotations.Add(new PdfAnnotation(writer, annot.Llx(), annot.Lly(), annot.Urx(), annot.Ury(), new PdfAction((string) annot.Attributes[Annotation.FILE])));
								break;
							case Annotation.FILE_DEST:
								annotations.Add(new PdfAnnotation(writer, annot.Llx(), annot.Lly(), annot.Urx(), annot.Ury(), new PdfAction((string) annot.Attributes[Annotation.FILE], (string) annot.Attributes[Annotation.DESTINATION])));
								break;
							case Annotation.FILE_PAGE:
								annotations.Add(new PdfAnnotation(writer, annot.Llx(), annot.Lly(), annot.Urx(), annot.Ury(), new PdfAction((string) annot.Attributes[Annotation.FILE], ((int) annot.Attributes[Annotation.PAGE]))));
								break;
							case Annotation.NAMED_DEST:
								annotations.Add(new PdfAnnotation(writer, annot.Llx(), annot.Lly(), annot.Urx(), annot.Ury(), new PdfAction(((int) annot.Attributes[Annotation.NAMED]))));
								break;
							case Annotation.LAUNCH:
								annotations.Add(new PdfAnnotation(writer, annot.Llx(), annot.Lly(), annot.Urx(), annot.Ury(), new PdfAction((string) annot.Attributes[Annotation.APPLICATION],(string) annot.Attributes[Annotation.PARAMETERS],(string) annot.Attributes[Annotation.OPERATION],(string) annot.Attributes[Annotation.DEFAULTDIR])));
								break;
							default:
								annotations.Add(new PdfAnnotation(writer, annot.Llx(IndentRight() - line.WidthLeft), annot.Lly(IndentTop() - currentHeight), annot.Urx(IndentRight() - line.WidthLeft + 20), annot.Ury(IndentTop() - currentHeight - 20), new PdfString(annot.Title), new PdfString(annot.Content)));
								break;
						}
						pageEmpty = false;
						break;
					}
					case Element.PHRASE: {
						// we cast the element to a phrase and set the leading of the document
						leading = ((Phrase) element).Leading;
						// we process the element
						element.process(this);
						break;
					}
					case Element.PARAGRAPH: {
						// we cast the element to a paragraph
						Paragraph paragraph = (Paragraph) element;
                    
						// we adjust the parameters of the document
						alignment = paragraph.Alignment;
						leading = paragraph.Leading;
                    
						carriageReturn();
						// we don't want to make orphans/widows
						if (currentHeight + line.Height + leading > IndentTop() - IndentBottom()) {
							newPage();
						}
						indentLeft += paragraph.IndentationLeft;
						indentRight += paragraph.IndentationRight;
                    
						carriageReturn();
                    
						//add by Jin-Hsia Yang
						isParagraphE = true;
						paraIndent += paragraph.IndentationLeft;
						//end add by Jin-Hsia Yang
                    
						IPdfPageEvent pageEvent = writer.PageEvent;
						if (pageEvent != null && isParagraph)
							pageEvent.onParagraph(writer, this, IndentTop() - currentHeight);
                    
						// if a paragraph has to be kept together, we wrap it in a table object
						if (paragraph.KeepTogether) {
							Table table = new Table(1, 1);
							table.Offset = 0f;
							table.Border = Table.NO_BORDER;
							table.WidthPercentage = 100f;
							table.TableFitsPage = true;
							Cell cell = new Cell(paragraph);
							cell.Border = Table.NO_BORDER;
							table.addCell(cell);
							this.Add(table);
							break;
						}
						else
							// we process the paragraph
							element.process(this);
                    
						//add by Jin-Hsia Yang and blowagie
						paraIndent -= paragraph.IndentationLeft;
						//end add by Jin-Hsia Yang and blowagie
                    
						carriageReturn();
                    
						if (pageEvent != null && isParagraph)
							pageEvent.onParagraphEnd(writer, this, IndentTop() - currentHeight);
                    
						alignment = Element.ALIGN_LEFT;
						indentLeft -= paragraph.IndentationLeft;
						indentRight -= paragraph.IndentationRight;
                    
						//add by Jin-Hsia Yang
						isParagraphE = false;
						//end add by Jin-Hsia Yang
                    
						break;
					}
					case Element.SECTION:
					case Element.CHAPTER: {
						// Chapters and Sections only differ in their constructor
						// so we cast both to a Section
						Section section = (Section) element;
                    
						// if the section is a chapter, we begin a new page
						if (section.isChapter()) {
							newPage();
						}
							// otherwise, we begin a new line
						else {
							newLine();
						}
						float fith = IndentTop() - currentHeight;
						int rotation = pageSize.Rotation;
						if (rotation == 90 || rotation == 180)
							fith = pageSize.Height - fith;
						PdfDestination destination = new PdfDestination(PdfDestination.FITH, fith);
						while (currentOutline.Level >= section.Depth) {
							currentOutline = currentOutline.Parent;
						}
						PdfOutline outline = new PdfOutline(currentOutline, destination, section.Title, section.BookmarkOpen);
						currentOutline = outline;
                    
						// some values are set
						carriageReturn();
						indentLeft += section.IndentationLeft;
						indentRight += section.IndentationRight;
                    
						IPdfPageEvent pageEvent = writer.PageEvent;
						if (pageEvent != null)
							if (element.Type == Element.CHAPTER)
								pageEvent.onChapter(writer, this, IndentTop() - currentHeight, section.Title);
							else
								pageEvent.onSection(writer, this, IndentTop() - currentHeight, section.Depth, section.Title);
                    
						// the title of the section (if any has to be printed)
						if (section.Title != null) {
							isParagraph = false;
							Add(section.Title);
							isParagraph = true;
						}
						indentLeft += section.Indentation;
						// we process the section
						element.process(this);
						// some parameters are set back to normal again
						indentLeft -= section.IndentationLeft + section.Indentation;
						indentRight -= section.IndentationRight;
                    
						if (pageEvent != null)
							if (element.Type == Element.CHAPTER)
								pageEvent.onChapterEnd(writer, this, IndentTop() - currentHeight);
							else
								pageEvent.onSectionEnd(writer, this, IndentTop() - currentHeight);
                    
						break;
					}
					case Element.LIST: {
						// we cast the element to a List
						List list = (List) element;
						// we adjust the document
						listIndentLeft += list.IndentationLeft;
						indentRight += list.IndentationRight;
						// we process the items in the list
						element.process(this);
						// some parameters are set back to normal again
						listIndentLeft -= list.IndentationLeft;
						indentRight -= list.IndentationRight;
						break;
					}
					case Element.LISTITEM: {
						// we cast the element to a ListItem
						ListItem listItem = (ListItem) element;
						// we adjust the document
						alignment = listItem.Alignment;
						listIndentLeft += listItem.IndentationLeft;
						indentRight += listItem.IndentationRight;
						leading = listItem.Leading;
						carriageReturn();
						// we prepare the current line to be able to show us the listsymbol
						line.ListItem = listItem;
						// we process the item
						element.process(this);
						// if the last line is justified, it should be aligned to the left
						//				if (line.hasToBeJustified()) {
						//					line.resetAlignment();
						//				}
						// some parameters are set back to normal again
						carriageReturn();
						listIndentLeft -= listItem.IndentationLeft;
						indentRight -= listItem.IndentationRight;
						break;
					}
					case Element.RECTANGLE: {
						Rectangle rectangle = (Rectangle) element;
						graphics.rectangle(rectangle);
						pageEmpty = false;
						break;
					}
					case Element.PTABLE: {
						// before every table, we add a new line and flush all lines
						newLine();
						flushLines();
						PdfPTable ptable = (PdfPTable)element;
						float totalWidth = (IndentRight() - IndentLeft()) * ptable.WidthPercentage / 100;
						float xWidth = 0;
						switch (ptable.HorizontalAlignment) {
							case Element.ALIGN_LEFT:
								xWidth = IndentLeft();
								break;
							case Element.ALIGN_RIGHT:
								xWidth = IndentRight() - totalWidth;
								break;
							default:
								xWidth = (IndentRight() + IndentLeft() - totalWidth) / 2;
								break;
						}
						ptable.TotalWidth = totalWidth;
						addPTable(ptable, xWidth);
                    
						break;
					}
					case Element.TABLE: {
                    
						/**
						 * This is a list of people who worked on the Table functionality.
						 * To see who did what, please check the CVS repository:
						 *
						 * Leslie Baski
						 * Matt Benson
						 * Francesco De Milato
						 * David Freels
						 * Bruno Lowagie
						 * Veerendra Namineni
						 * Geert Poels
						 * Tom Ring
						 * Paulo Soares
						 */
                    
						// correct table : fill empty cells/ parse table in table
						((Table) element).complete();
                    
						// before every table, we add a new line and flush all lines
						float offset = ((Table)element).Offset;
						if (float.IsNaN(offset)) offset = leading;
						carriageReturn();
						lines.Add(new PdfLine(IndentLeft(), IndentRight(), alignment, offset));
						currentHeight += offset;
						flushLines();
                    
						// initialisation of parameters
						float pagetop = IndentTop();
						float oldHeight = currentHeight;
						float cellDisplacement;
						PdfCell cell;
						PdfContentByte cellGraphics = new PdfContentByte(writer);
                    
						// constructing the PdfTable
						PdfTable table = new PdfTable((Table) element, IndentLeft(), IndentRight(), currentHeight > 0 ? (pagetop - currentHeight) - 6 : pagetop);
                    
						bool tableHasToFit = ((Table) element).hasToFitPageTable() ? table.Bottom < IndentBottom() : false;
						if (pageEmpty) tableHasToFit = false;
						bool cellsHaveToFit = ((Table) element).hasToFitPageCells();
                    
						// drawing the table
						ArrayList cells = table.Cells;
						ArrayList headercells = null;
						while (cells.Count != 0) {
							// initialisation of some extra parameters;
							float lostTableBottom = 0;
							float lostTableTop = 0;
                        
							// loop over the cells
							bool cellsShown = false;
							int currentRownumber = 0;
							for (ListIterator iterator = new ListIterator(cells); iterator.hasNext && !tableHasToFit; ) {
								cell = (PdfCell) iterator.Next;
								if (cell.Rownumber != currentRownumber && !cell.isHeader() && cellsHaveToFit) {
									currentRownumber = cell.Rownumber;
									int cellCount = 0;
									bool cellsFit = true;
									while (cell.Rownumber == currentRownumber && cellsFit && iterator.hasNext) {
										if (cell.Bottom < IndentBottom()) {
											cellsFit = false;
										}
										cell = (PdfCell) iterator.Next;
										cellCount++;
									}
									if (!cellsFit) {
										break;
									}
									for (int i = cellCount; i >= 0; i--) {
										cell = (PdfCell) iterator.Previous;
									}
									cell = (PdfCell) iterator.Next;
								}
								lines = cell.getLines(pagetop, IndentBottom());
								// if there are lines to add, add them
								if (lines != null && lines.Count > 0) {
									// we paint the borders of the cells
									cellsShown = true;
									cellGraphics.rectangle(cell.rectangle(pagetop, IndentBottom()));
									lostTableBottom = Math.Max(cell.Bottom, IndentBottom());
									lostTableTop = cell.Top;
                                
									// we write the text
									float cellTop = cell.top(pagetop - oldHeight);
									text.moveText(0, cellTop);
									cellDisplacement = flushLines() - cellTop;
									text.moveText(0, cellDisplacement);
									if (oldHeight + cellDisplacement > currentHeight) {
										currentHeight = oldHeight + cellDisplacement;
									}
								}
								ArrayList images = cell.getImages(pagetop, IndentBottom());
								foreach(Image image in images) {
									cellsShown = true;
									addImage(graphics, image, 0, 0, 0, 0, 0, 0);
								}
								// if a cell is allready added completely, remove it
								if (cell.mayBeRemoved()) {
									iterator.Remove();
								}
							}
							tableHasToFit = false;
							// we paint the graphics of the table after looping through all the cells
							if (cellsShown) {
								Rectangle tablerec = new Rectangle(table);
								tablerec.Border = table.Border;
								tablerec.BorderWidth = table.BorderWidth;
								tablerec.BorderColor = table.BorderColor;
								tablerec.BackgroundColor = table.BackgroundColor;
								tablerec.GrayFill = table.GrayFill;
								PdfContentByte under = writer.DirectContentUnder;
								under.rectangle(tablerec.rectangle(Top, IndentBottom()));
								under.Add(cellGraphics);
								// bugfix by Gerald Fehringer: now again add the border for the table
								// since it might have been covered by cell backgrounds
								tablerec.GrayFill = 0;
								tablerec.BackgroundColor = null;
								under.rectangle(tablerec.rectangle(Top, IndentBottom()));
								// end bugfix
							}
							cellGraphics = new PdfContentByte(null);
							// if the table continues on the next page
							if (cells.Count != 0) {
                            
								graphics.LineWidth = table.BorderWidth;
								if (cellsShown && (table.Border & Rectangle.BOTTOM) == Rectangle.BOTTOM) {
									// Draw the bottom line
                                
									// the color is set to the color of the element
									Color tColor = table.BorderColor;
									if (tColor != null) {
										graphics.ColorStroke = tColor;
									}
									graphics.moveTo(table.Left, Math.Max(table.Bottom, IndentBottom()));
									graphics.lineTo(table.Right,  Math.Max(table.Bottom, IndentBottom()));
									graphics.stroke();
									if (tColor != null) {
										graphics.resetRGBColorStroke();
									}
								}
                            
								// old page
								pageEmpty = false;
								float difference = lostTableBottom;
                            
								// new page
								newPage();
								flushLines();
                            
								// this part repeats the table headers (if any)
								headercells = table.HeaderCells;
								int size = headercells.Count;
								if (size > 0) {
									// this is the top of the headersection
									cell = (PdfCell) headercells[0];
									float oldTop = cell.Top;
									// loop over all the cells of the table header
									for (int i = 0; i < size; i++) {
										cell = (PdfCell) headercells[i];
										// calculation of the new cellpositions
										cell.Top = IndentTop() - oldTop + cell.Top;
										cell.Bottom = IndentTop() - oldTop + cell.Bottom - 2f * table.Cellspacing;
										pagetop = cell.Bottom;
										// we paint the borders of the cell
										cellGraphics.rectangle(cell.rectangle(IndentTop(), IndentBottom()));
										// we write the text of the cell
										ArrayList images = cell.getImages(IndentTop(), IndentBottom());
										foreach(Image image in images) {
											cellsShown = true;
											addImage(graphics, image, 0, 0, 0, 0, 0, 0);
										}
										lines = cell.getLines(IndentTop(), IndentBottom());
										float cellTop = cell.top(IndentTop());
										text.moveText(0, cellTop);
										cellDisplacement = flushLines() - cellTop;
										text.moveText(0, cellDisplacement);
									}
                                
									currentHeight =  IndentTop() - pagetop - table.Cellspacing;
									text.moveText(0, pagetop - IndentTop() + table.Cellspacing - currentHeight);
								}
								oldHeight = currentHeight;
                            
								// calculating the new positions of the table and the cells
								size = Math.Min(cells.Count, table.Columns);
								int ii = 0;
								while (ii < size ) {
									cell = (PdfCell) cells[ii];
									if (cell.top(-table.Cellspacing) > lostTableBottom) {
										float newBottom = pagetop - difference + cell.Bottom;
										float neededHeight = cell.RemainingHeight;
										if (newBottom > pagetop - neededHeight) {
											difference += newBottom - (pagetop - neededHeight);
										}
									}
									ii++;
								}
								size = cells.Count;
								table.Top = IndentTop();
								table.Bottom = pagetop - difference + table.bottom(table.Cellspacing);
								for (int i = 0; i < size; i++) {
									System.Diagnostics.Trace.WriteLine(i);
									cell = (PdfCell) cells[i];
									float newBottom = pagetop - difference + cell.Bottom;
									float newTop = pagetop - difference + cell.top(-table.Cellspacing);
									if (newTop > IndentTop() - currentHeight - table.Cellspacing) {
										newTop = IndentTop() - currentHeight - table.Cellspacing;
										//newTop = newBottom + cell.RemainingHeight;
									}
									cell.Top = newTop;
									cell.Bottom = newBottom;
								}
							}
						}
                    
						text.moveText(0, oldHeight - currentHeight);
						lines.Add(line);
						currentHeight += line.Height - pagetop + IndentTop();
						line = new PdfLine(IndentLeft(), IndentRight(), alignment, leading);
						pageEmpty = false;
						break;
					}
					case Element.GIF:
					case Element.JPEG:
					case Element.PNG:
					case Element.IMGRAW:
					case Element.IMGTEMPLATE: {
						carriageReturn();
						Add((Image) element);
						pageEmpty = false;
						break;
					}
					case Element.GRAPHIC: {
						Graphic graphic = (Graphic) element;
						graphic.processAttributes(IndentLeft(), IndentBottom(), IndentRight(), IndentTop(), IndentTop() - currentHeight);
						graphics.Add(graphic);
						pageEmpty = false;
						break;
					}
					default:
						return false;
				}
				return true;
			}
			catch(Exception e) {
				Console.Error.WriteLine("Error: " + e.Message);
				throw new DocumentException(e.Message);
			}
		}
    
		// methods to add Content
    
		/**
		 * Adds an image to the document and to the page resources.
		 * @param image the <CODE>Image</CODE> to add
		 * @return the name of the image added
		 * @throws PdfException on error
		 * @throws DocumentException on error
		 */
    
		internal PdfName addDirectImage(Image image) {
			Image maskImage = image.ImageMask;
			if (maskImage != null)
				addDirectImage(maskImage);
			PdfName name = addDirectImageSimple(image);
			if (!image.isImgTemplate())
				xObjectDictionary.put(name, writer.getImageReference(name));
			return name;
		}
    
		/** Adds an image to the document but not to the page resources. It is used with
		 * templates and <CODE>Document.Add(Image)</CODE>.
		 * @param image the <CODE>Image</CODE> to add
		 * @return the name of the image added
		 * @throws PdfException on error
		 * @throws DocumentException on error
		 */
		internal PdfName addDirectImageSimple(Image image) {
			PdfName name;
			// if the images is already added, just retrieve the name
			if (images.ContainsKey(image.MySerialId)) {
				name = (PdfName) images[image.MySerialId];
			}
				// if it's a new image, add it to the document
			else {
				if (image.isImgTemplate()) {
					name = new PdfName("img" + images.Count);
					if (image.TemplateData == null) {
						try {
							ImgWMF wmf = (ImgWMF)image;
							wmf.readWMF(writer.DirectContent.createTemplate(0, 0));
						}
						catch (Exception e) {
							throw new DocumentException(e.Message);
						}
					}
				}
				else {
					Image maskImage = image.ImageMask;
					PdfIndirectReference maskRef = null;
					if (maskImage != null) {
						PdfName mname = (PdfName)images[maskImage.MySerialId];
						maskRef = writer.getImageReference(mname);
					}
					PdfImage i = new PdfImage(image, "img" + images.Count, maskRef);
					//					if (image.hasICCProfile()) {
					//						PdfICCBased icc = new PdfICCBased(image.getICCProfile());
					//						PdfIndirectReference iccRef = writer.Add(icc);
					//						PdfArray iccArray = new PdfArray();
					//						iccArray.Add(PdfName.ICCBASED);
					//						iccArray.Add(iccRef);
					//						i.Add(PdfName.COLORSPACE, iccArray);
					//					}
					writer.Add(i);
					name = i.Name;
				}
				images.Add(image.MySerialId, name);
			}
			return name;
		}
    
		/**
		 * Adds an image to the Graphics object.
		 *
		 * @param image the image
		 * @param a an element of the transformation matrix
		 * @param b an element of the transformation matrix
		 * @param c an element of the transformation matrix
		 * @param d an element of the transformation matrix
		 * @param e an element of the transformation matrix
		 * @param f an element of the transformation matrix
		 * @throws DocumentException
		 */
    
		private void addImage(PdfContentByte graphics, Image image, float a, float b, float c, float d, float e, float f) {
			Annotation annotation = image.Annotation;
			if (image.hasAbsolutePosition()) {
				graphics.addImage(image);
				if (annotation != null) {
					annotation.setDimensions(image.AbsoluteX, image.AbsoluteY, image.AbsoluteX + image.ScaledWidth, image.AbsoluteY + image.ScaledHeight);
					Add(annotation);
				}
			}
			else {
				graphics.addImage(image, a, b, c, d, e, f);
				if (annotation != null) {
					annotation.setDimensions(e, f, e + image.ScaledWidth, f + image.ScaledHeight);
					Add(annotation);
				}
			}
		}
    
		/**
		 * Adds an image to the document.
		 * @param image the <CODE>Image</CODE> to add
		 * @throws PdfException on error
		 * @throws DocumentException on error
		 */
    
		private void Add(Image image) {
			pageEmpty = false;
        
			if (image.hasAbsolutePosition()) {
				addImage(graphics, image, 0, 0, 0, 0, 0, 0);
				return;
			}
        
			// if there isn't enough room for the image on this page, save it for the next page
			if (currentHeight != 0 && IndentTop() - currentHeight - image.ScaledHeight < IndentBottom()) {
				if (imageWait == null) {
					imageWait = image;
					return;
				}
				newPage();
				if (currentHeight != 0 && IndentTop() - currentHeight - image.ScaledHeight < IndentBottom()) {
					imageWait = image;
					return;
				}
			}
			// avoid endless loops
			if (image == imageWait)
				imageWait = null;
			bool textwrap = (image.Alignment & Image.TEXTWRAP) == Image.TEXTWRAP
				&& !((image.Alignment & Image.MIDDLE) == Image.MIDDLE);
			bool underlying = (image.Alignment & Image.UNDERLYING) == Image.UNDERLYING;
			float diff = leading / 2;
			if (textwrap) {
				diff += leading;
			}
			float lowerleft = IndentTop() - currentHeight - image.ScaledHeight -diff;
			float[] mt = image.Matrix;
			float startPosition = IndentLeft() - mt[4];
			if ((image.Alignment & Image.MIDDLE) == Image.RIGHT) startPosition = IndentRight() - image.ScaledWidth - mt[4];
			if ((image.Alignment & Image.MIDDLE) == Image.MIDDLE) startPosition = IndentLeft() + ((IndentRight() - IndentLeft() - image.ScaledWidth) / 2) - mt[4];
			if (image.hasAbsoluteX()) startPosition = image.AbsoluteX;
			addImage(graphics, image, mt[0], mt[1], mt[2], mt[3], startPosition, lowerleft - mt[5]);
			if (textwrap) {
				if (imageEnd < 0 || imageEnd < currentHeight + image.ScaledHeight + diff) {
					imageEnd = currentHeight + image.ScaledHeight + diff;
				}
				if ((image.Alignment & Image.RIGHT) == Image.RIGHT) {
					imageIndentRight += image.ScaledWidth;
				}
				else {
					imageIndentLeft += image.ScaledWidth;
				}
			}
			if (!(textwrap || underlying)) {
				currentHeight += image.ScaledHeight + diff;
				flushLines();
				text.moveText(0, - (image.ScaledHeight + diff));
				newLine();
			}
		}
    
		/**
		 * Initializes a page.
		 * <P>
		 * If the footer/header is set, it is printed.
		 * @throws DocumentException on error
		 */
    
		private void initPage() {
        
			// initialisation of some page objects
			annotations = delayedAnnotations;
			delayedAnnotations = new ArrayList();
			fontDictionary = new PdfFontDictionary();
			xObjectDictionary = new PdfXObjectDictionary();
			colorDictionary = new PdfColorDictionary();
			patternDictionary = new PdfPatternDictionary();
			shadingDictionary = new PdfShadingDictionary();
			writer.resetContent();
        
			// the pagenumber is incremented
			pageN++;
        
			// graphics and text are initialized
			float oldleading = leading;
			int oldAlignment = alignment;
        
			marginLeft = nextMarginLeft;
			marginRight = nextMarginRight;
			marginTop = nextMarginTop;
			marginBottom = nextMarginBottom;
			imageEnd = -1;
			imageIndentRight = 0;
			imageIndentLeft = 0;
			graphics = new PdfContentByte(writer);
			text = new PdfContentByte(writer);
			text.beginText();
			text.moveText(this.Left, Top);
			textEmptySize = text.Size;
			text.reset();
			text.beginText();
			leading = 16;
			indentBottom = 0;
			indentTop = 0;
			currentHeight = 0;
        
			// backgroundcolors, etc...
			pageSize = nextPageSize;
			thisCropSize = cropSize;
			if (pageSize.BackgroundColor != null
				|| pageSize.hasBorders()
				|| pageSize.BorderColor != null
				|| pageSize.GrayFill > 0) {
				Add(pageSize);
			}
        
			// if there is a watermark, the watermark is added
			if (watermark != null) {
				float[] mt = watermark.Matrix;
				addImage(graphics, watermark, mt[0], mt[1], mt[2], mt[3], watermark.OffsetX - mt[4], watermark.OffsetY - mt[5]);
			}
        
			// if there is a footer, the footer is added
			if (footer != null) {
				footer.PageNumber = pageN;
				leading = footer.paragraph().Leading;
				Add(footer.paragraph());
				// adding the footer limits the height
				indentBottom = currentHeight;
				text.moveText(this.Left, IndentBottom());
				flushLines();
				text.moveText(-this.Left, -this.Bottom);
				footer.Top = bottom(currentHeight);
				footer.Bottom = this.Bottom - (0.75f * leading);
				footer.Left = Left;
				footer.Right = Right;
				graphics.rectangle(footer);
				indentBottom = currentHeight + leading * 2;
				currentHeight = 0;
			}
        
			// we move to the left/top position of the page
			text.moveText(Left, Top);
        
			// if there is a header, the header = added
			if (header != null) {
				header.PageNumber = pageN;
				leading = header.paragraph().Leading;
				text.moveText(0, leading);
				Add(header.paragraph());
				newLine();
				indentTop = currentHeight - leading;
				header.Top = Top + leading;
				header.Bottom = IndentTop() + leading * 2 / 3;
				header.Left = Left;
				header.Right = Right;
				graphics.rectangle(header);
				flushLines();
				currentHeight = 0;
			}
        
			pageEmpty = true;
        
			// if there is an image waiting to be drawn, draw it
			try {
				if (imageWait != null) {
					Add(imageWait);
					imageWait = null;
				}
			}
			catch(Exception e) {
				throw e;
			}
        
			leading = oldleading;
			alignment = oldAlignment;
			carriageReturn();
			IPdfPageEvent pageEvent = writer.PageEvent;
			if (pageEvent != null) {
				if (firstPageEvent) {
					pageEvent.onOpenDocument(writer, this);
				}
				pageEvent.onStartPage(writer, this);
			}
			firstPageEvent = false;
		}
    
		/**
		 * If the current line is not empty or null, it is added to the arraylist
		 * of lines and a new empty line is added.
		 * @throws DocumentException on error
		 */
    
		private void carriageReturn() {
			// the arraylist with lines may not be null
			if (lines == null) {
				lines = new ArrayList();
			}
			// If the current line is not null
			if (line != null) {
				// we check if the end of the page is reached (bugfix by Francois Gravel)
				if (currentHeight + line.Height + leading < IndentTop() - IndentBottom()) {
					// if so nonempty lines are added and the heigt is augmented
					if (line.Size > 0) {
						currentHeight += line.Height;
						lines.Add(line);
						pageEmpty = false;
					}
				}
					// if the end of the line is reached, we start a new page
				else {
					newPage();
				}
			}
			if (imageEnd > -1 && currentHeight > imageEnd) {
				imageEnd = -1;
				imageIndentRight = 0;
				imageIndentLeft = 0;
			}
			// a new current line is constructed
			line = new PdfLine(IndentLeft(), IndentRight(), alignment, leading);
		}
    
		/**
		 * Adds the current line to the list of lines and also adds an empty line.
		 * @throws DocumentException on error
		 */
    
		private void newLine() {
			carriageReturn();
			if (lines != null && lines.Count > 0) {
				lines.Add(line);
				currentHeight += line.Height;
			}
			line = new PdfLine(IndentLeft(), IndentRight(), alignment, leading);
		}
    
		/**
		 * Writes all the lines to the text-object.
		 *
		 * @return the displacement that was caused
		 * @throws DocumentException on error
		 */
    
		private float flushLines() {
        
			// checks if the ArrayList with the lines is not null
			if (lines == null) {
				return 0;
			}
        
			//add by Jin-Hsia Yang
			bool newline=false;
			//end add by Jin-Hsia Yang
        
			// checks if a new Line has to be made.
			if (line != null && line.Size > 0) {
				lines.Add(line);
				line = new PdfLine(IndentLeft(), IndentRight(), alignment, leading);
            
				//add by Jin-Hsia Yang
				newline=true;
				//end add by Jin-Hsia Yang
            
			}
        
			// checks if the ArrayList with the lines is empty
			if (lines.Count == 0) {
				return 0;
			}
        
			// initialisation of some parameters
			Object[] currentValues = new Object[2];
			PdfFont currentFont = null;
			float displacement = 0;
			//PdfLine l;
			PdfChunk chunk;
			float lastBaseFactor = 0;
			currentValues[1] = lastBaseFactor;
			// looping over all the lines
			int counter = 0;
			foreach(PdfLine l in lines) {				
				System.Diagnostics.Trace.WriteLine(++counter);
				if(isParagraphE && isNewpage && newline) {
					newline=false;
					text.moveText(l.IndentLeft - IndentLeft() + listIndentLeft + paraIndent,-l.Height);
				}
				else {
					text.moveText(l.IndentLeft - IndentLeft() + listIndentLeft, -l.Height);
				}
            
				// is the line preceeded by a symbol?
				if (l.ListSymbol != null) {
					chunk = l.ListSymbol;
					text.moveText(- l.ListIndent, 0);
					if (chunk.Font.CompareTo(currentFont) != 0) {
						currentFont = chunk.Font;
						text.setFontAndSize(currentFont.Font, currentFont.Size);
					}
					if (chunk.color != null) {
						Color color = chunk.color;
						text.ColorFill = color;
						text.showText(chunk.ToString());
						text.resetRGBColorFill();
					}
					else if (chunk.isImage()) {
						Image image = chunk.Image;
						float[] matrix = image.Matrix;
						float xMarker = text.XTLM;
						float yMarker = text.YTLM;
						matrix[Image.CX] = xMarker + chunk.ImageOffsetX - matrix[Image.CX];
						matrix[Image.CY] = yMarker + chunk.ImageOffsetY - matrix[Image.CY];
						addImage(graphics, image, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
					}
					else {
						text.showText(chunk.ToString());
					}
					text.moveText(l.ListIndent, 0);
				}
            
				currentValues[0] = currentFont;
            
				writeLineToContent(l, text, graphics, currentValues, writer.SpaceCharRatio);
            
				currentFont = (PdfFont)currentValues[0];
            
				displacement += l.Height;
				if (IndentLeft() - listIndentLeft != l.IndentLeft) {
					text.moveText(IndentLeft() - l.IndentLeft - listIndentLeft, 0);
				}
            
			}
			lines = new ArrayList();
			return displacement;
		}
    
		// methods to retrieve information
    
		/**
		 * Gets the <CODE>PdfInfo</CODE>-object.
		 *
		 * @return	<CODE>PdfInfo</COPE>
		 */
    
		internal PdfInfo Info {
			get {
				return info;
			}
		}
    
		/**
		 * Gets the <CODE>PdfCatalog</CODE>-object.
		 *
		 * @param pages an indirect reference to this document pages
		 * @return <CODE>PdfCatalog</CODE>
		 */
    
		internal PdfCatalog getCatalog(PdfIndirectReference pages) {
			PdfCatalog catalog;
			if (rootOutline.Kids.Count > 0) {
				catalog = new PdfCatalog(this, pages, rootOutline.IndirectReference);
			}
			else
				catalog = new PdfCatalog(this, pages);
			if (openActionName != null) {
				PdfAction action = getLocalGotoAction(openActionName);
				catalog.OpenAction = action;
			}
			else if (openActionAction != null)
				catalog.OpenAction = openActionAction;
        
			if (additionalActions != null)   {
				catalog.AdditionalActions = additionalActions;
			}
        
			if (pageLabels != null)
				catalog.PageLabels = pageLabels;
			catalog.addNames(localDestinations, documentJavaScript, writer);
			catalog.ViewerPreferences = viewerPreferences;
			if (acroForm.isValid()) {
				try {
					catalog.AcroForm = writer.addToBody(acroForm).IndirectReference;
				}
				catch (IOException e) {
					throw e;
				}
			}
			return catalog;
		}
    
		// methods concerning the layout
    
		/**
		 * Returns the bottomvalue of a <CODE>Table</CODE> if it were added to this document.
		 *
		 * @param	table	the table that may or may not be added to this document
		 * @return	a bottom value
		 */
    
		internal float bottom(Table table) {
			// where will the table begin?
			float h = (currentHeight > 0) ? IndentTop() - currentHeight - 2f * leading : IndentTop();
			// constructing a PdfTable
			PdfTable tmp = new PdfTable(table, IndentLeft(), IndentRight(), h);
			return tmp.Bottom;
		}
    
		/**
		 * Checks if a <CODE>PdfPTable</CODE> fits the current page of the <CODE>PdfDocument</CODE>.
		 *
		 * @param	table	the table that has to be checked
		 * @param	margin	a certain margin
		 * @return	<CODE>true</CODE> if the <CODE>PdfPTable</CODE> fits the page, <CODE>false</CODE> otherwise.
		 */
    
		internal bool fitsPage(PdfPTable table, float margin) {
			float totalWidth = (IndentRight() - IndentLeft()) * table.WidthPercentage / 100;
			table.TotalWidth = totalWidth;
			return table.TotalHeight <= IndentTop() - currentHeight - IndentBottom() - margin;
		}
    
		/**
		 * Gets the indentation on the left side.
		 *
		 * @return	a margin
		 */
    
		private float IndentLeft() {
			return left(indentLeft + listIndentLeft + imageIndentLeft);
		}
    
		/**
		 * Gets the indentation on the right side.
		 *
		 * @return	a margin
		 */
    
		private float IndentRight() {
			return right(indentRight + imageIndentRight);
		}
    
		/**
		 * Gets the indentation on the top side.
		 *
		 * @return	a margin
		 */
    
		private float IndentTop() {
			return top(indentTop);
		}
    
		/**
		 * Gets the indentation on the bottom side.
		 *
		 * @return	a margin
		 */
    
		internal float IndentBottom() {
			return bottom(indentBottom);
		}
    
		/**
		 * Adds a named outline to the document .
		 * @param outline the outline to be added
		 * @param name the name of this local destination
		 */
		internal void addOutline(PdfOutline outline, string name) {
			localDestination(name, outline.PdfDestination);
		}
    
		/**
		 * Gets the AcroForm object.
		 */
    
		public PdfAcroForm AcroForm {
			get {
				return acroForm;
			}
		}
    
		/**
		 * Gets the root outline. All the outlines must be created with a parent.
		 * The first level is created with this outline.
		 * @return the root outline
		 */
		public PdfOutline RootOutline {
			get {
				return rootOutline;
			}
		}
    
		/**
		 * Adds a template to the page dictionary.
		 * @param template the template to be added
		 * @return the name by which this template is identified
		 */
		internal PdfName addTemplateToPage(PdfTemplate template) {
			PdfName name = writer.addDirectTemplateSimple(template);
			xObjectDictionary.put(name, template.IndirectReference);
			return name;
		}
    
		/**
		 * Writes a text line to the document. It takes care of all the attributes.
		 * <P>
		 * Before entering the line position must have been established and the
		 * <CODE>text</CODE> argument must be in text object scope (<CODE>beginText()</CODE>).
		 * @param line the line to be written
		 * @param text the <CODE>PdfContentByte</CODE> where the text will be written to
		 * @param graphics the <CODE>PdfContentByte</CODE> where the graphics will be written to
		 * @param currentValues the current font and extra spacing values
		 * @throws DocumentException on error
		 */
		internal void writeLineToContent(PdfLine line, PdfContentByte text, PdfContentByte graphics, Object[] currentValues, float ratio)  {
			PdfFont currentFont = (PdfFont)(currentValues[0]);
			float lastBaseFactor = ((float)(currentValues[1]));
			//PdfChunk chunk;
			int numberOfSpaces;
			int lineLen;
			bool isJustified;
			float hangingCorrection = 0;
			bool adjustMatrix = false;
        
			numberOfSpaces = line.NumberOfSpaces;
			lineLen = line.ToString().Length;
			// does the line need to be justified?
			isJustified = line.hasToBeJustified() && (numberOfSpaces != 0 || lineLen > 1);
			if (isJustified) {
				if (line.isNewlineSplit() && line.WidthLeft >= (lastBaseFactor * (ratio * numberOfSpaces + lineLen - 1))) {
					if (line.IsRTL) {
						text.moveText(line.WidthLeft - lastBaseFactor * (ratio * numberOfSpaces + lineLen - 1), 0);
					}
					text.WordSpacing = ratio * lastBaseFactor;
					text.CharacterSpacing = lastBaseFactor;
				}
				else {
					float width = line.WidthLeft;
					PdfChunk last = line.getChunk(line.Size - 1);
					if (last != null) {
						string s = last.ToString();
						char c;
						if (s.Length > 0 && hangingPunctuation.IndexOf((c = s[s.Length - 1])) >= 0) {
							float oldWidth = width;
							width += last.Font.width(c) * 0.4f;
							hangingCorrection = width - oldWidth;
						}
					}
					float baseFactor = width / (ratio * numberOfSpaces + lineLen - 1);
					text.WordSpacing = ratio * baseFactor;
					text.CharacterSpacing = baseFactor;
					lastBaseFactor = baseFactor;
				}
			}
        
			int lastChunkStroke = line.LastStrokeChunk;
			int chunkStrokeIdx = 0;
			float xMarker = text.XTLM;
			float baseXMarker = xMarker;
			float yMarker = text.YTLM;
			bool imageWasPresent = false;
        
			// looping over all the chunks in 1 line
			foreach(PdfChunk chunk in line) {
				if (chunk.Font.CompareTo(currentFont) != 0) {
					currentFont = chunk.Font;
					text.setFontAndSize(currentFont.Font, currentFont.Size);
				}
				Color color = chunk.color;
				float rise = 0;
				object fr = chunk.getAttribute(Chunk.SUBSUPSCRIPT);
				/*
								if (fr != null)
									rise = (float)fr;
								if (color != null)
									text.ColorFill = color;
								if (rise != 0)
									text.TextRise = rise;
            
								if (chunk.isImage()) {
									imageWasPresent = true;
								}
									// If it is a CJK chunk or Unicode TTF we will have to simulate the
									// space adjustment.
								else if (isJustified && numberOfSpaces > 0 && chunk.isSpecialEncoding()) {
									string s = chunk.ToString();
									int idx = s.IndexOf(' ');
									if (idx < 0)
										text.showText(chunk.ToString());
									else {
										float spaceCorrection = - ratio * lastBaseFactor * 1000f / chunk.Font.Size;
										PdfTextArray textArray = new PdfTextArray(s.Substring(0, idx));
										int lastIdx = idx;
										while ((idx = s.IndexOf(' ', lastIdx + 1)) >= 0) {
											textArray.Add(spaceCorrection);
											textArray.Add(s.Substring(lastIdx, idx - lastIdx));
											lastIdx = idx;
										}
										textArray.Add(spaceCorrection);
										textArray.Add(s.Substring(lastIdx));
										text.showText(textArray);
									}
								}
								else
									text.showText(chunk.ToString());
            
								if (rise != 0)
									text.TextRise = 0;
								if (color != null)
									text.resetRGBColorFill();
				*/            
				if (chunkStrokeIdx <= lastChunkStroke) {
					bool isStroked = (chunk.isAttribute(Chunk.STRIKETHRU) || chunk.isAttribute(Chunk.UNDERLINE));
					float width;
					if (isJustified) {
						width = chunk.getWidthCorrected(lastBaseFactor, ratio * lastBaseFactor);
					}
					else
						width = chunk.Width;
					if (chunk.isStroked()) {
						PdfChunk nextChunk = line.getChunk(chunkStrokeIdx + 1);
						if (isStroked) {
							graphics.LineWidth = chunk.Font.Size / 15;
							if (color != null)
								graphics.ColorStroke = color;
						}
						float shift = chunk.Font.Size / 3;
						if (chunk.isAttribute(Chunk.STRIKETHRU)) {
							float subtract = lastBaseFactor;
							if (nextChunk != null && nextChunk.isAttribute(Chunk.STRIKETHRU))
								subtract = 0;
							if (nextChunk == null)
								subtract += hangingCorrection;
							graphics.moveTo(xMarker, yMarker + shift);
							graphics.lineTo(xMarker + width - subtract, yMarker + shift);
							graphics.stroke();
						}
						if (chunk.isAttribute(Chunk.UNDERLINE)) {
							float subtract = lastBaseFactor;
							if (nextChunk != null && nextChunk.isAttribute(Chunk.UNDERLINE))
								subtract = 0;
							if (nextChunk == null)
								subtract += hangingCorrection;
							graphics.moveTo(xMarker, yMarker - shift);
							graphics.lineTo(xMarker + width - subtract, yMarker - shift);
							graphics.stroke();
						}
						if (chunk.isAttribute(Chunk.ACTION)) {
							float subtract = lastBaseFactor;
							if (nextChunk != null && nextChunk.isAttribute(Chunk.ACTION))
								subtract = 0;
							if (nextChunk == null)
								subtract += hangingCorrection;
							annotations.Add(new PdfAnnotation(writer, xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size, (PdfAction)chunk.getAttribute(Chunk.ACTION)));
						}
						if (chunk.isAttribute(Chunk.REMOTEGOTO)) {
							float subtract = lastBaseFactor;
							if (nextChunk != null && nextChunk.isAttribute(Chunk.REMOTEGOTO))
								subtract = 0;
							if (nextChunk == null)
								subtract += hangingCorrection;
							Object[] obj = (Object[])chunk.getAttribute(Chunk.REMOTEGOTO);
							string filename = (string)obj[0];
							if (obj[1] is string)
								remoteGoto(filename, (string)obj[1], xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size);
							else
								remoteGoto(filename, (int)obj[1], xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size);
						}
						if (chunk.isAttribute(Chunk.LOCALGOTO)) {
							float subtract = lastBaseFactor;
							if (nextChunk != null && nextChunk.isAttribute(Chunk.LOCALGOTO))
								subtract = 0;
							if (nextChunk == null)
								subtract += hangingCorrection;
							localGoto((string)chunk.getAttribute(Chunk.LOCALGOTO), xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size);
						}
						if (chunk.isAttribute(Chunk.LOCALDESTINATION)) {
							float subtract = lastBaseFactor;
							if (nextChunk != null && nextChunk.isAttribute(Chunk.LOCALDESTINATION))
								subtract = 0;
							if (nextChunk == null)
								subtract += hangingCorrection;
							localDestination((string)chunk.getAttribute(Chunk.LOCALDESTINATION), new PdfDestination(PdfDestination.XYZ, xMarker, yMarker + chunk.Font.Size, 0));
						}
						if (chunk.isAttribute(Chunk.GENERICTAG)) {
							float subtract = lastBaseFactor;
							if (nextChunk != null && nextChunk.isAttribute(Chunk.GENERICTAG))
								subtract = 0;
							if (nextChunk == null)
								subtract += hangingCorrection;
							Rectangle rect = new Rectangle(xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size);
							IPdfPageEvent pev = writer.PageEvent;
							if (pev != null)
								pev.onGenericTag(writer, this, rect, (string)chunk.getAttribute(Chunk.GENERICTAG));
						}
						if (chunk.isAttribute(Chunk.BACKGROUND)) {
							float subtract = lastBaseFactor;
							if (nextChunk != null && nextChunk.isAttribute(Chunk.BACKGROUND))
								subtract = 0;
							if (nextChunk == null)
								subtract += hangingCorrection;
							float fontSize = chunk.Font.Size;
							float ascender = chunk.Font.Font.getFontDescriptor(BaseFont.ASCENT, fontSize);
							float descender = chunk.Font.Font.getFontDescriptor(BaseFont.DESCENT, fontSize);
							graphics.ColorFill = (Color)chunk.getAttribute(Chunk.BACKGROUND);
							graphics.rectangle(xMarker, yMarker + descender, width - subtract, ascender - descender);
							graphics.fill();
							graphics.GrayFill = 0;
						}
						if (chunk.isAttribute(Chunk.PDFANNOTATION)) {
							float subtract = lastBaseFactor;
							if (nextChunk != null && nextChunk.isAttribute(Chunk.PDFANNOTATION))
								subtract = 0;
							if (nextChunk == null)
								subtract += hangingCorrection;
							float fontSize = chunk.Font.Size;
							float ascender = chunk.Font.Font.getFontDescriptor(BaseFont.ASCENT, fontSize);
							float descender = chunk.Font.Font.getFontDescriptor(BaseFont.DESCENT, fontSize);
							PdfAnnotation annot = PdfFormField.shallowDuplicate((PdfAnnotation)chunk.getAttribute(Chunk.PDFANNOTATION));
							annot.put(PdfName.RECT, new PdfRectangle(xMarker, yMarker + descender, xMarker + width - subtract, yMarker + ascender));
							addAnnotation(annot);
						}

					
						if (chunk.isAttribute(Chunk.SKEW)) {
							float[] param = (float[])chunk.getAttribute(Chunk.SKEW);
							text.setTextMatrix(1, param[0], param[1], 1, xMarker, yMarker);
						}
						if (chunk.isImage()) {
							Image image = chunk.Image;
							float[] matrix = image.Matrix;
							matrix[Image.CX] = xMarker + chunk.ImageOffsetX - matrix[Image.CX];
							matrix[Image.CY] = yMarker + chunk.ImageOffsetY - matrix[Image.CY];
							addImage(graphics, image, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
							text.setTextMatrix(xMarker + lastBaseFactor + image.ScaledWidth, yMarker);
						}
						if (isStroked) {
							graphics.LineWidth = chunk.Font.Size / 15;
							if (color != null)
								graphics.resetRGBColorStroke();
						}
					}
					xMarker += width;
					++chunkStrokeIdx;
				}

				if (chunk.Font.CompareTo(currentFont) != 0) {
					currentFont = chunk.Font;
					text.setFontAndSize(currentFont.Font, currentFont.Size);
				}
            
				rise = 0;
            
				object[] textRender = (Object[])chunk.getAttribute(Chunk.TEXTRENDERMODE);
				int tr = 0;
				float strokeWidth = 1;
				Color strokeColor = null;
				fr = chunk.getAttribute(Chunk.SUBSUPSCRIPT);
            
				if (textRender != null) {
					tr = ((int)textRender[0]) & 3;
					if (tr != PdfContentByte.TEXT_RENDER_MODE_FILL)
						text.TextRenderingMode = tr;
					if (tr == PdfContentByte.TEXT_RENDER_MODE_STROKE || tr == PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE) {
						strokeWidth = ((float)textRender[1]);
						if (strokeWidth != 1)
							text.LineWidth = strokeWidth;
						strokeColor = (Color)textRender[2];
						if (strokeColor == null)
							strokeColor = color;
						if (strokeColor != null)
							text.ColorStroke = strokeColor;
					}
				}
				if (fr != null)
					rise = (float)fr;
				if (color != null)
					text.ColorFill = color;
				if (rise != 0)
					text.TextRise = rise;
				if (chunk.isImage()) {
					adjustMatrix = true;
				}
					// If it is a CJK chunk or Unicode TTF we will have to simulate the
					// space adjustment.
				else if (isJustified && numberOfSpaces > 0 && chunk.isSpecialEncoding()) {
					String s = chunk.ToString();
					int idx = s.IndexOf(' ');
					if (idx < 0)
						text.showText(chunk.ToString());
					else {
						float spaceCorrection = - ratio * lastBaseFactor * 1000f / chunk.Font.Size;
						PdfTextArray textArray = new PdfTextArray(s.Substring(0, idx));
						int lastIdx = idx;
						while ((idx = s.IndexOf(' ', lastIdx + 1)) >= 0) {
							textArray.Add(spaceCorrection);
							textArray.Add(s.Substring(lastIdx, idx));
							lastIdx = idx;
						}
						textArray.Add(spaceCorrection);
						textArray.Add(s.Substring(lastIdx));
						text.showText(textArray);
					}
				}
				else
					text.showText(chunk.ToString());
            
				if (rise != 0)
					text.TextRise = 0;
				if (color != null)
					text.resetRGBColorFill();
				if (tr != PdfContentByte.TEXT_RENDER_MODE_FILL)
					text.TextRenderingMode = PdfContentByte.TEXT_RENDER_MODE_FILL;
				if (strokeColor != null)
					text.resetRGBColorStroke();
				if (strokeWidth != 1)
					text.LineWidth = 1;            
				if (chunk.isAttribute(Chunk.SKEW)) {
					adjustMatrix = true;
					text.setTextMatrix(xMarker, yMarker);
				}
			}
			if (isJustified) {
				text.WordSpacing = 0;
				text.CharacterSpacing = 0;
				if (line.isNewlineSplit())
					lastBaseFactor = 0;
			}
			if (imageWasPresent)
				text.setTextMatrix(baseXMarker, yMarker);
                
			if (adjustMatrix)
				text.moveText(baseXMarker - text.XTLM, 0);	
				

				
			currentValues[0] = currentFont;
			currentValues[1] = lastBaseFactor;
		}
    
		/**
		 * Implements a link to other part of the document. The jump will
		 * be made to a local destination with the same name, that must exist.
		 * @param name the name for this link
		 * @param llx the lower left x corner of the activation area
		 * @param lly the lower left y corner of the activation area
		 * @param urx the upper right x corner of the activation area
		 * @param ury the upper right y corner of the activation area
		 */
		internal void localGoto(string name, float llx, float lly, float urx, float ury) {
			PdfAction action = getLocalGotoAction(name);
			annotations.Add(new PdfAnnotation(writer, llx, lly, urx, ury, action));
		}
    
		internal PdfAction getLocalGotoAction(string name) {
			PdfAction action;
			Object[] obj = (Object[])localDestinations[name];
			if (obj == null)
				obj = new Object[3];
			if (obj[0] == null) {
				if (obj[1] == null) {
					obj[1] = writer.PdfIndirectReference;
				}
				action = new PdfAction((PdfIndirectReference)obj[1]);
				obj[0] = action;
				localDestinations.Add(name, obj);
			}
			else {
				action = (PdfAction)obj[0];
			}
			return action;
		}
    
		/**
		 * The local destination to where a local goto with the same
		 * name will jump.
		 * @param name the name of this local destination
		 * @param destination the <CODE>PdfDestination</CODE> with the jump coordinates
		 * @return <CODE>true</CODE> if the local destination was added,
		 * <CODE>false</CODE> if a local destination with the same name
		 * already existed
		 */
		internal bool localDestination(string name, PdfDestination destination) {
			Object[] obj = (Object[])localDestinations[name];
			if (obj == null)
				obj = new Object[3];
			if (obj[2] != null)
				return false;
			obj[2] = destination;
			localDestinations.Add(name, obj);
			destination.addPage(writer.CurrentPage);
			return true;
		}
    
		/**
		 * Implements a link to another document.
		 * @param filename the filename for the remote document
		 * @param name the name to jump to
		 * @param llx the lower left x corner of the activation area
		 * @param lly the lower left y corner of the activation area
		 * @param urx the upper right x corner of the activation area
		 * @param ury the upper right y corner of the activation area
		 */
		internal void remoteGoto(string filename, string name, float llx, float lly, float urx, float ury) {
			annotations.Add(new PdfAnnotation(writer, llx, lly, urx, ury, new PdfAction(filename, name)));
		}
    
		/**
		 * Implements a link to another document.
		 * @param filename the filename for the remote document
		 * @param page the page to jump to
		 * @param llx the lower left x corner of the activation area
		 * @param lly the lower left y corner of the activation area
		 * @param urx the upper right x corner of the activation area
		 * @param ury the upper right y corner of the activation area
		 */
		internal void remoteGoto(string filename, int page, float llx, float lly, float urx, float ury) {
			annotations.Add(new PdfAnnotation(writer, llx, lly, urx, ury, new PdfAction(filename, page)));
		}
    
		/** Sets the viewer preferences as the sum of several constants.
		 * @param preferences the viewer preferences
		 * @see PdfWriter#setViewerPreferences
		 */
    
		public int ViewerPreferences {
			set {
				viewerPreferences |= value;
			}
		}
    
		/** Implements an action in an area.
		 * @param action the <CODE>PdfAction</CODE>
		 * @param llx the lower left x corner of the activation area
		 * @param lly the lower left y corner of the activation area
		 * @param urx the upper right x corner of the activation area
		 * @param ury the upper right y corner of the activation area
		 */
		internal void setAction(PdfAction action, float llx, float lly, float urx, float ury) {
			annotations.Add(new PdfAnnotation(writer, llx, lly, urx, ury, action));
		}
    
		internal void setOpenAction(string name) {
			openActionName = name;
			openActionAction = null;
		}
    
		internal void setOpenAction(PdfAction action) {
			openActionAction = action;
			openActionName = null;
		}
    
		internal void addAdditionalAction(PdfName actionType, PdfAction action)  {
			if (additionalActions == null)  {
				additionalActions = new PdfDictionary();
			}
			additionalActions.put(actionType, action);
		}
    
		internal PdfPageLabels PageLabels {
			set {
				this.pageLabels = value;
			}
		}
    
		internal void addJavaScript(PdfAction js) {
			if (js.get(PdfName.JS) == null)
				throw new Exception("Only JavaScript actions are allowed.");
			try {
				documentJavaScript.Add(writer.addToBody(js).IndirectReference);
			}
			catch (IOException e) {
				throw e;
			}
		}
    
		internal Rectangle CropBoxSize {
			set {
				cropSize = new Rectangle(value);
			}
		}
    
		internal void addCalculationOrder(PdfFormField formField) {
			acroForm.addCalculationOrder(formField);
		}
    
		internal int SigFlags {
			set {
				acroForm.SigFlags = value;
			}
		}
    
		internal void addFormFieldRaw(PdfFormField field) {
			annotations.Add(field);
			ArrayList kids = field.getKids();
			if (kids != null) {
				for (int k = 0; k < kids.Count; ++k)
					addFormFieldRaw((PdfFormField)kids[k]);
			}
		}
    
		internal void addAnnotation(PdfAnnotation annot) {
			if (annot.isForm()) {
				PdfFormField field = (PdfFormField)annot;
				if (field.Parent == null)
					addFormFieldRaw(field);
			}
			else
				annotations.Add(annot);
		}
    
		/**
		 * Sets the display duration for the page (for presentations)
		 * @param seconds   the number of seconds to display the page
		 */
		internal int Duration {
			set {
				if (value > 0)
					this.duration = value;
				else
					this.duration = -1;
			}
		}
    
		/**
		 * Sets the transition for the page
		 * @param transition   the PdfTransition object
		 */
		internal PdfTransition Transition {
			set {
				this.transition = value;
			}
		}
	}
}