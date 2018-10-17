import { Component, OnInit } from '@angular/core';
import { SnippetsService } from 'src/app/services/snippets.service';
import { SnippetDetails } from 'src/app/data/features/snippet';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  snippetsList: SnippetDetails[];

  constructor(private snippets: SnippetsService,
    private router: Router) { }

  async ngOnInit() {
    this.snippetsList = (await this.snippets.getAllAsync()).snippets;
  }

  navigateToSnippetById(id: string) {
    this.router.navigate([id]);
  }

}
