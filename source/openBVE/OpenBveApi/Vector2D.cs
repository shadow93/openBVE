#pragma warning disable 0660, 0661

using System;

namespace OpenBveApi.Math {
	/// <summary>Represents a two-dimensional vector with double precision.</summary>
	public struct Vector2D {
		
		// --- members ---
		
		/// <summary>The x-coordinate.</summary>
		public double X;
		
		/// <summary>The y-coordinate.</summary>
		public double Y;
		
		
		// --- constructors ---
		
		/// <summary>Creates a new two-dimensional vector.</summary>
		/// <param name="x">The x-coordinate.</param>
		/// <param name="y">The y-coordinate.</param>
		public Vector2D(double x, double y) {
			this.X = x;
			this.Y = y;
		}
		
		
		// --- arithmetic operators ---
		
		/// <summary>Adds two vectors.</summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The sum of the two vectors.</returns>
		public static Vector2D operator +(Vector2D a, Vector2D b) {
			return new Vector2D(a.X + b.X, a.Y + b.Y);
		}
		
		/// <summary>Adds a vector and a scalar.</summary>
		/// <param name="a">The vector.</param>
		/// <param name="b">The scalar.</param>
		/// <returns>The sum of the vector and the scalar.</returns>
		public static Vector2D operator +(Vector2D a, double b) {
			return new Vector2D(a.X + b, a.Y + b);
		}
		
		/// <summary>Adds a scalar and a vector.</summary>
		/// <param name="a">The scalar.</param>
		/// <param name="b">The vector.</param>
		/// <returns>The sum of the scalar and the vector.</returns>
		public static Vector2D operator +(double a, Vector2D b) {
			return new Vector2D(a + b.X, a + b.Y);
		}
		
		/// <summary>Subtracts two vectors.</summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The difference of the two vectors.</returns>
		public static Vector2D operator -(Vector2D a, Vector2D b) {
			return new Vector2D(a.X - b.X, a.Y - b.Y);
		}
		
		/// <summary>Subtracts a scalar from a vector.</summary>
		/// <param name="a">The vector.</param>
		/// <param name="b">The scalar.</param>
		/// <returns>The difference of the vector and the scalar.</returns>
		public static Vector2D operator -(Vector2D a, double b) {
			return new Vector2D(a.X - b, a.Y - b);
		}
		
		/// <summary>Subtracts a vector from a scalar.</summary>
		/// <param name="a">The scalar.</param>
		/// <param name="b">The vector.</param>
		/// <returns>The difference of the scalar and the vector.</returns>
		public static Vector2D operator -(double a, Vector2D b) {
			return new Vector2D(a - b.X, a - b.Y);
		}
		
		/// <summary>Negates a vector.</summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The negation of the vector.</returns>
		public static Vector2D operator -(Vector2D vector) {
			return new Vector2D(-vector.X, -vector.Y);
		}
		
		/// <summary>Multiplies two vectors.</summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The product of the two vectors.</returns>
		public static Vector2D operator *(Vector2D a, Vector2D b) {
			return new Vector2D(a.X * b.X, a.Y * b.Y);
		}
		/// <summary>Multiplies a vector and a scalar.</summary>
		/// <param name="a">The vector.</param>
		/// <param name="b">The scalar.</param>
		/// <returns>The product of the vector and the scalar.</returns>
		public static Vector2D operator *(Vector2D a, double b) {
			return new Vector2D(a.X * b, a.Y * b);
		}
		
		/// <summary>Multiplies a scalar and a vector.</summary>
		/// <param name="a">The scalar.</param>
		/// <param name="b">The vector.</param>
		/// <returns>The product of the scalar and the vector.</returns>
		public static Vector2D operator *(double a, Vector2D b) {
			return new Vector2D(a * b.X, a * b.Y);
		}
		
