using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Html;

namespace FreneticDocs.Models
{
    public class DocsInit
    {
        public HttpRequest Request;

        public HttpResponse Response;

        public DocsInit(HttpRequest htr, HttpResponse hres)
        {
            Request = htr;
            Response = hres;
        }
    }
}
