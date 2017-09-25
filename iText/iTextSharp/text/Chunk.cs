using System;
using System.Drawing;
using System.Text;
using System.Collections;
using System.util;

using iTextSharp.text.pdf;
using iTextSharp.text.markup;

/*
 * $Id: Chunk.cs,v 1.4 2003/08/22 16:17:47 geraldhenson Exp $
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
	/// This is the smallest significant part of text that can be added to a document.
	/// </summary>
	/// <remarks>
	/// Most elements can be divided in one or more Chunks.
	/// A chunk is a string with a certain Font.
	/// all other layoutparameters should be defined in the object to which
	/// this chunk of text is added.
	/// </remarks>
	/// <example>
	/// <code>
	/// <strong>Chunk chunk = new Chunk("Hello world", FontFactory.getFont(FontFactory.COURIER, 20, Font.ITALIC, new Color(255, 0, 0)));</strong>
	/// document.Add(chunk);
	/// </code>
	/// </example>
	public class Chunk : IElement, IMarkupAttributes {

		// public static membervariables
    
		public static string OBJECT_REPLACEMENT_CHARACTER = "\ufffc";

		///<summary> This is a Chunk containing a newline. </summary>
		public static Chunk NEWLINE = new Chunk("\n");

		///<summary> Key for sub/basescript. </summary>
		public static string SUBSUPSCRIPT = "SUBSUPSCRIPT";

		///<summary> Key for underline. </summary>
		public static string UNDERLINE = "UNDERLINE";

		///<summary> Key for strikethru. </summary>
		public static string STRIKETHRU = "STRIKETHRU";

		///<summary> Key for color. </summary>
		public static string COLOR = "COLOR";

		///<summary> Key for encoding. </summary>
		public static string ENCODING = "ENCODING";

		///<summary> Key for remote goto. </summary>
		public static string REMOTEGOTO = "REMOTEGOTO";

		///<summary> Key for local goto. </summary>
		public static string LOCALGOTO = "LOCALGOTO";

		///<summary> Key for local destination. </summary>
		public static string LOCALDESTINATION = "LOCALDESTINATION";

		///<summary> Key for image. </summary>
		public static string IMAGE = "IMAGE";

		///<summary> Key for generic tag. </summary>
		public static string GENERICTAG = "GENERICTAG";

		///<summary> Key for newpage. </summary>
		public static string NEWPAGE = "NEWPAGE";

		///<summary> Key for split character. </summary>
		public static string SPLITCHARACTER = "SPLITCHARACTER";

		///<summary> Key for Action. </summary>
		public static string ACTION = "ACTION";

		///<summary> Key for background. </summary>
		public static string BACKGROUND = "BACKGROUND";

		///<summary> Key for annotation. </summary>
		public static string PDFANNOTATION = "PDFANNOTATION";

		///<summary> Key for hyphenation. </summary>
		public static string HYPHENATION = "HYPHENATION";

		///<summary> Key for text skewing. </summary>
		public static string SKEW = "SKEW";

		///<summary> Key for text rendering mode.</summary>
		public static string TEXTRENDERMODE = "TEXTRENDERMODE";



		// member variables

		///<summary> This is the content of this chunk of text. </summary>
		protected StringBuilder content = null;

		///<summary> This is the Font of this chunk of text. </summary>
		protected Font font = null;

		///<summary> Contains some of the attributes for this Chunk. </summary>
		protected Hashmap attributes = null;

		///<summary> Contains extra markupAttributes </summary>
		protected Properties markupAttributes = null;

		// constructors

		/// <summary>
		/// Empty constructor.
		/// </summary>
		/// <overloads>
		/// Has six overloads.
		/// </overloads>
		protected Chunk() {
		}

		/// <summary>
		/// Constructs a chunk of text with a certain content and a certain Font.
		/// </summary>
		/// <param name="content">the content</param>
		/// <param name="font">the font</param>
		public Chunk(string content, Font font) {
			this.content = new StringBuilder(content);
			this.font = font;
		}

		/// <summary>
		/// Constructs a chunk of text with a certain content, without specifying a Font.
		/// </summary>
		/// <param name="content">the content</param>
		public Chunk(string content) : this(content, new Font()) {}

		/// <summary>
		/// Constructs a chunk containing an Image.
		/// </summary>
		/// <param name="image">the image</param>
		/// <param name="offsetX">the image offset in the x direction</param>
		/// <param name="offsetY">the image offset in the y direction</param>
		public Chunk(Image image, float offsetX, float offsetY) : this(OBJECT_REPLACEMENT_CHARACTER, new Font()) {
			Image copyImage = Image.getInstance(image);
			copyImage.setAbsolutePosition(float.NaN, float.NaN);
			setAttribute(IMAGE, new Object[]{copyImage, offsetX, offsetY, false});
		}

		/// <summary>
		/// Constructs a chunk containing an Image.
		/// </summary>
		/// <param name="image">the image</param>
		/// <param name="offsetX">the image offset in the x direction</param>
		/// <param name="offsetY">the image offset in the y direction</param>
		/// <param name="changeLeading">true if the leading has to be adapted to the image</param>
		public Chunk(Image image, float offsetX, float offsetY, bool changeLeading) : this(OBJECT_REPLACEMENT_CHARACTER, new Font()) {
			setAttribute(IMAGE, new Object[]{image, offsetX, offsetY, changeLeading});
		}

		/// <summary>
		/// Returns a Chunk that has been constructed taking in account
		/// the value of some attributes.
		/// </summary>
		/// <param name="attributes">some attributes</param>
		public Chunk(Properties attributes) : this("", FontFactory.getFont(attributes)) {
			string value;
			if ((value = attributes.Remove(ElementTags.ITEXT)) != null) {
				append(value);
			}
			if ((value = attributes.Remove(ElementTags.LOCALGOTO)) != null) {
				setLocalGoto(value);
			}
			if ((value = attributes.Remove(ElementTags.REMOTEGOTO)) != null) {
				String destination = attributes.Remove(ElementTags.DESTINATION);
				String page = attributes.Remove(ElementTags.PAGE);
				if (page != null) {
					setRemoteGoto(value, int.Parse(page));
				}
				else if (destination != null) {
					setRemoteGoto(value, destination);
				}
			}
			if ((value = attributes.Remove(ElementTags.LOCALDESTINATION)) != null) {
				setLocalDestination(value);
			}
			if ((value = attributes.Remove(ElementTags.SUBSUPSCRIPT)) != null) {
				setTextRise(float.Parse(value));
			}
			if ((value = attributes.Remove(MarkupTags.CSS_VERTICALALIGN)) != null && value.EndsWith("%")) {
				float p = float.Parse(value.Substring(0, value.Length - 1)) / 100f;
				setTextRise(p * font.Size);
			}
			if ((value = attributes.Remove(ElementTags.GENERICTAG)) != null) {
				setGenericTag(value);
			}
			if ((value = attributes.Remove(ElementTags.BACKGROUNDCOLOR)) != null) {
				setBackground(MarkupParser.decodeColor(value));
			}
			if (attributes.Count > 0) this.MarkupAttributes = attributes;
		}

		// implementation of the Element-methods

		/// <summary>
		/// Processes the element by adding it (or the different parts) to an
		/// IElementListener.
		/// </summary>
		/// <param name="listener">an IElementListener</param>
		/// <returns>true if the element was processed successfully</returns>
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
		/// Gets the type of the text element.
		/// </summary>
		/// <value>a type</value>
		public int Type {
			get {
				return Element.CHUNK;
			}
		}

		/// <summary>
		/// Gets all the chunks in this element.
		/// </summary>
		/// <value>an ArrayList</value>
		public ArrayList Chunks {
			get {
				ArrayList tmp = new ArrayList();
				tmp.Add(this);
				return tmp;
			}
		}

		// methods

		/// <summary>
		/// appends some text to this Chunk.
		/// </summary>
		/// <param name="str">a string</param>
		/// <returns>a StringBuilder</returns>
		public StringBuilder append(string str) {
			return content.Append(str);
		}

		// methods to retrieve information

		/// <summary>
		/// Get/set the font of this Chunk.
		/// </summary>
		/// <value>a Font</value>
		public Font Font {
			get {
				return font;
			}

			set {
				this.font = value;
			}
		}


		/// <summary>
		/// Returns the content of this Chunk.
		/// </summary>
		/// <value>a string</value>
		public string Content {
			get {
				return content.ToString();
			}
		}

		/// <summary>
		/// Checks is this Chunk is empty.
		/// </summary>
		/// <returns>false if the Chunk contains other characters than space.</returns>
		public bool isEmpty() {
			return (content.ToString().Trim().Length == 0) && (content.ToString().IndexOf("\n") == -1) && (attributes == null);
		}

		/// <summary>
		/// Sets the text displacement relative to the baseline. Positive values rise the text,
		/// negative values lower the text.
		/// </summary>
		/// <remarks>
		/// It can be used to implement sub/basescript.
		/// </remarks>
		/// <param name="rise">the displacement in points</param>
		/// <returns>this Chunk</returns>
		public Chunk setTextRise(float rise) {
			return setAttribute(SUBSUPSCRIPT, rise);
		}

		/// <summary>
		/// Sets an action for this Chunk.
		/// </summary>
		/// <param name="action">the action</param>
		/// <returns>this Chunk</returns>
		public Chunk setAction(PdfAction action) {
			return setAttribute(ACTION, action);
		}

		/// <summary>
		/// Sets an anchor for this Chunk.
		/// </summary>
		/// <param name="url">the Uri to link to</param>
		/// <returns>this Chunk</returns>
		public Chunk setAnchor(Uri url) {
			return setAttribute(ACTION, new PdfAction(url));
		}

		/// <summary>
		/// Sets an anchor for this Chunk.
		/// </summary>
		/// <param name="url">the url to link to</param>
		/// <returns>this Chunk</returns>
		public Chunk setAnchor(string url) {
			return setAttribute(ACTION, new PdfAction(url));
		}

		/// <summary>
		/// Sets a local goto for this Chunk.
		/// </summary>
		/// <remarks>
		/// There must be a local destination matching the name.
		/// </remarks>
		/// <param name="name">the name of the destination to go to</param>
		/// <returns>this Chunk</returns>
		public Chunk setLocalGoto(string name) {
			return setAttribute(LOCALGOTO, name);
		}

		/// <summary>
		/// Sets the color of the background Chunk.
		/// </summary>
		/// <param name="color">the color of the background</param>
		/// <returns>this Chunk</returns>
		public Chunk setBackground(Color color) {
			return setAttribute(BACKGROUND, color);
		}

		/// <summary>
		/// Sets a generic annotation to this Chunk.
		/// </summary>
		/// <param name="annotation">the annotation</param>
		/// <returns>this Chunk</returns>
		public Chunk setAnnotation(PdfAnnotation annotation) {
			return setAttribute(PDFANNOTATION, annotation);
		}

		/// <summary>
		/// sets the hyphenation engine to this Chunk.
		/// </summary>
		/// <param name="hyphenation">the hyphenation engine</param>
		/// <returns>this Chunk</returns>
		public Chunk setHyphenation(IHyphenationEvent hyphenation) {
			return setAttribute(HYPHENATION, hyphenation);
		}

		/// <summary>
		/// Sets a goto for a remote destination for this Chunk.
		/// </summary>
		/// <param name="filename">the file name of the destination document</param>
		/// <param name="name">the name of the destination to go to</param>
		/// <returns>this Chunk</returns>
		public Chunk setRemoteGoto(string filename, string name) {
			return setAttribute(REMOTEGOTO, new Object[]{filename, name});
		}

		/// <summary>
		/// Sets a goto for a remote destination for this Chunk.
		/// </summary>
		/// <param name="filename">the file name of the destination document</param>
		/// <param name="page">the page of the destination to go to. First page is 1</param>
		/// <returns>this Chunk</returns>
		public Chunk setRemoteGoto(string filename, int page) {
			return setAttribute(REMOTEGOTO, new Object[]{filename, page});
		}

		/// <summary>
		/// Sets a local destination for this Chunk.
		/// </summary>
		/// <param name="name">the name for this destination</param>
		/// <returns>this Chunk</returns>
		public Chunk setLocalDestination(string name) {
			return setAttribute(LOCALDESTINATION, name);
		}

		/// <summary>
		/// Sets the generic tag Chunk.
		/// </summary>
		/// <remarks>
		/// The text for this tag can be retrieved with PdfPageEvent.
		/// </remarks>
		/// <param name="text">the text for the tag</param>
		/// <returns>this Chunk</returns>
		public Chunk setGenericTag(string text) {
			return setAttribute(GENERICTAG, text);
		}

		/// <summary>
		/// Sets the split characters.
		/// </summary>
		/// <param name="splitCharacter">the SplitCharacter interface</param>
		/// <returns>this Chunk</returns>
		public Chunk setSplitCharacter(ISplitCharacter splitCharacter) {
			return setAttribute(SPLITCHARACTER, splitCharacter);
		}

		/// <summary>
		/// Sets a new page tag..
		/// </summary>
		/// <returns>this Chunk</returns>
		public Chunk setNewPage() {
			return setAttribute(NEWPAGE, null);
		}

		/// <summary>
		/// Sets an arbitrary attribute.
		/// </summary>
		/// <param name="name">the key for the attribute</param>
		/// <param name="obj">the value of the attribute</param>
		/// <returns>this Chunk</returns>
		private Chunk setAttribute(string name, Object obj) {
			if (attributes == null)
				attributes = new Hashmap();
			attributes.Add(name, obj);
			return this;
		}

		/// <summary>
		/// Gets the attributes for this Chunk.
		/// </summary>
		/// <remarks>
		/// It may be null.
		/// </remarks>
		/// <value>a Hashmap</value>
		public Hashmap Attributes {
			get {
				return attributes;
			}
		}

		/// <summary>
		/// Checks the attributes of this Chunk.
		/// </summary>
		/// <returns>false if there aren't any.</returns>
		public bool hasAttributes() {
			return attributes != null;
		}

		/// <summary>
		/// Returns the image.
		/// </summary>
		/// <value>an Image</value>
		public Image Image {
			get {
				if (attributes == null) return null;
				Object[] obj = (Object[])attributes[Chunk.IMAGE];
				if (obj == null)
					return null;
				else {
					return (Image)obj[0];
				}
			}
		}

		/// <summary>
		/// Checks if a given tag corresponds with this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public static bool isTag(string tag) {
			return ElementTags.CHUNK.Equals(tag);
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
				return getKeySet(markupAttributes);
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
				this.markupAttributes = markupAttributes;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public static ICollection getKeySet(Hashtable table) {	
			return (table == null) ? new Hashtable().Keys  : table.Keys;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public static ICollection getKeySet(Properties table) {
			return (table == null) ? new Properties().Keys : table.Keys;
		}
	}
}
