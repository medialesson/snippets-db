import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { SnippetDetails, SnippetPostData } from 'src/app/data/features/snippet';

@Component({
  selector: 'app-snippets-editor',
  templateUrl: './snippets-editor.component.html',
  styleUrls: ['./snippets-editor.component.scss']
})
export class SnippetsEditorComponent implements OnInit {

  @Input() public snippet: SnippetPostData = new SnippetPostData();
  @Output() onSubmit = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

}
