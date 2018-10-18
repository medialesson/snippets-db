import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private httpClient: HttpClient) { }
  
  /**
   * getApiUrl
   */
  public static getApiUrl(endpoint: string): string {
    return environment.api.rootUrl + '/' + endpoint;
  }
}
