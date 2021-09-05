import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}
  user: any = {}
  constructor(public accounService: AccountService) { }
  ngOnInit(): void {
  }
  login() {
    this.accounService.login(this.model).subscribe(response => {
      this.user = response;
        console.log(response);
      }, error => {
        console.log(error);
      }
    );
  }
  logout() {
    this.accounService.logout();
  }

}
