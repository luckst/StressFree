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

            var words = wordData.GetWords();

            var maxSizeWord = words.Aggregate("", (max, w) => max.Length > w.Length ? max : w);
            char[,] wordsLetters = new char[maxSizeWord.Length, maxSizeWord.Length];

            foreach (var word in words)
            {
                var placed = false;
                while (!placed)
                {
                    Direction direction = GetDirection();
                    Random rnd = new Random();
                    int posX = rnd.Next(maxSizeWord.Length);
                    int posY = rnd.Next(maxSizeWord.Length);

                    placed = PlaceWords(wordsLetters, direction, posX, posY, word.Trim().Replace(" ", ""), maxSizeWord.Length, response);
                }
            }

            FillBlanks(wordsLetters, maxSizeWord.Length);

            response.WordsLetters = wordsLetters;
            response.Words = words;
            response.MaxSize = maxSizeWord.Length;

            return response;
        }

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
    }
}
