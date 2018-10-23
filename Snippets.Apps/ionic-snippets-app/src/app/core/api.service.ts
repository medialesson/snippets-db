import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

const API_ROOT = 'https://snippets-api-dev.azurewebsites.net';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private httpClient: HttpClient) { }

  /**
   * getApiUrl
   */
  public static getApiUrl(endpoint: string): string {
    return API_ROOT + '/' + endpoint;
  }
}
