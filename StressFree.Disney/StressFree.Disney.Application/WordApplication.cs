using StressFree.Disney.Data;
using StressFree.Disney.Entities;
using StressFree.Disney.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

                    placed = PlaceWords(wordsLetters, direction, posX, posY, word.Trim().Replace(" ", ""), maxSizeWord.Length);
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
            switch (rnd.Next(1, Enum.GetNames(typeof(Direction)).Length))
            {
                case 1: return Direction.Down;
                case 2: return Direction.Right;

            }
            return Direction.Down;
        }

        private bool PlaceWords(char[,] wordsLetters, Direction direction, int posX, int posY, string word, int maxSize)
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

                            //StoreWordPosition(Word, PlacementIndex_X, PlacementIndex_Y, OrientationDecision);
                            return true;
                        }
                        break;
                    case Direction.Right:
                        j = posX;
                        for (int i = 0; i < word.Length; i++)
                        {
                            if (j >= maxSize)
                                return false;
                            if (wordsLetters[j, posY] != '\0' && wordsLetters[posX, j] != word[i])
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

                            //StoreWordPosition(Word, PlacementIndex_X, PlacementIndex_Y, OrientationDecision);
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
