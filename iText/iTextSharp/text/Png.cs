using System;
using System.IO;
using System.Net;
using System.Text;

/*
 * $Id: Png.cs,v 1.4 2003/07/19 17:35:20 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie and Paulo Soares.
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
	/// An Png is the representation of a graphic element (PNG)
	/// that has to be inserted into the document
	/// </summary>
	/// <seealso cref="T:iTextSharp.text.Element"/>
	/// <seealso cref="T:iTextSharp.text.Image"/>
	/// <seealso cref="T:iTextSharp.text.Gif"/>
	/// <seealso cref="T:iTextSharp.text.Jpeg"/>
	public class Png : Image, IElement {
    
		// public static membervariables
    
		///<summary> Some PNG specific values. </summary>
		public static int[] PNGID = {137, 80, 78, 71, 13, 10, 26, 10};
    
		///<summary> A PNG marker. </summary>
		public static string IHDR = "IHDR";
    
		///<summary> A PNG marker. </summary>
		public static string PLTE = "PLTE";
    
		///<summary> A PNG marker. </summary>
		public static string IDAT = "IDAT";
    
		///<summary> A PNG marker. </summary>
		public static string IEND = "IEND";
    
		///<summary> A PNG marker. </summary>
		public static string tRNS = "tRNS";
    
		///<summary> A PNG marker. </summary>
		public static string pHYs = "pHYs";
    
		// Constructors
		Png(Image image) : base(image) {}
    
		/// <summary>
		/// Constructs a Png-object, using an url.
		/// </summary>
		/// <param name="url">the URL where the image can be found.</param>
		/// <overoads>
		/// Has six overloads.
		/// </overoads>    
		public Png(Uri url) : base(url) {
			processParameters();
		}

		public Png(Stream str) : base(str) {
			processParameters();
		}
    
		/// <summary>
		/// Constructs a Png-object, using an url.
		/// </summary>
		/// <remarks>
		/// Deprecated, use Image.getInstance(...) to create an Image
		/// </remarks>
		/// <param name="url">the URL where the image can be found.</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <overoads>
		/// Has six overloads.
		/// </overoads>    
		public Png(Uri url, float width, float height) : this(url) {
			scaledWidth = width;
			scaledHeight = height;
		}
    
		/**
		 * Constructs a Png-object, using a filename.
		 *
		 * @param		filename	a string-representation of the file that contains the Image.
		 * @deprecated	use Image.getInstance(...) to create an Image
		 */
		/// <summary>
		/// Constructs a Png-object, using a filename.
		/// </summary>
		/// <remarks>
		/// Deprecated, use Image.getInstance(...) to create an Image
		/// </remarks>
		/// <param name="filename">a string-representation of the file that contains the Image.</param>
		/// <overoads>
		/// Has six overloads.
		/// </overoads>    
		public Png(string filename) : this(Image.toURL(filename)) {}
    
		/// <summary>
		/// Constructs a Png-object, using a filename.
		/// </summary>
		/// <remarks>
		/// Deprecated, use Image.getInstance(...) to create an Image
		/// </remarks>
		/// <param name="filename">a string-representation of the file that contains the Image.</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <overoads>
		/// Has six overloads.
		/// </overoads>    
		public Png(string filename, float width, float height) : this(Image.toURL(filename), width, height) {}
    
		/// <summary>
		/// Constructs a Png-object from memory.
		/// </summary>
		/// <param name="img">the memory image</param>
		/// <overoads>
		/// Has six overloads.
		/// </overoads>    
		public Png(byte[] img) : base((Uri)null) {
			rawData = img;
			processParameters();
		}
    
		/// <summary>
		/// Constructs a Png-object from memory.
		/// </summary>
		/// <param name="img">the memory image</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <overoads>
		/// Has six overloads.
		/// </overoads>    
		public Png(byte[] img, float width, float height) : this(img) {
			scaledWidth = width;
			scaledHeight = height;
		}
    
		// private methods
    
		/// <summary>
		/// Gets an int from an Stream.
		/// </summary>
		/// <param name="istr">a Stream</param>
		/// <returns>the value of an int</returns>
		public static int getInt(Stream istr) {
			return (istr.ReadByte() << 24) + (istr.ReadByte() << 16) + (istr.ReadByte() << 8) + istr.ReadByte();
		}
    
		/// <summary>
		/// Gets a word from an Stream.
		/// </summary>
		/// <param name="istr">a Stream</param>
		/// <returns>the value of an int</returns>
		public static int getWord(Stream istr) {
			return (istr.ReadByte() << 8) + istr.ReadByte();
		}
    
		/// <summary>
		/// Gets a string from an Stream.
		/// </summary>
		/// <param name="istr">a Stream</param>
		/// <returns>the value of a string</returns>
		public static string getstring(Stream istr) {
			StringBuilder buf = new StringBuilder();
			for (int i = 0; i < 4; i++) {
				buf.Append((char)istr.ReadByte());
			}
			return buf.ToString();
		}
    
		// private methods
    
		/// <summary>
		/// This method checks if the image is a valid PNG and processes some parameters.
		/// </summary>
		private void processParameters() {
			this.type = Element.PNG;
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
				for (int i = 0; i < PNGID.Length; i++) {
					if (PNGID[i] != istr.ReadByte())	{
						throw new BadElementException(errorID + " is not a valid PNG-file.");
					}
				}
				while(true) {
					int len = getInt(istr);
					string id = getstring(istr);
					if (IHDR.Equals(id)) {
						scaledWidth = getInt(istr);
						this.Right = scaledWidth;
						scaledHeight = getInt(istr);
						this.Top = scaledHeight;
						skip(istr, len + 4 - 8);
						continue;
					}
					if (pHYs.Equals(id)) {
						int dx = getInt(istr);
						int dy = getInt(istr);
						int unit = istr.ReadByte();
						if (unit == 1) {
							dpiX = (int)((float)dx * 0.0254f + 0.5f);
							dpiY = (int)((float)dy * 0.0254f + 0.5f);
						}
						skip(istr, len + 4 - 9);
						continue;
					}
					if (IDAT.Equals(id) || IEND.Equals(id)) {
						break;
					}
					skip(istr, len + 4);
				}
			}
			finally {
				if (istr != null) {
					istr.Close();
				}
				plainWidth = Width;
				plainHeight = Height;
			}
		}
	}
}
