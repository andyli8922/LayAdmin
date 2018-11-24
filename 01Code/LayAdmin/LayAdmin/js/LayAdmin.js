layui.use(['layer', 'jquery', 'form', 'element', 'laytpl'], function () {
    var layer = layui.layer,
        $ = layui.jquery,
        element = layui.element,
        laytpl = layui.laytpl
        form = layui.form;

    //触发事件
    var tab = {
        tabAdd: function (title, url, id) {
            //新增一个Tab项
            element.tabAdd('xbs_tab', {
                title: title
              , content: '<iframe tab-id="' + id + '" frameborder="0" src="' + url + '" scrolling="yes" class="x-iframe"></iframe>'
              , id: id
            })
        }
          , tabDelete: function (othis) {
              //删除指定Tab项
              element.tabDelete('xbs_tab', '44'); //删除：“商品管理”


              othis.addClass('layui-btn-disabled');
          }
          , tabChange: function (id) {
              //切换到指定Tab项
              element.tabChange('xbs_tab', id); //切换到：用户管理
          }
    };
    //隐藏左侧菜单
    $('.container .left_open i').click(function (event) {
        if ($('.left-nav').css('left') == '0px') {
            $('.left-nav').animate({ left: '-221px' }, 100);
            $('.page-content').animate({ left: '0px' }, 100);
            $('.page-content-bg').hide();
        } else {
            $('.left-nav').animate({ left: '0px' }, 100);
            $('.page-content').animate({ left: '221px' }, 100);
            if ($(window).width() < 768) {
                $('.page-content-bg').show();
            }
        }
    });
    //左侧菜单效果
    $('.left-nav #nav li').click(function (event) {
        if ($(this).children('.sub-menu').length) {
            if ($(this).hasClass('open')) {
                $(this).removeClass('open');
                $(this).find('.nav_right').html('&#xe697;');
                $(this).children('.sub-menu').stop().slideUp();
                $(this).siblings().children('.sub-menu').slideUp();
            } else {
                $(this).addClass('open');
                $(this).children('a').find('.nav_right').html('&#xe6a6;');
                $(this).children('.sub-menu').stop().slideDown();
                $(this).siblings().children('.sub-menu').stop().slideUp();
                $(this).siblings().find('.nav_right').html('&#xe697;');
                $(this).siblings().removeClass('open');
            }
        } else {
            var url = $(this).children('a').attr('_href');
            var title = $(this).find('cite').html();
            var index = $('.left-nav #nav li').index($(this));

            for (var i = 0; i < $('.x-iframe').length; i++) {
                if ($('.x-iframe').eq(i).attr('tab-id') == index + 1) {
                    tab.tabChange(index + 1);
                    // 重复点击刷新
                    num = index + 1;
                    $('iframe[tab-id=' + num + ']').attr('src', $('iframe[tab-id=' + num + ']').attr('src'));
                    event.stopPropagation();
                    return;
                }
            };
            tab.tabAdd(title, url, index + 1);
            tab.tabChange(index + 1);
        }
        event.stopPropagation();
    })
});