document.addEventListener("DOMContentLoaded", function () {
    const header = document.querySelector(".header");
    const menu = document.querySelector(".sticky-menu");

    // Cập nhật chiều cao header vào khoảng cách top của menu
    const headerHeight = header.offsetHeight;
    menu.style.top = headerHeight + "px";
});

// Lắng nghe sự kiện cuộn trang
window.addEventListener("scroll", function () {
    const scrollUpButton = document.querySelector(".scroll-up");

    // Hiển thị nút cuộn lên khi người dùng cuộn xuống trên 200px
    if (window.scrollY > 200) {
        scrollUpButton.style.display = "block";
    } else {
        scrollUpButton.style.display = "none";
    }
});

// Cuộn lên đầu trang khi nhấn vào nút
document.querySelector(".scroll-up").addEventListener("click", function (e) {
    e.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ a
    window.scrollTo({
        top: 0, // Vị trí cuộn lên
        behavior: "smooth", // Cuộn mượt mà
    });
});
