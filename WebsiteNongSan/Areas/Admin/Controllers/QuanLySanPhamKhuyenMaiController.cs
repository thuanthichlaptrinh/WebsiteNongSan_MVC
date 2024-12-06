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
    public class QuanLySanPhamKhuyenMaiController : Controller
    {
        // GET: Admin/QuanLySanPhamKhuyenMai
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        // Hiển thị danh sách sản phẩm khuyến mãi
        public ActionResult DanhSachSanPhamKM(int? FilterLoai, int? FilterNcc, string search = "", string SortPrice = "", int page = 1)
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
                var ds = db.SanPhamKhuyenMais.Where(row => row.SanPham.TenSanPham.Contains(search)).ToList();
                ViewBag.Search = search;

                // Truyền loại với nhà cung cấp lên view
                ViewBag.dsl = db.LoaiSanPhams.ToList();
                ViewBag.dsncc = db.NhaCungCaps.ToList();

                // Lọc theo loại sản phẩm
                if (FilterLoai.HasValue)
                {
                    ds = ds.Where(row => row.SanPham.LoaiSanPham.IDLoaiSP == FilterLoai).ToList();
                }
                ViewBag.FilterLoai = FilterLoai;

                // Lọc theo nhà cung cấp
                if (FilterNcc.HasValue)
                {
                    ds = ds.Where(row => row.SanPham.NhaCungCap.IDNhaCungCap == FilterNcc).ToList();
                }
                ViewBag.FilterNcc = FilterNcc;

                // Lọc theo giá
                if (SortPrice == "tang")
                {
                    ds = ds.OrderBy(row => (row.SanPham.Gia - (row.SanPham.Gia * (row.KhuyenMai.PhanTramGiam / 100)))).ToList();
                }
                else if (SortPrice == "giam")
                {
                    ds = ds.OrderByDescending(row => (row.SanPham.Gia - (row.SanPham.Gia * (row.KhuyenMai.PhanTramGiam / 100)))).ToList();
                }
                ViewBag.SortPrice = SortPrice;

                // Phân trang
                int NoOfRecordPerPage = 6;
                int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ds.Count) / Convert.ToDouble(NoOfRecordPerPage)));
                int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;
                ds = ds.Skip(NoOfRecordToSkip).Take(NoOfRecordPerPage).ToList();

                ViewBag.Page = page;
                ViewBag.NoOfPages = NoOfPages;

                return View(ds);
            }
        }

        //  Thêm mới sản phẩm khuyến mãi
        public ActionResult ThemSanPhamKhuyenMai()
        {
            ViewBag.DsSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSanPham");
            ViewBag.DsKhuyenMai = new SelectList(db.KhuyenMais, "IDKhuyenMai", "TenKhuyenMai");
            return View();
        }
        [HttpPost]
        public ActionResult ThemSanPhamKhuyenMai(SanPhamKhuyenMai model)
        {
            if (ModelState.IsValid)
            {
                var checkIDSanPham = db.SanPhamKhuyenMais.FirstOrDefault(row => row.IDSanPham == model.IDSanPham);
                var checkTrangThai = db.SanPhams.FirstOrDefault(row => row.IDSanPham == model.IDSanPham);

                if (checkTrangThai != null && checkTrangThai.TrangThai == false)
                {
                    ModelState.AddModelError("IDSanPham", "Sản phẩm đã ngừng sản xuất.");
                    ViewBag.DsSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSanPham");
                    ViewBag.DsKhuyenMai = new SelectList(db.KhuyenMais, "IDKhuyenMai", "TenKhuyenMai");
                    return View();
                }

                if (checkTrangThai != null && checkTrangThai.SoLuongTon <= 0)
                {
                    ModelState.AddModelError("IDSanPham", "Sản phẩm này đã hết hàng.");
                    ViewBag.DsSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSanPham");
                    ViewBag.DsKhuyenMai = new SelectList(db.KhuyenMais, "IDKhuyenMai", "TenKhuyenMai");
                    return View();
                }

                if (checkIDSanPham != null)
                {
                    ModelState.AddModelError("IDSanPham", "Sản phẩm này đã được thêm giảm giá");
                    ViewBag.DsSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSanPham");
                    ViewBag.DsKhuyenMai = new SelectList(db.KhuyenMais, "IDKhuyenMai", "TenKhuyenMai");
                    return View();
                }
                else
                {
                    db.SanPhamKhuyenMais.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("DanhSachSanPhamKM");
                }
            }
            ViewBag.DsSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSanPham");
            ViewBag.DsKhuyenMai = new SelectList(db.KhuyenMais, "IDKhuyenMai", "TenKhuyenMai");
            return View();
        }

        // Cập nhật sản phẩm khuyến mãi
        [HttpGet]
        public ActionResult CapNhatSanPhamKhuyenMai(int idSanPham, int idKhuyenMai)
        {
            // Tìm bằng idSanPham bởi vì idSanPham không được trùng nhau trong bảng SanPhamKhuyenMai
            var editModel = db.SanPhamKhuyenMais.FirstOrDefault(row => row.IDSanPham == idSanPham);

            // Truyền danh sach sang view
            ViewBag.dssp = db.SanPhams.Select(row => new SelectListItem { Value = row.IDSanPham.ToString(), Text = row.TenSanPham, }).ToList();
            ViewBag.dskm = db.KhuyenMais.Select(row => new SelectListItem { Value = row.IDKhuyenMai.ToString(), Text = row.TenKhuyenMai }).ToList();

            return View(editModel);
        }
        [HttpPost]
        public ActionResult CapNhatSanPhamKhuyenMai(SanPhamKhuyenMai model, int idSanPham, int idKhuyenMai)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trạng thái của sản phẩm
                var checkTrangThai = db.SanPhams.FirstOrDefault(row => row.IDSanPham == idSanPham);
                if (checkTrangThai != null)
                {
                    if (checkTrangThai.TrangThai == false)
                    {
                        ModelState.AddModelError("IDSanPham", "Sản phẩm này đã ngừng kinh doanh.");
                    }
                    else if (checkTrangThai.SoLuongTon <= 0)
                    {
                        ModelState.AddModelError("IDSanPham", "Sản phẩm này đã hết hàng.");
                    }
                }

                // Cập nhật thông tin nếu tồn tại bản ghi khuyến mãi
                var editModel = db.SanPhamKhuyenMais.FirstOrDefault(row => row.IDSanPham == idSanPham);
                if (editModel != null)
                {
                    // Cập nhật các trường cần thiết
                    editModel.GhiChu = model.GhiChu;

                    db.SaveChanges();
                    return RedirectToAction("DanhSachSanPhamKM");
                }
                else
                {
                    ModelState.AddModelError("IDSanPham", "Không tìm thấy sản phẩm khuyến mãi này.");
                }
            }

            // Dữ liệu cho dropdown
            ViewBag.dssp = new SelectList(db.SanPhams, "IDSanPham", "TenSanPham");
            ViewBag.dskm = new SelectList(db.KhuyenMais, "IDKhuyenMai", "TenKhuyenMai");
            return View(model);
        }


        // Xóa sản phẩm khuyến mãi  
        public ActionResult XoaSanPhamKhuyenMai(int idSanPham, int idKhuyenMai)
        {
            var deleteModel = db.SanPhamKhuyenMais.FirstOrDefault(row => row.IDSanPham == idSanPham && row.IDKhuyenMai == idKhuyenMai);
            return View(deleteModel);
        }
        [HttpPost]
        public ActionResult XoaSanPhamKhuyenMai(SanPhamKhuyenMai model, int idSanPham, int idKhuyenMai)
        {
            var deleteModel = db.SanPhamKhuyenMais.FirstOrDefault(row => row.IDSanPham == idSanPham && row.IDKhuyenMai == idKhuyenMai);
            db.SanPhamKhuyenMais.Remove(deleteModel);
            db.SaveChanges();
            return RedirectToAction("DanhSachSanPhamKM");
        }
    }
}