import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User, UserEnvelope } from '../data/features/user';
import { HttpClient } from '@angular/common/http';

const JWT_TOKEN_KEY = "access_token";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private jwtHelper: JwtHelperService,
    private http: HttpClient) { }

  signOut() {
    localStorage.removeItem(JWT_TOKEN_KEY);
  }

  setJwtToken(token: string) {
    localStorage.setItem(JWT_TOKEN_KEY, token);
  }

  static getJwtToken(): string {
    return localStorage.getItem(JWT_TOKEN_KEY);
  }

  isJwtValid(): boolean {
    return !this.jwtHelper.isTokenExpired();
  }

  async registerAsync(email, displayName, password: string): Promise<User> {
    let response = await this.http.post<UserEnvelope>('https://snippets-api-dev.azurewebsites.net/users', {
      user: {
        email: email,
        displayName: displayName,
        password: password
      }
    }).toPromise();

    return response.user;
  }

  async loginAsync(email, password: string): Promise<User> {
    let response = await this.http.post<UserEnvelope>('https://snippets-api-dev.azurewebsites.net/users/auth', {
      user: {
        email: email,
        password: password
      }
    }).toPromise();

    return response.user;
  }
}
