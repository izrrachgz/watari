namespace Watari.Enumerados
{
  /// <summary>
  /// Tipos de rutinas sql
  /// </summary>
  public enum TipoRutina
  {
    /// <summary>
    /// Indica que la rutina es un procedimiento almacenado
    /// </summary>
    Procedimiento = 0,

    /// <summary>
    /// Indica que la rutina es una funcion escalar
    /// </summary>
    FuncionEscalar = 1,

    /// <summary>
    /// Indica que la rutina es una funcion de tabla
    /// </summary>
    FuncionTabla = 2
  }
}
