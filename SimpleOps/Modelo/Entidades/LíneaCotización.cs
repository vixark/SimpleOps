using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Precio dado a cliente. Se diferencia de PrecioCliente en que la tabla Cotizaciones es un registro histórico mientras que la tabla PreciosClientes tiene solo los últimos precios.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)] 
    class LíneaCotización { // No necesitan ser rastreadas porque son entidades de tipo Línea[EntidadPadre] que se crean en una sola operación con la EntidadPadre (Cotización) entonces su información de creación es la misma. Además, no admite más cambios después de creada por lo que no hay necesidad de crear propiedades para la información de actualización. No se admite actualización ni anulación de una cotización, solo admite recotización que se hace con una cotización nueva.


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } 

        public decimal Precio { get; set; } // Obligatorio.

        public Cotización? Cotización { get; set; } // Obligatorio.
        public int CotizaciónID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaCotización() { } // Solo para que EF Core no saque error.

        public LíneaCotización(Cotización cotización, Producto producto, decimal precio) 
            => (Cotización, CotizaciónID, ProductoID, Producto, Precio) = (cotización, cotización.ID, producto.ID, producto, precio);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Cotización, CotizaciónID)} de {ATexto(Producto, ProductoID)}";

        #endregion Métodos y Funciones>


    } // LíneaCotización>



} // SimpleOps.Modelo>
