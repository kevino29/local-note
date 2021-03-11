﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalNote.Models
{
    public class NoteModel
    {
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }

        public NoteModel()
        {
            this.NoteTitle = "Untitled Note";
            this.NoteContent = "";
        }

        public NoteModel(string content)
        {
            this.NoteTitle = "Untitled Note";
            this.NoteContent = content;
        }

        public NoteModel(string title, string content)
        {
            this.NoteTitle = title;
            this.NoteContent = content;
        }
    }
}