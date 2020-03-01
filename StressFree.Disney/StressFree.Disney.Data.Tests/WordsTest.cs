using Microsoft.Extensions.Options;
using Moq;
using StressFree.Disney.Entities;
using System;
using Xunit;

namespace StressFree.Disney.Data.Tests
{
    public class WordsTest
    {
        [Fact]
        public void CheckWord_Fiber_InList_MustReturn_True()
        {
            string wordToSearch = "FIBER";
            SettingsModel settingsModel = new SettingsModel() { Words = "VITAMIN A;FIBER;MICKEY MOUSE" };

            var mockSettings = new Mock<IOptions<SettingsModel>>();

            mockSettings.Setup(w => w.Value).Returns(settingsModel);

            var wordData = new WordData(mockSettings.Object);

            var result = wordData.ValidateWordInList(wordToSearch);

            Assert.True(result);
        }

        [Fact]
        public void CheckWord_Hello_InList_MustReturn_False()
        {
            string wordToSearch = "HELLO";
            SettingsModel settingsModel = new SettingsModel() { Words = "VITAMIN A;FIBER;MICKEY MOUSE" };

            var mockSettings = new Mock<IOptions<SettingsModel>>();

            mockSettings.Setup(w => w.Value).Returns(settingsModel);

            var wordData = new WordData(mockSettings.Object);

            var result = wordData.ValidateWordInList(wordToSearch);

            Assert.False(result);
        }

        [Fact]
        public void CheckWord_Count_Of_Words_MustReturn_AtLeast_3_Words()
        {
            SettingsModel settingsModel = new SettingsModel() { Words = "VITAMIN A;FIBER;MICKEY MOUSE;MULTIVITAMIN;PHINEAS;FERB" };

            var mockSettings = new Mock<IOptions<SettingsModel>>();

            mockSettings.Setup(w => w.Value).Returns(settingsModel);

            var wordData = new WordData(mockSettings.Object);

            var result = wordData.GetRandomWords();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.NotEqual(1, result.Count);
            Assert.NotEqual(2, result.Count);
        }
    }
}
