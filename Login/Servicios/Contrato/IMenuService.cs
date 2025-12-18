using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IMenuService
    {
        //Activo
        Task<List<ModuloMenuDto>> ObtenerMenuEstructuradoPorEmpresaAsync(string idEmpresa, int idCargo);
    }
}