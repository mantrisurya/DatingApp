import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiBaseUrl;
  members?: Member[];
  memberCache = new Map();
  user?: User;
  userParams?: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    accountService.currentUser$.pipe(take(1)).subscribe(res => {
      this.user = res;
      this.userParams = new UserParams(res);
    }) }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params?: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams?: UserParams) {
    var response = this.memberCache.get(Object.values(userParams ?? {}).join('-'));
    if (response) {
      return of(response);
    }
    if (userParams) {
      let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
      params = params.append('minAge', userParams.minAge.toString());
      params = params.append('maxAge', userParams.maxAge.toString());
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
      return this.getPaginatedResult<Member[]>(this.baseUrl + 'appusers', params)
        .pipe(map(res => {
          this.memberCache.set(Object.values(userParams).join('-'), res);
          return res;
        }));
    }
    return undefined;
  }

  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body ?? undefined;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination') ?? "");
        }
        return paginatedResult;
      }));
  };

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('pageSize', pageSize.toString());
    params = params.append('pageNumber', pageNumber.toString());

    return params;
  }

  getMember(userName?: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === userName);
    if (member) {
      return of(member);;
    }
    return this.http.get<Member>(this.baseUrl + 'appusers/' + userName);
  }

  updateMember(member?: Member) {
    return this.http.put(this.baseUrl + 'appusers', member).pipe(
      map(() => {
        if (member && this.members) {
          const index = this.members?.indexOf(member);
          this.members[index] = member;
        }
      })
    );
  }
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'appusers/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'appusers/delete-photo/' + photoId);
  }
}
