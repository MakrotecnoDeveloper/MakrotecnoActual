using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Services
{
    public class QrService : IQrService
    {
        public string GenerarQrBase64PNG(string contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
                contenido = "N/A";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);

                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrBitmap.Save(ms, ImageFormat.Png);
                            var bytes = ms.ToArray();
                            return Convert.ToBase64String(bytes);
                        }
                    }
                }
            }
        }
    }
}