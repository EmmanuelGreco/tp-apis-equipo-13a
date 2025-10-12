using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dominio;
using Negocio;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ArticuloController : ApiController
    {
        // GET: api/Articulo
        public IEnumerable<Articulo> Get()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            return negocio.listar();
        }

        // GET: api/Articulo/5
        public Articulo Get(int id)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            List<Articulo> lista = negocio.listar();

            return lista.Find(x=> x.Id == id);
        }

        // POST: api/Articulo
        public void Post([FromBody]ArticuloDTO articulo)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo nuevo = new Articulo();

            nuevo.Codigo = articulo.Codigo;
            nuevo.Nombre = articulo.Nombre;
            nuevo.Descripcion = articulo.Descripcion;
            nuevo.Marca = new Marca { Id = articulo.IdMarca };
            nuevo.Categoria = new Categoria { Id =  articulo.IdCategoria };
            nuevo.Precio = articulo.Precio;

            negocio.agregar(nuevo);
        }

        // PUT: api/Articulo/5
        public void Put(int id, [FromBody] ArticuloDTO articulo)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo existente = new Articulo();

            existente.Id = id;
            existente.Codigo = articulo.Codigo;
            existente.Nombre = articulo.Nombre;
            existente.Descripcion = articulo.Descripcion;
            existente.Marca = new Marca { Id = articulo.IdMarca };
            existente.Categoria = new Categoria { Id = articulo.IdCategoria };
            existente.Precio = articulo.Precio;

            negocio.modificar(existente);
        }

        // DELETE: api/Articulo/5
        public void Delete(int id)
        {
        }
    }
}
