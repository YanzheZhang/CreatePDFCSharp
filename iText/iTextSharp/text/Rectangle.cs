using System;
using System.Collections;
using System.util;
using System.Drawing;

/*
 * $Id: Rectangle.cs,v 1.3 2003/05/15 01:49:33 geraldhenson Exp $
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
	/// A Rectangle is the representation of a geometric figure.
	/// </summary>
	/// <seealso cref="T:iTextSharp.text.Element"/>
	/// <seealso cref="T:iTextSharp.text.Table"/>
	/// <seealso cref="T:iTextSharp.text.Cell"/>
	/// <seealso cref="T:iTextSharp.text.HeaderFooter"/>
	public class Rectangle : IElement, IMarkupAttributes {
    
		// static membervariables (concerning the presence of borders)
    
		///<summary> This is the value that will be used as <VAR>undefined</VAR>. </summary>
		public static int UNDEFINED = -1;
    
		///<summary> This represents one side of the border of the Rectangle. </summary>
		public static int TOP = 1;
    
		///<summary> This represents one side of the border of the Rectangle. </summary>
		public static int BOTTOM = 2;
    
		///<summary> This represents one side of the border of the Rectangle. </summary>
		public static int LEFT = 4;
    
		///<summary> This represents one side of the border of the Rectangle. </summary>
		public static int RIGHT = 8;
    
		///<summary> This represents a rectangle without borders. </summary>
		public static int NO_BORDER = 0;
    
		///<summary> This represents a type of border. </summary>
		public static int BOX = TOP + BOTTOM + LEFT + RIGHT;
    
		// membervariables
    
		///<summary> the lower left x-coordinate. </summary>
		protected float llx;
    
		///<summary> the lower left y-coordinate. </summary>
		protected float lly;
    
		///<summary> the upper right x-coordinate. </summary>
		protected float urx;
    
		///<summary> the upper right y-coordinate. </summary>
		protected float ury;
    
		///<summary> This represents the status of the 4 sides of the rectangle. </summary>
		protected int border = UNDEFINED;
    
		///<summary> This is the width of the border around this rectangle. </summary>
		protected float borderWidth = UNDEFINED;
    
		///<summary> This is the color of the border of this rectangle. </summary>
		protected Color color = null;
    
		///<summary> This is the color of the background of this rectangle. </summary>
		protected Color background = null;
    
		///<summary> This is the grayscale value of the background of this rectangle. </summary>
		protected float grayFill = 0;
    
		///<summary> This is the rotation value of this rectangle. </summary>
		protected int rotation = 0;

		///<summary> Contains extra markupAttributes </summary>
		protected Properties markupAttributes;
    
		// constructors
    
		/// <summary>
		/// Constructs a Rectangle-object.
		/// </summary>
		/// <param name="llx">lower left x</param>
		/// <param name="lly">lower left y</param>
		/// <param name="urx">upper right x</param>
		/// <param name="ury">upper right y</param>
		public Rectangle(float llx, float lly, float urx, float ury) {
			this.llx = llx;
			this.lly = lly;
			this.urx = urx;
			this.ury = ury;
		}
    
		/// <summary>
		/// Constructs a Rectangle-object starting from the origin (0, 0).
		/// </summary>
		/// <param name="urx">upper right x</param>
		/// <param name="ury">upper right y</param>
		public Rectangle(float urx, float ury) : this(0, 0, urx, ury) {}
    
		/// <summary>
		/// Constructs a Rectangle-object.
		/// </summary>
		/// <param name="rect">another Rectangle</param>
		public Rectangle(Rectangle rect) : this(rect.Left, rect.Bottom, rect.Right, rect.Top) {
			this.llx = rect.llx;
			this.lly = rect.lly;
			this.urx = rect.urx;
			this.ury = rect.ury;
			this.rotation = rect.rotation;
			this.border = rect.border;
			this.borderWidth = rect.borderWidth;
			this.color = rect.color;
			this.background = rect.background;
			this.grayFill = rect.grayFill;
		}
    
		// implementation of the Element interface
    
		/// <summary>
		/// Processes the element by adding it (or the different parts) to an
		/// IElementListener.
		/// </summary>
		/// <param name="listener">an IElementListener</param>
		/// <returns>true if the element was processed successfully</returns>
		public virtual bool process(IElementListener listener) {
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
		public virtual int Type {
			get {
				return Element.RECTANGLE;
			}
		}
    
		/// <summary>
		/// Gets all the chunks in this element.
		/// </summary>
		/// <value>an ArrayList</value>
		public virtual ArrayList Chunks {
			get {
				return new ArrayList();
			}
		}
    
		// methods
    
		/// <summary>
		/// Gets a Rectangle that is altered to fit on the page.
		/// </summary>
		/// <param name="top">the top position</param>
		/// <param name="bottom">the bottom position</param>
		/// <returns>a Rectangle</returns>
		public Rectangle rectangle(float top, float bottom) {
			Rectangle tmp = new Rectangle(this);
			tmp.Border = border;
			tmp.BorderWidth = borderWidth;
			tmp.BorderColor = color;
			tmp.BackgroundColor = background;
			tmp.GrayFill = grayFill;
			if (this.Top > top) {
				tmp.Top = top;
				tmp.Border = border - (border & TOP);
			}
			if (Bottom < bottom) {
				tmp.Bottom = bottom;
				tmp.Border = border - (border & BOTTOM);
			}
			return tmp;
		}
    
		/// <summary>
		/// Swaps the values of urx and ury and of lly and llx in order to rotate the rectangle.
		/// </summary>
		/// <returns>a Rectangle</returns>
		public Rectangle rotate() {
			Rectangle rect = new Rectangle(lly, llx, ury, urx);
			rect.rotation = rotation + 90;
			rect.rotation %= 360;
			return rect;
		}
    
		// methods to set the membervariables
    
		/// <summary>
		/// Get/set the upper right y-coordinate. 
		/// </summary>
		/// <value>a float</value>
		public virtual float Top {
			get {
				return ury;
			}

			set {
				ury = value;
			}
		}
    
		/// <summary>
		/// Get/set the border
		/// </summary>
		/// <value>a int</value>
		public int Border {
			get {
				return this.border;
			}

			set {
				border = value;
			}
		}
    
		/// <summary>
		/// Get/set the grayscale of the rectangle.
		/// </summary>
		/// <value>a float</value>
		public float GrayFill {
			get {
				return grayFill;
			}

			set {
				if (value >= 0 && value <= 1.0) {
					grayFill = value;
				}
			}
		}
    
		// methods to get the membervariables
    
		/// <summary>
		/// Get/set the lower left x-coordinate.
		/// </summary>
		/// <value>a float</value>
		public virtual float Left {
			get {
				return llx;
			}

			set {
				llx = value;
			}
		}
    
		/// <summary>
		/// Get/set the upper right x-coordinate.
		/// </summary>
		/// <value>a float</value>
		public virtual float Right {
			get {
				return urx;
			}
		
			set {
				urx = value;
			}
		}
    
		/// <summary>
		/// Get/set the lower left y-coordinate.
		/// </summary>
		/// <value>a float</value>
		public virtual float Bottom {
			get {
				return lly;
			}

			set {
				lly = value;
			}
		}
    
		/// <summary>
		/// Returns the lower left x-coordinate, considering a given margin.
		/// </summary>
		/// <param name="margin">a margin</param>
		/// <returns>the lower left x-coordinate</returns>
		public float left(float margin) {
			return llx + margin;
		}
    
		/// <summary>
		/// Returns the upper right x-coordinate, considering a given margin.
		/// </summary>
		/// <param name="margin">a margin</param>
		/// <returns>the upper right x-coordinate</returns>
		public float right(float margin) {
			return urx - margin;
		}
    
		/// <summary>
		/// Returns the upper right y-coordinate, considering a given margin.
		/// </summary>
		/// <param name="margin">a margin</param>
		/// <returns>the upper right y-coordinate</returns>
		public float top(float margin) {
			return ury - margin;
		}
    
		/// <summary>
		/// Returns the lower left y-coordinate, considering a given margin.
		/// </summary>
		/// <param name="margin">a margin</param>
		/// <returns>the lower left y-coordinate</returns>
		public float bottom(float margin) {
			return lly + margin;
		}
    
		/// <summary>
		/// Returns the width of the rectangle.
		/// </summary>
		/// <value>a width</value>
		public float Width {
			get {
				return urx - llx;
			}
		}
    
		/// <summary>
		/// Returns the height of the rectangle.
		/// </summary>
		/// <value>a height</value>
		public float Height {
			get {
				return ury - lly;
			}
		}
    
		/// <summary>
		/// Indicates if the table has borders.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool hasBorders() {
			return border > 0 && borderWidth > 0;
		}
    
		/// <summary>
		/// Indicates if the table has a some type of border.
		/// </summary>
		/// <param name="type">the type of border</param>
		/// <returns>a bool</returns>
		public bool hasBorder(int type) {
			return border != UNDEFINED && (border & type) == type;
		}
    
		/// <summary>
		/// Get/set the borderwidth.
		/// </summary>
		/// <value>a float</value>
		public float BorderWidth {
			get {
				return borderWidth;
			}

			set {
				borderWidth = value;
			}
		}
    
		/**
		 * Gets the color of the border.
		 *
		 * @return	a value
		 */
		/// <summary>
		/// Get/set the color of the border.
		/// </summary>
		/// <value>a Color</value>
		public Color BorderColor {
			get {
				return color;
			}

			set {
				color = value;
			}
		}
    
		/**
		 * Gets the backgroundcolor.
		 *
		 * @return	a value
		 */
		/// <summary>
		/// Get/set the backgroundcolor.
		/// </summary>
		/// <value>a Color</value>
		public Color BackgroundColor {
			get {
				return background;
			}

			set {
				background = value;
			}
		}

        /// <summary>
        /// Returns the rotation
        /// </summary>
        /// <value>a int</value>    
		public int Rotation {
			get {
				return rotation;
			}
		}
    
    
		/// <summary>
		/// See <see cref="M:iTextSharp.text.IMarkupAttributes.setMarkupAttribute(System.String,System.String)"/>
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <param name="value">attribute value</param>
		public virtual void setMarkupAttribute(string name, string value) {
			markupAttributes = (markupAttributes == null) ? new Properties() : markupAttributes;
			markupAttributes.Add(name, value);
		}
    
		/// <summary>
		/// See <see cref="M:iTextSharp.text.IMarkupAttributes.getMarkupAttribute(System.String)"/>
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <returns>attribute value</returns>
		public virtual string getMarkupAttribute(string name) {
			return (markupAttributes == null) ? null : markupAttributes[name].ToString();
		}
    
		/// <summary>
		/// See <see cref="P:iTextSharp.text.IMarkupAttributes.MarkupAttributeNames"/>
		/// </summary>
		/// <value>a collection of string attribute names</value>
		public virtual ICollection MarkupAttributeNames {
			get {
				return Chunk.getKeySet(markupAttributes);
			}
		}
    
		/// <summary>
		/// See <see cref="P:iTextSharp.text.IMarkupAttributes.MarkupAttributes"/>
		/// </summary>
		/// <value>a Properties-object containing all the markupAttributes.</value>
		public virtual Properties MarkupAttributes {
			get {
				return markupAttributes;
			}

			set {
				this.markupAttributes = value;
			}
		}
	}
}
