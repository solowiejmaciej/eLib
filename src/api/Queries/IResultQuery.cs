using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries;

public interface IResultQuery<TResult> : IRequest<Result<TResult, Error>>;