using System;
using System.Text;
using System.Collections;
using System.util;

/*
 * $Id: PdfAcroForm.cs,v 1.1.1.1 2003/02/04 02:57:00 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2002 Bruno Lowagie
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
 * Each PDF document can contain maximum 1 AcroForm.
 */

public class PdfAcroForm : PdfDictionary {

    private PdfWriter writer;


    /** This is a map containing FieldTemplates. */
    private Hashmap fieldTemplates = new Hashmap();

    /** This is an array containing DocumentFields. */
    private PdfArray documentFields = new PdfArray();

    /** This is an array containing the calculationorder of the fields. */
    private PdfArray calculationOrder = new PdfArray();

    /** Contains the signature flags. */
    private int sigFlags = 0;

    /** Creates new PdfAcroForm */
    internal PdfAcroForm(PdfWriter writer) : base() {
        this.writer = writer;
    }

    /**
     * Adds fieldTemplates.
     */

    internal void addFieldTemplates(Hashmap ft) {
		foreach(object key in ft.Keys) {
			fieldTemplates.Add(key, ft[key]);
		}
    }

    /**
     * Adds documentFields.
     */

    internal void addDocumentField(PdfIndirectReference piref) {
        documentFields.Add(piref);
    }

    /**
     * Closes the AcroForm.
     */

    internal bool isValid() {
        if (documentFields.Size == 0) return false;
        put(PdfName.FIELDS, documentFields);
        if (sigFlags != 0)
            put(PdfName.SIGFLAGS, new PdfNumber(sigFlags));
        if (calculationOrder.Size > 0)
            put(PdfName.CO, calculationOrder);
        if (fieldTemplates.Count == 0) return false;
        PdfDictionary dic = new PdfDictionary();
		foreach(PdfTemplate template in fieldTemplates.Keys) {
            PdfFormField.mergeResources(dic, (PdfDictionary)template.Resources);
        }
        put(PdfName.DR, dic);
        PdfDictionary fonts = (PdfDictionary)dic.get(PdfName.FONT);
        if (fonts != null) {
            put(PdfName.DA, new PdfString("/F1 0 Tf 0 g "));
            writer.eliminateFontSubset(fonts);
        }
        return true;
    }

    /**
     * Adds an object to the calculationOrder.
     */

    public void addCalculationOrder(PdfFormField formField) {
        calculationOrder.Add(formField.IndirectReference);
    }

    /**
     * Sets the signature flags.
     */

    public int SigFlags {
		set {
			sigFlags |= value;
		}
    }

    /**
     * Adds a formfield to the AcroForm.
     */

    public void addFormField(PdfFormField formField) {
        writer.addAnnotation(formField);
    }

    public PdfFormField addHtmlPostButton(string name, string caption, string value, string url, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfAction action = PdfAction.createSubmitForm(url, null, PdfAction.SUBMIT_HTML_FORMAT);
        PdfFormField button = new PdfFormField(writer, llx, lly, urx, ury, action);
        setButtonParams(button, PdfFormField.FF_PUSHBUTTON, name, value);
        drawButton(button, caption, font, fontSize, llx, lly, urx, ury);
        addFormField(button);
	return button;
    }

    public PdfFormField addResetButton(string name, string caption, string value, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfAction action = PdfAction.createResetForm(null, 0);
        PdfFormField button = new PdfFormField(writer, llx, lly, urx, ury, action);
        setButtonParams(button, PdfFormField.FF_PUSHBUTTON, name, value);
        drawButton(button, caption, font, fontSize, llx, lly, urx, ury);
        addFormField(button);
        return button;
    }

    public PdfFormField addMap(string name, string value, string url, PdfContentByte appearance, float llx, float lly, float urx, float ury) {
        PdfAction action = PdfAction.createSubmitForm(url, null, PdfAction.SUBMIT_HTML_FORMAT | PdfAction.SUBMIT_COORDINATES);
        PdfFormField button = new PdfFormField(writer, llx, lly, urx, ury, action);
        setButtonParams(button, PdfFormField.FF_PUSHBUTTON, name, null);
        PdfContentByte cb = writer.DirectContent;
        PdfAppearance pa = cb.createAppearance(urx - llx, ury - lly);
        pa.Add(appearance);
        button.setAppearance(PdfAnnotation.APPEARANCE_NORMAL, pa);
        addFormField(button);
        return button;
    }

