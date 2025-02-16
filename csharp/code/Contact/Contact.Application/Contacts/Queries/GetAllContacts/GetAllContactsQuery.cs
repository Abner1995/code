using Contact.Application.Common;
using Contact.Application.Contacts.Dtos;
using Contact.Domain.Constants;
using MediatR;

namespace Contact.Application.Contacts.Queries.GetAllContacts;

public class GetAllContactsQuery : IRequest<PagedResult<ContactDto>>
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}
