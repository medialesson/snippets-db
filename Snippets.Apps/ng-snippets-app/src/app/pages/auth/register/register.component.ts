import { Component, OnInit } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

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

  constructor() { }

  ngOnInit() {
  }

  public submitForm() {
    this.blockUI.start('Hang tight...');
    setTimeout(() => {
      this.blockUI.stop();
    }, 5000);
  }

}
