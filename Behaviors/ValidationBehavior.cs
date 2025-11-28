using MediatR;
using FluentValidation;

namespace Assessment.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    //IPipelineBehavior is like a checkpoint that runs between request and handler, allowing me to
    // inject custom logic like validation before the actual handler is called
    //TRequest , TResponse means it works for any request and response type
    //eventually, TRequest will be RegisterCommand, TResponse will be RegisterResult
    // because they are placeholders, they will be replaced with the actual types at runtime
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;


    // gets all validators for the request type via dependency injection
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }



    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    //request is the command, eg: RegisterCommand
    //next is the handler that processes the command, eg: RegisterCommandHandler
    // if validation passes, we call next() to continue to the handler
    // if validation fails, we throw an exception
    // if we DON'T call next(), the handler will never be called (the command won't be processed, stuck)
    {
        // if there are no validators, just continue with the request
        if (!_validators.Any())
        {
            return await next();
        }

        //run all of the validators on the request
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // collect all of the errors
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();
        
        //if there are any errors, throw an exception
        //we throw ValidationException (specifically) ASP.NET Core will automatically catch the error
        //and convert it to a proper 400 Bad Request response with the validation errors in the response body

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        //if there are no errors, continue with the request
        return await next();
    }
}