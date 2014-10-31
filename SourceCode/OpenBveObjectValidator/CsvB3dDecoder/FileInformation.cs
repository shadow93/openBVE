using System;
using TrainsimApi.Codecs;

namespace CsvB3dDecoder {
	internal class FileInformation {
		
		
		// --- members ---
		
		internal bool IsB3d;
		
		internal string File;
		
		internal bool StrictParsing;
		
		internal ErrorLogger Logger;
		
		
		// --- constructors ---
		
		internal FileInformation(bool isB3d, string file, bool strictParsing, ErrorLogger logger) {
			this.IsB3d = isB3d;
			this.File = file;
			this.StrictParsing = strictParsing;
			this.Logger = logger;
		}
		
		
	}
}
