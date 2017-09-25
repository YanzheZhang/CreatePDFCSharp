using System;
using System.IO;
using System.Drawing;
using System.Collections;

/*
 * $Id: MetaDo.cs,v 1.1.1.1 2003/02/04 02:58:46 geraldhenson Exp $
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

namespace iTextSharp.text.pdf.wmf
{
	/// <summary>
	/// Summary description for MetaDo.
	/// </summary>
	public class MetaDo
	{
    
    public const int META_SETBKCOLOR            = 0x0201;
    public const int META_SETBKMODE             = 0x0102;
    public const int META_SETMAPMODE            = 0x0103;
    public const int META_SETROP2               = 0x0104;
    public const int META_SETRELABS             = 0x0105;
    public const int META_SETPOLYFILLMODE       = 0x0106;
    public const int META_SETSTRETCHBLTMODE     = 0x0107;
    public const int META_SETTEXTCHAREXTRA      = 0x0108;
    public const int META_SETTEXTCOLOR          = 0x0209;
    public const int META_SETTEXTJUSTIFICATION  = 0x020A;
    public const int META_SETWINDOWORG          = 0x020B;
    public const int META_SETWINDOWEXT          = 0x020C;
    public const int META_SETVIEWPORTORG        = 0x020D;
    public const int META_SETVIEWPORTEXT        = 0x020E;
    public const int META_OFFSETWINDOWORG       = 0x020F;
    public const int META_SCALEWINDOWEXT        = 0x0410;
    public const int META_OFFSETVIEWPORTORG     = 0x0211;
    public const int META_SCALEVIEWPORTEXT      = 0x0412;
    public const int META_LINETO                = 0x0213;
    public const int META_MOVETO                = 0x0214;
    public const int META_EXCLUDECLIPRECT       = 0x0415;
    public const int META_INTERSECTCLIPRECT     = 0x0416;
    public const int META_ARC                   = 0x0817;
    public const int META_ELLIPSE               = 0x0418;
    public const int META_FLOODFILL             = 0x0419;
    public const int META_PIE                   = 0x081A;
    public const int META_RECTANGLE             = 0x041B;
    public const int META_ROUNDRECT             = 0x061C;
    public const int META_PATBLT                = 0x061D;
    public const int META_SAVEDC                = 0x001E;
    public const int META_SETPIXEL              = 0x041F;
    public const int META_OFFSETCLIPRGN         = 0x0220;
    public const int META_TEXTOUT               = 0x0521;
    public const int META_BITBLT                = 0x0922;
    public const int META_STRETCHBLT            = 0x0B23;
    public const int META_POLYGON               = 0x0324;
    public const int META_POLYLINE              = 0x0325;
    public const int META_ESCAPE                = 0x0626;
    public const int META_RESTOREDC             = 0x0127;
    public const int META_FILLREGION            = 0x0228;
    public const int META_FRAMEREGION           = 0x0429;
    public const int META_INVERTREGION          = 0x012A;
    public const int META_PAINTREGION           = 0x012B;
    public const int META_SELECTCLIPREGION      = 0x012C;
    public const int META_SELECTOBJECT          = 0x012D;
    public const int META_SETTEXTALIGN          = 0x012E;
    public const int META_CHORD                 = 0x0830;
    public const int META_SETMAPPERFLAGS        = 0x0231;
    public const int META_EXTTEXTOUT            = 0x0a32;
    public const int META_SETDIBTODEV           = 0x0d33;
    public const int META_SELECTPALETTE         = 0x0234;
    public const int META_REALIZEPALETTE        = 0x0035;
    public const int META_ANIMATEPALETTE        = 0x0436;
    public const int META_SETPALENTRIES         = 0x0037;
    public const int META_POLYPOLYGON           = 0x0538;
    public const int META_RESIZEPALETTE         = 0x0139;
    public const int META_DIBBITBLT             = 0x0940;
    public const int META_DIBSTRETCHBLT         = 0x0b41;
    public const int META_DIBCREATEPATTERNBRUSH = 0x0142;
    public const int META_STRETCHDIB            = 0x0f43;
    public const int META_EXTFLOODFILL          = 0x0548;
    public const int META_DELETEOBJECT          = 0x01f0;
    public const int META_CREATEPALETTE         = 0x00f7;
    public const int META_CREATEPATTERNBRUSH    = 0x01F9;
    public const int META_CREATEPENINDIRECT     = 0x02FA;
    public const int META_CREATEFONTINDIRECT    = 0x02FB;
    public const int META_CREATEBRUSHINDIRECT   = 0x02FC;
    public const int META_CREATEREGION          = 0x06FF;

    public PdfContentByte cb;
    public InputMeta meta;
    int left;
    int top;
    int right;
    int bottom;
    int inch;
    MetaState state = new MetaState();

    public MetaDo(Stream meta, PdfContentByte cb) {
        this.cb = cb;
        this.meta = new InputMeta(meta);
    }
    
    public void readAll() {
        if (meta.readInt() != unchecked((int)0x9AC6CDD7)) {
            throw new DocumentException("Not a placeable windows metafile");
        }
        meta.readWord();
        left = meta.readShort();
        top = meta.readShort();
        right = meta.readShort();
        bottom = meta.readShort();
        inch = meta.readWord();
		state.ScalingX = (float)(right - left) / (float)inch * 72f;
        state.ScalingY = (float)(bottom - top) / (float)inch * 72f;
        state.OffsetWx = left;
        state.OffsetWy = top;
        state.ExtentWx = right - left;
        state.ExtentWy = bottom - top;
        meta.readInt();
        meta.readWord();
        meta.skip(18);
        
        int tsize;
        int function;
        cb.LineCap = 1;
		cb.LineJoin = 1;
        for (;;) {
            int lenMarker = meta.Length;
            tsize = meta.readInt();
            if (tsize < 3)
                break;
            function = meta.readWord();
            switch (function) {
                case 0:
                    break;
                case META_CREATEPALETTE:
                case META_CREATEREGION:
                case META_DIBCREATEPATTERNBRUSH:
                    state.addMetaObject(new MetaObject());
                    break;
                case META_CREATEPENINDIRECT:
                {
                    MetaPen pen = new MetaPen();
                    pen.init(meta);
                    state.addMetaObject(pen);
                    break;
                }
                case META_CREATEBRUSHINDIRECT:
                {
                    MetaBrush brush = new MetaBrush();
                    brush.init(meta);
                    state.addMetaObject(brush);
                    break;
                }
                case META_CREATEFONTINDIRECT:
                {
                    MetaFont font = new MetaFont();
                    font.init(meta);
                    state.addMetaObject(font);
                    break;
                }
                case META_SELECTOBJECT:
                {
                    int idx = meta.readWord();
                    state.selectMetaObject(idx, cb);
                    break;
                }
                case META_DELETEOBJECT:
                {
                    int idx = meta.readWord();
                    state.deleteMetaObject(idx);
                    break;
                }
                case META_SAVEDC:
                    state.saveState(cb);
                    break;
                case META_RESTOREDC:
                {
                    int idx = meta.readShort();
                    state.restoreState(idx, cb);
                    break;
                }
                case META_SETWINDOWORG:
                    state.OffsetWy = meta.readShort();
                    state.OffsetWx = meta.readShort();
                    break;
                case META_SETWINDOWEXT:
                    state.ExtentWy = meta.readShort();
                    state.ExtentWx = meta.readShort();
                    break;
                case META_MOVETO:
                {
                    int y = meta.readShort();
                    Point p = new Point(meta.readShort(), y);
                    state.CurrentPoint = p;
                    break;
                }
                case META_LINETO:
                {
                    int y = meta.readShort();
                    int x = meta.readShort();
                    Point p = state.CurrentPoint;
                    cb.moveTo(state.transformX(p.X), state.transformY(p.Y));
                    cb.lineTo(state.transformX(x), state.transformY(y));
                    cb.stroke();
                    state.CurrentPoint = new Point(x, y);
                    break;
                }
                case META_POLYLINE:
                {
                    state.LineJoinPolygon = cb;
                    int len = meta.readWord();
                    int x = meta.readShort();
                    int y = meta.readShort();
                    cb.moveTo(state.transformX(x), state.transformY(y));
                    for (int k = 1; k < len; ++k) {
                        x = meta.readShort();
                        y = meta.readShort();
                        cb.lineTo(state.transformX(x), state.transformY(y));
                    }
                    cb.stroke();
                    break;
                }
                case META_POLYGON:
                {
                    if (isNullStrokeFill(false))
                        break;
                    int len = meta.readWord();
                    int sx = meta.readShort();
                    int sy = meta.readShort();
                    cb.moveTo(state.transformX(sx), state.transformY(sy));
                    for (int k = 1; k < len; ++k) {
                        int x = meta.readShort();
                        int y = meta.readShort();
                        cb.lineTo(state.transformX(x), state.transformY(y));
                    }
                    cb.lineTo(state.transformX(sx), state.transformY(sy));
                    strokeAndFill();
                    break;
                }
                case META_POLYPOLYGON:
                {
                    if (isNullStrokeFill(false))
                        break;
                    int numPoly = meta.readWord();
                    int[] lens = new int[numPoly];
                    for (int k = 0; k < lens.Length; ++k)
                        lens[k] = meta.readWord();
                    for (int j = 0; j < lens.Length; ++j) {
                        int len = lens[j];
                        int sx = meta.readShort();
                        int sy = meta.readShort();
                        cb.moveTo(state.transformX(sx), state.transformY(sy));
                        for (int k = 1; k < len; ++k) {
                            int x = meta.readShort();
                            int y = meta.readShort();
                            cb.lineTo(state.transformX(x), state.transformY(y));
                        }
                        cb.lineTo(state.transformX(sx), state.transformY(sy));
                    }
                    strokeAndFill();
                    break;
                }
                case META_ELLIPSE:
                {
                    if (isNullStrokeFill(state.LineNeutral))
                        break;
                    int b = meta.readShort();
                    int r = meta.readShort();
                    int t = meta.readShort();
                    int l = meta.readShort();
                    cb.arc(state.transformX(l), state.transformY(b), state.transformX(r), state.transformY(t), 0, 360);
                    strokeAndFill();
                    break;
                }
                case META_ARC:
                {
                    if (isNullStrokeFill(state.LineNeutral))
                        break;
                    float yend = state.transformY(meta.readShort());
                    float xend = state.transformX(meta.readShort());
                    float ystart = state.transformY(meta.readShort());
                    float xstart = state.transformX(meta.readShort());
                    float b = state.transformY(meta.readShort());
                    float r = state.transformX(meta.readShort());
                    float t = state.transformY(meta.readShort());
                    float l = state.transformX(meta.readShort());
                    float cx = (r + l) / 2;
                    float cy = (t + b) / 2;
                    float arc1 = getArc(cx, cy, xstart, ystart);
                    float arc2 = getArc(cx, cy, xend, yend);
                    arc2 -= arc1;
                    if (arc2 <= 0)
                        arc2 += 360;
                    cb.arc(l, b, r, t, arc1, arc2);
                    cb.stroke();
                    break;
                }
                case META_PIE:
                {
                    if (isNullStrokeFill(state.LineNeutral))
                        break;
                    float yend = state.transformY(meta.readShort());
                    float xend = state.transformX(meta.readShort());
                    float ystart = state.transformY(meta.readShort());
                    float xstart = state.transformX(meta.readShort());
                    float b = state.transformY(meta.readShort());
                    float r = state.transformX(meta.readShort());
                    float t = state.transformY(meta.readShort());
                    float l = state.transformX(meta.readShort());
                    float cx = (r + l) / 2;
                    float cy = (t + b) / 2;
                    float arc1 = getArc(cx, cy, xstart, ystart);
                    float arc2 = getArc(cx, cy, xend, yend);
                    arc2 -= arc1;
                    if (arc2 <= 0)
                        arc2 += 360;
                    ArrayList ar = PdfContentByte.bezierArc(l, b, r, t, arc1, arc2);
                    if (ar.Count == 0)
                        break;
                    float[] pt = (float [])ar[0];
                    cb.moveTo(cx, cy);
                    cb.lineTo(pt[0], pt[1]);
                    for (int k = 0; k < ar.Count; ++k) {
                        pt = (float [])ar[k];
                        cb.curveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
                    }
                    cb.lineTo(cx, cy);
                    strokeAndFill();
                    break;
                }
                case META_CHORD:
                {
                    if (isNullStrokeFill(state.LineNeutral))
                        break;
                    float yend = state.transformY(meta.readShort());
                    float xend = state.transformX(meta.readShort());
                    float ystart = state.transformY(meta.readShort());
                    float xstart = state.transformX(meta.readShort());
                    float b = state.transformY(meta.readShort());
                    float r = state.transformX(meta.readShort());
                    float t = state.transformY(meta.readShort());
                    float l = state.transformX(meta.readShort());
                    float cx = (r + l) / 2;
                    float cy = (t + b) / 2;
                    float arc1 = getArc(cx, cy, xstart, ystart);
                    float arc2 = getArc(cx, cy, xend, yend);
                    arc2 -= arc1;
                    if (arc2 <= 0)
                        arc2 += 360;
                    ArrayList ar = PdfContentByte.bezierArc(l, b, r, t, arc1, arc2);
                    if (ar.Count == 0)
                        break;
                    float[] pt = (float [])ar[0];
                    cx = pt[0];
                    cy = pt[1];
                    cb.moveTo(cx, cy);
                    for (int k = 0; k < ar.Count; ++k) {
                        pt = (float [])ar[k];
                        cb.curveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
                    }
                    cb.lineTo(cx, cy);
                    strokeAndFill();
                    break;
                }
                case META_RECTANGLE:
                {
                    if (isNullStrokeFill(true))
                        break;
                    float b = state.transformY(meta.readShort());
                    float r = state.transformX(meta.readShort());
                    float t = state.transformY(meta.readShort());
                    float l = state.transformX(meta.readShort());
                    cb.rectangle(l, b, r - l, t - b);
                    strokeAndFill();
                    break;
                }
                case META_INTERSECTCLIPRECT:
                {
                    float b = state.transformY(meta.readShort());
                    float r = state.transformX(meta.readShort());
                    float t = state.transformY(meta.readShort());
                    float l = state.transformX(meta.readShort());
                    cb.rectangle(l, b, r - l, t - b);
                    cb.eoClip();
                    cb.newPath();
                    break;
                }
                case META_EXTTEXTOUT:
                {
                    int y = meta.readShort();
                    int x = meta.readShort();
                    int count = meta.readWord();
                    int flag = meta.readWord();
                    int x1 = 0;
                    int y1 = 0;
                    int x2 = 0;
                    int y2 = 0;
                    if ((flag & (MetaFont.ETO_CLIPPED | MetaFont.ETO_OPAQUE)) != 0) {
                        x1 = meta.readShort();
                        y1 = meta.readShort();
                        x2 = meta.readShort();
                        y2 = meta.readShort();
                    }
                    byte[] text = new byte[count];
                    int k;
                    for (k = 0; k < count; ++k) {
                        byte c = (byte)meta.readByte();
                        if (c == 0)
                            break;
                        text[k] = c;
                    }
                    string s;
                    try {
						s = System.Text.Encoding.GetEncoding("windows-1252").GetString(text, 0, k);
                    }
                    catch (Exception e) {
						e.GetType();
						s = System.Text.ASCIIEncoding.ASCII.GetString(text, 0, k);
                    }
                    outputText(x, y, flag, x1, y1, x2, y2, s);
                    break;
                }
                case META_TEXTOUT:
                {
                    int count = meta.readWord();
                    byte[] text = new byte[count];
                    int k;
                    for (k = 0; k < count; ++k) {
                        byte c = (byte)meta.readByte();
                        if (c == 0)
                            break;
                        text[k] = c;
                    }
                    string s;
                    try {
						s = System.Text.Encoding.GetEncoding("windows-1252").GetString(text, 0, k);
                    }
                    catch (Exception e) {
						e.GetType();
						s = System.Text.ASCIIEncoding.ASCII.GetString(text, 0, k);
                    }
                    count = (count + 1) & 0xfffe;
                    meta.skip(count - k);
                    int y = meta.readShort();
                    int x = meta.readShort();
                    outputText(x, y, 0, 0, 0, 0, 0, s);
                    break;
                }
                case META_SETBKCOLOR:
                    state.CurrentBackgroundColor = meta.readColor();
                    break;
                case META_SETTEXTCOLOR:
                    state.CurrentTextColor = meta.readColor();
                    break;
                case META_SETTEXTALIGN:
                    state.TextAlign = meta.readWord();
                    break;
                case META_SETBKMODE:
                    state.BackgroundMode = meta.readWord();
                    break;
                case META_SETPOLYFILLMODE:
                    state.PolyFillMode = meta.readWord();
                    break;
                case META_SETPIXEL:
                {
                    Color color = meta.readColor();
                    int y = meta.readShort();
                    int x = meta.readShort();
                    cb.saveState();
                    cb.ColorFill = color;
                    cb.rectangle(state.transformX(x), state.transformY(y), .2f, .2f);
                    cb.fill();
                    cb.restoreState();
                    break;
                }
            }
            meta.skip((tsize * 2) - (meta.Length - lenMarker));
        }
        
    }
    
    public void outputText(int x, int y, int flag, int x1, int y1, int x2, int y2, string text) {
        MetaFont font = state.CurrentFont;
        float refX = state.transformX(x);
        float refY = state.transformY(y);
        float sin = (float)Math.Sin(font.Angle);
        float cos = (float)Math.Cos(font.Angle);
        float fontSize = font.getFontSize(state);
        BaseFont bf = font.Font;
        int align = state.TextAlign;
        float textWidth = bf.getWidthPoint(text, fontSize);
        float tx = 0;
        float ty = 0;
        float descender = bf.getFontDescriptor(BaseFont.DESCENT, fontSize);
        float ury = bf.getFontDescriptor(BaseFont.BBOXURY, fontSize);
        cb.saveState();
        cb.concatCTM(cos, sin, -sin, cos, refX, refY);
        if ((align & MetaState.TA_CENTER) == MetaState.TA_CENTER)
            tx = -textWidth / 2;
        else if ((align & MetaState.TA_RIGHT) == MetaState.TA_RIGHT)
            tx = -textWidth;
        if ((align & MetaState.TA_BASELINE) == MetaState.TA_BASELINE)
            ty = 0;
        else if ((align & MetaState.TA_BOTTOM) == MetaState.TA_BOTTOM)
            ty = -descender;
        else
            ty = -ury;
        Color textColor;
        if (state.BackgroundMode == MetaState.OPAQUE) {
            textColor = state.CurrentBackgroundColor;
            cb.ColorFill = textColor;
            cb.rectangle(tx, ty + descender, textWidth, ury - descender);
            cb.fill();
        }
        textColor = state.CurrentTextColor;
        cb.ColorFill = textColor;
        cb.beginText();
        cb.setFontAndSize(bf, fontSize);
        cb.setTextMatrix(tx, ty);
        cb.showText(text);
        cb.endText();
        if (font.isUnderline()) {
            cb.rectangle(tx, ty - fontSize / 4, textWidth, fontSize / 15);
            cb.fill();
        }
        if (font.isStrikeout()) {
            cb.rectangle(tx, ty + fontSize / 3, textWidth, fontSize / 15);
            cb.fill();
        }
        cb.restoreState();
    }
    
    public bool isNullStrokeFill(bool isRectangle) {
        MetaPen pen = state.CurrentPen;
        MetaBrush brush = state.CurrentBrush;
        bool noPen = (pen.Style == MetaPen.PS_NULL);
        int style = brush.Style;
        bool isBrush = (style == MetaBrush.BS_SOLID || (style == MetaBrush.BS_HATCHED && state.BackgroundMode == MetaState.OPAQUE));
        bool result = noPen && !isBrush;
        if (!noPen) {
            if (isRectangle)
                state.LineJoinRectangle = cb;
            else
                state.LineJoinPolygon = cb;
        }
        return result;
    }

    public void strokeAndFill(){
        MetaPen pen = state.CurrentPen;
        MetaBrush brush = state.CurrentBrush;
        int penStyle = pen.Style;
        int brushStyle = brush.Style;
        if (penStyle == MetaPen.PS_NULL) {
            cb.closePath();
            if (state.PolyFillMode == MetaState.ALTERNATE) {
                cb.eoFill();
            }
            else {
                cb.fill();
            }
        }
        else {
            bool isBrush = (brushStyle == MetaBrush.BS_SOLID || (brushStyle == MetaBrush.BS_HATCHED && state.BackgroundMode == MetaState.OPAQUE));
            if (isBrush) {
                if (state.PolyFillMode == MetaState.ALTERNATE)
                    cb.closePathEoFillStroke();
                else
                    cb.closePathFillStroke();
            }
            else {
                cb.closePathStroke();
            }
        }
    }
    
    internal static float getArc(float xCenter, float yCenter, float xDot, float yDot) {
        double s = Math.Atan2(yDot - yCenter, xDot - xCenter);
        if (s < 0)
            s += Math.PI * 2;
        return (float)(s / Math.PI * 180);
    }
	}
}
