import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from 'src/app/users/user';
import { SnippetDetails } from 'src/app/snippets/snippet';
import { SnippetsService } from 'src/app/snippets/snippets.service';
import { UsersService } from 'src/app/users/users.service';

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

  public navigateToSnippetById(id: string) {
    this.router.navigate([id]);
  }

}
