import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Group } from '../_models/group';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { AccountService } from './account.service';
import { BusyService } from './busy.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiBaseUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);

  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient, private busyService: BusyService) { }

  createHubConnectoin(user: User, otherUserName: string) {
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUserName, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

    this.hubConnection?.start().catch(error => console.log(error))
      .finally(() => this.busyService.idle());
    this.hubConnection.on("ReceiveMessageThread", messages => {
      this.messageThreadSource.next(messages);
    })

    this.hubConnection.on("NewMessage", message => {
      this.messageThread$.pipe(take(1)).subscribe(messages => {
        this.messageThreadSource.next([...messages, message])
      })
    })

    this.hubConnection.on("UpdatedGroup", (group: Group) => {
      if (group.connections.some(x => x.userName === otherUserName)) {
        this.messageThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message => {
            if (!message.dateRead) {
              message.dateRead = new Date(Date.now())
            }
          })
          this.messageThreadSource.next([...messages]);
        })
      }
    })
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.messageThreadSource.next([]);
      this.hubConnection?.stop();
    }
  }

  gentMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('container', container);
    return getPaginatedResult<Partial<Message[]>>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(userName?: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + userName);
  }

  async sendMessage(userName: string, content: string) {
    return this.hubConnection?.invoke("SendMessage", { recipientUserName: userName, content })
      .catch(error => console.log(error));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
