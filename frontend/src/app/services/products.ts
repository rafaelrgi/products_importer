import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from './../../environments/environment';
import { Product } from './../dtos/product.dto';

@Injectable({
  providedIn: 'root',
})
export class ProductsService {

  constructor(private http: HttpClient) { }

  fetchAll(
    page: number, perPage: number, sort: string, asc: boolean,
    name: string = '', priceMin: string = '', priceMax: string = '',
    expirationMin: string = '', expirationMax: string = '',
    showDeleted: boolean
  ): Observable<any> {
    const deleted = showDeleted ? '1' : '0'
    const order = asc ? 'asc' : 'desc';
    const filters = (name ? `&name=${name}` : '') +
      (priceMin ? `&priceMin=${priceMin}` : '') + (priceMax ? `&priceMax=${priceMax}` : '') +
      (expirationMin ? `&expirationMin=${expirationMin}` : '') + (expirationMax ? `&expirationMax=${expirationMax}` : '');

    return this.http.get<any>(
      `${environment.apiUrl}/products/?page=${page}&perPage=${perPage}&sort=${sort}&order=${order}${filters}&showDeleted=${deleted}`
    );
  }

  fetch(id: number): Observable<any> {
    return this.http.get<any>(`${environment.apiUrl}/products/${id}`);
  }

  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${environment.apiUrl}/products/${id}`);
  }

  undelete(id: number): Observable<any> {
    return this.http.patch<any>(`${environment.apiUrl}/products/activate/${id}`, null);
  }

  save(row: Product): Observable<any> {
    if (row.id)
      return this.http.put<any>(`${environment.apiUrl}/products/${row.id}`, row);
    return this.http.post<any>(`${environment.apiUrl}/products/`, row);
  }

}
