import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { SnippetDetails } from 'src/app/data/features/snippet';

@Component({
  selector: 'app-snippets-deck',
  templateUrl: './snippets-deck.component.html',
  styleUrls: ['./snippets-deck.component.scss']
})
export class SnippetsDeckComponent implements OnInit {

  @Input() public snippets: SnippetDetails[];

  constructor() { }

  ngOnInit() {
  }

  @Output() onClick = new EventEmitter();

}
