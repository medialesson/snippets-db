import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/auth/auth.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  email: string;
  password: string;

  @BlockUI() blockUI: NgBlockUI;

  constructor(private authService: AuthService,
    private router: Router) { }

  ngOnInit() {
  }

  public async submitForm() {
    this.blockUI.start('Signing you in...');

    let user = await this.authService.loginAsync(this.email, this.password);
    this.authService.setJwtToken(user.token);

    // Same story here
    setTimeout(() => {
      this.blockUI.stop();
      this.router.navigate(['/']);
    }, 1000);
  }

}
