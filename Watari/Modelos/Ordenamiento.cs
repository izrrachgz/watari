using System.Collections.Generic;
using Newtonsoft.Json;

namespace Watari.Modelos
{
  /// <summary>
  /// Provee un modelo de datos
  /// para especificar la manera de ordenamiento
  /// </summary>
  public class Ordenamiento
  {
    /// <summary>
    /// Especifica las columnas por las que
    /// se deben ordenar el conjunto
    /// </summary>
    [JsonProperty("columnas")]
    public List<string> Columnas { get; set; }

    /// <summary>
    /// Especifica la manera de ordenar
    /// el conjunto
    /// </summary>
    [JsonProperty("ascendente")]
    public bool Ascendente { get; set; }

    public Ordenamiento()
    {
      Columnas = new List<string>(1) { "Id" };
      Ascendente = true;
    }
  }
}
