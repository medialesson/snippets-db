import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';
import { SnippetPostData, SnippetDetailsEnvelope, SnippetPostDataEnvelope, SnippetsDetailsEnvelope, VoteEnum, VoteEnvelope } from './snippet';
import { ApiService } from '../services/api.service';

@Injectable({
  providedIn: 'root'
})
export class SnippetsService {

  constructor(private auth: AuthService,
    private http: HttpClient) { }

  async submitAsync(data: SnippetPostData): Promise<SnippetDetailsEnvelope> {
    data.language = 'plain';
    let envelope = new SnippetPostDataEnvelope(data);
    return await this.http.post<SnippetDetailsEnvelope>(ApiService.buildApiUrl('snippets'),
      envelope).toPromise();
  }

  async getAsync(id: string): Promise<SnippetDetailsEnvelope> {
    return await this.http.get<SnippetDetailsEnvelope>(ApiService.buildApiUrl('snippets/' + id))
      .toPromise();
  }

  async getAllAsync(): Promise<SnippetsDetailsEnvelope> {
    return await this.http.get<SnippetsDetailsEnvelope>(ApiService.buildApiUrl('snippets/'))
      .toPromise();
  }

  async getByUserAsync(id: string): Promise<SnippetsDetailsEnvelope> {
    let queryParams = new HttpParams().set('author', id);

    return await this.http.get<SnippetsDetailsEnvelope>(ApiService.buildApiUrl('snippets'), { 
      params: queryParams 
    }).toPromise();
  }

  async voteAsync(id: string, vote: VoteEnum): Promise<string> {
    let term: string;

    switch(vote) { // Sorry for this ugly switch/case
      case VoteEnum.upvote: term = 'upvote'; break;
      case VoteEnum.downvote: term = 'downvote'; break;
    }

    return (await this.http.post<VoteEnvelope>(ApiService.buildApiUrl(`snippets/${id}/${term}`), { }).toPromise()).vote.status;
  }
  
  buildRawUrl(id: string): string {
    return ApiService.buildApiUrl(`snippets/${id}/raw`);
  }
}
