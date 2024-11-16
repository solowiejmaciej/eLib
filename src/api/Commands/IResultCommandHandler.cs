using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Commands;

public interface IResultCommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse, Error>>
    where TCommand : IResultCommand<TResponse>;