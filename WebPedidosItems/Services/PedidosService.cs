using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPedidosItems.Models;
using System.Data.Entity;
using WebPedidosItems.Data;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;

namespace WebPedidosItems.Services
{
    public class PedidosService
    {
        

        public List<PedidoModel> GetPedidos()
        {
            using (DatabaseEntities bbdd = new DatabaseEntities())
            {
                var res = (from p in bbdd.Pedido
                              select new PedidoModel() {
                                        Codigo =p.Codigo,
                                        Nombre=p.Nombre,
                                        Persona=p.Persona,
                                        Cerrado=p.Cerrado
                            }).ToList();
                foreach (PedidoModel p in res)
                {
                    p.Total = GetTotal(p.Codigo);
                }
                return res;
            }
        }

        private int GetTotal(int codigoPedido)
        {
            List<ItemPedidoModel> items = GetSessionItems(codigoPedido);
            return (items.Count > 0) ? items.Sum(i=> i.Cantidad * i.Precio) : 0;

        }

        public void CreatePedido(PedidoModel pedido)
        {
            using (DatabaseEntities bbdd = new DatabaseEntities())
            {
                Pedido nuevo = new Pedido()
                {
                    Codigo = bbdd.Pedido.Max(p => p.Codigo) + 1,
                    Nombre = pedido.Nombre,
                    Persona = pedido.Persona,
                    Cerrado = pedido.Cerrado
                };
                bbdd.Pedido.Add(nuevo);
                bbdd.SaveChanges();
            }

        }

        public void EditPedido(PedidoModel pedido)
        {
            List<ItemPedidoModel> items = GetSessionItems(pedido.Codigo);
            Dictionary<int, List<ItemPedidoModel>> pedidos = ((Dictionary<int, List<ItemPedidoModel>>)System.Web.HttpContext.Current.Session["Pedidos"]);
            pedidos.Remove(pedido.Codigo);
            pedidos.Add(pedido.Codigo, items);
            System.Web.HttpContext.Current.Session["Pedidos"] = pedidos;

            using (DatabaseEntities bbdd = new DatabaseEntities())
            {
                Pedido edit = bbdd.Pedido.FirstOrDefault(p => p.Codigo == pedido.Codigo);
                if (edit != null)
                {
                    edit.Nombre = pedido.Nombre;
                    edit.Persona = pedido.Persona;
                }
                bbdd.SaveChanges();
            }
        }


        public PedidoModel GetPedido(int id)
        {

            using (DatabaseEntities bbdd = new DatabaseEntities())
            {
                PedidoModel pedido =  (from p in bbdd.Pedido
                        where p.Codigo == id
                        select new PedidoModel() {
                            Codigo = p.Codigo,
                            Nombre = p.Nombre,
                            Persona = p.Persona,
                            Cerrado = p.Cerrado
                        }).FirstOrDefault();

                if (pedido != null)
                {
                    pedido.Items = GetSessionItems(id);
                }
                return pedido;
            }

        }


        #region "Items pedido session"


        public ItemPedidoModel AddItem(ItemPedidoModel ItemPedidoModel)
        {
            
            List<ItemPedidoModel> items = GetSessionItems(ItemPedidoModel.CodigoPedido);
            ItemPedidoModel.CodigoItem = (items.Count==0?0:items.Max(i => i.CodigoItem)) + 1;
            items.Add(ItemPedidoModel);
            Dictionary<int, List<ItemPedidoModel>> pedidos = ((Dictionary<int, List<ItemPedidoModel>>)System.Web.HttpContext.Current.Session["Pedidos"]);
            pedidos.Remove(ItemPedidoModel.CodigoPedido);
            pedidos.Add(ItemPedidoModel.CodigoPedido, items);
            System.Web.HttpContext.Current.Session["Pedidos"] = pedidos;

            return ItemPedidoModel;
        }

        private List<ItemPedidoModel> GetItems(int codigoPedido)
        {
            using (DatabaseEntities bbdd = new DatabaseEntities())
            {
                var res = (from i in bbdd.ItemPedido
                        where i.CodigoPedido == codigoPedido 
                        select new ItemPedidoModel()
                        {
                            CodigoItem = i.CodigoItem,
                            CodigoPedido = i.CodigoPedido,
                            Nombre = i.Nombre,
                            Cantidad = i.Cantidad,
                            Precio = i.Precio
                        }).ToList();

                return res ?? new List<ItemPedidoModel>();
            }
        }

        private List<ItemPedidoModel> GetSessionItems(int codigoPedido)
        {
            if (System.Web.HttpContext.Current.Session["Pedidos"] == null)
            {
                System.Web.HttpContext.Current.Session["Pedidos"] = new Dictionary<int, List<ItemPedidoModel>>();
            }

            Dictionary<int, List<ItemPedidoModel>> pedidos = ((Dictionary<int, List<ItemPedidoModel>>)System.Web.HttpContext.Current.Session["Pedidos"]);
            List<ItemPedidoModel> items;
            if (pedidos.TryGetValue(codigoPedido, out items))
            {
                return items;
            }
            else
            {
                items = GetItems(codigoPedido);
                pedidos.Add(codigoPedido, items);
                System.Web.HttpContext.Current.Session["Pedidos"] = pedidos;
                
                return items;
            }
        }

        #endregion

        public void FinalizarPedido(int id)
        {
            Pedido pedido;
            using (DatabaseEntities bbdd = new DatabaseEntities())
            {
                pedido = (from p in bbdd.Pedido
                          where p.Codigo == id
                          select p).FirstOrDefault();
                if (pedido != null)
                {
                    //Update items
                    var previousItems = (from i in bbdd.ItemPedido
                               where i.CodigoPedido == id
                               select i).ToList();
                    bbdd.ItemPedido.RemoveRange(previousItems);
                    List<ItemPedidoModel> items = GetSessionItems(id);
                    foreach (ItemPedidoModel item in items)
                    {
                        bbdd.ItemPedido.Add(new ItemPedido()
                        {
                            CodigoItem = item.CodigoItem,
                            CodigoPedido = item.CodigoPedido,
                            Nombre = item.Nombre,
                            Cantidad = item.Cantidad,
                            Precio = item.Precio
                        });
                    }

                    //Marcar como cerrado
                    pedido.Cerrado = true;
                    bbdd.SaveChanges();
                }

            }
        }
    }
}