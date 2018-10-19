import { Component } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'ng-snippets-app';

  @BlockUI() blockUI: NgBlockUI;
  public isCollapsed: boolean = true;

  constructor(public auth: AuthService) {
  }

  public get isSignedIn() : boolean {
    return this.auth.isJwtValid();
  }
  
}
