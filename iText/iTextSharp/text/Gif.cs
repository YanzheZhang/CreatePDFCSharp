using System;
using System.IO;
using System.Net;

/*
 * $Id: Gif.cs,v 1.3 2003/07/19 17:35:20 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2000, 2001, 2002 by Bruno Lowagie.
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
	/// An Gif is the representation of a graphic element (GIF)
	/// that has to be inserted into the document
	/// </summary>
	/// <seealso cref="T:iTextSharp.text.Element"/>
	/// <seealso cref="T:iTextSharp.text.Image"/>
	/// <seealso cref="T:iTextSharp.text.Jpeg"/>
	/// <seealso cref="T:iTextSharp.text.Png"/>
	public class Gif : Image, IElement {
    
		// Constructors
		Gif(Image image) : base(image) {}
    
		/// <summary>
		/// Constructs a Gif-object, using an url.
		/// </summary>
		/// <param name="url">the URL where the image can be found</param>
		public Gif(Uri url) : base(url) {
			processParameters();
		}

		public Gif(Stream str) : base(str) {
			processParameters();
		}
    
		/// <summary>
		/// Constructs a Gif-object, using an url.
		/// </summary>
		/// <remarks>
		/// Deprecated, use Image.getInstance(...) to create an Image
		/// </remarks>
		/// <param name="url">the URL where the image can be found.</param>
		/// <param name="width">the width you want the image to have</param>
		/// <param name="height">the height you want the image to have.</param>	
		public Gif(Uri url, float width, float height) : this(url) {
			scaledWidth = width;
			scaledHeight = height;
		}
    
		/// <summary>
		/// Constructs a Gif-object, using a filename.
		/// </summary>
		/// <remarks>
		/// Deprecated, use Image.getInstance(...) to create an Image
		/// </remarks>
		/// <param name="filename">a string-representation of the file that contains the Image</param>
		public Gif(string filename) : this(Image.toURL(filename)) {}
    
		/// <summary>
		/// Constructs a Gif-object, using a filename.
		/// </summary>
		/// <remarks>
		/// Deprecated, use Image.getInstance(...) to create an Image
		/// </remarks>
		/// <param name="filename">a string-representation of the file that contains the Image</param>
		/// <param name="width">the width you want the image to have</param>
		/// <param name="height">the height you want the image to have.</param>	
		public Gif(string filename, float width, float height) : this(Image.toURL(filename), width, height) {}
    
		/// <summary>
		/// Constructs a Gif-object from memory.
		/// </summary>
		/// <param name="img">the memory image</param>
		public Gif(byte[] img) : base((Uri)null) {
			rawData = img;
			processParameters();
		}
    
		/// <summary>
		/// Constructs a Gif-object from memory.
		/// </summary>
		/// <param name="img">the memory image</param>
		/// <param name="width">the width you want the image to have</param>
		/// <param name="height">the height you want the image to have.</param>	
		public Gif(byte[] img, float width, float height) : this(img) {
			scaledWidth = width;
			scaledHeight = height;
		}
    
		// private methods
    
		/// <summary>
		/// This method checks if the image is a valid GIF and processes some parameters.
		/// </summary>
		private void processParameters() {
			type = Element.GIF;
			Stream istr = null;
			try {
				string errorID;
				if (rawData == null){
					WebRequest w = WebRequest.Create(url);
					istr = w.GetResponse().GetResponseStream();
					errorID = url.ToString();
				}
				else{
					istr = new MemoryStream(rawData);
					errorID = "Byte array";
				}
				if (istr.ReadByte() != 'G' || istr.ReadByte() != 'I' || istr.ReadByte() != 'F')	{
					throw new BadElementException(errorID + " is not a valid GIF-file.");
				}
				skip(istr, 3);
				scaledWidth = istr.ReadByte() + (istr.ReadByte() << 8);
				this.Right = scaledWidth;
				scaledHeight = istr.ReadByte() + (istr.ReadByte() << 8);
				this.Top = scaledHeight;
			}
			finally {
				if (istr != null) {
					istr.Close();
				}
				plainWidth = this.Width;
				plainHeight = this.Height;
			}
		}
	}
}
