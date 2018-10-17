import { Component, OnInit } from '@angular/core';
import { SnippetPostData, SnippetPostDataEnvelope } from 'src/app/data/features/snippet';
import { SnippetsService } from 'src/app/services/snippets.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss']
})
export class CreateComponent implements OnInit {

  snippetData: SnippetPostData = new SnippetPostData();

  @BlockUI() blockUI: NgBlockUI;

  constructor(private snippets: SnippetsService,
    private router: Router) { }

  ngOnInit() {
  }

  public async submitForm() {
    this.blockUI.start('Snipping away...');
    let snippet = await this.snippets.submitAsync(this.snippetData);

    let id = snippet.snippet.snippetId;
    

    setTimeout(() => {
      this.blockUI.stop();
      this.router.navigate([id]);
    }, 1000);
  }
}
