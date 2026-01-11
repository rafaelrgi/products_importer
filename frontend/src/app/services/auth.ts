import { Injectable, signal } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { User } from "../dtos/user.dto";
import { environment } from "./../../environments/environment";

@Injectable({ providedIn: "root" })
export class AuthService {
  private user: User | null = null;

  constructor(private http: HttpClient, private router: Router) { }

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${environment.authUrl}/auth`, { email, password });
  }

  logout() {
    this.user = null;
    localStorage.removeItem("user");
    localStorage.removeItem("token");
    if (this.router.url != "/login")
      this.router.navigate(["/login"]);
  }

  isAuth(): boolean {
    return (this.user !== null || localStorage.getItem("token") !== null);
  }

  getToken(): string | null {
    return localStorage.getItem("token");
  }
  setToken(token: string) {
    localStorage.setItem("token", token);
    this.router.navigate(["/"]);
  }

  getUser(): User | null {
    if (!this.user)
      this.user = JSON.parse(localStorage.getItem("user") || "null");
    return this.user;
  }
  setUser(user: User) {
    this.user = user;
    localStorage.setItem("user", JSON.stringify(user));
  }

}