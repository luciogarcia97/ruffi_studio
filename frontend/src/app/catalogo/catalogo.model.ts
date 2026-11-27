export interface Catalogo {
  categorias: Categoria[];
  productos: Producto[];
  entregas: MetodoEntrega[];
}

export interface Categoria {
  slug: string;
  nombre: string;
  descripcion: string;
}

export interface Producto {
  id: number;
  nombre: string;
  slug: string;
  descripcion: string;
  categoria: string;
  categoriaSlug: string;
  diasElaboracion: number;
  variantes: { id: number; nombre: string; precio: number }[];
  imagenes: { url: string; textoAlternativo: string }[];
  opciones: {
    id: number;
    nombre: string;
    ayuda: string;
    tipo: 'Texto' | 'ArchivosExternos';
    obligatoria: boolean;
    recargo: number;
  }[];
}

export interface MetodoEntrega {
  id: number;
  nombre: string;
  tipo: 'Retiro' | 'Envio';
  costo: number;
  gratisDesde: number | null;
}
