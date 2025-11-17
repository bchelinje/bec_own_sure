import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-theft-report-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="reports-container">
      <h1>Theft Reports</h1>
      <p>This feature is coming soon...</p>
    </div>
  `,
  styles: [`
    .reports-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
    }

    h1 {
      color: #333;
    }
  `]
})
export class TheftReportListComponent {}
