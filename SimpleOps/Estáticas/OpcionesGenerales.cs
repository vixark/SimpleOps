// Copyright Notice:
//
// SimpleOps® is a free ERP software for small businesses and independents.
// Copyright© 2021 Vixark (vixark@outlook.com).
// For more information about SimpleOps®, see https://simpleops.net.
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero
// General Public License as published by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the
// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public
// License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not,
// see https://www.gnu.org/licenses.
//
// This License does not grant permission to use the trade names, trademarks, service marks, or product names
// of the Licensor, except as required for reasonable and customary use in describing the origin of the Work
// and reproducing the content of the NOTICE file.
//
// Removing or changing the above text is not allowed.
//

using SimpleOps.Modelo;
using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Singleton {



    /// <summary>
    /// Configuraciones generales modificables por la empresa usuaria de SimpleOps. Inician en valores predeterminados que ninguna empresa tendría que 
    /// modificar para los escenarios más comunes y/o que sus valores pueden cambiar de manera global para todas las empresas de Colombia. Suelen ser 
    /// valores válidos para todas las empresas en Colombia. Si la empresa no modifica el valor de alguna propiedad (las modificadas se escriben en 
    /// GeneralesPropias.json), se usará el valor en Generales.json. Las Generales.json tienen la ventaja de ser actualizadas automáticamente cada año 
    /// o cada que hay un cambio externo, por ejemplo en la legislación. Sus valores siempre se cargan al iniciar desde Generales.json. Los valores 
    /// iniciales en código solo sirven para autogenerar el archivo Generales.json cuando no exista y para evitar tener que declarar las propiedades 
    /// permitiendo valores nulos. Su modificación está restringida por roles. En términos generales pocas configuraciones van aquí, ante de agregar 
    /// una configuración asegurarse de que se cumplen las condiciones, de lo contrario lo más normal es que vaya en OpcionesEmpresa.cs.
    /// </summary>
    sealed class OpcionesGenerales { // No cambiar los nombres de las propiedades porque estos se usan en los archivos de opciones JSON. Si alguna propiedad pudiera tener valores diferentes para diferentes usuarios/equipos de la empresa se debe usar OpcionesEquipo. No debe tener métodos ni propiedades autocalculadas (estos van en Global). Se usa el término Opciones y no Configuración u otros porque es el término usado por Visual Studio y Excel. 


        #region Patrón Singleton
        // Ver https://csharpindepth.com/Articles/Singleton.

        private static readonly Lazy<OpcionesGenerales> DatosLazy = new Lazy<OpcionesGenerales>(() => new OpcionesGenerales());

        public static OpcionesGenerales Datos { get { return DatosLazy.Value; } } // Normalmente esta sería la variable que se accede pero se prefiere hacer una variable auxiliar Generales en Global para tener un acceso más fácil sin necesidad de escribir OpcionesGenerales.Datos.

        public OpcionesGenerales() { } // Es necesario que sea public en .NET 7 para la serialización.

        #endregion Patrón Singleton>


        #region Variables Comportamiento

        public Dictionary<TipoImpuestoConsumo, double> PorcentajesImpuestosConsumo { get; set; } = new Dictionary<TipoImpuestoConsumo, double> { // Tomados de https://www.gerencie.com/que-es-el-impuesto-al-consumo.html.
            { TipoImpuestoConsumo.VehículosLujo, 0.16 },
            { TipoImpuestoConsumo.Aeronaves, 0.16 },
            { TipoImpuestoConsumo.Vehículos, 0.08 },
            { TipoImpuestoConsumo.MotocicletasLujo, 0.08 },
            { TipoImpuestoConsumo.Embarcaciones, 0.08 },
            { TipoImpuestoConsumo.ServiciosRestaurante, 0.08 },
            { TipoImpuestoConsumo.TelefoníaCelularYDatos, 0.04 },
        };

        public Dictionary<TipoImpuestoConsumo, decimal> ValoresUnitariosImpuestosConsumo { get; set; } = new Dictionary<TipoImpuestoConsumo, decimal> { // Tomados de https://www.gerencie.com/que-es-el-impuesto-al-consumo.html.
            { TipoImpuestoConsumo.BolsasPlásticas, 50 }, // Ver https://www.gerencie.com/impuesto-al-consumo-de-bolsas-plasticas.html.
        };

        #endregion Variables Comportamiento>


        #region Variables Legales
        // Siempre deben estar antes que Datos Predeterminados porque se usan en ellos.

        public string Moneda { get; set; } = "COP"; // COP para Peso Colombiano. Divisa aplicable a todas las facturas. Ver lista de valores posibles en el numeral 13.3.3 en la documentación de la facturación electrónica de la DIAN. Algunos valores: COP, USD, EUR, CNY, MXN, BRL, XAU, XAG, etc. Como no se implementa el elemento PaymentExchangeRate en la facturación electrónica este valor solo puede ser COP.

        public decimal MínimoUVTRetenciónIVAProductosLegal { get; set; } = 27; // Ver https://www.gerencie.com/retencion-en-la-fuente-por-iva-reteiva.html.

        public decimal MínimoUVTRetenciónIVAServiciosLegal { get; set; } = 4; // Ver https://www.gerencie.com/retencion-en-la-fuente-por-iva-reteiva.html.

        public double PorcentajeRetenciónIVALegal { get; set; } = 0.15; // Porcentaje legal de retención del IVA. Ver https://www.gerencie.com/retencion-en-la-fuente-por-iva-reteiva.html.

        public decimal UVT { get; set; } = 36308; // Ver https://www.gerencie.com/uvt-2021.html. 2020: 35607.

        public int HorasAjusteUtc { get; set; } = -5; // -5 para Colombia. Cantidad de horas fijas que se le restarán a la hora UTC para obtener la hora semilocal que se usará para almacenar las fechas de las operaciones en la base de datos. Es semilocal porque coincide con la hora local del equipo en los paises en los que no hay horario de verano ni zonas horarias. Para evitar referenciar a SimpleOps.exe desde Dian.dll este valor es escrito manualmente en Dian.sln.


        public List<RetenciónFuente> RetencionesFuente { get; set; } = new List<RetenciónFuente> { // Siempre que un elemento se especifique como declarante se deberá también especificar otra igual pero con NoDeclarante. Tomadas de https://www.gerencie.com/tabla-de-retencion-en-la-fuente-2020.html.
            new RetenciónFuente(ConceptoRetención.Generales, 27, 2.5, TipoDeclarante.Declarante),
            new RetenciónFuente(ConceptoRetención.Generales, 27, 3.5, TipoDeclarante.NoDeclarante),
            new RetenciónFuente(ConceptoRetención.TarjetaDébitoOCrédito, 0, 1.5, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.AgrícolasOPecuariosSinProcesamiento, 92, 1.5, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.AgrícolasOPecuaríosConProcesamiento, 27, 2.5, TipoDeclarante.Declarante),
            new RetenciónFuente(ConceptoRetención.AgrícolasOPecuaríosConProcesamiento, 27, 3.5, TipoDeclarante.NoDeclarante),
            new RetenciónFuente(ConceptoRetención.CaféPergaminoOCereza, 160, 0.5, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.CombustiblesDerivadosPetróleo, 0, 0.1, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.ActivosFijosPersonasNaturales, 0, 1, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.Vehículos, 0, 1, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.BienesRaícesVivienda, 20000, 2.5, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.BienesRaícesNoVivienda, 0, 2.5, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.ServiciosGenerales, 4, 4, TipoDeclarante.Declarante),
            new RetenciónFuente(ConceptoRetención.ServiciosGenerales, 4, 6, TipoDeclarante.NoDeclarante),
            new RetenciónFuente(ConceptoRetención.EmolumentosEclesiásticos, 27, 4, TipoDeclarante.Declarante),
            new RetenciónFuente(ConceptoRetención.EmolumentosEclesiásticos, 27, 3.5, TipoDeclarante.NoDeclarante),
            new RetenciónFuente(ConceptoRetención.TransporteCarga, 4, 1, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.TransporteNacionalTerrestrePasajeros, 27, 3.5, TipoDeclarante.Declarante),
            new RetenciónFuente(ConceptoRetención.TransporteNacionalTerrestrePasajeros, 27, 3.5, TipoDeclarante.NoDeclarante),
            new RetenciónFuente(ConceptoRetención.TransporteNacionalAéreoOMarítimoPasajeros, 4, 1, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.ServiciosPorEmpresasTemporales, 4, 1, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.ServiciosPorEmpresasVigilanciaYAseo, 4, 2, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.SaludPorIPS, 4, 2, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.HotelesYRestaurantes, 4, 3.5, TipoDeclarante.Declarante),
            new RetenciónFuente(ConceptoRetención.HotelesYRestaurantes, 4, 3.5, TipoDeclarante.NoDeclarante),
            new RetenciónFuente(ConceptoRetención.ArrendamientoBienesMuebles, 0, 4, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.ArrendamientoBienesInmuebles, 27, 3.5, TipoDeclarante.Declarante),
            new RetenciónFuente(ConceptoRetención.ArrendamientoBienesInmuebles, 27, 3.5, TipoDeclarante.NoDeclarante),
            new RetenciónFuente(ConceptoRetención.OtrosIngresosTributarios, 27, 2.5, TipoDeclarante.Declarante),
            new RetenciónFuente(ConceptoRetención.OtrosIngresosTributarios, 27, 3.5, TipoDeclarante.NoDeclarante),
            new RetenciónFuente(ConceptoRetención.HonorariosYComisiones, 0, 11, TipoDeclarante.Declarante),
            new RetenciónFuente(ConceptoRetención.HonorariosYComisiones, 0, 10, TipoDeclarante.NoDeclarante),
            new RetenciónFuente(ConceptoRetención.LicenciamientoSoftware, 0, 3.5, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.Intereses, 0, 7, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.RendimientosFinacierosRentaFija, 0, 4, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.LoteríasRifasYApuestas, 48, 20, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.ColocaciónIndependienteJuegosAzar, 5, 3, TipoDeclarante.Desconocido),
            new RetenciónFuente(ConceptoRetención.ContratosConstruccionYUrbanización, 0, 2, TipoDeclarante.Desconocido),
        };


        public static Dictionary<Banco, string> CódigosBancosBancolombia { get; set; } = new Dictionary<Banco, string> { // Ver https://www.satbancolombia.com/conversores/#!/bancos.
            { Banco.Bancamía, "1059" }, { Banco.Agrario, "1040" }, { Banco.AVVillas, "6013677" }, { Banco.CajaSocial, "5600829" },
            { Banco.Bancompartir, "1067" }, { Banco.Coopcentral, "1066" }, { Banco.Davivienda, "5895142" }, { Banco.Bogotá, "5600010" },
            { Banco.Occidente, "5600230" }, { Banco.Falabella, "1062" }, { Banco.Finandina, "1063" }, { Banco.GNBSudameris, "5600120" },
            { Banco.Multibank, "1064" }, { Banco.MundoMujer, "1047" }, { Banco.Pichincha, "1060" }, { Banco.Popular, "5600023" },
            { Banco.CredifinancieraProcredit, "1058" }, { Banco.SantanderDeNegocios, "1065" }, { Banco.Serfinanza, "1069" }, { Banco.W, "1053" },
            { Banco.Bancoldex, "1031" }, { Banco.Bancolombia, "5600078" }, { Banco.Bancoomeva, "1061" }, { Banco.BBVA, "5600133" },
            { Banco.Citibank, "5600094" }, { Banco.Coltefinanciera, "1370" }, { Banco.Confiar, "1292" }, { Banco.FinancieraDeAntioquia, "1283" },
            { Banco.Cotrafa, "1289" }, { Banco.Daviplata, "1551" }, { Banco.Juriscoop, "1121" }, { Banco.FinanciamientoItau, "1014" },
            { Banco.ItaúCorpbanca, "5600065" }, { Banco.Nequi, "1507" }, { Banco.ScotiabankColpatria, "5600191" }
        };

        #endregion Variables Legales>


        #region Datos Predeterminados
        // Los mínimos y porcentajes de retención de ICA y Extra se agregan aquí porque se consideran opciones generales que pueden aplicar para todas las empresas de una ciudad.

        public double PorcentajeRetenciónICAPredeterminado { get; set; } = 0; // Porcentaje de retención del ICA que aplican los clientes que tengan PorcentajeRetenciónICAPropio en nulo. Cero implica que no se aplica.

        public double PorcentajeRetencionesExtraPredeterminado { get; set; } = 0; // Porcentaje de retenciones extra que aplican los clientes que tengan PorcentajeRetencionesExtraPropio en nulo. Cero implica que no se aplica. 0 es el predeterminado porque no son retenciones comunes.

        public decimal MínimoRetenciónICAPredeterminado { get; set; } = 0; // Subtotal por encima del cual los clientes que tengan MínimoRetenciónICAPropio nulo aplican retención del ICA. Cero implica que aplica para todas las facturas.

        public decimal MínimoRetencionesExtraPredeterminado { get; set; } = 0; // Subtotal por encima del cual los clientes que tengan MínimoRetenciónExtraPropio nulo aplican retenciones extra. Cero implica que aplica para todas las facturas.

        public string PaísPredeterminado { get; set; } = "Colombia"; // País predeterminado en la creación de los municipios. Se agrega en generales porque en caso de adaptar a otro país, se podría usar otro archivo Generales.json.

        #endregion Datos Predeterminados>


    } // OpcionesGenerales>



} // SimpleOps.Singleton>
