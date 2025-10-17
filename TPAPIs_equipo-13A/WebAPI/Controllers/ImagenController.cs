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
    public class ImagenController : ApiController
    {
        /*
        // GET: api/Imagen        
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        */

        /*
        // GET: api/Imagen/5
        public string Get(int id)
        {
            return "value";
        }
        */

        // POST: api/Imagen
        public HttpResponseMessage Post([FromBody] ImagenesArticuloDTO imagenesArticulo)
        {
            try
            {
                ImagenNegocio negocio = new ImagenNegocio();

                List<Imagen> listaImagenes = imagenesArticulo.ImagenesUrl
                    .Select(x => new Imagen { ImagenUrl = x.ImagenUrl })
                    .ToList();

                negocio.agregarImagenes(imagenesArticulo.IdArticulo, listaImagenes);
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Imagen(es) agregada(s) correctamente." });
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Ocurrió un error en el servidor.");
            }
        }

        /*
        // PUT: api/Imagen/5
        public void Put(int id, [FromBody]string value)
        {
        }
        */

        /*
        // DELETE: api/Imagen/5
        public void Delete(int id)
        {
        }
        */
    }
}
