import { Component, Inject, OnInit } from '@angular/core';
import { ResponseBoard } from '../models/response-board';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  baseUrl: string;
  responseBoard: ResponseBoard;
  maxSize: number = 0;
  wordsLetters: string[][];

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.responseBoard = new ResponseBoard();
    this.baseUrl = baseUrl;
  }

  ngOnInit(): void {
    this.http.get(this.baseUrl + 'api/words/board').subscribe(result => {
      debugger;
      this.responseBoard = result as ResponseBoard;
      //this.maxSize = result["maxSize"];

      //this.wordsLetters = [];

      //for (var i = 0; i < this.maxSize; i++) {
      //  this.wordsLetters[i] = [];
      //  for (var j = 0; j < this.maxSize; j++) {
      //    this.wordsLetters[i][j] = result["wordsLetters"][i][j];
      //  }
      //}

    }, error => console.error(error));
  }
}
