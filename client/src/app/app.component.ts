import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating App';
  users: any;
  apiUrl = 'https://localhost:5001/api/AppUsers';
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.getUsers();
  }
  getUsers() {
    this.http.get(this.apiUrl).subscribe(
        result => { this.users = result; },
      error => { console.log("Error getting app users: {0}", error); }
      )
  }
}
