system.formValide = function (form) {
    form.validate();
    if (!form.valid()) {
        return false;
    } else {
        return true;
    }
}

system.sendForm = function (url, form, callback, btn) {
    system.btnDisabled(btn);
    let formData = new FormData($(form)[0]);
    $.ajax({
        url: url,
        type: 'POST',
        processData: false,
        contentType: false,
        data: formData,
        success: callback,
        error: function (xhr, resp, text) {
            system.btnEnabled(btn);
            Swal.fire("Error!", xhr.responseText, "error");
        }
    });
};


system.btnDisabled = function (btn) {
    if (btn != null) {
        $(btn).prop('disabled', true);
        $(btn).append('<div class="spinner-border text-secondary" role"status">' +
            '<span class="sr-only">Cargando...</span>' +
            '</div>');
    }
}

system.btnEnabled = function (btn) {
    if (btn != null) {
        $(btn).prop('disabled', false)
        $(btn).find(".spinner-border").each(function () {
            $(this).remove();
        });
    }
}