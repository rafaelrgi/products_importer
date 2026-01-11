import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from './../../environments/environment';
import { User } from '../dtos/user.dto';

@Injectable({
  providedIn: 'root',
})
export class UsersService {

  constructor(private http: HttpClient) { }

  fetchAll(): Observable<any> {
    return this.http.get<any>(`${environment.authUrl}/users/?page=1&perPage=-1`);
  }

  fetch(id: number): Observable<any> {
    return this.http.get<any>(`${environment.authUrl}/users/${id}`);
  }

  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${environment.authUrl}/users/${id}`);
  }

  undelete(id: number): Observable<any> {
    return this.http.patch<any>(`${environment.authUrl}/users/activate/${id}`, null);
  }

  save(user: User): Observable<any> {
    if (user.id)
      return this.http.put<any>(`${environment.authUrl}/users/${user.id}`, user);
    return this.http.post<any>(`${environment.authUrl}/users/`, user);
  }

}
