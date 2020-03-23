using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Watari.Enumerados;
using Watari.Extensiones;
using Watari.Modelos;
using Watari.Utilidades;

namespace Watari.Funciones.Sql
{
  /// <summary>
  /// Provee el mecanismo para manipular las rutinas asociadas
  /// al esquema del repositorio de datos
  /// </summary>
  public sealed class FuncionRutinasSql
  {
    #region Propiedades

    /// <summary>
    /// Configuracion de conexion hacia el repositorio de datos
    /// </summary>
    private string CadenaDeConexion { get; }

    /// <summary>
    /// Directorio donde se ubica la solucion
    /// </summary>
    private string DirectorioSolucion { get; }

    /// <summary>
    /// Directorio para almacenar los procedimientos
    /// </summary>
    private string DirectorioProcedimientos => $@"{DirectorioSolucion}Contexto\Esquema\Procedimientos\";

    /// <summary>
    /// Directorio para almacenar las funciones escalares
    /// </summary>
    private string DirectorioFuncionesEscalares => $@"{DirectorioSolucion}Contexto\Esquema\Funciones\Escalar\";

    /// <summary>
    /// Directorio para almacenar las funciones de tabla
    /// </summary>
    private string DirectorioFuncionesTabla => $@"{DirectorioSolucion}Contexto\Esquema\Funciones\Tabla\";

    /// <summary>
    /// Instruccion sql para obtener todas las rutina que no son de sistema
    /// </summary>
    private const string SqlObtenerRutinas = @"
      SELECT
          SPECIFIC_SCHEMA AS Esquema,
          SPECIFIC_NAME AS Nombre,
          (CASE WHEN ROUTINE_TYPE = N'PROCEDURE'
                THEN 0
                WHEN ROUTINE_TYPE = N'FUNCTION' AND DATA_TYPE <> N'TABLE'
                THEN 1
                WHEN ROUTINE_TYPE = N'FUNCTION' AND DATA_TYPE = N'TABLE'
                THEN 2
          END) AS Tipo,
          ROUTINE_DEFINITION AS Definicion,
          DATA_TYPE AS SubTipo,
          CREATED AS Creado,
	        LAST_ALTERED AS Modificado
      FROM INFORMATION_SCHEMA.ROUTINES
      WHERE LEFT(ROUTINE_NAME, 3) NOT IN ('sp_', 'xp_', 'ms_')";

    #endregion

    public FuncionRutinasSql(string cadenaDeConexion, string directorioEsquema)
    {
      CadenaDeConexion = cadenaDeConexion;
      DirectorioSolucion = directorioEsquema;
    }

    #region Metodos Publicos

    /// <summary>
    /// Agrega/Actualiza/Elimina todas las rutinas sql
    /// asociadas al esquema del repositorio de datos
    /// </summary>
    /// <returns></returns>
    public async Task Sincronizar()
    {
      List<RutinaSql> rutinas = await ObtenerRutinasSql();
      Console.WriteLine($@"Funciones Escalar : {rutinas.Count(r => r.Tipo.Equals(TipoRutina.FuncionEscalar))}.");
      Console.WriteLine($@"Funciones Tabla : {rutinas.Count(r => r.Tipo.Equals(TipoRutina.FuncionTabla))}.");
      Console.WriteLine($@"Procedimientos Almacenados : {rutinas.Count(r => r.Tipo.Equals(TipoRutina.Procedimiento))}.");
      if (rutinas.Any())
      {
        EliminarRutinasAnteriores();
        await GuardarRutinasEnArchivos(rutinas);
      }
    }

    /// <summary>
    /// Muestra todas las rutinas sql registradas en el esquema asociado
    /// </summary>
    /// <returns></returns>
    public async Task Listado()
    {
      List<RutinaSql> rutinas = await ObtenerRutinasSql();
      rutinas.ForEach(r =>
      {
        Console.WriteLine($@"{r.Tipo} [{r.Esquema}].[{r.Nombre}]");
        Console.WriteLine($@"   Creado      [{r.Creado}]");
        Console.WriteLine($@"   Modificado  [{r.Modificado}]");
        Console.WriteLine(@"");
      });
    }

    #endregion

    #region Metodos Privados

