import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DeviceService } from '@core/services/device.service';

@Component({
  selector: 'app-device-check',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="check-container">
      <h1>Check Serial Number</h1>
      <p>Enter a device serial number to check if it has been reported as stolen</p>

      <div class="search-box">
        <input
          type="text"
          [(ngModel)]="serialNumber"
          placeholder="Enter serial number"
          (keyup.enter)="checkSerialNumber()"
        />
        <button (click)="checkSerialNumber()" [disabled]="!serialNumber || isLoading">
          {{ isLoading ? 'Checking...' : 'Check' }}
        </button>
      </div>

      <div class="result-box" *ngIf="result">
        <div class="result-safe" *ngIf="result.isRegistered && !result.isStolen">
          <h3>✓ Device is Registered</h3>
          <p>This device is registered in our system and has NOT been reported as stolen.</p>
          <p *ngIf="result.owner"><strong>Owner:</strong> {{ result.owner }}</p>
        </div>

        <div class="result-danger" *ngIf="result.isStolen">
          <h3>⚠ Warning: Stolen Device</h3>
          <p>This device has been reported as stolen. Do not purchase or use this device.</p>
          <p><strong>Report Date:</strong> {{ result.reportDate | date }}</p>
          <p><strong>Police Report:</strong> {{ result.policeReportNumber || 'N/A' }}</p>
        </div>

        <div class="result-neutral" *ngIf="!result.isRegistered">
          <h3>Device Not Found</h3>
          <p>This device is not registered in our system.</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .check-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 2rem;
    }

    h1 {
      color: #333;
      margin-bottom: 0.5rem;
    }

    p {
      color: #666;
      margin-bottom: 2rem;
    }

    .search-box {
      display: flex;
      gap: 1rem;
      margin-bottom: 2rem;
    }

    .search-box input {
      flex: 1;
      padding: 1rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 1rem;
    }

    .search-box button {
      padding: 1rem 2rem;
      background-color: #1976d2;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 1rem;
    }

    .search-box button:hover:not(:disabled) {
      background-color: #1565c0;
    }

    .search-box button:disabled {
      background-color: #ccc;
      cursor: not-allowed;
    }

    .result-box {
      padding: 2rem;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .result-safe {
      background-color: #e8f5e9;
      border-left: 4px solid #4caf50;
    }

    .result-danger {
      background-color: #ffebee;
      border-left: 4px solid #f44336;
    }

    .result-neutral {
      background-color: #f5f5f5;
      border-left: 4px solid #999;
    }

    .result-box h3 {
      margin-top: 0;
      color: #333;
    }

    .result-box p {
      margin: 0.5rem 0;
    }
  `]
})
export class DeviceCheckComponent {
  serialNumber = '';
  isLoading = false;
  result: any = null;

  constructor(private deviceService: DeviceService) {}

  checkSerialNumber(): void {
    if (!this.serialNumber) return;

    this.isLoading = true;
    this.result = null;

    this.deviceService.checkSerialNumber(this.serialNumber).subscribe({
      next: (result) => {
        this.isLoading = false;
        this.result = result;
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Failed to check serial number', error);
      }
    });
  }
}
