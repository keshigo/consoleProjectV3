using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleAppV3.Models;
using System.Collections.Generic;

namespace ConsoleAppV3.Interface
{
    public interface INoteArchive
    {
        IEnumerable<Note> GetNotes();
        Note SearchNoteById(int id);
        void AddNote(Note note);
        void ChangeNote(Note note);
        void RemoveNote(int id);
        void SaveNote();
        void LoadNote();
    }
}