import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Toast, ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnectoin?: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnectoin = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

    this.hubConnectoin
      .start()
      .catch(error => console.log(error));

    this.hubConnectoin.on('UserIsOnline', userName => {
      this.onlineUsers$.pipe(take(1)).subscribe(userNames => {
        this.onlineUsersSource.next([...userNames, userName])
      })
    })

    this.hubConnectoin.on('UserIsOffline', userName => {
      this.toastr.warning(userName + ' has disconnected');
      this.onlineUsers$.pipe(take(1)).subscribe(userNames => {
        this.onlineUsersSource.next([...userNames.filter(x => x !== userName)])
      })
    })

    this.hubConnectoin.on('GetOnlineUsers', (userNames: string[]) => {
      this.onlineUsersSource.next(userNames);
    })

    this.hubConnectoin.on("NewMessageReceived", ({ userName, knownAs }) => {
      this.toastr.info(knownAs + ' has sent you a new message!')
        .onTap
        .pipe(take(1)).subscribe(() => {
          this.router.navigateByUrl('/members/' + userName + '?tab=3')
        });
    })
  }

  stopHubConnection() {
    this.hubConnectoin?.stop().catch(error => console.log(error));
  }
}
