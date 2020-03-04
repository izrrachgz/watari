using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Watari.Funciones.Sql;
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
    /// Configuracion asociada al contexto
    /// </summary>
    private static ConfiguracionContexto Configuracion { get; set; }

    /// <summary>
    /// Funciones de sql sobre rutinas
    /// </summary>
    private static FuncionRutinasSql FuncionRutinasSql { get; set; }

    #endregion

    /// <summary>
    /// Punto de entrada para la aplicacion
    /// </summary>
    /// <param name="args">Argumentos</param>
    static void Main(string[] args)
    {
      //Verificar los argumentos
      if (args == null || args.Length.Equals(0))
      {
        MostrarAyuda();
        return;
      }
      //Inicializar el entorno de trabajo
      Inicializar();
      //Procesar el comando solicitado
      ProcesarComando(args).Wait();
      Console.WriteLine(@":p");
    }

    #region Metodos Privados

    /// <summary>
    /// Establece el ambito de trabajo
    /// </summary>
    private static void Inicializar()
    {
      #region Inicializar la configuracion

      //Ruta hacia el archivo de configuracion
      string rutaConfiguracion = AppDomain.CurrentDomain.BaseDirectory + @"Configuracion.json";
      //Si no existe el archivo de configuracion se termina la tarea
      if (!File.Exists(rutaConfiguracion))
      {
        Console.WriteLine(@"No se ha encontrado el archivo de configuracion.");
        return;
      }
      StringBuilder sb = new StringBuilder();
      //Leer archivo
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
      }
      catch (Exception)
      {
        Console.WriteLine(@"No se ha podido interpretar el archivo de configuracion.");
      }

      #endregion

      #region Inicializar las instancias

      FuncionRutinasSql = new FuncionRutinasSql(Configuracion.CadenaDeConexion, Configuracion.DirectorioEsquema);

      #endregion
    }

    /// <summary>
    /// Determina la funcion y operacion a relizar
    /// segun el comando proporcionado e invoca su tarea
    /// </summary>
    /// <param name="comando">Secuencia de instrucciones</param>
    /// <returns></returns>
    private static async Task ProcesarComando(string[] args)
    {
      //El primer valor de argumento debe ser la funcion
      string funcion = args[0].ToLowerInvariant();
      //El primer valor de argumento debe ser la operacion de la funcion a ejecutar
      string operacion = args[1].ToLowerInvariant();
      //El tercer valor de argumento debe ser el parametro para la operacion
      string parametro = args[2].ToLowerInvariant();
      switch (funcion)
      {
        case @"sql":
          switch (operacion)
          {
            case @"-rutinassql":
              switch (parametro)
              {
                case @"-sincronizar":
                  await FuncionRutinasSql.Sincronizar();
                  break;
                default:
                  MostrarAyuda();
                  break;
              }
              break;
            default:
              MostrarAyuda();
              break;
          }
          break;
        default:
          MostrarAyuda();
          break;
      }
    }

    /// <summary>
    /// Muestra la ayuda de la aplicacion
    /// </summary>
    private static void MostrarAyuda()
    {
      Console.WriteLine(@"-------------Opciones---------------");
      Console.WriteLine(@">Sql -Rutinas -Sincronizar");
      Console.WriteLine(@"------------------------------------");
    }

    #endregion
  }
}
