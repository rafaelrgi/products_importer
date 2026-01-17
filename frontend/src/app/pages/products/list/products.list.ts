import { ChangeDetectorRef, Component, Inject, LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { formatDate, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from './../../../services/auth';
import { ProductsService } from './../../../services/products';
import { PageStatus } from './../../../enums/page-status';
import { Product } from './../../../dtos/product.dto';
import { Pagination } from './../../../dtos/pagination.dto';

@Component({
  selector: 'app-products-list',
  imports: [FormsModule],
  templateUrl: './products.list.html',
  styleUrl: './products.list.css',
})
export class ProductsList {
  PageStatus = PageStatus;
  status: PageStatus = PageStatus.None;
  error: string = '';
  rows: Product[] | null = null;
  pagination: Pagination | null = null;
  filtering: boolean = false;
  params: any | null = null;

  constructor(private productsService: ProductsService, private authService: AuthService,
    private cdRef: ChangeDetectorRef, private router: Router, private route: ActivatedRoute, private location: Location,
    @Inject(LOCALE_ID) private locale: string) { }

  ngOnInit(): void {
    this.fetch();
  }

  fetch(): void {
    this.status = PageStatus.Loading;

    if (!this.params)
      this.params = this.getQueryParams();
    const url = this.getQueryUrl();
    //save state in the query params
    this.location.replaceState('/products', url);

    this.productsService.fetchAll(url).subscribe({
      //success
      next: (response) => {
        this.pagination = new Pagination(response);
        this.rows = response.data;
        this.status = (this.pagination?.hasData ?? false) ? PageStatus.Ready : PageStatus.Empty;
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
    const deleted = Number(this.params.showDeleted) ? '1' : '0'
    const order = Number(this.params.asc) ? 'asc' : 'desc';
    const filters =
      (this.params.name ? `&name=${this.params.name}` : '') +
      (this.params.priceMin ? `&priceMin=${this.params.priceMin}` : '') +
      (this.params.priceMax ? `&priceMax=${this.params.priceMax}` : '') +
      (this.params.expirationMin ? `&expirationMin=${this.params.expirationMin}` : '') +
      (this.params.expirationMax ? `&expirationMax=${this.params.expirationMax}` : '');

    const url = `page=${this.params.page}&perPage=${this.params.perPage}&sort=${this.params.sort}&order=${order}${filters}&showDeleted=${deleted}`;
    return url;
  }

  getQueryParams(): any {
    const params = {
      page: this.route.snapshot.queryParamMap.get('page') ?? 1,
      perPage: this.route.snapshot.queryParamMap.get('perPage') ?? 15,
      sort: this.route.snapshot.queryParamMap.get('sort') ?? 'name',
      asc: Number(this.route.snapshot.queryParamMap.get('asc') ?? 1),
      name: this.route.snapshot.queryParamMap.get('name'),
      expMin: this.route.snapshot.queryParamMap.get('expMin'),
      expMax: this.route.snapshot.queryParamMap.get('expMax'),
      priceMin: this.route.snapshot.queryParamMap.get('priceMin'),
      priceMax: this.route.snapshot.queryParamMap.get('priceMax'),
      showDeleted: Number(this.route.snapshot.queryParamMap.get('showDeleted') ?? 0),
    };
    return params;
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  formatDate(dt: Date): string {
    const s = formatDate(dt, 'MM/dd/yyyy', this.locale);
    return (s == '01/01/0001') ? ' -- ' : s;
  }

  canPreviousPage(): boolean {
    return this.pagination!.page > 1;
  }
  canNextPage(): boolean {
    return this.pagination!.page < (this.pagination!.pageCount);
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

  getSortIcon(column: string): string {
    if (this.params.sort !== column)
      return 'swap_vert';
    if (this.params.asc)
      return 'arrow_downward';
    return 'arrow_upward';
  }

  //"id" or "name" or "quantity" or "price" or "expiration"
  sort(column: string): void {
    if (this.params.sort == column)
      this.params.asc = !this.params.asc;
    else {
      this.params.sort = column;
      this.params.asc = true;
    }

    this.params.page = 1;
    this.fetch();
  }

  showHideFilters(): void {
    this.filtering = !this.filtering;
    if (!this.filtering)
      this.clearFilters();
  }

  showHideDeleted(): void {
    this.params.showDeleted = !this.params.showDeleted;
    this.params.page = 1;
    this.fetch();
  }

  clearFilters(): void {
    this.params.name = '';
    this.params.expMin = '';
    this.params.expMax = '';
    this.params.priceMin = '';
    this.params.priceMax = '';
    this.params.page = 1;
    this.fetch();
  }

  filter(): void {
    this.params.page = 1;
    this.fetch();
  }

  add(): void {
    if (!this.isAdmin())
      return;
    this.router.navigate(['/products/0']);
  }

  edit(id: number): void {
    if (id <= 0)
      return;
    this.router.navigate([`/products/${id}`]);
  }

  delete(id: number): void {
    if (!this.isAdmin())
      return;
    this.status = PageStatus.Loading;
    if (!window.confirm('Are you sure you want to delete this record?'))
      return;
    this.productsService.delete(id).subscribe({
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
    if (!this.isAdmin())
      return;
    this.status = PageStatus.Loading;
    this.productsService.undelete(id).subscribe({
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