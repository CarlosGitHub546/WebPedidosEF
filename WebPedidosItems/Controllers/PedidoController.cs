using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPedidosItems.Models;
using WebPedidosItems.Services;

namespace WebPedidosItems.Controllers
{
    public class PedidoController : Controller
    {
        // GET: Pedido
        public ActionResult Index()
        {
            PedidosService servicio = new PedidosService();
            List<PedidoModel> listaPedidos = servicio.GetPedidos();

            int totalPedidos = 0;

            if (listaPedidos != null)
            {
                totalPedidos = listaPedidos.Sum(x => x.Total);
            }

            ViewBag.TotalPedidos = totalPedidos;

            //PersonaTotal personaTotal = getMaxPersonaTotal();


            return View(listaPedidos);
        }

        public ActionResult Create()
        {
            PedidoModel pedido = new PedidoModel();

            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PedidoModel pedido)
        {
            if (ModelState.IsValid)
            {
                PedidosService servicio = new PedidosService();
                servicio.CreatePedido(pedido);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            PedidosService servicio = new PedidosService();
            PedidoModel pedido = servicio.GetPedido(id);

            return View(pedido);
        }

        public ActionResult Preview(int id)
        {
            PedidosService servicio = new PedidosService();
            PedidoModel pedido = servicio.GetPedido(id);

            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PedidoModel pedido)
        {
            if (ModelState.IsValid)
            {
                PedidosService servicio = new PedidosService();
                servicio.EditPedido(pedido);
            }
            return RedirectToAction("Index");
        }

        public ActionResult AddItem(ItemPedidoModel itemPedido)
        {
            PedidosService servicio = new PedidosService();
            servicio.AddItem(itemPedido);
            return Json(true);
        }

        public ActionResult FinalizarPedido(int id)
        {
            PedidosService servicio = new PedidosService();
            servicio.FinalizarPedido(id);
            return Json(true);
        }
      
    }
}