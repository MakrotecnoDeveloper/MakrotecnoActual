// --- Traer productos reales desde Razor ---
const productos = window.modelo.productos || [];
const categorias = window.modelo.categoriaProductos || [];

function renderCategoriasExportacion() {
    const contenedor = document.getElementById("contenedorCategorias");
    if (!contenedor) return;

    contenedor.innerHTML = "";

    if (!categorias || categorias.length === 0) {
        contenedor.innerHTML = `<div class="text-muted">No hay categorías disponibles.</div>`;
        return;
    }

    categorias.forEach(cat => {
        const item = document.createElement("div");
        item.className = "form-check mb-1";
        item.innerHTML = `
            <input class="form-check-input categoria-exportar"
                   type="checkbox"
                   value="${cat.idCateProducto}"
                   id="cat_${cat.idCateProducto}"
                   disabled>
            <label class="form-check-label" for="cat_${cat.idCateProducto}">
                ${cat.descripcion}
            </label>
        `;

        contenedor.appendChild(item);
    });
}

function actualizarEstadoCategorias() {
    const checkTodasCategorias = document.getElementById("checkTodasCategorias");
    const checksCategorias = document.querySelectorAll(".categoria-exportar");

    if (!checkTodasCategorias) return;

    if (checkTodasCategorias.checked) {
        checksCategorias.forEach(chk => {
            chk.checked = false;
            chk.disabled = true;
        });
    } else {
        checksCategorias.forEach(chk => {
            chk.disabled = false;
        });
    }
}

