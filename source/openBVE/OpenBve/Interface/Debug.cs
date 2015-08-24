using System;
using System.Collections.Generic;
namespace OpenBve
{
	internal static class Debug
    {
		// messages
		internal enum MessageType {
			Warning,
			Error,
			Critical
		}
		internal struct Message {
			public Message(MessageType type, bool filenotfound, string text){
				this.Type = type;
				this.FileNotFound = filenotfound;
				this.Text = text;
			}
			internal readonly MessageType Type;
			internal readonly bool FileNotFound;
			internal readonly string Text;
		}
		internal static List<Message> Messages = new List<Message>();
		internal static int MessageCount {
			get{
				return Messages != null ? Messages.Count : 0;
			}
		}
		internal static void AddMessage(MessageType Type, bool FileNotFound, string Text) {
			if (Type == MessageType.Warning & !Options.Current.ShowWarningMessages) return;
			if (Type == MessageType.Error & !Options.Current.ShowErrorMessages) return;
			Message msg = new Message(Type,FileNotFound,Text);
			Messages.Add(msg);

			Program.AppendToLogFile(Text);
		}
		internal static void ClearMessages() {
			Messages.Clear();
		}
    }
}

