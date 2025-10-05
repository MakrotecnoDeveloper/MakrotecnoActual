function openUnifiedModal() {
    new bootstrap.Modal(document.getElementById("unifiedModal")).show();
}


$("#clienteForm").submit(function (e) {
    e.preventDefault();

    const cliente = {
        CedulaCliente: $("#c_cedula").val(),
        NombreCliente: $("#c_nombre").val(),
        EmpresaCliente: $("#c_empresa").val(),
        CiudadCliente: $("#c_ciudad").val(),
        TelefonoCliente: $("#c_telefono").val(),
        CorreoCliente: $("#c_correo").val(),
        DireccionCliente: $("#c_direccion").val()
    };

    $.ajax({
        url: '/Terceros/CrearCliente',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',  // 👈 obligatorio
        data: JSON.stringify(cliente),                  // 👈 manda JSON
        success: function (data) {
            if (data.success) {
                // 👉 Agregar nuevo cliente al select de dispositivos
                const sel = $("#d_cliente");
                const opt = new Option(data.cliente.nombreCliente, data.cliente.idCliente);
                sel.append(opt);                     // lo añade al final
                sel.val(data.cliente.idCliente);     // lo selecciona

                alert("Cliente agregado con éxito 🚀");
                $("#clienteForm")[0].reset();

                // 👉 Opción: permanecer en el modal pero cambiar a la pestaña Dispositivo
                const tabTrigger = document.querySelector('#dispositivo-tab');
                bootstrap.Tab.getOrCreateInstance(tabTrigger).show();

            } else {
                alert(data.message || "Error en el servidor ❌");
            }
        },
        error: function (xhr) {
            alert("Error al guardar el cliente ❌ " + xhr.status);
        }
    });
});


//Dispositivo
$("#deviceForm").submit(function (e) {
    e.preventDefault();

    const dispositivo = {
        FechaIngreso: $("#d_fecha").val(),
        Marca: $("#d_marca").val(),
        Modelo: $("#d_modelo").val(),
        Detalle: $("#d_detalle").val(),
        IdCliente: $("#d_cliente").val(),
        IMEI: $("#d_imei").val(),
        Clave: $("#d_clave").val(),
        Patron: $("#d_patron").val(),
        TipoDispositivo: $("#d_tipodispositivo").val()
    };

    $.ajax({
        url: '/OrdenServicio/CrearDispositivo',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(dispositivo),
        success: function (data) {
            if (data.success) {
                console.log(data);
                // 👉 Agregar nuevo dispositivo al select de Orden
                const sel = $("#m_dispositivo");
                const optText = `${data.dispositivo.idDispositivo} - ${data.dispositivo.nombreCliente} - ${data.dispositivo.fechaCliente}`;
                const opt = new Option(optText, data.dispositivo.idDispositivo);
                $(opt).attr("data-descripcion", data.dispositivo.detalle); // atributo extra
                sel.append(opt);
                sel.val(data.dispositivo.idDispositivo); // selecciona recién creado

                alert("Dispositivo agregado con éxito 🚀");
                $("#deviceForm")[0].reset();

                // 👉 Pasar a la pestaña Orden
                const tabTrigger = document.querySelector('#orden-tab');
                bootstrap.Tab.getOrCreateInstance(tabTrigger).show();

            } else {
                alert(data.message || "Error en el servidor ❌");
            }
        },
        error: function (xhr) {
            alert("Error al guardar el dispositivo ❌ " + xhr.status);
        }
    });
});

// ================== CREAR ORDEN DE SERVICIO ==================
document.getElementById("rowForm").addEventListener("submit", function (e) {
    e.preventDefault();

    const orden = {
        IdDispositivo: document.getElementById("m_dispositivo").value,
        ProblemaReportado: document.getElementById("m_descripcion").value,
        Estado: document.getElementById("m_estado").value,
        Observaciones: document.getElementById("m_observacion").value
    };

    fetch("/OrdenServicio/CrearOrden", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(orden)
    })
        .then(res => { if (!res.ok) throw new Error("Error al crear la orden"); return res.json(); })
        .then(data => {
            if (!data.success) throw new Error(data.message || "Error");
            appendOrdenRow(data.orden);           // ➜ inserta la fila en caliente
            alert(data.message);
            $("#rowForm")[0].reset();
            // Cerrar modal
            const modalElement = document.getElementById("unifiedModal");
            const modal = bootstrap.Modal.getInstance(modalElement)
                || new bootstrap.Modal(modalElement);
            modal.hide();
        })
        .catch(err => alert(err.message));
});

