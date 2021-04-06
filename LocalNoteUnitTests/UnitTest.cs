
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LocalNote.ViewModels;
using LocalNote.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace LocalNoteUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private static NoteViewModel vm = new NoteViewModel();
        private SaveCommand save = new SaveCommand(vm);

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
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_SaveCommand_IsFileNameInvalid_When_FileNameIsNull()
        {
            string fileName = null;
            save.IsFileNameInvalid(fileName);
        }
    }
}
