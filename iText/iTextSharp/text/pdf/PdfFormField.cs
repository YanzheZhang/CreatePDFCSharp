using System;
using System.Drawing;
using System.Collections;

using iTextSharp.text;

/*
 * Copyright 2002 by Paulo Soares.
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

	/** Implements form fields.
	 *
	 * @author Paulo Soares (psoares@consiste.pt)
	 */
	public class PdfFormField : PdfAnnotation {

		public static int FF_READ_ONLY = 1;
		public static int FF_REQUIRED = 2;
		public static int FF_NO_EXPORT = 4;
		public static int FF_NO_TOGGLE_TO_OFF = 16384;
		public static int FF_RADIO = 32768;
		public static int FF_PUSHBUTTON = 65536;
		public static int FF_MULTILINE = 4096;
		public static int FF_PASSWORD = 8192;
		public static int FF_COMBO = 131072;
		public static int FF_EDIT = 262144;
		public static int Q_LEFT = 0;
		public static int Q_CENTER = 1;
		public static int Q_RIGHT = 2;
		public static int MK_NO_ICON = 0;
		public static int MK_NO_CAPTION = 1;
		public static int MK_CAPTION_BELOW = 2;
		public static int MK_CAPTION_ABOVE = 3;
		public static int MK_CAPTION_RIGHT = 4;
		public static int MK_CAPTION_LEFT = 5;
		public static int MK_CAPTION_OVERLAID = 6;
		public static PdfName IF_SCALE_ALWAYS = PdfName.A;
		public static PdfName IF_SCALE_BIGGER = PdfName.B;
		public static PdfName IF_SCALE_SMALLER = PdfName.S;
		public static PdfName IF_SCALE_NEVER = PdfName.N;
		public static PdfName IF_SCALE_ANAMORPHIC = PdfName.A;
		public static PdfName IF_SCALE_PROPORTIONAL = PdfName.P;
		public static bool MULTILINE = true;
		public static bool SINGLELINE = false;
		public static bool PLAINTEXT = false;
		public static bool PASSWORD = true;
		public static PdfName[] mergeTarget = {PdfName.FONT, PdfName.XOBJECT, PdfName.COLORSPACE, PdfName.PATTERN};
    
		/** Holds value of property parent. */
		internal PdfFormField parent;
    
		internal ArrayList kids;
    
		/**
		 * Constructs a new <CODE>PdfAnnotation</CODE> of subtype link (Action).
		 */
    
		public PdfFormField(PdfWriter writer, float llx, float lly, float urx, float ury, PdfAction action) : base(writer, llx, lly, urx, ury, action) {
			put(PdfName.TYPE, PdfName.ANNOT);
			put(PdfName.SUBTYPE, PdfName.WIDGET);
			annotation = true;
		}

		/** Creates new PdfFormField */
		internal PdfFormField(PdfWriter writer) : base(writer, null) {
			form = true;
			annotation = false;
		}
    
		public void setWidget(Rectangle rect, PdfName highlight) {
			put(PdfName.TYPE, PdfName.ANNOT);
			put(PdfName.SUBTYPE, PdfName.WIDGET);
			put(PdfName.RECT, new PdfRectangle(rect));
			annotation = true;
			if (!highlight.Equals(HIGHLIGHT_INVERT))
				put(PdfName.H, highlight);
		}
    
		public static PdfFormField createEmpty(PdfWriter writer) {
			PdfFormField field = new PdfFormField(writer);
			return field;
		}
    
		public int Button {
			set {
				put(PdfName.FT, PdfName.BTN);
				if (value != 0)
					put(PdfName.FF, new PdfNumber(value));
			}
		}
    
		protected static PdfFormField createButton(PdfWriter writer, int flags) {
			PdfFormField field = new PdfFormField(writer);
			field.Button = flags;
			return field;
		}
    
		public static PdfFormField createPushButton(PdfWriter writer) {
			return createButton(writer, FF_PUSHBUTTON);
		}

		public static PdfFormField createCheckBox(PdfWriter writer) {
			return createButton(writer, 0);
		}

		public static PdfFormField createRadioButton(PdfWriter writer, bool noToggleToOff) {
			return createButton(writer, FF_RADIO + (noToggleToOff ? FF_NO_TOGGLE_TO_OFF : 0));
		}
    
		public static PdfFormField createTextField(PdfWriter writer, bool multiline, bool password, int maxLen) {
			PdfFormField field = new PdfFormField(writer);
			field.put(PdfName.FT, PdfName.TX);
			int flags = (multiline ? FF_MULTILINE : 0);
			flags += (password ? FF_PASSWORD : 0);
			field.put(PdfName.FF, new PdfNumber(flags));
			if (maxLen > 0)
				field.put(PdfName.MAXLEN, new PdfNumber(maxLen));
			return field;
		}
    
		protected static PdfFormField createChoice(PdfWriter writer, int flags, PdfArray options, int topIndex) {
			PdfFormField field = new PdfFormField(writer);
			field.put(PdfName.FT, PdfName.CH);
			field.put(PdfName.FF, new PdfNumber(flags));
			field.put(PdfName.OPT, options);
			if (topIndex > 0)
				field.put(PdfName.TI, new PdfNumber(topIndex));
			return field;
		}
    
		public static PdfFormField createList(PdfWriter writer, string[] options, int topIndex, bool unicode) {
			return createChoice(writer, 0, processOptions(options, unicode), topIndex);
		}

		public static PdfFormField createList(PdfWriter writer, string[] options, int topIndex) {
			return createList(writer, options, topIndex, false);
		}

		public static PdfFormField createList(PdfWriter writer, string[][] options, int topIndex, bool unicode) {
			return createChoice(writer, 0, processOptions(options, unicode), topIndex);
		}

		public static PdfFormField createList(PdfWriter writer, string[][] options, int topIndex) {
			return createList(writer, options, topIndex, false);
		}

		public static PdfFormField createCombo(PdfWriter writer, bool edit, string[] options, int topIndex, bool unicode) {
			return createChoice(writer, FF_COMBO + (edit ? FF_EDIT : 0), processOptions(options, unicode), topIndex);
		}
    
		public static PdfFormField createCombo(PdfWriter writer, bool edit, string[] options, int topIndex) {
			return createCombo(writer, edit, options, topIndex, false);
		}
    
		public static PdfFormField createCombo(PdfWriter writer, bool edit, string[][] options, int topIndex, bool unicode) {
			return createChoice(writer, FF_COMBO + (edit ? FF_EDIT : 0), processOptions(options, unicode), topIndex);
		}
    
		public static PdfFormField createCombo(PdfWriter writer, bool edit, string[][] options, int topIndex) {
			return createCombo(writer, edit, options, topIndex, false);
		}
    
		protected static PdfArray processOptions(string[] options, bool unicode) {
			string encoding = (unicode ? PdfObject.TEXT_UNICODE : PdfObject.ENCODING);
			PdfArray array = new PdfArray();
			for (int k = 0; k < options.Length; ++k) {
				array.Add(new PdfString(options[k], encoding));
			}
			return array;
		}
    
		protected static PdfArray processOptions(string[][] options, bool unicode) {
			string encoding = (unicode ? PdfObject.TEXT_UNICODE : PdfObject.ENCODING);
			PdfArray array = new PdfArray();
			for (int k = 0; k < options.Length; ++k) {
				string[] subOption = options[k];
				PdfArray ar2 = new PdfArray(new PdfString(subOption[0], encoding));
				ar2.Add(new PdfString(subOption[1], encoding));
				array.Add(ar2);
			}
			return array;
		}
    
		public static PdfFormField createSignature(PdfWriter writer) {
			PdfFormField field = new PdfFormField(writer);
			field.put(PdfName.FT, PdfName.SIG);
			field.put(PdfName.FF, new PdfNumber(0));
			writer.SigFlags = PdfWriter.SIGNATURE_EXISTS;
			return field;
		}
    
		/** Getter for property parent.
		 * @return Value of property parent.
		 */
		public PdfFormField Parent {
			get {
				return parent;
			}
		}
    
		public void addKid(PdfFormField field) {
			field.parent = this;
			if (kids == null)
				kids = new ArrayList();
			kids.Add(field);
		}
    
		internal ArrayList getKids() {
			return kids;
		}
    
		public int setFieldFlags(int flags) {
			PdfNumber obj = (PdfNumber)get(PdfName.FF);
			int old;
			if (obj == null)
				old = 0;
			else
				old = obj.IntValue;
			int v = old | flags;
			put(PdfName.FF, new PdfNumber(v));
			return old;
		}
    
		public string ValueAsstringUnicode {
			set {
				put(PdfName.V, new PdfString(value, PdfObject.TEXT_UNICODE));
			}
		}

		public string ValueAsstring {
			set {
				put(PdfName.V, new PdfString(value));
			}
		}

		public string ValueAsName {
			set {
				put(PdfName.V, new PdfName(value));
			}
		}

		public PdfSignature Value {
			set {
				put(PdfName.V, value);
			}

		}

		public string DefaultValueAsstring {
			set {
				put(PdfName.DV, new PdfString(value));
			}
		}

		public string DefaultValueAsstringUnicode {
			set {
				put(PdfName.DV, new PdfString(value, PdfObject.TEXT_UNICODE));
			}
		}

		public string DefaultValueAsName {
			set {
				put(PdfName.DV, new PdfName(value));
			}
		}
    
		public string FieldName {
			set {
				put(PdfName.T, new PdfString(value));
			}
		}
    
		public string UserName {
			set {
				put(PdfName.TU, new PdfString(value));
			}
		}
    
		public string MappingName {
			set {
				put(PdfName.TM, new PdfString(value));
			}
		}
    
		public int Quadding {
			set {
				put(PdfName.Q, new PdfNumber(value));
			}
		}
    
		internal static void mergeResources(PdfDictionary result, PdfDictionary source) {
			PdfDictionary dic = null;
			PdfDictionary res = null;
			PdfName target = null;
			for (int k = 0; k < mergeTarget.Length; ++k) {
				target = mergeTarget[k];
				if ((dic = (PdfDictionary)source.get(target)) != null) {
					if ((res = (PdfDictionary)result.get(target)) == null) {
						res = new PdfDictionary();
					}
					res.merge(dic);
					result.put(target, res);
				}
			}
		}
    
		internal override bool Used {
			set {
				used = true;
				if (parent != null)
					put(PdfName.PARENT, parent.IndirectReference);
				if (kids != null) {
					PdfArray array = new PdfArray();
					for (int k = 0; k < kids.Count; ++k)
						array.Add(((PdfFormField)kids[k]).IndirectReference);
					put(PdfName.KIDS, array);
				}
				if (templates == null)
					return;
				PdfDictionary dic = new PdfDictionary();
				foreach(PdfTemplate template in templates.Keys) {
					mergeResources(dic, (PdfDictionary)template.Resources);
				}
				put(PdfName.DR, dic);
			}
		}

		internal PdfDictionary getMK() {
			PdfDictionary mk = (PdfDictionary)get(PdfName.MK);
			if (mk == null) {
				mk = new PdfDictionary();
				put(PdfName.MK, mk);
			}
			return mk;
		}
    
		public int MKRotation {
			set {
				getMK().put(PdfName.R, new PdfNumber(value));
			}
		}
    
		internal PdfArray getMKColor(Color color) {
			PdfArray array = new PdfArray();
			int type = ExtendedColor.getType(color);
			switch (type) {
				case ExtendedColor.TYPE_GRAY: {
					array.Add(new PdfNumber(((GrayColor)color).Gray));
					break;
				}
				case ExtendedColor.TYPE_CMYK: {
					CMYKColor cmyk = (CMYKColor)color;
					array.Add(new PdfNumber(cmyk.Cyan));
					array.Add(new PdfNumber(cmyk.Magenta));
					array.Add(new PdfNumber(cmyk.Yellow));
					array.Add(new PdfNumber(cmyk.Black));
					break;
				}
				case ExtendedColor.TYPE_SEPARATION:
				case ExtendedColor.TYPE_PATTERN:
				case ExtendedColor.TYPE_SHADING:
					throw new RuntimeException("Separations, patterns and shadings are not allowed in MK dictionary.");
				default:
					array.Add(new PdfNumber(color.R / 255f));
					array.Add(new PdfNumber(color.G / 255f));
					array.Add(new PdfNumber(color.B / 255f));
					break;
			}
			return array;
		}
    
		public Color MKBorderColor {
			set {
				if (value == null)
					getMK().remove(PdfName.BC);
				else
					getMK().put(PdfName.BC, getMKColor(value));
			}
		}
    
		public Color MKBackgroundColor {
			set {
				if (value == null)
					getMK().remove(PdfName.BG);
				else
					getMK().put(PdfName.BG, getMKColor(value));
			}
		}
    
		public string MKNormalCaption {
			set {
				getMK().put(PdfName.CA, new PdfString(value));
			}
		}
    
		public string MKRolloverCaption {
			set {
				getMK().put(PdfName.RC, new PdfString(value));
			}
		}
    
		public string MKAlternateCaption {
			set {
				getMK().put(PdfName.AC, new PdfString(value));
			}
		}
    
		public string MKNormalCaptionUnicode {
			set {
				getMK().put(PdfName.CA, new PdfString(value, PdfObject.TEXT_UNICODE));
			}
		}
    
		public string MKRolloverCaptionUnicode {
			set {
				getMK().put(PdfName.RC, new PdfString(value, PdfObject.TEXT_UNICODE));
			}
		}
    
		public string MKAlternateCaptionUnicode {
			set {
				getMK().put(PdfName.AC, new PdfString(value, PdfObject.TEXT_UNICODE));
			}
		}
    
		public PdfTemplate MKNormalIcon {
			set {
				getMK().put(PdfName.I, value.IndirectReference);
			}
		}
    
		public PdfTemplate MKRolloverIcon {
			set {
				getMK().put(PdfName.RI, value.IndirectReference);
			}
		}
    
		public PdfTemplate MKAlternateIcon {
			set {
				getMK().put(PdfName.IX, value.IndirectReference);
			}
		}
    
		public void setMKIconFit(PdfName scale, PdfName scalingType, float leftoverLeft, float leftoverBottom) {
			PdfDictionary dic = new PdfDictionary();
			if (!scale.Equals(PdfName.A))
				dic.put(PdfName.SW, scale);
			if (!scalingType.Equals(PdfName.P))
				dic.put(PdfName.S, scalingType);
			if (leftoverLeft != 0.5f || leftoverBottom != 0.5f) {
				PdfArray array = new PdfArray(new PdfNumber(leftoverLeft));
				array.Add(new PdfNumber(leftoverBottom));
				dic.put(PdfName.A, array);
			}
			getMK().put(PdfName.IF, dic);
		}
    
		public int MKTextPosition {
			set {
				getMK().put(PdfName.TP, new PdfNumber(value));
			}
		}
	}
}