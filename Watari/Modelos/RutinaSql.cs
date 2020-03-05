using System;
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
    /// Esquema asociado (dbo,sys)
    /// </summary>
    public string Esquema { get; set; }

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

    /// <summary>
    /// Momento en el que se ha registrado
    /// </summary>
    public DateTime Creado { get; set; }

    /// <summary>
    /// Ultima vez que ha sido modificado
    /// </summary>
    public DateTime Modificado { get; set; }
  }
}
