using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class ImagenesArticuloDTO
    {
        public int IdArticulo { get; set; }
        public List<ImagenDTO> ImagenesUrl { get; set; }
    }
}