using SimpleOps.Modelo;
using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Legal {


    /// <summary>
    /// Clase auxiliar para pasar información de una línea de un documento electrónico entre los procedimientos en 
    /// <see cref="DocumentoElectrónico{D, M}.Crear(out string)"/>. Solo usa un campo para impuesto que puede ser usado
    /// para IVA o para ImpuestoConsumo.
    /// </summary>
    class LíneaDocumentoElectrónico<M> where M : MovimientoProducto {


        #region Propiedades

        public decimal Cantidad { get; set; }

        /// <summary>
        /// Si es muestra gratis no es cero.
        /// </summary>
        public decimal SubtotalReal { get; set; }

        public decimal Impuesto { get; set; }

        public ModoImpuesto ModoImpuesto { get; set; }

        public TipoTributo TipoTributo { get; set; }

        #endregion Propiedades


        #region Constructores

        public LíneaDocumentoElectrónico() { }


        public LíneaDocumentoElectrónico(M línea, TipoTributo tipoTributo) {

            if (línea.Producto == null) throw new Exception("No se esperaba que el producto fuera nulo.");
            if (línea.IVA == null) throw new Exception("No se esperaba que el IVA fuera nulo.");
            if (línea.ImpuestoConsumo == null) throw new Exception("No se esperaba que el impuesto al consumo fuera nulo.");

            (Cantidad, TipoTributo, SubtotalReal) = (línea.Cantidad, tipoTributo, línea.SubtotalBaseReal);
            Impuesto = línea.ObtenerValorImpuesto(tipoTributo);
            ModoImpuesto = línea.ObtenerModoImpuesto(tipoTributo);

        } // LíneaDocumentoElectrónico>


        #endregion  Constructores>

        public decimal TarifaImpuestoPorcentual => ObtenerTarifaImpuestoPorcentual(Impuesto, SubtotalReal);

        public decimal TarifaImpuestoUnitario => Impuesto / Cantidad;


        #region Propiedades Autocalculadas

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones


        public static List<LíneaDocumentoElectrónico<M>> CrearLista(List<M> líneas, TipoTributo tipoTributo) {

            var respuesta = new List<LíneaDocumentoElectrónico<M>>();
            foreach (var línea in líneas) {
                respuesta.Add(new LíneaDocumentoElectrónico<M>(línea, tipoTributo));
            }
            return respuesta;

        } // CrearLista>


        #endregion  Métodos y Funciones>


    } // LíneaDocumentoElectrónico>



} // SimpleOps.Legal>
