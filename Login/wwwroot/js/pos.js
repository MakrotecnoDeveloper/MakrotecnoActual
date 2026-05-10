let productoPendiente = null;
let cantidadPendiente = 1;
let carrito = [];
let inputActivo = null;
let ultimaFacturaGenerada = null;
let paginaActual = 1;
const tamanoPagina = 24;
let categoriaActual = "";
let textoActual = "";

function formato(num) {
    return Number(num || 0).toLocaleString("es-CO", {
        minimumFractionDigits: 0,
        maximumFractionDigits: 2
    });
}

function colorCategoria(cat) {
    const nombre = (cat || "").toLowerCase();

    if (nombre.includes("embut")) return "danger";
    if (nombre.includes("lact")) return "warning";
    if (nombre.includes("beb")) return "info";
    return "secondary";
}

async function cargarProductos(texto = "", categoria = "", pagina = 1) {
    textoActual = texto || "";
    categoriaActual = categoria || "";
    paginaActual = pagina || 1;

    const params = new URLSearchParams();

    if (textoActual) params.append("texto", textoActual);
    if (categoriaActual) params.append("categoria", categoriaActual);
    params.append("pagina", paginaActual);
    params.append("tamanoPagina", tamanoPagina);

    const r = await fetch(`${POS_URLS.buscarProductos}?${params.toString()}`);
    const data = await r.json();

    renderProductos(data.items || []);
    renderPaginacion(data);
}

function renderCategorias() {
    const cont = document.getElementById("categorias");
    cont.innerHTML = "";

    cont.innerHTML += `
        <button type="button" class="btn btn-secondary btn-lg" onclick="filtrarCategoria('')">
            Todas
        </button>
    `;

    POS_CATEGORIAS.forEach(cat => {
        cont.innerHTML += `
            <button type="button" class="btn btn-${colorCategoria(cat)} btn-lg" onclick="filtrarCategoria('${cat.replace(/'/g, "\\'")}')">
                ${cat}
            </button>
        `;
    });
}

function filtrarCategoria(categoria) {
    categoriaActual = categoria || "";
    paginaActual = 1;
    cargarProductos(document.getElementById("busqueda").value.trim(), categoriaActual, paginaActual);
}

function renderProductos(productos) {
    const cont = document.getElementById("productosUI");
    cont.innerHTML = "";

    if (!productos || productos.length === 0) {
        cont.innerHTML = `<div class="text-muted">No hay productos disponibles.</div>`;
        return;
    }

    productos.forEach(p => {
        cont.innerHTML += `
            <button type="button" class="btn btn-success p-3" onclick="agregarDesdeUI('${p.codigo}')">
                <strong>${p.nombre}</strong><br>
                <small>${p.codigo}</small><br>
                <small>Stock: ${formato(p.stockDisponible)}</small><br>
                $${formato(p.valorVenta)}
            </button>
        `;
    });
}

async function obtenerProductoPorCodigo(codigo) {
    const r = await fetch(`${POS_URLS.buscarCodigo}?codigo=${encodeURIComponent(codigo)}`);

    if (!r.ok) return null;

    return await r.json();
}

async function agregarProductoDesdeBusqueda() {
    const valor = document.getElementById("busqueda").value.trim();
    const cantidad = parseFloat(document.getElementById("cantidad").value) || 1;

    if (!valor) return;

    const producto = await obtenerProductoPorCodigo(valor);

    if (!producto) {
        alert("Producto no encontrado en esta sede.");
        return;
    }

    productoPendiente = producto;
    cantidadPendiente = cantidad;

    const modal = new bootstrap.Modal(document.getElementById("modalIVA"));
    modal.show();
}

async function agregarDesdeUI(codigo) {
    const producto = await obtenerProductoPorCodigo(codigo);

    if (!producto) {
        alert("Producto no encontrado en esta sede.");
        return;
    }

    productoPendiente = producto;
    cantidadPendiente = 1;

    const modal = new bootstrap.Modal(document.getElementById("modalIVA"));
    modal.show();
}

