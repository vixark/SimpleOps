using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.DocumentosGráficos {



    public class DatosCotización : DatosDocumento, IConLíneasProductos {


        #region Propiedades

        #pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura. Se acepta porque es necesario asignarla en el método CopiarA().
        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.
        #pragma warning restore CA2227

        public OpcionesColumnas Columnas { get; set; } = new OpcionesColumnas();

        public string? CondicionesComerciales { get; set; }

        public string? EnlaceDescargaXlsx { get; set; }

        public List<string> ReferenciasProductosPáginasExtra { get; private set; } = new List<string>();

        public Dictionary<string, DatosProducto> DatosProductos { get; private set; } = new Dictionary<string, DatosProducto>(); // Contiene la información que se escribe en el catálogo o cotización de todos los productos base y productos en Líneas. Es útil principalmente para acceder a la información de los productos base con su detalle de precios por producto específico.

        public int? CantidadFilasProductos { get; set; } 

        public int? CantidadColumnasProductos { get; set; }  

        public string? TítuloPáginasExtra { get; set; } 

        #endregion Propiedades>


        #region Constructores

        public DatosCotización() => (NombreDocumento) = ("Cotización");

        #endregion Constructores>


        #region Métodos y Funciones


        public decimal? ObtenerPrecio(string referencia) {

            if (Líneas == null || Líneas.Count == 0) return null;
            var línea = Líneas.FirstOrDefault(l => l.ProductoReferencia.IgualA(referencia));
            if (línea == null || línea.PrecioBase <= 0) return null;
            return línea.PrecioBase;

        } // ObtenerPrecio>


        #endregion Métodos y Funciones>


    } // DatosCotización>



} // SimpleOps.DocumentosGráficos>
