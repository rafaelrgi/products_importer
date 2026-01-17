import { ChangeDetectorRef, Component, Input } from '@angular/core';
import { FormBuilder, FormsModule } from '@angular/forms';
import { DatePipe, Location } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from './../../../services/auth';
import { ProductsService } from './../../../services/products';
import { Product } from './../../../dtos/product.dto';

@Component({
  selector: 'app-product-form',
  imports: [DatePipe, FormsModule],
  templateUrl: './product.form.html',
  styleUrl: './product.form.css',
})
export class ProductForm {
  @Input() id: number = 0;
  product: Product = {} as Product;
  error: string = '';
  nameError: string = '';
  expirationError: string = '';
  quantityError: string = '';
  priceError: string = '';
  expiration: string = '';
  price: number = 0;

  constructor(private fb: FormBuilder, private productsService: ProductsService, private authService: AuthService,
    private cdRef: ChangeDetectorRef, private router: Router, private location: Location) { }

  ngOnInit(): void {
    this.id = Number(this.id);
    this.fetch();
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  fetch(): void {
    //adding a new record?
    if (this.id === 0) {
      this.product = { id: 0 } as Product;
      return;
    }
    //load record from server
    this.productsService.fetch(this.id).subscribe({
      //success
      next: (response) => {
        this.product = response;
        this.expiration = this.formatDate(this.product.expiration.toString());
        this.price = this.product.price;
        this.cdRef.detectChanges();
      },
      //error
      error: (error) => {
        this.error = 'Error calling server: ' + (error.message || error.toString());
        this.cdRef.detectChanges();
      }
    });
  }

  setIsDeleted(): void {
    if (!this.product || !this.isAdmin())
      return;
    this.product!.isDeleted = !this.product!.isDeleted;
  }

  onSubmit(): void {
    if (!this.validateForm())
      return;
    this.product.isDeleted = this.product?.isDeleted ?? false;

    this.error = '';
    this.productsService.save(this.product).subscribe({
      next: (response) => {
        alert('Record saved!');
        this.close();
      },
      error: (err) => {
        this.error = err.message || err.error.message || 'Could not save the record. Please try again.';
        this.cdRef.detectChanges();
      }
    });

  }

  private validateForm(): boolean {
    this.error = '';
    this.nameError = '';
    this.expirationError = '';
    this.quantityError = '';
    this.priceError = '';

    this.product.name = this.product.name.trim();
    if (this.product.name.length < 2)
      this.nameError = 'Please enter a valid name';

    this.product.expiration = new Date(this.expiration);
    if (!(this.product.expiration instanceof Date && !isNaN(this.product.expiration.getTime())))
      this.expirationError = 'Please enter the expiration date';

    if (this.product.quantity < 0)
      this.quantityError = 'Please enter a valid quantity';

    if (this.product.price < 0)
      this.priceError = 'Please enter a valid price';

    if (this.error || this.nameError || this.expirationError || this.quantityError || this.priceError)
      return false;
    return true;
  }

  close(): void {
    this.location.back();
  }

  onPriceChange(event: Event): void {
    if (this.product.id === 0) {
      this.product.brl = this.product.cad = this.product.eur = this.product.mxn = this.product.gbp = 0;
      return;
    }
    const price = this.product.price * 10000;
    this.product.brl = Math.round((this.product.brl / this.price) * price) / 10000;
    this.product.cad = Math.round((this.product.cad / this.price) * price) / 10000;
    this.product.eur = Math.round((this.product.eur / this.price) * price) / 10000;
    this.product.mxn = Math.round((this.product.mxn / this.price) * price) / 10000;
    this.product.gbp = Math.round((this.product.gbp / this.price) * price) / 10000;
    this.price = this.product.price;
  }

  formatDate(dt: string): string {
    if (dt === '' || dt === '0001-01-01T00:00:00')
      return '';
    return dt.substring(0, 10);
  }

}