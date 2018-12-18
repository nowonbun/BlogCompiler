var post = function (obj) {
    $(obj.onLoad);
    return obj;
}({
    onLoad: function () {
        var modal = document.getElementById('myModal');
        $("img").on("click", function () {
            $("#ImgModal").show();
            $("#ImgModal .modal-content").attr("src", this.src);
        });
        $("#ImgModal").on("click", function () {
            $("#ImgModal").hide();
        });
        $("#ImgModal .close").on("click", function () {
            $("#ImgModal").hide();
        });
        $("article img").each(function () {
            $parent = $(this).parent();
            $parent.addClass("img-area");
            $(this).before($("<div class='image-title'></div>").css("max-width", $(this).css("max-width"))
                .append($("<i class='fa fa-plus-square code-collapse' style='margin-left:10px;margin-right:10px;'></i>"))
                .append("&nbsp;[이미지 보기]"));
            $parent.addClass("image-view image-view-disabled");
        });
        $("code").each(function () {
            $(this).before($("<div class='code-title'></div>").append($("<i class='fa fa-plus-square code-collapse' style='margin-right:10px;'></i>"))
                .append("&nbsp;[소스 보기]&nbsp;" + $(this).data("type")));
            $(this).parent().addClass("code-view code-view-disabled");
        });
        $(document).on("click", ".code-title", function () {
            $this = $(this);
            $i = $this.find("i.code-collapse");
            if ($i.hasClass("fa-plus-square")) {
                $i.removeClass("fa-plus-square");
                $i.addClass("fa-minus-square");
                $this.parent().removeClass("code-view-disabled");
            } else {
                $i.removeClass("fa-minus-square");
                $i.addClass("fa-plus-square");
                $this.parent().addClass("code-view-disabled");
            }
        });
        $(document).on("click", ".image-title", function () {
            $this = $(this);
            $i = $this.find("i.code-collapse");
            if ($i.hasClass("fa-plus-square")) {
                $i.removeClass("fa-plus-square");
                $i.addClass("fa-minus-square");
                $this.parent().removeClass("image-view-disabled");
            } else {
                $i.removeClass("fa-minus-square");
                $i.addClass("fa-plus-square");
                $this.parent().addClass("image-view-disabled");
            }
        });
    }
});