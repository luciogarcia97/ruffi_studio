export interface Catalogo {
  productos: Producto[];
  entregas: MetodoEntrega[];
}

export interface Producto {
  id: number;
  nombre: string;
  slug: string;
  descripcion: string;
  categoria: string;
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
