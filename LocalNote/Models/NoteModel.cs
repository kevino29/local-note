using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalNote.Models {
    public class NoteModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Title { get; set; }
        public ContentModel Content { get; set; }
        private bool needSaving = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteModel() {
            this.Title = "Untitled Note";
            this.Content = new ContentModel();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public NoteModel(string title, ContentModel content) {
            this.Title = title;
            this.Content = content;
        }

        /// <summary>
        /// Gets and sets the need saving property.
        /// </summary>
        public bool NeedSaving {
            get { return this.needSaving; }
            set { this.needSaving = value; }
        }

        /// <summary>
        /// Fire the PropertyChanged event based on the given property name.
        /// </summary>
        /// <param name="property"></param>
        public void FirePropertyChanged(string property) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
