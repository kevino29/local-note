using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalNote.Models
{
    public class NoteModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Title { get; set; }
        public string Content { get; set; }
        private bool needSaving = false;

        public NoteModel()
        {
            this.Title = "Untitled Note";
            this.Content = "";
        }

        public NoteModel(string title, string content)
        {
            this.Title = title;
            this.Content = content;
        }

        public bool NeedSaving
        {
            get { return this.needSaving; }
            set { this.needSaving = value; }
        }

        public void FirePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
