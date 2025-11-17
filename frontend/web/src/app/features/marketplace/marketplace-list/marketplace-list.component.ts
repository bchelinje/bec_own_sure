import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MarketplaceService } from '@core/services/marketplace.service';
import { MarketplaceListing } from '@core/models/marketplace.model';

@Component({
  selector: 'app-marketplace-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="marketplace-container">
      <h1>Marketplace</h1>

      <div class="filters">
        <select [(ngModel)]="selectedCategory" (change)="applyFilters()">
          <option value="">All Categories</option>
          <option value="Smartphone">Smartphones</option>
          <option value="Laptop">Laptops</option>
          <option value="Tablet">Tablets</option>
          <option value="Camera">Cameras</option>
          <option value="Watch">Watches</option>
        </select>

        <select [(ngModel)]="selectedCondition" (change)="applyFilters()">
          <option value="">All Conditions</option>
          <option value="new">New</option>
          <option value="like_new">Like New</option>
          <option value="good">Good</option>
          <option value="fair">Fair</option>
        </select>

        <input
          type="number"
          [(ngModel)]="minPrice"
          placeholder="Min Price"
          (change)="applyFilters()"
        />

        <input
          type="number"
          [(ngModel)]="maxPrice"
          placeholder="Max Price"
          (change)="applyFilters()"
        />
      </div>

      <div class="listings-grid" *ngIf="listings.length > 0; else noListings">
        <div class="listing-card" *ngFor="let listing of listings">
          <div class="listing-image">
            <img src="/assets/placeholder.jpg" alt="{{ listing.title }}" />
            <span class="price-badge">£{{ listing.price }}</span>
          </div>

          <div class="listing-info">
            <h3>{{ listing.title }}</h3>
            <p class="condition">{{ listing.condition }}</p>
            <p class="description">{{ listing.description | slice:0:100 }}...</p>
            <div class="listing-meta">
              <span>📍 {{ listing.location || 'Location not specified' }}</span>
              <span>👁 {{ listing.viewCount }} views</span>
            </div>
          </div>

          <div class="listing-actions">
            <a [routerLink]="['/marketplace', listing.id]" class="btn-view">View Details</a>
          </div>
        </div>
      </div>

      <ng-template #noListings>
        <div class="no-listings">
          <p>No listings found. Try adjusting your filters.</p>
        </div>
      </ng-template>
    </div>
  `,
  styles: [`
    .marketplace-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
    }

    h1 {
      color: #333;
      margin-bottom: 2rem;
    }

    .filters {
      display: flex;
      gap: 1rem;
      margin-bottom: 2rem;
      flex-wrap: wrap;
    }

    .filters select,
    .filters input {
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 1rem;
    }

    .listings-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 1.5rem;
    }

    .listing-card {
      background: white;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      transition: transform 0.2s;
    }

    .listing-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
    }

    .listing-image {
      position: relative;
      height: 200px;
      background-color: #f5f5f5;
      overflow: hidden;
    }

    .listing-image img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .price-badge {
      position: absolute;
      top: 1rem;
      right: 1rem;
      background-color: #1976d2;
      color: white;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      font-weight: bold;
    }

    .listing-info {
      padding: 1rem;
    }

    .listing-info h3 {
      margin: 0 0 0.5rem 0;
      color: #333;
    }

    .condition {
      display: inline-block;
      padding: 0.25rem 0.75rem;
      background-color: #4caf50;
      color: white;
      border-radius: 12px;
      font-size: 0.75rem;
      margin-bottom: 0.5rem;
    }

    .description {
      color: #666;
      font-size: 0.875rem;
      margin: 0.5rem 0;
    }

    .listing-meta {
      display: flex;
      justify-content: space-between;
      font-size: 0.875rem;
      color: #999;
      margin-top: 0.5rem;
    }

    .listing-actions {
      padding: 1rem;
      border-top: 1px solid #f0f0f0;
    }

    .btn-view {
      display: block;
      width: 100%;
      padding: 0.75rem;
      background-color: #1976d2;
      color: white;
      text-align: center;
      text-decoration: none;
      border-radius: 4px;
      transition: background-color 0.2s;
    }

    .btn-view:hover {
      background-color: #1565c0;
    }

    .no-listings {
      text-align: center;
      padding: 3rem;
      background: white;
      border-radius: 8px;
    }
  `]
})
export class MarketplaceListComponent implements OnInit {
  listings: MarketplaceListing[] = [];
  selectedCategory = '';
  selectedCondition = '';
  minPrice?: number;
  maxPrice?: number;

  constructor(private marketplaceService: MarketplaceService) {}

  ngOnInit(): void {
    this.loadListings();
  }

  loadListings(): void {
    this.marketplaceService.getListings(
      this.selectedCategory || undefined,
      this.minPrice,
      this.maxPrice,
      this.selectedCondition || undefined
    ).subscribe({
      next: (listings) => {
        this.listings = listings;
      },
      error: (error) => {
        console.error('Failed to load listings', error);
      }
    });
  }

  applyFilters(): void {
    this.loadListings();
  }
}