function confirmarIVA() {
    const iva = parseFloat(document.getElementById("inputIVA").value) || 0;
    procesarProductoConIVA(iva);
}

function cancelarIVA() {
    procesarProductoConIVA(0);
}

function procesarProductoConIVA(iva) {
    if (!productoPendiente) return;

    const producto = productoPendiente;
    const cantidad = cantidadPendiente;

    let existente = carrito.find(x => x.codigo === producto.codigo && Number(x.iva) === Number(iva));

    if (existente) {
        if ((existente.cantidad + cantidad) > producto.stockDisponible) {
            alert(`Stock insuficiente. Disponible: ${producto.stockDisponible}`);
            return;
        }

        existente.cantidad += cantidad;
        existente.total = existente.cantidad * existente.precio;
        existente.valorIva = (existente.total * existente.iva) / 100;
    } else {
        if (cantidad > producto.stockDisponible) {
            alert(`Stock insuficiente. Disponible: ${producto.stockDisponible}`);
            return;
        }

        let total = producto.valorVenta * cantidad;
        let valorIva = (total * iva) / 100;

        carrito.push({
            codigo: producto.codigo,
            nombre: producto.nombre,
            precio: Number(producto.valorVenta || 0),
            valorUnidad: Number(producto.valorUnidad || 0),
            categoria: producto.categoria || "Sin categoría",
            stockDisponible: Number(producto.stockDisponible || 0),
            cantidad: cantidad,
            total: total,
            iva: iva,
            valorIva: valorIva
        });
    }

    const modal = bootstrap.Modal.getInstance(document.getElementById("modalIVA"));
    if (modal) modal.hide();

    productoPendiente = null;
    cantidadPendiente = 1;

    document.getElementById("inputIVA").value = "";
    document.getElementById("busqueda").value = "";
    document.getElementById("cantidad").value = 1;
    document.getElementById("busqueda").focus();

    renderCarrito();
    calcularPagos();
}

function renderCarrito() {
    let tbody = document.getElementById("carrito");
    tbody.innerHTML = "";

    let subtotal = 0;
    let ivaTotal = 0;
    let total = 0;

    carrito.forEach((p, i) => {
        subtotal += p.total;
        ivaTotal += p.valorIva;
        total += (p.total + p.valorIva);

        tbody.innerHTML += `
            <tr>
                <td>
                    ${p.nombre}<br>
                    <small>${p.codigo}</small><br>
                    <small>IVA (${p.iva}%): $${formato(p.valorIva)}</small>
                </td>
                <td>${formato(p.cantidad)}</td>
                <td>$${formato(p.total + p.valorIva)}</td>
                <td>
                    <button type="button" class="btn btn-danger btn-sm" onclick="eliminarItem(${i})">X</button>
                </td>
            </tr>
        `;
    });

    document.getElementById("subtotal").innerText = formato(subtotal);
    document.getElementById("ivaTotal").innerText = formato(ivaTotal);
    document.getElementById("total").innerText = formato(total);
}

function eliminarItem(index) {
    carrito.splice(index, 1);
    renderCarrito();
    calcularPagos();
}

function totalCarrito() {
    return carrito.reduce((acc, p) => acc + p.total + p.valorIva, 0);
}

function calcularPagos() {
    let total = totalCarrito();

    let nequi = parseFloat(document.getElementById("nequi")?.value) || 0;
    let tarjeta = parseFloat(document.getElementById("tarjeta")?.value) || 0;
    let efectivo = parseFloat(document.getElementById("efectivo")?.value) || 0;
    let credito = parseFloat(document.getElementById("credito")?.value) || 0;

    let suma = nequi + tarjeta + efectivo + credito;
    let faltante = total - suma;

    let input = document.getElementById("faltante");

    if (faltante > 0) {
        input.value = "Faltan: $" + formato(faltante);
        input.style.color = "red";
    } else if (faltante < 0) {
        input.value = "Devuelta: $" + formato(Math.abs(faltante));
        input.style.color = "green";
    } else {
        input.value = "Pago exacto";
        input.style.color = "blue";
    }

    input.style.fontWeight = "bold";
}

