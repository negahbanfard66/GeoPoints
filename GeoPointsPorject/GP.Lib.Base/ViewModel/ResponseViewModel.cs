using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GP.Lib.Base.ViewModel
{
    public class ResponseViewModel<T> : ActionResult
    {
        public bool IsAuthenticated { get; set; }
        public bool IsValid { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string MessageTitle { get; set; }
        public T Result { get; set; }
    }
}
