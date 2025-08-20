using MediatR;
using ProductManagement.Core.Common;

namespace ProductManagement.Application.Features.Products.Commands
{
    public class DeleteProductCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}
