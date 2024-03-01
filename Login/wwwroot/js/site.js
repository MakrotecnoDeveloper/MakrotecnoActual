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