using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;
using SimpleOps.DocumentosGráficos;
using AutoMapper;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Ofrecimiento de precios de venta de varios productos a un cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)] // Desde la lógica de la operación no hay problema en realizar dos cotizaciones al mismo tiempo al mismo cliente desde dos computadores diferentes, aunque no sería deseable que se hicieran con precios diferentes la concurrencia no es tanto problema.
    class Cotización : Registro {


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        public List<LíneaCotización> Líneas { get; set; } = new List<LíneaCotización>();

        #endregion Propiedades>


        #region Constructores

        private Cotización() { } // Solo para que EF Core no saque error.

        public Cotización(Cliente cliente) => (ClienteID, Cliente) = (cliente.ID, cliente);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ID} a {ATexto(Cliente, ClienteID)}";


        public DatosCotización ObtenerDatos(OpcionesDocumento opcionesDocumento) {

            var mapeador = new Mapper(ConfiguraciónMapeadorVenta);
            var datos = mapeador.Map<DatosCotización>(this);
            datos.NombreDocumento = "Cotización";
            CompletarDatosCotización(opcionesDocumento, datos, Líneas);
            return datos;

        } // ObtenerDatos>


        #endregion Métodos y Funciones>


    } // Cotización>



} // SimpleOps.Modelo>
