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
        

        public async static void SaveNoteToFile(Models.NoteModel selected)
        {
            string fileName = selected.Title.Replace(" ", "_") + ".txt";
            try
            {
                // Create the file asynchronously, then add the content
                StorageFile noteFile = 
                    await notesFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.AppendTextAsync(noteFile, selected.Content);
            }
            catch (Exception)
            {
                Debug.WriteLine("An error occured when saving the note file.");
            }
        }

        public async static void DeleteNoteFile(Models.NoteModel selected)
        {
            string fileName = selected.Title.Replace(" ", "_") + ".txt";

            try
            {
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
