using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Watari.Extensiones;

namespace Watari.Funciones.Cs
{
  public sealed class FuncionGeneradorDeCodigo
  {
    #region Propiedades

    /// <summary>
    /// Directorio donde se ubica la solucion
    /// </summary>
    private string DirectorioSolucion { get; }

    /// <summary>
    /// Directorio donde se almacenan las entidades
    /// </summary>
    private string DirectorioEntidades => $@"{DirectorioSolucion}Contexto\Entidades\";

    /// <summary>
    /// Directorio donde se almacenan los proveedores de datos
    /// </summary>
    private string DirectorioProveedoresDeDatos => $@"{DirectorioSolucion}Negocio\ProveedoresDeDatos\";

    /// <summary>
    /// Contenido de la plantilla de la entidad
    /// </summary>
    private string ContenidoPlantillaEntidad { get; set; }

    /// <summary>
    /// Contenido de la plantilla del proveedor de datos
    /// </summary>
    private string ContenidoPlantillaProveedor { get; set; }

    /// <summary>
    /// Indica si ha concluido el proceso de inicializacion
    /// </summary>
    private bool Inicializado { get; set; }

    #endregion

    public FuncionGeneradorDeCodigo(string directorioSolucion)
    {
      DirectorioSolucion = directorioSolucion;
      Inicializado = false;
    }

    #region Metodos Publicos

    /// <summary>
    /// Crea la entidad base para las entidades especificadas
    /// </summary>
    /// <param name="nombres">Entidades</param>
    /// <returns></returns>
    public async Task Entidades(string[] nombres)
    {
      await Inicializar();
      //Verificar que se haya cargado el contenido de la plantilla de entidad
      if (ContenidoPlantillaEntidad.NoEsValida())
      {
        Console.WriteLine(@"La plantilla para construir la entidad no es valida.");
        return;
      }
      foreach (string nombre in nombres)
      {
        if (nombre.NoEsValida()) continue;
        string directorioEntidad = $@"{DirectorioEntidades}{nombre}.cs";
        //Verificar si existe el archivo de entidad
        if (File.Exists(directorioEntidad))
        {
          //Solicitar confirmacion
          Console.WriteLine($@"Ya existe la entidad {nombre}.cs, ¿Quieres sobreescribir su contenido (s/n)?");
          bool continuar = Console.ReadKey(false).Key.Equals(ConsoleKey.S);
          if (continuar)
          {
            //Escribir el contenido de la entidad
            string contenido = ContenidoPlantillaEntidad.Replace(@"{{Nombre}}", nombre);
            await File.WriteAllTextAsync(directorioEntidad, contenido);
          }
        }
        else
        {
          //Escribir el contenido de la entidad
          string contenido = ContenidoPlantillaEntidad.Replace(@"{{Nombre}}", nombre);
          await File.WriteAllTextAsync(directorioEntidad, contenido);
        }
      }
    }

    /// <summary>
    /// Crea el proveedor de datos base para las entidades
    /// especificadas
    /// </summary>
    /// <param name="nombres">Nombres de las Entidades</param>
    /// <returns></returns>
    public async Task ProveedoresDeDatos(string[] nombres)
    {
      await Inicializar();
      //Verificar que se haya cargado el contenido de la plantilla de proveedor de datos
      if (ContenidoPlantillaProveedor.NoEsValida())
      {
        Console.WriteLine(@"La plantilla para construir el proveedor de datos no es valida.");
        return;
      }
      foreach (string nombre in nombres)
      {
        if (nombre.NoEsValida()) continue;
        string directorioProveedor = $@"{DirectorioProveedoresDeDatos}{nombre}.cs";
        //Verificar si existe el archivo de entidad
        if (File.Exists(directorioProveedor))
        {
          //Solicitar confirmacion
          Console.WriteLine($@"Ya existe el proveedor de datos {nombre}.cs, ¿Quieres sobreescribir su contenido (s/n)?");
          bool continuar = Console.ReadKey(false).Key.Equals(ConsoleKey.S);
          if (continuar)
          {
            //Escribir el contenido del proveedor de datos
            string contenido = ContenidoPlantillaProveedor.Replace(@"{{Nombre}}", nombre);
            await File.WriteAllTextAsync(directorioProveedor, contenido);
          }
        }
        else
        {
          //Escribir el contenido del proveedor de datos
          string contenido = ContenidoPlantillaProveedor.Replace(@"{{Nombre}}", nombre);
          await File.WriteAllTextAsync(directorioProveedor, contenido);
        }
      }
    }

    #endregion

    #region Metodos Privados

    /// <summary>
    /// Prepara el entorno de trabajo
    /// </summary>
    /// <returns></returns>
    private async Task Inicializar()
    {
      if (Inicializado) return;
      //Verificar que exista la plantilla de entidades
      string directorioPlantillaEntidad = $@"{AppDomain.CurrentDomain.BaseDirectory}Plantillas\Cs\Entidad.txt";
      if (File.Exists(directorioPlantillaEntidad))
      {
        ContenidoPlantillaEntidad = await ObtenerContenidoDeArchivo(directorioPlantillaEntidad);
      }

      //Verificar que exista la plantilla de proveedores
      string directorioPlantillaProveedor = $@"{AppDomain.CurrentDomain.BaseDirectory}Plantillas\Cs\ProveedorDeDatos.txt";
      if (File.Exists(directorioPlantillaProveedor))
      {
        ContenidoPlantillaProveedor = await ObtenerContenidoDeArchivo(directorioPlantillaProveedor);
      }
      Inicializado = true;
    }

    /// <summary>
    /// Devuelve el contenido de un archivo
    /// </summary>
    /// <param name="url">Direccion hacia el archivo</param>
    /// <returns>Cadena Alfanumerica</returns>
    private async Task<string> ObtenerContenidoDeArchivo(string url)
    {
      if (!url.EsDireccionDeArchivo()) return @"";
      StringBuilder sb = new StringBuilder();
      using (FileStream fs = File.OpenRead(url))
      {
        using (StreamReader sr = new StreamReader(fs))
        {
          while (!sr.EndOfStream)
            sb.AppendLine(await sr.ReadLineAsync());
          sr.Dispose();
        }
        fs.Dispose();
      }
      return sb.ToString();
    }

    #endregion
  }
}
