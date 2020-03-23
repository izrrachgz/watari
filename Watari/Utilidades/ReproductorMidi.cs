using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Watari.Utilidades
{
  /// <summary>
  /// Provee un mecanismo para manipular archivos multimedia
  /// midi
  /// </summary>
  public sealed class ReproductorMidi
  {
    /// <summary>
    /// Interfaz de control de medios de windows
    /// </summary>
    /// <param name="command">Instruccion a ejecutar</param>
    /// <param name="buffer">Buffer de almacenamiento</param>
    /// <param name="bufferSize">Tamaño del buffer</param>
    /// <param name="hwndCallback">Controlador de callback</param>
    /// <returns></returns>
    [DllImport("winmm.dll")]
    static extern Int32 mciSendString(string command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

    /// <summary>
    /// Indica si la instancia actual se encuentra reproduciendo
    /// un recurso
    /// </summary>
    private bool Reproduciendo { get; set; }

    /// <summary>
    /// Direccion hacia el archivo midi asociado
    /// </summary>
    private string Url { get; }

    /// <summary>
    /// Crea una nueva instancia de reproductor midi
    /// </summary>
    /// <param name="url">Direccion hacia el archivo midi asociado</param>
    public ReproductorMidi(string url)
    {
      Reproduciendo = false;
      Url = url;
    }

    /// <summary>
    /// Reproduce el archivo especificado
    /// </summary>
    /// <returns></returns>
    public int Reproducir()
    {
      if (Reproduciendo) return 0;
      Detener();
      mciSendString($@"open {Url} alias track", new StringBuilder(@""), 0, IntPtr.Zero);
      return mciSendString($@"play track wait", new StringBuilder(@""), 0, IntPtr.Zero);
    }

    /// <summary>
    /// Detiene la reproduccion actual
    /// </summary>
    public void Detener()
    {
      mciSendString(@"stop track", new StringBuilder(@""), 0, IntPtr.Zero);
      mciSendString(@"close track", new StringBuilder(@""), 0, IntPtr.Zero);
    }

    /// <summary>
    /// Cierra todos los archivos abiertos
    /// </summary>
    public void CerrarTodo() => mciSendString(@"close all", new StringBuilder(@""), 0, IntPtr.Zero);
  }
}
