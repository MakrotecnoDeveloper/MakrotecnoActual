using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IQrService
    {
        string GenerarQrBase64PNG(string contenido);
    }
}