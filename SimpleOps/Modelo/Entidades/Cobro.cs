using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Registro de cobros realizados a clientes por facturas de venta vencidas.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)]
    class Cobro : Registro { // Es Registro porque no cambia una vez generada. No se necesita rastrear la información de actualización.


        #region Propiedades

        public Cliente? Cliente { get; set; }
        public int ClienteID { get; set; }

        /// <summary>
        /// Prefijo + número de las facuras de ventas que fueron cobradas.
        /// </summary>
        /// <MaxLength>1000</MaxLength>
        [MaxLength(1000)]
        public List<string> NúmerosFacturas { get; set; } = new List<string>(); // Es de fines informativos y de solo escritura una vez y lectura posterior, por lo tanto no se justifica la creación de otra tabla.

        /// <summary>
        /// Total de las facturas cobradas.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Los días de vencimiento de la factura de venta cobrada más antigua.
        /// </summary>
        public int MáximoDíasVencimiento { get; set; }

        /// <summary>
        /// Respuesta del cliente ante el cobro.
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)] 
        public string? Respuesta { get; set; }

        /// <summary>
        /// Desconocido = 0, Email = 1, Telefónico = 2, Personal = 3, AgenciaDeCobros = 4, Prejurídico = 5, Jurídico = 6, Otro = 255.
        /// </summary>
        public TipoCobro Tipo { get; set; } = TipoCobro.Desconocido;

        #endregion Propiedades>


        #region Constructores

        private Cobro() { } // Solo para que EF Core no saque error.

        public Cobro(Cliente cliente) {
            (ClienteID, Cliente) = (cliente.ID, cliente);
            NúmerosFacturas = new List<string>(); // Poner parámetro  List<Venta> ventas y pendiente hacer código genérico de cálculo de vencimiento de factura e implementar aquí la extracción de los valores requeridos por el cobro.
        } // Cobro>

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"del {FechaHoraCreación.ToShortDateString()} a {ATexto(Cliente, ClienteID)}";

        #endregion Métodos y Funciones>


    } // Cobro>



} // SimpleOps.Modelo>
