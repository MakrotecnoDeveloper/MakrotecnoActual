using Plataforma.Domain.Enums;

namespace Plataforma.Domain.Exceptions
{
    public class PedidoException : Exception
    {
        public PedidoErrorCode Code { get; }

        public PedidoException(PedidoErrorCode code, string message)
            : base(message)
        {
            Code = code;
        }
    }
}
