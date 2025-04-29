using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ConsoleAppV3.Interface;
using ConsoleAppV3.Models;

namespace ConsoleAppV3.Repositories
{
    public class NoteRepository : INoteArchive
    {
        private const string NoteFilePath = "notes.json";
        private List<Note> _notes = new List<Note>();
        private int _nextId = 1;

        public NoteRepository()
        {
            LoadNote();
        }

        public IEnumerable<Note> GetNotes() => _notes.AsReadOnly();

        public Note SearchNoteById(int id) => 
            _notes.FirstOrDefault(n => n.Id == id) ?? throw new Exception("Note not found");

        public void AddNote(Note note)
        {
            note.Id = _nextId++;
            _notes.Add(note);
            SaveNote();
        }

        public void ChangeNote(Note note)
        {
            var index = _notes.FindIndex(n => n.Id == note.Id);
            if (index != -1)
            {
                _notes[index] = note;
                SaveNote();
            }
        }

        public void RemoveNote(int id)
        {
            var note = _notes.FirstOrDefault(n => n.Id == id);
            if (note != null)
            {
                _notes.Remove(note);
                SaveNote();
            }
        }

        public void SaveNote()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(NoteFilePath, JsonSerializer.Serialize(_notes, options));
        }

        public void LoadNote()
        {
            try
            {
                if (File.Exists(NoteFilePath))
                {
                    var json = File.ReadAllText(NoteFilePath);
                    _notes = JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
                    _nextId = _notes.Count > 0 ? _notes.Max(n => n.Id) + 1 : 1;
                }
            }
            catch
            {
                _notes = new List<Note>();
                _nextId = 1;
            }
        }
    }
}