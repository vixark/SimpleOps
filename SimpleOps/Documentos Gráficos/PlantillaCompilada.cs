using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    public class PlantillaCompilada<T> where T : class { // Tomada de https://github.com/adoconnection/RazorEngineCore/wiki/@Include-and-@Layout.


        #region Propiedades

        private readonly IRazorEngineCompiledTemplate<PlantillaBase<T>> Cuerpo;

        private readonly Dictionary<string, IRazorEngineCompiledTemplate<PlantillaBase<T>>> Partes;

        #endregion Propiedades>


        #region Constructores

        public PlantillaCompilada(IRazorEngineCompiledTemplate<PlantillaBase<T>> cuerpo, 
            Dictionary<string, IRazorEngineCompiledTemplate<PlantillaBase<T>>> partes) => (Cuerpo, Partes) = (cuerpo, partes);

        #endregion Constructores>


        #region Métodos y Funciones

        public string ObtenerHtml(T modelo) => ObtenerHtml(Cuerpo, modelo);


        public string ObtenerHtml(IRazorEngineCompiledTemplate<PlantillaBase<T>> plantillaCompilada, T modelo) {

            if (plantillaCompilada == null) return "";

            var plantillaReferencia = default(PlantillaBase<T>);
  
            var htmlCuerpo = plantillaCompilada.Run(p => { // Ejecuta la plantillaCompilada (puede ser el cuerpo o una parte) y ejecuta todos los Incluir() que tenga y ejecuta ObtenerHtml() nuevamente de manera recursiva para esa parte. 
                p.Model = modelo;
                p.IncluirCallback = (clave, modeloIncluído) => Partes.ContainsKey(clave) ? ObtenerHtml(Partes[clave], modeloIncluído) : ""; // Se obtiene el HTML de cada una de las partes y se inserta en el HTML de respuesta.
                plantillaReferencia = p;
            });

            if (plantillaReferencia == null || plantillaReferencia.ClaveMarco == null) return htmlCuerpo;

            return Partes[plantillaReferencia.ClaveMarco].Run(p => { // Si la plantillaCompilada tiene un marco, el HTML devuelto es el del marco al que se le inserta el HTML del cuerpo obtenido anteriormente. 
                p.Model = modelo;
                p.IncluirCallback = (clave, modeloIncluído) => Partes.ContainsKey(clave) ? ObtenerHtml(Partes[clave], modeloIncluído) : "";
                p.CrearCuerpoCallback = () => htmlCuerpo;
            });

        } // ObtenerHtml>


        #endregion Métodos y Funciones>


    } // PlantillaCompilada>



} // SimpleOps.DocumentosGráficos>
