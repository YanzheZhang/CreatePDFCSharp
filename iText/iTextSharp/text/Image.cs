using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Collections;
using System.util;
using System.Reflection;

using iTextSharp.text.pdf;

/*
 * $Id: Image.cs,v 1.5 2003/07/19 17:35:20 geraldhenson Exp $
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
	/// An Image is the representation of a graphic element (JPEG, PNG or GIF)
	/// that has to be inserted into the document
	/// </summary>
	/// <seealso cref="T:iTextSharp.text.Element"/>
	/// <seealso cref="T:iTextSharp.text.Rectangle"/>
	public abstract class Image : Rectangle, IElement, IMarkupAttributes {
    
		// static membervariables (concerning the presence of borders)
    
		/// <summary> this is a kind of image Element. </summary>
		public const int DEFAULT = 0;
    
		/// <summary> this is a kind of image Element. </summary>
		public new const int RIGHT = 1;
    
		/// <summary> this is a kind of image Element. </summary>
		public new const int LEFT = 2;
    
		/// <summary> this is a kind of image Element. </summary>
		public const int MIDDLE = 3;
    
		/// <summary> this is a kind of image Element. </summary>
		public const int TEXTWRAP = 4;
    
		/// <summary> this is a kind of image Element. </summary>
		public const int UNDERLYING = 8;
    
		/// <summary> This represents a coordinate in the transformation matrix. </summary>
		public const int AX = 0;
    
		/// <summary> This represents a coordinate in the transformation matrix. </summary>
		public const int AY = 1;
    
		/// <summary> This represents a coordinate in the transformation matrix. </summary>
		public const int BX = 2;
    
		/// <summary> This represents a coordinate in the transformation matrix. </summary>
		public const int BY = 3;
    
		/// <summary> This represents a coordinate in the transformation matrix. </summary>
		public const int CX = 4;
    
		/// <summary> This represents a coordinate in the transformation matrix. </summary>
		public const int CY = 5;
    
		/// <summary> This represents a coordinate in the transformation matrix. </summary>
		public const int DX = 6;
    
		/// <summary> This represents a coordinate in the transformation matrix. </summary>
		public const int DY = 7;
    
		// membervariables
    
		/// <summary> Adobe invert CMYK JPEG </summary>
		protected bool invert = false;
    
		/// <summary> The imagetype. </summary>
		protected int type;
    
		/// <summary> The URL of the image. </summary>
		protected Uri url;
    
		/// <summary> The raw data of the image. </summary>
		protected byte[] rawData;
    
		/// <summary> The template to be treated as an image. </summary>
		protected PdfTemplate template;
    
		/// <summary> The alignment of the Image. </summary>
		protected int alignment;
    
		/// <summary> Text that can be shown instead of the image. </summary>
		protected string alt;
    
		/// <summary> This is the absolute X-position of the image. </summary>
		protected float absoluteX = float.NaN;
    
		/// <summary> This is the absolute Y-position of the image. </summary>
		protected float absoluteY = float.NaN;
    
		/// <summary> This is the width of the image without rotation. </summary>
		protected float plainWidth;
    
		/// <summary> This is the width of the image without rotation. </summary>
		protected float plainHeight;
    
		/// <summary> This is the scaled width of the image taking rotation into account. </summary>
		protected float scaledWidth;
    
		/// <summary> This is the original height of the image taking rotation into account. </summary>
		protected float scaledHeight;
    
		/// <summary> This is the rotation of the image. </summary>
		protected new float rotation;
    
		/// <summary> this is the colorspace of a jpeg-image. </summary>
		protected int colorspace = -1;
    
		/// <summary> this is the bits per component of the raw image. It also flags a CCITT image.</summary>
		protected int bpc = 1;
    
		/// <summary> this is the transparency information of the raw image</summary>
		protected int[] transparency;
    
		// serial stamping
    
		protected long mySerialId = getSerialId();
    
		static long serialId = 0;

		/// <summary> Holds value of property dpiX. </summary>
		protected int dpiX = 0;
    
		/// <summary> Holds value of property dpiY. </summary>
		protected int dpiY = 0;
    
		protected bool mask = false;
    
		protected Image imageMask;
    
		protected bool invertMask = false;
    
		/// <summary> Holds value of property interpolation. </summary>
		protected bool interpolation;
    
		/// <summary> if the annotation is not null the image will be clickable. </summary>
		protected Annotation annotation = null;

		internal class SID {
			internal static long id = 0;
		}
    
		/// <summary> Contains extra markupAttributes </summary>
		//protected Properties markupAttributes;
    
		/// <summary> ICC Profile attached </summary>
		//protected ICC_Profile profile = null;
    
		// constructors
    
		/// <summary>
		/// Constructs an Image-object, using an url.
		/// </summary>
		/// <param name="url">the URL where the image can be found.</param>
		public Image(Uri url) : base(0, 0) {
			this.url = url;
			this.alignment = DEFAULT;
			rotation = 0;
		}

		public Image(Stream str) : base(0, 0) {
			int size = (int)str.Length;
			byte[] imext = new byte[size];
			str.Read(imext, 0, size);
			this.rawData = imext;
			this.alignment = DEFAULT;
			rotation = 0;
		}
    
		/// <summary>
		/// Constructs an Image-object, using an url.
		/// </summary>
		/// <param name="image">another Image object.</param>
		protected Image(Image image) : base(image) {
			this.type = image.type;
			this.url = image.url;
			this.alignment = image.alignment;
			this.alt = image.alt;
			this.absoluteX = image.absoluteX;
			this.absoluteY = image.absoluteY;
			this.plainWidth = image.plainWidth;
			this.plainHeight = image.plainHeight;
			this.scaledWidth = image.scaledWidth;
			this.scaledHeight = image.scaledHeight;
			this.rotation = image.rotation;
			this.colorspace = image.colorspace;
			this.rawData = image.rawData;
			this.template = image.template;
			this.bpc = image.bpc;
			this.transparency = image.transparency;
			this.mySerialId = image.mySerialId;
			this.invert = image.invert;
			this.dpiX = image.dpiX;
			this.dpiY = image.dpiY;
			this.mask = image.mask;
			this.imageMask = image.imageMask;
			this.invertMask = image.invertMask;
			this.interpolation = image.interpolation;
			this.annotation = image.annotation;
			this.markupAttributes = image.markupAttributes;
			//this.profile = image.profile;
		}
    
		/// <summary>
		/// Gets an instance of an Image.
		/// </summary>
		/// <param name="image">an Image</param>
		/// <returns>an object of type Gif, Jpeg or Png</returns>
		public static Image getInstance(Image image) {
			try {
				return (Image)image.GetType().GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] {typeof(Image)}, null).Invoke(new object[] {image});
			}
			catch (Exception e) {
				throw e;
			}
		}

		public static Image getInstance(Stream str) {
			try {
				int c1 = str.ReadByte();
				int c2 = str.ReadByte();
				str.Seek(0, SeekOrigin.Begin);
            
				//str = null;
				if (c1 == 'G' && c2 == 'I') {
					return new Gif(str);
				}
				if (c1 == 0xFF && c2 == 0xD8) {
					return new Jpeg(str);
				}
				if (c1 == Png.PNGID[0] && c2 == Png.PNGID[1]) {
					return new Png(str);
				}
				if (c1 == 0xD7 && c2 == 0xCD) {
					return new ImgWMF(str);
				}
				throw new IOException("Image is not a recognized imageformat.");
			} finally {
				if (str != null) {
					str.Close();
				}
			}
		}
    
		/// <summary>
		/// Gets an instance of an Image.
		/// </summary>
		/// <param name="url">an URL</param>
		/// <returns>an object of type Gif, Jpeg or Png</returns>
		public static Image getInstance(Uri url) {
			Stream istr = null;
			try {
				WebRequest w = WebRequest.Create(url);
				istr = w.GetResponse().GetResponseStream();
				int c1 = istr.ReadByte();
				int c2 = istr.ReadByte();
				istr.Close();
            
				istr = null;
				if (c1 == 'G' && c2 == 'I') {
					return new Gif(url);
				}
				if (c1 == 0xFF && c2 == 0xD8) {
					return new Jpeg(url);
				}
				if (c1 == Png.PNGID[0] && c2 == Png.PNGID[1]) {
					return new Png(url);
				}
				if (c1 == 0xD7 && c2 == 0xCD) {
					return new ImgWMF(url);
				}
				throw new IOException(url.ToString() + " is not a recognized imageformat.");
			} finally {
				if (istr != null) {
					istr.Close();
				}
			}
		}

		/// <summary>
		/// Converts a .NET image to a native(PNG, JPG, GIF, WMF) image
		/// </summary>
		/// <param name="image"></param>
		/// <param name="?"></param>
		/// <returns></returns>
		public static Image getInstance(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format) {
			MemoryStream ms = new MemoryStream();
			image.Save(ms, format);
			ms.Seek(0, SeekOrigin.Begin);
			return getInstance(ms);
		}
    
		/// <summary>
		/// Gets an instance of an Image from a System.Drwaing.Image.
		/// </summary>
		/// <param name="image">the System.Drawing.Image to convert</param>
		/// <param name="color">
		/// if different from null the transparency
		/// pixels are replaced by this color
		/// </param>
		/// <param name="forceBW">if true the image is treated as black and white</param>
		/// <returns>an object of type ImgRaw</returns>
		public static Image getInstance(System.Drawing.Image image, object color, bool forceBW) {
			System.Drawing.Bitmap bm = (System.Drawing.Bitmap)image;
			int w = bm.Width;
			int h = bm.Height;
			if (forceBW) {
				int byteWidth = (w / 8) + ((w & 7) != 0 ? 1 : 0);
				byte[] pixelsByte = new byte[byteWidth * h];
            
				int index = 0;
				int size = h * w;
				int transColor = 1;
				if (color != null) {
					System.Drawing.Color c = (System.Drawing.Color)color;
					transColor = (c.R + c.G + c.B < 384) ? 0 : 1;
				}
				int[] transparency = null;
				int cbyte = 0x80;
				int wMarker = 0;
				int currByte = 0;
				if (color != null) {
					for (int j = 0; j < h; j++) {
						for (int i = 0; i < w; i++) {
							int alpha = bm.GetPixel(i, j).A;
							if (alpha < 250) {
								if (transColor == 1)
									currByte |= cbyte;
							}
							else {
								if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
									currByte |= cbyte;
							}
							cbyte >>= 1;
							if (cbyte == 0 || wMarker + 1 >= w) {
								pixelsByte[index++] = (byte)currByte;
								cbyte = 0x80;
								currByte = 0;
							}
							++wMarker;
							if (wMarker >= w)
								wMarker = 0;
						}
					}
				}
				else {
					for (int j = 0; j < h; j++) {
						for (int i = 0; i < w; i++) {
							if (transparency == null) {
								int alpha = bm.GetPixel(i, j).A;
								if (alpha == 0) {
									transparency = new int[2];
									transparency[0] = transparency[1] = ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0) ? 1 : 0;
								}
							}
							if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
								currByte |= cbyte;
							cbyte >>= 1;
							if (cbyte == 0 || wMarker + 1 >= w) {
								pixelsByte[index++] = (byte)currByte;
								cbyte = 0x80;
								currByte = 0;
							}
							++wMarker;
							if (wMarker >= w)
								wMarker = 0;
						}
					}
				}
				return Image.getInstance(w, h, 1, 1, pixelsByte, transparency);
			}
			else {
				byte[] pixelsByte = new byte[w * h * 3];
            
				int index = 0;
				int size = h * w;
				int red = 255;
				int green = 255;
				int blue = 255;
				if (color != null) {
					red = ((System.Drawing.Color)color).R;
					green = ((System.Drawing.Color)color).G;
					blue = ((System.Drawing.Color)color).B;
				}
				int[] transparency = null;
				if (color != null) {
					for (int j = 0; j < h; j++) {
						for (int i = 0; i < w; i++) {
							int alpha = (bm.GetPixel(i, j).ToArgb() >> 24) & 0xff;
							if (alpha < 250) {
								pixelsByte[index++] = (byte) red;
								pixelsByte[index++] = (byte) green;
								pixelsByte[index++] = (byte) blue;
							}
							else {
								pixelsByte[index++] = (byte) ((bm.GetPixel(i, j).ToArgb() >> 16) & 0xff);
								pixelsByte[index++] = (byte) ((bm.GetPixel(i, j).ToArgb() >> 8) & 0xff);
								pixelsByte[index++] = (byte) ((bm.GetPixel(i, j).ToArgb()) & 0xff);
							}
						}
					}
				}
				else {
					for (int j = 0; j < h; j++) {
						for (int i = 0; i < w; i++) {
							if (transparency == null) {
								int alpha = (bm.GetPixel(i, j).ToArgb() >> 24) & 0xff;
								if (alpha == 0) {
									transparency = new int[6];
									transparency[0] = transparency[1] = (bm.GetPixel(i, j).ToArgb() >> 16) & 0xff;
									transparency[2] = transparency[3] = (bm.GetPixel(i, j).ToArgb() >> 8) & 0xff;
									transparency[4] = transparency[5] = bm.GetPixel(i, j).ToArgb() & 0xff;
								}
							}
							pixelsByte[index++] = (byte) ((bm.GetPixel(i, j).ToArgb() >> 16) & 0xff);
							pixelsByte[index++] = (byte) ((bm.GetPixel(i, j).ToArgb() >> 8) & 0xff);
							pixelsByte[index++] = (byte) ((bm.GetPixel(i, j)).ToArgb() & 0xff);
						}
					}
				}
				return Image.getInstance(w, h, 3, 8, pixelsByte, transparency);
			}
		}
    
		/// <summary>
		/// Gets an instance of an Image from a System.Drawing.Image.
		/// </summary>
		/// <param name="image">the System.Drawing.Image to convert</param>
		/// <param name="color">
		/// if different from null the transparency
		/// pixels are replaced by this color
		/// </param>
		/// <returns>an object of type ImgRaw</returns>
		public static Image getInstance(System.Drawing.Image image, Color color) {
			return Image.getInstance(image, color, false);
		}
    
		/// <summary>
		/// Gets an instance of an Image.
		/// </summary>
		/// <param name="filename">a filename</param>
		/// <returns>an object of type Gif, Jpeg or Png</returns>
		public static Image getInstance(string filename) {
			return getInstance(toURL(filename));
		}
    
		/// <summary>
		/// Gets an instance of an Image.
		/// </summary>
		/// <param name="img">a byte array</param>
		/// <returns>an object of type Gif, Jpeg or Png</returns>
		public static Image getInstance(byte[] img) {
			Stream istr = null;
			try {
				istr = new MemoryStream(img);
				int c1 = istr.ReadByte();
				int c2 = istr.ReadByte();
				istr.Close();
				istr = null;
				if (c1 == 'G' && c2 == 'I') {
					return new Gif(img);
				}
				if (c1 == 0xFF && c2 == 0xD8) {
					return new Jpeg(img);
				}
				if (c1 == Png.PNGID[0] && c2 == Png.PNGID[1]) {
					return new Png(img);
				}
				if (c1 == 0xD7 && c2 == 0xCD) {
					return new ImgWMF(img);
				}
				throw new IOException("Could not find a recognized imageformat.");
			}
			finally {
				if (istr != null) {
					istr.Close();
				}
			}
		}
    
		/// <summary>
		/// Gets an instance of an Image in raw mode.
		/// </summary>
		/// <param name="width">the width of the image in pixels</param>
		/// <param name="height">the height of the image in pixels</param>
		/// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
		/// <param name="bpc">bits per component</param>
		/// <param name="data">the image data</param>
		/// <returns>an object of type ImgRaw</returns>
		public static Image getInstance(int width, int height, int components, int bpc, byte[] data) {
			return Image.getInstance(width, height, components, bpc, data, null);
		}
    
		/// <summary>
		/// Gets an instance of an Image in raw mode.
		/// </summary>
		/// <param name="template"></param>
		/// <returns></returns>
		public static Image getInstance(PdfTemplate template) {
			return new ImgTemplate(template);
		}
    
		/// <summary>
		/// Gets an instance of an Image in raw mode.
		/// </summary>
		/// <param name="width">the width of the image in pixels</param>
		/// <param name="height">the height of the image in pixels</param>
		/// <param name="reverseBits"></param>
		/// <param name="typeCCITT"></param>
		/// <param name="parameters"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Image getInstance(int width, int height, bool reverseBits, int typeCCITT, int parameters, byte[] data) {
			return Image.getInstance(width, height, reverseBits, typeCCITT, parameters, data, null);
		}
    
		/// <summary>
		/// 
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="reverseBits"></param>
		/// <param name="typeCCITT"></param>
		/// <param name="parameters"></param>
		/// <param name="data"></param>
		/// <param name="transparency"></param>
		/// <returns></returns>
		public static Image getInstance(int width, int height, bool reverseBits, int typeCCITT, int parameters, byte[] data, int[] transparency) {
			if (transparency != null && transparency.Length != 2)
				throw new BadElementException("Transparency length must be equal to 2 with CCITT images");
			Image img = new ImgCCITT(width, height, reverseBits, typeCCITT, parameters, data);
			img.transparency = transparency;
			return img;
		}

		/// <summary>
		/// Gets an instance of an Image in raw mode.
		/// </summary>
		/// <param name="width">the width of the image in pixels</param>
		/// <param name="height">the height of the image in pixels</param>
		/// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
		/// <param name="bpc">bits per component</param>
		/// <param name="data">the image data</param>
		/// <param name="transparency">
		/// transparency information in the Mask format of the
		/// image dictionary
		/// </param>
		/// <returns>an object of type ImgRaw</returns>
		public static Image getInstance(int width, int height, int components, int bpc, byte[] data, int[] transparency) {
			if (transparency != null && transparency.Length != components * 2)
				throw new BadElementException("Transparency length must be equal to (componentes * 2)");
			if (components == 1 && bpc == 1) {
				byte[] g4 = CCITTG4Encoder.compress(data, width, height);
				return Image.getInstance(width, height, false, Element.CCITTG4, Element.CCITT_BLACKIS1, g4, transparency);
			}
			Image img = new ImgRaw(width, height, components, bpc, data);
			img.transparency = transparency;
			return img;
		}
    
		/// <summary>
		/// Returns an Image that has been constructed taking in account
		/// the value of some attributes.
		/// </summary>
		/// <param name="attributes">Some attributes</param>
		/// <returns>an Image</returns>
		public static Image getInstance(Properties attributes) {
			string value = attributes.Remove(ElementTags.URL);
			if (value == null) throw new Exception("The URL of the image is missing.");
			Image image = Image.getInstance(value);
			int align = Element.ALIGN_LEFT;
			if ((value = attributes.Remove(ElementTags.ALIGN)) != null) {
				if (ElementTags.ALIGN_LEFT.ToLower().Equals(value)) align |= Image.LEFT;
				else if (ElementTags.ALIGN_RIGHT.ToLower().Equals(value)) align |= Image.RIGHT;
				else if (ElementTags.ALIGN_MIDDLE.ToLower().Equals(value)) align |= Image.MIDDLE;
			}
			if ((value = attributes.Remove(ElementTags.UNDERLYING)) != null) {
				if (bool.Parse(value)) align |= Image.UNDERLYING;
			}
			if ((value = attributes.Remove(ElementTags.TEXTWRAP)) != null) {
				if (bool.Parse(value)) align |= Image.TEXTWRAP;
			}
			image.alignment = align;
			if ((value = attributes.Remove(ElementTags.ALT)) != null) {
				image.Alt = value;
			}
			string x;
			string y;
			if (((x = attributes.Remove(ElementTags.ABSOLUTEX)) != null)
				&& ((y = attributes.Remove(ElementTags.ABSOLUTEY)) != null)) {
				image.setAbsolutePosition(float.Parse(x), float.Parse(y));
			}
			if ((value = attributes.Remove(ElementTags.PLAINWIDTH)) != null) {
				image.scaleAbsoluteWidth(float.Parse(value));
			}
			if ((value = attributes.Remove(ElementTags.PLAINHEIGHT)) != null) {
				image.scaleAbsoluteHeight(float.Parse(value));
			}
			if ((value = attributes.Remove(ElementTags.ROTATION)) != null) {
				image.setRotation(float.Parse(value));
			}
			if (attributes.Count > 0) image.MarkupAttributes = attributes;
			return image;
		}
    
		// methods to set information
    
		/// <summary>
		/// Sets the absolute position of the Image.
		/// </summary>
		/// <param name="absoluteX"></param>
		/// <param name="absoluteY"></param>
		public void setAbsolutePosition(float absoluteX, float absoluteY) {
			this.absoluteX = absoluteX;
			this.absoluteY = absoluteY;
		}
    
		/// <summary>
		/// Scale the image to an absolute width and an absolute height.
		/// </summary>
		/// <param name="newWidth">the new width</param>
		/// <param name="newHeight">the new height</param>
		public void scaleAbsolute(float newWidth, float newHeight) {
			plainWidth = newWidth;
			plainHeight = newHeight;
			float[] matrix = this.Matrix;
			scaledWidth = matrix[DX] - matrix[CX];
			scaledHeight = matrix[DY] - matrix[CY];
		}
    
		/// <summary>
		/// Scale the image to an absolute width.
		/// </summary>
		/// <param name="newWidth">the new width</param>
		public void scaleAbsoluteWidth(float newWidth) {
			plainWidth = newWidth;
			float[] matrix = this.Matrix;
			scaledWidth = matrix[DX] - matrix[CX];
			scaledHeight = matrix[DY] - matrix[CY];
		}
    
		/// <summary>
		/// Scale the image to an absolute height.
		/// </summary>
		/// <param name="newHeight">the new height</param>
		public void scaleAbsoluteHeight(float newHeight) {
			plainHeight = newHeight;
			float[] matrix = Matrix;
			scaledWidth = matrix[DX] - matrix[CX];
			scaledHeight = matrix[DY] - matrix[CY];
		}
    
		/// <summary>
		/// Scale the image to a certain percentage.
		/// </summary>
		/// <param name="percent">the scaling percentage</param>
		public void scalePercent(float percent) {
			scalePercent(percent, percent);
		}
    
		/// <summary>
		/// Scale the width and height of an image to a certain percentage.
		/// </summary>
		/// <param name="percentX">the scaling percentage of the width</param>
		/// <param name="percentY">the scaling percentage of the height</param>
		public void scalePercent(float percentX, float percentY) {
			plainWidth = (this.Width * percentX) / 100f;
			plainHeight = (this.Height * percentY) / 100f;
			float[] matrix = Matrix;
			scaledWidth = matrix[DX] - matrix[CX];
			scaledHeight = matrix[DY] - matrix[CY];
		}
    
		/// <summary>
		/// Scales the image so that it fits a certain width and height.
		/// </summary>
		/// <param name="fitWidth">the width to fit</param>
		/// <param name="fitHeight">the height to fit</param>
		public void scaleToFit(float fitWidth, float fitHeight) {
			float percentX = (fitWidth * 100) / this.Width;
			float percentY = (fitHeight * 100) / this.Height;
			scalePercent(percentX < percentY ? percentX : percentY);
		}
    
		/// <summary>
		/// Sets the rotation of the image in radians.
		/// </summary>
		/// <param name="r">rotation in radians</param>
		public void setRotation(float r) {
			double d=Math.PI;                  //__IDS__
			rotation = (float)(r % (2.0 * d)); //__IDS__
			if (rotation < 0) {
				rotation += (float)(2.0 * d);           //__IDS__
			}
			float[] matrix = Matrix;
			scaledWidth = matrix[DX] - matrix[CX];
			scaledHeight = matrix[DY] - matrix[CY];
		}
    
		/// <summary>
		/// Sets the rotation of the image in degrees.
		/// </summary>
		/// <param name="deg">rotation in degrees</param>
		public void setRotationDegrees(float deg) {
			double d=Math.PI;                  //__IDS__
			setRotation(deg / 180 * (float)d); //__IDS__
		}
    
		/// <summary>
		/// Get/set the annotation.
		/// </summary>
		/// <value>the Annotation</value>
		public Annotation Annotation {
			get {
				return annotation;
			}

			set {
				this.annotation = value;
			}
		}
    
		// methods to retrieve information
    
		/// <summary>
		/// Gets the bpc for the image.
		/// </summary>
		/// <remarks>
		/// this only makes sense for Images of the type RawImage.
		/// </remarks>
		/// <value>a bpc value</value>
		public int Bpc {
			get {
				return bpc;
			}
		}
    
		/// <summary>
		/// Gets the raw data for the image.
		/// </summary>
		/// <remarks>
		/// this only makes sense for Images of the type RawImage.
		/// </remarks>
		/// <value>the raw data</value>
		public byte[] RawData {
			get {
				return rawData;
			}
		}
    
		/// <summary>
		/// Get/set the template to be used as an image.
		/// </summary>
		/// <remarks>
		/// this only makes sense for Images of the type ImgTemplate.
		/// </remarks>
		/// <value>the template</value>
		public PdfTemplate TemplateData {
			get {
				return template;
			}

			set {
				this.template = value;
			}
		}
    
		/// <summary>
		/// Checks if the Images has to be added at an absolute position.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool hasAbsolutePosition() {
			return !float.IsNaN(absoluteY);
		}
    
		/// <summary>
		/// Checks if the Images has to be added at an absolute X position.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool hasAbsoluteX() {
			return !float.IsNaN(absoluteX);
		}
    
		/// <summary>
		/// Returns the absolute X position.
		/// </summary>
		/// <value>a position</value>
		public float AbsoluteX {
			get {
				return absoluteX;
			}
		}
    
		/// <summary>
		/// Returns the absolute Y position.
		/// </summary>
		/// <value>a position</value>
		public float AbsoluteY {
			get {
				return absoluteY;
			}
		}
    
		/// <summary>
		/// Returns the type.
		/// </summary>
		/// <value>a type</value>
		public override int Type {
			get {
				return type;
			}
		}
    
		/// <summary>
		/// Returns true if the image is a Gif-object.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isGif() {
			return type == Element.GIF;
		}
    
		/// <summary>
		/// Returns true if the image is a Jpeg-object.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isJpeg() {
			return type == Element.JPEG;
		}
    
		/// <summary>
		/// Returns true if the image is a Png-object.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isPng() {
			return type == Element.PNG;
		}
    
		/// <summary>
		/// Returns true if the image is a ImgRaw-object.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isImgRaw() {
			return type == Element.IMGRAW;
		}

		/// <summary>
		/// Returns true if the image is an ImgTemplate-object.
		/// </summary>
		/// <returns>a boolean</returns>
		public bool isImgTemplate() {
			return type == Element.IMGTEMPLATE;
		}
    
		/// <summary>
		/// Gets the string-representation of the reference to the image.
		/// </summary>
		/// <value>a string</value>
		public Uri Url {
			get {
				return url;
			}
		}
    
		/// <summary>
		/// Get/set the alignment for the image.
		/// </summary>
		/// <value>a value</value>
		public int Alignment {
			get {
				return alignment;
			}

			set {
				this.alignment = value;
			}
		}
    
		/// <summary>
		/// Get/set the alternative text for the image.
		/// </summary>
		/// <value>a string</value>
		public string Alt {
			get {
				return alt;
			}

			set {
				this.alt = value;
			}
		}
    
		/// <summary>
		/// Gets the scaled width of the image.
		/// </summary>
		/// <value>a value</value>
		public float ScaledWidth {
			get {
				return scaledWidth;
			}
		}
    
		/// <summary>
		/// Gets the scaled height of the image.
		/// </summary>
		/// <value>a value</value>
		public float ScaledHeight {
			get {
				return scaledHeight;
			}
		}
    
		/// <summary>
		/// Gets the colorspace for the image.
		/// </summary>
		/// <remarks>
		/// this only makes sense for Images of the type Jpeg.
		/// </remarks>
		/// <value>a colorspace value</value>
		public int Colorspace {
			get {
				return colorspace;
			}
		}
    
		/// <summary>
		/// Returns the transformation matrix of the image.
		/// </summary>
		/// <value>an array [AX, AY, BX, BY, CX, CY, DX, DY]</value>
		public float[] Matrix {
			get {
				float[] matrix = new float[8];
				float cosX = (float)Math.Cos(rotation);
				float sinX = (float)Math.Sin(rotation);
				matrix[AX] = plainWidth * cosX;
				matrix[AY] = plainWidth * sinX;
				matrix[BX] = (- plainHeight) * sinX;
				matrix[BY] = plainHeight * cosX;
				if (rotation < Math.PI / 2f) {
					matrix[CX] = matrix[BX];
					matrix[CY] = 0;
					matrix[DX] = matrix[AX];
					matrix[DY] = matrix[AY] + matrix[BY];
				}
				else if (rotation < Math.PI) {
					matrix[CX] = matrix[AX] + matrix[BX];
					matrix[CY] = matrix[BY];
					matrix[DX] = 0;
					matrix[DY] = matrix[AY];
				}
				else if (rotation < Math.PI * 1.5f) {
					matrix[CX] = matrix[AX];
					matrix[CY] = matrix[AY] + matrix[BY];
					matrix[DX] = matrix[BX];
					matrix[DY] = 0;
				}
				else {
					matrix[CX] = 0;
					matrix[CY] = matrix[AY];
					matrix[DX] = matrix[AX] + matrix[BX];
					matrix[DY] = matrix[BY];
				}
				return matrix;
			}
		}
    
		/// <summary>
		/// This method is an alternative for the Stream.skip()-method
		/// that doesn't seem to work properly for big values of size.
		/// </summary>
		/// <param name="istr">the stream</param>
		/// <param name="size">the number of bytes to skip</param>
		static public void skip(Stream istr, int size) {
			while (size > 0) {
				byte [] tmp = new Byte[size];
				size -= istr.Read(tmp, 0, size);
			}
		}
    
		/// <summary>
		/// This method makes a valid URL from a given filename.
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		/// <param name="filename">a given filename</param>
		/// <returns>a valid URL</returns>
		public static Uri toURL(string filename) {
			if (filename.StartsWith("file:/") || filename.StartsWith("http://")) {
				return new Uri(filename);
			}
			FileInfo f = new FileInfo(filename);
			string path = f.FullName;
			//			if (File.separatorChar != '/') {
			//				path = path.replace(File.separatorChar, '/');
			//			}
			if (!path.StartsWith("/")) {
				path = "/" + path;
			}
			//			if (!path.EndsWith("/")) {
			//				path = path + "/";
			//			}
			return new Uri("file:///" + path);
		}
    
		/// <summary>
		/// Returns the transparency.
		/// </summary>
		/// <value>the transparency</value>
		public int[] Transparency {
			get {
				return transparency;
			}
		}
    
		/// <summary>
		/// Checks if a given tag corresponds with this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public static bool isTag(string tag) {
			return ElementTags.IMAGE.Equals(tag);
		}
    
		/// <summary>
		/// Gets the plain width of the image.
		/// </summary>
		/// <value>a value</value>
		public float PlainWidth {
			get {
				return plainWidth;
			}
		}
    
		/// <summary>
		/// Gets the plain height of the image.
		/// </summary>
		/// <value>a value</value>
		public float PlainHeight {
			get {
				return plainHeight;
			}
		}
    
		/// <summary>
		/// generates new serial id
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		static protected long getSerialId() {
			++serialId;
			return serialId;
		}
    
		/// <summary>
		/// returns serial id for this object
		/// </summary>
		public long MySerialId {
			get {
				return mySerialId;
			}
		}
    
		/// <summary>
		/// Gets the dots-per-inch in the X direction. Returns 0 if not available.
		/// </summary>
		/// <value>the dots-per-inch in the X direction</value>
		public int DpiX {
			get {
				return dpiX;
			}
		}
    
		/// <summary>
		/// Gets the dots-per-inch in the Y direction. Returns 0 if not available.
		/// </summary>
		/// <value>the dots-per-inch in the Y direction</value>
		public int DpiY {
			get {
				return dpiY;
			}
		}
    
		/// <summary>
		/// Returns true if this Image has the
		/// requisites to be a mask.
		/// </summary>
		/// <returns>true if this Image can be a mask</returns>
		public bool isMaskCandidate() {
			if (type == Element.IMGRAW) {
				if (bpc > 0xff)
					return true;
				return bpc == 1 && colorspace == 1;
			}
			return type == Element.PNG && bpc == 1 && colorspace == 1;
		}
    
		/// <summary>
		/// Make this Image a mask.
		/// </summary>
		public void makeMask() {
			if (!isMaskCandidate())
				throw new DocumentException("This image can not be an image mask.");
			mask = true;
		}
    
		/// <summary>
		/// Get/set the explicit masking.
		/// </summary>
		/// <value>the explicit masking</value>
		public Image ImageMask {
			get {
				return imageMask;
			}

			set {
				if (this.mask)
					throw new DocumentException("An image mask can not contain another image mask.");
				if (!value.mask)
					throw new DocumentException("The image mask is not a mask. Did you do makeMask()?");
				imageMask = value;
			}
		}
    
		/// <summary>
		/// Returns true if this Image is a mask.
		/// </summary>
		/// <returns>true if this Image is a mask</returns>
		public bool isMask() {
			return mask;
		}
    
		/// <summary>
		/// Inverts the meaning of the bits of a mask.
		/// </summary>
		/// <value>true to invert the meaning of the bits of a mask</value>
		public bool InvertMask {
			set {
				this.invertMask = value;
			}
		}
    
		/// <summary>
		/// Returns true if the bits are to be inverted
		/// in the mask.
		/// </summary>
		/// <returns>true if the bits are to be inverted in the mask</returns>
		public bool isInvertMask() {
			return invertMask;
		}
		
		/// <summary>
		/// Returns true if the bits are to be inverted
		/// in the Jpeg
		/// </summary>
		/// <returns>true if the bits are to be inverted in the Jpeg</returns>
		public bool isInvertedJPEG() {
			return invert;
		}
    
		/// <summary>
		/// Getter for property interpolation.
		/// </summary>
		/// <returns>Value of property interpolation.</returns>
		public bool isInterpolation() {
			return interpolation;
		}
    
		/// <summary>
		/// Sets the image interpolation. Image interpolation attempts to
		/// produce a smooth transition between adjacent sample values.
		/// </summary>
		/// <value>New value of property interpolation.</value>
		public bool Interpolation {
			set {
				this.interpolation = value;
			}
		}
    
		/// <summary>
		/// See <see cref="M:iTextSharp.text.IMarkupAttributes.setMarkupAttribute(System.String,System.String)"/>
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <param name="value">attribute value</param>
		public override void setMarkupAttribute(string name, string value) {
			markupAttributes = (markupAttributes == null) ? new Properties() : markupAttributes;
			markupAttributes.Add(name, value);
		}
    
		/// <summary>
		/// See <see cref="M:iTextSharp.text.IMarkupAttributes.getMarkupAttribute(System.String)"/>
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <returns>attribute value</returns>
		public override string getMarkupAttribute(string name) {
			return (markupAttributes == null) ? null : markupAttributes[name];
		}
    
		/// <summary>
		/// See <see cref="P:iTextSharp.text.IMarkupAttributes.MarkupAttributeNames"/>
		/// </summary>
		/// <value>a collection of string attribute names</value>
		public override ICollection MarkupAttributeNames {
			get {
				return Chunk.getKeySet(markupAttributes);
			}
		}
    
		/// <summary>
		/// See <see cref="P:iTextSharp.text.IMarkupAttributes.MarkupAttributes"/>
		/// </summary>
		/// <value>a Properties-object containing all the markupAttributes.</value>
		public override Properties MarkupAttributes {
			get {
				return markupAttributes;
			}
		
			set {
				this.markupAttributes = value;
			}
		}
    
		/** Tags this image with an ICC profile.
		 * @param profile the profile
		 */    
		//    public void tagICC(ICC_Profile profile) {
		//        this.profile = profile;
		//    }
    
		/** Checks is the image has an ICC profile.
		 * @return the ICC profile or null
		 */    
		//    public bool hasICCProfile() {
		//        return (this.profile != null);
		//    }
    
		/** Gets the images ICC profile.
		 * @return the ICC profile
		 */    
		//    public ICC_Profile getICCProfile() {
		//        return profile;
		//    }
	}
}
