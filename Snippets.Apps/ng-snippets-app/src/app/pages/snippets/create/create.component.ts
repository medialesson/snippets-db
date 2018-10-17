import { Component, OnInit } from '@angular/core';
import { SnippetPostData, SnippetPostDataEnvelope } from 'src/app/data/features/snippet';
import { SnippetsService } from 'src/app/services/snippets.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss']
})
export class CreateComponent implements OnInit {

  snippetData: SnippetPostData = new SnippetPostData();

  @BlockUI() blockUI: NgBlockUI;

  constructor(private snippets: SnippetsService) { }

  ngOnInit() {
  }

  public async submitForm() {
    this.blockUI.start('We\'re launching your snippet into orbit...');
    let snippet = await this.snippets.submitAsync(this.snippetData);

    alert(snippet.snippet.snippetId);
    setTimeout(() => {
      this.blockUI.stop();
    }, 1000);
  }
}
