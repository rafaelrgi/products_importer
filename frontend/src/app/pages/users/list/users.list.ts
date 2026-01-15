import { ChangeDetectorRef, Component } from '@angular/core';
import { UsersService } from './../../../services/users';
import { User } from './../../../dtos/user.dto';
import { AuthService } from './../../../services/auth';
import { Router } from '@angular/router';
import { PageStatus } from './../../../enums/page-status';
import { Pagination } from './../../../dtos/pagination.dto';

@Component({
  selector: 'app-users-list',
  imports: [],
  templateUrl: './users.list.html',
  styleUrl: './users.list.css',
})
export class UsersList {
  PageStatus = PageStatus;
  status: PageStatus = PageStatus.None;
  error: string = '';
  rows: User[] | null = null;
  _pagination: Pagination | null = null;
  _page: number = 1;
  _perPage: number = 15;


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
    this.usersService.fetchAll(this._page, this._perPage).subscribe({
      //success
      next: (response) => {
        this._pagination = new Pagination(response);
        this.rows = response.data;
        this.status = (this.rows?.length ?? 0) > 0 ? PageStatus.Ready : PageStatus.Empty;
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

  canPreviousPage(): boolean {
    return this._page > 1;
  }
  canNextPage(): boolean {
    return this._page < (this._pagination?.pageCount ?? 0);
  }

  firstPage(): void {
    if (!this.canPreviousPage()) return;
    this._page = 1;
    this.fetch();
  }
  previousPage(): void {
    if (!this.canPreviousPage()) return;
    this._page--;
    this.fetch();
  }
  nextPage(): void {
    if (!this.canNextPage()) return;
    this._page++;
    this.fetch();
  }
  lastPage(): void {
    if (!this.canNextPage()) return;
    this._page = (this._pagination?.pageCount!);
    this.fetch();
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