    /// <summary>
    /// Determina si existen los directorios asociados a las rutinas
    /// sql y si no, los crea.
    /// </summary>
    private void Inicializar()
    {
      //Comprobar que exista el directorio de procedimientos, si no, crearlo.
      if (!Directory.Exists(DirectorioProcedimientos))
        Directory.CreateDirectory(DirectorioProcedimientos);

      //Comprobar que exista el directorio de funciones, si no, crearlo.
      if (!Directory.Exists(DirectorioSolucion + @"Funciones"))
        Directory.CreateDirectory(DirectorioSolucion + @"Funciones");

      //Comprobar que exista el directorio de funciones escalares, si no, crearlo.
      if (!Directory.Exists(DirectorioFuncionesEscalares))
        Directory.CreateDirectory(DirectorioFuncionesEscalares);

      //Comprobar que exista el directorio de funciones tipo tabla, si no, crearlo.
      if (!Directory.Exists(DirectorioFuncionesTabla))
        Directory.CreateDirectory(DirectorioFuncionesTabla);
    }

    /// <summary>
    /// Permite obtener todas las rutinas sql registradas
    /// en el esquema asociado
    /// </summary>
    /// <returns>Coleccion de rutinas sql</returns>
    private async Task<List<RutinaSql>> ObtenerRutinasSql()
    {
      ComandoSql comando = new ComandoSql(CadenaDeConexion);
      RespuestaColeccion<FilaDeTabla> resultados = await comando.Consulta(SqlObtenerRutinas);
      List<RutinaSql> rutinas;
      if (resultados.Correcto)
      {
        rutinas = resultados.Coleccion.Select(r => new RutinaSql
        {
          Esquema = r.Columnas.Obtener<string>(@"Esquema"),
          Nombre = r.Columnas.Obtener<string>(@"Nombre"),
          Definicion = r.Columnas.Obtener<string>(@"Definicion"),
          Tipo = r.Columnas.Obtener<TipoRutina>(@"Tipo"),
          Creado = r.Columnas.Obtener<DateTime>(@"Creado"),
          Modificado = r.Columnas.Obtener<DateTime>(@"Modificado")
        }).ToList();
      }
      else
      {
        rutinas = new List<RutinaSql>(0);
      }
      return rutinas;
    }

    /// <summary>
    /// Elimina todos los archivos de rutina anteriores
    /// y prepara los directorios para archivas las nuevas
    /// </summary>
    private void EliminarRutinasAnteriores()
    {
      try
      {
        Directory.Delete(DirectorioProcedimientos, true);
        Directory.Delete(DirectorioFuncionesTabla, true);
        Directory.Delete(DirectorioFuncionesEscalares, true);
      }
      catch (Exception e)
      {
        Console.WriteLine($@"No se han podido eliminar las rutinas anteriores {e.Message}");
      }
      Inicializar();
    }

    /// <summary>
    /// Guarda un listado de rutinas como archivos
    /// </summary>
    /// <param name="rutinas">Rutinas sql</param>
    /// <returns></returns>
    private async Task GuardarRutinasEnArchivos(List<RutinaSql> rutinas)
    {
      //Si no hay rutinas definidas se termina la tarea
      if (rutinas == null || !rutinas.Any()) return;

      for (int x = 0; x < rutinas.Count; x++)
      {
        RutinaSql r = rutinas.ElementAt(x);
        string directorio;
        //Determinar el tipo de rutina
        switch (r.Tipo)
        {
          case TipoRutina.Procedimiento:
            directorio = DirectorioProcedimientos;
            break;
          case TipoRutina.FuncionEscalar:
            directorio = DirectorioFuncionesEscalares;
            break;
          case TipoRutina.FuncionTabla:
            directorio = DirectorioFuncionesTabla;
            break;
          default:
            directorio = DirectorioSolucion;
            break;
        }
        string urlArchivo = $@"{directorio}{r.Nombre}.sql";
        //Abrir o escribir el archivo
        using (FileStream fs = File.OpenWrite(urlArchivo))
        {
          fs.Position = 0;
          Console.WriteLine($@"Escribiendo archivo {urlArchivo}.");
          //Escribir el contenido del archivo desde la posicion 0
          using (StreamWriter sw = new StreamWriter(fs))
          {
            sw.AutoFlush = true;
            await sw.WriteAsync(r.Definicion);
            sw.Dispose();
          }
          fs.Dispose();
        }
      }
    }

    #endregion
  }
}
