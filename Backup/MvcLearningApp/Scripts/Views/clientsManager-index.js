/// <reference path="jquery-1.7.1-vsdoc.js" />

$(document).ready(function () {
    _initData();
    _registerDelegates();
});     //end document ready

var _initData = function () {
    _getClientEvents();
};

var _registerDelegates = function () {
    _createClientEvents();

    _deleteClientEvents();
    _editClientEvents();
};   //end _registerDelegates

var _getClientEvents = function () {
    _getGrid(1);

    //event la apasarea lui ENTER
    $('#search-place').keypress('search_box', function (e) {
        if (e.which == 13) {
            e.preventDefault();
            var word = $('#search-place').find('input[type="text"]').val();
            _getGrid(1);
        }
    });
    // controlul textBox-ului de search
    /*var search_click = $('#search-place').children('a');
    search_click.on('click', function (e) {*/
    //echivalente 
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
                var myDiv = $('#list-place');
                if (data.Success === true) {
                    myDiv.html(data.Html);
                    _editClientEvents();    //atentie
                } else {
                    //AICI
                    myDiv.html(data.Message);
                }
            }
        })
        .always(function () { }).fail(function () { });
};    //end _getGrid

var _editClientEvents = function () {
    $('#clientsTable').on('click', 'a.btn-edit', function (e) {     //dependent cu atentie
        //$('#list-place').on('click', 'a.btn-edit', function (e) {     //echivalent cu ce e sus fara dependenta
        e.preventDefault();
        var editLink = $(e.currentTarget);
        var idEdit = editLink.parents().first().data('id');
        var raw = editLink.parents('tr').first();
        var copilRand_Name = raw.children('td:nth-child(1)');
        var copilRand_Date = raw.children('td:nth-child(2)');
        var copilRand_Email = raw.children('td:nth-child(3)');

        if (editLink.data('mode') == 'on') {
            editLink.data('mode', 'off');
            editLink.text('Update');

            //make row editable
            var nameVal = copilRand_Name.text().trim();     //trims => elimin spatiile albe
            copilRand_Name.html('<input type="text" name="name_box" style="float:left; width: 100px;" value="' + nameVal + '">');
            var dateVal = copilRand_Date.text().trim();
            copilRand_Date.html('<input type="text" name="date_box" style="float:left; width: 170px;" value="' + dateVal + '">');
            var mailVal = copilRand_Email.text().trim();
            copilRand_Email.html('<input type="text" name="email_box" style="float:left; width: 100px;" value="' + mailVal + '">');

            copilRand_Date.find('input[type="text"]').datepicker({});

        } else {
            //get new values
            var newName = copilRand_Name.find('input[type="text"]').val();
            var newDate = copilRand_Date.find('input[type="text"]').val();
            var newMail = copilRand_Email.find('input[type="text"]').val();

            //call server
            $.ajax(
                {
                    url: AjaxUrls.Url_SaveClient,
                    type: "POST",
                    data: "id=" + idEdit + "&name=" + newName + "&date=" + newDate + "&mail=" + newMail,
                    cache: false
                }).
                done(function (data) {
                    if (data) {
                        var nameVal = copilRand_Name.find('input[type="text"]').val();
                        var dateVal = copilRand_Date.find('input[type="text"]').val();
                        var mailVal = copilRand_Email.find('input[type="text"]').val();
                        var errorDiv = $('#error-place');

                        if (nameVal == "" || dateVal == "" || mailVal == "") {
                            errorDiv.children('p:nth-child(3)').next();
                            errorDiv.children('p:nth-child(3)').show('slow');
                            var timer = setInterval(function () {
                                errorDiv.children('p:nth-child(3)').slideUp(200);
                            }, 4000);
                        }
                        else {
                            errorDiv.children('p:nth-child(3)').hide();
                            $('table_row').first().html(data.Html);  // adaug un rand in tabel
                            editLink.data('mode', 'on');
                            editLink.text('Edit');
                            copilRand_Name.html(nameVal);
                            copilRand_Date.html(dateVal);
                            copilRand_Email.html(mailVal);
                        }
                    }
                }).
                always(function () {
                }).
                fail(function () {
                });
        }
    });
};                                            //end _editClientEvent

var _createClientEvents = function () {
    //show create form
    $('#btn-show-create').on('click', function (e) {
        e.preventDefault();
        var dialogHolder = $("#dialog-form-create");
        if (dialogHolder.data('dialog')) {
            dialogHolder.dialog('open');
        }
        else {
            dialogHolder.dialog({
                autoOpen: true,
                height: 370,
                width: 370,
                modal: true,
                buttons: {
                    "Create client": function () {
                        var form = $('#dialog-form-create form');
                        var info = form.serialize();
                        //var picture = $('#avatar').val();
                        $.ajax(
                        {   //Create
                            url: AjaxUrls.Url_CreateAsync,
                            type: "POST",
                            data: info,// + "&avatar=" + picture,
                            cache: false
                        })
                        .done(function (data) {
                            if (data) {
                                if (data.Success === true) {
                                    $('#list-place tbody').prepend(data.Html);  // adaug un rand in tabel
                                    //$('#btn-show-create').trigger('click');     -->                                                                                         //execut un event prin alt event
                                }
                                if (data.Form) {
                                    $('#dialog-form-create').html(data.Form);
                                }
                            }
                        })
                        .always(function () {})
                        .fail(function () {});
                        $('#dialog-form-create').dialog("close");
                        _getGrid(1);
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                },
                close: function () {
                    $('#name').val("");
                    $('#email').val("");
                    $('#phone').val("");
                    $('#avatar').val("");
                }
            });
        }
    });
};                   //end _createClientEvents

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
    var tr = myLink.parent().parent();
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
                    tr.remove();
                    _getGrid(1);    //reactualizez paginarea
                } else {
                    showError(data.Message);
                }
            }
        })
        .always(function () {
            errorDiv.children('img').hide();
        })
        .fail(function () { });
}

var showError = function (msg) {
    var errorDiv = $('#error-place');
    errorDiv.children('p').html(msg);
    errorDiv.children('p').show('slow');

    var timer = setInterval(function () {
        errorDiv.children('p').slideUp(200);
    }, 4000);
};