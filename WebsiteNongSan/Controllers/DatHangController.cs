using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Controllers
{
    public class DatHangController : Controller
    {
        // GET: DatHang
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        // Lấy giỏ hàng từ Session
        public List<GioHang> GetCart()
        {
            var cart = Session["gh"] as List<GioHang>;

            if (cart == null)
            {
                // Nếu session rỗng, lấy giỏ hàng từ database dựa vào IDNguoiDung
                var userId = (NguoiDung)Session["user"];
                if (userId != null)
                {
                    cart = db.GioHangs.Where(row => row.IDNguoiDung == userId.IDNguoiDung).ToList();
                    Session["gh"] = cart;
                }
                else
                {
                    cart = new List<GioHang>();
                }
            }

            return cart;
        }

        // Thêm 1 sản phẩm vào giỏ
        //public ActionResult ThemVaoGioHang(int id)
        //{
        //    // Lấy ID người dùng từ session
        //    var userId = (NguoiDung)Session["user"];
        //    if (userId == null)
        //    {
        //        return RedirectToAction("DangNhap", "NguoiDung"); // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
        //    }

        //    // Tìm sản phẩm theo ID
        //    var sanPham = db.SanPhams.Find(id);
        //    if (sanPham == null || sanPham.SoLuongTon <= 0)
        //    {
        //        return HttpNotFound("Sản phẩm không tồn tại hoặc đã hết hàng!");
        //    }

        //    var cart = Session["gh"] as List<GioHang>;
        //    if (cart == null)
        //    {
        //        cart = new List<GioHang>();
        //    }

        //    var cartItem = cart.FirstOrDefault(c => c.IDSanPham == id);

        //    if (cartItem == null)
        //    {
        //        var newCartItem = new GioHang
        //        {
        //            IDSanPham = sanPham.IDSanPham,
        //            IDNguoiDung = userId.IDNguoiDung,
        //            SoLuong = 1,
        //            NgayTao = DateTime.Now,
        //            TrangThai = "Đang xử lý"
        //        };

        //        cart.Add(newCartItem);
        //        db.GioHangs.Add(newCartItem);
        //    }
        //    else
        //    {
        //        cartItem.SoLuong++;
        //        cartItem.TrangThai = "Đang xử lý";

        //        var dbCartItem = db.GioHangs.FirstOrDefault(g => g.IDSanPham == id && g.IDNguoiDung == userId.IDNguoiDung);
        //        if (dbCartItem != null)
        //        {
        //            dbCartItem.SoLuong = cartItem.SoLuong;
        //        }
        //    }

        //    db.SaveChanges(); // Lưu thay đổi vào database
        //    //Session["gh"] = GetCart(); // Cập nhật lại giỏ hàng trong session
        //    Session["gh"] = cart; // Cập nhật lại giỏ hàng trong session


        //    return RedirectToAction("Index", "SanPham");
        //}
        public ActionResult ThemVaoGioHang(int id)
        {
            // Lấy ID người dùng từ session
            var userId = (NguoiDung)Session["user"];
            if (userId == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung"); // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
            }

            // Tìm sản phẩm theo ID
            var sanPham = db.SanPhams.Find(id);
            if (sanPham == null || sanPham.SoLuongTon <= 0)
            {
                return HttpNotFound("Sản phẩm không tồn tại hoặc đã hết hàng!");
            }

            var cart = db.GioHangs.Where(row => row.IDNguoiDung == userId.IDNguoiDung).ToList();
            if (cart == null)
            {
                cart = new List<GioHang>();
            }

            var cartItem = cart.FirstOrDefault(c => c.IDSanPham == id);

            if (cartItem == null)
            {
                var newCartItem = new GioHang
                {
                    IDSanPham = sanPham.IDSanPham,
                    IDNguoiDung = userId.IDNguoiDung,
                    SoLuong = 1,
                    NgayTao = DateTime.Now,
                    TrangThai = "Đang xử lý"
                };

                cart.Add(newCartItem);
                db.GioHangs.Add(newCartItem);
            }
            else
            {
                cartItem.SoLuong++;
                cartItem.TrangThai = "Đang xử lý";

                var dbCartItem = db.GioHangs.FirstOrDefault(g => g.IDSanPham == id && g.IDNguoiDung == userId.IDNguoiDung);
                if (dbCartItem != null)
                {
                    dbCartItem.SoLuong = cartItem.SoLuong;
                }
            }

            db.SaveChanges(); // Lưu thay đổi vào database
            //Session["gh"] = GetCart(); // Cập nhật lại giỏ hàng trong session
            Session["gh"] = cart; // Cập nhật lại giỏ hàng trong session

            return RedirectToAction("Index", "SanPham");
        }

        // Xem giỏ hàng 
        public ActionResult XemGioHang()
        {
            //var cart = GetCart(); // Lấy giỏ hàng từ session
            var user = Session["user"] as NguoiDung; 
            if (user != null)
            {
                var cart = db.GioHangs.Where(row => row.IDNguoiDung == user.IDNguoiDung).ToList();
                ViewBag.TongTien = cart.Sum(row => row.SoLuong * row.SanPham.Gia); // Tính tổng tiền giỏ hàng
                ViewBag.User = (NguoiDung)Session["user"];
                return View(cart);
            }
            else
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
        }

        // Hiển thị thông tin khách hàng
        public ActionResult ThongTinThanhToan(int id)
        {
            var gioHangNguoiDung = db.GioHangs.Where(row => row.IDNguoiDung == id).ToList();
            ViewBag.CartList = gioHangNguoiDung;
            ViewBag.User = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == id);
            ViewBag.TongTien = gioHangNguoiDung.Sum(row=>row.SoLuong * row.SanPham.Gia);
            return View();
        }

        // Đạt hàng
        [HttpPost]
        public ActionResult DatHang(int id, FormCollection fc)
        {
            var gioHangNguoiDung = db.GioHangs.Where(row => row.IDNguoiDung == id).ToList(); // Lấy danh sách sản phẩm trong giỏ hàng của người dùng

            if (!gioHangNguoiDung.Any())
            {
                ViewBag.Error = "Giỏ hàng trống. Không thể đặt hàng!";
                return RedirectToAction("XemGioHang");
            }
           
            var tongTien = gioHangNguoiDung.Sum(row => row.SoLuong * row.SanPham.Gia);  // Tính tổng tiền đơn hàng

            // Kiểm tra ngày giao hàng
            if (!DateTime.TryParse(fc["GioGiaoHang"], out DateTime ngayGiao) || ngayGiao >= DateTime.Now)
            {
                ViewBag.Error = "Ngày giao hàng không hợp lệ hoặc không được đặt trước ngày hiện tại!";
                var user = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == id);
                return RedirectToAction("ThongTinThanhToan");
            }

            // Tạo đơn hàng
            var donHang = new DonHang
            {
                NgayDat = DateTime.Now,
                NgayGiao = ngayGiao,
                TongTien = tongTien,
                TrangThai = "Đang xử lý",
                IDNguoiDung = id,
                DiaChi = fc["DiaChi"]
            };

            db.DonHangs.Add(donHang);
            db.SaveChanges(); // Lưu để có IDDonHang

            // Tạo chi tiết đơn hàng
            foreach (var item in gioHangNguoiDung)
            {
                var chiTietDonHang = new ChiTietDonHang
                {
                    IDDonHang = donHang.IDDonHang,
                    IDSanPham = int.Parse(item.IDSanPham.ToString()),
                    SoLuong = item.SoLuong,
                    GiaBan = item.SanPham.Gia,
                    ThanhTien = item.SoLuong * item.SanPham.Gia
                };
                db.ChiTietDonHangs.Add(chiTietDonHang);
            }

            // Kiểm tra và xử lý phương thức thanh toán
            var phuongThucThanhToan = fc["inputThanhToan"];
            if (phuongThucThanhToan != "cod" && phuongThucThanhToan != "atm")
            {
                ViewBag.Error = "Phương thức thanh toán không hợp lệ!";
                return RedirectToAction("XemGioHang");
            }

            // Thêm thông tin thanh toán
            var thanhToan = new ThanhToan
            {
                PhuongThuc = phuongThucThanhToan,
                NgayThanhToan = phuongThucThanhToan == "cod" ? (DateTime?)null : DateTime.Now,
                TongTienThanhToan = decimal.Parse(tongTien.ToString()),
                TrangThai = "Chưa thanh toán",
                IDDonHang = donHang.IDDonHang
            };
            db.ThanhToans.Add(thanhToan);

            // Xóa giỏ hàng trong cơ sở dữ liệu
            foreach (var item in gioHangNguoiDung)
            {
                var gioHangItem = db.GioHangs.FirstOrDefault(g => g.IDGioHang == item.IDGioHang);
                if (gioHangItem != null)
                {
                    db.GioHangs.Remove(gioHangItem);
                }
            }

            Session["gh"] = null; // Xóa giỏ hàng trong session
            db.SaveChanges();

            return RedirectToAction("ChiTietDatHang", new { id = donHang.IDDonHang });
        }

        // Thanh toán sản phẩm
        public ActionResult ChiTietDatHang(int id)
        {
            var donHang = db.DonHangs.FirstOrDefault(row => row.IDDonHang == id);

            ViewBag.ThanhToan = db.ThanhToans.FirstOrDefault(row => row.IDDonHang == id);
            ViewBag.DonHang = donHang;
            return View(donHang);
        }

        [HttpPost]
        public ActionResult HuyDon(int id)
        {
            var huy = db.DonHangs.FirstOrDefault(row => row.IDDonHang == id);
            huy.TrangThai = "Đã hủy";
            db.SaveChanges();
            return RedirectToAction("DonMua", "NguoiDung", new {id = id});
        }

    }
}