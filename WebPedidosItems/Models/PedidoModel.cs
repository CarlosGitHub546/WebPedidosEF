using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPedidosItems.Models
{
    public class PedidoModel
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public string Persona { get; set; }
        public bool Cerrado { get; set; }
        public List<ItemPedidoModel> Items { get; set; }

        public int Total { get; set; }
    }
}