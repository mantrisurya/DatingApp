import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from '../../_models/member';
import { Pagination } from '../../_models/pagination';
import { User } from '../../_models/user';
import { UserParams } from '../../_models/userParams';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members?: Member[];
  pagination?: Pagination;
  userParams?: UserParams;
  user?: User;
  genderList = [{ value: 'male', display: 'Males' }, {value: 'female', display: 'Females'}]

  constructor(private memberService: MembersService) {
    this.userParams = memberService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.setUserParams(this.userParams);
    this.memberService.getMembers(this.userParams)?.subscribe(res => {
      this.members = res.result;
      this.pagination = res.pagination;
    })
  }

  resetFilters() {
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.userParams) {
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
    }
    this.loadMembers();
  }
}
