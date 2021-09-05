import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating App';
  users: any;
  constructor(private accountService: AccountService) { }
  ngOnInit(): void {
    this.setCurrentUser();
  }
  setCurrentUser() {
    const storageUser = localStorage.getItem('user');
    if (storageUser) {
      const user: User = JSON.parse(storageUser);
      this.accountService.setCurrentUser(user);
    }
  }
}
