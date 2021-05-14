using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.DocumentosGráficos {



    /// <summary>
    /// DTO de lectura. Adecuado solo para escribir desde Producto a DatosLíneaProducto. Si se requiere un DTO para escribir en ambas direcciones, se 
    /// puede usar <see cref="Integración.DatosLíneaProducto"/>.
    /// </summary>
    public class DatosLíneaProducto {


        #region Propiedades

        public int Cantidad { get; set; }

        public decimal PrecioBase { get; set; } // Se pasa de manera redundante con PrecioBaseTexto para dar opción al usuario del código de usar el valor que prefiera al diseñar el HTML. Puede elegir PrecioBase (numérico) para presentarlo como desee o PrecioBaseTexto con el formato predeterminado de moneda en la configuración de SimpleOps.

        public string? PrecioBaseTexto { get; set; }

        public string? IVATexto { get; set; }

        public string? ImpuestoConsumoTexto { get; set; }

        public string? SubtotalBaseTexto { get; set; }

        public string? SubtotalBaseConImpuestosTexto { get; set; }

        public string ProductoReferencia { get; set; } = null!; // Nunca es nulo, solo es para que no saque advertencia.

        /// <summary>
        /// Descripción del producto completa incluyendo los atributos. Para cambiarla, cambia <see cref="Modelo.Producto.DescripciónBase"/>
        /// y <see cref="Modelo.Producto.Atributos"/> en el <see cref="Modelo.Producto"/> de origen.
        /// </summary>
        public string? ProductoDescripción { get; private set; } // Se usa private set porque la propiedad Descripción de Producto es de solo lectura. Se necesita el set para poder ser escrito desde el Automapper, pero al ser privado no podrá ser escrito por el usuario del código una vez cargado. A diferencia de SimpleOps.Integración.DatosLíneaProducto este DTO no requiere escribir de vuelta los datos en el objeto original.

        public string? ProductoDescripciónBase { get; set; }

        public List<string> ProductoAtributos { get; private set; } = null!; // Nunca es nulo, solo es para que no saque advertencia. Se usa private set porque la colección no debería ser reemplazada por otra, solo sus ítems.

        public string? ProductoUnidadTexto { get; set; }

        public string? ProductoArchivoInformación { get; set; }

        public string? ProductoArchivoImagen { get; set; }

        public List<string> ProductoCaracterísticas { get; private set; } = null!; // Nunca es nulo, solo es para que no saque advertencia. Se usa private set porque la colección no debería ser reemplazada por otra, solo sus ítems.

        #endregion Propiedades>


    } // DatosLíneaProducto>



} // SimpleOps.DocumentosGráficos>
