using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly ShoppingContext _context;

        private readonly string vnp_Returnurl = "https://localhost:44326/CheckOut/CheckOutReturn";
        private readonly string vnp_Url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly string vnp_TmnCode = "UDOPNWS1";
        private readonly string vnp_HashSecret = "EBAHADUGCOEWYXCMYZRMTMLSHGKNRPBN";

        public CheckOutController(ShoppingContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var User = HttpContext.Session.GetString("User");
            if (User != null)
            {
                var userSession = JsonConvert.DeserializeObject<UserSession>(User);
                var userForDB = await _context.Users.FirstOrDefaultAsync(m => m.Email == userSession.Email);
                if (userForDB != null)
                {
                    return View(userForDB);
                }
            }
            return Redirect("~/Users/Login");
        }
        [HttpPost]
        public async Task<IActionResult> EditAddress(User user)
        {

            var User = HttpContext.Session.GetString("User");
            if (User != null)
            {
                var userSession = JsonConvert.DeserializeObject<UserSession>(User);
                var userForDB = await _context.Users.FindAsync(userSession.Id);
                if (userForDB != null)
                {
                    userForDB.Address = user.Address;
                    userForDB.Ward = user.Ward;
                    userForDB.District = user.District;
                    userForDB.City = user.City;

                    _context.Users.Update(userForDB);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return Redirect("~/Users/Login");
        }
        public async Task<IActionResult> Confirm()
        {
            var user = HttpContext.Session.GetString("User");
            var cart = HttpContext.Session.GetString("cart");
            DateTime createDate = DateTime.Now;
            if (user == null || cart == null)
            {
                return Redirect("~/Home");
            }

            if (user != null && cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                User userInfo = JsonConvert.DeserializeObject<User>(user);
                var getUser = _context.Users.FirstOrDefault(x => x.Email == userInfo.Email);
                float TotalPayment = 0;
                foreach (var item in dataCart)
                {
                    if (item.product.DiscountId != null)
                    {
                        if (item.product.Discount.StartDate < DateTime.Now && DateTime.Now < item.product.Discount.EndDate)
                        {
                            TotalPayment += ((float)(item.product.Price - (item.product.Price * item.product.Discount.DiscountPercent) / 100)) * (int)item.Quantity;
                        }
                        else
                        {
                            TotalPayment += (float)item.product.Price * (int)item.Quantity;
                        }
                    }
                    else
                    {
                        TotalPayment += (float)item.product.Price * (int)item.Quantity;
                    }
                }
                
                Order newOrder = new Order()
                {
                    UserId = getUser.UserId,
                    Total = TotalPayment,
                    Status = 0,
                    CreateAt = createDate,
                    Note = null,
                };
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();
                var order = _context.Orders
                    .Where(x => x.CreateAt == createDate)
                    .Where(x => x.UserId == getUser.UserId)
                    .FirstOrDefault(x => x.UserId == getUser.UserId);
                foreach (var item in dataCart)
                {
                    float totalPriceProduct = 0;
                    if (item.product.DiscountId != null)
                    {
                        if (item.product.Discount.StartDate < DateTime.Now && DateTime.Now < item.product.Discount.EndDate)
                        {
                            totalPriceProduct = ((float)(item.product.Price - (item.product.Price * item.product.Discount.DiscountPercent) / 100)) * (int)item.Quantity;
                        }
                        else
                        {
                            totalPriceProduct = (float)item.product.Price * (int)item.Quantity;
                        }
                    }
                    else
                    {
                        totalPriceProduct = (float)item.product.Price * (int)item.Quantity;
                    }
                    DetailOrder orderDetails = new DetailOrder()
                    {
                        OrderId = order.OrderId,
                        ProductId = item.product.ProductId,
                        Quantity = item.Quantity,
                        Price = totalPriceProduct,
                    };
                    var discounts = _context.Discounts.FirstOrDefault(x => x.ManufacturerId == item.product.ManufacturerId);
                    if (discounts != null)
                    {
                        discounts.AmountOf -= item.Quantity;
                        discounts.Used += item.Quantity;
                        _context.Discounts.Update(discounts);
                    }
                    var warehouse = _context.WareHouses.FirstOrDefault(x => x.ProductId == item.product.ProductId);
                    warehouse.QuantityImported -= item.Quantity;
                    warehouse.QuantitySold += item.Quantity;

                    _context.WareHouses.Update(warehouse);
                    _context.DetailOrders.Add(orderDetails);
                }
                await _context.SaveChangesAsync();

                VnPayLibrary vnpay = new VnPayLibrary();

                vnpay.AddRequestData("vnp_Version", "2.0.0");
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);

                vnpay.AddRequestData("vnp_Locale", "vn");

                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh Toan don hang "+ order.OrderId.ToString());
                vnpay.AddRequestData("vnp_OrderType", "110000");
                vnpay.AddRequestData("vnp_Amount", TotalPayment.ToString() + "00"); //Tổng giá sản phẩm 
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                vnpay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));


                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                return Redirect(paymentUrl);
            }
            return Redirect("~/Cart");
        }
        public async Task<IActionResult> CheckOutReturn()
        {
            var vnpayData = HttpContext.Request.Query.ToList();
            if (HttpContext.Request.Query.ToList().Count > 0)
            {
                //var vnpayData = HttpContext.Request.Query.ToList();
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (var s in vnpayData)
                {
                    var value = s.Value;
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s.Value))
                    {
                        vnpay.AddResponseData(s.Key, s.Value);
                    }
                }

                //Ma hon hang
                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                String vnp_SecureHash = HttpContext.Request.Query["vnp_SecureHash"];
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    { //Thanh Cong
                      //Delete Session
                        var orders = _context.Orders.FirstOrDefault(x => x.OrderId == orderId);
                        orders.Status = 1;
                        _context.Orders.Update(orders);
                        HttpContext.Session.Remove("cart");
                        return View(0);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        TempData["Rs"] = "Thanh tóan thất bại. Mã lỗi" + vnp_ResponseCode;
                        //displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                        return View(1);
                    }
                }
                else
                {
                    //log.InfoFormat("Invalid signature, InputData={0}", Request.RawUrl);
                    //displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý";
                    return View(1);
                }
            }
            //return RedirectToAction(nameof(CheckOutReturn));
            return View(1);
        }
        //public async Task<IActionResult> CheckOutReturn()
        //{
        //    return View();
        //}
    }

}
