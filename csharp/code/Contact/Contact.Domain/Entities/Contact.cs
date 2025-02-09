namespace Contact.Domain.Entities;

public class Contact
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public virtual List<Phone>? Phones { get; set; }
}
