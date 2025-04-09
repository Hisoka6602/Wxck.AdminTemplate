using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.CrossCutting.Utils {

    public static class HttpContextHelper {
        public static IHttpContextAccessor? HttpContextAccessor { get; set; }

        public static HttpContext? Current => HttpContextAccessor?.HttpContext;
    }
}