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
        .then(res => {
            if (!res.ok) throw new Error("Error al crear cliente");
            return res.json();
        })
        .then(data => {
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
        .then(res => {
            if (!res.ok) throw new Error("Error al crear dispositivo");
            return res.json();
        })
        .then(data => {
            alert("Dispositivo agregado correctamente");
            bootstrap.Modal.getInstance(document.getElementById("deviceModal")).hide();
            document.getElementById("deviceForm").reset();
        })
        .catch(err => alert(err.message));
});


// ================== CREAR ORDEN DE SERVICIO ==================
document.getElementById("rowForm").addEventListener("submit", function (e) {
    e.preventDefault();

    const orden = {
        Fecha: document.getElementById("m_fecha").value,
        Cliente: document.getElementById("m_cliente").value,
        Telefono: document.getElementById("m_telefono").value,
        Password: document.getElementById("m_password").value,
        Marca: document.getElementById("m_marca").value,
        Modelo: document.getElementById("m_modelo").value,
        Descripcion: document.getElementById("m_descripcion").value,
        MedioPago: document.getElementById("m_mediopago").value,
        Proveedor: document.getElementById("m_proveedor").value,
        Observacion: document.getElementById("m_observacion").value,
        VRepuesto: document.getElementById("m_vrepuesto").value,
        VServicio: document.getElementById("m_vservicio").value,
        Total: document.getElementById("m_total").value
    };

    fetch("/OrdenServicio/CrearOrden", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(orden)
    })
        .then(res => {
            if (!res.ok) throw new Error("Error al crear la orden");
            return res.json();
        })
        .then(data => {
            alert("Orden de servicio creada correctamente");
            bootstrap.Modal.getInstance(document.getElementById("rowModal")).hide();
            document.getElementById("rowForm").reset();
        })
        .catch(err => alert(err.message));
});

// ============================== Autocompletado de Descripcion ====================== //
document.getElementById('m_cliente').addEventListener('change', function () {
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

    // =================== Cerrar Panel ======================= //
    $("#closePanel").click(function () {
        $("#sidePanel").fadeOut();
    });

});
