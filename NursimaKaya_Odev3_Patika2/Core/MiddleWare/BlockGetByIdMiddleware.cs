using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MiddleWare
{
    public class BlockGetByIdMiddleware
    {
        private readonly RequestDelegate next;

        public BlockGetByIdMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if(context.Request.Path.StartsWithSegments("/Vehicle/GetById"))
            {
                context.Response.StatusCode = 403;
                return;
            }

            await next.Invoke(context);
        }
    }
}
