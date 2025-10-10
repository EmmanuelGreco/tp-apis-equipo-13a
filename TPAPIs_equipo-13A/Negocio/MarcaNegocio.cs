using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace Negocio
{
    public class MarcaNegocio
    {
        public List<Marca> listar()
        {
            List<Marca> lista = new List<Marca>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Descripcion FROM MARCAS");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Marca aux = new Marca();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(string descripcion)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"INSERT INTO MARCAS (Descripcion) 
                                       VALUES (@descripcion);");
                datos.setearParametro("@descripcion", descripcion);
                datos.ejecutarLectura();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(int id, string descripcion)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"UPDATE MARCAS SET Descripcion = @descripcion
                                       WHERE Id = @id;");
                datos.setearParametro("@descripcion", descripcion);
                datos.setearParametro("@id", id);
                datos.ejecutarLectura();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public bool eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT Id FROM ARTICULOS WHERE IdMarca = @idMarca");
                datos.setearParametro("@idMarca", id);
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    return false;
                }
                else
                {
                    datos.cerrarConexion();
                    datos.setearConsulta("DELETE FROM MARCAS WHERE Id = @id");
                    datos.setearParametro("@id", id);
                    datos.ejecutarAccion();
                    return true;
                } 
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public bool existe(string descripcion, int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"SELECT Id FROM MARCAS WHERE Descripcion = @descripcion
                                       COLLATE SQL_Latin1_General_CP1_CS_AS AND Id <> @Id"); 
                datos.setearParametro("@descripcion", descripcion);
                datos.setearParametro("@Id", id);
                datos.ejecutarLectura();

                return datos.Lector.Read();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
