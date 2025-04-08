using Business.Exceptions;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackEndMicroondas.Controllers
{
    public class MicrowaveController : Controller
    {
        private readonly IMicrowaveService _microwaveService;

        public MicrowaveController(IMicrowaveService microwaveService)
        {
            _microwaveService = microwaveService;
            _microwaveService.OnHeatingUpdate += (display) =>
            {
            };
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Start(int? timeSeconds, int? power)
        {
            try
            {
                // Se nenhum valor for informado, aplica início rápido: 30 segundos, potência 10
                if (timeSeconds is null && power is null)
                {
                    timeSeconds = 30;
                    power = 10;
                }
                _microwaveService.StartHeating(timeSeconds.Value, power);
                return RedirectToAction("Index");
            }
            catch (MicrowaveException ex)
            {
                ViewBag.Error = ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult PauseOrCancel()
        {
            _microwaveService.PauseOrCancel();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Resume()
        {
            _microwaveService.ResumeHeating();
            return RedirectToAction("Index");
        }
    }
}
