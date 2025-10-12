using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace Negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> listaArticulos = new List<Articulo>();
            ImagenNegocio imagenNegocio = new ImagenNegocio();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT A.Id, Codigo, Nombre, A.Descripcion, M.Descripcion Marca, " +
                                     "C.Descripcion Categoria, A.IdMarca, A.IdCategoria, " +
                                     "Precio FROM ARTICULOS A, MARCAS M, CATEGORIAS C " +
                                     "WHERE M.Id = A.IdMarca AND C.Id = A.IdCategoria");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                    aux.Precio = (decimal)datos.Lector["Precio"];

                    // OPCIONAL: (redondea los decimales a 2 nums despues de la coma)
                    aux.Precio = Math.Round(aux.Precio, 2, MidpointRounding.AwayFromZero);

                    //Agregar lista de imagenes
                    List<Imagen> listaImagen = new List<Imagen>();
                    aux.ListaImagen = imagenNegocio.listarPorIdArticulo(aux.Id);

                    listaArticulos.Add(aux);
                }
                return listaArticulos;
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

        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            AccesoDatos datosImagen = new AccesoDatos();

            try
            {
                // Actualizo con SCOPE_IDENTITY() visto en clase
                // Investigacion del uso del @ antes de la consulta,
                // para concatenar sin usar "+" en salto de linea
                datos.setearConsulta(@"INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, 
                                                              IdMarca, IdCategoria, Precio) 

                                       VALUES (@codigo, @nombre, @descripcion, 
                                               @idMarca, @idCategoria, @precio);

                                       SELECT SCOPE_IDENTITY() AS NuevoID;");
                datos.setearParametro("@codigo", nuevo.Codigo);
                datos.setearParametro("@nombre", nuevo.Nombre);
                datos.setearParametro("@descripcion", nuevo.Descripcion);
                datos.setearParametro("@idMarca", nuevo.Marca.Id);
                datos.setearParametro("@idCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@precio", nuevo.Precio);

                int idArticulo = 0;                   
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    idArticulo = Convert.ToInt32(datos.Lector[0]);
                }
                datos.cerrarConexion();

                if (nuevo.ListaImagen != null && nuevo.ListaImagen.Count > 0)
                {
                    ImagenNegocio imagenNegocio = new ImagenNegocio();
                    imagenNegocio.agregarImagenes(idArticulo, nuevo.ListaImagen);
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

        public void modificar(Articulo existente)
        {
            AccesoDatos datos = new AccesoDatos();
            List<Articulo> listaArticulos = new List<Articulo>();
            ImagenNegocio imagenNegocio = new ImagenNegocio();

            try
            {
                // Investigacion del uso del @ antes de la consulta,
                // para concatenar sin usar "+" en salto de linea
                datos.setearConsulta(@"UPDATE ARTICULOS SET Codigo = @codigo, Nombre = @nombre, 
                                            Descripcion = @descripcion, IdMarca = @idmarca, 
                                            IdCategoria = @idcategoria, Precio = @precio
                                       WHERE Id = @id");
                datos.setearParametro("@codigo", existente.Codigo);
                datos.setearParametro("@nombre", existente.Nombre);
                datos.setearParametro("@descripcion", existente.Descripcion);
                datos.setearParametro("@idmarca", existente.Marca.Id);
                datos.setearParametro("@idcategoria", existente.Categoria.Id);
                datos.setearParametro("@precio", existente.Precio);
                datos.setearParametro("@id", existente.Id);
                datos.ejecutarAccion();

                // ACCIONES VIEJAS, DEL TP ANTERIOR.
                //datos.cerrarConexion();

                //// Lista de los IDS Actuales
                //List<int> idsActuales = imagenNegocio.obtenerImagenesActuales(existente.Id);

                //// Lista de los IDS que se quieren conservar
                //List<int> idsConservados = new List<int>();
                //foreach (var imagen in existente.ListaImagen)
                //{
                //    if (imagen.Id != 0)
                //        idsConservados.Add(imagen.Id);
                //}

                //// Elimino las que ya no se usan
                //imagenNegocio.eliminarImagenesNoUsadas(idsActuales, idsConservados);

                //// INSERT / UPDATE de las imagenes restantes
                //imagenNegocio.insertarActualizarImagenes(existente.ListaImagen, existente.Id);
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

        public void eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("DELETE FROM ARTICULOS WHERE Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
                datos.cerrarConexion();
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

        public bool existeCodigo(string codigo, int idArticulo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                // Fuerzo Case Sensitive
                datos.setearConsulta(@"SELECT Id FROM Articulos WHERE Codigo = @Codigo 
                                       COLLATE SQL_Latin1_General_CP1_CS_AS AND Id <> @Id");
                datos.setearParametro("@Codigo", codigo);
                datos.setearParametro("@Id", idArticulo);
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

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> listaArticulos = new List<Articulo>();
            ImagenNegocio imagenNegocio = new ImagenNegocio();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "SELECT A.Id, Codigo, Nombre, A.Descripcion, M.Descripcion Marca, " +
                                     "C.Descripcion Categoria, A.IdMarca, A.IdCategoria, " +
                                     "Precio FROM ARTICULOS A, MARCAS M, CATEGORIAS C " +
                                     "WHERE M.Id = A.IdMarca AND C.Id = A.IdCategoria AND ";
                if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre LIKE '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "Nombre LIKE '%" + filtro + "'";
                            break;
                        default: // Contiene
                            consulta += "Nombre LIKE '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    filtro = filtro.Trim().Replace(',', '.');
                    decimal precio = decimal.Parse(filtro, CultureInfo.InvariantCulture);
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Precio > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "Precio < " + filtro;
                            break;
                        default: // Igual a
                            consulta += "Precio = " + filtro;
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                    aux.Precio = (decimal)datos.Lector["Precio"];

                    // OPCIONAL: (redondea los decimales a 2 nums despues de la coma)
                    aux.Precio = Math.Round(aux.Precio, 2, MidpointRounding.AwayFromZero);

                    //Agregar lista de imagenes
                    List<Imagen> listaImagen = new List<Imagen>();
                    aux.ListaImagen = imagenNegocio.listarPorIdArticulo(aux.Id);

                    listaArticulos.Add(aux);
                }

                return listaArticulos;
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
