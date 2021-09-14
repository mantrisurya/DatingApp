import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) { }

  getUsersWithRole() {
    return this.http.get<Partial<User[]>>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(userName?: string, roles?: string[]) {
    return this.http.post(this.baseUrl + 'admin/edit-roles/' + userName + '?roles=' + roles, {});
  }
}
