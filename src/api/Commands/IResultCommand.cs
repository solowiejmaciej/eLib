using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Commands;

public interface IResultCommand<TResponse> : IRequest<Result<TResponse, Error>>;