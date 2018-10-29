import { Component, OnInit } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { HttpClient } from '@angular/common/http';
import { AuthService } from 'src/app/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  email: string;
  displayName: string;
  password: string;
  passwordConfirmation: string;

  @BlockUI() blockUI: NgBlockUI;

  constructor(private httpClient: HttpClient,
    private authService: AuthService,
    private router: Router) { }

  ngOnInit() {
  }

  public async submitForm() {
    // Block UI
    this.blockUI.start('Hang tight,\nwe\'re creating your account...');

    // Send request
    let user = await this.authService.registerAsync(this.email, this.displayName, this.password);
    this.authService.setJwtToken(user.tokens.token);

    // Add timeout for UX because the 
    // sign up process is actually blazing fast
    setTimeout(() => {
      this.blockUI.stop();
      this.router.navigate(['/']);
    }, 1500);
  }

}
