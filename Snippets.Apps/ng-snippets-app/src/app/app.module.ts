import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; // this is needed!
import { HttpClientModule } from '@angular/common/http';

import { JwtModule } from '@auth0/angular-jwt';
import { TimeAgoPipe } from 'time-ago-pipe';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BlockUIModule, BlockUI } from 'ng-block-ui';
import { LoadingBarHttpClientModule } from '@ngx-loading-bar/http-client';
import { LoadingBarRouterModule } from '@ngx-loading-bar/router';
import { TagInputModule } from 'ngx-chips';
import { HighlightModule } from 'ngx-highlightjs';
import { StickyModule } from 'ng2-sticky-kit';
import { ClipboardModule } from 'ngx-clipboard';
import { ToastrModule } from 'ngx-toastr';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthService } from './auth/auth.service';
import { SnippetsService } from './snippets/snippets.service';
import { SignoutComponent } from './auth/signout/signout.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { HomeComponent } from './core/home/home.component';
import { CreateComponent } from './snippets/create/create.component';
import { DetailsComponent } from './snippets/details/details.component';
import { ProfileDetailsComponent } from './profiles/profile-details/profile-details.component';
import { SnippetsDeckComponent } from './snippets/snippets-deck/snippets-deck.component';
import { SnippetsEditorComponent } from './snippets/snippets-editor/snippets-editor.component';
import { UsersService } from './users/users.service';

@NgModule({
  declarations: [
    AppComponent,
    RegisterComponent,
    LoginComponent,
    HomeComponent,
    SignoutComponent,
    CreateComponent,
    DetailsComponent,

    TimeAgoPipe,

    ProfileDetailsComponent,
    SnippetsDeckComponent,
    SnippetsEditorComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: AuthService.getJwtToken,
        headerName: 'Authorization',
        whitelistedDomains: [
          'localhost:5001', 
          'localhost:5000', 
          'snippets-api-dev.azurewebsites.net'
        ]
      }
    }),
    BrowserAnimationsModule,
    AppRoutingModule,

    BlockUIModule.forRoot(),
    NgbModule,
    TagInputModule,
    HighlightModule.forRoot(),
    StickyModule,
    ClipboardModule,
    ToastrModule.forRoot() ,

    LoadingBarHttpClientModule,
    LoadingBarRouterModule
  ],
  providers: [
    AuthService,
    SnippetsService,
    UsersService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
