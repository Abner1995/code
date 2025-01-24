using MediatR;

namespace CleanArchitecture.Application.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommand : IRequest
{
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
}
