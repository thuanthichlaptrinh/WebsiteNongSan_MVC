using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Areas.Admin.Controllers
{
    public class QuanLyLoaiSanPhamController : Controller
    {
        // GET: Admin/QuanLyLoaiSanPham
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        // Danh sách loại sản phẩm
        public ActionResult DanhSachLoaiSanPham(string Search = "", string SortName = "", int page = 1)
        {
            if (Session["user"] == null)
            {
                return Redirect("~/NguoiDung/DangNhap");
            }
            else
            {
                var user = (NguoiDung)Session["user"];
                if (user.VaiTro == "User")
                {
                    return Redirect("~/Admin/ThongBao/BaoLoi");
                }

                // Tìm kiếm
                var ds = db.LoaiSanPhams.Where(row => row.TenLoaiSP.Contains(Search)).ToList();
                ViewBag.Search = Search;
                ViewBag.Ds = ds;

                // Sắp xếp dựa trên SortName
                if (SortName == "tentang")
                {
                    ds = ds.OrderBy(row => row.TenLoaiSP).ToList();
                }
                else if (SortName == "tengiam")
                {
                    ds = ds.OrderByDescending(row => row.TenLoaiSP).ToList();
                }
                ViewBag.SortName = SortName;

                // Phân trang
                int NoOfRecordPerPage = 8; // số record muốn hiển thị lên trang
                int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ds.Count) / Convert.ToDouble(NoOfRecordPerPage))); // lấy số record trong ds chia cho số trang muốn phân
                int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;  // Số record cần skip
                ds = ds.Skip(NoOfRecordToSkip).Take(NoOfRecordPerPage).ToList(); // lấy 7 record hiển thị lên web
                ViewBag.Page = page;
                ViewBag.NoOfPages = NoOfPages;

                return View(ds);
            }
        }


        // Thêm loại sản phẩm
        public ActionResult ThemLoaiSanPham()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemLoaiSanPham(LoaiSanPham model)
        {
            if (ModelState.IsValid)
            {
                var checkTen = db.LoaiSanPhams.FirstOrDefault(row => row.TenLoaiSP == model.TenLoaiSP);
                if (checkTen == null)
                {
                    db.LoaiSanPhams.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("DanhSachLoaiSanPham");
                }
                else
                {
                    ModelState.AddModelError("TenLoaiSP", "Tên loại sản phẩm đã tồn tại.");
                }
            }
            return View(model);
        }

        // Cập nhật loại sản phẩm
        public ActionResult CapNhatLoaiSanPham(int id)
        {
            var editModel = db.LoaiSanPhams.FirstOrDefault(row => row.IDLoaiSP == id);
            return View(editModel);
        }
        [HttpPost]
        public ActionResult CapNhatLoaiSanPham(LoaiSanPham model, int id)
        {
            var editModel = db.LoaiSanPhams.FirstOrDefault(row => row.IDLoaiSP == id);
            if(ModelState.IsValid && editModel != null)
            {
                editModel.TenLoaiSP = model.TenLoaiSP;
                db.SaveChanges();
                return RedirectToAction("DanhSachLoaiSanPham");
            }
            return View();
        }

        // Xóa loại sản phẩm
        public ActionResult XoaLoaiSanPham(int id)
        {
            var deleteModel = db.LoaiSanPhams.FirstOrDefault(row => row.IDLoaiSP == id);
            return View(deleteModel);
        }
        [HttpPost]
        public ActionResult XoaLoaiSanPham(LoaiSanPham model, int id)
        {
            var deleteModel = db.LoaiSanPhams.FirstOrDefault(row => row.IDLoaiSP == id);
            db.LoaiSanPhams.Remove(deleteModel);
            db.SaveChanges();
            return RedirectToAction("DanhSachLoaiSanPham");
        }
    }
}