async function facturar() {
    let idCliente = parseInt(document.getElementById("idCliente").value || "0");
    let conceptos = document.getElementById("conceptos").value.trim();

    if (!idCliente) {
        alert("Debe seleccionar un cliente.");
        return;
    }

    if (carrito.length === 0) {
        alert("No hay productos en el carrito.");
        return;
    }

    let total = totalCarrito();

    let nequi = parseFloat(document.getElementById("nequi")?.value) || 0;
    let tarjeta = parseFloat(document.getElementById("tarjeta")?.value) || 0;
    let efectivo = parseFloat(document.getElementById("efectivo")?.value) || 0;
    let credito = parseFloat(document.getElementById("credito")?.value) || 0;
    let fechaCredito = document.getElementById("fechaVencimientoCredito")?.value || "";

    let suma = nequi + tarjeta + efectivo + credito;

    if (suma < total) {
        alert("Pago incompleto.");
        return;
    }

    if (credito > 0 && !fechaCredito) {
        alert("Debe indicar la fecha de vencimiento del crédito.");
        return;
    }

    const payload = {
        idCliente: idCliente,
        conceptos: conceptos,
        montoNequi: nequi,
        montoTarjeta: tarjeta,
        montoEfectivo: efectivo,
        montoCredito: credito,
        fechaVencimientoCredito: credito > 0 ? `${fechaCredito}T00:00:00` : null,
        items: carrito.map(p => ({
            codigo: p.codigo,
            cantidad: p.cantidad,
            ivaPorcentaje: p.iva
        }))
    };

    const btnFacturar = document.querySelector('button[onclick="facturar()"]');
    if (btnFacturar) btnFacturar.disabled = true;

    try {
        const r = await fetch(POS_URLS.facturar, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        const raw = await r.text();

        let data;
        try {
            data = JSON.parse(raw);
        } catch {
            console.error("Respuesta no JSON del servidor:", raw);
            alert("Error interno del servidor. Revisa la consola F12 o el detalle de la excepción.");
            return;
        }

        if (!r.ok || !data.ok) {
            alert(data.mensaje || "No fue posible facturar.");
            return;
        }

        ultimaFacturaGenerada = data;

        // Genera ticket antes de limpiar
        //generarTicketLocal(data.numeroFactura, total, nequi, tarjeta, efectivo, credito);

        // Limpia POS sin recargar la página
        limpiarPosDespuesDeVenta();

        // Mensaje final
        alert(data.mensaje || "Venta facturada correctamente.");
    }
    catch (error) {
        console.error("Error en facturar():", error);
        alert("Ocurrió un error inesperado al facturar.");
    }
    finally {
        if (btnFacturar) btnFacturar.disabled = false;
    }
}

function limpiarPosDespuesDeVenta() {
    carrito = [];
    productoPendiente = null;
    cantidadPendiente = 1;

    [
        "busqueda",
        "conceptos",
        "nequi",
        "tarjeta",
        "efectivo",
        "credito",
        "faltante",
        "fechaVencimientoCredito"
    ].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = "";
    });

    const cantidad = document.getElementById("cantidad");
    if (cantidad) cantidad.value = 1;

    const idCliente = document.getElementById("idCliente");
    if (idCliente) idCliente.value = "";

    renderCarrito();
    calcularPagos();
    toggleFechaCredito();

    const inputBusqueda = document.getElementById("busqueda");
    if (inputBusqueda) inputBusqueda.focus();
}

function generarTicketLocal(numeroFactura, total, nequi, tarjeta, efectivo, credito) {
    let ticket = document.getElementById("ticket");
    ticket.classList.remove("d-none");

    let productosHTML = carrito.map(p =>
        `${p.nombre}<br>x${formato(p.cantidad)} - $${formato(p.total + p.valorIva)}`
    ).join("<br><br>");

    ticket.innerHTML = `
        <h5>MAKROTECNO POS</h5>
        <p>Factura: ${numeroFactura || ''}</p>
        <hr>
        ${productosHTML}
        <hr>
        <strong>TOTAL: $${formato(total)}</strong><br><br>
        Nequi: $${formato(nequi)} <br>
        Tarjeta: $${formato(tarjeta)} <br>
        Efectivo: $${formato(efectivo)} <br>
        Crédito: $${formato(credito)} <br>
        <hr>
        <button onclick="window.print()" class="btn btn-dark btn-sm w-100">Imprimir</button>
    `;
}

