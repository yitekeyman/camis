var mobile=(function () {
    (function ($) {
        $(document).ready(function () {
            $WIN = $(window);
            var toggleButton = $('.header-menu-toggle'),
                nav = $('.header-nav-wrap');

            toggleButton.on('click', function (event) {
                event.preventDefault();

                toggleButton.toggleClass('is-clicked');
                nav.slideToggle();
            });

            if (toggleButton.is(':visible')) nav.addClass('mobile');

            $WIN.on('resize', function () {
                if (toggleButton.is(':visible')) nav.addClass('mobile');
                else nav.removeClass('mobile');
            });


        });
    })(jQuery);
});

var mobile2=(function () {
    (function ($) {
        $(document).ready(function () {
            $WIN = $(window);
            var toggleButton = $('.header-menu-toggle'),
                nav = $('.header-nav-wrap');
                if (nav.hasClass('mobile')) {
                    toggleButton.toggleClass('is-clicked');
                    nav.slideToggle();
                }

        });
    })(jQuery);
});

var imageViewer=(function () {
    (function ($) {
        $(document).ready(function () {
            // gallery category
            $('.templatemo-gallery-category a').click(function(e){
                e.preventDefault();
                $(this).parent().children('a').removeClass('active');
                $(this).addClass('active');
                var linkClass = $(this).attr('href');
                $('.gallery').each(function(){
                    if($(this).is(":visible") == true){
                        $(this).hide();
                    };
                });
                $(linkClass).fadeIn();
            });

            //gallery light box setup
            $('a.colorbox').colorbox({
                rel: function(){
                    return $(this).data('group');

                }
            });

        });
    })(jQuery);
});

