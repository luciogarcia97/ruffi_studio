import { CurrencyPipe } from '@angular/common';
import { httpResource } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { CATALOGO_URL } from '../catalogo/catalogo.config';
import { Catalogo, Producto } from '../catalogo/catalogo.model';

@Component({
  selector: 'app-tienda',
  imports: [CurrencyPipe],
  templateUrl: './tienda.html',
  styleUrl: './tienda.scss',
})
export class Tienda {
  private readonly url = inject(CATALOGO_URL);
  protected readonly catalogo = httpResource<Catalogo>(() => this.url);

  protected precioDesde(p: Producto): number {
    return Math.min(...p.variantes.map((v) => v.precio));
  }
}
