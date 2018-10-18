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

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegisterComponent } from './pages/auth/register/register.component';
import { LoginComponent } from './pages/auth/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { AuthService } from './services/auth.service';
import { SignoutComponent } from './pages/auth/signout/signout.component';
import { CreateComponent } from './pages/snippets/create/create.component';
import { DetailsComponent } from './pages/snippets/details/details.component';
import { SnippetsService } from './services/snippets.service';
import { ProfileDetailsComponent } from './pages/profiles/profile-details/profile-details.component';
import { UsersService } from './services/users.service';
import { SnippetsDeckComponent } from './views/snippets-deck/snippets-deck.component';
import { SnippetsEditorComponent } from './views/snippets-editor/snippets-editor.component';

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
