using System;
using System.Collections.Generic;
using System.Text;



// ************************************************************************************************************
// Importante  ************************************************************************************************
// ************************************************************************************************************
//
// Si se hace cualquier cambio en este archivo, se generará conflicto con los usuarios del código que hayan 
// personalizado alguno de los valores. La solución para el conflicto es que el usuario rechace el cambio 
// de sus personalizaciones en Cambios de GIT, haga Pull para incorporar los nuevos cambios y cambie nuevamente 
// los valores a sus valores personalizados.
//
// Aunque la solución es fácil, es preferible evitar hacer cambios a este archivo para que los usuarios del código 
// no tengan que hacer ese procedimiento.
//
// ************************************************************************************************************
// ************************************************************************************************************
// ************************************************************************************************************



namespace SimpleOps {



    /// <summary>
    /// Variables globales estáticas que permiten ejecutar y configurar SimpleOps más fácilmente en otros equipos y
    /// permiten la actualización desde el repositorio minimizando la cantidad de conflictos. Como es una clase que 
    /// pocas veces se va a modificar desde el desarrollo, puede ser modificada por los usuarios del código para 
    /// configurar las rutas de instalación y desarrollo y otras configuraciones sin necesidad de tenerlas que cambiar 
    /// nuevamente cada vez que se actualice el código desde el repositorio, cómo si puede suceder si estos valores
    /// se guardaran en Global.cs u OpcionesEmpresa.cs.
    /// </summary>
    static class Configuración {


        public const string RutaDesarrollo = @"D:\Programas\SimpleOps\Código\SimpleOps"; // Se usa para que al iniciar en modo de desarrollo copie los archivos CSHTML en la carpeta Plantillas en el código a la carpeta plantillas en la ruta de la aplicación.
        
        public const string RutaAplicación = @"D:\Programas\SimpleOps"; // Esta ruta debe ser la misma que está almacenada en '[RutaAplicación]\Opciones\Equipo.json', se necesita escribir aquí principalmente para poder ubicar inicialmente el archivo Equipo.json.

        public const bool ModoDesarrolloPlantillas = true; // En modo desarrollo se usa en verdadero para permitir que los cambios que se hagan a los archivos CSHTML en la carpeta Plantillas sean copiados a la ruta de la aplicación y para habilitar algunas líneas de código que facilitan el desarrollo de estas plantillas. En producción se deben usar directamente los archivos en la ruta de la aplicación porque no se tienen los de desarrollo.

        public const bool HabilitarPruebasUnitarias = true; // Se usa verdadero cuando se quieran realizar las pruebas al iniciar la aplicación.

        public const bool ModoIntegraciónTerceros = false; // Modo que permite integrar la funcionalidad de facturación electrónica y la generación de catálogos con programas terceros.


    } // Configuración>



} // SimpleOps>
