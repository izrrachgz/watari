using System;
using System.Collections.Generic;
using System.Linq;

namespace Watari.Modelos
{
  /// <summary>
  /// Provee un modelo de datos para retener una colección de objetos dentro de una respuesta simple
  /// </summary>
  /// <typeparam name="T">Entidad o clase</typeparam>
  public class RespuestaColeccion<T> : RespuestaBasica
  {
    /// <summary>
    /// Colección de datos genéricos para evaluar si la respuesta es correcta
    /// </summary>
    private List<T> _coleccion;

    /// <summary>
    /// Lista de objetos recolectados
    /// </summary>
    public List<T> Coleccion
    {
      get => _coleccion;
      set
      {
        _coleccion = value;
        Correcto = _coleccion != null && _coleccion.Any();
        Mensaje = Correcto ? $"Hay {Coleccion.Count} elementos en la lista." : @"No se ha encontrado la información solicitada.";
      }
    }

    /// <summary>
    /// Referencia de página que ha resultado en la lista
    /// </summary>
    public Paginado Paginado { get; set; }

    public RespuestaColeccion() { }

    /// <summary>
    /// Al recibir una exepcion se entiede que la respuesta es erronea
    /// </summary>
    /// <param name="exepcion">Error ocurrido</param>
    public RespuestaColeccion(Exception exepcion)
    {
      Exepcion = exepcion;
      Correcto = false;
      Mensaje = $@"Ha ocurrido un error interno, {exepcion.Message}.";
      Paginado = null;
      Coleccion = null;
    }

    /// <summary>
    /// Recibe la coleccion de datos asociada a la respuesta
    /// </summary>
    /// <param name="coleccion"></param>
    public RespuestaColeccion(List<T> coleccion)
    {
      Paginado = null;
      Coleccion = coleccion;
    }

    /// <summary>
    /// Recibe el paginado asociado a la solicitud y la coleccion de datos asociada a la respuesta
    /// </summary>
    /// <param name="paginado"></param>
    /// <param name="coleccion"></param>
    public RespuestaColeccion(Paginado paginado, List<T> coleccion)
    {
      Paginado = paginado;
      Coleccion = coleccion;
    }
  }
}