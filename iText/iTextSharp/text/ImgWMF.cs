using System;
using System.IO;
using System.Net;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.wmf;

/*
 * $Id: ImgWMF.cs,v 1.3 2003/07/19 17:35:20 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 1999, 2000, 2001, 2002 by Paulo Soares.
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
	/**
	 * An ImgWMF is the representation of a windows metafile
	 * that has to be inserted into the document
	 *
	 * @see		Element
	 * @see		Image
	 * @see		Gif
	 * @see		Png
	 */
	/// <summary>
	/// An ImgWMF is the representation of a windows metafile
	/// that has to be inserted into the document
	/// </summary>
	public class ImgWMF : Image, IElement {
    
		// Constructors
		/// <summary>
		/// Constructs an ImgWMF-object
		/// </summary>
		/// <param name="image">a Image</param>
		ImgWMF(Image image) : base(image) {}
    
		/// <summary>
		/// Constructs an ImgWMF-object, using an url.
		/// </summary>
		/// <param name="url">the URL where the image can be found</param>
		public ImgWMF(Uri url) : base(url) {
			processParameters();
		}

		public ImgWMF(Stream str) : base(str) {
			processParameters();
		}
    
		/// <summary>
		/// Constructs an ImgWMF-object, using a filename.
		/// </summary>
		/// <param name="filename">a string-representation of the file that contains the image.</param>
		public ImgWMF(string filename) : this(Image.toURL(filename)) {}
    
		/// <summary>
		/// Constructs an ImgWMF-object from memory.
		/// </summary>
		/// <param name="img">the memory image</param>
		public ImgWMF(byte[] img) : base((Uri)null) {
			rawData = img;
			processParameters();
		}
    
		/// <summary>
		/// This method checks if the image is a valid WMF and processes some parameters.
		/// </summary>
		private void processParameters() {
			type = Element.IMGTEMPLATE;
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
				InputMeta im = new InputMeta(istr);
				if (im.readInt() != unchecked((int)0x9AC6CDD7))	{
					throw new BadElementException(errorID + " is not a valid placeable windows metafile.");
				}
				im.readWord();
				int left = im.readShort();
				int top = im.readShort();
				int right = im.readShort();
				int bottom = im.readShort();
				int inch = im.readWord();
				dpiX = 72;
				dpiY = 72;
				scaledHeight = (float)(bottom - top) / inch * 72f;
				this.Top =scaledHeight;
				scaledWidth = (float)(right - left) / inch * 72f;
				this.Right = scaledWidth;
			}
			finally {
				if (istr != null) {
					istr.Close();
				}
				plainWidth = this.Width;
				plainHeight = this.Height;
			}
		}
    
		/// <summary>
		/// Reads the WMF into a template.
		/// </summary>
		/// <param name="template">the template to read to</param>
		public void readWMF(PdfTemplate template) {
			this.template = template;
			template.Width = this.Width;
			template.Height = this.Height;
			Stream istr = null;
			try {
				if (rawData == null){
					WebRequest w = WebRequest.Create(url);
					istr = w.GetResponse().GetResponseStream();
				}
				else{
					istr = new MemoryStream(rawData);
				}
				MetaDo meta = new MetaDo(istr, template);
				meta.readAll();
			}
			finally {
				if (istr != null) {
					istr.Close();
				}
			}
		}
	}
}
