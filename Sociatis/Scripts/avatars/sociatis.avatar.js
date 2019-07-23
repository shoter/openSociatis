window.Sociatis = window.Sociatis || {};

Sociatis.Avatar = (function () {

    var $cropper;
    var $avatarUpload;
    var $button;
    var uploader;
    var uploadID;
    var entityID;
    var cropper;

    var x, y, width, height;

    var startCroppie = function (data) {
        $button.hide();
        data = data.Data;
        uploadID = data.uploadID;


        var $image = $cropper.find("img");
        var image = $image[0];

        $image.attr("src", data.uploadPath);

        var canvasWidth = 45.0;
        var canvasHeight = 80.0;

        cropper = new Cropper(image, {
            viewMode: 2,
            dragMode: 'move',
            aspectRatio: 1,
            scalable: false,
            rotatable: false,
            zoomable: false,
            minCropBoxWidth: 200,
            minCropBoxHeight: 200,
            maxCropBoxWidth: 300,
            maxCropBoxHeight: 300,
            minCanvasWidth: (document.documentElement.clientWidth * canvasWidth / 100.0),
            minCanvasHeight: (document.documentElement.clientHeight * canvasHeight / 100.0),
            crop: function (e) {
                x = e.detail.x;
                y = e.detail.y;
                width = e.detail.width;
                height = e.detail.height;
            }
        });

        

        setTimeout(function () {
            $cropper.dialog({
                title: "crop image to save it",
                width: 'auto',
                height: 'auto',
                modal: true,
                resizable: false,
                 create: function () {
                    $(this).css("max-height", 40000);
                },
                close: function () {
                    $button.show();
                    cropper.destroy();
                }

            });
            Sociatis.AjaxEnd();
        }, 10);


        
    }

    var onCropFinish = function (data) {
        data = data.Data;

        $.notify(data.msg, "success");
        $(".infoAvatar img").attr("src", data.img);
        Sociatis.AjaxEnd();
        $cropper.dialog("close");
       ;
        
    }

    var crop = function () {
        Sociatis.AjaxBegin();
        var data = {
            x: x,
            y: y,
            width: width,
            height: height,
            entityID: entityID,
            uploadID: uploadID
        };

        var url = Sociatis.GetAdress("CropPicture", "Avatar");

        $.postJSON(url, data, data => {
            Sociatis.HandleJson(data, onCropFinish);
        }, (e) => { Sociatis.AjaxErrorGeneric(e); });
    }

    var init = function ($$button, $$cropper, entID) {
        $button = $($$button);
        $cropper = $($$cropper);
        $cropper.find("button").click(crop);
        $avatarUpload = $button.find("input");
        entityID = entID;

        $avatarUpload.fileupload({
            dataType: 'json',
            progressall: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('div.progress-bar>div').css(
                    'width',
                    progress + '%'
                );
            },
            submit: function () {
                Sociatis.AjaxBegin();
            },
            done: function (e, data) {
                var result = data.result;
                Sociatis.HandleJson(result, startCroppie);
            }
        });
    };

    return {
        Init: function ($$button, $$cropper, entityID) {
            init($$button, $$cropper, entityID);
        }
    };
})();