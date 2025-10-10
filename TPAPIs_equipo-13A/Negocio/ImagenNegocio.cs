using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace Negocio
{
    public class ImagenNegocio
    {
        public List<Imagen> listarPorIdArticulo(int IdArticulo)
        {
            List<Imagen> listaImagenes = new List<Imagen>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, IdArticulo, ImagenUrl " +
                                     "FROM IMAGENES WHERE IdArticulo = " + IdArticulo);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Imagen aux = new Imagen();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.IdArticulo = (int)datos.Lector["IdArticulo"];
                    aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];

                    listaImagenes.Add(aux);
                }
                return listaImagenes;
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

        public List<int> obtenerImagenesActuales(int idArticulo)
        {
            List<int> idsActuales = new List<int>();
            List<Imagen> listaImagenes = listarPorIdArticulo(idArticulo);

            foreach (var imagen in listaImagenes)
            {
                idsActuales.Add(imagen.Id);
            }

            return idsActuales;
        }

        public void eliminarImagenesNoUsadas(List<int> idsActuales, List<int> idsConservados)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                foreach (int idBD in idsActuales)
                {
                    if (!idsConservados.Contains(idBD))
                    {
                        datos = new AccesoDatos();
                        datos.setearConsulta("DELETE FROM IMAGENES WHERE Id = @idImagen");
                        datos.setearParametro("@idImagen", idBD);
                        datos.ejecutarAccion();
                    }
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

        public void insertarActualizarImagenes(List<Imagen> listaImagen, int idArticulo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                foreach (var imagen in listaImagen)
                {
                    datos = new AccesoDatos();
                    if (imagen.Id != 0)
                    {
                        // UPDATE imagen existente
                        datos.setearConsulta("UPDATE IMAGENES SET ImagenUrl = @imagenUrl WHERE Id = @idImagen");
                        datos.setearParametro("@idImagen", imagen.Id);
                        datos.setearParametro("@imagenUrl", imagen.ImagenUrl);
                    }
                    else
                    {
                        // INSERT nueva imagen
                        datos.setearConsulta("INSERT INTO IMAGENES (IdArticulo, ImagenUrl) VALUES (@idArticulo, @imagenUrl)");
                        datos.setearParametro("@idArticulo", idArticulo);
                        datos.setearParametro("@imagenUrl", imagen.ImagenUrl);
                    }

                    datos.ejecutarAccion();
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

        public void agregarImagenes(int idArticulo, List<Imagen> listaImagenes)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                foreach (var imagen in listaImagenes)
                {
                    datos = new AccesoDatos();
                    datos.setearConsulta("INSERT INTO Imagenes (IdArticulo, ImagenUrl) " +
                                            "VALUES (@idArticulo, @imagenUrl)");
                    datos.setearParametro("@idArticulo", idArticulo);
                    datos.setearParametro("@imagenUrl", imagen.ImagenUrl);
                    datos.ejecutarAccion();
                    datos.cerrarConexion();
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

        public void eliminarImagen(int idImagen)
        {
            eliminar("DELETE FROM IMAGENES WHERE Id = @id", idImagen);
        }

        public void eliminarConIdArticulo(int idArticulo)
        {
            eliminar("DELETE FROM IMAGENES WHERE IdArticulo = @id", idArticulo);
        }

        private void eliminar(string consultaDB, int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                if (id <= 0) return;

                datos.setearConsulta(consultaDB);
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
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
