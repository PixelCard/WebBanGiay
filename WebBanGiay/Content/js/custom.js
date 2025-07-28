$(document).ready(function () {
    // Initialize hero slider
    $('.hero__slider').owlCarousel({
        loop: true,
        margin: 0,
        nav: false,
        dots: true,
        autoplay: true,
        autoplayTimeout: 5000,
        autoplayHoverPause: true,
        items: 1,
        animateOut: 'fadeOut',
        animateIn: 'fadeIn'
    });

    // Initialize product sliders with better configuration
    $('.latest-product__slider').owlCarousel({
        loop: true,
        margin: 20,
        nav: true,
        dots: false,
        autoplay: true,
        autoplayTimeout: 5000,
        autoplayHoverPause: true,
        responsive: {
            0: {
                items: 1
            },
            768: {
                items: 2
            },
            992: {
                items: 3
            }
        }
    });

    // Initialize category slider
    $('.categories__slider').owlCarousel({
        loop: true,
        margin: 20,
        nav: true,
        dots: false,
        autoplay: true,
        autoplayTimeout: 4000,
        autoplayHoverPause: true,
        responsive: {
            0: {
                items: 2
            },
            576: {
                items: 3
            },
            768: {
                items: 4
            },
            992: {
                items: 5
            }
        }
    });

    // Enhanced product filtering
    $('.featured__controls ul li').on('click', function () {
        var filter = $(this).data('filter');

        // Update active state
        $('.featured__controls ul li').removeClass('active');
        $(this).addClass('active');

        // Add loading state to products
        $('.featured__item').addClass('loading');

        // Simulate loading delay for better UX
        setTimeout(function () {
            if (filter === '*') {
                $('.featured__item').removeClass('loading').show();
            } else {
                $('.featured__item').removeClass('loading').hide();
                $('.featured__item' + filter).show();
            }
        }, 300);
    });

    // Product hover effects
    $('.featured__item').hover(
        function () {
            $(this).find('.featured__item__pic__hover').fadeIn(200);
        },
        function () {
            $(this).find('.featured__item__pic__hover').fadeOut(200);
        }
    );

    // Latest product item hover effects
    $('.latest-product__item').hover(
        function () {
            $(this).css('transform', 'translateX(10px)');
        },
        function () {
            $(this).css('transform', 'translateX(0)');
        }
    );

    // Add to cart functionality
    $('.featured__item__pic__hover li:last-child a').on('click', function (e) {
        e.preventDefault();
        var productId = $(this).closest('.featured__item').find('h6 a').attr('href').split('/').pop();

        // Show loading state
        $(this).html('<i class="fa fa-spinner fa-spin"></i>');

        // Simulate add to cart
        setTimeout(function () {
            alert('Sản phẩm đã được thêm vào giỏ hàng!');
            $('.featured__item__pic__hover li:last-child a').html('<i class="fa fa-shopping-cart"></i>');
        }, 1000);
    });

    // Wishlist functionality
    $('.featured__item__pic__hover li:first-child a').on('click', function (e) {
        e.preventDefault();
        var $icon = $(this).find('i');

        if ($icon.hasClass('fa-heart-o')) {
            $icon.removeClass('fa-heart-o').addClass('fa-heart');
            $icon.css('color', '#e44d26');
        } else {
            $icon.removeClass('fa-heart').addClass('fa-heart-o');
            $icon.css('color', '');
        }
    });

    // Quick view functionality
    $('.featured__item__pic__hover li:nth-child(2) a').on('click', function (e) {
        e.preventDefault();
        var productId = $(this).closest('.featured__item').find('h6 a').attr('href').split('/').pop();

        // Redirect to product details
        window.location.href = '/ProductDetails/ProductDetailsPage/' + productId;
    });

    // Smooth scrolling for category links
    $('.hero__categories ul li a').on('click', function (e) {
        e.preventDefault();
        var category = $(this).text();

        // Scroll to featured section
        $('html, body').animate({
            scrollTop: $('.featured').offset().top - 100
        }, 800);

        // Show all products when category is clicked
        $('.featured__controls ul li').removeClass('active');
        $('.featured__controls ul li:first').addClass('active');
        $('.featured__item').show();
    });

    // Search functionality
    $('.hero__search__form form').on('submit', function (e) {
        // Remove preventDefault to allow form submission to work properly
        // e.preventDefault();
        var searchTerm = $(this).find('input[type="text"]').val();

        if (searchTerm.trim() !== '') {
            // Form will submit normally to the search page
            // No need for alert or custom handling
        }
    });

    // Lazy loading for images
    $('.featured__item__pic').each(function () {
        var $this = $(this);
        var imageUrl = $this.data('setbg');

        if (imageUrl) {
            var img = new Image();
            img.onload = function () {
                $this.css('background-image', 'url(' + imageUrl + ')');
            };
            img.onerror = function () {
                console.log('Failed to load image:', imageUrl);
                // Fallback to default image
                $this.css('background-image', 'url("/Content/img/featured/feature-1.jpg")');
            };
            img.src = imageUrl;
        }
    });

    // Handle hero slider images
    $('.hero__item').each(function () {
        var $this = $(this);
        var imageUrl = $this.data('setbg');

        if (imageUrl) {
            var img = new Image();
            img.onload = function () {
                $this.css('background-image', 'url(' + imageUrl + ')');
            };
            img.onerror = function () {
                console.log('Failed to load hero image:', imageUrl);
                // Fallback to default hero image
                $this.css('background-image', 'url("/Content/img/hero/banner.jpg")');
            };
            img.src = imageUrl;
        }
    });

    // Add loading animation for page load
    $(window).on('load', function () {
        $('.featured__item').removeClass('loading');
    });

    // Responsive adjustments
    $(window).on('resize', function () {
        // Reinitialize sliders on window resize
        $('.hero__slider').trigger('refresh.owl.carousel');
        $('.latest-product__slider').trigger('refresh.owl.carousel');
        $('.categories__slider').trigger('refresh.owl.carousel');
    });

    // Add smooth transitions for price changes
    $('.featured__item__text h5').each(function () {
        var $price = $(this);
        var originalPrice = $price.text();

        // Add hover effect to show original price
        $price.parent().hover(
            function () {
                $price.fadeOut(200, function () {
                    $price.text(originalPrice).fadeIn(200);
                });
            },
            function () {
                $price.fadeOut(200, function () {
                    $price.text(originalPrice).fadeIn(200);
                });
            }
        );
    });
}); 