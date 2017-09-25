using System;
using System.Collections;

/*
 * $Id: Document.cs,v 1.2 2003/03/12 20:10:11 geraldhenson Exp $
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
	/// A generic Document class.
	/// </summary>
	/// <remarks>
	/// All kinds of Text-elements can be added to a HTMLDocument.
	/// The Document signals all the listeners when an element
	/// has been added.<p/>
	/// <OL>
	///     <LI/>Once a document is created you can add some meta information.
	///     <LI/>You can also set the headers/footers.
	///     <LI/>You have to open the document before you can write content.
	///     <LI/>You can only write content (no more meta-formation!) once a document is opened.
	///     <LI/>When you change the header/footer on a certain page, this will be effective starting on the next page.
	///     <LI/>Ater closing the document, every listener (as well as its OutputStream) is closed too.
	/// </OL>
	/// </remarks>
	/// <example>
	/// <code>
	/// // creation of the document with a certain size and certain margins
	/// <strong>Document document = new Document(PageSize.A4, 50, 50, 50, 50);</strong>
	/// try {
	///     // creation of the different writers
	///     HtmlWriter.getInstance(<strong>document</strong>, System.out);
	///     PdfWriter.getInstance(<strong>document</strong>, new FileOutputStream("text.pdf"));
	///     // we add some meta information to the document
	///     <strong>document.addAuthor("Bruno Lowagie");
	///     document.addSubject("This is the result of a Test.");</strong>
	///     
	///     // we define a header and a footer
	///     HeaderFooter header = new HeaderFooter(new Phrase("This is a header."), false);
	///     HeaderFooter footer = new HeaderFooter(new Phrase("This is page "), new Phrase("."));
	///     footer.setAlignment(Element.ALIGN_CENTER);
	///     <strong>document.setHeader(header);
	///     document.setFooter(footer);</strong>
	///     // we open the document for writing
	///     <strong>document.open();
	///     document.Add(new Paragraph("Hello world"));</strong>
	/// }
	/// catch(DocumentException de) {
	///     Console.Error.WriteLine(de.Message);
	/// }
	/// <strong>document.Close();</strong>
	/// </code>
	/// </example>
	public class Document : IDocListener {

		// membervariables

		///<summary> This constant may only be changed by Paulo Soares and/or Bruno Lowagie. </summary>
		private static string ITEXT_VERSION = "iText# by Gerald Henson (r0.95 of lowagie.com, based on version Paulo build 103)";

		///<summary> Allows the pdf documents to be produced without compression for debugging purposes. </summary>
		public static bool compress = true;

		///<summary> The IDocListener. </summary>
		private ArrayList listeners = new ArrayList();

		///<summary> Is the document open or not? </summary>
		protected bool open;

		///<summary> Has the document allready been closed? </summary>
		protected bool close;

		// membervariables concerning the layout

		///<summary> The size of the page. </summary>
		protected Rectangle pageSize;

		///<summary> The watermark on the pages. </summary>
		protected Watermark watermark = null;

		///<summary> margin in x direction starting from the left </summary>
		protected float marginLeft = 0;

		///<summary> margin in x direction starting from the right </summary>
		protected float marginRight = 0;

		///<summary> margin in y direction starting from the top </summary>
		protected float marginTop = 0;

		///<summary> margin in y direction starting from the bottom </summary>
		protected float marginBottom = 0;

		///<summary> Content of JavaScript onLoad function </summary>
		protected string javaScript_onLoad = null;

		///<summary> Content of JavaScript onUnLoad function  </summary>
		protected string javaScript_onUnLoad = null;

		///<summary> Style class in HTML body tag </summary>
		protected string htmlStyleClass = null;

		// headers, footers

		///<summary> Current pagenumber </summary>
		protected int pageN = 0;

		///<summary> This is the textual part of a Page; it can contain a header </summary>
		protected HeaderFooter header = null;

		///<summary> This is the textual part of the footer </summary>
		protected HeaderFooter footer = null;

		// constructor

		/// <summary>
		/// Constructs a new Document-object.
		/// </summary>
		/// <overloads>
		/// Has three overloads.
		/// </overloads>
		public Document() : this(iTextSharp.text.PageSize.A4) {}

		/// <summary>
		/// Constructs a new Document-object.
		/// </summary>
		/// <param name="pageSize">the pageSize</param>
		public Document(Rectangle pageSize) : this(pageSize, 36, 36, 36, 36) {}

		/// <summary>
		/// Constructs a new Document-object.
		/// </summary>
		/// <param name="pageSize">the pageSize</param>
		/// <param name="marginLeft">the margin on the left</param>
		/// <param name="marginRight">the margin on the right</param>
		/// <param name="marginTop">the margin on the top</param>
		/// <param name="marginBottom">the margin on the bottom</param>
		public Document(Rectangle pageSize, float marginLeft, float marginRight, float marginTop, float marginBottom) {
			this.pageSize = pageSize;
			this.marginLeft = marginLeft;
			this.marginRight = marginRight;
			this.marginTop = marginTop;
			this.marginBottom = marginBottom;
		}

		// listener methods

		/// <summary>
		/// Adds a IDocListener to the Document.
		/// </summary>
		/// <param name="listener">the new IDocListener</param>
		public void addDocListener(IDocListener listener) {
			listeners.Add(listener);
		}

		/// <summary>
		/// Removes a IDocListener from the Document.
		/// </summary>
		/// <param name="listener">the IDocListener that has to be removed.</param>
		public void removeIDocListener(IDocListener listener) {
			listeners.Remove(listener);
		}

		// methods implementing the IDocListener interface

		/// <summary>
		/// Adds an Element to the Document.
		/// </summary>
		/// <param name="element">the Element to add</param>
		/// <returns>true if the element was added, false if not</returns>
		public virtual bool Add(IElement element) {
			if (close) {
				throw new DocumentException("The document has been closed. You can't add any Elements.");
			}
			int type = element.Type;
			if (open) {
				if (! (type == Element.CHUNK ||
					type == Element.PHRASE ||
					type == Element.PARAGRAPH ||
					type == Element.TABLE ||
					type == Element.PTABLE ||
					type == Element.ANCHOR ||
					type == Element.ANNOTATION ||
					type == Element.CHAPTER ||
					type == Element.SECTION ||
					type == Element.LIST ||
					type == Element.LISTITEM ||
					type == Element.RECTANGLE ||
					type == Element.PNG ||
					type == Element.JPEG ||
					type == Element.GIF ||
					type == Element.IMGRAW ||
					type == Element.IMGTEMPLATE ||
					type == Element.GRAPHIC)) {
					throw new DocumentException("The document is open; you can only add Elements with content.");
				}
			}
			else {
				if (! (type == Element.HEADER ||
					type == Element.TITLE ||
					type == Element.SUBJECT ||
					type == Element.KEYWORDS ||
					type == Element.AUTHOR ||
					type == Element.PRODUCER ||
					type == Element.CREATOR ||
					type == Element.CREATIONDATE)) {
					throw new DocumentException("The document is not open yet; you can only add Meta information.");
				}
			}
			bool success = false;
			foreach(IDocListener listener in listeners) {
				success |= listener.Add(element);
			}
			return success;
		}

		/// <summary>
		/// Opens the document.
		/// </summary>
		/// <remarks>
		/// Once the document is opened, you can't write any Header- or Meta-information
		/// anymore. You have to open the document before you can begin to add content
		/// to the body of the document.
		/// </remarks>
		public virtual void Open() {
			if (! close) {
				open = true;
			}
			foreach(IDocListener listener in listeners) {
				listener.setPageSize(pageSize);
				listener.setMargins(marginLeft, marginRight, marginTop, marginBottom);
				listener.Open();
			}
		}

		/// <summary>
		/// Sets the pagesize.
		/// </summary>
		/// <param name="pageSize">the new pagesize</param>
		/// <returns>a bool</returns>
		public virtual bool setPageSize(Rectangle pageSize) {
			this.pageSize = pageSize;
			foreach(IDocListener listener in listeners) {
				listener.setPageSize(pageSize);
			}
			return true;
		}

		/// <summary>
		/// Sets the Watermark.
		/// </summary>
		/// <param name="watermark">the watermark to add</param>
		/// <returns>true if the element was added, false if not.</returns>
		public virtual bool Add(Watermark watermark) {
			this.watermark = watermark;
			foreach(IDocListener listener in listeners) {
				listener.Add(watermark);
			}
			return true;
		}

		/// <summary>
		/// Removes the Watermark.
		/// </summary>
		public virtual void removeWatermark() {
			this.watermark = null;
			foreach(IDocListener listener in listeners) {
				listener.removeWatermark();
			}
		}

		/// <summary>
		/// Sets the margins.
		/// </summary>
		/// <param name="marginLeft">the margin on the left</param>
		/// <param name="marginRight">the margin on the right</param>
		/// <param name="marginTop">the margin on the top</param>
		/// <param name="marginBottom">the margin on the bottom</param>
		/// <returns></returns>
		public virtual bool setMargins(float marginLeft,float marginRight,float marginTop,float marginBottom) {
			this.marginLeft = marginLeft;
			this.marginRight = marginRight;
			this.marginTop = marginTop;
			this.marginBottom = marginBottom;
			foreach(IDocListener listener in listeners) {
				listener.setMargins(marginLeft, marginRight, marginTop, marginBottom);
			}
			return true;
		}

		/// <summary>
		/// Signals that an new page has to be started.
		/// </summary>
		/// <returns>true if the page was added, false if not.</returns>
		public virtual bool newPage() {
			if (!open || close) {
				return false;
			}
			foreach(IDocListener listener in listeners) {
				listener.newPage();
			}
			return true;
		}

		/// <summary>
		/// Changes the header of this document.
		/// </summary>
		/// <value>a HeaderFooter</value>
		public virtual HeaderFooter Header {
			set {
				this.header = value;
				foreach(IDocListener listener in listeners) {
					listener.Header = value;
				}
			}
		}

		/// <summary>
		/// Resets the header of this document.
		/// </summary>
		public virtual void resetHeader() {
			this.header = null;
			foreach(IDocListener listener in listeners) {
				listener.resetHeader();
			}
		}

		/// <summary>
		/// Changes the footer of this document.
		/// </summary>
		/// <value>a HeaderFooter</value>
		public virtual HeaderFooter Footer {
			set {
				this.footer = value;
				foreach(IDocListener listener in listeners) {
					listener.Footer = value;
				}
			}
		}

		/// <summary>
		/// Resets the footer of this document.
		/// </summary>
		public virtual void resetFooter() {
			this.footer = footer;
			foreach(IDocListener listener in listeners) {
				listener.resetFooter();
			}
		}

		/// <summary>
		/// Sets the page number to 0.
		/// </summary>
		public virtual void resetPageCount() {
			pageN = 0;
			foreach(IDocListener listener in listeners) {
				listener.resetPageCount();
			}
		}

		/// <summary>
		/// Sets the page number.
		/// </summary>
		/// <value>an int</value>
		public virtual int PageCount {
			set {
				this.pageN = value;
				foreach(IDocListener listener in listeners) {
					listener.PageCount = value;
				}
			}
		}

		/// <summary>
		/// Returns the current page number.
		/// </summary>
		/// <value>an int</value>
		public int PageNumber {
			get {
				return this.pageN;
			}
		}

		/// <summary>
		/// Closes the document.
		/// </summary>
		/// <remarks>
		/// Once all the content has been written in the body, you have to close
		/// the body. After that nothing can be written to the body anymore.
		/// </remarks>
		public virtual void Close() {
			if (! close) {
				open = false;
				close = true;
			}
			foreach(IDocListener listener in listeners) {
				listener.Close();
			}
		}

		// methods concerning the header or some meta information

		/// <summary>
		/// Adds a user defined header to the document.
		/// </summary>
		/// <param name="name">the name of the header</param>
		/// <param name="content">the content of the header</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool addHeader(string name, string content) {
			try {
				return Add(new Header(name, content));
			}
			catch(DocumentException de) {
				throw de;
			}
		}

		/// <summary>
		/// Adds the title to a Document.
		/// </summary>
		/// <param name="title">the title</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool addTitle(string title) {
			try {
				return Add(new Meta(Element.TITLE, title));
			}
			catch(DocumentException de) {
				throw de;
			}
		}

		/// <summary>
		/// Adds the subject to a Document.
		/// </summary>
		/// <param name="subject">the subject</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool addSubject(string subject) {
			try {
				return Add(new Meta(Element.SUBJECT, subject));
			}
			catch(DocumentException de) {
				throw de;
			}
		}

		/// <summary>
		/// Adds the keywords to a Document.
		/// </summary>
		/// <param name="keywords">keywords to add</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool addKeywords(string keywords) {
			try {
				return Add(new Meta(Element.KEYWORDS, keywords));
			}
			catch(DocumentException de) {
				throw de;
			}
		}

		/// <summary>
		/// Adds the author to a Document.
		/// </summary>
		/// <param name="author">the name of the author</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool addAuthor(string author) {
			try {
				return Add(new Meta(Element.AUTHOR, author));
			}
			catch(DocumentException de) {
				throw de;
			}
		}

		/// <summary>
		/// Adds the creator to a Document.
		/// </summary>
		/// <param name="creator">the name of the creator</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool addCreator(string creator) {
			try {
				return Add(new Meta(Element.CREATOR, creator));
			}
			catch(DocumentException de) {
				throw de;
			}
		}

		/// <summary>
		/// Adds the producer to a Document.
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		public bool addProducer() {
			try {
				return Add(new Meta(Element.PRODUCER, "iText# by lowagie.com"));
			}
			catch(DocumentException de) {
				throw de;
			}
		}

		/// <summary>
		/// Adds the current date and time to a Document.
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		public bool addCreationDate() {
			try {
				return Add(new Meta(Element.CREATIONDATE, DateTime.Now.ToString()));
			}
			catch(DocumentException de) {
				throw de;
			}
		}

		// methods to get the layout of the document.

		/// <summary>
		/// Returns the left margin.
		/// </summary>
		/// <value>the left margin</value>
		public float LeftMargin {
			get {
				return marginLeft;
			}
		}

		/// <summary>
		/// Return the right margin.
		/// </summary>
		/// <value>the right margin</value>
		public float RightMargin {
			get {
				return marginRight;
			}
		}

		/// <summary>
		/// Returns the top margin.
		/// </summary>
		/// <value>the top margin</value>
		public float TopMargin {
			get {
				return marginTop;
			}
		}

		/// <summary>
		/// Returns the bottom margin.
		/// </summary>
		/// <value>the bottom margin</value>
		public float BottomMargin {
			get {
				return marginBottom;
			}
		}

		/// <summary>
		/// Returns the lower left x-coordinate.
		/// </summary>
		/// <value>the lower left x-coordinate</value>
		public float Left {
			get {
				return pageSize.left(marginLeft);
			}
		}

		/// <summary>
		/// Returns the upper right x-coordinate.
		/// </summary>
		/// <value>the upper right x-coordinate.</value>
		public float Right {
			get {
				return pageSize.right(marginRight);
			}
		}

		/// <summary>
		/// Returns the upper right y-coordinate.
		/// </summary>
		/// <value>the upper right y-coordinate.</value>
		public float Top {
			get {
				return pageSize.top(marginTop);
			}
		}

		/// <summary>
		/// Returns the lower left y-coordinate.
		/// </summary>
		/// <value>the lower left y-coordinate.</value>
		public float Bottom {
			get {
				return pageSize.bottom(marginBottom);
			}
		}

		/// <summary>
		/// Returns the lower left x-coordinate considering a given margin.
		/// </summary>
		/// <param name="margin">a margin</param>
		/// <returns>the lower left x-coordinate</returns>
		public float left(float margin) {
			return pageSize.left(marginLeft + margin);
		}

		/// <summary>
		/// Returns the upper right x-coordinate, considering a given margin.
		/// </summary>
		/// <param name="margin">a margin</param>
		/// <returns>the upper right x-coordinate</returns>
		public float right(float margin) {
			return pageSize.right(marginRight + margin);
		}

		/// <summary>
		/// Returns the upper right y-coordinate, considering a given margin.
		/// </summary>
		/// <param name="margin">a margin</param>
		/// <returns>the upper right y-coordinate</returns>
		public float top(float margin) {
			return pageSize.top(marginTop + margin);
		}

		/// <summary>
		/// Returns the lower left y-coordinate, considering a given margin.
		/// </summary>
		/// <param name="margin">a margin</param>
		/// <returns>the lower left y-coordinate</returns>
		public float bottom(float margin) {
			return pageSize.bottom(marginBottom + margin);
		}

		/// <summary>
		/// Gets the pagesize.
		/// </summary>
		/// <value>the page size</value>
		public Rectangle PageSize {
			get {
				return this.pageSize;
			}
		}

		/// <summary>
		/// Checks if the document is open.
		/// </summary>
		/// <returns>true if the document is open</returns>
		public bool isOpen() {
			return open;
		}

		/// <summary>
		/// Gets the iText version.
		/// </summary>
		/// <value>iText version</value>
		public static string Version {
			get {
				return ITEXT_VERSION;
			}
		}

		/// <summary>
		/// Gets the JavaScript onLoad command.
		/// </summary>
		/// <value>the JavaScript onLoad command.</value>
		public string JavaScript_onLoad {
			get {
				return this.javaScript_onLoad;
			}

			set {
				this.javaScript_onLoad = value;
			}
		}

		/// <summary>
		/// Gets the JavaScript onUnLoad command.
		/// </summary>
		/// <value>the JavaScript onUnLoad command</value>
		public string JavaScript_onUnLoad {
			get {
				return this.javaScript_onUnLoad;
			}

			set {
				this.javaScript_onUnLoad = value;
			}
		}

		/// <summary>
		/// Gets the style class of the HTML body tag
		/// </summary>
		/// <value>the style class of the HTML body tag</value>
		public string HtmlStyleClass {
			get {
				return this.htmlStyleClass;
			}

			set {
				this.htmlStyleClass = value;
			}
		}
	}
}
