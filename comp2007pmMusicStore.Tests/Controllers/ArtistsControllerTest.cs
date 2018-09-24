using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2007pmMusicStore.Controllers;
using Moq;
using comp2007pmMusicStore.Models;
using System.Linq;
using System.Web.Mvc;

namespace comp2007pmMusicStore.Tests.Controllers
{
    [TestClass]
    public class ArtistsControllerTest
    {
        ArtistsController controller;
        Mock<IMockArtistsRepository> mock;
        List<Artist> artists;

        [TestInitialize]
        public void TestInitialize()
        {
            // runs automatically before each unit test
            // instantiate the mock object
            mock = new Mock<IMockArtistsRepository>();

            // instantiate the mock artst data
            artists = new List<Artist>
            {
                new Artist { ArtistId = 1, Name = "Artist 1" },
                new Artist { ArtistId = 2, Name = "Artist 2" },
                new Artist { ArtistId = 3, Name = "Artist 3" }
            };

            // bind the data to the mock
            mock.Setup(m => m.Artists).Returns(artists.AsQueryable());

            // initialize the controller and inject the dependency
            controller = new ArtistsController(mock.Object);
        }

        [TestMethod]
        public void IndexViewLoads()
        {
            // act
            var actual = controller.Index();

            // assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void IndexLoadsArtists()
        {
            // act - cast ActionResult to ViewResut, then Model to List of Artists
            var actual = (List<Artist>)((ViewResult)controller.Index()).Model;

            // assert
            CollectionAssert.AreEqual(artists, actual);
        }

        [TestMethod]
        public void DetailsValidArtistId()
        {
            // act - valid id's: 1 / 2 / 3
            var actual = (Artist)((ViewResult)controller.Details(1)).Model;

            // assert
            Assert.AreEqual(artists[0], actual);
        }

        [TestMethod]
        public void DetailsInvalidArtistId()
        {
            // act - expect the Error view to load if no matching artist
            var actual = (ViewResult)controller.Details(4);

            // assert
            Assert.AreEqual("Error", actual.ViewName);

        }

        [TestMethod]
        public void DetailsNoArtistId()
        {
            // act 
            var actual = (ViewResult)controller.Details(null);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        // GET: Edit
        [TestMethod]
        public void EditGetValidId()
        {
            // act
            var actual = ((ViewResult)controller.Edit(1)).Model;

            // assert
            Assert.AreEqual(artists[0], actual);
        }

        [TestMethod]
        public void EditGetInvalidId()
        {
            // act
            var actual = (ViewResult)controller.Edit(4);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        [TestMethod]
        public void EditGetNoId()
        {
            int? id = null;

            // act
            var actual = (ViewResult)controller.Edit(id);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        // POST: Edit
        [TestMethod]
        public void EditPostValid()
        {
            // act
            var actual = (RedirectToRouteResult)controller.Edit(artists[0]);

            // assert
            Assert.AreEqual("Index", actual.RouteValues["action"]);
        }

        [TestMethod]
        public void EditPostInvalid()
        {
            // arrange - manually set model state to invalid
            controller.ModelState.AddModelError("key", "update error");

            // act
            var actual = (ViewResult)controller.Edit(artists[0]);

            // assert
            Assert.AreEqual("Edit", actual.ViewName);
        }

        // Create
        [TestMethod]
        public void CreateViewLoads()
        {
            // act
            var actual = (ViewResult)controller.Create();

            // assert
            Assert.AreEqual("Create", actual.ViewName);
        }

        [TestMethod]
        public void CreateValid()
        {
            // arrange
            Artist a = new Artist
            {
                Name = "New Artist"
            };

            // act
            var actual = (RedirectToRouteResult)controller.Create(a);

            // assert
            Assert.AreEqual("Index", actual.RouteValues["action"]);
        }

        [TestMethod]
        public void CreateInvalid()
        {
            // arrange
            Artist a = new Artist
            {
                Name = "New Artist"
            };

            controller.ModelState.AddModelError("key", "create error");

            // act
            var actual = (ViewResult)controller.Create(a);

            // assert
            Assert.AreEqual("Create", actual.ViewName);
        }

        // DELETE
        [TestMethod]
        public void DeleteGetValidId()
        {
            // act
            var actual = ((ViewResult)controller.Delete(1)).Model;

            // assert
            Assert.AreEqual(artists[0], actual);
        }

        [TestMethod]
        public void DeleteGetInvalidId()
        {
            // act
            var actual = (ViewResult)controller.Delete(4);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        [TestMethod]
        public void DeleteGetNoId()
        {
            // act
            var actual = (ViewResult)controller.Delete(null);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        [TestMethod]
        public void DeletePostValid()
        {
            // act
            var actual = (RedirectToRouteResult)controller.DeleteConfirmed(1);

            // assert
            Assert.AreEqual("Index", actual.RouteValues["action"]);
        }
    }
}
