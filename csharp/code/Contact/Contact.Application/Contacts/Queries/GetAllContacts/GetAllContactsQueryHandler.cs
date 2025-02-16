using AutoMapper;
using Contact.Application.Common;
using Contact.Application.Contacts.Dtos;
using Contact.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Contact.Application.Contacts.Queries.GetAllContacts;

public class GetAllContactsQueryHandler(ILogger<GetAllContactsQueryHandler> logger,
    IMapper mapper,
    IContactsRepository contactsRepository) : IRequestHandler<GetAllContactsQuery, PagedResult<ContactDto>>
{
    public async Task<PagedResult<ContactDto>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all contacts");
        var (contacts, totalCount) = await contactsRepository.GetAllMatchingAsync(request.SearchPhrase,
            request.PageSize,
        request.PageNumber,
        request.SortBy,
            request.SortDirection);

        var contactDtos = mapper.Map<IEnumerable<ContactDto>>(contacts);

        var result = new PagedResult<ContactDto>(contactDtos, totalCount, request.PageSize, request.PageNumber);
        return result;
    }
}