function nuevaVenta() {
    carrito = [];
    productoPendiente = null;
    cantidadPendiente = 1;
    ultimaFacturaGenerada = null;

    ["busqueda", "conceptos", "nequi", "tarjeta", "efectivo", "credito", "faltante", "fechaVencimientoCredito"].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = "";
    });

    const cantidad = document.getElementById("cantidad");
    if (cantidad) cantidad.value = 1;

    const idCliente = document.getElementById("idCliente");
    if (idCliente) idCliente.value = "";

    renderCarrito();
    calcularPagos();
    toggleFechaCredito();

    document.getElementById("busqueda").focus();
}

function escribirNumero(num) {
    if (!inputActivo) return;
    inputActivo.value = (inputActivo.value || "") + num;
    calcularPagos();
}

function borrarNumero() {
    if (!inputActivo) return;
    inputActivo.value = inputActivo.value.slice(0, -1);
    calcularPagos();
}

function limpiarNumero() {
    if (!inputActivo) return;
    inputActivo.value = "";
    calcularPagos();
}

window.onload = () => {
    renderCategorias();
    cargarProductos("", "", 1);

    ["nequi", "tarjeta", "efectivo", "credito"].forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.addEventListener("focus", () => inputActivo = el);
            el.addEventListener("input", calcularPagos);
        }
    });

    if (creditoInput) {
        creditoInput.addEventListener("input", toggleFechaCredito);
        toggleFechaCredito();
    }

    const inputBusqueda = document.getElementById("busqueda");
    inputBusqueda.focus();

    inputBusqueda.addEventListener("keydown", async function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            await agregarProductoDesdeBusqueda();
        }
    });

    inputBusqueda.addEventListener("input", function () {
        paginaActual = 1;
        cargarProductos(this.value.trim(), categoriaActual, paginaActual);
    });
};
function renderPaginacion(data) {
    const cont = document.getElementById("paginacionProductos");
    if (!cont) return;

    cont.innerHTML = "";

    const totalPaginas = data.totalPaginas || 0;
    const pagina = data.paginaActual || 1;

    if (totalPaginas <= 1) return;

    cont.innerHTML += `
        <button type="button" class="btn btn-outline-secondary"
            ${pagina <= 1 ? "disabled" : ""}
            onclick="cargarProductos(textoActual, categoriaActual, ${pagina - 1})">
            Anterior
        </button>
    `;

    let inicio = Math.max(1, pagina - 2);
    let fin = Math.min(totalPaginas, pagina + 2);

    for (let i = inicio; i <= fin; i++) {
        cont.innerHTML += `
            <button type="button"
                class="btn ${i === pagina ? "btn-primary" : "btn-outline-primary"}"
                onclick="cargarProductos(textoActual, categoriaActual, ${i})">
                ${i}
            </button>
        `;
    }

    cont.innerHTML += `
        <button type="button" class="btn btn-outline-secondary"
            ${pagina >= totalPaginas ? "disabled" : ""}
            onclick="cargarProductos(textoActual, categoriaActual, ${pagina + 1})">
            Siguiente
        </button>
    `;
}

const creditoInput = document.getElementById("credito");
const bloqueFechaCredito = document.getElementById("bloqueFechaCredito");
const fechaVencimientoCredito = document.getElementById("fechaVencimientoCredito");

function toggleFechaCredito() {
    if (!creditoInput || !bloqueFechaCredito || !fechaVencimientoCredito) return;

    const montoCredito = parseFloat(creditoInput.value || "0");
    const mostrar = montoCredito > 0;

    bloqueFechaCredito.style.display = mostrar ? "" : "none";

    if (!mostrar) {
        fechaVencimientoCredito.value = "";
    }
}