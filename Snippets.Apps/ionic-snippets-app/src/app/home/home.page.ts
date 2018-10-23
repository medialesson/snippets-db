import { Component } from '@angular/core';
import { SnippetsService } from '../snippets/snippets.service';
import { SnippetDetails } from '../snippets/snippet';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss']
})
export class HomePage {

  snippetsList: SnippetDetails[];

  constructor(private snippets: SnippetsService) {
    snippets.getAllAsync().then(data => {
      this.snippetsList = data.snippets;
    });
  }
}
