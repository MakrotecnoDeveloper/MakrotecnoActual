let index = 1;
function insertarFilasCrearPedido() {
    //Filas dinamicas
    const nuevaFila = `
                <tr>
                    <td>
                        <input type="text" class="form-control codigoProducto" name="Productos[${index}].Codigo" placeholder="Código" id="codfact${index}">
                        <div class="opcionesCodigosProducto"></div>
                    </td>
                    <td>
                        <input type="number" class="form-control" name="Productos[${index}].Stock" placeholder="Cantidad" id="stock${index}">
                    </td>
                    <td>
                        <input type="number" class="form-control" name="Productos[${index}].VNeto" placeholder="Venta Neto" id="vneto${index}">
                    </td>
                    <td>
                        <input type="text" class="form-control" name="Productos[${index}].VVenta" readonly placeholder="Valor Venta" id="vventa${index}">
                    </td>
                    <td>
                        <input type="number" class="form-control" name="Productos[${index}].VTotal" readonly placeholder="Valor Total" id="vtotal${index}">
                    </td>
                    <td>
                        <button type="button" class="btn btn-danger btn-sm" onclick="eliminarFila(this)">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
    $('#tablaProductos').append(nuevaFila);
    index = index + 1;
}

// Función para mostrar las opciones de autocompletado
function mostrarOpcionesAutocompletado(opciones, container) {
    var opcionesHtml = '';
    opciones.forEach(function (opcion) {
        opcionesHtml += '<div class="opcion">' + opcion.value + '</div>';
    });
    container.html(opcionesHtml);
}

// Manejar el evento de entrada en el campo de código de producto dinámico
$(document).on('input', '.codigoProducto', function () {
    var inputActual = $(this); // Input que se está editando
    var codigo = inputActual.val(); // Valor del input actual
    var contenedorOpciones = inputActual.siblings('.opcionesCodigosProducto'); // Contenedor asociado a este input

    if (codigo.length >= 3) {
        // Llamada AJAX para obtener las opciones de autocompletado
        $.ajax({
            url: '/Compras/BuscarProductoPorCodigo',
            type: 'GET',
            data: { codigo: codigo },
            success: function (response) {
                // Mostrar las opciones en el contenedor asociado
                mostrarOpcionesAutocompletado(response, contenedorOpciones);
            },
            error: function () {
                console.error('Error al obtener los códigos.');
                contenedorOpciones.empty(); // Limpiar opciones en caso de error
            }
        });
    } else {
        contenedorOpciones.empty(); // Limpiar opciones si el texto es muy corto
    }
});

// Manejar la selección de una opción de autocompletado
$(document).on('click', '.opcion', function () {
    var codigoSeleccionado = $(this).text(); // Obtener el texto de la opción seleccionada
    var contenedorOpciones = $(this).closest('.opcionesCodigosProducto'); // Contenedor asociado
    var inputCodigo = contenedorOpciones.siblings('.codigoProducto'); // Input asociado
    var fila = contenedorOpciones.closest('tr'); // Fila de la tabla donde se encuentra el input

    // Asignar el código seleccionado al input
    inputCodigo.val(codigoSeleccionado);
    contenedorOpciones.empty(); // Limpiar las opciones de autocompletado

    // Llamada AJAX para obtener los detalles del producto y autocompletar los campos
    $.ajax({
        url: '/Compras/BuscarProductoPorCodigo',
        type: 'GET',
        data: { codigo: codigoSeleccionado },
        success: function (data) {
            if (Array.isArray(data) && data.length > 0) {
                var producto = data[0];
                // Completar los campos 'vneto' y 'vventa' en la fila correspondiente
                fila.find('#vneto' + fila.index()).val(producto.valorNeto);
                fila.find('#vventa' + fila.index()).val(producto.valorVenta);
                console.log(producto.valorVenta);
            }
        },
        error: function () {
            console.error('Error al obtener los datos del producto.');
        }
    });
});
// Cálculo dinámico del valor total al editar 'Stock' o 'VVenta'
$(document).on('input', '[id^="stock"], [id^="vneto"], [id^="vventa"]', function () {
    var fila = $(this).closest('tr'); // Fila actual
    var tabla = fila.closest('table'); // Tabla actual

    var stock = parseFloat(fila.find('[id^="stock"]').val()) || 0; // Valor de stock
    var vventa;

    if (tabla.data('tipo') === 'compra') {
        vventa = parseFloat(fila.find('[id^="vneto"]').val()) || 0;
    } else if (tabla.data('tipo') === 'venta') {
        vventa = parseFloat(fila.find('[id^="vventa"]').val()) || 0;
    }

    // Calcular y asignar el valor total
    var vtotal = stock * vventa;
    fila.find('[id^="vtotal"]').val(vtotal.toFixed(2)); // Mostrar con 2 decimales
});

function AcumularProductosDePedido() {
    let subtotal = 0;

    // Recorrer cada fila de la tabla para sumar los valores totales
    $('#tablaProductos tr').each(function () {
        let vtotal = parseFloat($(this).find('input[name$=".VTotal"]').val()) || 0; // Leer el valor total
        subtotal += vtotal; // Acumular el valor total en el subtotal
    });

    // Actualizar el campo subtotal
    $('#subtotalFactura').val(subtotal.toFixed(2));

    // Leer valores de IVA y descuento
    let ivaPorcentaje = parseFloat($('#ivaFactura').val()) || 0; // Leer el porcentaje de IVA, si está vacío, será 0
    let descuento = parseFloat($('#descuentoFactura').val()) || 0; // Leer el descuento, si está vacío, será 0

    // Calcular el IVA
    let iva = (subtotal * ivaPorcentaje) / 100;

    // Calcular el total con descuento e IVA
    let total = subtotal + iva - descuento;

    // Actualizar los campos de IVA y total
    $('#ivaFactura').val(iva.toFixed(2));
    $('#totalFactura').val(total.toFixed(2));
}