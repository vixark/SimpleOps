using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    public class DatosEmpresa {


        #region Propiedades

        public string? Nombre { get; set; } 

        public string? TeléfonoPrincipal { get; set; }

        public string? EmailVentas { get; set; }

        public string? SitioWeb { get; set; }

        public string? MunicipioUbicaciónNombre { get; set; }

        public string? DirecciónUbicaciónEfectiva { get; set; }

        public string? RazónSocial { get; set; }

        public string? NitCompleto { get; set; }

        public string? TipoContribuyenteTexto { get; set; }

        public bool DetallarImpuestoSiPorcentajesDiferentes { get; set; }

        public bool MostrarUnidad { get; set; }

        public string? EnlaceWebADetalleProducto { get; set; }

        public int? PrimerNúmeroFacturaAutorizada { get; set; }

        public int? ÚltimoNúmeroFacturaAutorizada { get; set; }

        public decimal? NúmeroAutorizaciónFacturación { get; set; }

        public DateTime? FinAutorizaciónFacturación { get; set; }

        #endregion Propiedades>


    } // DatosEmpresa>



} // SimpleOps.DocumentosGráficos>
