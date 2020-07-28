using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;



namespace SimpleOps.DocumentosGráficos {



    /// <summary>
    /// Plantilla con parámetro de tipo que puede tomar cualquier clase (usualmente una clase simple como Datos[].cs) que permite escribir código Razor (cshtml) 
    /// de manera tipada. En el HTML se debe usar @Model para acceder al objeto (usualmente Datos[].cs) y a sus propiedades. Permite la adición de
    /// partes de HTML incluídos dentro de esta usando @Incluir("ClaveParte", Model) y permite usar marcos de HTML referenciando
    /// el marco en esta con @{ ClaveMarco = "[ClavePlantillaActual]"; } y generando el cuerpo en el marco con @CrearCuerpo().
    /// Ver <see cref="Pruebas.GeneraciónPdf"/> para un ejemplo de su funcionamiento.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PlantillaBase<T> : RazorEngineTemplateBase<T> where T : class { // Tomada de https://github.com/adoconnection/RazorEngineCore/wiki/@Include-and-@Layout.


        #region Propiedades

        /// <summary>
        /// Función para incluir en la plantilla actual una parte de HTML usando su clave en el diccionario de partes 
        /// (<see cref="CompilarPlantilla{T}(RazorEngine, string, IDictionary{string, string})"/>). En el HTML se usa así 
        /// @Incluir("[ClavePlantillaAIncluir]", Model).
        /// </summary>
        public Func<string, T, string>? IncluirCallback { get; set; }

        /// <summary>
        /// Función para usar un marco (equivalente a _Layout.cshtml) para la plantilla actual (que funciona como cuerpo). El marco se referencia 
        /// en el HTML cuerpo con su clave en el diccionario de partes (<see cref="CompilarPlantilla{T}(RazorEngine, string, IDictionary{string, string})"/>)
        /// así @{ ClaveMarco = "[ClaveMarco]"; } y en el HTML del marco se inserta el cuerpo con con @CrearCuerpo().
        /// </summary>
        public Func<string>? CrearCuerpoCallback { get; set; }

        /// <summary>
        /// Clave en el diccionario de partes (<see cref="CompilarPlantilla{T}(RazorEngine, string, IDictionary{string, string})"/>)
        /// de la parte de HTML que servirá como marco (equivalente a _Layout.cshtml) para la plantilla actual.
        /// Se establece en el HTML de la plantilla actual así @{ ClaveMarco = "[ClaveMarco]"; }. Además, en el HTML del marco se debe
        /// crear el cuerpo (plantilla actual) con @CrearCuerpo().
        /// </summary>
        public string? ClaveMarco { get; set; }

        #endregion Propiedades>


        #region Métodos

        public string? Incluir(string clave, T modelo) => IncluirCallback == null ? null : IncluirCallback(clave, modelo);

        public string? CrearCuerpo() => CrearCuerpoCallback == null ? null : CrearCuerpoCallback();

        #endregion Métodos>


    } // PlantillaBase>



} // SimpleOps.DocumentosGráficos>