document.addEventListener("DOMContentLoaded", function () {
    renderCategoriasExportacion();

    const checkTodasCategorias = document.getElementById("checkTodasCategorias");
    const checkTodosCampos = document.getElementById("checkTodosCampos");

    actualizarEstadoCategorias();

    if (checkTodasCategorias) {
        checkTodasCategorias.addEventListener("change", function () {
            actualizarEstadoCategorias();
        });
    }

    if (checkTodosCampos) {
        checkTodosCampos.addEventListener("change", function () {
            const checks = document.querySelectorAll(".campo-exportar");
            checks.forEach(c => c.checked = this.checked);
        });
    }

    document.addEventListener("change", function (e) {
        if (e.target.classList.contains("campo-exportar")) {
            const todos = document.querySelectorAll(".campo-exportar");
            const marcados = document.querySelectorAll(".campo-exportar:checked");
            const checkTodos = document.getElementById("checkTodosCampos");

            if (checkTodos) {
                checkTodos.checked = todos.length === marcados.length;
            }
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    renderCategoriasExportacion();

    const checkTodasCategorias = document.getElementById("checkTodasCategorias");
    const checkTodosCampos = document.getElementById("checkTodosCampos");

    if (checkTodasCategorias) {
        checkTodasCategorias.addEventListener("change", function () {
            const checks = document.querySelectorAll(".categoria-exportar");
            checks.forEach(c => c.checked = this.checked);
        });
    }

    if (checkTodosCampos) {
        checkTodosCampos.addEventListener("change", function () {
            const checks = document.querySelectorAll(".campo-exportar");
            checks.forEach(c => c.checked = this.checked);
        });
    }

    document.addEventListener("change", function (e) {
        if (e.target.classList.contains("categoria-exportar")) {
            const todas = document.querySelectorAll(".categoria-exportar");
            const marcadas = document.querySelectorAll(".categoria-exportar:checked");
            document.getElementById("checkTodasCategorias").checked = todas.length === marcadas.length;
        }

        if (e.target.classList.contains("campo-exportar")) {
            const todos = document.querySelectorAll(".campo-exportar");
            const marcados = document.querySelectorAll(".campo-exportar:checked");
            document.getElementById("checkTodosCampos").checked = todos.length === marcados.length;
        }
    });
});

function exportarExcelPersonalizado() {
    const todasCategorias = document.getElementById("checkTodasCategorias").checked;

    let categoriasSeleccionadas = [];
    if (!todasCategorias) {
        categoriasSeleccionadas = Array.from(document.querySelectorAll(".categoria-exportar:checked"))
            .map(x => x.value);

        if (categoriasSeleccionadas.length === 0) {
            alert("Debes seleccionar al menos una categoría o marcar 'Todas las categorías'.");
            return;
        }
    }

    const camposSeleccionados = Array.from(document.querySelectorAll(".campo-exportar:checked"))
        .map(x => x.value);

    if (camposSeleccionados.length === 0) {
        alert("Debes seleccionar al menos un campo para exportar.");
        return;
    }

    let url = "/Producto/ExportarExcelPersonalizado?";
    url += "todasCategorias=" + todasCategorias;
    url += "&categorias=" + encodeURIComponent(categoriasSeleccionadas.join(","));
    url += "&campos=" + encodeURIComponent(camposSeleccionados.join(","));

    window.location.href = url;
}

const categoriasMap = {};
categorias.forEach(cat => {
    categoriasMap[cat.idCateProducto] = cat;
});

  // --- Variables de paginación ---
  let currentPage = 1;
  const perPage = 20;
  let filteredData = [...productos];

  // --- Funciones asíncronas ---
  async function cargarProductos(page = 1, data = productos) {
    return new Promise((resolve) => {
      setTimeout(() => {
        const start = (page - 1) * perPage;
        const end = start + perPage;
        const slice = data.slice(start, end);
        resolve(slice);
      }, 200);
    });
  }

async function mostrarProductos(lista) {
    const grid = document.getElementById("grid");
    grid.innerHTML = "";

      const esRol6 = window.userData?.esRol6; // viene del backend
    lista.forEach(p => {

        const categoria = categoriasMap[p.idCatepro];
        const nombreCategoria = categoria?.descripcion || "Sin categoría";
        const nombreServicio = categoria?.servicio?.nombreServicio || "Sin servicio";

          const estado = p.cantidadProducto === 0 ? "none" : p.cantidadProducto < 3 ? "low" : "ok";

          const card = document.createElement("div");
          card.className = "card";

          card.innerHTML = `
            <img src="${p.imagenPath}" alt="${p.nombreProducto}">
            <h3>${p.nombreProducto}</h3>
            <p>Código: ${p.cod_Producto}</p>
            <p>Categoría: ${nombreCategoria}</p>
            <p>Servicio: ${nombreServicio}</p>
            <p class="stock ${estado}">
              ${p.cantidadProducto === 0 ? "Sin stock" : p.cantidadProducto < 3 ? "Casi sin stock (" + p.cantidadProducto + ")" : "Stock: " + p.cantidadProducto}
            </p>
            <button class="btn btn-info" onclick="location.href='${editarProductoUrl}?searchTerm=${p.cod_Producto}'">
              Editar Producto
            </button>
            ${esRol6 ? `<button class="btn btn-danger" onclick="location.href='${eliminarProductoUrl}?id=${p.cod_Producto}'">Eliminar</button>` : ""}
          `;
      grid.appendChild(card);
    });

    // Actualizar info de paginación
    const pageInfo = document.getElementById("pageInfo");
    const totalPages = Math.ceil(filteredData.length / perPage);
    pageInfo.textContent = `Página ${currentPage} de ${totalPages}`;
    document.getElementById("prevBtn").disabled = currentPage === 1;
    document.getElementById("nextBtn").disabled = currentPage >= totalPages;
  }

  async function updatePage() {
    const data = await cargarProductos(currentPage, filteredData);
    mostrarProductos(data);
  }

  function nextPage() {
    const totalPages = Math.ceil(filteredData.length / perPage);
    if (currentPage < totalPages) {
      currentPage++;
      updatePage();
    }
  }

  function prevPage() {
    if (currentPage > 1) {
      currentPage--;
      updatePage();
    }
  }

  // --- Filtros ---
  async function filterBy(tipo) {
    currentPage = 1;
    switch(tipo) {
      case 'stock': filteredData = productos.filter(p => p.cantidadProducto > 2); break;
      case 'low': filteredData = productos.filter(p => p.cantidadProducto > 0 && p.cantidadProducto < 3); break;
      case 'none': filteredData = productos.filter(p => p.cantidadProducto === 0); break;
      case 'categoria': filteredData = productos.filter(p => p.categoria === "Periféricos"); break;
      case 'servicio': filteredData = productos.filter(p => p.servicio === "Mantenimiento"); break;
      case 'codigo': filteredData = productos.slice(0, 1000); break;
      case 'nombre': filteredData = productos; break;
      default: filteredData = productos;
    }
    updatePage();
  }

  async function searchProducts() {
    const term = document.getElementById("searchInput").value.toLowerCase();
    currentPage = 1;
    filteredData = productos.filter(p =>
      p.nombreProducto.toLowerCase().includes(term) ||
      p.cod_Producto.toLowerCase().includes(term)
    );
    updatePage();
  }

  // Inicial
  (async () => {
    updatePage();
  })();