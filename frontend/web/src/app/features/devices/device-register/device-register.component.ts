import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DeviceService } from '@core/services/device.service';

@Component({
  selector: 'app-device-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="register-container">
      <h1>Register New Device</h1>

      <form [formGroup]="deviceForm" (ngSubmit)="onSubmit()">
        <div class="form-group">
          <label for="serialNumber">Serial Number *</label>
          <input
            type="text"
            id="serialNumber"
            formControlName="serialNumber"
            placeholder="Enter device serial number"
          />
        </div>

        <div class="form-group">
          <label for="category">Category *</label>
          <select id="category" formControlName="category">
            <option value="">Select category</option>
            <option value="Smartphone">Smartphone</option>
            <option value="Laptop">Laptop</option>
            <option value="Tablet">Tablet</option>
            <option value="Camera">Camera</option>
            <option value="Watch">Watch</option>
            <option value="Other">Other</option>
          </select>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="brand">Brand</label>
            <input
              type="text"
              id="brand"
              formControlName="brand"
              placeholder="e.g., Apple, Samsung"
            />
          </div>

          <div class="form-group">
            <label for="model">Model</label>
            <input
              type="text"
              id="model"
              formControlName="model"
              placeholder="e.g., iPhone 15 Pro"
            />
          </div>
        </div>

        <div class="form-group">
          <label for="description">Description</label>
          <textarea
            id="description"
            formControlName="description"
            rows="4"
            placeholder="Any additional details about your device"
          ></textarea>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="purchaseDate">Purchase Date</label>
            <input
              type="date"
              id="purchaseDate"
              formControlName="purchaseDate"
            />
          </div>

          <div class="form-group">
            <label for="purchasePrice">Purchase Price (£)</label>
            <input
              type="number"
              id="purchasePrice"
              formControlName="purchasePrice"
              placeholder="0.00"
            />
          </div>
        </div>

        <div class="form-group">
          <label for="retailer">Retailer</label>
          <input
            type="text"
            id="retailer"
            formControlName="retailer"
            placeholder="Where did you purchase it?"
          />
        </div>

        <div class="error-message" *ngIf="errorMessage">
          {{ errorMessage }}
        </div>

        <div class="form-actions">
          <button type="button" (click)="cancel()" class="btn-cancel">Cancel</button>
          <button type="submit" [disabled]="deviceForm.invalid || isLoading" class="btn-primary">
            {{ isLoading ? 'Registering...' : 'Register Device' }}
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .register-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 2rem;
    }

    h1 {
      color: #333;
      margin-bottom: 2rem;
    }

    form {
      background: white;
      padding: 2rem;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 1rem;
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    label {
      display: block;
      margin-bottom: 0.5rem;
      color: #555;
      font-weight: 500;
    }

    input, select, textarea {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 1rem;
      box-sizing: border-box;
      font-family: inherit;
    }

    textarea {
      resize: vertical;
    }

    .error-message {
      color: #f44336;
      margin-bottom: 1rem;
    }

    .form-actions {
      display: flex;
      gap: 1rem;
      justify-content: flex-end;
    }

    .btn-primary, .btn-cancel {
      padding: 0.75rem 1.5rem;
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

    .btn-primary:hover:not(:disabled) {
      background-color: #1565c0;
    }

    .btn-primary:disabled {
      background-color: #ccc;
      cursor: not-allowed;
    }

    .btn-cancel {
      background-color: #f5f5f5;
      color: #333;
    }

    .btn-cancel:hover {
      background-color: #e0e0e0;
    }
  `]
})
export class DeviceRegisterComponent {
  deviceForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private deviceService: DeviceService,
    private router: Router
  ) {
    this.deviceForm = this.fb.group({
      serialNumber: ['', Validators.required],
      category: ['', Validators.required],
      brand: [''],
      model: [''],
      description: [''],
      purchaseDate: [''],
      purchasePrice: [''],
      retailer: ['']
    });
  }

  onSubmit(): void {
    if (this.deviceForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      this.deviceService.registerDevice(this.deviceForm.value).subscribe({
        next: (response) => {
          this.router.navigate(['/devices', response.id]);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Failed to register device';
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/devices']);
  }
}