		/// <summary>Divides two vectors.</summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The quotient of the two vectors.</returns>
		/// <exception cref="System.DivideByZeroException">Raised when any member of the second vector is zero.</exception>
		public static Vector2D operator /(Vector2D a, Vector2D b) {
			if (b.X == 0.0 | b.Y == 0.0) {
				throw new DivideByZeroException();
			} else {
				return new Vector2D(a.X / b.X, a.Y / b.Y);
			}
		}
		
		/// <summary>Divides a vector by a scalar.</summary>
		/// <param name="a">The vector.</param>
		/// <param name="b">The scalar.</param>
		/// <returns>The quotient of the vector and the scalar.</returns>
		/// <exception cref="System.DivideByZeroException">Raised when the scalar is zero.</exception>
		public static Vector2D operator /(Vector2D a, double b) {
			if (b == 0.0) {
				throw new DivideByZeroException();
			} else {
				double factor = 1.0 / b;
				return new Vector2D(a.X * factor, a.Y * factor);
			}
		}
		
		/// <summary>Divides a scalar by a vector.</summary>
		/// <param name="a">The scalar.</param>
		/// <param name="b">The vector.</param>
		/// <returns>The quotient of the scalar and the vector.</returns>
		/// <exception cref="DivideByZeroException">Raised when any member of the vector is zero.</exception>
		public static Vector2D operator /(double a, Vector2D b) {
			if (b.X == 0.0 | b.Y == 0.0) {
				throw new DivideByZeroException();
			} else {
				return new Vector2D(a / b.X, a / b.Y);
			}
		}
		
		
		// --- comparisons ---
		
		/// <summary>Checks whether the two specified vectors are equal.</summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>Whether the two vectors are equal.</returns>
		public static bool operator ==(Vector2D a, Vector2D b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			return true;
		}
		
		/// <summary>Checks whether the two specified vectors are unequal.</summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>Whether the two vectors are unequal.</returns>
		public static bool operator !=(Vector2D a, Vector2D b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			return false;
		}
		
		
		// --- instance functions ---
		
		/// <summary>Normalizes the vector.</summary>
		/// <exception cref="System.DivideByZeroException">Raised when the vector is a null vector.</exception>
		public void Normalize() {
			double norm = this.X * this.X + this.Y * this.Y;
			if (norm == 0.0) {
				throw new DivideByZeroException();
			} else {
				double factor = 1.0 / System.Math.Sqrt(norm);
				this.X *= factor;
				this.Y *= factor;
			}
		}
		
		/// <summary>Translates the vector by a specified offset.</summary>
		/// <param name="offset">The offset.</param>
		public void Translate(Vector2D offset) {
			this.X += offset.X;
			this.Y += offset.Y;
		}
		
		/// <summary>Scales the vector by a specified factor.</summary>
		/// <param name="factor">The factor.</param>
		public void Scale(Vector2D factor) {
			this.X *= factor.X;
			this.Y *= factor.Y;
		}
		
		/// <summary>Rotates the vector by the specified angle.</summary>
		/// <param name="cosineOfAngle">The cosine of the angle.</param>
		/// <param name="sineOfAngle">The sine of the angle.</param>
		public void Rotate(double cosineOfAngle, double sineOfAngle) {
			double x = cosineOfAngle * this.X - sineOfAngle * this.Y;
			double y = sineOfAngle * this.X + cosineOfAngle * this.Y;
			this = new Vector2D(x, y);
		}
		
		/// <summary>Checks whether the vector is a null vector.</summary>
		/// <returns>A boolean indicating whether the vector is a null vector.</returns>
		public bool IsNullVector() {
			return this.X == 0.0 & this.Y == 0.0;
		}
		
		/// <summary>Checks whether the vector is considered a null vector.</summary>
		/// <param name="tolerance">The highest absolute value that each component of the vector may have before the vector is not considered a null vector.</param>
		/// <returns>A boolean indicating whether the vector is considered a null vector.</returns>
		public bool IsNullVector(double tolerance) {
			if (this.X < -tolerance) return false;
			if (this.X > tolerance) return false;
			if (this.Y < -tolerance) return false;
			if (this.Y > tolerance) return false;
			return true;
		}
		
