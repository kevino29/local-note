using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LocalNote.Repositories
{
    public class NotesRepo
    {
        private static StorageFolder notesFolder = ApplicationData.Current.LocalFolder;
        
        /// <summary>
        /// Saves the given note's data to a file.
        /// </summary>
        /// <param name="selected"></param>
        public async static void SaveNoteToFile(Models.NoteModel selected)
        {
            string fileName = selected.Title.Replace(" ", "_") + ".txt";
            try
            {
                // Create the file asynchronously, then add the content asynchronously
                StorageFile noteFile = 
                    await notesFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.AppendTextAsync(noteFile, selected.Content);
            }
            catch (Exception)
            {
                Debug.WriteLine("An error occured when saving the note file.");
            }
        }

        /// <summary>
        /// Deletes the give note's data file.
        /// </summary>
        /// <param name="selected"></param>
        public async static void DeleteNoteFile(Models.NoteModel selected)
        {
            string fileName = selected.Title.Replace(" ", "_") + ".txt";
            try
            {
                // Get the targeted file, then delete it asynchronously
                StorageFile noteToDelete =
                    await notesFolder.GetFileAsync(fileName);
                await noteToDelete.DeleteAsync();
            }
            catch (Exception)
            {
                Debug.WriteLine("An error occurred when deleting the note file.");
            }
        }
    }
}
