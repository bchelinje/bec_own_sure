import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DeviceService } from '@core/services/device.service';
import { Device } from '@core/models/device.model';

@Component({
  selector: 'app-device-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="detail-container" *ngIf="device">
      <div class="header">
        <h1>{{ device.brand }} {{ device.model }}</h1>
        <span class="status-badge" [class]="device.status.toLowerCase()">
          {{ device.status }}
        </span>
      </div>

      <div class="detail-grid">
        <div class="info-card">
          <h3>Device Information</h3>
          <div class="info-row">
            <span class="label">Category:</span>
            <span>{{ device.category }}</span>
          </div>
          <div class="info-row">
            <span class="label">Serial Number:</span>
            <span>{{ device.serialNumber }}</span>
          </div>
          <div class="info-row">
            <span class="label">Verification Code:</span>
            <span class="code">{{ device.verificationCode }}</span>
          </div>
          <div class="info-row">
            <span class="label">Verified:</span>
            <span>{{ device.isVerified ? 'Yes' : 'No' }}</span>
          </div>
          <div class="info-row">
            <span class="label">Registered:</span>
            <span>{{ device.registeredAt | date:'medium' }}</span>
          </div>
          <div class="info-row" *ngIf="device.description">
            <span class="label">Description:</span>
            <span>{{ device.description }}</span>
          </div>
        </div>

        <div class="actions-card">
          <h3>Actions</h3>
          <button class="btn-action">Report as Stolen</button>
          <button class="btn-action">Create Marketplace Listing</button>
          <button class="btn-action">Upload Photo</button>
          <button class="btn-action">Upload Document</button>
          <button class="btn-danger" (click)="deleteDevice()">Delete Device</button>
        </div>
      </div>

      <div class="back-link">
        <a routerLink="/devices">← Back to Devices</a>
      </div>
    </div>
  `,
  styles: [`
    .detail-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 2rem;
    }

    .header h1 {
      color: #333;
      margin: 0;
    }

    .status-badge {
      padding: 0.5rem 1rem;
      border-radius: 12px;
      font-size: 0.875rem;
      font-weight: 500;
    }

    .status-badge.active {
      background-color: #4caf50;
      color: white;
    }

    .status-badge.stolen {
      background-color: #f44336;
      color: white;
    }

    .detail-grid {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 2rem;
      margin-bottom: 2rem;
    }

    .info-card, .actions-card {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    h3 {
      margin-top: 0;
      color: #333;
      border-bottom: 2px solid #1976d2;
      padding-bottom: 0.5rem;
    }

    .info-row {
      display: grid;
      grid-template-columns: 150px 1fr;
      padding: 0.75rem 0;
      border-bottom: 1px solid #f0f0f0;
    }

    .info-row:last-child {
      border-bottom: none;
    }

    .label {
      font-weight: 500;
      color: #666;
    }

    .code {
      font-family: monospace;
      background-color: #f5f5f5;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
    }

    .actions-card {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .btn-action, .btn-danger {
      padding: 0.75rem;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 1rem;
      transition: background-color 0.2s;
    }

    .btn-action {
      background-color: #1976d2;
      color: white;
    }

    .btn-action:hover {
      background-color: #1565c0;
    }

    .btn-danger {
      background-color: #f44336;
      color: white;
    }

    .btn-danger:hover {
      background-color: #d32f2f;
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
export class DeviceDetailComponent implements OnInit {
  device: Device | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadDevice(id);
    }
  }

  loadDevice(id: string): void {
    this.deviceService.getDevice(id).subscribe({
      next: (device) => {
        this.device = device;
      },
      error: (error) => {
        console.error('Failed to load device', error);
        this.router.navigate(['/devices']);
      }
    });
  }

  deleteDevice(): void {
    if (!this.device) return;

    if (confirm('Are you sure you want to delete this device?')) {
      this.deviceService.deleteDevice(this.device.id).subscribe({
        next: () => {
          this.router.navigate(['/devices']);
        },
        error: (error) => {
          console.error('Failed to delete device', error);
          alert('Failed to delete device');
        }
      });
    }
  }
}
