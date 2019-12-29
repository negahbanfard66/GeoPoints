using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GeographicalPointsProject.Controllers.Base
{
    public class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;
        protected BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }

        [NonAction]
        public void LogError(Exception exception)
        {
            try
            {
                _logger.LogError(exception.Message, exception);
            }
            catch (Exception ex)
            {
                throw new Exception("NLog Error: " + ex.Message);
            }
        }
    }
}
