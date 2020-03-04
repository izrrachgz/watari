using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Watari.Enumerados;
using Watari.Modelos;

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
    /// Directorio para almacenar las rutinas
    /// </summary>
    private string DirectorioEsquema { get; }

    /// <summary>
    /// Directorio para almacenar los procedimientos
    /// </summary>
    private string DirectorioProcedimientos => DirectorioEsquema + @"Procedimientos\";

    /// <summary>
    /// Directorio para almacenar las funciones escalares
    /// </summary>
    private string DirectorioFuncionesEscalares => DirectorioEsquema + @"Funciones\Escalar\";

    /// <summary>
    /// Directorio para almacenar las funciones de tabla
    /// </summary>
    private string DirectorioFuncionesTabla => DirectorioEsquema + @"Funciones\Tabla\";

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
      DirectorioEsquema = directorioEsquema.EndsWith(@"\")
        ? directorioEsquema
        : directorioEsquema + @"\";
    }

    #region Metodos Publicos

    /// <summary>
    /// Agrega/Actualiza/Elimina todas las rutinas sql
    /// asociadas al esquema del repositorio de datos
    /// </summary>
    /// <returns></returns>
    public async Task Sincronizar()
    {
      if (CadenaDeConexion.Trim().Length.Equals(0))
      {
        Console.WriteLine(@"La cadena de conexion en el archivo de configuracion no es valida.");
        return;
      }
      if (DirectorioEsquema.Trim().Length.Equals(0))
      {
        Console.WriteLine(@"El directorio de almacenamiento en el archivo de configuracion no es valido.");
        return;
      }
      using (SqlConnection conexion = new SqlConnection(CadenaDeConexion))
      {
        try
        {
          //Establecer la conexion con el repositorio
          await conexion.OpenAsync();
          List<RutinaSql> rutinas = null;
          using (SqlCommand comando = new SqlCommand(SqlObtenerRutinas, conexion))
          {
            using (SqlDataReader lector = await comando.ExecuteReaderAsync())
            {
              if (lector.HasRows)
              {
                //Leer todas las rutinas
                rutinas = new List<RutinaSql>();
                while (await lector.ReadAsync())
                {
                  RutinaSql rutina = new RutinaSql()
                  {
                    Nombre = $@"[{lector.GetString(0)}].[{lector.GetString(1)}]",
                    Tipo = (TipoRutina)lector.GetInt32(2),
                    Definicion = lector.GetString(3)
                  };
                  rutinas.Add(rutina);
                }
                Console.WriteLine($@"Funciones Escalar : {rutinas.Count(r => r.Tipo.Equals(TipoRutina.FuncionEscalar))}.");
                Console.WriteLine($@"Funciones Tabla : {rutinas.Count(r => r.Tipo.Equals(TipoRutina.FuncionTabla))}.");
                Console.WriteLine($@"Procedimientos Almacenados : {rutinas.Count(r => r.Tipo.Equals(TipoRutina.Procedimiento))}.");
                //Eliminar todas las rutinas anteriores
                EliminarRutinasAnteriores();
              }
              else
              {
                Console.WriteLine(@"No hay ninguna rutina asociada al esquema.");
              }
              lector.Dispose();
            }
            comando.Dispose();
          }
          //Terminar la sesion
          if (conexion.State.Equals(ConnectionState.Open))
            conexion.Close();
          //Guardar todas las rutinas
          if (rutinas != null && rutinas.Any())
            await GuardarRutinasEnArchivos(rutinas);
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
        conexion.Dispose();
      }
    }

    #endregion

    #region Metodos Privados

    /// <summary>
    /// Determina si existen los directorios asociados a las rutinas
    /// sql y si no, los crea.
    /// </summary>
    private void InicializarDirectoriosDelEsquema()
    {
      //Comprobar que exista el directorio de procedimientos, si no, crearlo.
      if (!Directory.Exists(DirectorioProcedimientos))
        Directory.CreateDirectory(DirectorioProcedimientos);

      //Comprobar que exista el directorio de funciones, si no, crearlo.
      if (!Directory.Exists(DirectorioEsquema + @"Funciones"))
        Directory.CreateDirectory(DirectorioEsquema + @"Funciones");

      //Comprobar que exista el directorio de funciones escalares, si no, crearlo.
      if (!Directory.Exists(DirectorioFuncionesEscalares))
        Directory.CreateDirectory(DirectorioFuncionesEscalares);

      //Comprobar que exista el directorio de funciones tipo tabla, si no, crearlo.
      if (!Directory.Exists(DirectorioFuncionesTabla))
        Directory.CreateDirectory(DirectorioFuncionesTabla);
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
      InicializarDirectoriosDelEsquema();
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
            directorio = DirectorioEsquema;
            break;
        }
        string urlArchivo = $@"{directorio}{r.Nombre}.sql";
        //Abrir o escribir el archivo
        using (FileStream fs = File.OpenWrite(urlArchivo))
        {
          fs.Position = 0;
          Console.WriteLine($@"Escribiendo archivo {r.Nombre}.");
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
