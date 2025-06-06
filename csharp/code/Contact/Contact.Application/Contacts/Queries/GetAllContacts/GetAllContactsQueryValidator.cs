﻿using Contact.Application.Contacts.Dtos;
using FluentValidation;

namespace Contact.Application.Contacts.Queries.GetAllContacts;

public class GetAllContactsQueryValidator : AbstractValidator<GetAllContactsQuery>
{
    private int[] allowPageSizes = [5, 10, 15, 30];
    private string[] allowedSortByColumnNames = [nameof(ContactDto.Id), nameof(ContactDto.UserName)];

    public GetAllContactsQueryValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .Must(value => allowPageSizes.Contains(value))
            .WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");

        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }
}
