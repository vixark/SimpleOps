using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Elemento de la tabla ListasDePrecios donde se guardan las listas de precios que maneja la empresa para sus clientes y las condiciones para acceder a ellos.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)] // Se implementa control pesimista porque no hay manera fácil de crear una clave múltiple para controlar los duplicados en una inserción optimista. La dificultad se presenta por las propiedades que pueden ser nulas y que deberían ser parte de la clave múltiple.
    class PrecioLista : Precio {


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Desconocido = 0, Consumidor = 1, Distribuidor = 2, GrandesContratos = 3, Otro = 255.
        /// </summary>
        public TipoCliente TipoCliente { get; set; } = TipoCliente.Desconocido; // Obligatorio.

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string? SubtipoCliente { get; set; }

        /// <summary>
        /// Si se especifica y existen dos PrecioLista con el mismo (TipoCliente, SubtipoCliente, Producto) se elegirá el que tenga el mínimo MáximoDíasCrédito que sea mayor a DíasCrédito del cliente.
        /// </summary>
        public int? MáximoDíasCrédito { get; set; }

        /// <summary>
        /// Sirve para establecer precios diferenciados entre clientes que tienen representante comercial y clientes que no tienen representante comercial.
        /// </summary>
        public bool TieneRepresentanteComercial { get; set; }

        #endregion Propiedades>


        #region Constructores

        private PrecioLista() : base(null, 0) { } // Solo para que Entity Framework no saque error.

        public PrecioLista(Producto producto, TipoCliente tipoCliente, decimal valor) : base(producto, valor) => (TipoCliente) = (tipoCliente);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() =>
            $"de {ATexto(Producto, ProductoID)} a {TipoCliente}{(SubtipoCliente != null ? $" {SubtipoCliente}" : "")} " +
            $"{(TieneRepresentanteComercial ? "con" : "sin")} representante comercial" +
            $"{(MáximoDíasCrédito != null ? $" con máximo {MáximoDíasCrédito} días de crédito" : "")}.";

        #endregion Métodos y Funciones>


    } // PrecioLista>



} // SimpleOps.Modelo>
