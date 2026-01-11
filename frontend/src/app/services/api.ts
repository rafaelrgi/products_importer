import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiStatus } from '../dtos/api_status.dto';
import { environment } from "./../../environments/environment";

@Injectable({
  providedIn: 'root',
})
export class ApiService {

  constructor(private http: HttpClient) { }

  getStatus(): Observable<ApiStatus> {
    return this.http.get<ApiStatus>(environment.authUrl);
  }

}
