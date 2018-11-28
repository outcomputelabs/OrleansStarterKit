using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Api.Controllers
{
    public class LobbyController : Controller
    {
        private readonly IClusterClient _client;

        public LobbyController(IClusterClient client)
        {
            _client = client;
        }

        // GET: Lobby
        public ActionResult Index()
        {
            return View();
        }

        // GET: Lobby/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Lobby/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Lobby/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Lobby/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Lobby/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Lobby/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Lobby/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}