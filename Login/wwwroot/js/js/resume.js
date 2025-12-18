(function($) {
  "use strict"; // Start of use strict

  // Smooth scrolling using jQuery easing
  $('a.js-scroll-trigger[href*="#"]:not([href="#"])').click(function() {
    if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
      var target = $(this.hash);
      target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
      if (target.length) {
        $('html, body').animate({
          scrollTop: (target.offset().top)
        }, 1000, "easeInOutExpo");
        return false;
      }
    }
  });

  // Closes responsive menu when a scroll trigger link is clicked
  $('.js-scroll-trigger').click(function() {
    $('.navbar-collapse').collapse('hide');
  });

  // Activate scrollspy to add active class to navbar items on scroll
  $('body').scrollspy({
    target: '#sideNav'
  });

})(jQuery); // End of use strict


//Inicio Codigo para PreAgendamiento manejo de calendario
$(document).ready(function () {
    let servicioId = null;
    let nombreProducto = null;
    let fechaSeleccionada = null;

    $(".selectable-service").click(function () {
        servicioId = $(this).data("id");
        nombreProducto = $(this).data("nombre");

        // Reset formulario por si quedó info anterior
        $("#nombreCliente").val("");
        $("#celularCliente").val("");
        $("#datepicker").val("");
        fechaSeleccionada = null;

        $(".popup-overlay, .popup-content").fadeIn();

        // Iniciar Flatpickr al mostrar la ventana
        $("#datepicker").flatpickr({
            enableTime: true,
            dateFormat: "Y-m-d H:i",
            time_24hr: true,
            minDate: "today",
            defaultDate: null,
            onChange: function (selectedDates, dateStr) {
                fechaSeleccionada = dateStr;
                console.log("Fecha seleccionada:", fechaSeleccionada);
            }
        });
    });

    $(".close-popup").click(function () {
        $(".popup-overlay, .popup-content").fadeOut();
    });

    $("#btnConfirmar").click(function () {
        let nombreCliente = $("#nombreCliente").val().trim();
        let celularCliente = $("#celularCliente").val().trim();
        let fechaFinal = $("#datepicker").val().trim();

        if (!nombreCliente || !celularCliente || !fechaSeleccionada) {
            alert("Debes completar todos los campos y seleccionar una fecha.");
            return;
        }
        $.ajax({
            url: "/Home/ValidarFecha",
            method: "POST",
            data: {
                fecha: fechaSeleccionada,
                codProducto: servicioId,
                nombreProducto: nombreProducto,
                nombreCliente: nombreCliente,
                celularCliente: celularCliente
            },
            success: function (response) {
                alert(response.mensaje);

                if (response.success) {
                    $(".popup-overlay, .popup-content").fadeOut();

                    let mensaje = `📅 *Cita Pre-Agendada*%0A🔹 *Servicio:* ${nombreProducto}%0A📅 *Fecha:* ${fechaSeleccionada}%0A👤 *Cliente:* ${nombreCliente}%0A📞 *Celular:* ${celularCliente}%0A⚠️ *Este es un preagendamiento y puede estar sujeto a cambios.*`;

                    let whatsappUrl = `https://wa.me/573195378423?text=${mensaje}`;
                    window.open(whatsappUrl, "_blank");
                }
            }
        });
    });
});
//Fin
