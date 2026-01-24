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

  fetchAll(queryUrl: string): Observable<any> {
    return this.http.get<any>(
      `${environment.apiUrl}/products/?${queryUrl}`
    );
  }

  fetch(id: number): Observable<any> {
    return this.http.get<any>(`${environment.apiUrl}/products/${id}?showDeleted=true`);
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
