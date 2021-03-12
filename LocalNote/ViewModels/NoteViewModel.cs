using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalNote.Models;
using LocalNote.Commands;
using Windows.Storage;

namespace LocalNote.ViewModels
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ObservableCollection<NoteModel> notes;
        private readonly ObservableCollection<NoteModel> notesForLV;
        private string noteTitle;
        private string noteContent;
        private string filter;
        private bool editMode;
        private bool readOnly;
        private NoteModel selectedNote;

        public ObservableCollection<NoteModel> Notes { get { return notes; } }
        public ObservableCollection<NoteModel> NotesForLV { get { return notesForLV; } }
        public SaveCommand SaveCommand { get; }
        public AddCommand AddCommand { get; }
        public EditCommand EditCommand { get; }

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
                    noteTitle = value.Title;
                    noteContent = value.Content;
                }

                // Notify that the title and content changed
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NoteTitle)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NoteContent)));
                
                // Always turn off edit mode when switching notes
                EditMode = false;
                EditCommand.FireCanExecuteChanged();

                // Always turn on read only mode when switching notes
                ReadOnly = true;
                FirePropertyChanged(nameof(ReadOnly));
            }
        }

        public void FirePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public string NoteTitle {
            get { return noteTitle; }
            set 
            {
                if (SelectedNote != null)
                {
                    SelectedNote.Title = value;
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
                    SelectedNote.Content = value;
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

        public bool EditMode
        {
            get { return this.editMode; }
            set
            {
                if (SelectedNote == null)
                {
                    editMode = false;
                    return;
                }
                this.editMode = value;
            }
        }

        public bool ReadOnly
        {
            get { return this.readOnly; }
            set
            {
                if (EditMode)
                {
                    readOnly = false;
                    return;
                }
                this.readOnly = value;
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
            var result = Notes.Where(note => note.Title.ToLowerInvariant()
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
            EditCommand = new EditCommand(this);

            LoadNotes();
        }

        public async void LoadNotes()
        {
            StorageFolder notesFolder = ApplicationData.Current.LocalFolder;
            IReadOnlyList<StorageFile> fileList = await notesFolder.GetFilesAsync();

            foreach(var file in fileList)
            {
                string content = await FileIO.ReadTextAsync(file);
                notes.Add(new NoteModel(file.DisplayName.Replace("_", " "), content));
                notesForLV.Add(notes.Last());
            }
        }
    }
}
