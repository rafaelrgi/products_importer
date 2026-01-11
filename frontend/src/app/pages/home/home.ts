import { Component, ViewChild, OnInit, ChangeDetectorRef } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { ApiService } from '../../services/api';
import { AuthService } from '../../services/auth';
import { ApiStatus } from '../../dtos/api_status.dto';
import { User } from "./../../dtos/user.dto";

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  public apiStatus: ApiStatus | null = null;

  constructor(private apiService: ApiService, private authService: AuthService, private cdRef: ChangeDetectorRef) { }

  @ViewChild('sidenav') sidenav!: MatSidenav;

  ngOnInit(): void {
    this.apiService.getStatus().subscribe({
      //success
      next: (response) => {
        if (!response.isAuth)
          return this.authService.logout();

        if (response.user)
          this.authService.setUser(response.user);

        this.apiStatus = response;
        this.cdRef.detectChanges();
      },
      //error
      error: (error) => {
        this.apiStatus = <ApiStatus>{
          status: `Error ${error.status}: ${error.message}`,
          baseDir: "",
          isAuth: false,
          isDocker: false,
          user: <User>{}
        };
        console.error('Error calling server:', error);
        this.cdRef.detectChanges();
      }
    });
  }

  toggleSidenav() {
    this.sidenav.toggle();
  }

}