    public void setButtonParams(PdfFormField button, int characteristics, string name, string value) {
        button.Button = characteristics;
		button.Flags = PdfAnnotation.FLAGS_PRINT;
        button.setPage();
        button.FieldName = name;
        if (value != null) button.ValueAsstring = value;
    }

    public void drawButton(PdfFormField button, string caption, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfContentByte cb = writer.DirectContent;
        PdfAppearance pa = cb.createAppearance(urx - llx, ury - lly);
        pa.drawButton(0f, 0f, urx - llx, ury - lly, caption, font, fontSize);
        button.setAppearance(PdfAnnotation.APPEARANCE_NORMAL, pa);
    }

    public PdfFormField addHiddenField(string name, string value) {
        PdfFormField hidden = PdfFormField.createEmpty(writer);
        hidden.FieldName = name;
        hidden.ValueAsName = value;
        addFormField(hidden);
        return hidden;
    }

    public PdfFormField addSingleLineTextField(string name, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfFormField field = PdfFormField.createTextField(writer, PdfFormField.SINGLELINE, PdfFormField.PLAINTEXT, 0);
        setTextFieldParams(field, text, name, llx, lly, urx, ury);
        drawSingleLineOfText(field, text, font, fontSize, llx, lly, urx, ury);
        addFormField(field);
        return field;
    }

    public PdfFormField addMultiLineTextField(string name, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfFormField field = PdfFormField.createTextField(writer, PdfFormField.MULTILINE, PdfFormField.PLAINTEXT, 0);
        setTextFieldParams(field, text, name, llx, lly, urx, ury);
        drawMultiLineOfText(field, text, font, fontSize, llx, lly, urx, ury);
        addFormField(field);
        return field;
    }

    public PdfFormField addSingleLinePasswordField(string name, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfFormField field = PdfFormField.createTextField(writer, PdfFormField.SINGLELINE, PdfFormField.PASSWORD, 0);
        setTextFieldParams(field, text, name, llx, lly, urx, ury);
        drawSingleLineOfText(field, text, font, fontSize, llx, lly, urx, ury);
        addFormField(field);
        return field;
    }

    public void setTextFieldParams(PdfFormField field, string text, string name, float llx, float lly, float urx, float ury) {
        field.setWidget(new Rectangle(llx, lly, urx, ury), PdfAnnotation.HIGHLIGHT_INVERT);
        field.ValueAsstring = text;
		field.DefaultValueAsstring = text;
        field.FieldName = name;
        field.Flags = PdfAnnotation.FLAGS_PRINT;
        field.setPage();
    }

    public void drawSingleLineOfText(PdfFormField field, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfContentByte da = new PdfContentByte(writer);
        da.setFontAndSize(font, fontSize);
        da.resetRGBColorFill();
        field.DefaultAppearancestring = da;
        PdfContentByte cb = writer.DirectContent;
        cb.moveTo(0, 0);
        PdfAppearance tp = cb.createAppearance(urx - llx, ury - lly);
        tp.drawTextField(0f, 0f, urx - llx, ury - lly);
        tp.beginVariableText();
        tp.saveState();
        tp.rectangle(3f, 3f, urx - llx - 6f, ury - lly - 6f);
        tp.clip();
        tp.newPath();
        tp.beginText();
        tp.setFontAndSize(font, fontSize);
        tp.resetRGBColorFill();
        tp.setTextMatrix(4, (ury - lly) / 2 - (fontSize * 0.3f));
        tp.showText(text);
        tp.endText();
        tp.restoreState();
        tp.endVariableText();
        field.setAppearance(PdfAnnotation.APPEARANCE_NORMAL, tp);
    }

