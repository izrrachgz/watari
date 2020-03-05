using System;
using System.Linq;
using Newtonsoft.Json;
using Watari.Extensiones;

namespace Watari.Modelos
{
  /// <summary>
  /// Provee un modelo de datos para realizar consultas paginadas
  /// </summary>
  public class Paginado
  {
    /// <summary>
    /// Palabras clave para búsqueda
    /// </summary>
    public string Busqueda { get; set; }

    /// <summary>
    /// Indica si hay que incluir registros eliminados
    /// </summary>
    public bool Eliminados { get; set; }

    /// <summary>
    /// Indica el inicio del rango de tiempo de búsqueda
    /// </summary>
    public DateTime RangoFechaInicio { get; set; }

    /// <summary>
    /// Indica el fin del rango de tiempo de búsqueda
    /// </summary>
    public DateTime RangoFechaFin { get; set; }

    /// <summary>
    /// Indica el indice de pagina actual => 0
    /// </summary>
    [JsonIgnore]
    public int PaginaIndice { get; set; }

    /// <summary>
    /// Indica el indice de pagina solicitado => 1
    /// </summary>
    public int Pagina { get; set; }

    /// <summary>
    /// Indica el total de paginas que hay con base con los resultados de consulta => TotalElementos/Elementos
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Cantidad de elementos que se incluyen en la pagina => 10
    /// </summary>
    public int Elementos { get; set; }

    /// <summary>
    /// Total de elementos que hay con base con los resultados de consulta => query.Count()
    /// </summary>
    public int TotalElementos { get; set; }

    /// <summary>
    /// Especifica la manera de ordenamiento
    /// </summary>
    public Ordenamiento Orden { get; set; }

    /// <summary>
    /// Inicializa la solicitud de pagina con valores predeterminados
    /// </summary>
    public Paginado()
    {
      Busqueda = @"";
      Eliminados = false;
      Pagina = 1;
      TotalPaginas = 0;
      PaginaIndice = Pagina - 1;
      Elementos = 50;
      TotalElementos = 0;
      RangoFechaInicio = DateTime.MinValue;
      RangoFechaFin = DateTime.MinValue;
      Orden = new Ordenamiento();
    }

    /// <summary>
    /// Inicializa la solicitud de pagina a traves de una solicitud
    /// </summary>
    /// <param name="solicitud"></param>
    public Paginado(SolicitudPagina solicitud)
    {
      Busqueda = solicitud.Busqueda;
      Eliminados = solicitud.Eliminados;
      Pagina = solicitud.Pagina;
      Pagina = Pagina < 1 ? 1 : Pagina;
      PaginaIndice = Pagina - 1;
      PaginaIndice = PaginaIndice < 0 ? 0 : PaginaIndice;
      Elementos = solicitud.Elementos;
      Elementos = Elementos < 50 ? 50 : Elementos;
      if (!solicitud.FechaInicio.NoEsValida())
      {
        DateTime.TryParse(solicitud.FechaInicio, out DateTime inicio);
        RangoFechaInicio = inicio;
      }
      if (!solicitud.FechaFin.NoEsValida())
      {
        DateTime.TryParse(solicitud.FechaFin, out DateTime fin);
        RangoFechaFin = fin;
      }
      Orden = solicitud.Orden ?? new Ordenamiento();
    }

    /// <summary>
    /// Prepara el paginado mediante las condiciones especificadas en la consulta
    /// Este metodo debe usarse siempre al obtener el objeto de consulta completo
    /// </summary>
    /// <param name="query"></param>
    public void CalcularPaginado<T>(IQueryable<T> query)
      => CalcularPaginado(query.Count());

    /// <summary>
    /// Prepara el paginado mediante las condiciones especificadas en la consulta
    /// Este metodo debe usarse siempre al obtener el objeto de consulta completo
    /// </summary>
    /// <param name="total"></param>
    public void CalcularPaginado(int total)
    {
      TotalElementos = total;
      TotalPaginas = TotalElementos / Elementos;
      if (!(TotalPaginas * Elementos).Equals(TotalElementos)) TotalPaginas++;
      Pagina = Pagina > TotalPaginas ? TotalPaginas : Pagina;
      PaginaIndice = Pagina - 1;
      PaginaIndice = PaginaIndice < 0 ? 0 : PaginaIndice;
    }
  }
}