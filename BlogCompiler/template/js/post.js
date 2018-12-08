var post = function (obj) {
    $(obj.onLoad);
    return obj;
}({
    onLoad: function () {
        var modal = document.getElementById('myModal');
        $("img").on("click", function () {
            $("#ImgModal").show();
            $("#ImgModal .modal-content").attr("src", this.src);
            //$("body").css("position","fixed");
        });
        $("#ImgModal").on("click", function () {
            //$("body").css("position", "");
            $("#ImgModal").hide();
        });
        $("#ImgModal .close").on("click", function () {
            //$("body").css("position", "");
            $("#ImgModal").hide();
        });
    }
});