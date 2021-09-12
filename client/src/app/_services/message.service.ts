import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Message } from '../_models/message';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiBaseUrl;
  constructor(private http: HttpClient) { }

  gentMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('container', container);
    return getPaginatedResult<Partial<Message[]>>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(userName?: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + userName);
  }

  sendMessage(userName: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', {recipientUserName: userName, content})
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
