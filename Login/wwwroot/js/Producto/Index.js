// --- Traer productos reales desde Razor ---
const productos = window.modelo.productos || [];
const categorias = window.modelo.categoriaProductos || [];
const servicio = window.modelo.servicio || null;

// Ejemplo de verificación:
//console.log("Productos cargados:", productos.length);
//console.log("Categorías disponibles:", categorias.length);

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
      console.log(esRol6);
      lista.forEach(p => {
          const estado = p.cantidadProducto === 0 ? "none" : p.cantidadProducto < 3 ? "low" : "ok";

          const card = document.createElement("div");
          card.className = "card";

          card.innerHTML = `
            <img src="${p.imagenPath}" alt="${p.nombreProducto}">
            <h3>${p.nombreProducto}</h3>
            <p>Código: ${p.cod_Producto}</p>
            <p>Categoría: ${p.idCatepro}</p>
            <p>Servicio: ${p.servicio}</p>
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