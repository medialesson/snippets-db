import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { HttpClient } from '@angular/common/http';
import { SnippetPostData, SnippetPostDataEnvelope, SnippetDetailsEnvelope, SnippetsDetailsEnvelope } from '../data/features/snippet';
import { ApiService } from './api.service';

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
}
