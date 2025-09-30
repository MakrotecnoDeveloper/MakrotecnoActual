document.addEventListener("DOMContentLoaded", function () {
    new Splide(".splide", {
        type: "loop",
        drag: "free",
        focus: "center",
        autoScroll: {
            speed: 2,   // Velocidad de desplazamiento
            pauseOnHover: true, // Se pausa al pasar el mouse
            pauseOnFocus: true, //pausa cuando un slide recibe foco.
        },
        arrows: true, // Si no quieres flechas
        pagination: true, // Si no quieres puntos
    }).mount(window.splide.Extensions); // Monta Splide + AutoScroll
});