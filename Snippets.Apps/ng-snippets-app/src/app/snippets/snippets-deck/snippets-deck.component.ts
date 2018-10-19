import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { SnippetDetails } from 'src/app/snippets/snippet';

@Component({
  selector: 'app-snippets-deck',
  templateUrl: './snippets-deck.component.html',
  styleUrls: ['./snippets-deck.component.scss']
})
export class SnippetsDeckComponent implements OnInit {

  @Input() public snippets: SnippetDetails[];

  @Output() onItemClick = new EventEmitter();
  @Output() onDisplayNameClick = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

}
