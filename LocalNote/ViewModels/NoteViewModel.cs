using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalNote.Models;
using LocalNote.Commands;
using LocalNote.Commands.EditorCommands;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace LocalNote.ViewModels {
    public class NoteViewModel : INotifyPropertyChanged {
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Private properties
        /// </summary>
        private ObservableCollection<NoteModel> notes;
        private ObservableCollection<NoteModel> notesForLV;
        private string noteTitle;
        private ContentModel noteContent;
        private string filter;
        private bool editMode;
        private bool readOnly;
        private NoteModel selectedNote;
        private NoteModel buffer;
        private RichEditBox editor;
        private string editorCommandsVisibility;

        /// <summary>
        /// Getters and setters
        /// </summary>
        public ObservableCollection<NoteModel> Notes { get { return notes; } }
        public ObservableCollection<NoteModel> NotesForLV { get { return notesForLV; } }
        public SaveCommand SaveCommand { get; set; }
        public AddCommand AddCommand { get; set; }
        public EditCommand EditCommand { get; set; }
        public CancelCommand CancelCommand { get; set; }
        public DeleteCommand DeleteCommand { get; set; }
        public ExitCommand ExitCommand { get; set; }
        public FontIncrease FontIncrease { get; set; }
        public FontDecrease FontDecrease { get; set; }
        public BoldCommand BoldCommand { get; set; }
        public ItalicCommand ItalicCommand { get; set; }
        public UnderlineCommand UnderlineCommand { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NoteViewModel() {
            Init();
            LoadNotesFromDatabase();
        }

        /// <summary>
        /// Constructor with editor parameter.
        /// </summary>
        /// <param name="editor"></param>
        public NoteViewModel(RichEditBox editor) {
            this.editor = editor;
            Init();
            LoadNotesFromDatabase();
        }

        #region Getters and Setters
        /// <summary>
        /// Gets and sets the selected note property.
        /// </summary>
        public NoteModel SelectedNote {
            get {
                if (selectedNote != null)
                    return selectedNote;
                else {
                    // If there is no selected note,
                    // Create a buffer note and set that as selected
                    Buffer = new NoteModel();
                    selectedNote = Buffer;
                    return selectedNote;
                }
            }
            set {
                selectedNote = value;
                if (value == null) {
                    noteTitle = "";
                    noteContent = new ContentModel();
                } else {
                    noteTitle = value.Title;
                    noteContent = value.Content;
                }

                // Change the editor's text
                try {
                    // Need to turn off read only to set the text of the editor
                    editor.IsReadOnly = false;
                    editor.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, noteContent.Rtf);
                } catch (UnauthorizedAccessException e) {
                    Debug.WriteLine(e);
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

                // Always turn on read only mode when switching notes
                ReadOnly = true;

                // Always turn off cancel edit when switching notes

                // Always hide the edito command bar
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EditorCommandsVisibility)));
            }
        }

        /// <summary>
        /// Gets the editor property
        /// </summary>
        public RichEditBox Editor {
            get { return this.editor; }
        }

        /// <summary>
        /// Gets and sets the editor command bar visibility.
        /// </summary>
        public string EditorCommandsVisibility {
            get { 
                return this.editorCommandsVisibility; 
            }
            set {
                if (SelectedNote != null) {
                    if (EditMode) this.editorCommandsVisibility = "Visible";
                    else this.editorCommandsVisibility = "Collapsed";
                } else this.editorCommandsVisibility = value;
            } 
        }

        /// <summary>
        /// Gets and sets the buffer property.
        /// </summary>
        public NoteModel Buffer {
            get { return this.buffer; }
            set { this.buffer = value; }
        }

        /// <summary>
        /// Gets and sets the note title for the currenlty selected note.
        /// </summary>
        public string NoteTitle {
            get { return noteTitle; }
            set {
                if (SelectedNote != null) {
                    // Check if the title changed from previous
                    if (SelectedNote.Title != value) {
                        SelectedNote.Title = value;

                        // Only activate the saving command when the title changes
                        SelectedNote.NeedSaving = true;
                        FirePropertyChanged(nameof(NoteTitle));
                        SelectedNote.FirePropertyChanged("NeedSaving");
                        SaveCommand.FireCanExecuteChanged();
                    } else SelectedNote.Title = value;
                } else noteTitle = value;
            }
        }

        /// <summary>
        /// Gets and sets the note content for the currenlty selected note.
        /// </summary>
        public ContentModel NoteContent {
            get { return noteContent; }
            set {
                if (SelectedNote != null) {
                    // Check if the content changed from previous
                    if (SelectedNote.Content.PlainText != value.PlainText &&
                        value.PlainText != "\r\r") {
                        SelectedNote.Content.Rtf = value.Rtf;
                        SelectedNote.Content.PlainText = value.PlainText;

                        // Only activate the saving command when the content changes
                        SelectedNote.NeedSaving = true;
                        FirePropertyChanged(nameof(NoteContent));
                        SelectedNote.FirePropertyChanged("NeedSaving");
                        SaveCommand.FireCanExecuteChanged();
                    } else {
                        SelectedNote.Content.Rtf = value.Rtf;
                        SelectedNote.Content.PlainText = value.PlainText;
                    }
                } else noteContent = value;
            }
        }

        /// <summary>
        /// Gets and sets the filter string.
        /// </summary>
        public string Filter {
            get { return this.filter; }
            set {
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
        public bool EditMode {
            get { return this.editMode; }
            set {
                // Edit mode is always off when there is no note selected
                if (SelectedNote == null) {
                    editMode = false;
                    EditCommand.FireCanExecuteChanged();
                    CancelCommand.FireCanExecuteChanged();
                    return;
                }
                this.editMode = value;
                EditCommand.FireCanExecuteChanged();
                CancelCommand.FireCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets and sets the read-only property.
        /// </summary>
        public bool ReadOnly {
            get { return this.readOnly; }
            set {
                // Read only mode is always off when in edit mode
                if (EditMode) {
                    readOnly = false;
                    FirePropertyChanged(nameof(ReadOnly));
                    EditCommand.FireCanExecuteChanged();
                    CancelCommand.FireCanExecuteChanged();
                    return;
                }
                this.readOnly = value;
                FirePropertyChanged(nameof(ReadOnly));
                EditCommand.FireCanExecuteChanged();
                CancelCommand.FireCanExecuteChanged();
            }
        }
        #endregion

        /// <summary>
        /// Properties initializer
        /// </summary>
        private void Init() {
            notes = new ObservableCollection<NoteModel>();
            notesForLV = new ObservableCollection<NoteModel>();
            SaveCommand = new SaveCommand(this);
            AddCommand = new AddCommand(this);
            EditCommand = new EditCommand(this);
            CancelCommand = new CancelCommand(this);
            DeleteCommand = new DeleteCommand(this);
            ExitCommand = new ExitCommand(this);
            FontIncrease = new FontIncrease(this);
            FontDecrease = new FontDecrease(this);
            BoldCommand = new BoldCommand(this);
            ItalicCommand = new ItalicCommand(this);
            UnderlineCommand = new UnderlineCommand(this);
            EditMode = false;
            ReadOnly = true;
        }

        /// <summary>
        /// Fires the PropertyChanged event with the given property name.
        /// </summary>
        /// <param name="property"></param>
        public void FirePropertyChanged(string property) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Performs a filter on the notes list based on the filter property.
        /// </summary>
        private void PerformFilter() {
            if (this.filter == null) {
                filter = "";

                // Clear the collection of notes used to display to the UI
                NotesForLV.Clear();
                foreach (var note in Notes) {
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
            foreach (var x in toRemove) {
                NotesForLV.Remove(x);
            }

            // Add all the notes that matches the filter to the list that is displayed in the UI
            for (int i = 0; i < result.Count; i++) {
                var resultItem = result[i];

                if (i + 1 > NotesForLV.Count || !NotesForLV[i].Equals(resultItem))
                    NotesForLV.Insert(i, resultItem);
            }
        }

        public void Editor_TextChanged() {
            editor.Document.GetText(Windows.UI.Text.TextGetOptions.FormatRtf, out string rtf);
            editor.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string text);
            NoteContent.Rtf = rtf;
            NoteContent.PlainText = text;
            FirePropertyChanged(nameof(NoteContent));
        }

        /// <summary>
        /// Loads each note's data from file.
        /// </summary>
        private async void LoadNotes() {
            // Get the folder where the notes are stored
            // Then get all the files within that folder
            StorageFolder notesFolder = ApplicationData.Current.LocalFolder;
            IReadOnlyList<StorageFile> fileList = await notesFolder.GetFilesAsync();

            // Read each file
            foreach (var file in fileList) {
                // Skip the database file
                if (file.FileType == ".db") continue;

                string content = await FileIO.ReadTextAsync(file);
                notes.Add(new NoteModel(file.DisplayName.Replace("_", " "), new ContentModel(content, content)));
                notesForLV.Add(notes.Last());
            }
        }

        /// <summary>
        /// Loads each note's data from database.
        /// </summary>
        private async void LoadNotesFromDatabase() {
            notes = await Repositories.DatabaseRepo.GetNotes();

            foreach (var note in notes) {
                notesForLV.Add(note);
            }
            SelectedNote = Buffer;
            FirePropertyChanged("SelectedNote");
        }
    }
}
