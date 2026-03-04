namespace Plataforma.Domain.Enums
{
    public enum PedidoErrorCode
    {
        UsuarioSinCedula = 1,
        UsuarioSinSede = 2,
        SedeSinPdv = 3,
        StockInsuficiente = 4,
        InventarioNoEncontrado = 5,
        VentaNoExiste = 6,
        VentaNoModificable = 7,
        EstadoInvalido = 8,
        CedulaNoValida = 9,
        SedeNoEncontrada = 10
    }
}
