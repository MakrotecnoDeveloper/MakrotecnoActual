function openUnifiedModal() {
    new bootstrap.Modal(document.getElementById("unifiedModal")).show();
}


$("#clienteForm").submit(function (e) {
    e.preventDefault();

    const cedula = $("#c_cedula").val();
    const cliente = {
        CedulaCliente: cedula ? parseInt(cedula, 10) : null,
        NombreCliente: $("#c_nombre").val()?.trim() || null,
        EmpresaCliente: $("#c_empresa").val()?.trim() || null,
        CiudadCliente: $("#c_ciudad").val()?.trim() || null,
        TelefonoCliente: $("#c_telefono").val()?.trim() || null,
        CorreoCliente: $("#c_correo").val()?.trim() || null,
        DireccionCliente: $("#c_direccion").val()?.trim() || null
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
    <td>${o.idOrden ?? ''}</td>
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
      <button class="btn btn-sm btn-success nuevaOrdenBtn" data-id="${o.idOrden}">Nuevo Registro</button>
      <button class="btn btn-sm btn-success asignarTecnico" data-id="${o.idOrden}">Asignar Tecnico</button>
      <button class="btn btn-sm btn-info verOrdenBtn" data-id="${o.idOrden}">Ver Registros</button>
    </td>`;
    document.querySelector('#mainTable #tbody').prepend(tr);
}
        //Agregar fila si encuentra la orden
    function fmtDateCO(iso) {
        try { return new Date(iso).toLocaleDateString('es-CO', { day: '2-digit', month: '2-digit', year: 'numeric' }); }
        catch { return iso ?? ""; }
    }
    function fmtTimeCO(iso) {
        try { return new Date(iso).toLocaleTimeString('es-CO', { hour: '2-digit', minute: '2-digit' }); }
        catch { return ""; }
    }

    function estadoPill(texto) {
        const e = (texto || "").toLowerCase();
        let cls = "pill pill-gray";
        if (e.includes("final")) cls = "pill pill-dark";
        else if (e.includes("ejec") || e.includes("proceso")) cls = "pill pill-blue";
        else if (e.includes("pend")) cls = "pill pill-yellow";
        else if (e.includes("rech")) cls = "pill pill-red";
        return `<span class="${cls}">${texto || ""}</span>`;
    }

    function tecnicoPill(cedula) {
        return `<span class="pill pill-outline">${cedula ?? ""}</span>`;
    }

    // Render EXACTO de la tabla nueva: 7 columnas
    function renderOrdenRowV2(o) {
        const id = o.idOrden ?? o.IdOrden ?? "";
        const codigo = `ST-${id}`;
        const fecha = o.fechaIngreso ?? o.FechaIngreso;

        const cliente = (o.cliente ?? o.Cliente ?? "").trim();
        const marca = (o.marca ?? o.Marca ?? "").trim();
        const modelo = (o.modelo ?? o.Modelo ?? "").trim();
        const problema = (o.descripcion ?? o.Descripcion ?? "").trim();
        const estado = (o.estado ?? o.Estado ?? "").trim();
        const cedula = o.cedula ?? o.Cedula ?? "";
        const total = o.total ?? o.Total ?? o.valorPago ?? o.ValorPago ?? 0;

        const puedeEditar = estado !== "Rechazada" && estado !== "Finalizada";

        return `
      <tr>
        <td class="fw-semibold">${codigo}</td>

        <td>
          <div>${fmtDateCO(fecha)}</div>
          <div class="text-muted small">${fmtTimeCO(fecha)}</div>
        </td>

        <td>
          <div class="fw-semibold">${cliente || "-"}</div>
          <div class="text-muted small">${marca}${marca && modelo ? " · " : ""}${modelo}</div>
          ${problema ? `<div class="text-muted small mt-1">Problema: <span class="text-dark">${escapeHtml(problema)}</span></div>` : ""}
        </td>

        <td>${tecnicoPill(cedula)}</td>

        <td>${estadoPill(estado)}</td>

        <td class="text-end">
          <div class="fw-semibold">$ ${Number(total || 0).toLocaleString("es-CO")}</div>
          <div class="text-muted small">estimado</div>
        </td>

        <td class="text-end">
          <div class="d-inline-flex gap-2 justify-content-end">
            <button class="icon-btn verOrdenBtn" data-id="${id}" title="Ver">
              <i class="bi bi-eye"></i>
            </button>

            ${puedeEditar ? `
              <button class="icon-btn icon-btn-blue modOrdenBtn" data-id="${id}" title="Editar">
                <i class="bi bi-pencil"></i>
              </button>

              <button class="icon-btn icon-btn-green nuevaOrdenBtn" data-id="${id}" title="Nuevo registro">
                <i class="bi bi-plus-lg"></i>
              </button>

              <button class="icon-btn asignarTecnico" data-id="${id}" title="Asignar técnico">
                <i class="bi bi-person"></i>
              </button>
            ` : ``}
          </div>
        </td>
      </tr>`;
    }

    function escapeHtml(str) {
        return (str || "")
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#039;");
    }


    function mostrarSoloOrdenBuscada(o) {
        $("#tbody").html(renderOrdenRow(o));
    }

    function restaurarUltimas10() {
        // Simple: recargar la página para volver a las 10 últimas (rápido y sin complicarte)
        location.reload();
    }

    $(document).on("click", "#btnBuscarOrden", function () {
        const idOrden = $("#buscarIdOrden").val();
        if (!idOrden) return alert("Ingresa un ID de orden válido.");

        $("#btnBuscarOrden").prop("disabled", true).text("Buscando...");

        $.get("/OrdenServicio/BuscarOrdenPorId", { idOrden })
            .done(function (res) {
                if (!res.success) {
                    alert(res.message || "No se pudo traer la orden.");
                    return;
                }
                mostrarSoloOrdenBuscada(res.orden);
            })
            .fail(function () {
                alert("Error consultando la orden ❌");
            })
            .always(function () {
                $("#btnBuscarOrden").prop("disabled", false).text("🔎 Buscar");
            });
    });

    // Enter para buscar
    $(document).on("keydown", "#buscarIdOrden", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            $("#btnBuscarOrden").click();
        }
    });

    // Si borras el input, vuelve a las 10 últimas
    $(document).on("input", "#buscarIdOrden", function () {
        if ($(this).val() === "") {
            restaurarUltimas10();
        }
    });

    //Fin Agregar fila si encuentra la orden


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

// ===================== Cambiar de tab por JS =====================
function irATab(tabSelector) {
    const btn = document.querySelector(tabSelector);
    if (!btn) return;
    bootstrap.Tab.getOrCreateInstance(btn).show();
}

// ===================== Detalle visual del dispositivo seleccionado =====================
$(document).on("change", "#m_dispositivo", function () {
    const opt = this.options[this.selectedIndex];
    const detalle = opt?.getAttribute("data-detalle") || "Sin detalle";
    $("#detalleDispositivoSel").text(detalle);
});

// ===================== Checkbox: "No existe dispositivo" => Ir a Dispositivo =====================
$(document).on("change", "#chkNoExisteDispositivo", function () {
    if (this.checked) {
        $("#chkNoExisteCliente").prop("checked", false);
        irATab("#dispositivo-tab");
        setTimeout(() => $("#d_cliente").focus(), 150);
    }
});

// ===================== Checkbox: "No existe cliente" => Ir a Cliente =====================
$(document).on("change", "#chkNoExisteCliente", function () {
    if (this.checked) {
        $("#chkNoExisteDispositivo").prop("checked", false);
        irATab("#cliente-tab");
        setTimeout(() => $("#c_nombre").focus(), 150);
    }
});

// ===================== Botones rápidos =====================
$(document).on("click", "#btnIrADispositivo", function () {
    $("#chkNoExisteDispositivo").prop("checked", false);
    $("#chkNoExisteCliente").prop("checked", false);
    irATab("#dispositivo-tab");
    setTimeout(() => $("#d_cliente").focus(), 150);
});

$(document).on("click", "#btnIrACliente", function () {
    $("#chkNoExisteDispositivo").prop("checked", false);
    $("#chkNoExisteCliente").prop("checked", false);
    irATab("#cliente-tab");
    setTimeout(() => $("#c_nombre").focus(), 150);
});

// ===================== UX: al abrir modal, siempre arranca en Orden =====================
function openUnifiedModal() {
    const modalEl = document.getElementById("unifiedModal");
    new bootstrap.Modal(modalEl).show();

    // forzar el tab Orden al abrir
    setTimeout(() => {
        irATab("#orden-tab");
        $("#chkNoExisteDispositivo").prop("checked", false);
        $("#chkNoExisteCliente").prop("checked", false);
        $("#detalleDispositivoSel").text("Selecciona un dispositivo para ver el detalle.");
        $("#m_dispositivo").focus();
    }, 150);
}

// ===================== Limpieza de checks al guardar (recomendado) =====================
// Cuando guardas cliente, en tu success actual (CrearCliente) agrega:
// $("#chkNoExisteCliente").prop("checked", false);

// Cuando guardas dispositivo, en tu success actual (CrearDispositivo) agrega:
// $("#chkNoExisteDispositivo").prop("checked", false);

function calcularKPIsTabla() {
    const rows = $("#tbody tr");
    $("#kpiTotalOrdenes").text(rows.length);

    let enProceso = 0;
    let finalizadas = 0;

    rows.each(function () {
        const estado = $(this).find("td:eq(4) .badge").text().trim(); // columna Estado
        if (estado === "Finalizada") finalizadas++;
        else if (estado && estado !== "Rechazada") enProceso++;
    });

    $("#kpiEnProceso").text(enProceso);
    $("#kpiFinalizadas").text(finalizadas);
}

$(document).ready(function () {
    calcularKPIsTabla();
});

function cargarFiltros() {
    $.get("/OrdenServicio/ObtenerFiltros")
        .done(function (res) {
            if (!res.success) return;

            // Estados
            const selEstado = $("#filtroEstado");
            selEstado.empty();
            selEstado.append(new Option("Todos los estados", "__ALL__"));
            (res.estados || []).forEach(e => selEstado.append(new Option(e, e)));

            // Técnicos
            const selTec = $("#filtroTecnico");
            selTec.empty();

            if (window.__ROL__ === "Administrador") {
                selTec.append(new Option("Todos los técnicos", "__ALL__"));
            }

            (res.tecnicos || []).forEach(t => {
                selTec.append(new Option(t.nombre, t.cedula));
            });

            // Si es técnico: selecciona su cédula y bloquea el combo
            if (window.__ROL__ !== "Administrador") {
                selTec.val(String(window.__CEDULA__));
                selTec.prop("disabled", true);
            }
        })
        .fail(function () {
            console.log("No se pudieron cargar filtros");
        });
}

function pintarTablaV2(lista) {
    const html = (lista || []).map(o => renderOrdenRowV2(o)).join("");
    $("#tbody").html(html);
}

function aplicarFiltros() {
    const estado = $("#filtroEstado").val() || "__ALL__";

    let tecnico = $("#filtroTecnico").val() || "__ALL__";
    if (tecnico === "__ALL__") tecnico = 0;

    $.get("/OrdenServicio/FiltrarOrdenes", { estado, tecnico })
        .done(function (res) {
            if (!res.success) return alert("No se pudo filtrar");
            pintarTablaV2(res.ordenes || []);
            calcularKPIsTabla(); // si la tienes
        })
        .fail(function () {
            alert("Error filtrando ❌");
        });
}


// Eventos
$(document).on("change", "#filtroEstado", aplicarFiltros);
$(document).on("change", "#filtroTecnico", aplicarFiltros);

// Inicial
$(document).ready(function () {
    cargarFiltros();
});

function mostrarSoloOrdenBuscada(o) {
    $("#tbody").html(renderOrdenRowV2(o));
    calcularKPIsTabla();
}

