using Microsoft.EntityFrameworkCore;
using Plataforma.Helpers;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class PromocionesService : IPromocionesService
    {
        private readonly BaseAdmContext _dbContext;

        public PromocionesService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MkPromociones>> GetListAsync()
        {
            try
            {
                return await _dbContext.MKPromociones.Where(x => x.Estado == true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(HttpErrorMessages.INTERNAL_ERROR + "///FIND");
            }
        }
        public async Task<MkPromociones?> GetByIdAsync(int id)
        {
            try
            {
                if (id == 0) throw new Exception(HttpErrorMessages.MISSING_VALUES + "///PK");
                var oPromo = await _dbContext.MKPromociones.FirstOrDefaultAsync(p => p.Id == id);
                return oPromo;
            }
            catch (Exception ex)
            {
                throw new Exception(HttpErrorMessages.NO_RECORDS_FOUND);
            }
        }
        public async Task<MkPromociones> CrearAsync(MkPromociones nuevo)
        {
            try
            {
                _dbContext.MKPromociones.Add(nuevo);
                await _dbContext.SaveChangesAsync();
                return nuevo;
            }
            catch (Exception ex)
            {
                throw new Exception(HttpErrorMessages.INTERNAL_ERROR + "///Create");
            }
        }

        public async Task<MkPromociones?> EditarAsync(int id, MkPromociones cambios)
        {
            try
            {
                if (id == 0) throw new Exception(HttpErrorMessages.MISSING_VALUES);

                var oEditOld = await GetByIdAsync(id);

                if (oEditOld == null) throw new Exception(HttpErrorMessages.NO_RECORDS_FOUND);

                oEditOld.NombreArchivo = cambios.NombreArchivo;
                oEditOld.Estado = cambios.Estado;
                oEditOld.Descripcion = cambios.Descripcion;
                await _dbContext.SaveChangesAsync();
                return oEditOld;
            }
            catch (Exception ex)
            {
                throw new Exception(HttpErrorMessages.INTERNAL_ERROR + "//EDIT");
            }
        }
        public async Task<bool> EliminarAsync(int id)
        {

            try
            {
                if (id == 0) throw new Exception(HttpErrorMessages.MISSING_VALUES);

                MkPromociones oDel = await _dbContext.MKPromociones.FirstOrDefaultAsync(x => x.Id == id);

                if (oDel == null) throw new Exception(HttpErrorMessages.NO_RECORDS_FOUND);

                _dbContext.MKPromociones.Remove(oDel);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

                throw new Exception(HttpErrorMessages.INTERNAL_ERROR);

            }
        }


    }
}
