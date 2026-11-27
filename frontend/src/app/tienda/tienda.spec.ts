import { registerLocaleData } from '@angular/common';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import localeEsAr from '@angular/common/locales/es-AR';
import { LOCALE_ID } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { Tienda } from './tienda';

describe('Tienda', () => {
  beforeEach(() => {
    registerLocaleData(localeEsAr);
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: LOCALE_ID, useValue: 'es-AR' },
      ],
    });
  });

  function crear() {
    const fixture = TestBed.createComponent(Tienda);
    TestBed.tick();
    const req = TestBed.inject(HttpTestingController).expectOne('/api/catalogo');
    return { fixture, req };
  }

  it('muestra el menor precio y la entrega bonificada', async () => {
    const { fixture, req } = crear();
    req.flush({
      productos: [{
        id: 1, nombre: 'Pared', slug: 'pared', descripcion: '', categoria: 'C', diasElaboracion: 7,
        variantes: [{ id: 1, nombre: 'A3', precio: 18000 }, { id: 2, nombre: 'A4', precio: 12000 }],
        imagenes: [], opciones: [],
      }],
      entregas: [{ id: 1, nombre: 'Envío', tipo: 'Envio', costo: 5000, gratisDesde: 40000 }],
    });
    await fixture.whenStable();
    const texto = fixture.nativeElement.textContent;
    expect(texto).toMatch(/Desde\s+\$\s*12\.000/);
    expect(texto).toMatch(/gratis desde\s+\$\s*40\.000/);
  });

  it('avisa cuando el catálogo no carga', async () => {
    const { fixture, req } = crear();
    req.flush('', { status: 500, statusText: 'Error' });
    await fixture.whenStable();
    expect(fixture.nativeElement.querySelector('[role=alert]')).toBeTruthy();
  });
});
