using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppV3.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime NoteCreationTime { get; private set; }
        public bool IsCompleted { get; set; }
        public User User { get; set; }
        public Note() { }
        public Note(string title, User user)
        {
            Title = title;
            User = user;
            NoteCreationTime = DateTime.Now;
        }
    }
}