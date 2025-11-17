import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { MarketplaceListing, CreateListingRequest } from '../models/marketplace.model';

@Injectable({
  providedIn: 'root'
})
export class MarketplaceService {
  private apiUrl = `${environment.apiUrl}/marketplace`;

  constructor(private http: HttpClient) {}

  getListings(
    category?: string,
    minPrice?: number,
    maxPrice?: number,
    condition?: string,
    location?: string
  ): Observable<MarketplaceListing[]> {
    let params = new HttpParams();

    if (category) params = params.set('category', category);
    if (minPrice !== undefined) params = params.set('minPrice', minPrice.toString());
    if (maxPrice !== undefined) params = params.set('maxPrice', maxPrice.toString());
    if (condition) params = params.set('condition', condition);
    if (location) params = params.set('location', location);

    return this.http.get<MarketplaceListing[]>(this.apiUrl, { params });
  }

  getListing(id: string): Observable<MarketplaceListing> {
    return this.http.get<MarketplaceListing>(`${this.apiUrl}/${id}`);
  }

  createListing(request: CreateListingRequest): Observable<MarketplaceListing> {
    return this.http.post<MarketplaceListing>(this.apiUrl, request);
  }

  updateListing(id: string, request: Partial<CreateListingRequest>): Observable<MarketplaceListing> {
    return this.http.put<MarketplaceListing>(`${this.apiUrl}/${id}`, request);
  }

  deleteListing(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getMyListings(): Observable<MarketplaceListing[]> {
    return this.http.get<MarketplaceListing[]>(`${this.apiUrl}/my-listings`);
  }
}
