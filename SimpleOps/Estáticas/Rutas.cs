using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps {



    /// <summary>
    /// Variables globales estáticas que permiten ejecutar y configurar SimpleOps más fácilmente en otros equipos y
    /// permiten la actualización desde el repositorio minimizando la cantidad de conflictos.
    /// Cómo es una clase que pocas veces se va a modificar desde el desarrollo 
    /// puede ser modificada por los usuarios del código para configurar las rutas de instalación
    /// y desarrollo sin necesidad de tenerlas que cambiar nuevamente cada vez que se actualice el código desde
    /// el repositorio, cómo si puede suceder si estas rutas se guardaran en Global.cs u OpcionesEmpresa.cs.
    /// </summary>
    static class Rutas {


        public static string Desarrollo = @"D:\Archivos\Proyectos\SimpleOps\Código\SimpleOps\SimpleOps"; // Se usa para que al iniciar en modo de desarrollo copie los archivos CSHTML en Plantillas a 'CarpetaPlantillas' en la ruta de la aplicación. En el computador de desarrollo en casa está en D:\Archivos\Proyectos\SimpleOps\Código\SimpleOps\SimpleOps. En el computador de desarrollo de la empresa está en E:\Dropbox\Desarrollos\SimpleOps\Código\SimpleOps\SimpleOps.
        
        public static string Aplicación = @"D:\Programas\SimpleOps"; // En el computador de producción está en D:\Programas\SimpleOps. En el computador de desarrollo en casa está en D:\Programas\SimpleOps. En el computador de desarrollo de la empresa está en E:\SimpleOps. Esta ruta debe ser la misma que está almacenada en '[RutaAplicación]\Opciones\Equipo.json', se necesita escribir aquí principalmente para poder ubicar inicialmente este archivo de opciones.


    } // Rutas>



} // SimpleOps>
