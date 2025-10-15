using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ArticuloController : ApiController
    {
        // GET: api/Articulo
        public HttpResponseMessage Get()
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();
                IEnumerable<Articulo> lista = negocio.listar();
                
                if (lista == null || lista.Count() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontraron artículos.");

                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Ocurrió un error en el servidor.");
            }
        }

        // GET: api/Articulo/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();
                List<Articulo> lista = negocio.listar();

                if (lista == null || lista.Count() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontraron artículos.");
                
                Articulo encontrado = lista.Find(x => x.Id == id);
                if (encontrado == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontró un artículo con este id.");
                else 
                    return Request.CreateResponse(HttpStatusCode.OK, encontrado);
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Ocurrió un error en el servidor.");
            }
        }

        // POST: api/Articulo
        public HttpResponseMessage Post([FromBody]ArticuloDTO articulo)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "IdCategoria e IdMarca deben ser números enteros. Precio debe ser un número decimal." });

                ArticuloNegocio articuloNegocio = new ArticuloNegocio();

                if(articulo.IdMarca == null|| articulo.IdCategoria == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "No se han enviado todos los campos correspondientes." });


                MarcaNegocio marcaNegocio = new MarcaNegocio();
                CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
                
                Marca marca = marcaNegocio.listar().Find(x => x.Id == articulo.IdMarca);
                Categoria categoria = categoriaNegocio.listar().Find(x => x.Id == articulo.IdCategoria);

                if (marca == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "La marca no existe." });

                if (categoria == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "La categoría no existe." });


                if(articulo.Precio < 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "El precio debe ser mayor o igual a cero." });

                Articulo nuevo = new Articulo
                {
                    Codigo =        articulo.Codigo != null         ? articulo.Codigo : "",
                    Nombre =        articulo.Nombre != null         ? articulo.Nombre : "",
                    Descripcion =   articulo.Descripcion != null    ? articulo.Descripcion : "",
                    Marca =         new Marca { Id = (int)articulo.IdMarca },
                    Categoria =     new Categoria { Id = (int)articulo.IdCategoria },
                    Precio =        articulo.Precio != null         ? (decimal)articulo.Precio : 0
                };
                
                articuloNegocio.agregar(nuevo);
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Artículo agregado correctamente." });
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Ocurrió un error en el servidor.");
            }
        }

        // PUT: api/Articulo/5
        public HttpResponseMessage Put(int id, [FromBody]ArticuloDTO articulo)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "IdCategoria e IdMarca deben ser números enteros. Precio debe ser un número decimal." });

                if (articulo.Codigo == null && articulo.Nombre == null && articulo.Descripcion == null && 
                   articulo.IdMarca == null && articulo.IdCategoria == null && articulo.Precio == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "Ningún campo a modificar." });

                ArticuloNegocio articuloNegocio = new ArticuloNegocio();
                MarcaNegocio marcaNegocio = new MarcaNegocio();
                CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
            
                List<Articulo> lista = articuloNegocio.listar();

                if (lista == null || lista.Count() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontraron artículos.");

                Articulo encontrado = lista.Find(x => x.Id == id);
                if (encontrado == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontró un artículo con este id.");


                Marca marca = marcaNegocio.listar().Find(x => x.Id == articulo.IdMarca);
                Categoria categoria = categoriaNegocio.listar().Find(x => x.Id == articulo.IdCategoria);
                if (articulo.IdMarca != null && marca == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "La marca no existe." });

                if (articulo.IdCategoria != null && categoria == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "La categoría no existe." });

                if (articulo.Precio < 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "El precio debe ser mayor o igual a cero." });

                //if ()

                Articulo modificado = new Articulo()
                {
                    Id = id,
                    Codigo =        articulo.Codigo != null         ? articulo.Codigo                                   : encontrado.Codigo,
                    Nombre =        articulo.Nombre != null         ? articulo.Nombre                                   : encontrado.Nombre,
                    Descripcion =   articulo.Descripcion != null    ? articulo.Descripcion                              : encontrado.Descripcion,
                    Marca =         articulo.IdMarca != null        ? new Marca { Id = (int)articulo.IdMarca }          : encontrado.Marca,
                    Categoria =     articulo.IdCategoria != null    ? new Categoria { Id = (int)articulo.IdCategoria }  : encontrado.Categoria,
                    Precio =        articulo.Precio != null         ? (decimal)articulo.Precio                          : encontrado.Precio
                };

                articuloNegocio.modificar(modificado);
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Artículo modificado correctamente." });
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Ocurrió un error en el servidor.");
            }
        }

        // DELETE: api/Articulo/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                ArticuloNegocio articuloNegocio = new ArticuloNegocio();
                List<Articulo> lista = articuloNegocio.listar();
            
                Articulo encontrado = lista.Find(x => x.Id == id);
                if (encontrado == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontró un artículo con este id.");
                else
                {
                    articuloNegocio.eliminar(id);
                    return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Artículo eliminado correctamente." });
                }
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Ocurrió un error en el servidor.");
            }

                
        }
    }
}
