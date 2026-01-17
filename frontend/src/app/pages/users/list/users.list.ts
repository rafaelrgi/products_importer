import { ChangeDetectorRef, Component } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from './../../../services/users';
import { User } from './../../../dtos/user.dto';
import { AuthService } from './../../../services/auth';
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
  pagination: Pagination | null = null;
  params: any | null = null;


  constructor(private usersService: UsersService, private authService: AuthService, private location: Location,
    private route: ActivatedRoute, private cdRef: ChangeDetectorRef, private router: Router) { }

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

    if (!this.params)
      this.params = this.getQueryParams();
    const url = this.getQueryUrl();
    //save state in the query params
    this.location.replaceState('/users', url);

    this.usersService.fetchAll(url).subscribe({
      //success
      next: (response) => {
        this.pagination = new Pagination(response);
        this.rows = response.data;
        this.status = (this.rows?.length ?? 0) > 0 ? PageStatus.Ready : PageStatus.Empty;
        this.cdRef.detectChanges();
      },
      //error
      error: (error) => {
        if (error.status == 401) {
          this.router.navigate(['/login/']);
          return;
        }
        this.status = PageStatus.Error;
        this.error = 'Error calling server: ' + (error.message || error.toString());
        this.cdRef.detectChanges();
      }
    });
  }

  getQueryUrl(): string {
    const url = `page=${this.params.page}&perPage=${this.params.perPage}`;
    return url;
  }

  getQueryParams(): any {
    const params = {
      page: this.route.snapshot.queryParamMap.get('page') ?? 1,
      perPage: this.route.snapshot.queryParamMap.get('perPage') ?? 15,
    };
    return params;
  }

  canPreviousPage(): boolean {
    return this.params.page > 1;
  }
  canNextPage(): boolean {
    return this.params.page < (this.pagination?.pageCount ?? 0);
  }

  firstPage(): void {
    if (!this.canPreviousPage()) return;
    this.params.page = 1;
    this.fetch();
  }
  previousPage(): void {
    if (!this.canPreviousPage()) return;
    this.params.page--;
    this.fetch();
  }
  nextPage(): void {
    if (!this.canNextPage()) return;
    this.params.page++;
    this.fetch();
  }
  lastPage(): void {
    if (!this.canNextPage()) return;
    this.params.page = (this.pagination?.pageCount!);
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
