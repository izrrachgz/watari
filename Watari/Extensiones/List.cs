using System;
using System.Collections.Generic;
using System.Linq;
using Watari.Modelos;
using Watari.Utilidades;

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

    /// <summary>
    /// Permite obtener una plantilla a partir de una lista de plantillas
    /// utilizando como busqueda el nombre
    /// </summary>
    /// <param name="plantillas">Coleccion de plantillas</param>
    /// <param name="nombre">Nombre</param>
    /// <returns>Plantilla</returns>
    public static Plantilla Obtener(this List<Plantilla> plantillas, string nombre)
    {
      if (nombre.NoEsValida()) return null;
      return plantillas.FirstOrDefault(p => p.Nombre.Equals(nombre));
    }
  }
}
