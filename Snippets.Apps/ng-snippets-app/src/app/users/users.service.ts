import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User, UserEnvelope } from './user';
import { ApiService } from '../services/api.service';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private http: HttpClient) { }

  public async getByIdAsync(id: string): Promise<User> {
    return (await this.http.get<UserEnvelope>(ApiService.buildApiUrl('user/' + id)).toPromise()).user;
  }
}
