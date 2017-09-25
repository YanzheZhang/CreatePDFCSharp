using System;
using System.Collections;
using System.util;

/*
 * $Id: List.cs,v 1.3 2003/03/19 23:42:10 geraldhenson Exp $
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
	/// A List contains several ListItems.
	/// </summary>
	/// <example>
	/// <B>Example 1:</B>
	/// <code>
	/// <strong>List list = new List(true, 20);
	/// list.Add(new ListItem("First line"));
	/// list.Add(new ListItem("The second line is longer to see what happens once the end of the line is reached. Will it start on a new line?"));
	/// list.Add(new ListItem("Third line"));</strong>
	/// </code>
	/// 
	/// The result of this code looks like this:
	/// <OL>
	/// 	<LI>
	/// 		First line
	/// 	</LI>
	/// 	<LI>
	/// 		The second line is longer to see what happens once the end of the line is reached. Will it start on a new line?
	/// 	</LI>
	/// 	<LI>
	/// 		Third line
	/// 	</LI>
	/// </OL>
	/// 
	/// <B>Example 2:</B>
	/// <code>
	/// <strong>List overview = new List(false, 10);
	/// overview.Add(new ListItem("This is an item"));
	/// overview.Add("This is another item");</strong>
	/// </code>
	/// 
	/// The result of this code looks like this:
	/// <UL>
	///		<LI>
	///			This is an item
	///		</LI>
	///		<LI>
	///			This is another item
	///		</LI>
	///	</UL>
	/// </example>
	/// <seealso cref="T:iTextSharp.text.Element"/>
	/// <seealso cref="T:iTextSharp.text.ListItem"/>
	public class List : ITextElementArray, IMarkupAttributes {
    
		// membervariables
    
		/// <summary> This is the ArrayList containing the different ListItems. </summary>
		private ArrayList list = new ArrayList();
    
		/// <summary> This variable indicates if the list has to be numbered. </summary>
		private bool numbered;
		private bool lettered;
    
		/// <summary> This variable indicates the first number of a numbered list. </summary>
		private int first = 1;
		private char firstCh = 'A';
		private char lastCh  = 'Z';
    
		/// <summary> This is the listsymbol of a list that is not numbered. </summary>
		private Chunk symbol = new Chunk("-");
    
		/// <summary> The indentation of this list on the left side. </summary>
		private float indentationLeft = 0;
    
		/// <summary> The indentation of this list on the right side. </summary>
		private float indentationRight = 0;
    
		/// <summary> The indentation of the listitems. </summary>
		private int symbolIndent;

		/// <summary> Contains extra markupAttributes </summary>
		protected Properties markupAttributes;
    
		// constructors
    
		/// <summary>
		/// Constructs a List.
		/// </summary>
		/// <remarks>
		/// the parameter symbolIndent is important for instance when
		/// generating PDF-documents; it indicates the indentation of the listsymbol.
		/// </remarks>
		/// <param name="numbered">a boolean</param>
		/// <param name="symbolIndent">the indentation that has to be used for the listsymbol</param>
		public List(bool numbered, int symbolIndent) {
			this.numbered = numbered;
			this.lettered = false;
			this.symbolIndent = symbolIndent;
		}
    
		/// <summary>
		/// Constructs a List.
		/// </summary>
		/// <param name="numbered">a boolean</param>
		/// <param name="lettered">a boolean</param>
		/// <param name="symbolIndent">the indentation that has to be used for the listsymbol</param>
		public List(bool numbered, bool lettered, int symbolIndent ) {
			this.numbered = numbered;
			this.lettered = lettered;
			this.symbolIndent = symbolIndent;
		}
    
		/// <summary>
		/// Returns a List that has been constructed taking in account
		/// the value of some attributes.
		/// </summary>
		/// <param name="attributes">Some attributes</param>
		public List(Properties attributes) {
			string value= attributes.Remove(ElementTags.LISTSYMBOL);
			if (value == null) {
				value = "-";
			}
			symbol = new Chunk(value, FontFactory.getFont(attributes));
        
			this.numbered = false;
			if ((value = attributes.Remove(ElementTags.NUMBERED)) != null) {
				this.numbered = bool.Parse(value);
				if ( this.lettered && this.numbered )
					this.lettered = false;
			}
			if ((value = attributes.Remove(ElementTags.LETTERED)) != null) {
				this.lettered = bool.Parse(value);
				if ( this.numbered && this.lettered )
					this.numbered = false;
			}
			this.symbolIndent = 0;
			if ((value = attributes.Remove(ElementTags.SYMBOLINDENT)) != null) {
				this.symbolIndent = int.Parse(value);
			}
        
			if ((value = attributes.Remove(ElementTags.FIRST)) != null) {
				char khar = value[0];
				if ( char.IsLetter( khar ) ) {
					setFirst( khar );
				}
				else {
					setFirst((char)int.Parse(value));
				}
			}
			if ((value = attributes.Remove(ElementTags.INDENTATIONLEFT)) != null) {
				this.indentationLeft = float.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.INDENTATIONRIGHT)) != null) {
				this.IndentationRight = float.Parse(value);
			}
			if (attributes.Count > 0) this.markupAttributes = attributes;
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
				foreach(IElement ele in list) {
					listener.Add(ele);
				}
				return true;
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
				return Element.LIST;
			}
		}
    
		/// <summary>
		/// Gets all the chunks in this element.
		/// </summary>
		/// <value>an ArrayList</value>
		public ArrayList Chunks {
			get {
				ArrayList tmp = new ArrayList();
				foreach(IElement ele in list) {
					tmp.AddRange(ele.Chunks);
				}
				return tmp;
			}
		}
    
		// methods to set the membervariables
    
		/// <summary>
		/// Adds an Object to the List.
		/// </summary>
		/// <param name="o">the object to add</param>
		/// <returns>true is successful</returns>
		public bool Add(Object o) {
			if (o is ListItem) {
				ListItem item = (ListItem) o;
				if (numbered || lettered) {
					Chunk chunk;
					if ( numbered )
						chunk = new Chunk((first + list.Count).ToString(), symbol.Font);
					else
						chunk = new Chunk(nextLetter(), symbol.Font);
					chunk.append(".");
					item.ListSymbol = chunk;
				}
				else {
					item.ListSymbol = symbol;
				}
				item.IndentationLeft = symbolIndent;
				item.IndentationRight = 0;
				list.Add(item);
			}
			else if (o is List) {
				List nested = (List) o;
				nested.indentationLeft = nested.IndentationLeft + symbolIndent;
				first--;
				list.Add(nested);
				return true;
			}
			else if (o is string) {
				return this.Add(new ListItem((string) o));
			}
			return false;
		}
    
		/// <summary>
		/// Sets the Letter that has to come first in the list.
		/// </summary>
		/// <param name="first">a letter</param>
		public void setFirst(char first) {
			this.firstCh = first;
			if ( char.IsLower(this.firstCh)) {
				this.lastCh = 'z';
			}
			else {
				this.lastCh = 'Z';
			}
		}

		/// <summary>
		/// Sets the symbol
		/// </summary>
		/// <value>a Chunk</value>
		public Chunk ListSymbol {
			set {
				this.symbol = value;
			}
		}

		/// <summary>
		/// Sets the listsymbol.
		/// </summary>
		/// <remarks>
		/// This is a shortcut for setListSymbol(Chunk symbol).
		/// </remarks>
		/// <param name="symbol">a string</param>
		public void setListSymbol(string symbol) {
			this.symbol = new Chunk(symbol);
		}
    
		// methods to retrieve information
    
		/// <summary>
		/// Gets all the items in the list.
		/// </summary>
		/// <value>an ArrayList containing ListItems</value>
		public ArrayList Items {
			get {
				return list;
			}
		}
    
		/// <summary>
		/// Gets the size of the list.
		/// </summary>
		/// <value>a size</value>
		public int Size {
			get {
				return list.Count;
			}
		}
    
		/// <summary>
		/// Gets the leading of the first listitem.
		/// </summary>
		/// <value>a leading</value>
		public float Leading {
			get {
				if (list.Count < 1) {
					return -1;
				}
				ListItem item = (ListItem)list[0];
				return item.Leading;
			}
		}
    
		/// <summary>
		/// Checks if the list is numbered.
		/// </summary>
		/// <returns>true if the list is numbered, false otherwise.</returns>
		public bool isNumbered() {
			return numbered;
		}
    
		/// <summary>
		/// Gets the symbol indentation.
		/// </summary>
		/// <value>the symbol indentation</value>
		public int SymbolIndent {
			get {
				return symbolIndent;
			}
		}
    
		/// <summary>
		/// Get/set the symbol indentation.
		/// </summary>
		/// <value>a Chunk</value>
		public Chunk Symbol {
			get {
				return symbol;
			}

			set {
				this.symbol = value;
			}
		}
    
		/// <summary>
		/// Get/set the first number
		/// </summary>
		/// <value>an int</value>
		public int First {
			get {
				return first;
			}

			set {
				this.first = value;
			}
		}
    
		/// <summary>
		/// Get/set the indentation of this paragraph on the left side.
		/// </summary>
		/// <value>the indentation</value>
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
		/// <value>the indentation</value>
		public float IndentationRight {
			get {
				return indentationRight;
			}

			set {
				this.indentationRight = value;
			}
		}
    
		/// <summary>
		/// Checks if a given tag corresponds with the listsymbol tag of this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public static bool isSymbol(string tag) {
			return ElementTags.LISTSYMBOL.Equals(tag);
		}
    
		/// <summary>
		/// Checks if a given tag corresponds with this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public static bool isTag(string tag) {
			return ElementTags.LIST.Equals(tag);
		}

		/// <summary>
		/// Retrieves the next letter in the sequence
		/// </summary>
		/// <returns>string contains the next character (A-Z or a-z)</returns>
		private string nextLetter() {

			int num_in_list = list.Count;
			int max_ival = (lastCh + 0);
			int ival = (firstCh + num_in_list);
			while ( ival > max_ival ) {
				ival -= 26;
			}
			char[] new_char = new char[1];
			new_char[0] = (char) ival;
			string ret = new string( new_char );
			return ret;
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
			return (markupAttributes == null) ? null : (string)markupAttributes[name];
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
