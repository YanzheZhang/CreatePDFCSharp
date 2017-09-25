using System;
using System.Collections;
using System.util;
using System.IO;

using iTextSharp.text;

/*
 * $Id: TagMap.cs,v 1.4 2003/03/23 22:27:45 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text.xml {

	/// <summary>
	/// The <CODE>TagMap</CODE>-class maps several XHTML-tags to iText-objects.
	/// </summary>
	public class TagMap : Hashmap {
    
		class AttributeHandler : ParserBase {
        
			/// <summary> This is a tag </summary>
			public static String TAG = "tag";
        
			/// <summary> This is a tag </summary>
			public static String ATTRIBUTE = "attribute";
        
			/// <summary> This is an attribute </summary>
			public static String NAME = "name";
        
			/// <summary> This is an attribute </summary>
			public static String ALIAS = "alias";
        
			/// <summary> This is an attribute </summary>
			public static String VALUE = "value";
        
			/// <summary> This is an attribute </summary>
			public static String CONTENT = "content";
        
			/// <summary> This is the tagmap using the AttributeHandler </summary>
			private Hashmap tagMap;
        
			/// <summary> This is the current peer. </summary>
			private XmlPeer currentPeer;
        
			/// <summary>
			/// Constructs a new SAXiTextHandler that will translate all the events
			/// triggered by the parser to actions on the <CODE>Document</CODE>-object.
			/// </summary>
			/// <param name="tagMap">A Hashmap containing XmlPeer-objects</param>
			public AttributeHandler(Hashmap tagMap) {
				this.tagMap = tagMap;
			}
        
			/// <summary>
			/// This method gets called when a start tag is encountered.
			/// </summary>
			/// <param name="tag">the name of the tag that is encountered</param>
			/// <param name="lname"></param>
			/// <param name="n"></param>
			/// <param name="attrs">the list of attributes</param>
			public override void startElement(String tag, String lname, String n, Hashtable attrs) {
				String name = (string)attrs[NAME];
				String alias = (string)attrs[ALIAS];
				String value = (string)attrs[VALUE];
				if (name != null) {
					if(TAG.Equals(lname)) {
						currentPeer = new XmlPeer(name, alias);
					}
					else if (ATTRIBUTE.Equals(lname)) {
						if (alias != null) {
							currentPeer.addAlias(name, alias);
						}
						if (value != null) {
							currentPeer.addValue(name, value);
						}
					}
				}
				value = (string)attrs[CONTENT];
				if (value != null) {
					currentPeer.Content = value;
				}
			}
        
			/// <summary>
			/// This method gets called when ignorable white space encountered.
			/// </summary>
			/// <param name="ch">an array of characters</param>
			/// <param name="start">the start position in the array</param>
			/// <param name="length">the number of characters to read from the array</param>
			public void ignorableWhitespace(char[] ch, int start, int length) {
				// do nothing
			}
        
			/// <summary>
			/// This method gets called when characters are encountered.
			/// </summary>
			/// <param name="content">an array of characters</param>
			/// <param name="start">the start position in the array</param>
			/// <param name="length">the number of characters to read from the array</param>
			public override void characters(string content, int start, int length) {
				// do nothing
			}
        
			/// <summary>
			/// This method gets called when an end tag is encountered.
			/// </summary>
			/// <param name="tag">the name of the tag that ends</param>
			/// <param name="lname"></param>
			/// <param name="name"></param>
			public override void endElement(String tag, String lname, String name) {
				if (TAG.Equals(lname))
					tagMap.Add(currentPeer.Alias, currentPeer);
			}
		}
    
		/// <summary>
		/// Constructs a Tagmap object
		/// </summary>
		/// <param name="tagfile">the file of tags to parse</param>
		public TagMap(String tagfile) {
			try {
				init(tagfile);
			} catch(Exception e) {
				throw e;
			}
		}

		/// <summary>
		/// Parses the xml document
		/// </summary>
		/// <param name="tagfile"></param>
		protected void init(string tagfile) {
			try {
				AttributeHandler a = new AttributeHandler(this);
				a.Parse(tagfile);
			}
			catch(Exception e) {
				throw e;
			}
		}


	}
}