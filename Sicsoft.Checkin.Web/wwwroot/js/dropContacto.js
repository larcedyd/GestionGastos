$(function () {
    $('.summernote').summernote();
    $(".select2").select2({
        theme: 'bootstrap4',
    });

    $('.custom-file-input').on('change', function () {
        let fileName = $(this).val().split('\\').pop();
        $(this).next('.custom-file-label').addClass("selected").html(fileName);
    });

    $('.i-checks').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
    });
})


Dropzone.options.dzForm = {
    init: function () {
        var dzForm = this;
        let mockFile = { name: "Contacto", size: 36000 };
        dzForm.emit("addedfile", mockFile);
        let callback = null; // Optional callback when it's done
        let crossOrigin = null; // Added to the `img` tag for crossOrigin handling
        let resizeThumbnail = true; // Tells Dropzone whether it should resize the image first

        dzForm.emit("thumbnail", mockFile, "@Model.Contacto.ImageFullPath", callback, crossOrigin, resizeThumbnail);

        dzForm.on("addedfile", function (file) {
        //    alert(file);
        ///    console.log(file);
        ///    console.log(file.dataURL);

          ///  let doc = file;
         ///   console.log(doc.upload);
          //  alert(doc.dataURL);
            // dzForm.removeFile();
            //dzForm.removeFile();

                 //$("div.dz-image").remove();
                 let mockFile = { name: "Contacto", size: 36000};
                 dzForm.emit("addedfile", mockFile);
                let callback = null; // Optional callback when it's done
               let crossOrigin = null; // Added to the `img` tag for crossOrigin handling
            let resizeThumbnail = true; // Tells Dropzone whether it should resize the image first
            dzForm.pop(file[0]);
            dzForm.emit("thumbnail", mockFile, doc.dataURL, callback, crossOrigin, resizeThumbnail);
            dzForm.removeFile(file);
        });
        //dzForm.on("maxfilesexceeded", function (file) { this.removeFile(file); });
    },


    uploadMultiple: false,
    dictDefaultMessage: "Arrastra o click aqui"

}

$('#linkEliminate').click(function () {
    $('div.dz-success').remove();
   // $("div.dz-image").remove();
   // $("div.dz-preview dz-image-preview").remove();

});