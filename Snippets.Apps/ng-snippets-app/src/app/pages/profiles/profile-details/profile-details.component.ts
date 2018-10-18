import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from 'src/app/services/users.service';
import { User } from 'src/app/data/features/user';
import { SnippetDetails } from 'src/app/data/features/snippet';
import { SnippetsService } from 'src/app/services/snippets.service';

@Component({
  selector: 'app-profile-details',
  templateUrl: './profile-details.component.html',
  styleUrls: ['./profile-details.component.scss']
})
export class ProfileDetailsComponent implements OnInit {

  id: string;
  user: User = new User();
  userSnippets: SnippetDetails[];

  constructor(private router: Router,
    private route: ActivatedRoute,
    private users: UsersService,
    private snippets: SnippetsService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.id = params['id'];

      this.users.getByIdAsync(this.id).then(result => {
        this.user = result;
      });

      this.snippets.getByUserAsync(this.id).then(result => {
        this.userSnippets = result.snippets;
      });
    });
  }

}
