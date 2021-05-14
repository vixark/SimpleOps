// *****************************************************************************************************************************************************
// Importante  *****************************************************************************************************************************************
// *****************************************************************************************************************************************************
//
// Si se hace cualquier cambio en este archivo, se generará conflicto con los usuarios del código que hayan personalizado alguno de los valores.
// La solución para el conflicto es que el usuario rechace el cambio de sus personalizaciones en Cambios de GIT, haga Pull para incorporar los nuevos
// cambios y cambie nuevamente los valores a sus valores personalizados.
//
// Aunque la solución es fácil, es preferible evitar hacer cambios a este archivo para que los usuarios del código no tengan que hacer ese procedimiento.
//
// *****************************************************************************************************************************************************
// *****************************************************************************************************************************************************
// *****************************************************************************************************************************************************



namespace SimpleOps {



    /// <summary>
    /// Constantes globales que permiten configurar SimpleOps en equipos diferentes manteniendo estas configuraciones al actualizar el código 
    /// desde repositorio. Como es una clase que pocas veces se va a modificar desde el desarrollo, puede ser modificada por los usuarios del código 
    /// para configurar las rutas de instalación y desarrollo y otras configuraciones sin necesidad de tenerlas que cambiar nuevamente cada vez que se 
    /// actualice el código desde el repositorio, cómo sí podría suceder si se guardaran en Global.cs, OpcionesEmpresa.cs u OpcionesEquipo.cs. 
    /// Estas configuraciones solo son de interés para el instalador, el usuario del código o el desarrollador. No se deben agregar configuraciones que 
    /// el usuario del programa quisiera cambiar en algún momento (estas deben ir en OpcionesEmpresa.cs u OpcionesEquipo.cs). Tampoco se deben agregar 
    /// configuraciones que el usuario del código podría modificar, pero no es usual (estas deben ir en Global.cs).
    /// </summary>
    static class Configuración {


        public const string RutaDesarrollo = @"D:\Programas\SimpleOps\Código\SimpleOps"; // Se usa para poder ubicar las plantillas CSHTML en el modo de desarrollo de plantillas.
        
        public const string RutaAplicación = @"D:\Programas\SimpleOps"; // Esta ruta debe ser la misma que está almacenada en '[RutaAplicación]\Opciones\Equipo.json'. Se necesita escribir aquí principalmente para poder ubicar inicialmente el archivo Equipo.json.

        public const bool ModoIntegraciónTerceros = true; // Modo que permite integrar la funcionalidad de facturación electrónica y la generación de catálogos con programas terceros o Excel.

        public const bool UsarSQLite = true; // Si es falso, se usa Microsoft SQL.

        public const bool GuardarFechaReducidaSQLite = true; // Si es verdadero, la base de datos se construirá con algunas columnas (FechaHoraDeCreación y otras que no requieren exactitud a la hora y minuto) con formato yyMMdd y otras columnas con formato yyMMddhhmmssf. Esto ahorra alrededor de 15% de espacio comparado con el formato por defecto. En los ensayos realizados con los datos iniciales se obtuvo el mismo tiempo de escritura de toda la base de datos de 5:30 min así que se espera que el rendimiento sea el mismo y el tamaño pasó de 23 752 KB a 20 268 KB. Ver https://stackoverflow.com/questions/49261542/entityframework-core-format-datetime-in-sqlite-database/59981186#59981186. Para cambiar este comportamiento se debe borrar la base de datos, reiniciar las migraciones y recrearla.

        public const bool GuardarDecimalComoDoubleSQLite = true; // Si es verdadero, todas las propiedades decimal se almacenarán en la base de datos SQLite como double (real). Esto implica una pérdida de resolución númerica, pero permite realizar operaciones de comparación como OrderBy() directamente en la base de datos y reduce en alrededor de 7% el tamaño de la base de datos. En los ensayos realizados se pasó de 20 268 KB (usando OptimizarTamañoSQLiteConFechaReducida) a 18 816 KB. Ver recomendación en https://docs.microsoft.com/en-us/ef/core/providers/sqlite/?tabs=dotnet-core-cli.

        public const bool HabilitarRastreoDeDatosSensibles = false; // Si se establece en verdadero, en la ventana 'Inmediato' se podrá ver el detalle de las acciones ejecutados en la base de datos, incluyendo los datos. Si se establece en falso, EF Core reemplazará los datos por textos de reemplazo. En producción siempre debe estar en falso.

        public const bool HabilitarPlantillasPropias = true; // Si es verdadero, se buscarán en la ruta de la aplicación las plantillas finalizadas en Propia (por ejemplo, CatálogoPdfPropia.cshtml) y estas se usaran para generar los documentos y se copiarán a la ruta de la aplicación. Esto es útil principalmente para el desarrollo de dos versiones de las plantillas, 1. Desarrollo de las plantillas públicas para el repositorio (las no Propias). 2. Desarrollo de versiones propias o personalizadas para una empresa en particular. Normalmente su valor es verdadero, pues si no se encuentra la plantilla Propia, se usa la normal. Solo se establece en falso cuando se quieren editar y probar las plantillas no propias, lo cual no es un necesidad común para un usuario del código.


    } // Configuración>



} // SimpleOps>
