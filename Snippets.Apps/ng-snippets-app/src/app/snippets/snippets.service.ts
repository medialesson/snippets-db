import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';
import { SnippetPostData, SnippetDetailsEnvelope, SnippetPostDataEnvelope, SnippetsDetailsEnvelope } from './snippet';
import { ApiService } from '../services/api.service';

@Injectable({
  providedIn: 'root'
})
export class SnippetsService {

  constructor(private auth: AuthService,
    private http: HttpClient) { }

  async submitAsync(data: SnippetPostData): Promise<SnippetDetailsEnvelope> {
    let envelope = new SnippetPostDataEnvelope(data);
    return await this.http.post<SnippetDetailsEnvelope>(ApiService.getApiUrl('snippets'),
      envelope).toPromise();
  }

  async getAsync(id: string): Promise<SnippetDetailsEnvelope> {
    return await this.http.get<SnippetDetailsEnvelope>(ApiService.getApiUrl('snippets/' + id))
      .toPromise();
  }

  async getAllAsync(): Promise<SnippetsDetailsEnvelope> {
    return await this.http.get<SnippetsDetailsEnvelope>(ApiService.getApiUrl('snippets/'))
      .toPromise();
  }

  async getByUserAsync(id: string): Promise<SnippetsDetailsEnvelope> {
    let queryParams = new HttpParams().set('author', id);

    return await this.http.get<SnippetsDetailsEnvelope>(ApiService.getApiUrl('snippets'), { 
      params: queryParams 
    }).toPromise();
  }
}
