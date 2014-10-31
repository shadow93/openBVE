using System;

namespace CsvB3dDecoder {
	internal class LineInformation {
		
		
		// --- members ---
		
		internal string Command;
		
		internal string[] Arguments;
		
		internal int ArgumentCount;
		
		internal int LineNumber;
		
		internal FileInformation FileInfo;
		
		
		// --- constructors ---
		
		internal LineInformation(string command, string[] arguments, int argumentCount, int lineNumber, FileInformation fileInfo) {
			this.Command = command;
			this.Arguments = arguments;
			this.ArgumentCount = argumentCount;
			this.LineNumber = lineNumber;
			this.FileInfo = fileInfo;
		}
		
		
	}
}
