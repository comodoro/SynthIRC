using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Log;
using System.IO;
namespace SynthIRC
{
    class Logger
        
    {
        private StreamWriter sw;

        public Logger(string filename)
        {
            sw = new StreamWriter(filename);
        }

        public void appendLine(string line)
        {
            sw.WriteLine(line);
            sw.Flush();
        }
    /*
        private FileRecordSequence sequence = null;

        public Logger(string filename) {
            sequence = new FileRecordSequence(filename, FileAccess.ReadWrite);
        }

        public void appendLine(String text)
        {
            SequenceNumber previous = SequenceNumber.Invalid;
            previous = sequence.Append(CreateData(text + "\n"), SequenceNumber.Invalid, SequenceNumber.Invalid, RecordAppendOptions.ForceFlush);
        }

        public static IList<ArraySegment<byte>> CreateData(string str)
        {
            Encoding enc = Encoding.ASCII;
            byte[] array = enc.GetBytes(str);
            ArraySegment<byte>[] segments = new ArraySegment<byte>[1];
            segments[0] = new ArraySegment<byte>(array);
            return Array.AsReadOnly<ArraySegment<byte>>(segments);
        }

      */
    }
}
