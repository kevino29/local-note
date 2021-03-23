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

        private ObservableCollection<NoteModel> notes;
        private ObservableCollection<NoteModel> notesForLV;
        private string noteTitle;
        private string noteContent;
        private string filter;
        private bool editMode;
        private bool readOnly;
        private NoteModel selectedNote;
        private NoteModel buffer;

        public ObservableCollection<NoteModel> Notes { get { return notes; } }
        public ObservableCollection<NoteModel> NotesForLV { get { return notesForLV; } }
        public SaveCommand SaveCommand { get; }
        public AddCommand AddCommand { get; }
        public EditCommand EditCommand { get; }
        public DeleteCommand DeleteCommand { get; }
        public ExitCommand ExitCommand { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteViewModel()
        {
            notes = new ObservableCollection<NoteModel>();
            notesForLV = new ObservableCollection<NoteModel>();
            SaveCommand = new SaveCommand(this);
            AddCommand = new AddCommand(this);
            EditCommand = new EditCommand(this);
            DeleteCommand = new DeleteCommand(this);
            ExitCommand = new ExitCommand(this);
            Buffer = new NoteModel();
            SelectedNote = Buffer;
            EditMode = false;
            ReadOnly = true;

            LoadNotes();
        }

        /// <summary>
        /// Gets and sets the selected note property.
        /// </summary>
        public NoteModel SelectedNote
        {
            get 
            { 
                if (selectedNote != null)
                    return selectedNote;
                else
                {
                    // If there is no selected note,
                    // Create a buffer note and set that as selected
                    Buffer = new NoteModel();
                    selectedNote = Buffer;
                    return selectedNote;
                }
            }
            set
            {
                selectedNote = value;
                if (value == null)
                {
                    noteTitle = "";
                    noteContent = "";
                }
                else
                {
                    noteTitle = value.Title;
                    noteContent = value.Content;
                }

                // Notify that the title and content changed
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NoteTitle)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NoteContent)));

                // Always notify if the selected note needs saving
                SaveCommand.FireCanExecuteChanged();

                // Always turn on the ability to delete the selected note
                DeleteCommand.FireCanExecuteChanged();
                
                // Always turn off edit mode when switching notes
                EditMode = false;
                EditCommand.FireCanExecuteChanged();

                // Always turn on read only mode when switching notes
                ReadOnly = true;
                FirePropertyChanged(nameof(ReadOnly));
            }
        }

        /// <summary>
        /// Gets and sets the buffer property.
        /// </summary>
        public NoteModel Buffer
        {
            get { return this.buffer; }
            set { this.buffer = value; }
        }

        /// <summary>
        /// Gets and sets the note title for the currenlty selected note.
        /// </summary>
        public string NoteTitle {
            get { return noteTitle; }
            set 
            {
                if (SelectedNote != null)
                {
                    // Check if the title changed from previous
                    if (SelectedNote.Title != value)
                    {
                        SelectedNote.Title = value;

                        // Only activate the saving command when the title changes
                        SelectedNote.NeedSaving = true;
                        SelectedNote.FirePropertyChanged("NeedSaving");
                        SaveCommand.FireCanExecuteChanged();
                    }
                    else
                        SelectedNote.Title = value;
                }
                else
                    noteTitle = value;
            }
        }

        /// <summary>
        /// Gets and sets the note content for the currenlty selected note.
        /// </summary>
        public string NoteContent
        {
            get { return noteContent; }
            set
            {
                if (SelectedNote != null)
                {
                    // Check if the content changed from previous
                    if (SelectedNote.Content != value)
                    {
                        SelectedNote.Content = value;

                        // Only activate the saving command when the content changes
                        SelectedNote.NeedSaving = true;
                        SelectedNote.FirePropertyChanged("NeedSaving");
                        SaveCommand.FireCanExecuteChanged();
                    }
                    else SelectedNote.Content = value;
                }
                else noteContent = value;
            }
        }

        /// <summary>
        /// Gets and sets the filter string.
        /// </summary>
        public string Filter
        {
            get { return this.filter; }
            set 
            {
                // Skip if the new filter is the same as previous
                if (value == filter) { return; }

                // Otherwise, perform a filter
                this.filter = value;
                PerformFilter();
            }
        }

        /// <summary>
        /// Gets and sets the edit mode property.
        /// </summary>
        public bool EditMode
        {
            get { return this.editMode; }
            set
            {
                // Edit mode is always off when there is no note selected
                if (SelectedNote == null)
                {
                    editMode = false;
                    return;
                }
                this.editMode = value;
            }
        }

        /// <summary>
        /// Gets and sets the read-only property.
        /// </summary>
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set
            {
                // Read only mode is always off when in edit mode
                if (EditMode)
                {
                    readOnly = false;
                    return;
                }
                this.readOnly = value;
            }
        }

        /// <summary>
        /// Fires the PropertyChanged event with the given property name.
        /// </summary>
        /// <param name="property"></param>
        public void FirePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Performs a filter on the notes list based on the filter property.
        /// </summary>
        private void PerformFilter()
        {
            if (this.filter == null)
            {
                filter = "";

                // Clear the collection of notes used to display to the UI
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

            // Add all the notes that matches the filter to the list that is displayed in the UI
            for (int i = 0; i < result.Count; i++)
            {
                var resultItem = result[i];

                if (i + 1 > NotesForLV.Count || !NotesForLV[i].Equals(resultItem))
                    NotesForLV.Insert(i, resultItem);
            }
        }

        /// <summary>
        /// Updates the order of the notes lists based on the invidual note's title.
        /// </summary>
        public void UpdateNotesLists()
        {
            // Order the notes list based on the note title
            List<NoteModel> orderedList = Notes.OrderBy(x => x.Title).ToList();
            notes = new ObservableCollection<NoteModel>(orderedList);

            // Copy the notes list
            NotesForLV.Clear();
            foreach (var note in Notes)
            {
                NotesForLV.Add(note);
            }
        }

        /// <summary>
        /// Loads each note's data from file.
        /// </summary>
        private async void LoadNotes()
        {
            // Get the folder where the notes are stored
            // Then get all the files within that folder
            StorageFolder notesFolder = ApplicationData.Current.LocalFolder;
            IReadOnlyList<StorageFile> fileList = await notesFolder.GetFilesAsync();

            // Read each file
            foreach(var file in fileList)
            {
                string content = await FileIO.ReadTextAsync(file);
                notes.Add(new NoteModel(file.DisplayName.Replace("_", " "), content));
                notesForLV.Add(notes.Last());
            }
        }
    }
}
