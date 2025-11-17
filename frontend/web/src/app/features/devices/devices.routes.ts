import { Routes } from '@angular/router';

export const DEVICE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./device-list/device-list.component').then(m => m.DeviceListComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./device-register/device-register.component').then(m => m.DeviceRegisterComponent)
  },
  {
    path: 'check',
    loadComponent: () => import('./device-check/device-check.component').then(m => m.DeviceCheckComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./device-detail/device-detail.component').then(m => m.DeviceDetailComponent)
  }
];
