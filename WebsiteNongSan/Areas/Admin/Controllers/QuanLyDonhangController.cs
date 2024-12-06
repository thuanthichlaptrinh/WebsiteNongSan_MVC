using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Areas.Admin.Controllers
{
    public class QuanLyDonhangController : Controller
    {
        // GET: Admin/QuanLyDonhang
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        // Hiển thị danh sách các đơn hàng
        public ActionResult DanhSachDonHang(string Search="", string SortTrangThai="", int page=1)
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

                var ds = db.DonHangs.ToList();

                if (!string.IsNullOrEmpty(Search))
                {
                    ds = ds.Where(row => row.NguoiDung.TenNguoiDung.Contains(Search)).ToList();
                }

                if (SortTrangThai == "giaoThanhCong")
                {
                    ds = ds.Where(row => row.TrangThai == "Đã giao").ToList();
                }
                else if(SortTrangThai == "choGiaoHang")
                {
                    ds = ds.Where(row => row.TrangThai == "Chờ giao hàng").ToList();
                }
                else if(SortTrangThai == "dangXuLy")
                {
                    ds = ds.Where(row => row.TrangThai == "Đang xử lý").ToList();
                }
                else if (SortTrangThai == "daHuy")
                {
                    ds = ds.Where(row => row.TrangThai == "Đã hủy").ToList();
                }

                ViewBag.SortTrangThai = SortTrangThai;


                // Phân trang
                int NoOfRecordPerPage = 9; // số record muốn hiển thị lên trang
                int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ds.Count) / Convert.ToDouble(NoOfRecordPerPage))); // lấy số record trong ds chia cho số trang muốn phân
                int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;  // Số record cần skip
                ds = ds.Skip(NoOfRecordToSkip).Take(NoOfRecordPerPage).ToList(); // lấy 7 record hiển thị lên web
                ViewBag.Page = page;
                ViewBag.NoOfPages = NoOfPages;

                ViewBag.Search = Search;
                ViewBag.SortTrangThai = SortTrangThai;
                return View(ds);
            }
        }

        // Chi tiết đơn hàng
        public ActionResult ChiTietDonHang(int id)
        {
            var ctdh = db.ChiTietDonHangs.FirstOrDefault(row => row.IDDonHang == id);

            if (ctdh == null)
            {
                return HttpNotFound("Đơn hàng không tồn lại");
            }
            
            ViewBag.CTDH = db.ChiTietDonHangs.Where(row => row.IDDonHang == id).ToList();
            ViewBag.GioHang = db.GioHangs.FirstOrDefault(row => row.IDNguoiDung == ctdh.DonHang.IDNguoiDung);
            return View(ctdh);
        }

        [HttpPost]
        public ActionResult DuyetDon(int id, FormCollection fc)
        {
            var donHang = db.DonHangs.FirstOrDefault(row => row.IDDonHang == id);
            if (donHang == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại!";
                return RedirectToAction("ChiTietDonHang", new { id });
            }

            // Lấy giá trị từ FormCollection
            string duyetDon = fc["txtDuyetDon"];

            if (!string.IsNullOrEmpty(duyetDon))
            {
                donHang.TrangThai = "Chờ giao hàng"; 
                db.SaveChanges();

                TempData["Success"] = "Đơn hàng đã được duyệt thành công!";
            }

            return RedirectToAction("ChiTietDonHang", new { id });
        }

        // Xác nhận đơn hàng đã được giao
        [HttpPost]
        public ActionResult XacNhanDonHang(int id)
        {
            var edit = db.DonHangs.FirstOrDefault(row => row.IDDonHang == id);
            edit.TrangThai = "Đã giao";
            db.SaveChanges();
            return RedirectToAction("DanhSachDonHang");
        }

    }
}