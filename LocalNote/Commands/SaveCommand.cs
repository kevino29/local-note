using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LocalNote.Commands
{
    public class SaveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private ViewModels.NoteViewModel noteViewModel;

        public SaveCommand(ViewModels.NoteViewModel noteViewModel)
        {
            this.noteViewModel = noteViewModel;
        }

        public bool CanExecute(object parameter)
        {
            if (noteViewModel.SelectedNote == null) return false;
            return noteViewModel.SelectedNote.NeedSaving;
        }

        public async void Execute(object parameter)
        {
            // Check if the note has just been created
            if (noteViewModel.SelectedNote.Title == "Untitled Note")
            {
                // Create a save note dialog
                Views.SaveNoteDialog save = new Views.SaveNoteDialog();
                ContentDialogResult result;

                // This loop makes sure that the user doesn't enter
                // a duplicate, invalid, or empty title for a note
                while (true)
                {
                    bool duplicate = false;
                    result = await save.ShowAsync();

                    // Check for empty title
                    bool empty = string.IsNullOrEmpty(save.NoteTitle);

                    // Check for invalid file name characters
                    bool invalid = save.NoteTitle.Contains("_") ||
                        save.NoteTitle.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;

                    // Check against duplicates
                    foreach (var note in noteViewModel.Notes)
                    {
                        if (note.Title == save.NoteTitle)
                        {
                            duplicate = true;
                            break;
                        }
                    }

                    // Look for an error
                    string content;
                    if (empty)
                        content = "The title cannot be empty. Please enter a title.";
                    else if (duplicate)
                        content = "That title already exists. Please enter a new unique title.";
                    else if (invalid)
                        content = "The title contains invalid character(s). Please enter a new title.";
                    else break; // Skip, if there are none

                    // Show a dialog that there is an error
                    ContentDialog error = new ContentDialog()
                    {
                        Title = "Error Occurred",
                        Content = content,
                        PrimaryButtonText = "OK",
                    };
                    await error.ShowAsync();
                }

                if (result == ContentDialogResult.Primary)
                {
                    // Get the new note title from the dialog
                    noteViewModel.SelectedNote.Title = save.NoteTitle;
                    
                    // Check if the selected note is a buffer
                    // Add it to the notes lists first
                    if (noteViewModel.SelectedNote == noteViewModel.Buffer)
                    {
                        noteViewModel.Notes.Add(noteViewModel.SelectedNote);
                        noteViewModel.NotesForLV.Add(noteViewModel.SelectedNote);

                        // Set the buffer back to null
                        noteViewModel.Buffer = null;
                    }

                    // Notify that the selected note and the selected note's title changed
                    noteViewModel.FirePropertyChanged("SelectedNote");
                    noteViewModel.SelectedNote.FirePropertyChanged("Title");
                }
                // Do not continue if the user clicks 'Cancel'
                else return;
            }

            // Save the note data to file
            Repositories.NotesRepo.SaveNoteToFile(noteViewModel.SelectedNote);

            // Show a dialog that the save was successful
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Saved successfully",
                Content = "'" + noteViewModel.SelectedNote.Title + "'" + " has been saved.",
                PrimaryButtonText = "OK",
            };
            await dialog.ShowAsync();

            // After saving defaults ...

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

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
