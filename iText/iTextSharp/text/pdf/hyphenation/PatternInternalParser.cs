using System;
using System.IO;
using System.Text;
using System.Collections;
using System.util;

/*
 * $Id: PatternInternalParser.cs,v 1.1.1.1 2003/02/04 02:58:43 geraldhenson Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation {
	/// <summary>
	/// Summary description for PatternInternalParser.
	/// </summary>
	public class PatternInternalParser : IPatternConsumer {

		IPatternConsumer consumer;

		public PatternInternalParser() {
		}

		public PatternInternalParser(IPatternConsumer consumer) {
			this.consumer = consumer;
		}

		public IPatternConsumer Consumer {
			set {
				this.consumer = value;
			}
		}
    
		protected string getHyphstring(Stream istr) {
			StreamReader isr = new StreamReader(istr, System.Text.Encoding.UTF8);
			char[] c = new char[4000];
			StringBuilder buf = new StringBuilder();
			while (true) {
				int n = isr.Read(c, 0, c.Length);
				if (n < 0)
					break;
				buf.Append(c, 0, n);
			}
			isr.Close();
			return buf.ToString();
		}

		public void parse(string filename) {
			Stream istr;
			try {
				istr = new FileStream(filename, System.IO.FileMode.Open);
			}
			catch (IOException ioe) {
				throw ioe;
			}
			parse(istr);
		}
    
		public void parse(Stream istr) {
			string hyphs;
			try {
				hyphs = getHyphstring(istr);
			}
			catch (IOException ioe) {
				throw ioe;
			}
			parsestring(hyphs);
		}

		public void parsestring(string hyphs) {
			StringTokenizer tk = new StringTokenizer(hyphs);
			readClasses(tk);
			readExceptions(tk);
			readPatterns(tk);
		}

		protected void readClasses(StringTokenizer tk) {
			string token = "";
			while (tk.hasMoreTokens()) {
				token = tk.nextToken();
				if (token.Equals("*"))
					break;
				consumer.addClass(token);
			}
		}

		protected void readExceptions(StringTokenizer tk) {
			string token = "";
			while (tk.hasMoreTokens()) {
				token = tk.nextToken();
				if (token.Equals("*"))
					break;
				string word = token;
				ArrayList vec = new ArrayList();
				while (tk.hasMoreTokens()) {
					token = tk.nextToken();
					if (token.Equals("{")) {
						string t1 = tk.nextToken();
						if (t1.Equals("N"))
							t1 = null;
						string t2 = tk.nextToken();
						if (t2.Equals("N"))
							t2 = null;
						string t3 = tk.nextToken();
						if (t3.Equals("N"))
							t3 = null;
						Hyphen hy = new Hyphen(t2, t1, t3);
						vec.Add(hy);
					}
					else if (token.Equals("#")) {
						break;
					}
					else
						vec.Add(token);
				}
				consumer.addException(word, vec);
			}
		}
    
		protected void readPatterns(StringTokenizer tk) {
			string token = "";
			while (tk.hasMoreTokens()) {
				token = tk.nextToken();
				consumer.addPattern(token, tk.nextToken());
			}
		}
    
		// IPatternConsumer implementation for testing purposes
		public void addClass(string c) {
			Console.Error.WriteLine("class: " + c);
		}

		public void addException(string w, ArrayList e) {
			Console.Error.WriteLine("exception: " + w + " : " + e.ToString());
		}

		public void addPattern(string p, string v) {
			Console.Error.WriteLine("pattern: " + p + " : " + v);
		}
	}
}
