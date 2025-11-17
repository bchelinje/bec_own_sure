import { Routes } from '@angular/router';

export const THEFT_REPORT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./theft-report-list/theft-report-list.component').then(m => m.TheftReportListComponent)
  }
];
