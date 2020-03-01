//using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StressFree.Disney.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StressFree.Disney.Data
{
    public class WordData : IWordData
    {
        private readonly IOptions<SettingsModel> appSettings;

        public WordData(IOptions<SettingsModel> appSettings)
        {
            this.appSettings = appSettings;
        }

        public List<string> GetRandomWords()
        {
            var wordsString = appSettings.Value.Words;
            var wordList = wordsString.Split(';').Where(w => w.Length > 2).ToList();
            var random = new Random();

            var wordsToTake = random.Next(3, wordList.Count);

            return wordList.Take(wordsToTake).ToList();
        }

        public bool ValidateWordInList(string word)
        {
            var wordsString = appSettings.Value.Words;
            var wordList = wordsString.Split(';').Where(w => w.Length > 2).ToList();

            return wordList.Any(w => w.Trim().Replace(" ", "") == word);
        }
    }
}
