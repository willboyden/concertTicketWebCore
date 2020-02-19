using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace concertTicketWebCore.Controllers
{
    public class DataExplorationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}