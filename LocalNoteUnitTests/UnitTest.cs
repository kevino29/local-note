using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using LocalNote.ViewModels;
using LocalNote.Commands;
using LocalNote.Repositories;
using System.Threading.Tasks;

namespace LocalNoteUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly NoteViewModel vm = new NoteViewModel();
        private static readonly SaveCommand save = new SaveCommand(vm);

        #region Initial Tests
        [UITestMethod]
        public void Test_SaveCommand_IsFileNameEmpty_When_FileNameIsEmpty()
        {
            string fileName = "";
            Assert.IsTrue(save.IsFileNameEmpty(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameEmpty_When_FileNameIsNotEmpty()
        {
            string fileName = "something";
            Assert.IsFalse(save.IsFileNameEmpty(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameEmpty_When_FileNameIsNumber()
        {
            string fileName = "89";
            Assert.IsFalse(save.IsFileNameEmpty(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameEmpty_When_FileNameIsNull()
        {
            string fileName = null;
            Assert.IsTrue(save.IsFileNameEmpty(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameDuplicate_When_FileNameIsNull()
        {
            string fileName = null;
            Assert.IsFalse(save.IsFileNameDuplicate(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameDuplicate_When_FileNameIsEpmty()
        {
            string fileName = "";
            Assert.IsFalse(save.IsFileNameDuplicate(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameDuplicate_When_FileNameAlreadyExists()
        {
            vm.Notes.Add(new LocalNote.Models.NoteModel("TITLE", new LocalNote.Models.ContentModel("CONTENT", "CONTENT")));
            string fileName = "TITLE";
            Assert.IsTrue(save.IsFileNameDuplicate(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameInvalid_When_FileNameIsValid()
        {
            string fileName = "this is valid";
            Assert.IsFalse(save.IsFileNameInvalid(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameInvalid_When_FileNameIsInvalid()
        {
            string fileName = "this_is_invalid";
            Assert.IsTrue(save.IsFileNameInvalid(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameInvalid_When_FileNameHasInvalidChars()
        {
            string fileName = "invalid chars include:|<>_!?";
            Assert.IsTrue(save.IsFileNameInvalid(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameInvalid_When_FileNameIsEmpty()
        {
            string fileName = "";
            Assert.IsFalse(save.IsFileNameInvalid(fileName));
        }

        [UITestMethod]
        public void Test_SaveCommand_IsFileNameInvalid_When_FileNameIsNull()
        {
            string fileName = null;
            Assert.IsFalse(save.IsFileNameInvalid(fileName));
        }
        #endregion

        #region Additional Tests
        // All the Test Methods (except for the methods that expects an exception thrown) are passing tests.
        // Running all but those exception methods will give the tests a pass.
        [TestMethod]
        public async Task Test_DatabaseRepo_AddingNoteInTheDatabase() {
            DatabaseRepo.InitializeDB();
            LocalNote.Models.NoteModel noteToBeAdded =
                new LocalNote.Models.NoteModel("THIS IS THE TITLE",
                    new LocalNote.Models.ContentModel("NO CONTENT FOR YOU", "NO CONTENT FOR YOU"));
            DatabaseRepo.AddNote(noteToBeAdded);
            var actual = await DatabaseRepo.GetNote("THIS IS THE TITLE");
            Assert.IsTrue(noteToBeAdded == actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_DatabaseRepo_AddingNoteInTheDatabase_WhenNoteIsNull() {
            DatabaseRepo.InitializeDB();
            LocalNote.Models.NoteModel noteToBeAdded = null;
            DatabaseRepo.AddNote(noteToBeAdded);
        }

        [TestMethod]
        public async Task Test_DatabaseRepo_UpdatingNoteInTheDatabase() {
            DatabaseRepo.InitializeDB();
            LocalNote.Models.NoteModel noteToBeUpdated =
                new LocalNote.Models.NoteModel("THIS IS THE TITLE",
                    new LocalNote.Models.ContentModel("THERE IS CONTENT NOW", "THERE IS CONTENT NOW"));
            DatabaseRepo.UpdateNote(noteToBeUpdated);
            var actual = await DatabaseRepo.GetNote("THIS IS THE TITLE");
            Assert.IsTrue(noteToBeUpdated == actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_DatabaseRepo_UpdatingNoteInTheDatabase_WhenNoteIsNull() {
            DatabaseRepo.InitializeDB();
            LocalNote.Models.NoteModel noteToBeAdded = null;
            DatabaseRepo.UpdateNote(noteToBeAdded);
        }

        [TestMethod]
        public async Task Test_DatabaseRepo_GettingNoteInTheDatabase() {
            DatabaseRepo.InitializeDB();
            LocalNote.Models.NoteModel note =
                new LocalNote.Models.NoteModel("THIS IS THE TITLE",
                    new LocalNote.Models.ContentModel("THERE IS CONTENT NOW", "THERE IS CONTENT NOW"));
            var actual = await DatabaseRepo.GetNote(note.Title);
            Assert.IsTrue(note == actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Test_DatabaseRepo_GettingNoteInTheDatabase_WhenTitleIsNull() {
            DatabaseRepo.InitializeDB();
            await DatabaseRepo.GetNote(null);
        }

        [TestMethod]
        public async Task Test_DatabaseRepo_DeletingNoteInTheDatabase() {
            DatabaseRepo.InitializeDB();
            LocalNote.Models.NoteModel noteToBeDeleted =
                new LocalNote.Models.NoteModel("THIS IS THE TITLE",
                    new LocalNote.Models.ContentModel("THERE IS CONTENT NOW", "THERE IS CONTENT NOW"));
            DatabaseRepo.DeleteNote(noteToBeDeleted);
            var actual = await DatabaseRepo.GetNote("THIS IS THE TITLE");
            Assert.IsNull(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_DatabaseRepo_DeletingNoteInTheDatabase_WhenNoteIsNull() {
            DatabaseRepo.InitializeDB();
            LocalNote.Models.NoteModel noteToBeAdded = null;
            DatabaseRepo.DeleteNote(noteToBeAdded);
        }
        #endregion
    }
}
