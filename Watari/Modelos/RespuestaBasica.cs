using System;
using Newtonsoft.Json;

namespace Watari.Modelos
{
  /// <summary>
  /// Provee un modelo de datos para proporcionar una respuesta simple
  /// </summary>
  public class RespuestaBasica
  {
    /// <summary>
    /// Indica si la respuesta es correcta o no
    /// </summary>
    public bool Correcto { get; set; }

    /// <summary>
    /// Proporciona una breve descripción de la respuesta a la solicitud
    /// </summary>
    public string Mensaje { get; set; }

    /// <summary>
    /// Proporciona una descripción del error ocurrido
    /// </summary>
    [JsonIgnore]
    public Exception Exepcion { get; set; }

    /// <summary>
    /// Al crear una instancia sin valores, se infiere el desconocimiento del contexto de la tarea, por lo que predeterminadamente se asume que es incorrecto
    /// </summary>
    public RespuestaBasica()
    {
      Correcto = false;
      Mensaje = @"";
    }

    /// <summary>
    /// Al recibir una exepcion se entiede que la respuesta es erronea
    /// </summary>
    /// <param name="exepcion">Error ocurrido</param>
    public RespuestaBasica(Exception exepcion)
    {
      Correcto = false;
      Exepcion = exepcion;
      Mensaje = $@"Ha ocurrido un error interno, {exepcion.Message}.";
    }

    /// <summary>
    /// Recibe los parametros y los asocia a sus propiedades
    /// </summary>
    /// <param name="correcto">Indica si la respuesta ha cumplido con la tarea solicitada correctamente</param>
    public RespuestaBasica(bool correcto)
    {
      Correcto = correcto;
      Mensaje = Correcto ? @"Solicitud completada de correctamente." : @"No se ha podido completar la solicitud.";
    }

    /// <summary>
    /// Recibe los parametros y los asocia a sus propiedades
    /// </summary>
    /// <param name="correcto">Indica si la respuesta ha cumplido con la tarea solicitada correctamente</param>
    /// <param name="mensaje">Indica el estado de la tarea solicitada</param>
    public RespuestaBasica(bool correcto, string mensaje)
    {
      Correcto = correcto;
      Mensaje = mensaje;
    }
  }
}