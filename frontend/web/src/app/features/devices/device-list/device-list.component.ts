import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DeviceService } from '@core/services/device.service';
import { Device } from '@core/models/device.model';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="device-list-container">
      <div class="header">
        <h1>My Devices</h1>
        <a routerLink="/devices/register" class="btn-primary">Register New Device</a>
      </div>

      <div class="devices-grid" *ngIf="devices.length > 0; else noDevices">
        <div class="device-card" *ngFor="let device of devices">
          <div class="device-header">
            <h3>{{ device.brand }} {{ device.model }}</h3>
            <span class="status-badge" [class]="device.status.toLowerCase()">
              {{ device.status }}
            </span>
          </div>

          <div class="device-details">
            <p><strong>Category:</strong> {{ device.category }}</p>
            <p><strong>Serial Number:</strong> {{ device.serialNumber }}</p>
            <p><strong>Registered:</strong> {{ device.registeredAt | date:'short' }}</p>
            <p *ngIf="device.description"><strong>Description:</strong> {{ device.description }}</p>
          </div>

          <div class="device-actions">
            <a [routerLink]="['/devices', device.id]" class="btn-view">View Details</a>
            <button (click)="deleteDevice(device.id)" class="btn-delete">Delete</button>
          </div>
        </div>
      </div>

      <ng-template #noDevices>
        <div class="no-devices">
          <p>You haven't registered any devices yet.</p>
          <a routerLink="/devices/register" class="btn-primary">Register Your First Device</a>
        </div>
      </ng-template>
    </div>
  `,
  styles: [`
    .device-list-container {
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

    .btn-primary {
      padding: 0.75rem 1.5rem;
      background-color: #1976d2;
      color: white;
      text-decoration: none;
      border-radius: 4px;
      transition: background-color 0.2s;
    }

    .btn-primary:hover {
      background-color: #1565c0;
    }

    .devices-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 1.5rem;
    }

    .device-card {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .device-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 1rem;
    }

    .device-header h3 {
      margin: 0;
      color: #333;
    }

    .status-badge {
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.75rem;
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

    .status-badge.forsale {
      background-color: #ff9800;
      color: white;
    }

    .device-details {
      margin-bottom: 1rem;
    }

    .device-details p {
      margin: 0.5rem 0;
      color: #666;
      font-size: 0.875rem;
    }

    .device-actions {
      display: flex;
      gap: 0.5rem;
    }

    .btn-view {
      flex: 1;
      padding: 0.5rem;
      background-color: #1976d2;
      color: white;
      text-align: center;
      text-decoration: none;
      border-radius: 4px;
    }

    .btn-delete {
      flex: 1;
      padding: 0.5rem;
      background-color: #f44336;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
    }

    .no-devices {
      text-align: center;
      padding: 3rem;
      background: white;
      border-radius: 8px;
    }

    .no-devices p {
      color: #666;
      margin-bottom: 1.5rem;
    }
  `]
})
export class DeviceListComponent implements OnInit {
  devices: Device[] = [];

  constructor(private deviceService: DeviceService) {}

  ngOnInit(): void {
    this.loadDevices();
  }

  loadDevices(): void {
    this.deviceService.getUserDevices().subscribe({
      next: (devices) => {
        this.devices = devices;
      },
      error: (error) => {
        console.error('Failed to load devices', error);
      }
    });
  }

  deleteDevice(id: string): void {
    if (confirm('Are you sure you want to delete this device?')) {
      this.deviceService.deleteDevice(id).subscribe({
        next: () => {
          this.devices = this.devices.filter(d => d.id !== id);
        },
        error: (error) => {
          console.error('Failed to delete device', error);
          alert('Failed to delete device');
        }
      });
    }
  }
}
