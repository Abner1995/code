using Contact.Application.Phones.Dtos;

namespace Contact.Application.Contacts.Dtos;

public class ContactDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public List<QueryPhoneDto>? Phones { get; set; }
}
