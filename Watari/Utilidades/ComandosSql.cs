using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Watari.Extensiones;
using Watari.Modelos;

namespace Watari.Utilidades
{
  /// <summary>
  /// Provee el mecanismo para ejecutar
  /// comandos sql
  /// </summary>
  public class ComandoSql
  {
    /// <summary>
    /// Cadena de conexion al repositorio de datos
    /// </summary>
    private string CadenaDeConexion { get; }

    public ComandoSql(string cadenaDeConexion)
    {
      CadenaDeConexion = cadenaDeConexion;
    }

    /// <summary>
    /// Ejecuta una instruccion Sql
    /// </summary>
    /// <param name="sql">Instruccion de consulta : sql plano, nombre de procedimiento almacenado</param>
    /// <param name="parametros">Parametros para agregar al comando</param>
    /// <param name="tipo">Tipo de instruccion</param>
    /// <param name="tiempoDeEspera">Tiempo para esperar el termino de ejecucion</param>
    /// <returns>Listado de resultado de filas</returns>
    private async Task<RespuestaColeccion<FilaDeTabla>> Sql(string sql, SqlParameter[] parametros = null, CommandType tipo = CommandType.Text, short tiempoDeEspera = 30)
    {
      //Verificar consulta
      if (sql.NoEsValida())
        return new RespuestaColeccion<FilaDeTabla>() { Correcto = false, Mensaje = @"La instruccion sql no es valida." };
      RespuestaColeccion<FilaDeTabla> respuesta;
      using (SqlConnection conexion = new SqlConnection(CadenaDeConexion))
      {
        try
        {
          //Establecer la conexion con el repositorio de datos
          await conexion.OpenAsync();
          using (SqlCommand comando = new SqlCommand(sql, conexion))
          {
            //Establecer el tipo de comando que ejecutaremos
            comando.CommandType = tipo;
            //Establecer el tiempo de espera de la terminacion del comando
            comando.CommandTimeout = tiempoDeEspera;
            //Agregar los parametros proporcionados
            if (parametros != null && parametros.Length > 0)
              comando.Parameters.AddRange(parametros);
            //Leer los resultados
            using (SqlDataReader lector = await comando.ExecuteReaderAsync())
            {
              //Si hay al menos 1 resultado agregarlo a la lista de respuesta
              if (lector.HasRows)
              {
                List<FilaDeTabla> lista = new List<FilaDeTabla>();
                int indice = 0;
                while (await lector.ReadAsync())
                {
                  List<ColumnaDeFila> columnas = new List<ColumnaDeFila>(lector.FieldCount);
                  //Agregar cada valor de columna a la fila
                  for (int c = 0; c < lector.FieldCount; c++)
                    columnas.Add(new ColumnaDeFila(c, lector.GetName(c), lector.GetValue(c)));
                  lista.Add(new FilaDeTabla(indice, columnas));
                  indice++;
                }
                respuesta = new RespuestaColeccion<FilaDeTabla>(lista);
              }
              else
              {
                //Falso positivo, el servidor ha completado la tarea correctamente pero no ha devuelto ningun resultado
                respuesta = new RespuestaColeccion<FilaDeTabla>()
                {
                  Correcto = false,
                  Mensaje = @"La ejecucion del comando no ha devuelto ningun resultado."
                };
              }
              lector.Dispose();
            }
            comando.Dispose();
          }
          //Cerrar la conexion establecida
          if (conexion.State.Equals(ConnectionState.Open)) conexion.Close();
        }
        catch (Exception ex)
        {
          respuesta = new RespuestaColeccion<FilaDeTabla>(ex);
        }
        conexion.Dispose();
      }
      return respuesta;
    }

    /// <summary>
    /// Ejecuta un comando sql de tipo texto plano
    /// </summary>
    /// <param name="sql">Texto plano en formato sql para ejecutar</param>
    /// <param name="parametros">Parametros para agregar al comando</param>
    /// <param name="tiempoDeEspera">Tiempo para esperar el termino de ejecucion</param>
    /// <returns>Listado de resultado de filas</returns>
    public async Task<RespuestaColeccion<FilaDeTabla>> Consulta(string sql, SqlParameter[] parametros = null, short tiempoDeEspera = 30)
      => await Sql(sql, parametros, CommandType.Text, tiempoDeEspera);

    /// <summary>
    /// Ejecuta un procedimiento almacenado
    /// </summary>
    /// <param name="procedimiento">Nombre del procedimiento</param>
    /// <param name="parametros">Parametros para agregar al comando</param>
    /// <param name="tiempoDeEspera">Tiempo para esperar el termino de ejecucion</param>
    /// <returns>Listado de resultado de filas</returns>
    public async Task<RespuestaColeccion<FilaDeTabla>> Procedimiento(string procedimiento, SqlParameter[] parametros = null, short tiempoDeEspera = 30)
      => await Sql(procedimiento, parametros, CommandType.StoredProcedure, tiempoDeEspera);
  }
}
