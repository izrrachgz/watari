using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Watari.Enumerados;
using Watari.Modelos;

namespace Watari
{
  /// <summary>
  /// Provee el mecanismo para sincronizar el esquema
  /// definido de un contexto de datos referente a sus rutinas (procedimientos y funciones escalares)
  /// </summary>
  class Program
  {
    #region Propiedades

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

    /// <summary>
    /// Configuracion asociada al contexto
    /// </summary>
    private static ConfiguracionContexto Configuracion { get; set; }

    #endregion

    /// <summary>
    /// Punto de entrada para la aplicacion
    /// </summary>
    /// <param name="args">Argumentos</param>
    static void Main(string[] args)
    {
      string rutaConfiguracion = @"";
      //Si se ha suplido un argumento se interpreta como ruta hacia la configuracion
      if (!args.Length.Equals(0))
        rutaConfiguracion = args[0];
      if (rutaConfiguracion == null || rutaConfiguracion.Trim().Length.Equals(0))
      {
        Console.WriteLine(@"No has proporcionado un archivo de configuracion, buscando uno...");
        DirectoryInfo info = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        string directorioSuperior = info.Parent?.ToString() ?? @"";
        directorioSuperior = directorioSuperior.EndsWith(@"\") ? directorioSuperior : directorioSuperior + @"\";
        string directorioConfiguracion = directorioSuperior + @"ConfiguracionContexto.json";
        if (File.Exists(directorioConfiguracion))
        {
          rutaConfiguracion = directorioConfiguracion;
          Console.WriteLine($@"Utilizando el archivo de configuracion: '{directorioConfiguracion}'");
        }
      }
      //Si no existe el archivo de configuracion se termina la tarea
      if (!File.Exists(rutaConfiguracion))
      {
        Console.WriteLine(@"No se ha encontrado el archivo de configuracion.");
        return;
      }
      StringBuilder sb = new StringBuilder();
      using (FileStream fs = File.OpenRead(rutaConfiguracion))
      {
        //Leer la configuracion
        using (StreamReader sr = new StreamReader(fs))
        {
          while (!sr.EndOfStream)
            sb.Append(sr.ReadLine());
          sr.Dispose();
        }
        fs.Dispose();
      }
      if (sb.Length.Equals(0))
        return;
      try
      {
        //Establecer valores de configuracion
        Configuracion = JsonConvert.DeserializeObject<ConfiguracionContexto>(sb.ToString());
        sb.Clear();
        SincronizarEsquema().Wait();
      }
      catch (Exception)
      {
        Console.WriteLine(@"No se ha podido interpretar el archivo de configuracion.");
      }
      Console.WriteLine(@"Ha concluido el proceso de sincronizacion.");
    }

    #region Metodos Privados

    /// <summary>
    /// Obtiene todas las rutinas sql y actualiza
    /// las asociadas conforme a los directorios proporcionados
    /// </summary>
    /// <returns></returns>
    private static async Task SincronizarEsquema()
    {
      if (Configuracion.CadenaDeConexion.Trim().Length.Equals(0))
      {
        Console.WriteLine(@"La cadena de conexion en el archivo de configuracion no es valida.");
        return;
      }
      if (Configuracion.DirectorioEsquema.Trim().Length.Equals(0))
      {
        Console.WriteLine(@"El directorio de almacenamiento en el archivo de configuracion no es valido.");
        return;
      }
      using (SqlConnection conexion = new SqlConnection(Configuracion.CadenaDeConexion))
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
            await GuardarArchivoRutinas(rutinas);
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
        conexion.Dispose();
      }
    }

    /// <summary>
    /// Guarda un listado de rutinas como archivos
    /// </summary>
    /// <param name="rutinas">Rutinas sql</param>
    /// <returns></returns>
    private static async Task GuardarArchivoRutinas(List<RutinaSql> rutinas)
    {
      //Si no hay rutinas definidas se termina la tarea
      if (rutinas == null || !rutinas.Any()) return;

      #region Verificacion de directorios

      Configuracion.DirectorioEsquema = Configuracion.DirectorioEsquema.EndsWith(@"\")
        ? Configuracion.DirectorioEsquema
        : Configuracion.DirectorioEsquema + @"\";

      //Directorio para almacenar los procedimientos
      string directorioProcedimientos = Configuracion.DirectorioEsquema + @"Procedimientos\";

      //Directorio para almacenar las funciones escalares
      string directorioFuncionesEscalares = Configuracion.DirectorioEsquema + @"Funciones\Escalar\";

      //Directorio para almacenar las funciones de tabla
      string directorioFuncionesTabla = Configuracion.DirectorioEsquema + @"Funciones\Tabla\";

      //Comprobar que exista el directorio de procedimientos, si no, crearlo.
      if (!Directory.Exists(directorioProcedimientos))
        Directory.CreateDirectory(directorioProcedimientos);

      //Comprobar que exista el directorio de funciones, si no, crearlo.
      if (!Directory.Exists(Configuracion.DirectorioEsquema + @"Funciones"))
        Directory.CreateDirectory(Configuracion.DirectorioEsquema + @"Funciones");

      //Comprobar que exista el directorio de funciones escalares, si no, crearlo.
      if (!Directory.Exists(directorioFuncionesEscalares))
        Directory.CreateDirectory(directorioFuncionesEscalares);

      //Comprobar que exista el directorio de funciones tipo tabla, si no, crearlo.
      if (!Directory.Exists(directorioFuncionesTabla))
        Directory.CreateDirectory(directorioFuncionesTabla);


      #endregion

      for (int x = 0; x < rutinas.Count; x++)
      {
        RutinaSql r = rutinas.ElementAt(x);
        string directorio;
        //Determinar el tipo de rutina
        switch (r.Tipo)
        {
          case TipoRutina.Procedimiento:
            directorio = directorioProcedimientos;
            break;
          case TipoRutina.FuncionEscalar:
            directorio = directorioFuncionesEscalares;
            break;
          case TipoRutina.FuncionTabla:
            directorio = directorioFuncionesTabla;
            break;
          default:
            directorio = Configuracion.DirectorioEsquema;
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
