using SimpleOps.Modelo;
using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    public class OpcionesColumnas {


        #region Propiedades Estáticas

        public static int AnchoTotalesFactura { get; set; } = 80; // 80 permite acomodar hasta 99 999 999. Afecta el ancho de las columnas con valores totales (Subtotal, IVA, Consumo y Total) y el ancho de la sección de totales de la factura. Se debe establecer al iniciar la aplicación. No se maneja cómo una propiedad del objeto porque no es usual que una empresa necesite cambiar este valor según la factura.

        #endregion Propiedades Estáticas>


        #region Propiedades

        public bool MostrarIVAYTotal { get; set; }

        public bool MostrarImpuestoConsumoYTotal { get; set; }

        public bool MostrarUnidad { get; set; }

        public string? EnlaceWebADetalleProducto { get; set; }

        public int RadioFilaNombresColumnas { get; set; } = 5;

        public int AnchoReferencia { get; set; } = 90; // 90 es el valor elegido para acomodar una referencia de 10 letras con un pequeño espacio a la derecha.

        public int AnchoCantidad { get; set; } = 60; // 60 es el tamaño mínimo para que quepa el título cantidad.

        public int AnchoPrecio { get; set; } = 70; // 70 permite acomodar hasta 9 999 999.

        public int AnchoSubtotal { get; set; } = AnchoTotalesFactura; // 80 permite acomodar hasta 99 999 999.

        public int MargenColumnasExtremos { get; set; } = 15; // Margen aplicado a las columnas que están a la inicio y final de la tabla para evitar que se vean muy al borde.

        public int MargenDerechoReferencia { get; set; } = 5; // Se agrega un pequeño margen a la derecha para evitar que cuando la referencia sea muy larga quede muy cerca a la descripción.

        public int AnchoLista { get; set; }

        public int RellenoVerticalLíneas { get; set; } = 8; // "Padding" vertical en cada celda .

        public int MargenInferiorNombres { get; set; } = 5; // Margen inferior debajo de la fila de nombres de columnas.

        #endregion Propiedades>


        #region Propiedades Autocalculadas>

        public int AnchoUnidad => MostrarUnidad ? 70 : 0; // Aunque cabe en 60 se le da un poco de espacio con 70 para que no quede muy cerca de cantidad.

        public int AnchoIVA => MostrarIVAYTotal ? AnchoTotalesFactura : 0; // 80 permite acomodar hasta 99 999 999.

        public int AnchoConsumo => MostrarImpuestoConsumoYTotal ? AnchoTotalesFactura : 0; // 80 permite acomodar hasta 99 999 999.

        public int AnchoTotal => MostrarIVAYTotal || MostrarImpuestoConsumoYTotal ? AnchoTotalesFactura : 0; // 80 permite acomodar hasta 99 999 999.

        public int AnchoDescripción => Math.Max(AnchoLista - AnchoReferencia - AnchoCantidad - AnchoUnidad - AnchoPrecio - AnchoSubtotal - AnchoIVA - AnchoConsumo
            - AnchoTotal - MargenColumnasExtremos * 2 - MargenDerechoReferencia - 16, 70); // El 16 es un valor experimental de ajuste fino para lograr que todas las columnas de ambas tablas queden del mismo ancho. Se limita a 70 el ancho mínimo de esta columna porque la fórmula podría dar valores negativos.

        #endregion Propiedades Autocalculadas>


    } // OpcionesColumnas>



} // SimpleOps.DocumentosGráficos>

