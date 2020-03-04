using Watari.Enumerados;

namespace Watari.Modelos
{
  /// <summary>
  /// Provee un modelo de datos que representa
  /// una rutina basica de sql
  /// </summary>
  public class RutinaSql
  {
    /// <summary>
    /// Nombre de la rutina
    /// </summary>
    public string Nombre { get; set; }

    /// <summary>
    /// Definicion de funcion
    /// </summary>
    public string Definicion { get; set; }

    /// <summary>
    /// Tipo de rutina
    /// </summary>
    public TipoRutina Tipo { get; set; }
  }
}
