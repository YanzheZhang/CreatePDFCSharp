using System;

namespace iTextSharp.text {
	/// <summary>
	/// Base class for Color, serves as wrapper class for <see cref="T:System.Drawing.Color"/>
	/// to allow extension.
	/// </summary>
	public class Color {
		System.Drawing.Color color;

		/// <summary>
		/// Constuctor for Color object.
		/// </summary>
		/// <param name="red">The red component value for the new Color structure. Valid values are 0 through 255.</param>
		/// <param name="green">The green component value for the new Color structure. Valid values are 0 through 255.</param>
		/// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 255.</param>
		public Color(int red, int green, int blue) {
			color = System.Drawing.Color.FromArgb(red, green, blue);
		}

		/// <summary>
		/// Constructor for Color object
		/// </summary>
		/// <param name="red">The red component value for the new Color structure. Valid values are 0 through 1.</param>
		/// <param name="green">The green component value for the new Color structure. Valid values are 0 through 1.</param>
		/// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 1.</param>
		public Color(float red, float green, float blue) {
			color = System.Drawing.Color.FromArgb((int)(red * 255 + .5), (int)(red * 255 + .5), (int)(red * 255 + .5));
		}

		/// <summary>
		/// Constructor for Color object
		/// </summary>
		/// <param name="color">a Color object</param>
		/// <overloads>
		/// Has three overloads.
		/// </overloads>
		public Color(System.Drawing.Color color) {
			this.color = color;
		}

		/// <summary>
		/// Gets the red component value of this <see cref="T:System.Drawing.Color"/> structure.
		/// </summary>
		/// <value>The red component value of this <see cref="T:System.Drawing.Color"/> structure.</value>
		public int R {
			get {
				return color.R;
			}
		}

		/// <summary>
		/// Gets the green component value of this <see cref="T:System.Drawing.Color"/> structure.
		/// </summary>
		/// <value>The green component value of this <see cref="T:System.Drawing.Color"/> structure.</value>
		public int G {
			get {
				return color.G;
			}
		}

		/// <summary>
		/// Gets the blue component value of this <see cref="T:System.Drawing.Color"/> structure.
		/// </summary>
		/// <value>The blue component value of this <see cref="T:System.Drawing.Color"/> structure.</value>
		public int B {
			get {
				return color.B;
			}
		}
	}
}
