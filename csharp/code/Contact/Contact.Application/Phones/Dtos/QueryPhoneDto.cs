namespace Contact.Application.Phones.Dtos;

public class QueryPhoneDto
{
    public int Id { get; set; }
    public int ContactId { get; set; }
    public int UserId { get; set; }
    public string Mobile { get; set; }
}
