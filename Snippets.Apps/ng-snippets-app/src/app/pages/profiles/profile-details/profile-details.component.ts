import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from 'src/app/services/users.service';
import { User } from 'src/app/data/features/user';

@Component({
  selector: 'app-profile-details',
  templateUrl: './profile-details.component.html',
  styleUrls: ['./profile-details.component.scss']
})
export class ProfileDetailsComponent implements OnInit {

  id: string;
  user: User = new User();

  constructor(private router: Router,
    private route: ActivatedRoute,
    private users: UsersService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.id = params['id'];

      this.users.getByIdAsync(this.id).then(result => {
        this.user = result;
      });
    });
  }

}
