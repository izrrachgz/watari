namespace Watari.Modelos
{
  /// <summary>
  /// Provee el modelo de datos que representa una instancia
  /// del archivo de configuracion del contexto de datos
  /// </summary>
  public class ConfiguracionContexto
  {
    /// <summary>
    /// Cadena de conexion hacia el repositorio de datos
    /// </summary>
    public string CadenaDeConexion { get; set; }

    /// <summary>
    /// Direccion hacia la carpeta de solucion del proyecto
    /// </summary>
    public string DirectorioSolucion { get; set; }
  }
}
