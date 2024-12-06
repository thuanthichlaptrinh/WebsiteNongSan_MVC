using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebsiteNongSan.Models;

namespace WebsiteNongSan.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        private QuanLyNongSanHUITEntities db = new QuanLyNongSanHUITEntities();

        // Trang chủ
        public ActionResult Index(string search = "")
        {
            var ds = db.SanPhams.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                ds = ds.Where(sp => sp.TenSanPham.Contains(search));
            }

            ViewBag.Search = search;
            return View(ds.ToList());
        }

        // Hiển thị danh mục sản phẩm
        public ActionResult DanhMucSanPham(string search = "")
        {
            var danhMuc = db.LoaiSanPhams.ToList();
            return PartialView(danhMuc);
        }


        // Hiển thị các sản phẩm khuyến mãi
        public ActionResult SanPhamKhuyenMai()
        {
            DateTime ngayHienTai = DateTime.Now;
            var ds = db.SanPhamKhuyenMais.Where(row => row.KhuyenMai.NgayBatDau <= ngayHienTai && row.KhuyenMai.NgayKetThuc >= ngayHienTai).ToList();
            return View(ds);
        }

        // Danh sách các sản phẩm khuyến mãi 
        public ActionResult DanhSachSanPhamKhuyenMai(string SortPrice = "", string FilterPrice = "", string NhaCungCap = "")
        {
            var ds = db.SanPhamKhuyenMais.Where(row => row.SanPham.TrangThai == true && row.SanPham.SoLuongTon > 0);

            if (FilterPrice == "<100")
            {
                ds = ds.Where(row => row.SanPham.Gia < 100000);
                if (!ds.Any())
                {
                    ViewBag.FilterPrice = "Không có sản phẩm nào tồn tại trong mức giá dưới 100.000đ.";
                }
            }
            else if (FilterPrice == "100-300")
            {
                ds = ds.Where(row => row.SanPham.Gia >= 100000 && row.SanPham.Gia <= 300000);
                if (!ds.Any())
                {
                    ViewBag.FilterPrice = "Không có sản phẩm nào tồn tại trong mức giá từ 100.000đ đến 300.000đ.";
                }
            }
            else if (FilterPrice == ">300")
            {
                ds = ds.Where(row => row.SanPham.Gia > 300000);
                if (!ds.Any())
                {
                    ViewBag.FilterPrice = "Không có sản phẩm nào tồn tại trên mức giá 300.000đ.";
                }
            }

            if (SortPrice == "tang")
            {
                ds = ds.OrderBy(row => row.SanPham.Gia);
                if (!ds.Any())
                {
                    ViewBag.SortPrice = "Không có sản phẩm nào để sắp xếp tăng dần.";
                }
            }
            else if (SortPrice == "giam")
            {
                ds = ds.OrderByDescending(row => row.SanPham.Gia);
                if (!ds.Any())
                {
                    ViewBag.SortPrice = "Không có sản phẩm nào để sắp xếp giảm dần.";
                }
            }

            ViewBag.dsl = db.LoaiSanPhams.ToList();
            return View(ds.ToList());
        }


        // Hiển thị danh sách sản phẩm 
        public ActionResult DanhSachSanPham()
        {
            var ds = db.SanPhams.ToList();
            ViewBag.dsl = db.LoaiSanPhams.ToList();
            ViewBag.dssp = db.SanPhams.ToList();
            return View(ds);
        }

        // Hiện thị danh sách sản phẩm với từng loại lên trang chủ
        //public ActionResult DanhSachSanPhamPartial(string Search = "")
        //{
        //    var ds = db.SanPhams.ToList();

        //    if (Search != null)
        //    {
        //        var sanPham = db.SanPhams.Where(row => row.TenSanPham.Contains(Search)).ToList();

        //        ds = ds.Where(lsp => sanPham.Any(sp => sp.IDLoaiSP == lsp.IDLoaiSP)).ToList();
        //    }

        //    ViewBag.dsl = db.LoaiSanPhams.ToList();
        //    return PartialView(ds);
        //}

        public ActionResult DanhSachSanPhamPartial(string search = "")
        {
            var ds = db.SanPhams.AsQueryable();
            ViewBag.dsl = db.LoaiSanPhams.ToList();
            ViewBag.SanPhamKhuyenMai = db.SanPhamKhuyenMais.ToList();
            return PartialView(ds.ToList());
        }


        // Hiển thị danh sách sản phẩm theo loại từ menu hoặc link
        public ActionResult DanhSachSanPhamTheoLoai(int? id, string SortPrice = "", string FilterPrice = "", string KhuyenMai="", string NhaCungCap = "")
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "SanPham");
            }

            var ds = db.SanPhams.Where(row => row.IDLoaiSP == id).ToList();

            if (!ds.Any())
            {
                ViewBag.Message = "Không tìm thấy sản phẩm hoặc chưa có sản phẩm nào trong danh mục.";
            }

            // Lọc theo giá
            if (FilterPrice == "<100")
            {
                ds = ds.Where(row => row.Gia < 100000).ToList();
                if (!ds.Any())
                {
                    ViewBag.FilterPrice = "Không có sản phẩm nào tồn tại trong mức giá dưới 100.000đ.";
                }
            }
            else if (FilterPrice == "100-300")
            {
                ds = ds.Where(row => row.Gia >= 100000 && row.Gia <= 300000).ToList();
                if (!ds.Any())
                {
                    ViewBag.FilterPrice = "Không có sản phẩm nào tồn tại trong mức giá từ 100.000đ đến 300.000đ.";
                }
            }
            else if (FilterPrice == ">300")
            {
                ds = ds.Where(row => row.Gia > 300000).ToList();
                if (!ds.Any())
                {
                    ViewBag.FilterPrice = "Không có sản phẩm nào tồn tại trên mức giá 300.000đ.";
                }
            }

            // Sắp xếp theo giá tăng hoặc giảm
            if (SortPrice == "tang")
            {
                ds = ds.OrderBy(row => row.Gia).ToList();
                if (!ds.Any())
                {
                    ViewBag.SortPrice = "Không có sản phẩm nào để sắp xếp tăng dần.";
                }
            }
            else if (SortPrice == "giam")
            {
                ds = ds.OrderByDescending(row => row.Gia).ToList();
                if (!ds.Any())
                {
                    ViewBag.SortPrice = "Không có sản phẩm nào để sắp xếp giảm dần.";
                }
            }

            if (KhuyenMai == "yes")
            {
                DateTime ngayHienTai = DateTime.Now;

                var sanPhamKhuyenMai = db.SanPhamKhuyenMais
                    .Where(row => row.KhuyenMai.NgayBatDau <= ngayHienTai && row.KhuyenMai.NgayKetThuc >= ngayHienTai)
                    .Select(row => row.IDSanPham) 
                    .ToList();

                ds = ds.Where(row => sanPhamKhuyenMai.Contains(row.IDSanPham)).ToList();
            }


            // Lọc theo nhà cung cấp
            if (NhaCungCap!="" && NhaCungCap!= null)
            {
                ds = ds.Where(row => row.IDNhaCungCap == int.Parse(NhaCungCap)).ToList();
                if (!ds.Any())
                {
                    ViewBag.NCC = "Không tìm thấy sản phẩm có nhà cung cấp này.";
                }
            }

            ViewBag.DS = db.LoaiSanPhams.Where(row => row.IDLoaiSP == id).FirstOrDefault();
            ViewBag.NhaCungCap = db.NhaCungCaps.ToList();
            ViewBag.SanPhamKhuyenMai = db.SanPhamKhuyenMais.ToList();
            return View(ds);
        }

        // Chi tiết sản phẩm
        public ActionResult ChiTietSanPham(int id)
        {
            var detailProduct = db.SanPhams.FirstOrDefault(row => row.IDSanPham == id);
            var danhSachLoai = db.LoaiSanPhams.Where(row => row.IDLoaiSP == detailProduct.IDLoaiSP).FirstOrDefault();
            var danhSachSP = db.SanPhams.Where(row => row.IDLoaiSP == danhSachLoai.IDLoaiSP && row.TenSanPham != detailProduct.TenSanPham).ToList();

            ViewBag.DanhSachSP = danhSachSP;
            return View(detailProduct);
        }
    }
}