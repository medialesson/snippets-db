import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { SnippetsService } from 'src/app/snippets/snippets.service';
import { SnippetDetails } from 'src/app/snippets/snippet';
import { ToastrService } from 'ngx-toastr';

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
    private snippets: SnippetsService,
    private toastr: ToastrService) {

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

  copySnippet() {
    this.toastr.info('Snippet has been copied to your clipboard', '', {
      positionClass: 'toast-bottom-center'
    });
  }

}
