import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MarketplaceService } from '@core/services/marketplace.service';
import { MarketplaceListing } from '@core/models/marketplace.model';

@Component({
  selector: 'app-marketplace-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="detail-container" *ngIf="listing">
      <div class="detail-grid">
        <div class="image-section">
          <img src="/assets/placeholder.jpg" alt="{{ listing.title }}" />
        </div>

        <div class="info-section">
          <h1>{{ listing.title }}</h1>
          <div class="price">£{{ listing.price }}</div>
          <span class="condition-badge">{{ listing.condition }}</span>

          <div class="details">
            <h3>Description</h3>
            <p>{{ listing.description }}</p>

            <h3>Details</h3>
            <div class="detail-row">
              <span>Category:</span>
              <span>{{ listing.category }}</span>
            </div>
            <div class="detail-row">
              <span>Condition:</span>
              <span>{{ listing.condition }}</span>
            </div>
            <div class="detail-row">
              <span>Location:</span>
              <span>{{ listing.location }}</span>
            </div>
            <div class="detail-row">
              <span>Shipping:</span>
              <span>{{ listing.isShippingAvailable ? 'Available' : 'Not Available' }}</span>
            </div>
            <div class="detail-row">
              <span>Views:</span>
              <span>{{ listing.viewCount }}</span>
            </div>
            <div class="detail-row">
              <span>Listed:</span>
              <span>{{ listing.listedAt | date:'medium' }}</span>
            </div>
          </div>

          <div class="actions">
            <button class="btn-primary">Contact Seller</button>
            <button class="btn-secondary">Add to Wishlist</button>
          </div>
        </div>
      </div>

      <div class="back-link">
        <a routerLink="/marketplace">← Back to Marketplace</a>
      </div>
    </div>
  `,
  styles: [`
    .detail-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
    }

    .detail-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 2rem;
      background: white;
      padding: 2rem;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      margin-bottom: 2rem;
    }

    .image-section img {
      width: 100%;
      border-radius: 8px;
    }

    .info-section h1 {
      margin-top: 0;
      color: #333;
    }

    .price {
      font-size: 2rem;
      font-weight: bold;
      color: #1976d2;
      margin: 1rem 0;
    }

    .condition-badge {
      display: inline-block;
      padding: 0.5rem 1rem;
      background-color: #4caf50;
      color: white;
      border-radius: 4px;
      font-size: 0.875rem;
    }

    .details {
      margin: 2rem 0;
    }

    .details h3 {
      color: #333;
      margin: 1.5rem 0 1rem 0;
    }

    .detail-row {
      display: grid;
      grid-template-columns: 150px 1fr;
      padding: 0.75rem 0;
      border-bottom: 1px solid #f0f0f0;
    }

    .detail-row span:first-child {
      font-weight: 500;
      color: #666;
    }

    .actions {
      display: flex;
      gap: 1rem;
    }

    .btn-primary, .btn-secondary {
      flex: 1;
      padding: 1rem;
      border: none;
      border-radius: 4px;
      font-size: 1rem;
      cursor: pointer;
      transition: background-color 0.2s;
    }

    .btn-primary {
      background-color: #1976d2;
      color: white;
    }

    .btn-primary:hover {
      background-color: #1565c0;
    }

    .btn-secondary {
      background-color: #f5f5f5;
      color: #333;
    }

    .btn-secondary:hover {
      background-color: #e0e0e0;
    }

    .back-link {
      margin-top: 2rem;
    }

    .back-link a {
      color: #1976d2;
      text-decoration: none;
      font-weight: 500;
    }
  `]
})
export class MarketplaceDetailComponent implements OnInit {
  listing: MarketplaceListing | null = null;

  constructor(
    private route: ActivatedRoute,
    private marketplaceService: MarketplaceService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadListing(id);
    }
  }

  loadListing(id: string): void {
    this.marketplaceService.getListing(id).subscribe({
      next: (listing) => {
        this.listing = listing;
      },
      error: (error) => {
        console.error('Failed to load listing', error);
      }
    });
  }
}
