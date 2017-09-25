using System;
using System.IO;
using System.Collections;

/*
 * Copyright (c) 2001 Sun Microsystems, Inc. All Rights Reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * -Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 *
 * -Redistribution in binary form must reproduct the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 *
 * Neither the name of Sun Microsystems, Inc. or the names of contributors may
 * be used to endorse or promote products derived from this software without
 * specific prior written permission.
 *
 * This software is provided "AS IS," without a warranty of any kind. ALL
 * EXPRESS OR IMPLIED CONDITIONS, REPRESENTATIONS AND WARRANTIES, INCLUDING ANY
 * IMPLIED WARRANTY OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE OR
 * NON-INFRINGEMENT, ARE HEREBY EXCLUDED. SUN AND ITS LICENSORS SHALL NOT BE
 * LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING
 * OR DISTRIBUTING THE SOFTWARE OR ITS DERIVATIVES. IN NO EVENT WILL SUN OR ITS
 * LICENSORS BE LIABLE FOR ANY LOST REVENUE, PROFIT OR DATA, OR FOR DIRECT,
 * INDIRECT, SPECIAL, CONSEQUENTIAL, INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER
 * CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, ARISING OUT OF THE USE OF
 * OR INABILITY TO USE SOFTWARE, EVEN IF SUN HAS BEEN ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGES.
 *
 * You acknowledge that Software is not designed,licensed or intended for use in
 * the design, construction, operation or maintenance of any nuclear facility.
 *
 * Adapted from the JAI codecs TIFFLZWDecoder.
 */
namespace iTextSharp.text.pdf {
	/**
	 * A class for performing LZW decoding.
	 *
	 *
	 */
	public class LZWDecoder {
    
		byte[][] stringTable;
		byte[] data = null;
		Stream uncompData;
		int tableIndex, bitsToGet = 9;
		int bytePointer;
		int nextData = 0;
		int nextBits = 0;
    
		internal int[] andTable = {
							 511,
							 1023,
							 2047,
							 4095
						 };
    
		public LZWDecoder() {
		}
    
		/**
		 * Method to decode LZW compressed data.
		 *
		 * @param data            The compressed data.
		 * @param uncompData      Array to return the uncompressed data in.
		 */
		public void decode(byte[] data, Stream uncompData) {
        
			if(data[0] == (byte)0x00 && data[1] == (byte)0x01) {
				throw new RuntimeException("LZW flavour not supported.");
			}
        
			initializestringTable();
        
			this.data = data;
			this.uncompData = uncompData;
        
			// Initialize pointers
			bytePointer = 0;
        
			nextData = 0;
			nextBits = 0;
        
			int code, oldCode = 0;
			byte[] str;
        
			while ((code = this.NextCode) != 257) {
            
				if (code == 256) {
                
					initializestringTable();
					code = NextCode;
                
					if (code == 257) {
						break;
					}
                
					writestring(stringTable[code]);
					oldCode = code;
                
				} else {
                
					if (code < tableIndex) {
                    
						str = stringTable[code];
                    
						writestring(str);
						addstringToTable(stringTable[oldCode], str[0]);
						oldCode = code;
                    
					} else {
                    
						str = stringTable[oldCode];
						str = composestring(str, str[0]);
						writestring(str);
						addstringToTable(str);
						oldCode = code;
					}
				}
			}
		}
    
    
		/**
		 * Initialize the string table.
		 */
		public void initializestringTable() {
        
			stringTable = new byte[4096][];
        
			for (int i=0; i<256; i++) {
				stringTable[i] = new byte[1];
				stringTable[i][0] = (byte)i;
			}
        
			tableIndex = 258;
			bitsToGet = 9;
		}
    
		/**
		 * Write out the string just uncompressed.
		 */
		public void writestring(byte[] str) {
			try {
				new StreamWriter(uncompData).Write(str);
			}
			catch (IOException e) {
				throw e;
			}
		}
    
		/**
		 * Add a new string to the string table.
		 */
		public void addstringToTable(byte[] oldstring, byte newstring) {
			int length = oldstring.Length;
			byte[] str = new byte[length + 1];
			Array.Copy(oldstring, 0, str, 0, length);
			str[length] = newstring;
        
			// Add this new string to the table
			stringTable[tableIndex++] = str;
        
			if (tableIndex == 511) {
				bitsToGet = 10;
			} else if (tableIndex == 1023) {
				bitsToGet = 11;
			} else if (tableIndex == 2047) {
				bitsToGet = 12;
			}
		}
    
		/**
		 * Add a new string to the string table.
		 */
		public void addstringToTable(byte[] str) {
        
			// Add this new string to the table
			stringTable[tableIndex++] = str;
        
			if (tableIndex == 511) {
				bitsToGet = 10;
			} else if (tableIndex == 1023) {
				bitsToGet = 11;
			} else if (tableIndex == 2047) {
				bitsToGet = 12;
			}
		}
    
		/**
		 * Append <code>newstring</code> to the end of <code>oldstring</code>.
		 */
		public byte[] composestring(byte[] oldstring, byte newstring) {
			int length = oldstring.Length;
			byte[] str = new byte[length + 1];
			Array.Copy(oldstring, 0, str, 0, length);
			str[length] = newstring;
        
			return str;
		}
    
		// Returns the next 9, 10, 11 or 12 bits
		public int NextCode {
			get {
				// Attempt to get the next code. The exception is caught to make
				// this robust to cases wherein the EndOfInformation code has been
				// omitted from a strip. Examples of such cases have been observed
				// in practice.
				try {
					nextData = (nextData << 8) | (data[bytePointer++] & 0xff);
					nextBits += 8;
            
					if (nextBits < bitsToGet) {
						nextData = (nextData << 8) | (data[bytePointer++] & 0xff);
						nextBits += 8;
					}
            
					int code =
						(nextData >> (nextBits - bitsToGet)) & andTable[bitsToGet-9];
					nextBits -= bitsToGet;
            
					return code;
				} catch(Exception e) {
					e.GetType();
					// Strip not terminated as expected: return EndOfInformation code.
					return 257;
				}
			}
		}
	}
}