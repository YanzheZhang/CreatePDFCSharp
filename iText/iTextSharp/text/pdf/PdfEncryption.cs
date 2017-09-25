using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

/*
 * $Id: PdfEncryption.cs,v 1.1.1.1 2003/02/04 02:57:21 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 Paulo Soares
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
 *
 * @author  Paulo Soares (psoares@consiste.pt)
 */
public class PdfEncryption {

    static byte[] pad = {
        (byte)0x28, (byte)0xBF, (byte)0x4E, (byte)0x5E, (byte)0x4E, (byte)0x75,
        (byte)0x8A, (byte)0x41, (byte)0x64, (byte)0x00, (byte)0x4E, (byte)0x56,
        (byte)0xFF, (byte)0xFA, (byte)0x01, (byte)0x08, (byte)0x2E, (byte)0x2E,
        (byte)0x00, (byte)0xB6, (byte)0xD0, (byte)0x68, (byte)0x3E, (byte)0x80,
        (byte)0x2F, (byte)0x0C, (byte)0xA9, (byte)0xFE, (byte)0x64, (byte)0x53,
        (byte)0x69, (byte)0x7A};
        
    byte[] state = new byte[256];
    int x;
    int y;
    /** The encryption key for a particular object/generation */
    byte[] key;
    /** The encryption key length for a particular object/generation */
    int keySize;
    /** The global encryption key */
    byte[] mkey;
    /** Work area to prepare the object/generation bytes */
    byte[] extra = new byte[5];
    /** The message digest algorithm MD5 */
	MD5 md5;
    /** The encryption key for the owner */
    byte[] ownerKey = new byte[32];
    /** The encryption key for the user */
    byte[] userKey = new byte[32];
    int permissions;
    byte[] documentID;
    
    public PdfEncryption() {
        try {
            md5 = new MD5CryptoServiceProvider();
        }
        catch (Exception e) {
             throw e;
       }
    }
    
    public void setupAllKeys(byte[] userPassword, byte[] ownerPassword, int permissions, bool strength128Bits) {
        if (strength128Bits)
            permissions |= unchecked((int)0xfffff0c0);
        else
            permissions |= unchecked((int)0xffffffc0);
        permissions &= unchecked((int)0xfffffffc);
        this.permissions = permissions;
        long time;
        byte[] userPad = new byte[32];
        if (userPassword == null)
            Array.Copy(pad, 0, userPad, 0, 32);
        else {
            Array.Copy(userPassword, 0, userPad, 0, Math.Min(userPassword.Length, 32));
            if (userPassword.Length < 32)
                Array.Copy(pad, 0, userPad, userPassword.Length, 32 - userPassword.Length);
        }
        byte[] ownerPad = new byte[32];
        if (ownerPassword == null || ownerPassword.Length == 0) {
            Array.Copy(pad, 0, ownerPad, 0, 32);
            time = (long)TimeSpan.Parse(DateTime.Now.ToString("d.hh:mm:ss.ff")).TotalMilliseconds;
            //string s = time + "-" + mem;
			string s = "testing";
			Array.Copy(md5.ComputeHash(Encoding.ASCII.GetBytes(s)), 0, ownerPad, 0, 16);
        }
        else {
			Array.Copy(ownerPassword, 0, ownerPad, 0, Math.Min(ownerPassword.Length, 32));
            if (ownerPassword.Length < 32)
                Array.Copy(pad, 0, ownerPad, ownerPassword.Length, 32 - ownerPassword.Length);
        }
        mkey = new byte[strength128Bits ? 16 : 5];
		byte[] digest = md5.ComputeHash(ownerPad);
        if (strength128Bits) {
            for (int k = 0; k < 50; ++k)
				digest = md5.ComputeHash(digest);
            Array.Copy(userPad, 0, ownerKey, 0, 32);
            for (int i = 0; i < 20; ++i){
                for (int j = 0; j < mkey.Length ; ++j)
                    mkey[j] = (byte)(digest[j] ^ i);
                prepareRC4Key(mkey);
                encryptRC4(ownerKey);
            }
        }
        else {
            prepareRC4Key(digest, 0, mkey.Length);
            encryptRC4(userPad, ownerKey);
        }
        time = (long)TimeSpan.Parse(DateTime.Now.ToString("d.hh:mm:ss.ff")).TotalMilliseconds;
        //string st = time + "+" + mem;
		string st = "testing";
		documentID = md5.ComputeHash(Encoding.ASCII.GetBytes(st));
		MemoryStream m = new MemoryStream();
		m.Write(userPad, 0, userPad.Length);
		m.Write(ownerKey, 0, ownerKey.Length);
        extra[0] = (byte)permissions;
        extra[1] = (byte)(permissions >> 8);
        extra[2] = (byte)(permissions >> 16);
        extra[3] = (byte)(permissions >> 24);
		m.Write(extra, 0, 4);
		m.Write(documentID, 0, documentID.Length);
        digest = md5.ComputeHash(m.ToArray());
        if (strength128Bits) {
            for (int k = 0; k < 50; ++k)
                digest = md5.ComputeHash(digest);
        }
        Array.Copy(digest, 0, mkey, 0, mkey.Length);
        
        // Compute the user key
        
        if (strength128Bits) {
			m = new MemoryStream();
			m.Write(pad, 0, pad.Length);
			m.Write(documentID, 0, documentID.Length);
            digest = md5.ComputeHash(m.ToArray());
            Array.Copy(digest, 0, userKey, 0, 16);
            for (int k = 16; k < 32; ++k)
                userKey[k] = 0;
            for (int i = 0; i < 20; ++i) {
                for (int j = 0; j < mkey.Length; ++j)
                    digest[j] = (byte)(mkey[j] ^ i);
                prepareRC4Key(digest, 0, mkey.Length);
                encryptRC4(userKey, 0, 16);
            }
        }
        else {
            prepareRC4Key(mkey);
            encryptRC4(pad, userKey);
        }
    }
    
