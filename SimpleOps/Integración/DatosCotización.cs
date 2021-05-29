using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Integración {



    class DatosCotización : DatosDocumento {


        #region Propiedades

        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.

        public int TamañoImágenes { get; set; }

        public string? CondicionesComerciales { get; set; }

        public string? EnlaceDescargaXlsx { get; set; }

        public List<string> ReferenciasProductosPáginasExtra { get; set; } = new List<string>(); // No se puede usar private set; porque el automapper requiere un set público para funcionar correctamente. Referencias de los productos que se añadirán a las páginas extra del catálogo. Estas referencias pueden ser de productos base o de productos específicos. Si se da el caso que un producto base tiene la misma referencia que uno específico, se prefiere el base. Se manejan por fuera de DatosLíneaProducto/Producto porque son datos que solo le corresponden a la generación del catálogo y no es necesario agregarlos a la tabla Productos para uso general.

        public int? CantidadFilasProductos { get; set; } 

        public int? CantidadColumnasProductos { get; set; } 

        public int? ÍndiceInversoInserciónPáginasExtra { get; set; }

        public string? ContactoNombre { get; set; }

        public string? ContactoTeléfono { get; set; }

        public string? ContactoEmail { get; set; } // Puede ser nulo porque el contacto puede ser nulo.

        #endregion Propiedades>


    } // DatosCotización>



} // SimpleOps.Integración>
