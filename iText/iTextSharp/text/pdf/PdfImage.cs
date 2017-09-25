using System;
using System.IO;
using System.Net;

using iTextSharp.text;

/*
 * $Id: PdfImage.cs,v 1.1.1.1 2003/02/04 02:57:28 geraldhenson Exp $
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
 *
 * REMARK:
 * LZW/GIF is covered by a software patent which is owned by Unisys Corporation.
 * Unisys refuses to license this patent for PDF-related use in software
 * even when this software is released for free and may be freely distributed.
 * HOWEVER:
 * This library doesn't compress or decompress data using the LZW
 * algorithm, nor does it create or visualize GIF-images in any way;
 * it only copies parts of an existing GIF file into a PDF file.
 *
 * More information about the GIF format can be found in the following documents:
 * * GRAPHICS INTERCHANGE FORMAT(sm) Version 89a
 *   (c)1987,1988,1989,1990 Copyright CompuServe Incorporated. Columbus, Ohio
 * * LZW and GIF explained
 *   Steve Blackstock
 * * http://mistress.informatik.unibw-muenchen.de/
 *   very special thanks to klee@informatik.unibw-muenchen.de for the algorithm
 *   to extract the LZW data from a GIF.
 */

namespace iTextSharp.text.pdf {

	/**
	 * <CODE>PdfImage</CODE> is a <CODE>PdfStream</CODE> containing an image-<CODE>Dictionary</CODE> and -stream.
	 */

	public class PdfImage : PdfStream {
    
		internal static int TRANSFERSIZE = 4096;
		// membervariables
    
		/** This is the <CODE>PdfName</CODE> of the image. */
		protected PdfName name = null;
    
		// constructor
    
		/**
		 * Constructs a <CODE>PdfImage</CODE>-object.
		 *
		 * @param image the <CODE>Image</CODE>-object
		 * @param name the <CODE>PdfName</CODE> for this image
		 * @throws BadPdfFormatException on error
		 */
    
