using StressFree.Disney.Data;
using StressFree.Disney.Entities;
using StressFree.Disney.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StressFree.Disney.Application
{
    public class WordApplication : IWordApplication
    {
        readonly IWordData wordData;

        public WordApplication(IWordData wordData)
        {
            this.wordData = wordData;
        }

        public ResponseBoard GetInitialBoard()
        {
            ResponseBoard response = new ResponseBoard();
            response.UsedWords = new List<UsedWord>();
            response.IntersectionLetters = new List<string>();

            try
            {
                var words = wordData.GetRandomWords();

                var maxSizeWord = words.Aggregate("", (max, w) => max.Length > w.Length ? max : w);
                var maxSize = maxSizeWord.Length;
                char[,] wordsLetters = new char[maxSize, maxSize];

                foreach (var word in words)
                {
                    var placed = false;
                    var countPlacingAttempts = 0;

                    while (!placed)
                    {
                        Direction direction = GetDirection();
                        Random rnd = new Random();
                        int posX = rnd.Next(maxSize);
                        int posY = rnd.Next(maxSize);

                        placed = PlaceWords(wordsLetters, direction, posX, posY, word.Trim().Replace(" ", ""), maxSize, response);
                        if (!placed)
                            countPlacingAttempts++;

                        //if (countPlacingAttempts > 100)
                        //    throw new Exception();
                    }
                }

                FillBlanks(wordsLetters, maxSize);

                response.WordsLetters = wordsLetters;
                response.Words = words;
                response.MaxSize = maxSize;
            }
            catch (Exception)
            {
                response.MaxSize = 0;
            }

            return response;
        }

        public bool ValidateWordInList(string word)
        {
            return wordData.ValidateWordInList(word);
        }

        #region Private methods
        private Direction GetDirection()
        {
            Random rnd = new Random();
            var countDirections = Enum.GetNames(typeof(Direction)).Length + 1;
            switch (rnd.Next(1, countDirections))
            {
                case 1: return Direction.Down;
                case 2: return Direction.Right;

            }
            return Direction.Down;
        }

        private bool PlaceWords(char[,] wordsLetters, Direction direction, int posX, int posY, string word, int maxSize, ResponseBoard response)
        {
            try
            {
                bool avalaible = true;
                int j = 0;

                switch (direction)
                {
                    case Direction.Down:
                        j = posY;
                        for (int i = 0; i < word.Length; i++)
                        {
                            if (j >= maxSize)
                                return false;
                            if (wordsLetters[posX, j] != '\0' && wordsLetters[posX, j] != word[i])
                            {
                                avalaible = false;
                                break;
                            }
                            j++;
                        }
                        if (avalaible)
                        {
                            j = posY;
                            for (int i = 0; i < word.Length; i++)
                            {
                                if (wordsLetters[posX, j] == word[i])
                                {
                                    response.IntersectionLetters.Add($"{posX},{j}");
                                }
                                wordsLetters[posX, j] = word[i];
                                j++;
                            }

                            response.UsedWords.Add(SaveUsedWord(word, posX, posY, direction));
                            return true;
                        }
                        break;
                    case Direction.Right:
                        j = posX;
                        for (int i = 0; i < word.Length; i++)
                        {
                            if (j >= maxSize)
                                return false;
                            if (wordsLetters[j, posY] != '\0' && wordsLetters[j, posY] != word[i])
                            {
                                avalaible = false;
                                break;
                            }
                         
                            j++;
                        }
                        if (avalaible)
                        {
                            j = posX;
                            for (int i = 0; i < word.Length; i++)
                            {
                                if (wordsLetters[j, posY] == word[i])
                                {
                                    response.IntersectionLetters.Add($"{j},{posY}");
                                }
                                wordsLetters[j, posY] = word[i];
                                j++;
                            }

                            response.UsedWords.Add(SaveUsedWord(word, posX, posY, direction));
                            return true;
                        }
                        break;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private UsedWord SaveUsedWord(string word, int posX, int posY, Direction direction)
        {
            var usedWord = new UsedWord()
            {
                Direction = direction,
                PosX = posX,
                PosY = posY,
                Word = word
            };
            return usedWord;
        }

        private void FillBlanks(char[,] wordsLetters, int maxSize)
        {
            Random rnd = new Random();
            for (int i = 0; i < maxSize; i++)
                for (int j = 0; j < maxSize; j++)
                    if (wordsLetters[i, j] == '\0')
                        wordsLetters[i, j] = (char)(65 + rnd.Next(26));
        }
        #endregion
    }
}
