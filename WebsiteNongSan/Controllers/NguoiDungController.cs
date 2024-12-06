using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        // ĐĂNG NHẬP
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection fc)
        {
            string taiKhoan = fc["taiKhoan"];
            string matKhau = fc["matKhau"];
            var nguoiDung = db.NguoiDungs.SingleOrDefault(row => row.SDT == taiKhoan && row.MatKhau == matKhau);

            if (nguoiDung != null) // vì chỉ tồn tại 1 tk 
            {
                Session["user"] = nguoiDung;
                if (nguoiDung.VaiTro == "User")
                {
                    return RedirectToAction("Index", "SanPham");
                }
                else
                {
                    return Redirect("~/Admin/QuanLyNguoiDung/DanhSachNguoiDung");
                }
            }
            else
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng";
                return View();
            }
        }

        // ĐĂNG KÝ
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(NguoiDung model, FormCollection fc, HttpPostedFileBase txtHinhAnh)
        {
            if (txtHinhAnh == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh!";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    string tenFile = txtHinhAnh.FileName;
                    string duongDan = Server.MapPath("~/Content/HinhAnh/") + tenFile;
                    txtHinhAnh.SaveAs(duongDan);

                    if (model.MatKhau != fc["MatKhau"])
                    {
                        ModelState.AddModelError("MatKhau", "Mật khẩu không khớp.");
                        return View();
                    }

                    model.HinhAnh = tenFile;
                    model.NgayTao = DateTime.Now;
                    model.VaiTro = "User";

                    db.NguoiDungs.Add(model);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                    return RedirectToAction("DangNhap");
                }
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại.";
            return View();
        }

        // ĐĂNG XUẤT
        public ActionResult DangXuat()
        {
            var nguoiDung = (NguoiDung)Session["user"];
            Session.Remove("user");
            FormsAuthentication.SignOut();

            if(nguoiDung != null && nguoiDung.VaiTro == "User")
            {
                return Redirect("~/SanPham/Index");
            }
            return RedirectToAction("DangNhap");
        }

        public ActionResult ThongTinNguoiDung(int id)
        {
            var user = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == id);
            ViewBag.ActiveTab = "Info";
            return View(user);
        }

        public ActionResult DonMua(int id, string search="")
        {
            var donHangs = db.DonHangs
                .Where(dh => dh.IDNguoiDung == id)
                .Include(dh => dh.ChiTietDonHangs.Select(ct => ct.SanPham)).ToList();

            // Lọc danh sách nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                donHangs = donHangs.Where(dh =>
                    dh.IDDonHang.ToString().Contains(search) || // Lọc theo mã đơn hàng
                    dh.ChiTietDonHangs.Any(ct => ct.SanPham.TenSanPham.Contains(search)) // Lọc theo tên sản phẩm
                ).ToList();
            }

            ViewBag.ActiveTab = "DonMua"; // Gắn tab hiện tại
            ViewBag.Search = search; // Lưu lại từ khóa tìm kiếm
            return View(donHangs);
        }
        
        public ActionResult DonVuaDat(int id)
        {
            var donHangs = db.DonHangs
               .Where(dh => dh.IDNguoiDung == id && dh.TrangThai == "Đang xử lý")
               .Include(dh => dh.ChiTietDonHangs.Select(ct => ct.SanPham))
               .ToList();
            ViewBag.ActiveTab = "DonVuaDat";
            return View(donHangs);
        }

        public ActionResult DonChoGiao(int id)
        {
            var donHangs = db.DonHangs
               .Where(dh => dh.IDNguoiDung == id && dh.TrangThai == "Chờ giao hàng")
               .Include(dh => dh.ChiTietDonHangs.Select(ct => ct.SanPham))
               .ToList();
            ViewBag.ActiveTab = "DonChoGiao";
            return View(donHangs);
        }

        public ActionResult DonDaGiao(int id)
        {
            var donHangs = db.DonHangs
               .Where(dh => dh.IDNguoiDung == id && dh.TrangThai == "Đã giao")
               .Include(dh => dh.ChiTietDonHangs.Select(ct => ct.SanPham))
               .ToList();
            ViewBag.ActiveTab = "DonDaGiao";
            return View(donHangs);
        }

        public ActionResult DonDaHuy(int id)
        {
            var donHangs = db.DonHangs
               .Where(dh => dh.IDNguoiDung == id && dh.TrangThai == "Đã hủy")
               .Include(dh => dh.ChiTietDonHangs.Select(ct => ct.SanPham))
               .ToList();
            ViewBag.ActiveTab = "DonDaHuy";
            return View(donHangs);
        }

        public ActionResult ChinhSuaThongTin(int id)
        {
            var x = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == id);
            return View(x);
        }
        [HttpPost]
        public ActionResult ChinhSuaThongTin(NguoiDung model, int id, HttpPostedFileBase txtHinhAnh)
        {
            var x = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == id);
            return View(x);
        }

    }
}