import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

const JWT_TOKEN_KEY = "access_token";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private jwtHelper: JwtHelperService) { }

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
}
