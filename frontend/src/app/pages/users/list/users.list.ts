import { ChangeDetectorRef, Component } from '@angular/core';
import { UsersService } from './../../../services/users';
import { User } from './../../../dtos/user.dto';
import { AuthService } from './../../../services/auth';
import { Router } from '@angular/router';
import { PageStatus } from './../../../enums/page-status';

@Component({
  selector: 'app-users-list',
  imports: [],
  templateUrl: './users.list.html',
  styleUrl: './users.list.css',
})
export class UsersList {
  public PageStatus = PageStatus;
  public status: PageStatus = PageStatus.None;
  public error: string = '';
  public users: User[] | null = null;

  constructor(private usersService: UsersService, private authService: AuthService,
    private cdRef: ChangeDetectorRef, private router: Router) { }

  ngOnInit(): void {
    if (this.authService.isAdmin()) {
      this.fetch();
      return;
    }

    const id = this.authService.getUser()?.id ?? 0;
    if (id > 0)
      this.router.navigate([`/users/${id}`]);
    else
      this.router.navigate(['/home']);
  }

  fetch(): void {
    this.status = PageStatus.Loading;
    this.usersService.fetchAll().subscribe({
      //success
      next: (response) => {
        this.users = response.data;
        this.status = (this.users?.length ?? 0) > 0 ? PageStatus.Ready : PageStatus.Empty;
        this.cdRef.detectChanges();
      },
      //error
      error: (error) => {
        this.status = PageStatus.Error;
        this.error = 'Error calling server: ' + (error.message || error.toString());
        this.cdRef.detectChanges();
      }
    });
  }

  add(): void {
    this.router.navigate(['/users/0']);
  }

  edit(id: number): void {
    if (id <= 0)
      return;
    this.router.navigate([`/users/${id}`]);
  }

  delete(id: number): void {
    this.status = PageStatus.Loading;
    if (!window.confirm('Are you sure you want to delete this record?'))
      return;
    this.usersService.delete(id).subscribe({
      //success
      next: (response) => {
        this.status = PageStatus.Ready;
        this.fetch();
        this.cdRef.detectChanges();
      },
      //error
      error: (error) => {
        this.status = PageStatus.Error;
        this.error = 'Error calling server: ' + (error.message || error.toString());
        this.cdRef.detectChanges();
      }
    });
  }

  undelete(id: number): void {
    this.status = PageStatus.Loading;
    this.usersService.undelete(id).subscribe({
      //success
      next: (response) => {
        this.status = PageStatus.Ready;
        this.fetch();
      },
      //error
      error: (error) => {
        this.status = PageStatus.Error;
        this.error = 'Error calling server: ' + (error.message || error.toString());
        this.cdRef.detectChanges();
      }
    });
  }

}
