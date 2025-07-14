
var PhotoUpload = (function () {
    var setBaseImage = function (img) {
        var canvas = document.getElementById('viewport'), context = canvas.getContext('2d');
     
        var base_image = new Image();
        base_image.src = img.src;
        base_image.onload = function () {
            setHeightandWidth(base_image);
            canvas.height = base_image.height;
            canvas.width = base_image.width;
            context.drawImage(base_image, 0, 0, base_image.width, base_image.height);
        }

    }
    var fileUploadChange = function (_this) {
        var file = $(_this)[0].files[0];
        var reader = new FileReader();
        var base_image = new Image();     
        var canvas = document.getElementById('viewport'), context = canvas.getContext('2d');


        reader.onloadend = function () {
            base_image.src = reader.result;
            PhotoUpload.SetBaseImage(base_image);
            setTimeout(function () {
                $('#viewport').Jcrop({
                    onChange: updatePreview,
                    onSelect: updatePreview,
                    allowSelect: true,
                    allowMove: true,
                    allowResize: true,
                    setSelect: [($('#viewport').width() / 2) - 75,
                                                                           ($('#viewport').height() / 2) - 100,
                                                                           ($('#viewport').width() / 2) + 75,
                                                                           ($('#viewport').height() / 2) + 100
                    ],
                    aspectRatio: 3 / 4,
                });
                $("#btnPhotoUpload,#btnPhotoCancel").show();
                $("#btnPhotoDelete").hide();
            }, 500);
           
        }

        var ext = file.name.substring(file.name.lastIndexOf(".") + 1, file.name.length).toLowerCase();
        if (file && (ext == "jpg" || ext == "jpeg" || ext == "png")) {
            reader.readAsDataURL(file);
        }
        else {
            //alert("This file not in allowed format.jpeg,jpg,png files are only allowed !.");
            EduSuite.AlertMessage({ title: Resources.Warning, content: "This file not in allowed format.jpeg,jpg,png files are only allowed !.", type: 'orange' })
        }

    }





    return {
        SetBaseImage: setBaseImage,
        FileUploadChange: fileUploadChange
    }

}());

function updatePreview(c) {
    if (parseInt(c.w) > 0) {
        // Show image preview
        var imageObj = $("#viewport")[0];
        var canvas = $("#canvas")[0];
        var context = canvas.getContext("2d");

        if (imageObj != null && c.x != 0 && c.y != 0 && c.w != 0 && c.h != 0) {
            canvas.height = c.h;
            canvas.width = c.w;
            context.drawImage(imageObj, c.x, c.y, c.w, c.h, 0, 0, canvas.width, canvas.height);
        }
    }
}

function setHeightandWidth(img) {
    var maxWidth = 450; // Max width for the image
    var maxHeight = 600;    // Max height for the image
    var ratio = 0;  // Used for aspect ratio
    var width = img.width;    // Current image width
    var height = img.height;  // Current image height

    // Check if the current width is larger than the max
    if (width > maxWidth) {
        ratio = maxWidth / width;   // get ratio for scaling image
        $(this).css("width", maxWidth); // Set new width
        $(this).css("height", height * ratio);  // Scale height based on ratio
        height = height * ratio;    // Reset height to match scaled image
        width = width * ratio;    // Reset width to match scaled image
    }

    // Check if current height is larger than max
    if (height > maxHeight) {
        ratio = maxHeight / height; // get ratio for scaling image
        $(this).css("height", maxHeight);   // Set new height
        $(this).css("width", width * ratio);    // Scale width based on ratio
        width = width * ratio;    // Reset width to match scaled image
        height = height * ratio;    // Reset height to match scaled image
    }
    img.width = width;
    img.height = height;
}