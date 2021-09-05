import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css']
})
export class NotFoundComponent implements OnInit {

  constructor(public accounService: AccountService) { }

  ngOnInit(): void {
  }

}
