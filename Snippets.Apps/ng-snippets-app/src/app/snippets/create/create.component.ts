import { Component, OnInit } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Router } from '@angular/router';
import { SnippetPostData } from '../snippet';
import { SnippetsService } from '../snippets.service';

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

    let id = snippet.snippet.id;

    setTimeout(() => {
      this.blockUI.stop();
      this.router.navigate([id]);
    }, 1000);
  }
}
