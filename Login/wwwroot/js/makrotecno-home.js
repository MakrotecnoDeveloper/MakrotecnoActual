// makrotecno-home.js
(() => {
    // Menú móvil
    const menuToggle = document.getElementById("menuToggle");
    const mobileMenu = document.getElementById("mobileMenu");
    let menuOpen = false;

    if (menuToggle && mobileMenu) {
        menuToggle.addEventListener("click", () => {
            menuOpen = !menuOpen;
            mobileMenu.style.display = menuOpen ? "block" : "none";
        });
    }

    // Buscador
    const searchInput = document.getElementById("searchInput");
    const serviceCards = Array.from(document.querySelectorAll(".service-item"));

    if (searchInput) {
        searchInput.addEventListener("input", (e) => {
            const q = (e.target.value || "").trim().toLowerCase();
            serviceCards.forEach(card => {
                const name = (card.dataset.name || "").toLowerCase();
                const desc = (card.dataset.desc || "").toLowerCase();
                card.style.display = (name + " " + desc).includes(q) ? "" : "none";
            });
        });
    }

    // Modal
    const backdrop = document.getElementById("modalBackdrop");
    const modalTitle = document.getElementById("modalTitle");
    const modalDesc = document.getElementById("modalDesc");
    const modalImg = document.getElementById("modalImg");
    const modalClose = document.getElementById("modalClose");
    const modalSeeMore = document.getElementById("modalSeeMore");

    function openModal({ title, desc, url, img }) {
        if (!backdrop) return;

        if (modalTitle) modalTitle.textContent = title || "Servicio";
        if (modalDesc) modalDesc.textContent = desc || "";
        if (modalSeeMore) modalSeeMore.setAttribute("href", url || "#");

        if (modalImg) {
            modalImg.src = img || "";
            modalImg.alt = title ? `Imagen de ${title}` : "Imagen del servicio";
        }

        backdrop.style.display = "flex";
        document.body.style.overflow = "hidden";
    }

    function closeModal() {
        if (!backdrop) return;
        backdrop.style.display = "none";
        document.body.style.overflow = "";
    }

    serviceCards.forEach(card => {
        card.addEventListener("click", (ev) => {
            // si clic en "Ver más", dejamos navegar
            if (ev.target.closest("a")) return;

            openModal({
                title: card.dataset.name || "Servicio",
                desc: card.dataset.desc || "",
                url: card.dataset.url || "#",
                img: card.dataset.img || ""
            });
        });
    });

    if (backdrop) {
        backdrop.addEventListener("click", (e) => {
            if (e.target === backdrop) closeModal();
        });
    }
    if (modalClose) modalClose.addEventListener("click", closeModal);

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape" && backdrop && backdrop.style.display === "flex") {
            closeModal();
        }
    });
})();



$(document).ready(function () {
    $('.categoria-link').click(function (e) {
        e.preventDefault(); // Previene el comportamiento predeterminado del enlace
        var categoria = $(this).data('categoria');

        $.ajax({
            url: '@Url.Action("traerProductoXCategoria", "Producto")',
            type: 'GET',
            data: { categoria: categoria },
            success: function (data) {
                $('#productos-container').html(data);
            },
            error: function () {
                alert('Error al cargar los productos.');
            }
        });
    });
});