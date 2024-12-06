using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        // GET: Admin/ThongKe
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        public int ThongKeTongSanPham()
        {
            int SoLuongSanPham = db.SanPhams.Count();
            return SoLuongSanPham;
        }

        public int ThongKeTongSanPhamKhuyenMai()
        {
            int SoLuongSanPham = db.SanPhamKhuyenMais.Count();
            return SoLuongSanPham;
        }

        public int ThongKeSoDonHang()
        {
            return db.DonHangs.Count();
        }

        public int ThongKeNguoiDung()
        {
            return db.NguoiDungs.Count();
        }

        public ActionResult Index()
        {
            var user = (NguoiDung)Session["user"];
            if (user == null)
            {
                return Redirect("~/NguoiDung/DangNhap");
            }
            else
            {
                if (user.VaiTro == "User")
                {
                    return Redirect("~/Admin/ThongBao/BaoLoi");
                }
                var sanPhamBanChay = db.ChiTietDonHangs
                                    .GroupBy(ct => new
                                    {
                                        ct.SanPham.TenSanPham,
                                        Ngay = DbFunctions.TruncateTime(ct.DonHang.NgayDat)
                                    })
                                    .Select(g => new
                                    {
                                        TenSanPham = g.Key.TenSanPham,
                                        Ngay = g.Key.Ngay,
                                        SoLuong = g.Count()
                                    })
                                    .OrderBy(g => g.Ngay)
                                    .ToList();

                // Chuẩn bị dữ liệu cho trục X (ngày tháng) và dữ liệu sản phẩm
                var ngayThang = sanPhamBanChay.Select(sp => sp.Ngay).Distinct().ToList();
                var sanPhamBanChayGrouped = sanPhamBanChay
                                                .GroupBy(sp => sp.TenSanPham)
                                                .Select(g => new
                                                {
                                                    TenSanPham = g.Key,
                                                    Ngay = g.Select(sp => sp.Ngay).ToList(),
                                                    SoLuong = g.Select(sp => sp.SoLuong).ToList()
                                                }).ToList();

                ViewBag.NgayThang = ngayThang;
                ViewBag.SanPhamBanChay = sanPhamBanChayGrouped.Take(3);
                ViewBag.SoSanPham = ThongKeTongSanPham();
                ViewBag.SoSanPhamKM = ThongKeTongSanPhamKhuyenMai();
                ViewBag.SoDonHang = ThongKeSoDonHang();
                ViewBag.SoNguoiDung = ThongKeNguoiDung();
                return View();
            }
        }

    }
}