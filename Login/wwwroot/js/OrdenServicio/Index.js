// APERTURA DE MODALS //
document.addEventListener("DOMContentLoaded", function () {
    // Abrir modal Agregar Fila
    document.getElementById("openRowFormBtn").addEventListener("click", function () {
        new bootstrap.Modal(document.getElementById("rowModal")).show();
    });

    // Abrir modal Agregar Dispositivo
    document.getElementById("openDeviceBtn").addEventListener("click", function () {
        new bootstrap.Modal(document.getElementById("deviceModal")).show();
    });

    // Abrir modal Agregar Cliente
    document.getElementById("openClienteBtn").addEventListener("click", function () {
        new bootstrap.Modal(document.getElementById("clienteModal")).show();
    });

});
// FIN APERTURA MODALS //

// ================== CREAR CLIENTE ==================
document.getElementById("clienteForm").addEventListener("submit", function (e) {
    e.preventDefault();
    const cliente = {
        CedulaCliente: document.getElementById("c_cedula").value,
        NombreCliente: document.getElementById("c_nombre").value,
        EmpresaCliente: document.getElementById("c_empresa").value,
        CiudadCliente: document.getElementById("c_ciudad").value,
        TelefonoCliente: document.getElementById("c_telefono").value,
        CorreoCliente: document.getElementById("c_correo").value,
        DireccionCliente: document.getElementById("c_direccion").value
    };

    fetch("/Terceros/CrearCliente", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(cliente)
    })
        .then(res => { if (!res.ok) throw new Error("Error al crear cliente"); return res.json(); })
        .then(data => {
            if (!data.success) throw new Error(data.message || "Error");

            // ➜ Agregar al select de clientes del modal de dispositivo
            const sel = document.getElementById("d_cliente");
            const opt = new Option(data.cliente.nombreCliente, data.cliente.idCliente);
            sel.add(opt);                // lo agrega al final
            sel.value = data.cliente.idCliente; // lo selecciona

            alert("Cliente agregado correctamente");
            bootstrap.Modal.getInstance(document.getElementById("clienteModal")).hide();
            document.getElementById("clienteForm").reset();
        })
        .catch(err => alert(err.message));
});


// ================== CREAR DISPOSITIVO ==================
document.getElementById("deviceForm").addEventListener("submit", function (e) {
    e.preventDefault();

    const dispositivo = {
        FechaIngreso: document.getElementById("d_fecha").value,
        Marca: document.getElementById("d_marca").value,
        Modelo: document.getElementById("d_modelo").value,
        Detalle: document.getElementById("d_detalle").value,
        IdCliente: document.getElementById("d_cliente").value,
        IMEI: document.getElementById("d_imei").value,
        Clave: document.getElementById("d_clave").value,
        Patron: document.getElementById("d_patron").value,
        TipoDispositivo: document.getElementById("d_tipodispositivo").value
    };

    fetch("/OrdenServicio/CrearDispositivo", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(dispositivo)
    })
        .then(res => { if (!res.ok) throw new Error("Error al crear dispositivo"); return res.json(); })
        .then(data => {
            if (!data.success) throw new Error(data.message || "Error");

            if (!data.dispositivo) {
                console.error('Respuesta sin "dispositivo":', data);
                alert(data.message || "Dispositivo creado, pero la respuesta no incluyó el objeto.");
                return;
            }

            const sel = document.getElementById("m_dispositivo");
            const opt = document.createElement('option');
            opt.value = data.dispositivo.idDispositivo;
            opt.textContent = data.dispositivo.imei || '(sin IMEI)';
            opt.setAttribute('data-descripcion', data.dispositivo.detalle || '');
            sel.appendChild(opt);
            sel.value = data.dispositivo.idDispositivo;
            sel.dispatchEvent(new Event('change'));

            alert("Dispositivo agregado correctamente");
            bootstrap.Modal.getInstance(document.getElementById("deviceModal")).hide();
            document.getElementById("deviceForm").reset();
        })
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


// ================== CREAR ORDEN DE SERVICIO ==================
document.getElementById("rowForm").addEventListener("submit", function (e) {
    e.preventDefault();

    const orden = {
        FechaIngreso: document.getElementById("m_fecha").value,
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
            bootstrap.Modal.getInstance(document.getElementById("rowModal")).hide();
            document.getElementById("rowForm").reset();
        })
        .catch(err => alert(err.message));
});

// ============================== Autocompletado de Descripcion ====================== //
document.getElementById('m_dispositivo').addEventListener('change', function () {
    const selectedOption = this.options[this.selectedIndex];
    const descripcion = selectedOption.getAttribute('data-descripcion') || '';
    document.getElementById('m_descripcion').value = descripcion;
});

// == Ventana flotante para ver el historico de las ordenes y crear un historico ===== //
$(document).ready(function () {

    // Abrir panel para visualizar orden
    $(document).on("click", ".verOrdenBtn", function () {
        const idOrden = $(this).data("id");

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
            $("#sidePanelTitle").text("Crear Nueva Orden");
            $("#sidePanelContent").html(html);
            $("#sidePanel").fadeIn();
        });
    });

// =================== Abrir panel para modificar orden ======================= //
    $(document).on("click", ".modOrdenBtn", function () {
        const idOrden = $(this).data("id");

        $.get(`/OrdenServicio/ModOrden/${idOrden}`, function (html) {
            $("#sidePanelTitle").text("Crear Nueva Orden");
            $("#sidePanelContent").html(html);
            $("#sidePanel").fadeIn();
        });
    });

    // =================== Abrir panel para asignar tecnico ======================= //
    $(document).on("click", ".asignarTecnico", function () {
        const idOrden = $(this).data("id");

        $.get(`/OrdenServicio/AsignarEmpleado/${idOrden}`, function (html) {
            $("#sidePanelTitle").text("Crear Nueva Orden");
            $("#sidePanelContent").html(html);
            $("#sidePanel").fadeIn();
        });
    });


    // =================== Cerrar Panel ======================= //
    $("#closePanel").click(function () {
        $("#sidePanel").fadeOut();
    });

});
