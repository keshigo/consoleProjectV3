using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppV3.Exceptions
{
    [Serializable]
    internal class NoteNotFoundException : Exception
    {
        public NoteNotFoundException() { }
        public NoteNotFoundException(string message) : base(message) { }
        public NoteNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}