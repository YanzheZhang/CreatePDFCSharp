using System;
using System.Collections;
using System.util;

using iTextSharp.text.markup;

/*
 * $Id: Paragraph.cs,v 1.2 2003/03/20 03:27:51 geraldhenson Exp $
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
	/// A Paragraph is a series of Chunks and/or Phrases.
	/// </summary>
	/// <remarks>
	/// A Paragraph has the same qualities of a Phrase, but also
	/// some additional layout-parameters:
	/// <UL>
	/// <LI/>the indentation
	/// <LI/>the alignment of the text
	/// </UL>
	/// </remarks>
	/// <example>
	/// <code>
	/// <strong>Paragraph p = new Paragraph("This is a paragraph",
	///				FontFactory.getFont(FontFactory.HELVETICA, 18, Font.BOLDITALIC, new Color(0, 0, 255)));</strong>
	///	</code>
	/// </example>
	/// <seealso cref="T:iTextSharp.text.Element"/>
	/// <seealso cref="T:iTextSharp.text.Phrase"/>
	/// <seealso cref="T:iTextSharp.text.ListItem"/>
	public class Paragraph : Phrase, ITextElementArray, IMarkupAttributes {
    
		// membervariables
    
		///<summary> The alignment of the text. </summary>
		protected int alignment = Element.ALIGN_UNDEFINED;
    
		///<summary> The indentation of this paragraph on the left side. </summary>
		protected float indentationLeft;
    
		///<summary> The indentation of this paragraph on the right side. </summary>
		protected float indentationRight;
    
		///<summary> Does the paragraph has to be kept together on 1 page. </summary>
		protected bool keeptogether = false;
    
		// constructors
    
		/// <summary>
		/// Constructs a Paragraph.
		/// </summary>
		public Paragraph() : base() {}
    
		/// <summary>
		/// Constructs a Paragraph with a certain leading.
		/// </summary>
		/// <param name="leading">the leading</param>
		public Paragraph(float leading) : base(leading) {}
    
		/// <summary>
		/// Constructs a Paragraph with a certain Chunk.
		/// </summary>
		/// <param name="chunk">a Chunk</param>
		public Paragraph(Chunk chunk) : base(chunk) {}
    
		/// <summary>
		/// Constructs a Paragraph with a certain Chunk
		/// and a certain leading.
		/// </summary>
		/// <param name="leading">the leading</param>
		/// <param name="chunk">a Chunk</param>
		public Paragraph(float leading, Chunk chunk) : base(leading, chunk) {}
    
		/// <summary>
		/// Constructs a Paragraph with a certain string.
		/// </summary>
		/// <param name="str">a string</param>
		public Paragraph(string str) : base(str) {}
    
		/// <summary>
		/// Constructs a Paragraph with a certain string
		/// and a certain Font.
		/// </summary>
		/// <param name="str">a string</param>
		/// <param name="font">a Font</param>
		public Paragraph(string str, Font font) : base(str, font) {}
    
		/// <summary>
		/// Constructs a Paragraph with a certain string
		/// and a certain leading.
		/// </summary>
		/// <param name="leading">the leading</param>
		/// <param name="str">a string</param>
		public Paragraph(float leading, string str) : base(leading, str) {}
    
		/// <summary>
		/// Constructs a Paragraph with a certain leading, string
		/// and Font.
		/// </summary>
		/// <param name="leading">the leading</param>
		/// <param name="str">a string</param>
		/// <param name="font">a Font</param>
		public Paragraph(float leading, string str, Font font) : base(leading, str, font) {}
    
		/// <summary>
		/// Constructs a Paragraph with a certain Phrase.
		/// </summary>
		/// <param name="phrase">a Phrase</param>
		public Paragraph(Phrase phrase) : base(phrase.Leading, "", phrase.Font) {
			base.Add(phrase);
		}
    
		/// <summary>
		/// Returns a Paragraph that has been constructed taking in account
		/// the value of some attributes.
		/// </summary>
		/// <param name="attributes">Some attributes</param>
		public Paragraph(Properties attributes) : this("", FontFactory.getFont(attributes)) {
			string value;
			if ((value = attributes.Remove(ElementTags.ITEXT)) != null) {
				Chunk chunk = new Chunk(value);
				if ((value = attributes.Remove(ElementTags.GENERICTAG)) != null) {
					chunk.setGenericTag(value);
				}
				Add(chunk);
			}
			if ((value = attributes.Remove(ElementTags.ALIGN)) != null) {
				setAlignment(value);
			}
			if ((value = attributes.Remove(ElementTags.LEADING)) != null) {
				this.Leading = float.Parse(value);
			}
			else if ((value = attributes.Remove(MarkupTags.CSS_LINEHEIGHT)) != null) {
				this.Leading = MarkupParser.parseLength(value);
			}
			else {
				this.Leading = 16;
			}
			if ((value = attributes.Remove(ElementTags.INDENTATIONLEFT)) != null) {
				this.IndentationLeft = float.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.INDENTATIONRIGHT)) != null) {
				IndentationRight = float.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.KEEPTOGETHER)) != null) {
				keeptogether = bool.Parse(value);
			}
			if (attributes.Count > 0) MarkupAttributes = attributes;
		}
    
		// implementation of the Element-methods
    
		/// <summary>
		/// Gets the type of the text element.
		/// </summary>
		/// <value>a type</value>
		public override int Type {
			get {
				return Element.PARAGRAPH;
			}
		}
    
		// methods
    
		/// <summary>
		/// Adds an Object to the Paragraph.
		/// </summary>
		/// <param name="o">the object to add</param>
		/// <returns>a boolean</returns>
		public override bool Add(Object o) {
			if (o is List) {
				List list = (List) o;
				list.IndentationLeft = list.IndentationLeft + indentationLeft;
				list.IndentationRight = indentationRight;
				base.Add(list);
				return true;
			}
			else if (o is Image) {
				base.addSpecial((Image) o);
				return true;
			}
			else if (o is Paragraph) {
				base.Add(o);
				base.Add(Chunk.NEWLINE);
				return true;
			}
			base.Add(o);
			return true;
		}
    
		// setting the membervariables
    
		/// <summary>
		/// Sets the alignment of this paragraph.
		/// </summary>
		/// <param name="alignment">the new alignment as a string</param>
		public void setAlignment(string alignment) {
			alignment = alignment.ToLower();
			if (ElementTags.ALIGN_CENTER.ToLower().Equals(alignment)) {
				this.alignment = Element.ALIGN_CENTER;
				return;
			}
			if (ElementTags.ALIGN_RIGHT.ToLower().Equals(alignment)) {
				this.alignment = Element.ALIGN_RIGHT;
				return;
			}
			if (ElementTags.ALIGN_JUSTIFIED.ToLower().Equals(alignment)) {
				this.alignment = Element.ALIGN_JUSTIFIED;
				return;
			}
			this.alignment = Element.ALIGN_LEFT;
		}
    
		/// <summary>
		/// Set/get if this paragraph has to be kept together on one page.
		/// </summary>
		/// <value>a boolean</value>
		public bool KeepTogether {
			get {
				return keeptogether;
			}

			set {
				this.keeptogether = value;
			}
		}
    
		// methods to retrieve information
    
		/// <summary>
		/// Get/set the alignment of this paragraph.
		/// </summary>
		/// <value>a integer</value>
		public int Alignment{
			get {
				return alignment;
			}

			set {
				this.alignment = value;
			}
		}
    
		/// <summary>
		/// Get/set the indentation of this paragraph on the left side.
		/// </summary>
		/// <value>a float</value>
		public float IndentationLeft {
			get {
				return indentationLeft;
			}

			set {
				this.indentationLeft = value;
			}
		}
    
		/// <summary>
		/// Get/set the indentation of this paragraph on the right side.
		/// </summary>
		/// <value>a float</value>
		public float IndentationRight {
			get {
				return indentationRight;
			}
            
			set {
				this.indentationRight = value;
			}
		}
    
		/// <summary>
		/// Checks if a given tag corresponds with this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public new static bool isTag(string tag) {
			return ElementTags.PARAGRAPH.Equals(tag);
		}
	}
}
