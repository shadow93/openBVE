using System;

namespace OpenBveApi.Math {
	/// <summary>Represents an orientation in three-dimensional space.</summary>
	public struct Orientation3f {

		// --- members ---

		/// <summary>The vector pointing right.</summary>
		public Vector3f X;

		/// <summary>The vector pointing up.</summary>
		public Vector3f Y;

		/// <summary>The vector pointing forward.</summary>
		public Vector3f Z;


		// --- constructors ---

		/// <summary>Creates a new orientation in three-dimensional space.</summary>
		/// <param name="x">The vector pointing right.</param>
		/// <param name="y">The vector pointing up.</param>
		/// <param name="z">The vector pointing forward.</param>
		public Orientation3f(Vector3f x, Vector3f y, Vector3f z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}


		// --- read-only fields ---

		/// <summary>Represents a null orientation.</summary>
		public static readonly Orientation3f Null = new Orientation3f(Vector3f.Null, Vector3f.Null, Vector3f.Null);

		/// <summary>Represents the default orientation with X = {1, 0, 0}, Y = {0, 1, 0} and Z = {0, 0, 1}.</summary>
		public static readonly Orientation3f Default = new Orientation3f(Vector3f.Right, Vector3f.Up, Vector3f.Forward);

	}
}
