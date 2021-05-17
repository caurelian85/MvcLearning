 $(document).ready(function () {
        _addClientPicture();
    });


    var _addClientPicture = function () {
        $('#save_img').on('click', function (e) {
            e.preventDefault();
            var data = $('#input_text').val();
            $.ajax({
                url: AjaxUrls.Url_AddPicture,
                type: "POST",
                data: data,
                cache: false
            })
            .done(function (data) {
                if (data) {
                    if (data.Success == true) {
                        //$('').replaceWith();
                    }
                    else {
                        showError(data.Message);
                    }

                }
            })
            .always(function () {
            })
            .fail(function () {
            });
        });
    };