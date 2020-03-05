using System;

namespace Watari.Modelos
{
  /// <summary>
  /// Provee un modelo de datos para representar una celda de una tabla
  /// </summary>
  public class ColumnaDeFila
  {
    /// <summary>
    /// Indice de la columna
    /// </summary>
    public int Indice { get; }

    /// <summary>
    /// Nombre de la columna
    /// </summary>
    public string Nombre { get; }

    /// <summary>
    /// Valor de la celda
    /// </summary>
    public object Celda { get; }

    public ColumnaDeFila() { }

    public ColumnaDeFila(int indice, string nombre, object valor)
    {
      Indice = indice;
      Nombre = nombre;
      Celda = valor == DBNull.Value ? null : valor;
    }
  }
}
