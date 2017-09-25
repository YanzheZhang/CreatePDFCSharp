using System;
using System.Collections;
using System.Drawing;
using System.util;

using iTextSharp.text;

/*
 * $Id: PdfChunk.cs,v 1.2 2003/08/22 16:18:12 geraldhenson Exp $
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
	 * A <CODE>PdfChunk</CODE> is the PDF translation of a <CODE>Chunk</CODE>.
	 * <P>
	 * A <CODE>PdfChunk</CODE> is a <CODE>PdfString</CODE> in a certain
	 * <CODE>PdfFont</CODE> and <CODE>Color</CODE>.
	 *
	 * @see		PdfString
	 * @see		PdfFont
	 * @see		iTextSharp.text.Chunk
	 * @see		iTextSharp.text.Font
	 */

	public class PdfChunk : ISplitCharacter{


		private static float ITALIC_ANGLE = 0.21256f;

		/** The allowed attributes in variable <CODE>attributes</CODE>. */
		private static Hashmap keysAttributes = new Hashmap();
    
		/** The allowed attributes in variable <CODE>noStroke</CODE>. */
		private static Hashmap keysNoStroke = new Hashmap();
    
		static PdfChunk() {
			keysAttributes.Add(Chunk.ACTION, null);
			keysAttributes.Add(Chunk.STRIKETHRU, null);
			keysAttributes.Add(Chunk.UNDERLINE, null);
			keysAttributes.Add(Chunk.REMOTEGOTO, null);
			keysAttributes.Add(Chunk.LOCALGOTO, null);
			keysAttributes.Add(Chunk.LOCALDESTINATION, null);
			keysAttributes.Add(Chunk.GENERICTAG, null);
			keysAttributes.Add(Chunk.NEWPAGE, null);
			keysAttributes.Add(Chunk.IMAGE, null);
			keysAttributes.Add(Chunk.BACKGROUND, null);
			keysAttributes.Add(Chunk.PDFANNOTATION, null);
			keysAttributes.Add(Chunk.SKEW, null);
			keysNoStroke.Add(Chunk.SUBSUPSCRIPT, null);
			keysNoStroke.Add(Chunk.SPLITCHARACTER, null);
			keysNoStroke.Add(Chunk.HYPHENATION, null);
			keysNoStroke.Add(Chunk.TEXTRENDERMODE, null);
		}
    
		// membervariables

		/** The value of this object. */
		protected string value = PdfObject.NOTHING;
    
		/** The encoding. */
		protected string encoding = PdfObject.ENCODING;
    
    
		/** The font for this <CODE>PdfChunk</CODE>. */
		protected PdfFont font;
    
		protected ISplitCharacter splitCharacter;
		/**
		 * Metric attributes.
		 * <P>
		 * This attributes require the mesurement of characters widths when rendering
		 * such as underline.
		 */
		protected Hashmap attributes = new Hashmap();
    
		/**
		 * Non metric attributes.
		 * <P>
		 * This attributes do not require the mesurement of characters widths when rendering
		 * such as Color.
		 */
		protected Hashmap noStroke = new Hashmap();
    
		/** <CODE>true</CODE> if the chunk split was cause by a newline. */
		protected bool newlineSplit;
    
		/** The image in this <CODE>PdfChunk</CODE>, if it has one */
		protected Image image;
    
		/** The offset in the x direction for the image */
		protected float offsetX;
    
		/** The offset in the y direction for the image */
		protected float offsetY;

		/** Indicates if the height and offset of the Image has to be taken into account */
		protected bool changeLeading = false;

		// constructors
    
		/**
		 * Constructs a <CODE>PdfChunk</CODE>-object.
		 *
		 * @param string the content of the <CODE>PdfChunk</CODE>-object
		 * @param font the <CODE>PdfFont</CODE>
		 * @param attributes the metrics attributes
		 * @param noStroke the non metric attributes
		 */
    
		internal PdfChunk(string str, PdfChunk other) {
			value = str;
			this.font = other.font;
			this.attributes = other.attributes;
			this.noStroke = other.noStroke;
			Object[] obj = (Object[])attributes[Chunk.IMAGE];
			if (obj == null)
				image = null;
			else {
				image = (Image)obj[0];
				offsetX = ((float)obj[1]);
				offsetY = ((float)obj[2]);
				changeLeading = bool.Parse(obj[3].ToString());
			}
			encoding = font.Font.Encoding;
			splitCharacter = (ISplitCharacter)noStroke[Chunk.SPLITCHARACTER];
			if (splitCharacter == null)
				splitCharacter = this;
		}
    
		/**
		 * Constructs a <CODE>PdfChunk</CODE>-object.
		 *
		 * @param chunk the original <CODE>Chunk</CODE>-object
		 * @param action the <CODE>PdfAction</CODE> if the <CODE>Chunk</CODE> comes from an <CODE>Anchor</CODE>
		 */
    
		internal PdfChunk(Chunk chunk, PdfAction action) {
			value = chunk.Content;
        
			iTextSharp.text.Font f = chunk.Font;
			float size = f.Size;
			if (size == iTextSharp.text.Font.UNDEFINED)
				size = 12;
			BaseFont bf = f.BaseFont;
			int family;
			int style = f.Style;

			if (bf == null) {
				// translation of the font-family to a PDF font-family
				if (style == iTextSharp.text.Font.UNDEFINED) {
					style = iTextSharp.text.Font.NORMAL;
				}
				switch(f.Family) {
					case iTextSharp.text.Font.COURIER:
					switch(style & iTextSharp.text.Font.BOLDITALIC) {
						case iTextSharp.text.Font.BOLD:
							family = PdfFont.COURIER_BOLD;
							break;
						case iTextSharp.text.Font.ITALIC:
							family = PdfFont.COURIER_OBLIQUE;
							break;
						case iTextSharp.text.Font.BOLDITALIC:
							family = PdfFont.COURIER_BOLDOBLIQUE;
							break;
						default:
						case iTextSharp.text.Font.NORMAL:
							family = PdfFont.COURIER;
							break;
					}
						break;
					case iTextSharp.text.Font.TIMES_NEW_ROMAN:
					switch(style & iTextSharp.text.Font.BOLDITALIC) {
						case iTextSharp.text.Font.BOLD:
							family = PdfFont.TIMES_BOLD;
							break;
						case iTextSharp.text.Font.ITALIC:
							family = PdfFont.TIMES_ITALIC;
							break;
						case iTextSharp.text.Font.BOLDITALIC:
							family = PdfFont.TIMES_BOLDITALIC;
							break;
						default:
						case iTextSharp.text.Font.NORMAL:
							family = PdfFont.TIMES_ROMAN;
							break;
					}
						break;
					case iTextSharp.text.Font.SYMBOL:
						family = PdfFont.SYMBOL;
						break;
					case iTextSharp.text.Font.ZAPFDINGBATS:
						family = PdfFont.ZAPFDINGBATS;
						break;
					default:
					case iTextSharp.text.Font.HELVETICA:
					switch(style & iTextSharp.text.Font.BOLDITALIC) {
						case iTextSharp.text.Font.BOLD:
							family = PdfFont.HELVETICA_BOLD;
							break;
						case iTextSharp.text.Font.ITALIC:
							family = PdfFont.HELVETICA_OBLIQUE;
							break;
						case iTextSharp.text.Font.BOLDITALIC:
							family = PdfFont.HELVETICA_BOLDOBLIQUE;
							break;
						default:
						case iTextSharp.text.Font.NORMAL:
							family = PdfFont.HELVETICA;
							break;
					}
						break;
				}
				// creation of the PdfFont with the right size
				font = new PdfFont(family, size);

      
      
			}
			else{
				// bold simulation
				if ((style & iTextSharp.text.Font.BOLD) != 0)
					attributes.Add(Chunk.TEXTRENDERMODE, new Object[]{PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE, size / 30f, null});
				// italic simulation
				if ((style & iTextSharp.text.Font.ITALIC) != 0)
					attributes.Add(Chunk.SKEW, new float[]{0, ITALIC_ANGLE});
			
				font = new PdfFont(bf, size);
			}
			
				


			// other style possibilities
			Hashmap attr = chunk.Attributes;
			if (attr != null) {
				foreach(string name in attr.Keys) {
					if (keysAttributes.ContainsKey(name)) {
						attributes.Add(name, attr[name]);
					}
					else if (keysNoStroke.ContainsKey(name)) {
						noStroke.Add(name, attr[name]);
					}
				}
				if ("".Equals(attr[Chunk.GENERICTAG])) {
					attributes.Add(Chunk.GENERICTAG, chunk.Content);
				}
			}
			if (f.isUnderlined())
				attributes.Add(Chunk.UNDERLINE, null);
			if (f.isStrikethru())
				attributes.Add(Chunk.STRIKETHRU, null);
			if (action != null)
				attributes.Add(Chunk.ACTION, action);
			// the color can't be stored in a PdfFont
			noStroke.Add(Chunk.COLOR, f.Color);
			noStroke.Add(Chunk.ENCODING, font.Font.Encoding);
			Object[] obj = (Object[])attributes[Chunk.IMAGE];
			if (obj == null)
				image = null;
			else {
				image = (Image)obj[0];
				offsetX = ((float)obj[1]);
				offsetY = ((float)obj[2]);
				changeLeading = bool.Parse(obj[3].ToString());
			}
			font.Image = image;
			encoding = font.Font.Encoding;
			splitCharacter = (ISplitCharacter)noStroke[Chunk.SPLITCHARACTER];
			if (splitCharacter == null)
				splitCharacter = this;
		}
    
		// methods
    
		protected int getWord(string text, int start) {
			int len = text.Length;
			while (start < len) {
				if (!char.IsLetter(text[start]))
					break;
				++start;
			}
			return start;
		}
    
		/**
		 * Splits this <CODE>PdfChunk</CODE> if it's too long for the given width.
		 * <P>
		 * Returns <VAR>null</VAR> if the <CODE>PdfChunk</CODE> wasn't truncated.
		 *
		 * @param		width		a given width
		 * @return		the <CODE>PdfChunk</CODE> that doesn't fit into the width.
		 */
    
		internal PdfChunk split(float width) {
			newlineSplit = false;
			if (image != null) {
				if (image.ScaledWidth > width) {
					PdfChunk pc = new PdfChunk(Chunk.OBJECT_REPLACEMENT_CHARACTER, this);
					value = "";
					attributes = new Hashmap();
					image = null;
					font = new PdfFont(PdfFont.HELVETICA, 12);
					return pc;
				}
				else
					return null;
			}
			IHyphenationEvent hyphenationEvent = (IHyphenationEvent)noStroke[Chunk.HYPHENATION];
			int currentPosition = 0;
			int splitPosition = -1;
			float currentWidth = 0;
        
			// loop over all the characters of a string
			// or until the totalWidth is reached
			int lastSpace = -1;
			float lastSpaceWidth = 0;
			int length = value.Length;
			char character = (char)0;
			BaseFont ft = font.Font;
			if (ft.FontType == BaseFont.FONT_TYPE_CJK && ft.getUnicodeEquivalent(' ') != ' ') {
				while (currentPosition < length) {
					// the width of every character is added to the currentWidth
					char cidChar = value[currentPosition];
					character = ft.getUnicodeEquivalent(cidChar);
					// if a newLine or carriageReturn is encountered
					if (character == '\n') {
						newlineSplit = true;
						string returnValue = value.Substring(currentPosition + 1);
						value = value.Substring(0, currentPosition);
						if (value.Length < 1) {
							value = "\u0001";
						}
						PdfChunk pc = new PdfChunk(returnValue, this);
						return pc;
					}
					currentWidth += font.width(cidChar);
					if (character == ' ') {
						lastSpace = currentPosition + 1;
						lastSpaceWidth = currentWidth;
					}
					if (currentWidth > width)
						break;
					// if a split-character is encountered, the splitPosition is altered
					if (splitCharacter.isSplitCharacter(character))
						splitPosition = currentPosition + 1;
					currentPosition++;
				}
			}
			else {
				while (currentPosition < length) {
					// the width of every character is added to the currentWidth
					character = value[currentPosition];
					// if a newLine or carriageReturn is encountered
					if (character == '\r' || character == '\n') {
						newlineSplit = true;
						int inc = 1;
						if (character == '\r' && currentPosition + 1 < length && value[currentPosition + 1] == '\n')
							inc = 2;
						string returnValue = value.Substring(currentPosition + inc);
						value = value.Substring(0, currentPosition);
						if (value.Length < 1) {
							value = " ";
						}
						PdfChunk pc = new PdfChunk(returnValue, this);
						return pc;
					}
					currentWidth += font.width(character);
					if (character == ' ') {
						lastSpace = currentPosition + 1;
						lastSpaceWidth = currentWidth;
					}
					if (currentWidth > width)
						break;
					// if a split-character is encountered, the splitPosition is altered
					if (splitCharacter.isSplitCharacter(character))
						splitPosition = currentPosition + 1;
					currentPosition++;
				}
			}
        
			// if all the characters fit in the total width, null is returned (there is no overflow)
			if (currentPosition == length) {
				return null;
			}
			// otherwise, the string has to be truncated
			if (splitPosition < 0) {
				string returnValue = value;
				value = "";
				PdfChunk pc = new PdfChunk(returnValue, this);
				return pc;
			}
			if (lastSpace > splitPosition && splitCharacter.isSplitCharacter(' '))
				splitPosition = lastSpace;
			if (hyphenationEvent != null && lastSpace < currentPosition) {
				int wordIdx = getWord(value, lastSpace);
				if (wordIdx > lastSpace) {
					string pre = hyphenationEvent.getHyphenatedWordPre(value.Substring(lastSpace, wordIdx - lastSpace), font.Font, font.Size, width - lastSpaceWidth);
					string post = hyphenationEvent.HyphenatedWordPost;
					if (post.Length > 0) {
						string returnValue = post + value.Substring(wordIdx);
						value = trim(value.Substring(0, lastSpace) + pre);
						PdfChunk pc = new PdfChunk(returnValue, this);
						return pc;
					}
				}
			}
			string retVal = value.Substring(splitPosition);
			value = trim(value.Substring(0, splitPosition));
			PdfChunk tmp = new PdfChunk(retVal, this);
			return tmp;
		}
    
		/**
		 * Truncates this <CODE>PdfChunk</CODE> if it's too long for the given width.
		 * <P>
		 * Returns <VAR>null</VAR> if the <CODE>PdfChunk</CODE> wasn't truncated.
		 *
		 * @param		width		a given width
		 * @return		the <CODE>PdfChunk</CODE> that doesn't fit into the width.
		 */
    
		internal PdfChunk truncate(float width) {
			if (image != null) {
				if (image.ScaledWidth > width) {
					PdfChunk pc = new PdfChunk("", this);
					value = "";
					attributes.Remove(Chunk.IMAGE);
					image = null;
					font = new PdfFont(PdfFont.HELVETICA, 12);
					return pc;
				}
				else
					return null;
			}
        
			int currentPosition = 0;
			float currentWidth = 0;
        
			// it's no use trying to split if there isn't even enough place for a space
			if (width < font.Width) {
				string returnValue = value.Substring(1);
				value = value.Substring(0, 1);
				PdfChunk pc = new PdfChunk(returnValue, this);
				return pc;
			}
        
			// loop over all the characters of a string
			// or until the totalWidth is reached
			int length = value.Length;
			char character;
			while (currentPosition < length) {
				// the width of every character is added to the currentWidth
				character = value[currentPosition];
				currentWidth += font.width(character);
				if (currentWidth > width)
					break;
				currentPosition++;
			}
        
			// if all the characters fit in the total width, null is returned (there is no overflow)
			if (currentPosition == length) {
				return null;
			}
        
			// otherwise, the string has to be truncated
			//currentPosition -= 2;
			// we have to chop off minimum 1 character from the chunk
			if (currentPosition == 0) {
				currentPosition = 1;
			}
			string retVal = value.Substring(currentPosition);
			value = value.Substring(0, currentPosition);
			PdfChunk tmp = new PdfChunk(retVal, this);
			return tmp;
		}
    
		// methods to retrieve the membervariables
    
		/**
		 * Returns the font of this <CODE>Chunk</CODE>.
		 *
		 * @return	a <CODE>PdfFont</CODE>
		 */
    
		internal PdfFont Font {
			get {
				return font;
			}
		}
    
		/**
		 * Returns the color of this <CODE>Chunk</CODE>.
		 *
		 * @return	a <CODE>Color</CODE>
		 */
    
		internal Color color {
			get {
				return (Color)noStroke[Chunk.COLOR];
			}
		}
    
		/**
		 * Returns the width of this <CODE>PdfChunk</CODE>.
		 *
		 * @return	a width
		 */
    
		internal float Width {
			get {
				if (image != null)
					return image.ScaledWidth;
				return font.Font.getWidthPoint(value, font.Size);
			}
		}
    
		/**
		 * Checks if the <CODE>PdfChunk</CODE> split was caused by a newline.
		 * @return <CODE>true</CODE> if the <CODE>PdfChunk</CODE> split was caused by a newline.
		 */
    
		public bool isNewlineSplit() {
			return newlineSplit;
		}
    
		/**
		 * Gets the width of the <CODE>PdfChunk</CODE> taking into account the
		 * extra character and word spacing.
		 * @param charSpacing the extra character spacing
		 * @param wordSpacing the extra word spacing
		 * @return the calculated width
		 */
    
		public float getWidthCorrected(float charSpacing, float wordSpacing) {
			if (image != null) {
				return image.ScaledWidth + charSpacing;
			}
			int numberOfSpaces = 0;
			int idx = -1;
			while ((idx = value.IndexOf(' ', idx + 1)) >= 0)
				++numberOfSpaces;
			return font.Font.getWidthPoint(value, font.Size) + value.Length * charSpacing + numberOfSpaces * wordSpacing;
		}
    
		/**
		 * Trims the last space.
		 * @return the width of the space trimmed, otherwise 0
		 */
    
		public float trimLastSpace() {
			BaseFont ft = font.Font;
			if (ft.FontType == BaseFont.FONT_TYPE_CJK && ft.getUnicodeEquivalent(' ') != ' ') {
				if (value.Length > 1 && value.EndsWith("\u0001")) {
					value = value.Substring(0, value.Length - 1);
					return font.width('\u0001');
				}
			}
			else {
				if (value.Length > 1 && value.EndsWith(" ")) {
					value = value.Substring(0, value.Length - 1);
					return font.width(' ');
				}
			}
			return 0;
		}
    
		/**
		 * Gets an attribute. The search is made in <CODE>attributes</CODE>
		 * and <CODE>noStroke</CODE>.
		 * @param name the attribute key
		 * @return the attribute value or null if not found
		 */
    
		internal Object getAttribute(string name) {
			if (attributes.ContainsKey(name))
				return attributes[name];
			return noStroke[name];
		}
    
		/**
		 *Checks if the attribute exists.
		 * @param name the attribute key
		 * @return <CODE>true</CODE> if the attribute exists
		 */
    
		internal bool isAttribute(string name) {
			if (attributes.ContainsKey(name))
				return true;
			return noStroke.ContainsKey(name);
		}
    
		/**
		 * Checks if this <CODE>PdfChunk</CODE> needs some special metrics handling.
		 * @return <CODE>true</CODE> if this <CODE>PdfChunk</CODE> needs some special metrics handling.
		 */
    
		internal bool isStroked() {
			return (attributes.Count > 0);
		}
    
		/**
		 * Checks if there is an image in the <CODE>PdfChunk</CODE>.
		 * @return <CODE>true</CODE> if an image is present
		 */
    
		internal bool isImage() {
			return image != null;
		}
    
		/**
		 * Gets the image in the <CODE>PdfChunk</CODE>.
		 * @return the image or <CODE>null</CODE>
		 */
    
		internal Image Image {
			get {
				return image;
			}
		}
    
		/**
		 * Gets the image offset in the x direction
		 * @return the image offset in the x direction
		 */
    
		internal float ImageOffsetX {
			get {
				return offsetX;
			}

			set {
				this.offsetX = value;
			}
		}
    
		/**
		 * Gets the image offset in the y direction
		 * @return Gets the image offset in the y direction
		 */
    
		internal float ImageOffsetY {
			get {
				return offsetY;
			}

			set {
				this.offsetY = value;
			}
		}
    
		/**
		 * sets the value.
		 */
    
		internal string Value {
			set {
				this.value = value;
			}
		}

		public override string ToString() {
			return value;
		}

		/**
		 * Tells you if this string is in Chinese, Japanese, Korean or Identity-H.
		 */
    
		internal bool isSpecialEncoding() {
			return encoding.Equals(CJKFont.CJK_ENCODING) || encoding.Equals(BaseFont.IDENTITY_H);
		}
    
		/**
		 * Gets the encoding of this string.
		 *
		 * @return		a <CODE>string</CODE>
		 */
    
		internal string Encoding {
			get {
				return encoding;
			}
		}

		internal int Length {
			get {
				return value.Length;
			}
		}
		/**
		 * Checks if a character can be used to split a <CODE>PdfString</CODE>.
		 * <P>
		 * for the moment every character less than or equal to SPACE and the character '-' are 'splitCharacters'.
		 *
		 * @param	c		the character that has to be checked
		 * @return	<CODE>true</CODE> if the character can be used to split a string, <CODE>false</CODE> otherwise
		 */
		public bool isSplitCharacter(char c) {
			if (c <= ' ' || c == '-') {
				return true;
			}
			if (c < 0x2e80)
				return false;
			return ((c >= 0x2e80 && c < 0xd7a0)
				|| (c >= 0xf900 && c < 0xfb00)
				|| (c >= 0xfe30 && c < 0xfe50)
				|| (c >= 0xff61 && c < 0xffa0));
		}
    
		internal bool isExtSplitCharacter(char c) {
			return splitCharacter.isSplitCharacter(c);
		}
    
		/**
		 * Removes all the <VAR>' '</VAR> and <VAR>'-'</VAR>-characters on the right of a <CODE>string</CODE>.
		 * <P>
		 * @param	string		the <CODE>string<CODE> that has to be trimmed.
		 * @return	the trimmed <CODE>string</CODE>
		 */    
		internal string trim(string str) {
			BaseFont ft = font.Font;
			if (ft.FontType == BaseFont.FONT_TYPE_CJK && ft.getUnicodeEquivalent(' ') != ' ') {
				while (str.EndsWith("\u0001")) {
					str = str.Substring(0, str.Length - 1);
				}
			}
			else {
				while (str.EndsWith(" ") || str.EndsWith("\t")) {
					str = str.Substring(0, str.Length - 1);
				}
			}
			return str;
		}

		public bool ChangeLeading {
			get {
				return changeLeading;
			}
		}
    
		internal float getCharWidth(char c) {
			if (noPrint(c))
				return 0;
			return font.width(c);
		}
    
		public static bool noPrint(char c) {
			return ((c >= 0x200b && c <= 0x200f) || (c >= 0x202a && c <= 0x202e));
		}    
	}
}
