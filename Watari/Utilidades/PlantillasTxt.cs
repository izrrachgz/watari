using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watari.Extensiones;

namespace Watari.Utilidades
{
  public sealed class Plantilla
  {
    /// <summary>
    /// Nombre del archivo de la plantilla
    /// </summary>
    public string Nombre { get; }

    /// <summary>
    /// Extension del archivo al generar la plantilla
    /// </summary>
    public string Extension { get; }

    /// <summary>
    /// Url de acceso
    /// </summary>
    private string Direccion { get; }

    /// <summary>
    /// Contenido de la plantilla
    /// </summary>
    private string Definicion { get; set; }

    /// <summary>
    /// Contenido de la plantilla
    /// </summary>
    public string Contenido => Definicion;

    /// <summary>
    /// Permite construir una nueva instancia de un archivo de plantilla
    /// </summary>
    /// <param name="url">Direccion donde se encuentra la plantilla</param>
    /// <param name="nombre">Nombre del archivo de plantilla</param>
    /// <param name="extension">Extension del archivo a partir de la plantilla</param>
    public Plantilla(string url, string nombre, string extension)
    {
      Nombre = nombre;
      Extension = extension;
      Direccion = $@"{AppDomain.CurrentDomain.BaseDirectory}Plantillas\{url}{nombre}.txt";
    }

    /// <summary>
    /// Permite cargar el contenido de la plantilla
    /// metiante la definicion
    /// </summary>
    /// <returns></returns>
    public async Task Cargar()
    {
      //Verificar que la url sea valida
      if (!File.Exists(Direccion))
      {
        Console.WriteLine($@"La direccion para leer la plantilla {Nombre} no es valida.");
        return;
      }
      StringBuilder sb = new StringBuilder();
      using (FileStream fs = File.OpenRead(Direccion))
      {
        //Leer el flujo del archivo
        using (StreamReader sr = new StreamReader(fs))
        {
          //Agregar el contenido
          while (!sr.EndOfStream)
            sb.AppendLine(await sr.ReadLineAsync());
          sr.Dispose();
        }
        fs.Dispose();
      }
      Definicion = sb.ToString();
    }

    /// <summary>
    /// Permite generar la plantilla a partir de su definicion
    /// sustituyendo las claves por los valores dados
    /// </summary>
    /// <param name="urlDestino">Directorio de destino</param>
    /// <param name="nombre">Nombre del archivo</param>
    /// <param name="claves">Claves para reemplazar</param>
    /// <param name="valores">Valores para sustituir</param>
    /// <returns></returns>
    public async Task Generar(string urlDestino, string nombre, string[] claves, Dictionary<string, string> valores)
    {
      //Verificar los parametros
      if (!urlDestino.EsDireccionDeDirectorio() || nombre.NoEsValida() || claves.NoEsValida() || valores == null || valores.Count.Equals(0))
      {
        Console.WriteLine(@"Los parametros para generar el archivo no son validos.");
        return;
      }
      //Verificar que todas las claves se encuentren en los valores
      if (!claves.ToList().TrueForAll(valores.ContainsKey))
      {
        Console.WriteLine(@"Los valores para sustituir no coinciden.");
        return;
      }
      //Reemplazar el contenido
      string contenido = Contenido;
      foreach (string c in claves)
      {
        if (!valores.ContainsKey(c)) continue;
        contenido = contenido.Replace(c, valores[c]);
      }
      //Verificar si existe el archivo
      string directorio = $@"{urlDestino}{nombre}.{Extension}";
      if (File.Exists(directorio))
      {
        Console.WriteLine($@"Ya existe el archivo {nombre}.{Extension}, Por seguridad no se ha escrito nada.");
        return;
      }
      //Escribir el contenido de la plantilla
      Console.WriteLine($@"Escribiendo archivo {nombre}.{Extension}");
      await File.WriteAllTextAsync(directorio, contenido);
    }
  }
}
