using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoProject1.Models;

namespace MongoProject1.Controllers
{
    public class HomeController : Controller
    {
        //TODO: source - https://metanit.com/nosql/mongodb/4.14.php
        private readonly ComputerContext db = new ComputerContext();

        public async Task<ActionResult> Index(ComputerFilter filter)
        {
            var computers = await db.GetComputers(filter.Year, filter.ComputerName);
            var model = new ComputerList() { Computers = computers, Filter = filter};

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Computer computer)
        {
            if (ModelState.IsValid)
            {
                await db.Create(computer);

                return RedirectToAction("Index");
            }

            return View(computer);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            var computer = await db.GetComputer(id);

            if (computer == null)
            {
                return HttpNotFound();
            }

            return View(computer);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Computer computer)
        {
            if (ModelState.IsValid)
            {
                await db.Update(computer);

                return RedirectToAction("Index");
            }

            return View(computer);
        }

        public async Task<ActionResult> Delete(string id)
        {
            await db.Remove(id);

            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> AttachImage(string id)
        {
            var computer = await db.GetComputer(id);

            if (computer == null)
            {
                return HttpNotFound();
            }

            return View(computer);
        }

        [HttpPost]
        public async Task<ActionResult> AttachImage(string id, HttpPostedFileBase uploadedFile)
        {
            if (uploadedFile != null)
            {
                await db.StoreImange(id, uploadedFile.InputStream, uploadedFile.FileName);
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> GetImage(string id)
        {
            var image = await db.GetImage(id);

            if (image == null)
            {
                return HttpNotFound();
            }

            return File(image, "image/png");
        }
    }
}