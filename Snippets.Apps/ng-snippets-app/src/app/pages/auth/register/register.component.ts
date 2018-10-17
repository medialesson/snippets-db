import { Component, OnInit } from '@angular/core';

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

  constructor() { }

  ngOnInit() {
  }

  public submitForm() {
    if(this.password === this.passwordConfirmation) {

    }
    else {
      alert('Your password doesn\'t match');
    }
  }

}
