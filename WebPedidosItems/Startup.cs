using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebPedidosItems.Startup))]
namespace WebPedidosItems
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
