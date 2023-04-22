namespace ContactManagerStarter.Provider.Domain.Entities;

public class EmailAddress : Entity
{
    public string Email { get; set; }
    public EmailType Type { get; set; }
    public bool IsPrimary { get; set; }
    public Guid ContactId { get; set; }
    public virtual Contact Contact { get; set; }
}
