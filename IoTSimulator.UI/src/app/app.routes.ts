import { Routes } from '@angular/router';
import { MainLayoutComponent } from './shared/layout/main-layout/main-layout.component';
import { DashboardComponent } from './features/dashboard/dashboard/dashboard.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: '/dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        component: DashboardComponent
      },
      {
        path: 'houses',
        loadComponent: () => import('./features/houses/houses-list/houses-list.component').then(c => c.HousesListComponent)
      },
      {
        path: 'rooms',
        loadComponent: () => import('./features/rooms/rooms-list/rooms-list.component').then(c => c.RoomsListComponent)
      },
      {
        path: 'devices',
        loadComponent: () => import('./features/devices/devices-list/devices-list.component').then(c => c.DevicesListComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
