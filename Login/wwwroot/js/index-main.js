$(document).ready(function () {
    $('.categoria-link').click(function (e) {
        e.preventDefault(); // Previene el comportamiento predeterminado del enlace
        var categoria = $(this).data('categoria');

        $.ajax({
            url: '@Url.Action("traerProductoXCategoria", "Producto")',
            type: 'GET',
            data: { categoria: categoria },
            success: function (data) {
                $('#productos-container').html(data);
            },
            error: function () {
                alert('Error al cargar los productos.');
            }
        });
    });
});