import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiBaseUrl;
  members?: Member[];
  constructor(private http: HttpClient) { }

  getMembers() : Observable<Member[]> {
    if (this.members) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'appusers').pipe(
      map((members: Member[]) => {
        this.members = members;
        return this.members;
      })
    );
  }

  getMember(userName?: string) {
    const member = this.members?.find(x => x.userName === userName);
    if (member !== undefined) return of(member);
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
