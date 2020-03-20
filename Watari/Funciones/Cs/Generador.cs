using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Watari.Extensiones;
using Watari.Utilidades;

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
    /// Directorio donde se almacenan las configuraciones de entidad
    /// </summary>
    private string DirectorioConfiguracionEntidad => $@"{DirectorioSolucion}Contexto\Esquema\Configuraciones\";

    /// <summary>
    /// Directorio donde se almacenan los proveedores de datos
    /// </summary>
    private string DirectorioProveedoresDeDatos => $@"{DirectorioSolucion}Negocio\ProveedoresDeDatos\";

    /// <summary>
    /// Directorio donde se almacenan los controladores api
    /// </summary>
    private string DirectorioControladoresApi => $@"{DirectorioSolucion}Api\Controllers\";

    /// <summary>
    /// Indica si ha concluido el proceso de inicializacion
    /// </summary>
    private bool Inicializado { get; set; }

    /// <summary>
    /// Plantillas para generar archivos
    /// </summary>
    private List<Plantilla> Plantillas { get; set; }

    #endregion

    public FuncionGeneradorDeCodigo(string directorioSolucion)
    {
      DirectorioSolucion = directorioSolucion;
      Plantillas = new List<Plantilla>(0);
      Inicializado = false;
    }

    #region Metodos Publicos

    /// <summary>
    /// Crea la entidad, su configuracion y el proveedor de datos
    /// asociado.
    /// </summary>
    /// <param name="nombres">Eentidades</param>
    /// <returns></returns>
    public async Task ConjuntoEntidad(string[] nombres)
    {
      //Generar todas las entidades
      await Entidades(nombres);
      //Generar todas sus configuraciones
      await ConfiguracionesDeEntidades(nombres);
      //Generar todos sus proveedores de datos
      await ProveedoresDeDatos(nombres);
      //Generar todos sus controladores api
      await ControladoresApi(nombres);
    }

    /// <summary>
    /// Crea la entidad base para las entidades especificadas
    /// </summary>
    /// <param name="nombres">Entidades</param>
    /// <returns></returns>
    public async Task Entidades(string[] nombres)
    {
      await Inicializar();
      Plantilla p = Plantillas.Obtener(@"Entidad");
      //Verificar que se haya cargado el contenido de la plantilla
      if (p == null || p.Contenido.NoEsValida())
      {
        Console.WriteLine(@"La plantilla para construir la entidad no es valida.");
        return;
      }
      string[] claves = { @"{{Nombre}}" };
      foreach (string nombre in nombres)
      {
        await p.Generar(DirectorioEntidades, nombre, claves, new Dictionary<string, string>(1) { { @"{{Nombre}}", nombre } });
      }
    }

    /// <summary>
    /// Crea la entidad base para las entidades especificadas
    /// </summary>
    /// <param name="nombres">Entidades</param>
    /// <returns></returns>
    public async Task ConfiguracionesDeEntidades(string[] nombres)
    {
      await Inicializar();
      Plantilla p = Plantillas.Obtener(@"ConfiguracionEntidad");
      //Verificar que se haya cargado el contenido de la plantilla
      if (p == null || p.Contenido.NoEsValida())
      {
        Console.WriteLine(@"La plantilla para construir la configuracion de la entidad no es valida.");
        return;
      }
      string[] claves = { @"{{Nombre}}" };
      foreach (string nombre in nombres)
      {
        await p.Generar(DirectorioConfiguracionEntidad, nombre, claves, new Dictionary<string, string>(1) { { @"{{Nombre}}", nombre } });
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
      Plantilla p = Plantillas.Obtener(@"ProveedorDeDatos");
      //Verificar que se haya cargado el contenido de la plantilla
      if (p == null || p.Contenido.NoEsValida())
      {
        Console.WriteLine(@"La plantilla para construir el proveedor de datos no es valida.");
        return;
      }
      string[] claves = { @"{{Nombre}}" };
      foreach (string nombre in nombres)
      {
        await p.Generar(DirectorioProveedoresDeDatos, nombre, claves, new Dictionary<string, string>(1) { { @"{{Nombre}}", nombre } });
      }
    }

    /// <summary>
    /// Crea el controlador api de datos base para las entidades
    /// especificadas
    /// </summary>
    /// <param name="nombres">Nombres de los Controladores</param>
    /// <returns></returns>
    public async Task ControladoresApi(string[] nombres)
    {
      await Inicializar();
      Plantilla p = Plantillas.Obtener(@"ControladorApi");
      //Verificar que se haya cargado el contenido de la plantilla
      if (p == null || p.Contenido.NoEsValida())
      {
        Console.WriteLine(@"La plantilla para construir el controlador api no es valida.");
        return;
      }
      string[] claves = { @"{{Nombre}}" };
      foreach (string nombre in nombres)
      {
        await p.Generar(DirectorioControladoresApi, nombre, claves, new Dictionary<string, string>(1) { { @"{{Nombre}}", nombre } });
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
      Plantillas = new List<Plantilla>(3)
      {
        new Plantilla(@"Cs\", @"Entidad", @"cs"),
        new Plantilla(@"Cs\", @"ConfiguracionEntidad", @"cs"),
        new Plantilla(@"Cs\", @"ProveedorDeDatos", @"cs"),
        new Plantilla(@"Cs\", @"ControladorApi", @"cs"),
      };
      foreach (Plantilla p in Plantillas)
        await p.Cargar();
      Inicializado = true;
    }

    #endregion
  }
}
