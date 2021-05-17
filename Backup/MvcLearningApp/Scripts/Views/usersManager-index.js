$(document).ready(function () {
    _getUsers();
    //_editClientEvent();
    _saveClickEvent();
    _deleteClientEvents();
});

var _getUsers = function () {
    _getGrid(1);

    //event la apasarea lui ENTER
    $('#search-place').keypress('search_box', function (e) {
        if (e.which == 13) {
            e.preventDefault();
            var word = $('#search-place').find('input[type="text"]').val();
            _getGrid(1);
        }
    });

    $('#search-place').on('click', 'a', function (e) {
        e.preventDefault();
        var word = $('#search-place').find('input[type="text"]').val();
        _getGrid(1);
    });


    //paginarea
    $('#list-place').on('click', 'a[class^="btn-page"]', function (e) {    //^ startWith
        e.preventDefault();

        page_location = $(this).attr("data-page");
        console.log(page_location); //afisare in consola
        _getGrid(page_location);
    });
};

var _getGrid = function (page) {
    var word = $('#search-place').find('input[type="text"]').val(); //iau valoarea din textBox

    $.ajax(
        {
            url: AjaxUrls.Url_GetListAsync,
            type: "POST",
            data: "page=" + page + "&words=" + word,
            cache: false
        })
        .done(function (data) {
            if (data) {
                if (data.Success === true) {
                    var myDiv = $('#list-place');
                    myDiv.html(data.Html);
                } else {
                    //AICI
                    var myDiv1 = $('#list-place');
                    myDiv1.html(data.Message);
                }
            }
        })
        .always(function () { })
        .fail(function () { });
};       //end _getGrid

   //window.ckOn = false;
var _editClientEvent = function (id, enable) {
    /*if (ckOn) {
    return;
    }
    ckOn = true;*/

    $.ajax(
            {
                url: AjaxUrls.Url_SaveClient,
                type: "POST",
                data: "id=" + id + "&Enable=" + enable,
                cache: false
            })
            .done(function (data) {

            })
            .always(function () {
                //ckOn = true;
                /*$('a[class$= "save"]').css("visibility", "visible");
                if (ckOn === true)
                $('a[class$= "save"]').css("display", ""); */
            })
            .fail(function () {
            });
    //});
};

    var _saveClickEvent = function () {
        //click on save
        $('#list-place').on('click', 'a[class$= "save"]', function (e) {
            e.preventDefault();
            var saveLink = $(e.currentTarget);
            var raw = saveLink.parents('tr').first();
            var idSave = saveLink.parents().data('id');
            var EnableState = raw.find('input[class="toggle-enable"]').prop('checked');
            _editClientEvent(idSave, EnableState);
        });
    };

    var _deleteClientEvents = function () {
        //click on delete
        $('#list-place').on('click', 'a[class$="del"]', function (e) {
            e.preventDefault();
            var myLink = $(e.currentTarget);
            var id = myLink.parents().first().data('id');
            $("#dialog-confirm").dialog({
                resizable: false,
                height: 140,
                modal: true,
                buttons: {
                    "Delete": function () {
                        var errorDiv = $('#error-place');
                        errorDiv.children('p').hide();
                        errorDiv.children('img').show();
                        deleteClient(id);
                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                }
            });
        });
    };              // end _DeleteClientEvents

    var deleteClient = function (id) {
        var errorDiv = $('#error-place');
        var myLink = $('list-place').find('td[class^="btn-del"]');
        var tableRow = myLink.parent().parent();
        $.ajax(
        {    //Delete
            url: AjaxUrls.Url_DeleteAsync,
            type: "POST",
            data: "id=" + id,
            cache: false
        })
        .done(function (data) {
            if (data) {
                if (data.Success === true) {
                    tableRow.remove();
                    _getGrid(1);    //reactualizez paginarea
                    //alert(data.Message);
                } else {
                    $("#dialog-message").dialog("distroy");
                    $("#dialog-message").dialog({
                        title:"Delete User",
                        modal: true,
                        buttons:{
                            Ok:function(){
                                $(this).dialog("close");
                            }
                        }
                    });
                }
            }
        })
        .always(function () {
            errorDiv.children('img').hide();
        })
        .fail(function () { });
    }