using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalNote.Models;

namespace LocalNote.ViewModels
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string noteTitle;
        private string noteContent;
        private NoteModel selectedNote;
        private ObservableCollection<NoteModel> notes;

        public ObservableCollection<NoteModel> Notes { get { return notes; } }

        public NoteModel SelectedNote
        {
            get { return selectedNote; }
            set
            {
                selectedNote = value;
                if (value == null)
                {
                    NoteContent = "Select a note to see the content";
                }
                else
                {
                    noteTitle = value.NoteTitle;
                    noteContent = value.NoteContent;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoteTitle"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoteContent"));
            }
        }

        public string NoteTitle {
            get { return noteTitle; }
            set { noteTitle = value; }
        }

        public string NoteContent
        {
            get { return noteContent; }
            set { noteContent = value; }
        }

        public NoteViewModel()
        {
            notes = new ObservableCollection<NoteModel>();

            for (int i = 1; i <= 100; i++)
            {
                notes.Add(new NoteModel("Title " + i, i +  " Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                    "Ut eleifend dui eu leo bibendum hendrerit. Integer vehicula, nisi sed sodales aliquet, " +
                    "ipsum sapien malesuada nunc, tristique accumsan quam libero sed erat. " +
                    "Nulla nunc leo, mattis non massa efficitur, semper ullamcorper nisi. " +
                    "Morbi quis porttitor arcu. Maecenas risus lacus, tempor et tempor commodo, porta ut magna. " +
                    "Aliquam at tempus eros, nec scelerisque ipsum. Cras vel purus eget tortor accumsan " +
                    "lobortis non et ex."));
            }
        }
    }
}
