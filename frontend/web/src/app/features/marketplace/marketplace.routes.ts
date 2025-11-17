import { Routes } from '@angular/router';

export const MARKETPLACE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./marketplace-list/marketplace-list.component').then(m => m.MarketplaceListComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./marketplace-detail/marketplace-detail.component').then(m => m.MarketplaceDetailComponent)
  }
];
