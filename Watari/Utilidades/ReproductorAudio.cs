using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Watari.Utilidades
{
  /// <summary>
  /// Provee un mecanismo para manipular archivos multimedia
  /// de audio
  /// </summary>
  public sealed class ReproductorAudio
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
    public ReproductorAudio(string url)
    {
      Reproduciendo = false;
      Url = url;
    }

    /// <summary>
    /// Reproduce el archivo especificado y espera a que termine
    /// </summary>
    /// <param name="esperar">Especifica si hay que esperar al termino de la pista</param>
    /// <returns></returns>
    public int Reproducir(bool esperar = true)
    {
      if (Reproduciendo) return 0;
      Detener();
      string tipo = Url.Contains(@".mp3") ? @"type mpegvideo" : @"";
      mciSendString($@"open {Url} {tipo}", new StringBuilder(@""), 0, IntPtr.Zero);
      return mciSendString($@"play {Url} {(esperar ? @"wait" : @"")}", new StringBuilder(@""), 0, IntPtr.Zero);
    }

    /// <summary>
    /// Detiene la reproduccion actual
    /// </summary>
    public void Detener()
    {
      mciSendString($@"stop {Url}", new StringBuilder(@""), 0, IntPtr.Zero);
      mciSendString($@"close {Url}", new StringBuilder(@""), 0, IntPtr.Zero);
    }

    /// <summary>
    /// Cierra todos los archivos abiertos
    /// </summary>
    public void CerrarTodo() => mciSendString(@"close all", new StringBuilder(@""), 0, IntPtr.Zero);
  }
}