    public void drawMultiLineOfText(PdfFormField field, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfContentByte da = new PdfContentByte(writer);
        da.setFontAndSize(font, fontSize);
        da.resetRGBColorFill();
        field.DefaultAppearancestring = da;
        PdfContentByte cb = writer.DirectContent;
        cb.moveTo(0, 0);
        PdfAppearance tp = cb.createAppearance(urx - llx, ury - lly);
        tp.drawTextField(0f, 0f, urx - llx, ury - lly);
        tp.beginVariableText();
        tp.saveState();
        tp.rectangle(3f, 3f, urx - llx - 6f, ury - lly - 6f);
        tp.clip();
        tp.newPath();
        tp.beginText();
        tp.setFontAndSize(font, fontSize);
        tp.resetRGBColorFill();
        tp.setTextMatrix(4, 5);
        System.util.StringTokenizer tokenizer = new System.util.StringTokenizer(text, "\n");
        float yPos = ury - lly;
        while (tokenizer.hasMoreTokens()) {
            yPos -= fontSize * 1.2f;
            tp.showTextAligned(PdfContentByte.ALIGN_LEFT, tokenizer.nextToken(), 3, yPos, 0);
        }
        tp.endText();
        tp.restoreState();
        tp.endVariableText();
        field.setAppearance(PdfAnnotation.APPEARANCE_NORMAL, tp);
    }

    public PdfFormField addCheckBox(string name, string value, bool status, float llx, float lly, float urx, float ury) {
        PdfFormField field = PdfFormField.createCheckBox(writer);
        setCheckBoxParams(field, name, value, status, llx, lly, urx, ury);
        drawCheckBoxAppearences(field, value, llx, lly, urx, ury);
        addFormField(field);
        return field;
    }

    public void setCheckBoxParams(PdfFormField field, string name, string value, bool status, float llx, float lly, float urx, float ury) {
        field.setWidget(new Rectangle(llx, lly, urx, ury), PdfAnnotation.HIGHLIGHT_TOGGLE);
        field.FieldName = name;
        if (status) {
            field.ValueAsName = value;
            field.AppearanceState = value;
        }
        else {
            field.ValueAsName = "Off";
            field.AppearanceState = "Off";
        }
        field.Flags = PdfAnnotation.FLAGS_PRINT;
        field.setPage();
        field.BorderStyle = new PdfBorderDictionary(1, PdfBorderDictionary.STYLE_SOLID);
    }

    public void drawCheckBoxAppearences(PdfFormField field, string value, float llx, float lly, float urx, float ury) {
        BaseFont font = null;
        try {
            font = BaseFont.createFont(BaseFont.ZAPFDINGBATS, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
        }
        catch(Exception e) {
            throw e;
        }
        float size = (ury - lly);
        PdfContentByte da = new PdfContentByte(writer);
        da.setFontAndSize(font, size);
        da.resetRGBColorFill();
        field.DefaultAppearancestring = da;
        PdfContentByte cb = writer.DirectContent;
        cb.moveTo(0, 0);
        PdfAppearance tpOn = cb.createAppearance(urx - llx, ury - lly);
        tpOn.drawTextField(0f, 0f, urx - llx, ury - lly);
        tpOn.saveState();
        tpOn.resetRGBColorFill();
        tpOn.beginText();
        tpOn.setFontAndSize(font, size);
        tpOn.showTextAligned(PdfContentByte.ALIGN_CENTER, "4", (urx - llx) / 2, (ury - lly) / 2 - (size * 0.3f), 0);
        tpOn.endText();
        tpOn.restoreState();
        field.setAppearance(PdfAnnotation.APPEARANCE_NORMAL, value, tpOn);
        PdfAppearance tpOff = cb.createAppearance(urx - llx, ury - lly);
        tpOff.drawTextField(0f, 0f, urx - llx, ury - lly);
        field.setAppearance(PdfAnnotation.APPEARANCE_NORMAL, "Off", tpOff);
    }

    public PdfFormField getRadioGroup(string name, string defaultValue, bool noToggleToOff) {
        PdfFormField radio = PdfFormField.createRadioButton(writer, noToggleToOff);
        radio.FieldName = name;
        radio.ValueAsName = defaultValue;
        return radio;
    }

    public void addRadioGroup(PdfFormField radiogroup) {
        addFormField(radiogroup);
    }

    public PdfFormField addRadioButton(PdfFormField radiogroup, string value, float llx, float lly, float urx, float ury) {
        PdfFormField radio = PdfFormField.createEmpty(writer);
        radio.setWidget(new Rectangle(llx, lly, urx, ury), PdfAnnotation.HIGHLIGHT_TOGGLE);
        string name = ((PdfName)radiogroup.get(PdfName.V)).ToString().Substring(1);
        if (name.Equals(value)) {
            radio.AppearanceState = value;
        }
        else {
            radio.AppearanceState = "Off";
        }
        drawRadioAppearences(radio, value, llx, lly, urx, ury);
        radiogroup.addKid(radio);
        return radio;
    }

    public void drawRadioAppearences(PdfFormField field, string value, float llx, float lly, float urx, float ury) {
        PdfContentByte cb = writer.DirectContent;
        cb.moveTo(0, 0);
        PdfAppearance tpOn = cb.createAppearance(urx - llx, ury - lly);
        tpOn.drawRadioField(0f, 0f, urx - llx, ury - lly, true);
        field.setAppearance(PdfAnnotation.APPEARANCE_NORMAL, value, tpOn);
        PdfAppearance tpOff = cb.createAppearance(urx - llx, ury - lly);
        tpOff.drawRadioField(0f, 0f, urx - llx, ury - lly, false);
        field.setAppearance(PdfAnnotation.APPEARANCE_NORMAL, "Off", tpOff);
    }

    public PdfFormField addSelectList(string name, string[] options, string defaultValue, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfFormField choice = PdfFormField.createList(writer, options, 0);
        setChoiceParams(choice, name, defaultValue, llx, lly, urx, ury);
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < options.Length; i++) {
            text.Append(options[i]).Append("\n");
        }
        drawMultiLineOfText(choice, text.ToString(), font, fontSize, llx, lly, urx, ury);
        addFormField(choice);
        return choice;
    }