    public void prepareKey() {
        prepareRC4Key(key, 0, keySize);
    }
    
    public void setHashKey(int number, int generation) {
        extra[0] = (byte)number;
        extra[1] = (byte)(number >> 8);
        extra[2] = (byte)(number >> 16);
        extra[3] = (byte)generation;
        extra[4] = (byte)(generation >> 8);
		MemoryStream m = new MemoryStream();
		m.Write(mkey, 0, mkey.Length);
		m.Write(extra, 0, extra.Length);
        key = md5.ComputeHash(m.ToArray());
        keySize = mkey.Length + 5;
        if (keySize > 16)
            keySize = 16;
    }
    
    public PdfLiteral FileID {
		get {
			ByteBuffer b = new ByteBuffer();
			b.Append('[');
			b.Append(PdfContentByte.escapestring(documentID));
			b.Append(PdfContentByte.escapestring(documentID));
			b.Append(']');
			return new PdfLiteral(b.toByteArray());
		}
    }
    
    public PdfDictionary EncryptionDictionary {
		get {
			PdfDictionary dic = new PdfDictionary();
			dic.put(PdfName.FILTER, PdfName.STANDARD);
			dic.put(PdfName.O, new PdfLiteral(PdfContentByte.escapestring(ownerKey)));
			dic.put(PdfName.U, new PdfLiteral(PdfContentByte.escapestring(userKey)));
			dic.put(PdfName.P, new PdfNumber(permissions));
			if (mkey.Length > 5) {
				dic.put(PdfName.V, new PdfNumber(2));
				dic.put(PdfName.R, new PdfNumber(3));
				dic.put(PdfName.LENGTH, new PdfNumber(128));
			}
			else {
				dic.put(PdfName.V, new PdfNumber(1));
				dic.put(PdfName.R, new PdfNumber(2));
			}
			return dic;
		}
    }
    
    public void prepareRC4Key(byte[] key) {
        prepareRC4Key(key, 0, key.Length);
    }
    
    public void prepareRC4Key(byte[] key, int off, int len) {
        int index1 = 0;
        int index2 = 0;
        for (int k = 0; k < 256; ++k)
            state[k] = (byte)k;
        x = 0;
        y = 0;
        byte tmp;
        for (int k = 0; k < 256; ++k) {
            index2 = (key[index1 + off] + state[k] + index2) & 255;
            tmp = state[k];
            state[k] = state[index2];
            state[index2] = tmp;
            index1 = (index1 + 1) % len;
        }
    }
    
    public void encryptRC4(byte[] dataIn, int off, int len, byte[] dataOut) {
        int length = len + off;
        byte tmp;
        for (int k = off; k < length; ++k) {
            x = (x + 1) & 255;
            y = (state[x] + y) & 255;
            tmp = state[x];
            state[x] = state[y];
            state[y] = tmp;
            dataOut[k] = (byte)(dataIn[k] ^ state[(state[x] + state[y]) & 255]);
        }
    }

    public void encryptRC4(byte[] data, int off, int len) {
        encryptRC4(data, off, len, data);
    }

    public void encryptRC4(byte[] dataIn, byte[] dataOut) {
        encryptRC4(dataIn, 0, dataIn.Length, dataOut);
    }
    
    public void encryptRC4(byte[] data) {
        encryptRC4(data, 0, data.Length, data);
    }
}
}