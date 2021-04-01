using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using LocalNote.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LocalNote.Repositories {
    public static class DatabaseRepo {
        private static readonly string conn = "Filename=notesDb.db";

        /// <summary>
        /// Initializes the database.
        /// </summary>
        public static void InitializeDB() {
            using (SqliteConnection db = new SqliteConnection("Filename=notesDb.db")) {
                // Opens the database
                db.Open();

                // Create the command to create a table
                var create = new SqliteCommand {
                    Connection = db,
                    CommandText =
                    @"
                        CREATE TABLE IF NOT EXISTS NotesTable (
                            notes_id INTEGER PRIMARY KEY AUTOINCREMENT, 
                            title nvarchar(100) NOT NULL, 
                            content nvarchar(255) NOT NULL
                        );
                    "
                };

                // Execute the command
                try {
                    create.ExecuteReader();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Adds a note to the database.
        /// </summary>
        /// <param name="note"></param>
        public static void AddNote(NoteModel note) {
            using (SqliteConnection db = new SqliteConnection("Filename=notesDb.db")) {
                // Opens the database
                db.Open();

                // Create the command to insert notes in the notes table
                var insert = new SqliteCommand {
                    Connection = db,
                    CommandText =
                    @"
                        INSERT INTO NotesTable (title, content)
                        VALUES (@title, @content);
                    "
                };

                // Add the parameters to the command
                insert.Parameters.AddWithValue("@title", note.Title);
                insert.Parameters.AddWithValue("@content", note.Content);

                // Execute the command
                try {
                    insert.ExecuteReader();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Gets all the notes from the database.
        /// </summary>
        /// <returns></returns>
        public async static Task<ObservableCollection<NoteModel>> GetNotes() {
            ObservableCollection<NoteModel> notes = new ObservableCollection<NoteModel>();

            using (SqliteConnection db = new SqliteConnection("Filename=notesDb.db")) {
                // Open the database
                db.Open();

                // Create the select command to get all the notes in the table
                var select = new SqliteCommand {
                    Connection = db,
                    CommandText = "SELECT title, content FROM NotesTable;"
                };

                // Execute the command
                SqliteDataReader query = null;
                try {
                    query = select.ExecuteReader();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }

                // Add all the records returned to the notes collection
                while (query.Read()) {
                    notes.Add(new NoteModel(query.GetString(0), query.GetString(1)));
                }
            }
            // Return the notes collection
            return notes;
        }
    }
}
