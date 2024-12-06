using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Areas.Admin.Controllers
{
    public class QuanLyKhuyenMaiController : Controller
    {
        // GET: Admin/QuanLyKhuyenMai
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        // Danh sách khuyến mãi
        public ActionResult DanhSachKhuyenMai(string search = "", string SortName="", string SortDate1 = "", string SortDate2 = "", int page = 1)
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
                var ds = db.KhuyenMais.Where(row => row.TenKhuyenMai.Contains(search)).ToList();

                // Lọc theo giá
                if (SortName == "tang")
                {
                    ds = ds.OrderBy(row => row.TenKhuyenMai).ToList();
                }
                else if (SortName == "giam")
                {
                    ds = ds.OrderByDescending(row => row.TenKhuyenMai).ToList();
                }
                ViewBag.SortName = SortName;

                // Lọc theo ngày tạo
                DateTime ngayTao1, ngayTao2;
                bool isDate1Valid = DateTime.TryParse(SortDate1, out ngayTao1);
                bool isDate2Valid = DateTime.TryParse(SortDate2, out ngayTao2);

                if (isDate1Valid && isDate2Valid)
                {
                    ds = ds.Where(row => row.NgayBatDau >= ngayTao1 && row.NgayKetThuc <= ngayTao2).ToList();
                }
                ViewBag.SortDate1 = SortDate1;
                ViewBag.SortDate2 = SortDate2;

                // Phân trang
                int NoOfRecordPerPage = 7; // số record muốn hiển thị lên trang
                int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ds.Count) / Convert.ToDouble(NoOfRecordPerPage))); // lấy số record trong ds chia cho số trang muốn phân
                int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;  // Số record cần skip
                ds = ds.Skip(NoOfRecordToSkip).Take(NoOfRecordPerPage).ToList(); // lấy 7 record hiển thị lên web
                ViewBag.Page = page;
                ViewBag.NoOfPages = NoOfPages;
                return View(ds);
            }
        }

        // Thêm mới khuyến mãi
        public ActionResult ThemKhuyenMai()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemKhuyenMai(KhuyenMai model)
        {
            if(model.NgayBatDau == null)
            {
                ModelState.AddModelError("NgayBatDau", "Vui lòng nhập ngày bắt đầu.");
            }
            else if(model.NgayKetThuc == null)
            {
                ModelState.AddModelError("NgayKetThuc", "Vui lòng nhập ngày kết thúc.");
            }

            if (ModelState.IsValid)
            {
                var checkTen = db.KhuyenMais.FirstOrDefault(row => row.TenKhuyenMai == model.TenKhuyenMai);

                if (checkTen != null)
                {
                    ModelState.AddModelError("TenKhuyenMai", "Tên khuyến mãi đã tồn tại.");
                }
                else if (model.NgayBatDau < DateTime.Now.Date)
                {
                    ModelState.AddModelError("NgayBatDau", "Ngày bắt đầu không được ở quá khứ.");
                }
                else if (model.NgayKetThuc <= model.NgayBatDau)
                {
                    ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau hoặc giờ lớn hơn ngày bắt đầu.");
                }
                else if (model.PhanTramGiam <= 0 || model.PhanTramGiam > 100)
                {
                    ModelState.AddModelError("PhanTramGiam", "Phần trăm giảm giá phải nằm trong khoảng từ 1 đến 100.");
                }
                else
                {
                    db.KhuyenMais.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("DanhSachKhuyenMai");
                }
            }
            return View(model);
        }

        // Cập nhật khuyến mãi
        [HttpGet]
        public ActionResult CapNhatKhuyenMai(int id)
        {
            var editModel = db.KhuyenMais.FirstOrDefault(row => row.IDKhuyenMai == id);
            if (editModel == null)
            {
                return HttpNotFound();
            }
            return View(editModel);
        }
        [HttpPost]
        public ActionResult CapNhatKhuyenMai(KhuyenMai model, int id)
        {
            if (ModelState.IsValid)
            {
                var editModel = db.KhuyenMais.FirstOrDefault(row => row.IDKhuyenMai == id);
                editModel.TenKhuyenMai = model.TenKhuyenMai;
                editModel.PhanTramGiam = model.PhanTramGiam;

                if (model.NgayBatDau != null)
                {
                    editModel.NgayBatDau = model.NgayBatDau;
                }
                if (model.NgayKetThuc != null)
                {
                    editModel.NgayKetThuc = model.NgayKetThuc;
                }

                db.SaveChanges();
                return RedirectToAction("DanhSachKhuyenMai");
            }
            return View();
        }

        // Xóa khuyến mãi
        [HttpGet]
        public ActionResult XoaKhuyenMai(int id)
        {
            var deleteModel = db.KhuyenMais.FirstOrDefault(row => row.IDKhuyenMai == id);
            return View(deleteModel);
        }
        [HttpPost]
        public ActionResult XoaKhuyenMai(KhuyenMai model, int id)
        {
            var deleteModel = db.KhuyenMais.FirstOrDefault(row => row.IDKhuyenMai == id);
            db.KhuyenMais.Remove(deleteModel);
            db.SaveChanges();
            return RedirectToAction("DanhSachKhuyenMai");
        }
    }
}