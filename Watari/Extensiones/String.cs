using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Watari.Extensiones
{
  /// <summary>
  /// Provee metodos de extensión para cadenas de caracteres
  /// </summary>
  public static class ExtensionesDeCadenas
  {
    /// <summary>
    /// Indica si una cadena es nula o al unirla sin espacios esta vacia
    /// </summary>
    /// <param name="caracteres">Cadena para comprobar</param>
    /// <returns>Verdadero o falso</returns>
    public static bool NoEsValida(this string caracteres)
    {
      return caracteres == null || caracteres.Trim().Length.Equals(0);
    }

    /// <summary>
    /// Indica si una cadena de la coleccón es nula o al unirla sin espacios esta vacia
    /// </summary>
    /// <param name="cadenas">Cadenas para comprobar</param>
    /// <returns>Verdadero o falso</returns>
    public static bool NoEsValida(this string[] cadenas)
    {
      return cadenas.Any(c => c.NoEsValida());
    }

    /// <summary>
    /// Indica si el formato de una cadena es un numero valido
    /// </summary>
    /// <param name="cadena">Cadena para comprobar</param>
    /// <returns>Verdadero o falso</returns>
    public static bool EsNumero(this string cadena)
    {
      return !cadena.NoEsValida() && new Regex(@"^\d").IsMatch(cadena);
    }

    /// <summary>
    /// Indica si el formato de una cadena es un numero telefonico celular valido
    /// </summary>
    /// <param name="cadena">Cadena para comprobar</param>
    /// <returns>Verdadero o falso</returns>
    public static bool EsCelular(this string cadena)
    {
      return !cadena.NoEsValida() && new Regex(@"^[0-9]{10,12}$").IsMatch(cadena);
    }

    /// <summary>
    /// Indica si el formato de una cadena es un correo electronico valido
    /// </summary>
    /// <param name="cadena">Cadena para comprobar</param>
    /// <returns>Verdadero o falso</returns>
    public static bool EsCorreo(this string cadena)
    {
      if (cadena.NoEsValida()) return false;
      try
      {
        MailAddress correo = new MailAddress(cadena);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Indica si el formato de una cadena es una direccion de directorio valida
    /// </summary>
    /// <param name="cadena">Cadena para comprobar</param>
    /// <returns>Verdadero o falso</returns>
    public static bool EsDireccionDeDirectorio(this string cadena)
    {
      return !cadena.NoEsValida() && Directory.Exists(cadena);
    }

    /// <summary>
    /// Indica si el formato de una cadena es una direccion de archivo valida
    /// </summary>
    /// <param name="cadena">Cadena para comprobar</param>
    /// <returns>Verdadero o falso</returns>
    public static bool EsDireccionDeArchivo(this string cadena)
    {
      if (cadena.NoEsValida()) return false;
      bool resultado;
      try
      {
        resultado = new Uri(cadena).IsFile;
      }
      catch (Exception)
      {
        resultado = false;
      }
      return resultado;
    }

    /// <summary>
    /// Indica si el formato de una cadena es una direccion web valida
    /// </summary>
    /// <param name="cadena">Cadena para comprobar</param>
    /// <returns>Verdadero o falso</returns>
    public static bool EsDireccionWeb(this string cadena)
    {
      if (cadena.NoEsValida()) return false;
      bool resultado;
      try
      {
        resultado = new Uri(cadena).IsWellFormedOriginalString();
      }
      catch (Exception)
      {
        resultado = false;
      }
      return resultado;
    }
  }
}