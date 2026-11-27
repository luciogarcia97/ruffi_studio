import { InjectionToken } from '@angular/core';

/** Origen del catálogo: la API, o un JSON estático en modo catálogo (etapa 3). */
export const CATALOGO_URL = new InjectionToken<string>('CATALOGO_URL', {
  providedIn: 'root',
  factory: () => '/api/catalogo',
});
