using System;
using System.Collections.Generic;
using System.Linq;
using Watari.Modelos;

namespace Watari.Extensiones
{
  /// <summary>
  /// Provee metodos de extensión para listas de columnas
  /// </summary>
  public static class ExtensionesDeListas
  {
    /// <summary>
    /// Permite obtener un valor de columna
    /// utilizando como busqueda la clave unica
    /// del elemento
    /// </summary>
    /// <typeparam name="T">Tipo de objeto interpretado retenido en el valor asociado</typeparam>
    /// <param name="columnas">Coleccion de columnas en la fila</param>
    /// <param name="nombre">Clave de identificacion unica</param>
    /// <returns>Valor de objeto interpretado</returns>
    public static T Obtener<T>(this List<ColumnaDeFila> columnas, string nombre)
    {
      if (columnas == null || !columnas.Any()) return default(T);
      object valor = columnas.FirstOrDefault(c => c.Nombre.Equals(nombre))?.Celda ?? default(T);
      valor = valor == DBNull.Value ? null : valor;
      return (T)valor;
    }
  }
}
