using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LocalNote.Commands {
    public class SaveCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        private readonly ViewModels.NoteViewModel noteViewModel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="noteViewModel"></param>
        public SaveCommand(ViewModels.NoteViewModel noteViewModel) {
            this.noteViewModel = noteViewModel;
        }

        /// <summary>
        /// Returns true if the command can be executed. Returns false otherwise.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) {
            if (noteViewModel.SelectedNote == null) return false;
            return noteViewModel.SelectedNote.NeedSaving;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter"></param>
        public async void Execute(object parameter) {
            string saveNoteTitle = noteViewModel.SelectedNote.Title;

            // Check if the note has just been created
            if (noteViewModel.SelectedNote.Title == "Untitled Note") {
                // Create a save note dialog
                Views.SaveNoteDialog save = new Views.SaveNoteDialog();
                ContentDialogResult result;

                // This loop makes sure that the user doesn't enter
                // a duplicate, invalid, or empty title for a note
                while (true) {
                    result = await save.ShowAsync();

                    // Save the title given by the user
                    saveNoteTitle = save.NoteTitle;

                    // Check for empty error
                    bool empty = IsFileNameEmpty(saveNoteTitle);

                    // Check for invalid error
                    bool invalid = IsFileNameInvalid(saveNoteTitle);

                    // Check for duplicate error
                    bool duplicate = IsFileNameDuplicate(saveNoteTitle);

                    // Look for an error
                    string content;
                    if (empty)
                        content = "The title cannot be empty. Please enter a title.";
                    else if (duplicate)
                        content = "That title already exists. Please enter a new unique title.";
                    else if (invalid)
                        content = "The title contains invalid character(s). Please enter a new title.";
                    else break; // Skip, if there are none

                    // Don't show an error if the user clicks Cancel
                    if (result == ContentDialogResult.Secondary) break;

                    // Show a dialog that there is an error
                    ContentDialog error = new ContentDialog() {
                        Title = "Error Occurred",
                        Content = content,
                        PrimaryButtonText = "OK",
                    };
                    await error.ShowAsync();
                }

                if (result == ContentDialogResult.Primary) {
                    // Get the new note title from the dialog
                    noteViewModel.SelectedNote.Title = saveNoteTitle;

                    // Check if the selected note is a buffer
                    // Add it to the notes lists first
                    if (noteViewModel.SelectedNote == noteViewModel.Buffer) {
                        noteViewModel.Notes.Add(noteViewModel.SelectedNote);
                        noteViewModel.NotesForLV.Add(noteViewModel.SelectedNote);

                        // Set the buffer back to null
                        noteViewModel.Buffer = null;
                    }

                    // Update the notes list order base on note titles
                    // THIS IS BUGGY (OPTIONAL FIX)
                    //noteViewModel.UpdateNotesLists();

                    // Notify that the selected note and the selected note's title changed
                    noteViewModel.FirePropertyChanged("SelectedNote");
                    noteViewModel.SelectedNote.FirePropertyChanged("Title");
                }
                // Do not continue if the user clicks 'Cancel'
                else return;

                // Save the note data to file
                //Repositories.NotesRepo.SaveNoteToFile(noteViewModel.SelectedNote);

                // Save the note to the database
                Repositories.DatabaseRepo.AddNote(noteViewModel.SelectedNote);
            }

            // Note to be saved already exists
            else {
                // Double checking
                if (await Repositories.DatabaseRepo.NoteExist(noteViewModel.SelectedNote)) {
                    Repositories.DatabaseRepo.UpdateNote(noteViewModel.SelectedNote);
                }
            }

            // Show a dialog that the save was successful
            ContentDialog dialog = new ContentDialog() {
                Title = "Saved successfully",
                Content = "'" + saveNoteTitle + "'" + " has been saved.",
                PrimaryButtonText = "OK",
            };
            await dialog.ShowAsync();

            // After saving defaults...
            AfterSavingDefaults();
        }

        public bool IsFileNameEmpty(string fileName) {
            // Check for empty file name
            return string.IsNullOrEmpty(fileName);
        }

        public bool IsFileNameDuplicate(string fileName) {
            if (IsFileNameEmpty(fileName)) return false;

            // Check against duplicates
            foreach (var note in noteViewModel.Notes) {
                if (note.Title == fileName) return true;
            }
            return false;
        }

        public bool IsFileNameInvalid(string fileName) {
            if (IsFileNameEmpty(fileName)) return false;

            // Check for invalid file name characters
            try {
                return fileName.Contains("_") ||
                    fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
            } catch (NullReferenceException) {
                throw new NullReferenceException();
            }
        }

        private void AfterSavingDefaults() {
            // Turn off need saving
            this.noteViewModel.SelectedNote.NeedSaving = false;
            this.noteViewModel.SelectedNote.FirePropertyChanged("NeedSaving");
            this.noteViewModel.SaveCommand.FireCanExecuteChanged();

            // Turn off edit mode
            this.noteViewModel.EditMode = false;
            this.noteViewModel.FirePropertyChanged("EditMode");
            this.noteViewModel.EditCommand.FireCanExecuteChanged();

            // Turn on read only mode
            this.noteViewModel.ReadOnly = true;
            this.noteViewModel.FirePropertyChanged("ReadOnly");
        }

        /// <summary>
        /// Fires the CanExecuteChanged event.
        /// </summary>
        public void FireCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
