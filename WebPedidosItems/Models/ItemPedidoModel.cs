using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPedidosItems.Models
{
    public class ItemPedidoModel
    {
        public int CodigoPedido { get; set; }
        public int CodigoItem { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public int Precio { get; set; }
    }
}