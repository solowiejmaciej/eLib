using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries;

public interface IResultQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, Result<TResult, Error>>
    where TQuery : IResultQuery<TResult>;