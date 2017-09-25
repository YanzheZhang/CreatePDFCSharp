using System;
using System.Collections;
using System.Drawing;
using System.util;

using iTextSharp.text.pdf;

/*
 * $Id: Graphic.cs,v 1.2 2003/03/19 17:29:27 geraldhenson Exp $
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
	/// A Graphic element can contain several geometric figures (curves, lines,...).
	/// </summary>
	/// <remarks>
	/// If you want to use this Element, please read the Sections 8.4 and 8.5 of
	/// the PDF Reference Manual version 1.3 first.
	/// </remarks>
	/// <seealso cref="T:iTextSharp.text.Element"/>
	public class Graphic : PdfContentByte, IElement {
    
		/// <summary> This is a type of Graphic. </summary>
		public static string HORIZONTAL_LINE = "HORIZONTAL";
    
		/// <summary> This is a type of Graphic. </summary>
		public static string BORDER = "BORDER";
    
		/// <summary> Contains some of the attributes for this Graphic. </summary>
		private Hashmap attributes;
    
		// constructor
    
		/// <summary>
		/// Constructs a Graphic-object.
		/// </summary>
		public Graphic() : base(null) {}
    
		// implementation of the Element interface
    
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
				return Element.GRAPHIC;
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
    
		/// <summary>
		/// Orders this graphic to draw a horizontal line.
		/// </summary>
		/// <param name="linewidth">the width</param>
		/// <param name="percentage">the percentage</param>
		public void setHorizontalLine(float linewidth, float percentage) {
			if (attributes == null) attributes = new Hashmap();
			attributes.Add(HORIZONTAL_LINE, new Object[]{linewidth, percentage, new Color(0, 0, 0)});
		}
    
		/**
		 * Orders this graphic to draw a horizontal line.
		 */
		/// <summary>
		/// Orders this graphic to draw a horizontal line.
		/// </summary>
		/// <param name="linewidth">the width</param>
		/// <param name="percentage">the percentage</param>
		/// <param name="color">the Color</param>
		public void setHorizontalLine(float linewidth, float percentage, Color color) {
			if (attributes == null) attributes = new Hashmap();
			attributes.Add(HORIZONTAL_LINE, new Object[]{linewidth, percentage, color});
		}
    
		/**
		 * draws a horizontal line.
		 */
		/// <summary>
		/// draws a horizontal line.
		/// </summary>
		/// <param name="linewidth">the width</param>
		/// <param name="color">the Color</param>
		/// <param name="x1"></param>
		/// <param name="x2"></param>
		/// <param name="y"></param>
		public void drawHorizontalLine(float lineWidth, Color color, float x1, float x2, float y) {
			LineWidth = lineWidth;
			ColorStroke = color;
			moveTo(x1, y);
			lineTo(x2, y);
			stroke();
			resetRGBColorStroke();
		}
    
		/// <summary>
		/// Orders this graphic to draw a horizontal line.
		/// </summary>
		/// <param name="linewidth">the width</param>
		/// <param name="extraSpace"></param>
		public void setBorder(float linewidth, float extraSpace) {
			if (attributes == null) attributes = new Hashmap();
			attributes.Add(BORDER, new Object[]{linewidth, extraSpace, new Color(0, 0, 0)});
		}
    
		/// <summary>
		/// Orders this graphic to draw a horizontal line.
		/// </summary>
		/// <param name="linewidth"></param>
		/// <param name="extraSpace"></param>
		/// <param name="color"></param>
		public void setBorder(float linewidth, float extraSpace, Color color) {
			if (attributes == null) attributes = new Hashmap();
			attributes.Add(BORDER, new Object[]{linewidth, extraSpace, color});
		}
    
		/// <summary>
		/// Draws a border
		/// </summary>
		/// <param name="lineWidth"></param>
		/// <param name="color"></param>
		/// <param name="llx"></param>
		/// <param name="lly"></param>
		/// <param name="urx"></param>
		/// <param name="ury"></param>
		public void drawBorder(float lineWidth, Color color, float llx, float lly, float urx, float ury) {
			LineWidth = lineWidth;
			ColorStroke = color;
			rectangle(llx, lly, urx - llx, ury - lly);
			stroke();
			resetRGBColorStroke();
		}
    
		/// <summary>
		/// Processes the attributes of this object.
		/// </summary>
		/// <param name="llx">the lower left x-value</param>
		/// <param name="lly">the lower left y-value</param>
		/// <param name="urx">the upper right x-value</param>
		/// <param name="ury">the upper right y-value</param>
		/// <param name="y"></param>
		public void processAttributes(float llx, float lly, float urx, float ury, float y) {
			if (attributes == null) return;
			Object[] o;
			foreach(string attribute in attributes.Keys) {
				o = (Object[]) attributes[attribute];
				if (HORIZONTAL_LINE.Equals(attribute)) {
					float p = ((float)o[1]);
					float w = (urx - llx) * (100.0f - p) / 200.0f;
					drawHorizontalLine(((float)o[0]), (Color)o[2], llx + w, urx - w, y);
				}
				if (BORDER.Equals(attribute)) {
					float extra = ((float)o[1]);
					drawBorder(((float)o[0]), (Color)o[2], llx - extra, lly - extra, urx + extra, ury + extra);
				}
			}
		}
	}
}