// ============================== Autocompletado de Descripcion ====================== //
document.getElementById('m_dispositivo').addEventListener('change', function () {
    const selectedOption = this.options[this.selectedIndex];
    const descripcion = selectedOption.getAttribute('data-descripcion') || '';
    document.getElementById('m_descripcion').value = descripcion;
});

// ================= Agregar Fila a la tabla Principal =============
function appendOrdenRow(o) {
    function fmtDateCO(iso) { try { return new Date(iso).toLocaleDateString('es-CO'); } catch { return iso; } }
    const tr = document.createElement('tr');
    tr.innerHTML = `
        <td>${fmtDateCO(o.fechaIngreso)}</td>
        <td>${o.cliente ?? ''}</td>
        <td>${o.telefono ?? ''}</td>
        <td>${o.password ?? ''}</td>
        <td>${o.marca ?? ''}</td>
        <td>${o.modelo ?? ''}</td>
        <td>${o.descripcion ?? ''}</td>
        <td>${o.observacion ?? ''}</td>
        <td>${o.estado ?? ''}</td>
        <td>${o.cedula ?? ''}</td>
        <td>
          <button class="btn btn-sm btn-info modOrdenBtn" data-id="${o.idOrden}">Modificar Orden</button>
          <button class="btn btn-sm btn-info verOrdenBtn" data-id="${o.idOrden}">Ver Registros</button>
          <button class="btn btn-sm btn-success nuevaOrdenBtn" data-id="${o.idOrden}">Nuevo Registro</button>
          <button class="btn btn-sm btn-success asignarTecnico" data-id="${o.idOrden}">Asignar Tecnico</button>
        </td>`;
    document.querySelector('#mainTable #tbody').prepend(tr);
}


// == Ventana flotante para ver el historico de las ordenes y crear un historico ===== //
$(document).ready(function () {

    // Abrir panel para visualizar orden
    $(document).on("click", ".verOrdenBtn", function () {
        const idOrden = $(this).data("id");
        console.log("Click verOrdenBtn, id:", idOrden); // 👈 prueba
        $.get(`/OrdenServicio/DetalleHistOrden/${idOrden}`, function (html) {
            $("#sidePanelTitle").text("Detalle de la Orden");
            $("#sidePanelContent").html(html);
            $("#sidePanel").fadeIn();
        });
    });

    // =================== Abrir panel para nueva orden ======================= //
    $(document).on("click", ".nuevaOrdenBtn", function () {
        const idOrden = $(this).data("id");

        $.get(`/OrdenServicio/CrearHistOrden/${idOrden}`, function (html) {
            $("#sidePanelTitle").text("Crear Nuevo Historico");
            $("#sidePanelContent").html(html);
            $("#sidePanel").fadeIn();
        });
    });

    // =================== Abrir panel para modificar orden ======================= //
    $(document).on("click", ".modOrdenBtn", function () {
        const idOrden = $(this).data("id");

        $.get(`/OrdenServicio/ModOrden/${idOrden}`, function (html) {
            $("#sidePanelTitle").text("Modificar Orden");
            $("#sidePanelContent").html(html);
            $("#sidePanel").fadeIn();
        });
    });

    // =================== Abrir panel para asignar tecnico ======================= //
    $(document).on("click", ".asignarTecnico", function () {
        const idOrden = $(this).data("id");

        $.get(`/OrdenServicio/AsignarEmpleado/${idOrden}`, function (html) {
            $("#sidePanelTitle").text("Asignar Tecnico");
            $("#sidePanelContent").html(html);
            $("#sidePanel").fadeIn();
        });
    });

    // =================== Cerrar Panel ======================= //
    $("#closePanel").click(function () {
        $("#sidePanel").fadeOut();
    });

});