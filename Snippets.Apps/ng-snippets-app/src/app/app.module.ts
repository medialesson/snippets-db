import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BlockUIModule, BlockUI } from 'ng-block-ui';
import { JwtModule } from '@auth0/angular-jwt';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegisterComponent } from './pages/auth/register/register.component';
import { LoginComponent } from './pages/auth/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { AuthService } from './services/auth.service';
import { SignoutComponent } from './pages/auth/signout/signout.component';
import { CreateComponent } from './pages/snippets/create/create.component';
import { DetailsComponent } from './pages/snippets/details/details.component';

@NgModule({
  declarations: [
    AppComponent,
    RegisterComponent,
    LoginComponent,
    HomeComponent,
    SignoutComponent,
    CreateComponent,
    DetailsComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    BlockUIModule.forRoot(),
    NgbModule,
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: AuthService.getJwtToken
      }
    }),
    AppRoutingModule
  ],
  providers: [
    AuthService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
