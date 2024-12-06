using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Areas.Admin.Models
{
    public class QuanLyNguoiDungController : Controller
    {
        // GET: Admin/QuanLyNguoiDung
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        public static string GetLastName(string fullName)
        {
            var nameParts = fullName.Trim().Split(' ');
            return nameParts[nameParts.Length - 1];
        }

        // DANH SÁCH
        public ActionResult DanhSachNguoiDung(string Search = "", string SortName = "", string FilterVaiTro = "", string SortDate1 = "", string SortDate2 = "", int page = 1)
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
                    return Redirect("~/Admin/ThongBao/BaoLoi"); // không cho vào trang admin
                }

                // Tìm kiếm 
                var ds = db.NguoiDungs.Where(row=>row.TenNguoiDung.Contains(Search) || row.Email.Contains(Search) || row.SDT.Contains(Search)).ToList();
                ViewBag.Search = Search;

                // Sắp xếp theo tên
                if (SortName == "tentang")
                {
                    ds = ds.OrderBy(row => GetLastName(row.TenNguoiDung))
                           .ThenBy(row => row.TenNguoiDung) 
                           .ToList();
                }
                else if (SortName == "tengiam")
                {
                    ds = ds.OrderByDescending(row => GetLastName(row.TenNguoiDung))
                           .ThenByDescending(row => row.TenNguoiDung) 
                           .ToList();
                }
                ViewBag.SortName = SortName;


                // Lọc theo vai trò
                if (FilterVaiTro == "userFil")
                {
                    ds = ds.Where(row => row.VaiTro == "User").ToList();
                }
                else if(FilterVaiTro == "adminFil")
                {
                    ds = ds.Where(row => row.VaiTro == "Admin").ToList();
                }

                // Lọc theo ngày tạo
                DateTime ngayTao1, ngayTao2;
                bool isDate1Valid = DateTime.TryParse(SortDate1, out ngayTao1);
                bool isDate2Valid = DateTime.TryParse(SortDate2, out ngayTao2);

                if (isDate1Valid && isDate2Valid)
                {
                    ds = ds.Where(row => row.NgayTao >= ngayTao1 && row.NgayTao <= ngayTao2).ToList();
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

        // THÊM MỚI 
        public ActionResult ThemMoiNguoiDung()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoiNguoiDung(NguoiDung model, HttpPostedFileBase txtHinhAnh)
        {
            if (txtHinhAnh == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh!";
                return View();
            }
            else
            {
                var checkSDT = db.NguoiDungs.FirstOrDefault(row => row.SDT == model.SDT);
                var checkEmail = db.NguoiDungs.FirstOrDefault(row => row.Email == model.Email);

                if (checkSDT != null || checkEmail != null)
                {
                    if (checkSDT != null)
                    {
                        ModelState.AddModelError("SDT", "Số điện thoại đã tồn tại!");
                    }
                    if (checkEmail != null)
                    {
                        ModelState.AddModelError("Email", "Email đã tồn tại!");
                    }
                    return View("ThemMoiNguoiDung", model); 
                }

                if (ModelState.IsValid)
                {
                    string tenFile = txtHinhAnh.FileName;
                    string duongDan = Server.MapPath("~/Content/HinhAnh/") + tenFile;

                    txtHinhAnh.SaveAs(duongDan);
                    model.HinhAnh = tenFile;
                    model.NgayTao = DateTime.Now;

                    db.NguoiDungs.Add(model);
                    db.SaveChanges();

                    return RedirectToAction("DanhSachNguoiDung"); 
                }
            }
            return View();
        }

        // CẬP NHẬT
        public ActionResult CapNhatNguoiDung(int id)
        {
            var editModel = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == id);
            return View(editModel);
        }
        [HttpPost]
        public ActionResult CapNhatNguoiDung(NguoiDung model, HttpPostedFileBase txtHinhAnh1)
        {
            var editModel = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == model.IDNguoiDung);
            if (editModel != null)
            {
                var checkSDT = db.NguoiDungs.FirstOrDefault(row => row.SDT == model.SDT && row.IDNguoiDung != model.IDNguoiDung);
                var checkEmail = db.NguoiDungs.FirstOrDefault(row => row.Email == model.Email && row.IDNguoiDung != model.IDNguoiDung);

                if (checkSDT != null)
                    ModelState.AddModelError("SDT", "Số điện thoại đã tồn tại!");
                if (checkEmail != null)
                    ModelState.AddModelError("Email", "Email đã tồn tại!");

                if (ModelState.IsValid)
                {
                    editModel.TenNguoiDung = model.TenNguoiDung;
                    editModel.SDT = model.SDT;
                    editModel.Email = model.Email;
                    editModel.MatKhau = model.MatKhau;
                    editModel.DiaChi = model.DiaChi;
                    editModel.VaiTro = model.VaiTro;

                    if (txtHinhAnh1 != null)
                    {
                        string tenFile = txtHinhAnh1.FileName;
                        string duongDan = Server.MapPath("~/Content/HinhAnh/") + tenFile;

                        txtHinhAnh1.SaveAs(duongDan);
                        editModel.HinhAnh = tenFile;
                    }

                    db.SaveChanges();
                    return RedirectToAction("DanhSachNguoiDung");
                }
            }
            return View("CapNhatNguoiDung", model);
        }

        // CHI TIẾT
        public ActionResult ChiTietNguoiDung(int id)
        {
            var detailModel = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == id);
            return View(detailModel);
        }

        // XÓA
        public ActionResult XoaNguoiDung(int id)
        {
            var deleteModel = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == id);
            return View(deleteModel);
        }
        [HttpPost]
        public ActionResult XoaNguoiDung(NguoiDung model, int id)
        {
            var deleteModel = db.NguoiDungs.FirstOrDefault(row => row.IDNguoiDung == id);
            db.NguoiDungs.Remove(deleteModel);
            db.SaveChanges();
            return RedirectToAction("DanhSachNguoiDung");
        }
    }
}