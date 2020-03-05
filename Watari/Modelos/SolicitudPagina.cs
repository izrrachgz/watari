using Newtonsoft.Json;

namespace Watari.Modelos
{
  /// <summary>
  /// Provee un modelo de datos utilizado para inicializar los valores de búsqueda paginada
  /// </summary>
  public class SolicitudPagina
  {
    [JsonProperty("eliminados")]
    public bool Eliminados { get; set; }

    [JsonProperty("busqueda")]
    public string Busqueda { get; set; }

    [JsonProperty("elementos")]
    public int Elementos { get; set; }

    [JsonProperty("fechaInicio")]
    public string FechaInicio { get; set; }

    [JsonProperty("fechaFin")]
    public string FechaFin { get; set; }

    [JsonProperty("pagina")]
    public int Pagina { get; set; }

    [JsonProperty("orden")]
    public Ordenamiento Orden { get; set; }

    public SolicitudPagina()
    {
      Eliminados = false;
      Busqueda = @"";
      Elementos = 50;
      FechaInicio = @"";
      FechaFin = @"";
      Pagina = 1;
      Orden = new Ordenamiento();
    }
  }
}
