using FluentValidation;
using MediatR;
using ProductManagement.Core.Common;

namespace ProductManagement.Application.Common
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Any())
                {
                    var errors = failures.Select(f => f.ErrorMessage).ToList();

                    // Return validation error result
                    if (typeof(TResponse).IsGenericType)
                    {
                        var resultType = typeof(TResponse).GetGenericTypeDefinition();
                        if (resultType == typeof(Result<>))
                        {
                            var genericType = typeof(TResponse).GetGenericArguments()[0];
                            var method = typeof(Result<>).MakeGenericType(genericType)
                                .GetMethod("Failure", new[] { typeof(List<string>) });

                            return (TResponse)method!.Invoke(null, new object[] { errors })!;
                        }
                    }
                    else if (typeof(TResponse) == typeof(Result))
                    {
                        return (TResponse)(object)Result.Failure(errors);
                    }
                }
            }

            return await next();
        }
    }
}
