import { Component, Inject, OnInit } from '@angular/core';
import { ResponseBoard } from '../models/response-board';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UsedWord } from '../models/used-word';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  baseUrl: string;
  responseBoard: ResponseBoard;
  selectedIndexs: Array<string>;
  previousPos: string;
  direction: number = 1;
  wordSelected: string;
  foundedWordsPositions: Array<string>;
  hintedWordsPositions: Array<string>;
  foundedWordsWithoutHint: Array<string>;
  totalFoundedWords: Array<string>;
  actualWordHinted: UsedWord;
  countHintedWords: number = 0;
  score: number = 0;
  finish: boolean = false;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.responseBoard = new ResponseBoard();
    this.baseUrl = baseUrl;
    this.selectedIndexs = new Array<string>();
    this.foundedWordsPositions = new Array<string>();
    this.hintedWordsPositions = new Array<string>();
    this.foundedWordsWithoutHint = new Array<string>();
    this.totalFoundedWords = new Array<string>();
    this.wordSelected = '';
    this.actualWordHinted = new UsedWord();
  }

  ngOnInit(): void {
    this.getBoard();
  }

  getBoard() {
    this.http.get(this.baseUrl + 'api/words/board', { headers: new HttpHeaders({ timeout: `${10000}` }) }).subscribe(result => {
      this.responseBoard = result as ResponseBoard;
    }, error => {
      this.responseBoard.maxSize = 0;
    });
  }

  cellClicked(r: number, c: number, del: boolean = false, founded: boolean = false) {

    let actualPos = `${r},${c}`

    if (founded && !this.responseBoard.intersectionLetters.find(i => i === actualPos))
      return false;

    if (this.actualWordHinted.word) {
      let actualHintPos = `${this.actualWordHinted.posX},${this.actualWordHinted.posY}`;
      if (actualPos !== actualHintPos) {
        this.deleteHint();
      }
      else
        this.actualWordHinted = new UsedWord();
    }

    if (!del) {
      if (this.selectedIndexs.length > 0) {
        let previousRow = parseInt(this.previousPos.split(',')[0]);
        let previousCol = parseInt(this.previousPos.split(',')[1]);
        if (this.selectedIndexs.length === 1) {
          if ((previousRow + 1) === r && previousCol === c) {
            this.direction = 1;
          }
          else if ((previousCol + 1) === c && previousRow === r) {
            this.direction = 2;
          }
          else
            return false;
        }
        else {
          switch (this.direction) {
            case 1:
              if ((previousRow + 1) !== r || previousCol !== c) {
                return false;
              }
              break;
            case 2:
              if ((previousCol + 1) !== c || previousRow !== r) {
                return false;
              }
              break;
          }
        }

      }

      this.previousPos = actualPos;

      if (this.selectedIndexs.indexOf(actualPos) === -1) {
        this.selectedIndexs.push(actualPos);
      }

      this.wordSelected += this.responseBoard.wordsLetters[r][c];
      if (this.wordSelected.length >= 3)
        this.searchWord();
    }
    else {
      let previousRow = parseInt(this.previousPos.split(',')[0]);
      let previousCol = parseInt(this.previousPos.split(',')[1]);
      switch (this.direction) {
        case 1:
          if ((previousRow) === r) {
            previousRow--;
          }
          else
            return false;
          break;
        case 2:
          if ((previousCol) === c) {
            previousCol--;
          }
          else
            return false;
          break;
      }

      if (this.selectedIndexs.length > 1)
        this.previousPos = `${previousRow},${previousCol}`;
      else
        this.previousPos = '';

      let index = this.selectedIndexs.indexOf(actualPos);
      this.selectedIndexs.splice(index, 1);

      this.wordSelected = this.wordSelected.substring(0, this.wordSelected.length - 1);
    }

  }

  private deleteHint() {
    let aux = 0;
    switch (this.actualWordHinted.direction) {
      case 1:
        aux = this.actualWordHinted.posY;
        for (let i = this.actualWordHinted.word.length; i > 0; i--) {
          let index = this.hintedWordsPositions.indexOf(`${this.actualWordHinted.posX},${aux}`);
          this.hintedWordsPositions.splice(index, 1);
          aux++;
        }
        break;
      case 2:
        aux = this.actualWordHinted.posX;
        for (let i = this.actualWordHinted.word.length; i > 0; i--) {
          let index = this.hintedWordsPositions.indexOf(`${aux},${this.actualWordHinted.posY}`);
          this.hintedWordsPositions.splice(index, 1);
          aux++;
        }
        break;
    }

    this.actualWordHinted = new UsedWord();
  }

  validateIndex(r, c) {
    if (this.selectedIndexs.find(i => i === `${r},${c}`)) {
      return true;
    }
    return false;
  }

  validateFounded(r, c) {

    if (this.foundedWordsPositions.find(i => i === `${r},${c}`)) {
      return true;
    }
    return false;
  }

  validateFoundedSearchArea(word) {

    if (this.totalFoundedWords.find(w => w.replace(" ", "").trim() === word.replace(" ", "").trim())) {
      return true;
    }
    return false;
  }

  validateHinted(r, c) {

    if (this.validateIndex(r, c))
      return false;

    if (this.hintedWordsPositions.find(i => i === `${r},${c}`)) {
      return true;
    }
    return false;
  }

  searchWord() {
    this.http.get(this.baseUrl + `api/words/validateWord?word=${this.wordSelected}`).subscribe(result => {
      if (result["exists"]) {

        var usedWord = this.responseBoard.usedWords.find(w => w.word === this.wordSelected);

        if (usedWord) {
          switch (usedWord.direction) {
            case 1:
              let aux = usedWord.posY;
              for (let i = 0; i < usedWord.word.length; i++) {
                this.foundedWordsPositions.push(`${usedWord.posX},${aux}`)
                aux++;
              }
              break;
            case 2:
              aux = usedWord.posX;
              for (let i = 0; i < usedWord.word.length; i++) {
                this.foundedWordsPositions.push(`${aux},${usedWord.posY}`)
                aux++;
              }
              break;
          }

          this.totalFoundedWords.push(usedWord.word);

          this.validateEndGame();

          this.previousPos = '';
          this.selectedIndexs = new Array<string>();
          this.wordSelected = '';
        }
      }
    }, error => {
      console.error(error)
    });
  }

  getHint(word) {

    if (this.actualWordHinted.word) {
      alert("Only one hint at time");
      return;
    }

    var usedWord = this.responseBoard.usedWords.find(w => w.word === word.replace(" ", "").trim());
    let aux = 0;

    switch (usedWord.direction) {
      case 1:
        aux = usedWord.posY;
        for (let i = 0; i < word.replace(" ", "").trim().length; i++) {
          this.hintedWordsPositions.push(`${usedWord.posX},${aux}`)
          aux++;
        }
        break;
      case 2:
        aux = usedWord.posX;
        for (let i = 0; i < word.replace(" ", "").trim().length; i++) {
          this.hintedWordsPositions.push(`${aux},${usedWord.posY}`)
          aux++;
        }
        break;
    }

    this.countHintedWords++;
    this.actualWordHinted = usedWord;

    this.previousPos = '';
    this.selectedIndexs = new Array<string>();
    this.wordSelected = '';
  }

  private validateEndGame() {
    if (this.totalFoundedWords.length === this.responseBoard.words.length) {
      this.finish = true;
      this.score = this.totalFoundedWords.length - this.countHintedWords;
    }
  }

  resetBoard() {
    this.responseBoard = new ResponseBoard();
    this.selectedIndexs = new Array<string>();
    this.foundedWordsPositions = new Array<string>();
    this.hintedWordsPositions = new Array<string>();
    this.foundedWordsWithoutHint = new Array<string>();
    this.totalFoundedWords = new Array<string>();
    this.wordSelected = '';
    this.actualWordHinted = new UsedWord();
    this.score = 0;
    this.direction = 1;
    this.finish = false;
    this.countHintedWords = 0;

    this.getBoard();
  }
}
