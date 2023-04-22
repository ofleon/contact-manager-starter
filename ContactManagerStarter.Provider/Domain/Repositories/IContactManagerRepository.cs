using ContactManagerStarter.Provider.Domain.Entities;

namespace ContactManagerStarter.Provider.Domain.Repositories;

public interface IContactManagerRepository
{
    Task<IEnumerable<Contact>> GetAsync();
    Task<Contact?> GetContactByIdAsync(Guid id);
    Task AddAsync(Contact contact);
    Task UpdateAsync(Contact contact);
    Task DeleteAsync(Contact contact);
    Task DeleteEmailAddressesAsync(IEnumerable<EmailAddress> emailAddresses);
    Task DeleteAddressesAsync(IEnumerable<Address> addresses);
}
