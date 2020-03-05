using System;

namespace Watari.Modelos
{
  /// <summary>
  /// Provee un modelo de datos para retener un objeto dentro de una respuesta simple
  /// </summary>
  /// <typeparam name="T">Entidad o clase</typeparam>
  public class RespuestaModelo<T> : RespuestaBasica
  {
    /// <summary>
    /// Modelo de datos genérico para evaluar si la respuesta es correcta
    /// </summary>
    private T _modelo;

    /// <summary>
    /// Modelo de datos genérico para incluir en la respuesta
    /// </summary>
    public T Modelo
    {
      get => _modelo;
      set
      {
        _modelo = value;
        Correcto = _modelo != null;
        Mensaje = Correcto ? @"Información encontrada." : @"No se ha encontrado la información solicitada.";
      }
    }

    public RespuestaModelo() { }

    /// <summary>
    /// Al recibir una exepcion se entiede que la respuesta es erronea
    /// </summary>
    /// <param name="exepcion">Error ocurrido</param>
    public RespuestaModelo(Exception exepcion)
    {
      Correcto = false;
      Exepcion = exepcion;
      Mensaje = @"Ha ocurrido un error interno.";
    }

    /// <summary>
    /// Recibe el modelo asociado a la respuesta
    /// </summary>
    /// <param name="modelo"></param>
    public RespuestaModelo(T modelo)
    {
      Modelo = modelo;
    }
  }
}