    public PdfFormField addSelectList(string name, string[][] options, string defaultValue, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfFormField choice = PdfFormField.createList(writer, options, 0);
        setChoiceParams(choice, name, defaultValue, llx, lly, urx, ury);
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < options.Length; i++) {
            text.Append(options[i][1]).Append("\n");
        }
        drawMultiLineOfText(choice, text.ToString(), font, fontSize, llx, lly, urx, ury);
        addFormField(choice);
        return choice;
    }

    public PdfFormField addComboBox(string name, string[] options, string defaultValue, bool editable, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfFormField choice = PdfFormField.createCombo(writer, editable, options, 0);
        setChoiceParams(choice, name, defaultValue, llx, lly, urx, ury);
        if (defaultValue == null) {
            defaultValue = options[0];
        }
        drawSingleLineOfText(choice, defaultValue, font, fontSize, llx, lly, urx, ury);
        addFormField(choice);
        return choice;
    }

    public PdfFormField addComboBox(string name, string[][] options, string defaultValue, bool editable, BaseFont font, float fontSize, float llx, float lly, float urx, float ury) {
        PdfFormField choice = PdfFormField.createCombo(writer, editable, options, 0);
        setChoiceParams(choice, name, defaultValue, llx, lly, urx, ury);
        string value = null;
        for (int i = 0; i < options.Length; i++) {
            if (options[i][0].Equals(defaultValue)) {
                value = options[i][1];
                break;
            }
        }
        if (value == null) {
            value = options[0][1];
        }
        drawSingleLineOfText(choice, value, font, fontSize, llx, lly, urx, ury);
        addFormField(choice);
        return choice;
    }

    public void setChoiceParams(PdfFormField field, string name, string defaultValue, float llx, float lly, float urx, float ury) {
        field.setWidget(new Rectangle(llx, lly, urx, ury), PdfAnnotation.HIGHLIGHT_INVERT);
        if (defaultValue != null) {
            field.ValueAsstring = defaultValue;
            field.DefaultValueAsstring = defaultValue;
        }
        field.FieldName = name;
        field.Flags = PdfAnnotation.FLAGS_PRINT;
        field.setPage();
        field.BorderStyle = new PdfBorderDictionary(2, PdfBorderDictionary.STYLE_SOLID);
    }
}
}