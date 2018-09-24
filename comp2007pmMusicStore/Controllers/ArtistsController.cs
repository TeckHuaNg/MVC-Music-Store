using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using comp2007pmMusicStore.Models;

namespace comp2007pmMusicStore.Controllers
{
    public class ArtistsController : Controller
    {
        // db connection moved to EFArtistsRepository
        //private MusicStoreModel db = new MusicStoreModel();
        private IMockArtistsRepository db;

        // default constructor - no dependency incoming => use the database
        public ArtistsController()
        {
            this.db = new EFArtistsRepository();
        }

        // mock constructor - mock object passed as a dependency for unit testing
        public ArtistsController(IMockArtistsRepository mockRepo)
        {
            this.db = mockRepo;
        }

        // GET: Artists
        public ActionResult Index()
        {
            return View(db.Artists.OrderBy(a => a.Name).ToList());
        }

        // GET: Artists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //return RedirectToAction("Index");
                return View("Error");
            }
            // original code for ef only
            //Artist artist = db.Artists.Find(id);

            // new code for ef or unit testing
            Artist artist = db.Artists.SingleOrDefault(a => a.ArtistId == id);

            if (artist == null)
            {
                //return HttpNotFound();
                //return RedirectToAction("Index");
                return View("Error");
            }
            return View(artist);
        }

        // GET: Artists/Create
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArtistId,Name")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                //db.Artists.Add(artist);
                //db.SaveChanges();
                db.Save(artist);
                return RedirectToAction("Index");
            }

            return View("Create", artist);
        }

        // GET: Artists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return View("Error");
            }
            //Artist artist = db.Artists.Find(id);
            Artist artist = db.Artists.SingleOrDefault(a => a.ArtistId == id);
            if (artist == null)
            {
                //return HttpNotFound();
                return View("Error");
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArtistId,Name")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(artist).State = EntityState.Modified;
                //db.SaveChanges();
                db.Save(artist);
                return RedirectToAction("Index");
            }
            return View("Edit", artist);
        }

        // GET: Artists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return View("Error");
            }
            //Artist artist = db.Artists.Find(id);
            Artist artist = db.Artists.SingleOrDefault(a => a.ArtistId == id);
            if (artist == null)
            {
                //return HttpNotFound();
                return View("Error");
            }
            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Artist artist = db.Artists.Find(id);
            Artist artist = db.Artists.SingleOrDefault(a => a.ArtistId == id);
            //db.Artists.Remove(artist);
            //db.SaveChanges();
            db.Delete(artist);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
