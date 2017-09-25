using System;
using System.Collections;
using System.util;

/*
 * $Id: Annotation.cs,v 1.2 2003/03/09 23:05:11 geraldhenson Exp $
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
	/// An Annotation is a little note that can be added to a page
	/// on a document.
	/// </summary>
	/// <seealso cref="T:iTextSharp.text.Element"/>
	/// <seealso cref="T:iTextSharp.text.Anchor"/>
	public class Annotation : IElement, IMarkupAttributes {
    
		// membervariables
    
		/// <summary>This is a possible annotation type.</summary>
		public const int TEXT = 0;    
		/// <summary>This is a possible annotation type.</summary>
		public const int URL_NET = 1;    
		/// <summary>This is a possible annotation type.</summary>
		public const int URL_AS_STRING = 2;    
		/// <summary>This is a possible annotation type.</summary>
		public const int FILE_DEST = 3;    
		/// <summary>This is a possible annotation type.</summary>
		public const int FILE_PAGE = 4;    
		/// <summary>This is a possible annotation type.</summary>
		public const int NAMED_DEST = 5;    
		/// <summary>This is a possible annotation type.</summary>
		public const int LAUNCH = 6;
    
		/// <summary>This is a possible attribute.</summary>
		public static string TITLE = "title";
		/// <summary>This is a possible attribute.</summary>
		public static string CONTENT = "content";
		/// <summary>This is a possible attribute.</summary>
		public static string URL = "url";
		/// <summary>This is a possible attribute.</summary>
		public static string FILE = "file";
		/// <summary>This is a possible attribute.</summary>
		public static string DESTINATION = "destination";
		/// <summary>This is a possible attribute.</summary>
		public static string PAGE = "page";
		/// <summary>This is a possible attribute.</summary>
		public static string NAMED = "named";
		/// <summary>This is a possible attribute.</summary>
		public static string APPLICATION = "application";
		/// <summary>This is a possible attribute.</summary>
		public static string PARAMETERS = "parameters";
		/// <summary>This is a possible attribute.</summary>
		public static string OPERATION = "operation";
		/// <summary>This is a possible attribute.</summary>
		public static string DEFAULTDIR = "defaultdir";
		/// <summary>This is a possible attribute.</summary>
		public static string LLX = "llx";
		/// <summary>This is a possible attribute.</summary>
		public static string LLY = "lly";
		/// <summary>This is a possible attribute.</summary>
		public static string URX = "urx";
		/// <summary>This is a possible attribute.</summary>
		public static string URY = "ury";
    
		/// <summary>This is the type of annotation.</summary>
		protected int annotationtype;
    
		/// <summary>This is the title of the Annotation.</summary>
		protected Hashmap annotationAttributes = new Hashmap();

		/// <summary>Contains extra markupAttributes</summary> 
		protected Properties markupAttributes = null;
    
		/// <summary>This is the lower left x-value</summary>
		protected float llx = float.NaN;
		/// <summary>This is the lower left y-value</summary>
		protected float lly = float.NaN;
		/// <summary>This is the upper right x-value</summary>
		protected float urx = float.NaN;
		/// <summary>This is the upper right y-value</summary>
		protected float ury = float.NaN;
    
		// constructors
    
		/// <summary>
		/// Constructs an Annotation with a certain title and some text.
		/// </summary>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		private Annotation(float llx, float lly, float urx, float ury) {
			this.llx = llx;
			this.lly = lly;
			this.urx = urx;
			this.ury = ury;
		}
    
		/// <summary>
		/// Constructs an Annotation with a certain title and some text.
		/// </summary>
		/// <param name="title">the title of the annotation</param>
		/// <param name="text">the content of the annotation</param>
		public Annotation(string title, string text) {
			annotationtype = TEXT;
			annotationAttributes.Add(TITLE, title);
			annotationAttributes.Add(CONTENT, text);
		}
    
		/// <summary>
		/// Constructs an Annotation with a certain title and some text.
		/// </summary>
		/// <param name="title">the title of the annotation</param>
		/// <param name="text">the content of the annotation</param>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		public Annotation(string title, string text, float llx, float lly, float urx, float ury) : this(llx, lly, urx, ury) {
			annotationtype = TEXT;
			annotationAttributes.Add(TITLE, title);
			annotationAttributes.Add(CONTENT, text);
		}
    
		/// <summary>
		/// Constructs an Annotation.
		/// </summary>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		/// <param name="url">the external reference</param>
		public Annotation(float llx, float lly, float urx, float ury, Uri url) : this(llx, lly, urx, ury) {
			annotationtype = URL_NET;
			annotationAttributes.Add(URL, url);
		}
    
		/// <summary>
		/// Constructs an Annotation.
		/// </summary>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		/// <param name="url">the external reference</param>
		public Annotation(float llx, float lly, float urx, float ury, string url) : this(llx, lly, urx, ury) {
			annotationtype = URL_AS_STRING;
			annotationAttributes.Add(FILE, url);
		}
    
		/// <summary>
		/// Constructs an Annotation.
		/// </summary>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		/// <param name="file">an external PDF file</param>
		/// <param name="dest">the destination in this file</param>
		public Annotation(float llx, float lly, float urx, float ury, string file, string dest) : this(llx, lly, urx, ury) {
			annotationtype = FILE_DEST;
			annotationAttributes.Add(FILE, file);
			annotationAttributes.Add(DESTINATION, dest);
		}
    
		/// <summary>
		/// Constructs an Annotation.
		/// </summary>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		/// <param name="file">an external PDF file</param>
		/// <param name="page">a page number in this file</param>
		public Annotation(float llx, float lly, float urx, float ury, string file, int page) : this(llx, lly, urx, ury) {
			annotationtype = FILE_PAGE;
			annotationAttributes.Add(FILE, file);
			annotationAttributes.Add(PAGE, page);
		}
    
		/// <summary>
		/// Constructs an Annotation.
		/// </summary>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		/// <param name="named">a named destination in this file</param>
		/// <overloads>
		/// Has nine overloads.
		/// </overloads>
		public Annotation(float llx, float lly, float urx, float ury, int named) : this(llx, lly, urx, ury) {
			annotationtype = NAMED_DEST;
			annotationAttributes.Add(NAMED, named);
		}
    
		/// <summary>
		/// Constructs an Annotation.
		/// </summary>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		/// <param name="application">an external application</param>
		/// <param name="parameters">parameters to pass to this application</param>
		/// <param name="operation">the operation to pass to this application</param>
		/// <param name="defaultdir">the default directory to run this application in</param>
		public Annotation(float llx, float lly, float urx, float ury, string application, string parameters, string operation, string defaultdir) : this(llx, lly, urx, ury) {
			annotationtype = LAUNCH;
			annotationAttributes.Add(APPLICATION, application);
			annotationAttributes.Add(PARAMETERS, parameters);
			annotationAttributes.Add(OPERATION, operation);
			annotationAttributes.Add(DEFAULTDIR, defaultdir);
		}
    
		/// <summary>
		/// Constructs an Annotation taking into account
		/// the value of some attributes
		/// </summary>
		/// <param name="attributes">some attributes</param>
		public Annotation(Properties attributes) {
			string value = attributes.Remove(ElementTags.LLX);
			if (value != null) {
				llx = float.Parse(value);
			}
			value = attributes.Remove(ElementTags.LLY);
			if (value != null) {
				lly = float.Parse(value);
			}
			value = attributes.Remove(ElementTags.URX);
			if (value != null) {
				urx = float.Parse(value);
			}
			value = attributes.Remove(ElementTags.URY);
			if (value != null) {
				ury = float.Parse(value);
			}
			string title = attributes.Remove(ElementTags.TITLE);
			string text = attributes.Remove(ElementTags.CONTENT);
			if (title != null || text != null) {
				annotationtype = TEXT;
			}
			else if ((value = attributes.Remove(ElementTags.URL)) != null) {
				annotationtype = URL_AS_STRING;
				annotationAttributes.Add(FILE, value);
			}
			else if ((value = attributes.Remove(ElementTags.NAMED)) != null) {
				annotationtype = NAMED_DEST;
				annotationAttributes.Add(NAMED, int.Parse(value));
			}
			else {
				string file = attributes.Remove(ElementTags.FILE);
				string destination = attributes.Remove(ElementTags.DESTINATION);
				string page = attributes.Remove(ElementTags.PAGE);
				if (file != null) {
					annotationAttributes.Add(FILE, file);
					if (destination != null) {
						annotationtype = FILE_DEST;
						annotationAttributes.Add(DESTINATION, destination);
					}
					else if (page != null) {
						annotationtype = FILE_PAGE;
						annotationAttributes.Add(FILE, file);
						annotationAttributes.Add(PAGE, int.Parse(page));
					}
				}
				else if ((value = attributes.Remove(ElementTags.NAMED)) != null) {
					annotationtype = LAUNCH;
					annotationAttributes.Add(APPLICATION, value);
					annotationAttributes.Add(PARAMETERS, attributes.Remove(ElementTags.PARAMETERS));
					annotationAttributes.Add(OPERATION, attributes.Remove(ElementTags.OPERATION));
					annotationAttributes.Add(DEFAULTDIR, attributes.Remove(ElementTags.DEFAULTDIR));
				}
			}
			if (annotationtype == TEXT) {
				if (title == null) title = "";
				if (text == null) text = "";
				annotationAttributes.Add(TITLE, title);
				annotationAttributes.Add(CONTENT, text);
			}
			if (attributes.Count > 0) this.MarkupAttributes = attributes;
		}
    
		// implementation of the Element-methods
    
		/// <summary>
		/// Gets the type of the text element
		/// </summary>
		public int Type {
			get {
				return Element.ANNOTATION;
			}
		}
    
		// methods
    
		/// <summary>
		/// Processes the element by adding it (or the different parts) to an
		/// IElementListener.
		/// </summary>
		/// <param name="listener">an IElementListener</param>
		/// <returns>true if the element was process successfully</returns>
		public bool process(IElementListener listener) {
			try {
				return listener.Add(this);
			}
			catch(DocumentException de) {
				de.GetType();
				return false;
			}
		}
    
		/// <summary>
		/// Gets all the chunks in this element.
		/// </summary>
		/// <value>an ArrayList</value>
		public ArrayList Chunks {
			get {
				return new ArrayList();
			}
		}
   
		// methods
    
		/// <summary>
		/// Sets the dimensions of this annotation.
		/// </summary>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		public void setDimensions (float llx, float lly, float urx, float ury) {
			this.llx = llx;
			this.lly = lly;
			this.urx = urx;
			this.ury = ury;
		}
    
		// methods to retrieve information
    
		/// <summary>
		/// Returns the lower left x-value.
		/// </summary>
		/// <returns>a value</returns>
		public float Llx() {
			return llx;
		}
    
		/// <summary>
		/// Returns the lower left y-value.
		/// </summary>
		/// <returns>a value</returns>
		public float Lly() {
			return lly;
		}
    
		/// <summary>
		/// Returns the uppper right x-value.
		/// </summary>
		/// <returns>a value</returns>
		public float Urx() {
			return urx;
		}
    
		/// <summary>
		/// Returns the uppper right y-value.
		/// </summary>
		/// <returns>a value</returns>
		public float Ury() {
			return ury;
		}
    
		/// <summary>
		/// Returns the lower left x-value.
		/// </summary>
		/// <param name="def">the default value</param>
		/// <returns>a value</returns>
		public float Llx(float def) {
			if (llx == float.NaN) return def;
			return llx;
		}
    
		/// <summary>
		/// Returns the lower left y-value.
		/// </summary>
		/// <param name="def">the default value</param>
		/// <returns>a value</returns>
		public float Lly(float def) {
			if (lly == float.NaN) return def;
			return lly;
		}
    
		/// <summary>
		/// Returns the upper right x-value.
		/// </summary>
		/// <param name="def">the default value</param>
		/// <returns>a value</returns>
		public float Urx(float def) {
			if (urx == float.NaN) return def;
			return urx;
		}
    
		/// <summary>
		/// Returns the upper right y-value.
		/// </summary>
		/// <param name="def">the default value</param>
		/// <returns>a value</returns>
		public float Ury(float def) {
			if (ury == float.NaN) return def;
			return ury;
		}
    
		/// <summary>
		/// Returns the type of this Annotation.
		/// </summary>
		/// <value>a type</value>
		public int AnnotationType {
			get {
				return annotationtype;
			}
		}
    
		/// <summary>
		/// Returns the title of this Annotation.
		/// </summary>
		/// <value>a name</value>
		public string Title {
			get {
				string s = (string)annotationAttributes[TITLE];
				if (s == null) s = "";
				return s;
			}
		}
    
		/// <summary>
		/// Gets the content of this Annotation.
		/// </summary>
		/// <value>a reference</value>
		public string Content {
			get {
				string s = (string)annotationAttributes[CONTENT];
				if (s == null) s = "";
				return s;
			}
		}
    
		/// <summary>
		/// Gets the content of this Annotation.
		/// </summary>
		/// <value>a reference</value>
		public Hashmap Attributes {
			get {
				return annotationAttributes;
			}
		}
    
		/// <summary>
		/// Checks if a given tag corresponds with this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresonds</returns>
		public static bool isTag(string tag) {
			return ElementTags.ANNOTATION.Equals(tag);
		}
    
		/// <summary>
		/// See <see cref="M:iTextSharp.text.IMarkupAttributes.setMarkupAttribute(System.String,System.String)"/>
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <param name="value">attribute value</param>
		public void setMarkupAttribute(string name, string value) {
			markupAttributes = (markupAttributes == null) ? new Properties() : markupAttributes;
			markupAttributes.Add(name, value);
		}
    
		/// <summary>
		/// See <see cref="M:iTextSharp.text.IMarkupAttributes.getMarkupAttribute(System.String)"/>
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <returns>attribute value</returns>
		public string getMarkupAttribute(string name) {
			return (markupAttributes == null) ? null : markupAttributes[name];
		}
    
		/// <summary>
		/// See <see cref="P:iTextSharp.text.IMarkupAttributes.MarkupAttributeNames"/>
		/// </summary>
		/// <value>a collection of string attribute names</value>
		public ICollection MarkupAttributeNames {
			get {
				return Chunk.getKeySet(markupAttributes);
			}
		}
    
		/// <summary>
		/// See <see cref="P:iTextSharp.text.IMarkupAttributes.MarkupAttributes"/>
		/// </summary>
		/// <value>a Properties-object containing all the markupAttributes.</value>
		public Properties MarkupAttributes {
			get {
				return markupAttributes;
			}

			set {
				this.markupAttributes = value;
			}
		}
	}
}
