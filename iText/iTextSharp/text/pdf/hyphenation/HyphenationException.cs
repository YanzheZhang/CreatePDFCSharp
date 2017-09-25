using System;

/*
 * $Id: HyphenationException.cs,v 1.1.1.1 2003/02/04 02:58:41 geraldhenson Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation
{
	/**
	 * @author Carlos Villegas <cav@uniscope.co.jp>
	 */
	public class HyphenationException : Exception
	{
		public HyphenationException() : base() {}

		public HyphenationException(string message) : base(message) {}
	}
}
