$(document).ready(function () {
    var n = 0;
    $('#search-product-btn').on('click', function () {
        $('#search-box').addClass('active');
        $('#results-box').removeClass('active');
        $('#results').html('');
        n = 1;
    });
    $('#sinstock-product-btn').on('click', function () {
        $('#search-box').addClass('active');
        $('#results-box').removeClass('active');
        $('#results').html('');
        n = 2;
    });
    $('#proxsinstock-product-btn').on('click', function () {
        $('#search-box').addClass('active');
        $('#results-box').removeClass('active');
        $('#results').html('');
        n = 3;
    });

    $('#close-search-box').on('click', function () {
        $('#search-box').removeClass('active');
    });

    $('#search-btn').on('click', function () {
        var searchTerm = $('#product-code').val();
        var categoriaTerm = $('#category').val();
        switch (n) {
            case 1:
                $.ajax({
                    url: '@Url.Action("Buscar", "Producto")',
                    type: 'GET',
                    data: {
                        searchTerm: searchTerm,
                        categoriaTerm: categoriaTerm
                    },
                    success: function (data) {
                        $('#search-box').removeClass('active');
                        $('#results-box').addClass('active');
                        $('#results').html(data);
                    },
                    error: function () {
                        $('#results').append('<div>Error al buscar productos.</div>');
                    }
                });
                break;
            case 2:
                $.ajax({
                    url: '@Url.Action("BuscarSinStock", "Producto")',
                    type: 'GET',
                    data: {
                        __RequestVerificationToken: $('[name="__RequestVerificationToken"]').val(),
                        searchTerm: searchTerm,
                        categoriaTerm: categoriaTerm
                    },
                    success: function (data) {
                        $('#search-box').removeClass('active');
                        $('#results-box').addClass('active');
                        $('#results').html(data);
                    },
                    error: function () {
                        $('#results').append('<div>Error al buscar productos.</div>');
                    }
                });
                break;
            case 3:
                $.ajax({
                    url: searchNextNoStok,
                    type: 'GET',
                    data: {
                        __RequestVerificationToken: $('[name="__RequestVerificationToken"]').val(),
                        searchTerm: searchTerm,
                        categoriaTerm: categoriaTerm
                    },
                    success: function (data) {
                        $('#search-box').removeClass('active');
                        $('#results-box').addClass('active');
                        $('#results').html(data);
                    },
                    error: function () {
                        $('#results').append('<div>Error al buscar productos.</div>');
                    }
                });
                break;
            default:
                console.log("No ha seleccionado una opcion");
                break;
        }
    });
});