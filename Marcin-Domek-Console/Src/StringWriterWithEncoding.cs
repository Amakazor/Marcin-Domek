﻿using System.IO;
using System.Text;

namespace Marcin_Domek_Console.Src
{
    internal class StringWriterWithEncoding : StringWriter
    {
        public override Encoding Encoding => _encoding ?? base.Encoding;
        private readonly Encoding _encoding;

        public StringWriterWithEncoding() { }

        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }
    }
}