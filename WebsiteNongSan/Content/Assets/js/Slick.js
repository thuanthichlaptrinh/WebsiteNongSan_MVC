$(document).ready(function () {
    $(".slick-carousel").slick({
        slidesToShow: 4, // Hiển thị 4 sản phẩm
        slidesToScroll: 1,
        infinite: true, // Vòng lặp vô tận
        arrows: true, // Hiển thị mũi tên
        dots: false, // Ẩn dấu chấm chỉ số
        prevArrow: '<button type="button" class="slick-prev"><</button>',
        nextArrow: '<button type="button" class="slick-next">></button>',
        responsive: [
            {
                breakpoint: 1024, // Màn hình <1024px
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                },
            },
            {
                breakpoint: 768, // Màn hình <768px
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                },
            },
            {
                breakpoint: 480, // Màn hình <480px
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                },
            },
        ],
    });
});
