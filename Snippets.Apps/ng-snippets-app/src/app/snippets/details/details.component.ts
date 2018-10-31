import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { SnippetsService } from 'src/app/snippets/snippets.service';
import { SnippetDetails, VoteEnum } from 'src/app/snippets/snippet';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class DetailsComponent implements OnInit {

  id: string;
  snippet: SnippetDetails = new SnippetDetails();
  currentVote: string = 'removed';

  constructor(private router: Router,
    private route: ActivatedRoute,
    public snippets: SnippetsService,
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

  async upvoteSnippet() {
    this.currentVote = await this.snippets.voteAsync(this.snippet.id, VoteEnum.upvote);
  }

  async downvoteSnippet() {
    this.currentVote = await this.snippets.voteAsync(this.snippet.id, VoteEnum.downvote);
  }

}
