
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
        private static SaveCommand save = new SaveCommand(vm);

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
            vm.Notes.Add(new LocalNote.Models.NoteModel("TITLE", "CONTENT"));
            string fileName = "TITLE";
            Assert.IsTrue(save.IsFileNameDuplicate(fileName));
        }
    }
}
