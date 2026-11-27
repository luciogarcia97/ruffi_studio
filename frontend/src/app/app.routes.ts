import { Routes } from '@angular/router';
import { Tienda } from './tienda/tienda';

export const routes: Routes = [
  { path: '', component: Tienda, title: 'Ruffi Calendarios' },
  { path: 'admin', title: 'Panel | Ruffi', loadComponent: () => import('./admin/admin').then((m) => m.Admin) },
  { path: '**', redirectTo: '' },
];
