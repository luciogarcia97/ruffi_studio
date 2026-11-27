# Ruffi Calendarios: plan base

Estado: etapa 1 terminada. Las tablas de etapas posteriores se agregan con sus migraciones.

## Decisiones tomadas

- Uso propio, venta en Argentina (ARS, zona horaria America/Argentina/Buenos_Aires).
- Backend C# / ASP.NET Core, frontend Angular, base de datos MySQL 8.
- Cloudflare plan gratuito con Pages conectado a GitHub, sin dominio propio (`*.pages.dev`).
- Por ahora la web es un catálogo, con la base lista para ecommerce.
- Retiro y envío. El envío puede tener costo y bonificarse desde cierto monto.
- Login con email y contraseña, y con Google.
- Pago por transferencia, extensible a otros medios.
- Sin factura. Los datos fiscales son opcionales y quedan previstos.
- Los archivos de personalización se manejan fuera de la web.

## Arquitectura

### Dos modos de funcionamiento

| | Modo catálogo | Modo ecommerce |
|---|---|---|
| Público | Angular en Cloudflare Pages | Igual |
| Datos | JSON publicado desde el panel | API en línea |
| Pedido | Mensaje de WhatsApp armado desde el carrito | Checkout, login y transferencia |
| API + MySQL | En la PC local | En un servidor |
| Panel | Local | En línea |

Angular consume el catálogo por una interfaz con dos implementaciones (JSON y API),
elegida por configuración. En modo catálogo no hay login, ni se guardan pedidos
o datos de clientes; el pedido se concreta por WhatsApp. Los precios del JSON son
informativos: en modo ecommerce el servidor siempre recalcula.

Publicar: el panel local exporta JSON e imágenes al proyecto, se sube a GitHub y
Pages despliega. Todo lo exportado es público.

En modo ecommerce, una Pages Function reenvía `/api/*` a la API, para que el
navegador vea un único origen y funcionen las cookies de sesión.

### Estructura

```text
backend/
  src/Ruffi.Api             endpoints, autenticación
  src/Ruffi.Application     casos de uso, interfaces
  src/Ruffi.Domain          entidades y reglas
  src/Ruffi.Infrastructure  EF Core, MySQL, pagos, mail
  tests/
frontend/                   tienda y panel, separados por rutas lazy
docs/
docker-compose.yml          MySQL para desarrollo
```

Versiones: .NET 10 (LTS), `MySql.EntityFrameworkCore` 10, Angular 22.

## Modelo de datos

| Área | Tablas |
|---|---|
| Identidad | `usuarios`, `roles`, `logins_externos` (ASP.NET Identity) |
| Clientes | `clientes`, `domicilios`, `datos_fiscales` |
| Catálogo | `categorias`, `productos`, `variantes`, `imagenes`, `opciones_personalizacion` |
| Carrito | `carritos`, `carrito_items` |
| Pedidos | `pedidos`, `pedido_items`, `pedido_item_opciones`, `pedido_historial` |
| Pagos | `medios_pago`, `pagos` |
| Envíos | `metodos_entrega` (tipo, costo, gratis desde) |
| Sistema | `configuracion`, `auditoria` |

Reglas:

- Importes en `DECIMAL(12,2)`; fechas en UTC.
- El pedido guarda copia de precios, nombres, domicilio, envío y datos fiscales.
- Los productos con ventas no se borran, se desactivan.
- Disponible para vender: activo y dentro de sus fechas de venta; cupo opcional.
- Los totales se calculan siempre en el servidor.

Tres estados independientes por pedido:

- Pago: pendiente, informado, confirmado, rechazado, devuelto.
- Producción: pendiente, esperando archivos, en producción, terminado.
- Entrega: pendiente, listo para retirar, enviado, entregado.

La cancelación se registra aparte, con fecha y motivo.

Envío: si el subtotal de productos alcanza `gratis_desde`, el costo es 0.
El retiro siempre cuesta 0.

## Etapas

1. **Base:** solución .NET, Angular, MySQL, migraciones, tests. Listo cuando
   Angular muestra productos de ejemplo desde la API.
2. **Panel de catálogo:** login de admin, productos, variantes e imágenes.
3. **Catálogo público:** tienda, carrito, WhatsApp, exportación y Pages.
   **Primera versión publicable.**
4. **Clientes:** registro, Google, domicilios y datos fiscales.
5. **Pedidos:** checkout, entrega, transferencia y mails.
6. **Gestión:** pedidos, producción, clientes, reportes y Excel.
7. **Ecommerce en línea:** servidor, backups y seguridad.
8. **Futuro:** Mercado Pago, factura electrónica, cotización de envíos, cupones.

## Pendiente de definir

- Número de WhatsApp y datos bancarios (se cargan por configuración, no en el repo).
- Costo de envío y monto de bonificación.
- Logo, colores y lista inicial de calendarios.
