using System;
using TrainsimApi.Vectors;

namespace TrainsimApi.Geometry {
	public class BveFormRoofCrackTransformation {
		
		
		// --- members ---
		
		private double NearDistance;
		
		private double FarDistance;
		
		
		// --- constructors ---
		
		public BveFormRoofCrackTransformation(double nearDistance, double farDistance) {
			this.NearDistance = nearDistance;
			this.FarDistance = farDistance;
		}
		
		
		// --- functions ---
		
		public void Transform(ref Vector3d p0, ref Vector3d p1, Vector3d p2, Vector3d p3, ref Vector3d p4, ref Vector3d p5, Vector3d p6, Vector3d p7) {
			p0.X = this.NearDistance - p3.X;
			p1.X = this.FarDistance  - p2.X;
			p4.X = this.NearDistance - p7.X;
			p5.X = this.NearDistance - p6.X;
		}
		
		
	}
}