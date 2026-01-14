import { ChangeDetectorRef, Component, Inject, LOCALE_ID } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from './../../../services/auth';
import { ProductsService } from './../../../services/products';
import { PageStatus } from './../../../enums/page-status';
import { Product } from './../../../dtos/product.dto';
import { Pagination } from './../../../dtos/pagination.dto';
import { formatDate } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-products-list',
  imports: [FormsModule],
  templateUrl: './products.list.html',
  styleUrl: './products.list.css',
})
export class ProductsList {
  public PageStatus = PageStatus;
  public status: PageStatus = PageStatus.None;
  public error: string = '';
  public pagination: Pagination | null = null;
  public rows: Product[] | null = null;
  _sort: string = 'name';
  _asc: boolean = true;
  _page: number = 1;
  _perPage: number = 15;
  _filtering: boolean = false;
  _name: string = '';
  _expMin: string = '';
  _expMax: string = '';
  _priceMin: string = '';
  _priceMax: string = '';

  constructor(private productsService: ProductsService, private authService: AuthService,
    private cdRef: ChangeDetectorRef, private router: Router, private route: ActivatedRoute,
    @Inject(LOCALE_ID) private locale: string) { }

  ngOnInit(): void {
    this.fetch();
  }

  fetch(): void {
    this.status = PageStatus.Loading;
    this.productsService.fetchAll(this._page, this._perPage, this._sort, this._asc, this._name, this._priceMin, this._priceMax, this._expMin, this._expMax).subscribe({
      //success
      next: (response) => {
        this.pagination = new Pagination(response);
        this.rows = response.data;
        this.status = (this.pagination?.hasData ?? false) ? PageStatus.Ready : PageStatus.Empty;
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

  formatDate(dt: Date): string {
    const s = formatDate(dt, 'MM/dd/yyyy', this.locale);
    return (s == '01/01/0001') ? ' -- ' : s;
  }

  canPreviousPage(): boolean {
    return this._page > 1;
  }
  canNextPage(): boolean {
    return this._page < (this.pagination?.pageCount ?? 0);
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
    this._page = (this.pagination?.pageCount!);
    this.fetch();
  }

  getSortIcon(column: string): string {
    if (this._sort !== column)
      return 'swap_vert';
    if (this._asc)
      return 'arrow_downward';
    return 'arrow_upward';
  }

  //"id" or "name" or "quantity" or "price" or "expiration"
  sort(column: string): void {
    if (this._sort == column)
      this._asc = !this._asc;
    else {
      this._sort = column;
      this._asc = true;
    }

    this._page = 1;
    this.fetch();
  }

  showHideFilters(): void {
    this._filtering = !this._filtering;
    if (!this._filtering)
      this.clearFilters();
  }

  clearFilters(): void {
    this._name = '';
    this._expMin = '';
    this._expMax = '';
    this._priceMin = '';
    this._priceMax = '';
    this._page = 1;
    this.fetch();
  }

  filter(): void {
    this._page = 1;
    this.fetch();
  }

  add(): void {
    this.router.navigate(['/products/0']);
  }

  edit(id: number): void {
    if (id <= 0)
      return;
    this.router.navigate([`/products/${id}`]);
  }

  delete(id: number): void {
    this.status = PageStatus.Loading;
    if (!window.confirm('Are you sure you want to delete this record?'))
      return;
    this.productsService.delete(id).subscribe({
      //success
      next: (response) => {
        this.status = PageStatus.Ready;
        //TODO: this.fetch();
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
    this.productsService.undelete(id).subscribe({
      //success
      next: (response) => {
        this.status = PageStatus.Ready;
        //TODO: this.fetch();
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