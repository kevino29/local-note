using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalNote.Models;
using LocalNote.Commands;

namespace LocalNote.ViewModels
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string noteTitle;
        private string noteContent;
        private string filter;
        private NoteModel selectedNote;
        private ObservableCollection<NoteModel> notes;
        private ObservableCollection<NoteModel> notesForLV;

        public ObservableCollection<NoteModel> Notes { get { return notes; } }
        public ObservableCollection<NoteModel> NotesForLV { get { return notesForLV; } }
        public SaveCommand SaveCommand { get; }
        public AddCommand AddCommand { get; }

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NoteTitle)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NoteContent)));
            }
        }

        public void FireSelectedNotePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNote)));
        }

        public string NoteTitle {
            get { return noteTitle; }
            set 
            {
                if (SelectedNote != null)
                {
                    SelectedNote.NoteTitle = value;
                    SaveCommand.FireCanExecuteChanged();
                }
                else
                    noteTitle = value;
            }
        }

        public string NoteContent
        {
            get { return noteContent; }
            set
            {
                if (SelectedNote != null)
                {
                    SelectedNote.NoteContent = value;
                    SaveCommand.FireCanExecuteChanged();
                }
                else
                    noteContent = value;
            }
        }

        public string Filter
        {
            get { return this.filter; }
            set 
            {
                if (value == filter) { return; }
                this.filter = value;
                PerformFilter();
            }
        }

        public void PerformFilter()
        {
            if (this.filter == null)
            {
                filter = "";
                NotesForLV.Clear();
                foreach (var note in Notes)
                {
                    NotesForLV.Add(note);
                }
            }

            // If filter has a value, then lowercase and trim the string
            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();

            // Use LINQ query to get all the notes that match the filter text
            // Then turn it into a list
            var result = Notes.Where(note => note.NoteTitle.ToLowerInvariant()
                .Contains(lowerCaseFilter))
                .ToList();

            // Get the notes that we want to remove
            var toRemove = NotesForLV.Except(result).ToList();

            // Remove the notes that does not match the filter
            foreach (var x in toRemove)
            {
                NotesForLV.Remove(x);
            }

            // Add all the notes that matches the filter to the list that is shown to the user
            for (int i = 0; i < result.Count; i++)
            {
                var resultItem = result[i];
                if (i + 1 > NotesForLV.Count || !NotesForLV[i].Equals(resultItem))
                {
                    NotesForLV.Insert(i, resultItem);
                }
            }
        }

        public NoteViewModel()
        {
            notes = new ObservableCollection<NoteModel>();
            notesForLV = new ObservableCollection<NoteModel>();
            SaveCommand = new SaveCommand(this);
            AddCommand = new AddCommand(this);

            for (int i = 1; i <= 100; i++)
            {
                notes.Add(new NoteModel("Title " + i, i +  ". Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                    "Ut eleifend dui eu leo bibendum hendrerit. Integer vehicula, nisi sed sodales aliquet, " +
                    "ipsum sapien malesuada nunc, tristique accumsan quam libero sed erat. " +
                    "Nulla nunc leo, mattis non massa efficitur, semper ullamcorper nisi. " +
                    "Morbi quis porttitor arcu. Maecenas risus lacus, tempor et tempor commodo, porta ut magna. " +
                    "Aliquam at tempus eros, nec scelerisque ipsum. Cras vel purus eget tortor accumsan " +
                    "lobortis non et ex."));
                notesForLV.Add(notes.Last());
            }
        }
    }
}