		/// <summary>Gets the euclidean norm.</summary>
		/// <returns>The euclidean norm.</returns>
		public double Norm() {
			return System.Math.Sqrt(this.X * this.X + this.Y * this.Y);
		}
		
		/// <summary>Gets the square of the euclidean norm.</summary>
		/// <returns>The square of the euclidean norm.</returns>
		public double NormSquared() {
			return this.X * this.X + this.Y * this.Y;
		}

		
		// --- static functions ---
		
		/// <summary>Gives the dot product of two vectors.</summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		public static double Dot(Vector2D a, Vector2D b) {
			return a.X * b.X + a.Y * b.Y;
		}
		
		/// <summary>Normalizes a vector.</summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The normalized vector.</returns>
		/// <exception cref="System.DivideByZeroException">Raised when the vector is a null vector.</exception>
		public static Vector2D Normalize(Vector2D vector) {
			double norm = vector.X * vector.X + vector.Y * vector.Y;
			if (norm == 0.0) {
				throw new DivideByZeroException();
			} else {
				double factor = 1.0 / System.Math.Sqrt(norm);
				return new Vector2D(vector.X * factor, vector.Y * factor);
			}
		}
		
		/// <summary>Translates a vector by a specified offset.</summary>
		/// <param name="vector">The vector.</param>
		/// <param name="offset">The offset.</param>
		/// <returns>The translated vector.</returns>
		public static Vector2D Translate(Vector2D vector, Vector2D offset) {
			double x = vector.X + offset.X;
			double y = vector.Y + offset.Y;
			return new Vector2D(x, y);
		}
		
		/// <summary>Scales a vector by a specified factor.</summary>
		/// <param name="vector">The vector.</param>
		/// <param name="factor">The factor.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector2D Scale(Vector2D vector, Vector2D factor) {
			double x = vector.X * factor.X;
			double y = vector.Y * factor.Y;
			return new Vector2D(x, y);
		}
		
		/// <summary>Rotates a vector by a specified angle.</summary>
		/// <param name="vector">The vector.</param>
		/// <param name="cosineOfAngle">The cosine of the angle.</param>
		/// <param name="sineOfAngle">The sine of the angle.</param>
		/// <returns>The rotated vector.</returns>
		public static Vector2D Rotate(Vector2D vector, double cosineOfAngle, double sineOfAngle) {
			double x = cosineOfAngle * vector.X - sineOfAngle * vector.Y;
			double y = sineOfAngle * vector.X + cosineOfAngle * vector.Y;
			return new Vector2D(x, y);

		}
		
		/// <summary>Checks whether a vector is a null vector.</summary>
		/// <returns>A boolean indicating whether the vector is a null vector.</returns>
		public static bool IsNullVector(Vector2D vector) {
			return vector.X == 0.0 & vector.Y == 0.0;
		}
		
		/// <summary>Gets the euclidean norm of the specified vector.</summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The euclidean norm.</returns>
		public static double Norm(Vector2D vector) {
			return System.Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
		}
		
		/// <summary>Gets the square of the euclidean norm of the specified vector.</summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The square of the euclidean norm.</returns>
		public static double NormSquared(Vector2D vector) {
			return vector.X * vector.X + vector.Y * vector.Y;
		}
		
		
		// --- read-only fields ---
		
		/// <summary>Represents a null vector.</summary>
		public static readonly Vector2D Null = new Vector2D(0.0, 0.0);
		
		/// <summary>Represents a vector pointing left.</summary>
		public static readonly Vector2D Left = new Vector2D(-1.0, 0.0);
		
		/// <summary>Represents a vector pointing right.</summary>
		public static readonly Vector2D Right = new Vector2D(1.0, 0.0);
		
		/// <summary>Represents a vector pointing up.</summary>
		public static readonly Vector2D Up = new Vector2D(0.0, -1.0);
		
		/// <summary>Represents a vector pointing down.</summary>
		public static readonly Vector2D Down = new Vector2D(0.0, 1.0);
		
	}
}