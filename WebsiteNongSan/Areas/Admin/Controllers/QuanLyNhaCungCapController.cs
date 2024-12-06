using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Areas.Admin.Controllers
{
    public class QuanLyNhaCungCapController : Controller
    {
        // GET: Admin/QuanLyNhaCungCap
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        // Hiển thị danh sách nhà cung cấp
        public ActionResult DanhSachNhaCungCap(string search = "", string SortName = "", int page = 1)
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
                var ds = db.NhaCungCaps.Where(row => row.TenNhaCungCap.Contains(search) || row.SDT.Contains(search) || row.Email.Contains(search)).ToList();
                ViewBag.Search = search;

                // Sắp xếp theo tên
                if (SortName == "tang")
                {
                    ds = ds.OrderBy(row => row.TenNhaCungCap).ToList();
                }
                else if (SortName == "giam")
                {
                    ds = ds.OrderByDescending(row => row.TenNhaCungCap).ToList();
                }
                ViewBag.SortName = SortName;

                // Phân trang
                int NoOfRecordPerPage = 9;
                int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ds.Count) / Convert.ToDouble(NoOfRecordPerPage)));
                int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;
                ds = ds.Skip(NoOfRecordToSkip).Take(NoOfRecordPerPage).ToList();
                ViewBag.Page = page;
                ViewBag.NoOfPages = NoOfPages;

                return View(ds);
            }
        }

        // Thêm nhà cung cấp
        public ActionResult ThemNhaCungCap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemNhaCungCap(NhaCungCap model)
        {
            var checkSDT = db.NhaCungCaps.FirstOrDefault(row => row.SDT == model.SDT);
            var checkEmail = db.NhaCungCaps.FirstOrDefault(row => row.Email == model.Email);
            var checkTenNCC = db.NhaCungCaps.FirstOrDefault(row => row.TenNhaCungCap == model.TenNhaCungCap);
            if (checkSDT != null)
            {
                ModelState.AddModelError("SDT", "Số điện thoại đã được đăng ký.");
            }
            else if (checkEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được đăng ký.");
            }
            else if(checkTenNCC != null)
            {
                ModelState.AddModelError("TenNhaCungCap", "Tên nhà cung cấp đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                db.NhaCungCaps.Add(model);
                db.SaveChanges();
                return RedirectToAction("DanhSachNhaCungCap");
            }
            return View();
        }

        // Cập nhật thông tin nhà cung cấp
        public ActionResult CapNhatNhaCungCap(int id)
        {
            var editModel = db.NhaCungCaps.FirstOrDefault(row => row.IDNhaCungCap == id);
            return View(editModel);
        }
        [HttpPost]
        public ActionResult CapNhatNhaCungCap(NhaCungCap model, int id)
        {
            var checkSDT = db.NhaCungCaps.FirstOrDefault(row => row.SDT == model.SDT && row.IDNhaCungCap != id);
            var checkEmail = db.NhaCungCaps.FirstOrDefault(row => row.Email == model.Email && row.IDNhaCungCap != id);
            var checkTenNCC = db.NhaCungCaps.FirstOrDefault(row => row.TenNhaCungCap == model.TenNhaCungCap && row.IDNhaCungCap != id);
            if (checkSDT != null)
            {
                ModelState.AddModelError("SDT", "Số điện thoại đã được đăng ký.");
            }
            else if (checkEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được đăng ký.");
            }
            else if (checkTenNCC != null)
            {
                ModelState.AddModelError("TenNhaCungCap", "Tên nhà cung cấp đã tồn tại.");
            }

            if(ModelState.IsValid)
            {
                var editModel = db.NhaCungCaps.FirstOrDefault(row=>row.IDNhaCungCap == id);
                editModel.TenNhaCungCap = model.TenNhaCungCap;
                editModel.SDT = model.SDT;
                editModel.Email = model.Email;
                editModel.DiaChi = model.DiaChi;
                editModel.GhiChu = model.GhiChu;
                db.SaveChanges();
                return RedirectToAction("DanhSachnhaCungCap");
            }
            return View();
        }

        // Xóa nhà cung cấp
        public ActionResult XoaNhaCungCap(int id)
        {
            var deleteModel = db.NhaCungCaps.FirstOrDefault(row => row.IDNhaCungCap == id);
            return View(deleteModel);
        }
        [HttpPost]
        public ActionResult XoaNhaCungCap(NhaCungCap model, int id)
        {
            var deleteModel = db.NhaCungCaps.FirstOrDefault(row => row.IDNhaCungCap == id);
            db.NhaCungCaps.Remove(deleteModel);
            db.SaveChanges();   
            return RedirectToAction("DanhSachNhaCungCap");
        }
    }
}