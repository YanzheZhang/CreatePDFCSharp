using System;
using System.Text;

/*
 * $Id: Hyphen.cs,v 1.1.1.1 2003/02/04 02:58:41 geraldhenson Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation {
	/**
	 * This class represents a hyphen. A 'full' hyphen is made of 3 parts:
	 * the pre-break text, post-break text and no-break. If no line-break
	 * is generated at this position, the no-break text is used, otherwise,
	 * pre-break and post-break are used. Typically, pre-break is equal to
	 * the hyphen character and the others are empty. However, this general
	 * scheme allows support for cases in some languages where words change
	 * spelling if they're split across lines, like german's 'backen' which
	 * hyphenates 'bak-ken'. BTW, this comes from TeX.
	 *
	 * @author Carlos Villegas <cav@uniscope.co.jp>
	 */

	public class Hyphen {
		public string preBreak;
		public string noBreak;
		public string postBreak;

		internal Hyphen(string pre, string no, string post) {
			preBreak = pre;
			noBreak = no;
			postBreak = post;
		}

		internal Hyphen(string pre) {
			preBreak = pre;
			noBreak = null;
			postBreak = null;
		}

		public override string ToString() {
			if (noBreak == null && postBreak == null && preBreak != null
				&& preBreak.Equals("-"))
				return "-";
			StringBuilder res = new StringBuilder("{");
			res.Append(preBreak);
			res.Append("}{");
			res.Append(postBreak);
			res.Append("}{");
			res.Append(noBreak);
			res.Append('}');
			return res.ToString();
		}
	}
}