		public PdfImage(Image image, string name, PdfIndirectReference maskRef) : base() {
			this.name = new PdfName(name);
			put(PdfName.TYPE, PdfName.XOBJECT);
			put(PdfName.SUBTYPE, PdfName.IMAGE);
			put(PdfName.NAME, this.name);
			put(PdfName.WIDTH, new PdfNumber(image.Width));
			put(PdfName.HEIGHT, new PdfNumber(image.Height));
			if (maskRef != null)
				put(PdfName.MASK, maskRef);
			if (image.isMask() && image.isInvertMask())
				put(PdfName.DECODE, new PdfLiteral("[1 0]"));
			if (image.isInterpolation())
				put(PdfName.INTERPOLATE, PdfBoolean.PDFTRUE);
			Stream istr = null;
			try {
            
				// Raw Image data
				if (image.isImgRaw()) {
					// will also have the CCITT parameters
					int colorspace = image.Colorspace;
					int[] transparency = image.Transparency;
					if (transparency != null && !image.isMask() && maskRef == null) {
						string s = "[";
						for (int k = 0; k < transparency.Length; ++k)
							s += transparency[k] + " ";
						s += "]";
						put(PdfName.MASK, new PdfLiteral(s));
					}
					bytes = image.RawData;
					put(PdfName.LENGTH, new PdfNumber(bytes.Length));
					int bpc = image.Bpc;
					if (bpc > 0xff) {
						if (!image.isMask())
							put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
						put(PdfName.BITSPERCOMPONENT, new PdfNumber(1));
						put(PdfName.FILTER, PdfName.CCITTFAXDECODE);
						int k = bpc - Element.CCITTG3_1D;
						PdfDictionary decodeparms = new PdfDictionary();
						if (k != 0)
							decodeparms.put(PdfName.K, new PdfNumber(k));
						if ((colorspace & Element.CCITT_BLACKIS1) != 0)
							decodeparms.put(PdfName.BLACKIS1, PdfBoolean.PDFTRUE);
						if ((colorspace & Element.CCITT_ENCODEDBYTEALIGN) != 0)
							decodeparms.put(PdfName.ENCODEDBYTEALIGN, PdfBoolean.PDFTRUE);
						if ((colorspace & Element.CCITT_ENDOFLINE) != 0)
							decodeparms.put(PdfName.ENDOFLINE, PdfBoolean.PDFTRUE);
						if ((colorspace & Element.CCITT_ENDOFBLOCK) != 0)
							decodeparms.put(PdfName.ENDOFBLOCK, PdfBoolean.PDFFALSE);
						decodeparms.put(PdfName.COLUMNS, new PdfNumber(image.Width));
						decodeparms.put(PdfName.ROWS, new PdfNumber(image.Height));
						put(PdfName.DECODEPARMS, decodeparms);
					}
					else {
						if (!image.isMask()) {
							switch(colorspace) {
								case 1:
									put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
									break;
								case 3:
									put(PdfName.COLORSPACE, PdfName.DEVICERGB);
									break;
								case 4:
								default:
									put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
									break;
							}
						}
						put(PdfName.BITSPERCOMPONENT, new PdfNumber(image.Bpc));
						try {
							flateCompress();
						}
						catch(PdfException pe) {
							throw pe;
						}
					}
					if (image.isMask())
						put(PdfName.IMAGEMASK, PdfBoolean.PDFTRUE);
					return;
				}
            
				// GIF, JPEG or PNG
				string errorID;
				if (image.RawData == null){
					WebRequest w = WebRequest.Create(image.Url);
					istr = w.GetResponse().GetResponseStream();
					errorID = image.Url.ToString();
				}
				else{
					istr = new MemoryStream(image.RawData);
					errorID = "Byte array";
				}
				streamBytes = new MemoryStream();
				int i = 0;
				switch(image.Type) {
					case Element.PNG:
						put(PdfName.FILTER, PdfName.FLATEDECODE);
						for (int j = 0; j < Png.PNGID.Length; j++) {
							if (Png.PNGID[j] != istr.ReadByte()) {
								throw new BadPdfFormatException(errorID + " is not a PNG file.");
							}
						}
						int colorType = 0;
						while (true) {
							int len = Png.getInt(istr);
							string marker = Png.getstring(istr);
							if (Png.IDAT.Equals(marker)) {
								transferBytes(istr, streamBytes, len);
								Png.getInt(istr);
							}
							else if (Png.tRNS.Equals(marker) && !image.isMask() && maskRef == null) {
								switch (colorType) {
									case 0:
										if (len >= 2) {
											len -= 2;
											int gray = Png.getWord(istr);
											put(PdfName.MASK, new PdfLiteral("["+gray+" "+gray+"]"));
										}
										break;
									case 2:
										if (len >= 6) {
											len -= 6;
											int red = Png.getWord(istr);
											int green = Png.getWord(istr);
											int blue = Png.getWord(istr);
											put(PdfName.MASK, new PdfLiteral("["+red+" "+red+" "+green+" "+green+" "+blue+" "+blue+"]"));
										}
										break;
									case 3:
										if (len > 0) {
											int idx = 0;
											while (len-- != 0) {
												if (istr.ReadByte() == 0) {
													put(PdfName.MASK, new PdfLiteral("["+idx+" "+idx+"]"));
													break;
												}
												++idx;
											}
										}
										break;
								}
								Image.skip(istr, len + 4);
							}
							else if (Png.IHDR.Equals(marker)) {
								int w = Png.getInt(istr);
								int h = Png.getInt(istr);
                            
								int bitDepth = istr.ReadByte();
								if (bitDepth == 16) {
									throw new BadPdfFormatException(errorID + " Bit depth 16 is not suported.");
								}
								put(PdfName.BITSPERCOMPONENT, new PdfNumber(bitDepth));
                            
								colorType = istr.ReadByte();
								if (! (colorType == 0 || colorType == 2 || colorType == 3)) {
									throw new BadPdfFormatException(errorID + " Colortype " + colorType + " is not suported.");
								}
								if (colorType == 0) {
									put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
								}
								else if (colorType == 2) {
									put(PdfName.COLORSPACE, PdfName.DEVICERGB);
								}
                            
								int compressionMethod = istr.ReadByte();
								int filterMethod = istr.ReadByte();
                            
								int interlaceMethod = istr.ReadByte();
								if (interlaceMethod != 0) {
									throw new BadPdfFormatException(errorID + " Interlace method " + interlaceMethod + " is not suported.");
								}
                            
								PdfDictionary decodeparms = new PdfDictionary();
								decodeparms.put(PdfName.BITSPERCOMPONENT, new PdfNumber(bitDepth));
								decodeparms.put(PdfName.PREDICTOR, new PdfNumber(15));
								decodeparms.put(PdfName.COLUMNS, new PdfNumber(w));
								decodeparms.put(PdfName.COLORS, new PdfNumber((colorType == 2) ? 3: 1));
								put(PdfName.DECODEPARMS, decodeparms);
                            
								Png.getInt(istr);
							}
							else if (Png.PLTE.Equals(marker)) {
								if (colorType == 3) {
									PdfArray colorspace = new PdfArray();
									colorspace.Add(PdfName.INDEXED);
									colorspace.Add(PdfName.DEVICERGB);
									colorspace.Add(new PdfNumber(len / 3 - 1));
									ByteBuffer colortable = new ByteBuffer();
									while ((len--) > 0) {
										colortable.Append_i(istr.ReadByte());
									}
									colorspace.Add(new PdfString(colortable.toByteArray()));
									put(PdfName.COLORSPACE, colorspace);
									Png.getInt(istr);
								}
								else {
									Image.skip(istr, len + 4);
								}
							}
							else if (Png.IEND.Equals(marker)) {
								break;
							}
							else {
								for (int j = -4; j < len; j++) {
									istr.ReadByte();
								}
							}
                        
						}
						break;
					case Element.JPEG:
						put(PdfName.FILTER, PdfName.DCTDECODE);
					switch(image.Colorspace) {
						case 1:
							put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
							break;
						case 3:
							put(PdfName.COLORSPACE, PdfName.DEVICERGB);
							break;
						default:
							put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
							if (image.isInvertedJPEG()) {
								put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
							}
							break;
					}
						put(PdfName.BITSPERCOMPONENT, new PdfNumber(8));
						if (image.RawData != null){
							bytes = image.RawData;
							streamBytes = null;
							put(PdfName.LENGTH, new PdfNumber(bytes.Length));
							return;
						}
						transferBytes(istr, streamBytes, -1);
						break;
					case Element.GIF:
						// HEADER + INFO + COLORTABLE
                    
						// Byte 0-2: header
						// checks if the file really is a GIF-file
						if (istr.ReadByte() != 'G' || istr.ReadByte() != 'I' || istr.ReadByte() != 'F') {
							throw new BadPdfFormatException(errorID + " is not a GIF-file (GIF header not found).");
						}
                    
						put(PdfName.FILTER, PdfName.LZWDECODE);
                    
						PdfDictionary decodeparams = new PdfDictionary();
						decodeparams.put(PdfName.EARLYCHANGE, new PdfNumber(0));
						put(PdfName.DECODEPARMS, decodeparams);
                    
						PdfArray cspace = new PdfArray();
						cspace.Add(PdfName.INDEXED);
						cspace.Add(PdfName.DEVICERGB);
						// Byte 3-5: version
						// Byte 6-7: logical screen width
						// Byte 8-9: logical screen height
						// Byte 10: Packed Fields
						for (int j = 0; j < 8; j++) {
							i = istr.ReadByte();
						}
						// Byte 10: bit 1: Global Color Table Flag
						if ((i & 0x80) == 0) {
							throw new BadPdfFormatException(errorID + " is not a supported GIF-file (there is no global color table present).");
						}
						// Byte 10: bit 6-8: Size of Global Color Table
						int nColors = 1 << ((i & 7) + 1);
						cspace.Add(new PdfNumber(nColors - 1));
						// Byte 11: Background color index
						istr.ReadByte();
						// Byte 12: Pixel aspect ratio
						istr.ReadByte();
						// Byte 13-...: Global color table
						ByteBuffer ctable = new ByteBuffer();
						for (int j = 0; j < nColors; j++) {
							ctable.Append_i(istr.ReadByte());	// red
							ctable.Append_i(istr.ReadByte());	// green
							ctable.Append_i(istr.ReadByte());	// blue
						}
						cspace.Add(new PdfString(ctable.toByteArray()));
						put(PdfName.COLORSPACE, cspace);
						put(PdfName.BITSPERCOMPONENT, new PdfNumber(8));
                    
						// IMAGE DESCRIPTOR
                    
						// Byte 0: Image separator
						// only simple gif files with image immediate following global color table are supported
						// 0x2c is a fixed value for the image separator
						if (istr.ReadByte() != 0x2c) {
							throw new BadPdfFormatException(errorID + " is not a supported GIF-file (the image separator '0x2c' is not found after reading the color table).");
						}
						// Byte 1-2: Image Left Position
						// Byte 3-4: Image Top Position
						// Byte 5-6: Image Width
						// Byte 7-8: Image Height
						// ignore position and size
						for (int j = 0; j < 8; j++) {
							istr.ReadByte();
						}
						// Byte 9: Packed Fields
						// Byte 9: bit 1: Local Color Table Flag
						// Byte 9: bit 2: Interlace Flag
						if ((istr.ReadByte() & 0xc0) > 0) {
							throw new BadPdfFormatException(errorID + " is not a supported GIF-file (interlaced gifs or gifs using local color table can't be inserted).");
						}
                    
						// Byte 10: LZW initial code
						if (istr.ReadByte() != 0x08) {
							throw new BadPdfFormatException(errorID + " is not a supported GIF-file (initial LZW code not supported).");
						}
						// Read the Image Data
						int code = 0;
						int codelength = 9;
						int tablelength = 257;
						int bitsread = 0;
						int bitstowrite = 0;
						int bitsdone = 0;
						int bitsleft = 23;
						int bytesdone = 0;
						int bytesread = 0;
						int byteswritten = 0;
						// read the size of the first Data Block
						int size = istr.ReadByte();
						// Check if there is any data in the GIF
						if (size < 1) {
							throw new BadPdfFormatException(errorID + " is not a supported GIF-file. (no image data found).");
						}
						// if possible, we read the first 24 bits of data
						size--; bytesread++; bitsread = istr.ReadByte();
						if (size > 0) {
							size--; bytesread++; bitsread += (istr.ReadByte() << 8);
							if (size > 0) {
								size--; bytesread++; bitsread += (istr.ReadByte() << 16);
							}
						}
						while (bytesread > byteswritten) {
							tablelength++;
							// we extract a code with length=codelength
							code = (bitsread >> bitsdone) & ((1 << codelength) - 1);
							// we delete the bytesdone in bitsread and append the next byte(s)
							bytesdone = (bitsdone + codelength) / 8;
							bitsdone = (bitsdone + codelength) % 8;
							while (bytesdone > 0) {
								bytesdone--;
								bitsread = (bitsread >> 8);
								if (size > 0) {
									size--; bytesread++; bitsread += (istr.ReadByte() << 16);
								}
								else {
									size = istr.ReadByte();
									if (size > 0) {
										size--; bytesread++; bitsread += (istr.ReadByte() << 16);
									}
								}
							}
							// we package all the bits that are done into bytes and write them to the stream
							bitstowrite += (code << (bitsleft - codelength + 1));
							bitsleft -= codelength;
							while (bitsleft < 16) {
								streamBytes.WriteByte((byte)(bitstowrite >> 16));
								byteswritten++;
								bitstowrite = (bitstowrite & 0xFFFF) << 8;
								bitsleft += 8;
							}
							if (code == 256) {
								codelength = 9;
								tablelength = 257;
							}
							if (code == 257) {
								break;
							}
							if (tablelength == (1 << codelength)) {
								codelength++;
							}
						}
						if (bytesread - byteswritten > 2) {
							throw new BadPdfFormatException(errorID + " is not a supported GIF-file (unexpected end of data block).");
						}
						break;
					default:
						throw new BadPdfFormatException(errorID + " is an unknown Image format.");
				}
				put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
			}
			catch(IOException ioe) {
				throw new BadPdfFormatException(ioe.Message);
			}
			finally {
				if (istr != null) {
					try{
						istr.Close();
					}
					catch (Exception ee) {
						ee.GetType();
						// empty on purpose
					}
				}
			}
		}
    
		/**
		 * Returns the <CODE>PdfName</CODE> of the image.
		 *
		 * @return		the name
		 */
    
		public PdfName Name {
			get {
				return name;
			}
		}
    
		internal static void transferBytes(Stream istr, Stream ostr, int len) {
			byte[] buffer = new byte[TRANSFERSIZE];
			if (len < 0)
				len = 0x7ffffff;
			int size;
			while (len != 0) {
				size = istr.Read(buffer, 0, Math.Min(len, TRANSFERSIZE));
				if (size <= 0)
					return;
				ostr.Write(buffer, 0, size);
				len -= size;
			}
		}
	}
}