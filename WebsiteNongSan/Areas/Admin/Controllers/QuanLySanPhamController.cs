using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Areas.Admin.Controllers
{
    public class QuanLySanPhamController : Controller
    {
        // GET: Admin/QuanLySanPham
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        public ActionResult DanhSachSanPham(int? FilterLoai, int? FilterNcc, string search = "", string SortPrice = "", int page = 1)
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
                var ds = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
                ViewBag.Search = search;

                // Truyền loại với nhà cung cấp lên view
                ViewBag.dsl = db.LoaiSanPhams.ToList();
                ViewBag.dsncc = db.NhaCungCaps.ToList();

                // Lọc theo loại sản phẩm
                if (FilterLoai.HasValue)
                {
                    ds = ds.Where(row => row.IDLoaiSP == FilterLoai).ToList();
                }
                ViewBag.FilterLoai = FilterLoai;

                // Lọc theo nhà cung cấp
                if (FilterNcc.HasValue)
                {
                    ds = ds.Where(row => row.IDNhaCungCap == FilterNcc).ToList();
                }
                ViewBag.FilterNcc = FilterNcc;

                // Lọc theo giá
                if (SortPrice == "tang")
                {
                    ds = ds.OrderBy(row => row.Gia).ToList();
                }
                else if (SortPrice == "giam")
                {
                    ds = ds.OrderByDescending(row => row.Gia).ToList();
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


        // Thêm mới sản phẩm
        public ActionResult ThemMoiSanPham()
        {
            ViewBag.DsLoai = new SelectList(db.LoaiSanPhams, "IDLoaiSP", "TenLoaiSP");
            ViewBag.DsNCC = new SelectList(db.NhaCungCaps, "IDNhaCungCap", "TenNhaCungCap");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoiSanPham(SanPham model, HttpPostedFileBase txtHinhAnh)
        {
            if (txtHinhAnh == null)
            {
                ViewBag.Error = "Vui lòng nhập hình ảnh";
                return View();
            }
            else
            {
                var checkTen = db.SanPhams.FirstOrDefault(row => row.TenSanPham == model.TenSanPham);
                if (checkTen != null)
                {
                    ModelState.AddModelError("SDT", "Sản phẩm đã tồn tại trong danh sách!");
                    return View("ThemMoiSanPham", model);
                }

                if (ModelState.IsValid)
                {
                    string tenFile = txtHinhAnh.FileName;
                    string duongDan = Server.MapPath("~/Content/HinhAnh/") + tenFile;

                    txtHinhAnh.SaveAs(duongDan);
                    model.HinhAnh = tenFile;
                    model.NgayTao = DateTime.Now;
                    model.TrangThai = true; // true là đang kinh doanh, false là ngừng kinh doanh

                    db.SanPhams.Add(model); 
                    db.SaveChanges();

                    return RedirectToAction("DanhSachSanPham");
                }
            }
            return View();
        }

        // Cập nhật sản phẩm
        [HttpGet]
        public ActionResult CapNhatSanPham(int id)
        {
            var editModel = db.SanPhams.FirstOrDefault(row => row.IDSanPham == id);
            ViewBag.DsLoai = db.LoaiSanPhams.Select(loai => new SelectListItem 
            { 
                Value = loai.IDLoaiSP.ToString(), 
                Text = loai.TenLoaiSP 
            }).ToList();

            ViewBag.DsNCC = db.NhaCungCaps.Select(ncc => new SelectListItem 
            { 
                Value = ncc.IDNhaCungCap.ToString(), 
                Text = ncc.TenNhaCungCap 
            }).ToList();
            return View(editModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CapNhatSanPham(SanPham model, int id, HttpPostedFileBase txtHinhAnh)
        { 
            var editModel = db.SanPhams.FirstOrDefault(row => row.IDSanPham == id);
            if (editModel != null)
            {
                var checkTen = db.SanPhams.FirstOrDefault(row => row.TenSanPham == model.TenSanPham && row.IDSanPham != id);
                
                if (checkTen != null)
                    ModelState.AddModelError("TenSanPham", "Tên sản phẩm đã tồn tại!");

                ModelState.Clear();
                if (ModelState.IsValid)
                {
                    editModel.TenSanPham = model.TenSanPham;
                    editModel.Gia = model.Gia;
                    editModel.SoLuongTon = model.SoLuongTon;
                    editModel.MoTa = model.MoTa;
                    editModel.IDLoaiSP = model.IDLoaiSP;
                    editModel.IDNhaCungCap = model.IDNhaCungCap;
                    editModel.DonViTinh = model.DonViTinh;
                    editModel.TrangThai = model.TrangThai;

                    if (txtHinhAnh != null)
                    {
                        string tenFile = txtHinhAnh.FileName;
                        string duongDan = Server.MapPath("~/Content/HinhAnh/") + tenFile;

                        txtHinhAnh.SaveAs(duongDan);
                        editModel.HinhAnh = tenFile;
                    }

                    db.SaveChanges();
                    return RedirectToAction("DanhSachSanPham");
                }
            }
            else
            {
                ViewBag.Error = "Dữ liệu không hợp lệ.";
            }

            ViewBag.DsLoai = db.LoaiSanPhams.Select(loai => new SelectListItem { Value = loai.IDLoaiSP.ToString(), Text = loai.TenLoaiSP }).ToList();
            ViewBag.DsNCC = db.NhaCungCaps.Select(ncc => new SelectListItem { Value = ncc.IDNhaCungCap.ToString(), Text = ncc.TenNhaCungCap }).ToList();

            return View("CapNhatSanPham", model);
        }

        // Chi tiết sản phẩm
        public ActionResult ChiTietSanPham(int id)
        {
            var detailModel = db.SanPhams.FirstOrDefault(row => row.IDSanPham == id);
            return View(detailModel);
        }

        // Xóa sản phẩm
        public ActionResult XoaSanPham(int id)
        {
            var deleteModel = db.SanPhams.FirstOrDefault(row => row.IDSanPham == id);
            return View(deleteModel);
        }
        [HttpPost]
        public ActionResult XoaSanPham(SanPham model, int id)
        {
            var deleteModel = db.SanPhams.FirstOrDefault(row => row.IDSanPham == id);   
            db.SanPhams.Remove(deleteModel);
            db.SaveChanges();
            return RedirectToAction("DanhSachSanPham");
        }

        // Các bước nhúng CkEditor
        // 1. Tải bộ plugin và cho vào project
        // 2. kéo file js vào 
        // 3. thay đổi input nội dung thành textarea có đặt id cho input đó
        // 4. Viết lệnh js cho textarea
        // 5. Lưu dữ liệu - tắt kiểm tra HTML cho action lưu dữ liệu
    }
}