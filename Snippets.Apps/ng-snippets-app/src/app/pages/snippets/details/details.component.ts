import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { SnippetsService } from 'src/app/services/snippets.service';
import { SnippetDetails } from 'src/app/data/features/snippet';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class DetailsComponent implements OnInit {

  id: string;
  snippet: SnippetDetails = new SnippetDetails();

  constructor(private router: Router,
    private route: ActivatedRoute,
    private snippets: SnippetsService) {

    this.route.params.subscribe(params => {
      this.id = params['id'];

      this.snippets.getAsync(this.id).then(envelope => {
        this.snippet = envelope.snippet;
      });
    });
  }

  ngOnInit() {
  }

  downloadSnippet() {
    
  }

}
