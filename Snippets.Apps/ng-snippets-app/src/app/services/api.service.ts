import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private httpClient: HttpClient) { }

  // get(path: string) {
  //   return this.httpClient.get(environment.api.rootUrl + '/' + path);
  // }
}
