﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebPedidosItems.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("Index", "Pedido");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}