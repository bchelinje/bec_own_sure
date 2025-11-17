import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DeviceService } from '@core/services/device.service';
import { AuthService } from '@core/services/auth.service';
import { Device } from '@core/models/device.model';
import { User } from '@core/models/user.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="dashboard-container">
      <div class="dashboard-header">
        <h1>Welcome, {{ user?.firstName || user?.email }}!</h1>
        <p>Subscription: <strong>{{ user?.subscriptionTier }}</strong></p>
      </div>

      <div class="dashboard-grid">
        <div class="card">
          <h3>My Devices</h3>
          <div class="stat-number">{{ devices.length }}</div>
          <a routerLink="/devices" class="card-link">View All Devices →</a>
        </div>

        <div class="card">
          <h3>Active Devices</h3>
          <div class="stat-number">{{ activeDevicesCount }}</div>
          <a routerLink="/devices" class="card-link">Manage Devices →</a>
        </div>

        <div class="card">
          <h3>Marketplace</h3>
          <p>Buy or sell verified devices securely</p>
          <a routerLink="/marketplace" class="card-link">Browse Marketplace →</a>
        </div>

        <div class="card">
          <h3>Quick Actions</h3>
          <div class="quick-actions">
            <a routerLink="/devices/register" class="btn-action">Register Device</a>
            <a routerLink="/devices/check" class="btn-action">Check Serial Number</a>
            <a routerLink="/reports" class="btn-action">Report Theft</a>
          </div>
        </div>
      </div>

      <div class="recent-devices" *ngIf="devices.length > 0">
        <h2>Recent Devices</h2>
        <div class="device-list">
          <div class="device-item" *ngFor="let device of devices.slice(0, 5)">
            <div class="device-info">
              <h4>{{ device.brand }} {{ device.model }}</h4>
              <p>{{ device.category }} - {{ device.serialNumber }}</p>
              <span class="status-badge" [class]="device.status.toLowerCase()">
                {{ device.status }}
              </span>
            </div>
            <a [routerLink]="['/devices', device.id]" class="btn-view">View</a>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
    }

    .dashboard-header {
      margin-bottom: 2rem;
    }

    .dashboard-header h1 {
      color: #333;
      margin-bottom: 0.5rem;
    }

    .dashboard-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
      margin-bottom: 3rem;
    }

    .card {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .card h3 {
      margin-top: 0;
      color: #333;
    }

    .stat-number {
      font-size: 3rem;
      font-weight: bold;
      color: #1976d2;
      margin: 1rem 0;
    }

    .card-link {
      color: #1976d2;
      text-decoration: none;
      font-weight: 500;
    }

    .card-link:hover {
      text-decoration: underline;
    }

    .quick-actions {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .btn-action {
      padding: 0.75rem;
      background-color: #1976d2;
      color: white;
      text-align: center;
      text-decoration: none;
      border-radius: 4px;
      transition: background-color 0.2s;
    }

    .btn-action:hover {
      background-color: #1565c0;
    }

    .recent-devices h2 {
      color: #333;
      margin-bottom: 1rem;
    }

    .device-list {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .device-item {
      background: white;
      padding: 1rem;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .device-info h4 {
      margin: 0 0 0.5rem 0;
      color: #333;
    }

    .device-info p {
      margin: 0 0 0.5rem 0;
      color: #666;
      font-size: 0.875rem;
    }

    .status-badge {
      display: inline-block;
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

    .btn-view {
      padding: 0.5rem 1rem;
      background-color: #1976d2;
      color: white;
      text-decoration: none;
      border-radius: 4px;
    }

    .btn-view:hover {
      background-color: #1565c0;
    }
  `]
})
export class DashboardComponent implements OnInit {
  user: User | null = null;
  devices: Device[] = [];
  activeDevicesCount = 0;

  constructor(
    private authService: AuthService,
    private deviceService: DeviceService
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getCurrentUser();
    this.loadDevices();
  }

  loadDevices(): void {
    this.deviceService.getUserDevices().subscribe({
      next: (devices) => {
        this.devices = devices;
        this.activeDevicesCount = devices.filter(d => d.status === 'Active').length;
      },
      error: (error) => {
        console.error('Failed to load devices', error);
      }
    });
  }
}
