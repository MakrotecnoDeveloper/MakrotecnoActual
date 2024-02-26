    // Agrega un controlador de eventos al botón con ID "btnAgregarProducto"
    $("#btnAgregarProducto").click(function () {
        // Obtiene los valores de los campos
        var id_empresa = $("#id_empresa").val();
        var codigo = $("#codigo").val();
        var descripcion = $("#descripcion").val();
        var valorNeto = $("#valorNeto").val();
        var valorVenta = $("#valorVenta").val();
        var stock = $("#stock").val();
        var categoria = $("#categorias").val();
        // Llama a la función para enviar el producto
        enviarProducto(id_empresa, codigo, descripcion, valorNeto, valorVenta, stock, categoria);
    });
// Función para enviar un producto
function enviarProducto(id_empresa, codigo, descripcion, valorNeto, valorVenta, stock, categoria) {
    // Crea un objeto con los datos del producto
    var data = {
        id_empresa: id_empresa,
        codigo: codigo,
        descripcion: descripcion,
        valorNeto: valorNeto,
        valorVenta: valorVenta,
        stock: stock,
        categoria: categoria
    };
    // Realiza la solicitud AJAX
    console.log(data);
    $.ajax({
        type: "POST",
        url: "/Producto/Insertar", // Ajusta la URL según tu ruta
        data: data,
        success: function (response) {
            // Lógica para manejar el éxito
            alert("Producto agregado exitosamente.");
        },
        error: function (error) {
            // Lógica para manejar el error
            alert("Error al agregar el producto.");
        }
    });
}
//Fin codigo

        $(document).ready(function () {
            $('#buscarBtn').on('click', function () {
                // Obtén los valores de los filtros
                var id_empresa = $('#filtroEmpresa').val();
                var codigo = $('#filtroCodigo').val();
                var descripcion = $('#filtroDescripcion').val();
                var valorNeto = parseFloat($('#filtroValorNeto').val());
                var valorVenta = parseFloat($('#filtroValorVenta').val());
                var stock = parseInt($('#filtroStock').val());
                var categoria = $('#filtroCategoria').val();

                // Realiza la solicitud AJAX
                $.ajax({
                    url: '/Producto/BuscarProductosAsync',
                    type: 'POST',
                    data: {
                        id_empresa: id_empresa,
                        codigo: codigo,
                        descripcion: descripcion,
                        valorNeto: valorNeto,
                        valorVenta: valorVenta,
                        stock: stock,
                        categoria: categoria
                    },
                    success: function (resultados) {
                        // Actualiza la tabla con los resultados
                        actualizarTabla(resultados);
                    },
                    error: function (error) {
                        console.log(error);
                    }
                });
            });

        function actualizarTabla(resultados) {
            // Implementa lógica para actualizar la tabla con los resultados
            // Puedes usar el mismo código que tienes en tu script actualizado para mostrar los resultados
            // Asegúrate de adaptar el código según tus necesidades
            console.log(resultados);
        }
    });