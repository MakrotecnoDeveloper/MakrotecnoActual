using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Plataforma.Models;
public class ContactFormModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100)]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Debe ser un correo válido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El mensaje es obligatorio")]
    [StringLength(1000, ErrorMessage = "El mensaje no puede superar los 1000 caracteres.")]
    [RegularExpression(@"^[^<>]*$", ErrorMessage = "El mensaje no puede contener caracteres como < o >.")]
    public string Message { get; set; }

    public IFormFile Attachment { get; set; }

}
