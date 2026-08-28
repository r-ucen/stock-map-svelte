using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.User;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class GetUserHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(query.UserId, cancellationToken);
        return user ?? throw new UserNotFoundException("User not found");
    }
}