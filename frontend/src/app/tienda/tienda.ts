import { CurrencyPipe } from '@angular/common';
import { httpResource } from '@angular/common/http';
import {
  Component,
  DestroyRef,
  ElementRef,
  afterRenderEffect,
  computed,
  inject,
  signal,
  viewChild,
  viewChildren,
} from '@angular/core';
import { CATALOGO_URL } from '../catalogo/catalogo.config';
import { Catalogo, Categoria, Producto } from '../catalogo/catalogo.model';

interface Seccion {
  id: string;
  nombre: string;
}

const COLORES_BANDERIN = ['--azul', '--amarillo', '--rosa', '--turquesa', '--naranja', '--lila', '--cielo'];

/** Cada categoría toma un par de colores de la paleta, en rotación. */
const TEMAS = [
  { fuerte: 'var(--azul)', suave: 'var(--cielo)' },
  { fuerte: 'var(--marino)', suave: 'var(--rosa)' },
  { fuerte: 'var(--marino)', suave: 'var(--turquesa)' },
  { fuerte: 'var(--marino)', suave: 'var(--amarillo)' },
];

const SECCIONES_FIJAS: Seccion[] = [
  { id: 'como-pedir', nombre: 'Cómo pedir' },
  { id: 'envios', nombre: 'Envíos' },
];

@Component({
  selector: 'app-tienda',
  imports: [CurrencyPipe],
  templateUrl: './tienda.html',
  styleUrl: './tienda.scss',
})
export class Tienda {
  private readonly url = inject(CATALOGO_URL);
  protected readonly catalogo = httpResource<Catalogo>(() => this.url);

  protected readonly categorias = computed(() => {
    if (!this.catalogo.hasValue()) return [];
    const { categorias, productos } = this.catalogo.value();
    return categorias.map((c, i) => ({
      ...c,
      tema: TEMAS[i % TEMAS.length],
      productos: productos.filter((p) => p.categoriaSlug === c.slug),
    }));
  });

  protected readonly secciones = computed<Seccion[]>(() => [
    ...this.categorias().map((c: Categoria) => ({ id: c.slug, nombre: c.nombre })),
    ...SECCIONES_FIJAS,
  ]);

  /** Hasta tres fotos de productos distintos para la portada. */
  protected readonly fotosPortada = computed(() =>
    this.catalogo.hasValue()
      ? this.catalogo.value().productos.flatMap((p) => p.imagenes.slice(0, 1)).slice(0, 3)
      : [],
  );

  protected readonly activa = signal<string | null>(null);

  protected readonly banderines = Array.from({ length: 14 }, (_, i) => ({
    x: 18 + i * 52,
    color: `var(${COLORES_BANDERIN[i % COLORES_BANDERIN.length]})`,
  }));

  private readonly elementosSeccion = viewChildren<ElementRef<HTMLElement>>('seccion');
  private readonly navegacion = viewChild<ElementRef<HTMLElement>>('navegacion');

  constructor() {
    let observador: IntersectionObserver | undefined;
    let hashPendiente = location.hash.slice(1);
    inject(DestroyRef).onDestroy(() => observador?.disconnect());

    // Marca como activa la sección que cruza la franja superior de la pantalla.
    afterRenderEffect(() => {
      const elementos = this.elementosSeccion().map((e) => e.nativeElement);
      observador?.disconnect();
      if (typeof IntersectionObserver === 'undefined') return;
      observador = new IntersectionObserver(
        (entradas) => {
          for (const e of entradas) {
            if (e.isIntersecting) this.activa.set(e.target.id);
            else if (this.activa() === e.target.id && e.boundingClientRect.top > 0) this.activa.set(null);
          }
        },
        { rootMargin: '-35% 0px -60% 0px' },
      );
      elementos.forEach((el) => observador!.observe(el));

      // Si se entró con #seccion, la sección recién existe cuando llega el catálogo.
      const destino = hashPendiente && elementos.find((el) => el.id === hashPendiente);
      if (destino) {
        hashPendiente = '';
        destino.scrollIntoView({ behavior: 'instant' });
      }
    });

    // En celular la barra se desliza de costado: mantiene visible el enlace activo.
    afterRenderEffect(() => {
      const id = this.activa();
      const nav = this.navegacion()?.nativeElement;
      const enlace = id && nav?.querySelector<HTMLElement>(`a[href="#${id}"]`);
      if (!nav || !enlace || nav.scrollWidth <= nav.clientWidth) return;
      nav.scrollTo({ left: enlace.offsetLeft - (nav.clientWidth - enlace.offsetWidth) / 2, behavior: 'smooth' });
    });
  }

  protected precioDesde(p: Producto): number {
    return Math.min(...p.variantes.map((v) => v.precio));
  }

  protected medidas(p: Producto): string {
    const nombres = p.variantes.map((v) => v.nombre);
    return nombres.length > 1 ? `${nombres.slice(0, -1).join(', ')} o ${nombres.at(-1)}` : '';
  }
}
