import { registerLocaleData } from '@angular/common';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import localeEsAr from '@angular/common/locales/es-AR';
import { LOCALE_ID } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { Catalogo, Producto } from '../catalogo/catalogo.model';
import { Tienda } from './tienda';

const producto = (id: number, categoriaSlug: string, precios = [100]): Producto => ({
  id, nombre: `P${id}`, slug: `p${id}`, descripcion: '', categoria: categoriaSlug, categoriaSlug, diasElaboracion: 7,
  variantes: precios.map((precio, i) => ({ id: id * 10 + i, nombre: `V${i}`, precio })),
  imagenes: [], opciones: [],
});

const catalogo: Catalogo = {
  categorias: [
    { slug: 'calendarios', nombre: 'Calendarios', descripcion: 'De pared' },
    { slug: 'fiestas', nombre: 'Fiestas infantiles', descripcion: '' },
  ],
  productos: [producto(1, 'calendarios', [18000, 12000]), producto(2, 'fiestas')],
  entregas: [{ id: 1, nombre: 'Envío', tipo: 'Envio', costo: 5000, gratisDesde: 40000 }],
};

describe('Tienda', () => {
  beforeEach(() => {
    registerLocaleData(localeEsAr);
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), { provide: LOCALE_ID, useValue: 'es-AR' }],
    });
  });

  async function crear(respuesta: Catalogo | 'error' = catalogo) {
    const fixture = TestBed.createComponent(Tienda);
    TestBed.tick();
    const req = TestBed.inject(HttpTestingController).expectOne('/api/catalogo');
    if (respuesta === 'error') req.flush('', { status: 500, statusText: 'Error' });
    else req.flush(respuesta);
    await fixture.whenStable();
    return fixture.nativeElement as HTMLElement;
  }

  it('arma una sección por categoría con sus productos', async () => {
    const el = await crear();
    const titulos = (id: string) => [...el.querySelectorAll(`#${id} h3`)].map((h) => h.textContent);
    expect(titulos('calendarios')).toEqual(['P1']);
    expect(titulos('fiestas')).toEqual(['P2']);
  });

  it('enlaza cada sección desde la barra', async () => {
    const el = await crear();
    const enlaces = [...el.querySelectorAll('.menu a')].map((a) => a.getAttribute('href'));
    expect(enlaces).toEqual(['#calendarios', '#fiestas', '#como-pedir', '#envios']);
    for (const href of enlaces) expect(el.querySelector(href!)).toBeTruthy();
  });

  it('muestra el menor precio, las medidas y el envío bonificado', async () => {
    const texto = (await crear()).textContent!;
    expect(texto).toMatch(/Desde\s+\$\s*12\.000/);
    expect(texto).toContain('En V0 o V1');
    expect(texto).toMatch(/Gratis en compras desde\s+\$\s*40\.000/);
  });

  it('avisa cuando el catálogo no carga', async () => {
    const el = await crear('error');
    expect(el.querySelector('[role=alert]')?.textContent).toContain('No se pudieron cargar');
  });
});
