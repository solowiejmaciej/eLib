using System.Collections;
using eLib.Common.Dtos;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries.User;

public record GetAllUsersQuery(PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<UserDto>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PaginationResult<UserDto>, Error>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<PaginationResult<UserDto>, Error>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllPaginatedWithDetailsAsync(request.PaginationParameters, cancellationToken);
        return users.MapToDto(x => x.MapToDto());
    }
}