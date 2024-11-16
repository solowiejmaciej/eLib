using System.Collections;
using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries.User;

public record GetAllUsersQuery() : IResultQuery<IEnumerable<UserDto>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserDto>, Error>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<IEnumerable<UserDto>, Error>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllWithDetailsAsync(cancellationToken);
        var userDtos = users.Select(x => x.MapToDto());
        return Result<IEnumerable<UserDto>, Error>.FromEnumerable(userDtos);
    }
}