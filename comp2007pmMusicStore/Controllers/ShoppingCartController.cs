using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using comp2007pmMusicStore.Models;
using Stripe;
using comp2007pmMusicStore.Code;

namespace comp2007pmMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        // db 
        private MusicStoreModel db = new MusicStoreModel();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            // check / generate CartId
            GetCartId();

            MigrateCart();
            string CurrentCartId = Session["CartId"].ToString();

            // get current cart contents
            var cartItems = db.Carts.Where(c => c.CartId == CurrentCartId);

            // calc cart Total.  ? indicates value may be null
            decimal? CartTotal = (from c in cartItems
                                 select (int?)c.Count * c.Album.Price).Sum();
            ViewBag.CartTotal = CartTotal;
            
            return View(cartItems);
        }

        // GET: /addtocart/5
        public ActionResult AddToCart(int AlbumId)
        {
            // check / generate CartId
            GetCartId();
            string CurrentCartId = Session["CartId"].ToString();

            // check if this album is already in this cart
            var cartItem = db.Carts.SingleOrDefault(c => c.AlbumId == AlbumId
                && c.CartId == CurrentCartId);

            if (cartItem == null) { 
                // use the Cart model to add the album to the user's cart
                cartItem = new Cart
                {
                    AlbumId = AlbumId,
                    CartId = CurrentCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };

                db.Carts.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }
            db.SaveChanges(); // commit insert or update

            // show the cart page
            return RedirectToAction("Index");
        }

        public void GetCartId()
        {
            if (Session["CartId"] == null)
            {
                // is user logged in?
                if (User.Identity.Name == "")
                {
                    // generate unique id - session-specific
                    Session["CartId"] = Guid.NewGuid().ToString();
                }
                else
                {
                    // use the username as the cartId (persists across sessions)
                    Session["CartId"] = User.Identity.Name;
                }
            }
        }

        // GET: /removefromcart/5
        public ActionResult RemoveFromCart(int AlbumId)
        {
            // get current Cart
            string CurrentCartId = Session["CartId"].ToString();

            // get selected album from current cart
            var cartItem = db.Carts.SingleOrDefault(c => c.CartId == CurrentCartId
                && c.AlbumId == AlbumId);

            // delete
            db.Carts.Remove(cartItem);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        // GET: /checkout
        public ActionResult Checkout()
        {
            MigrateCart();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: /checkout
        public ActionResult Checkout(FormCollection values)
        {
            // create new order and populate from form post values
            Order order = new Order();
            TryUpdateModel(order);

            // auto-fill the appropriate values
            order.Username = User.Identity.Name;
            order.Email = User.Identity.Name;
            order.OrderDate = DateTime.Now;

            // calc & populate total based on the cart
            // get current cart contents
            var cartItems = db.Carts.Where(c => c.CartId == order.Email);

            // calc cart Total.  ? indicates value may be null
            decimal CartTotal = (from c in cartItems
                                  select (int)c.Count * c.Album.Price).Sum();
            order.Total = CartTotal;

            // store the order in a session variable
            Session["Order"] = order;
            return RedirectToAction("Payment");
        }

        [Authorize]
        public ActionResult Payment()
        {
            ViewBag.StripePublishableKey = ConfigurationManager.AppSettings["StripePublishableKey"];
            Order order = Session["Order"] as Order;

            ViewBag.Total = order.Total;
            ViewBag.CentsTotal = order.Total * 100;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(string stripeEmail, string stripeToken)
        {
            // get order from session
            Order order = Session["Order"] as Order;

            // get Stripe Secret Key from web.config
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["StripeSecretKey"]);

            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = Convert.ToInt16(order.Total * 100),
                Description = "MVC Music Store Purchase",
                Currency = "cad",
                CustomerId = customer.Id
            });

            // save order
            db.Orders.Add(order);
            db.SaveChanges();

            // save details
            var CartItems = db.Carts.Where(c => c.CartId == order.Username);
            foreach (Cart Item in CartItems)
            {
                var OrderDetail = new OrderDetail();
                OrderDetail.OrderId = order.OrderId;
                OrderDetail.AlbumId = Item.AlbumId;
                OrderDetail.Quantity = Item.Count;
                OrderDetail.UnitPrice = Item.Album.Price;
                db.OrderDetails.Add(OrderDetail);
            }
            db.SaveChanges();

            // empty cart
            foreach (Cart Item in CartItems)
            {
                db.Carts.Remove(Item);
            }
            db.SaveChanges();
            Session["CartId"] = null;
            Session["Order"] = null;

            // Show receipt
            return RedirectToAction("Details", "Orders", new { id = HttpUtility.UrlEncode(Utilities.Encrypt(order.OrderId.ToString())) });
        }

        public void MigrateCart()
        {
            if (!String.IsNullOrEmpty(Session["CartId"].ToString()) && User.Identity.IsAuthenticated)
            {
                if (Session["CartId"].ToString() != User.Identity.Name)
                {
                    // get cart items with current random id
                    string CurrentCartId = Session["CartId"].ToString();
                    var CartItems = db.Carts.Where(c => c.CartId == CurrentCartId);

                    foreach (Cart Item in CartItems)
                    {
                        // update each item to attach it to the username
                        Item.CartId = User.Identity.Name;
                    }
                    db.SaveChanges();

                    // update the session Cart Id to the User's email
                    Session["CartId"] = User.Identity.Name;
                }
            }
        }
    }
}