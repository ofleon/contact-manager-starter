using ContactManagerStarter.Provider.Domain.Entities;
using ContactManagerStarter.Provider.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerStarter.Provider.Infrastructure.Services;

public class ContactManagerRepository : IContactManagerRepository
{
    private readonly ContactManagerStarterDbContext contactDbContext;

    public ContactManagerRepository(ContactManagerStarterDbContext contactDbContext)
    {
        this.contactDbContext = contactDbContext;
    }

    // Get all contacts
    public async Task<IEnumerable<Contact>> GetAsync() =>
        await contactDbContext.Contacts
        .Include(x => x.EmailAddresses)
            .OrderBy(x => x.FirstName)
            .ToListAsync();

    // Get contact by Id
    public async Task<Contact?> GetContactByIdAsync(Guid id) =>
        await contactDbContext.Contacts
            .Include(x => x.EmailAddresses)
            .Include(x => x.Addresses)
            .FirstOrDefaultAsync(x => x.Id == id);
        

    // Insert New Contact
    public async Task AddAsync(Contact contact)
    {
        contactDbContext.Contacts.Add(contact);
        await contactDbContext.SaveChangesAsync();
    }

    // Update Contact
    public async Task UpdateAsync(Contact contact)
    {
        contactDbContext.Contacts.Update(contact);
        await contactDbContext.SaveChangesAsync();
    }

    // Delete Contact
    public async Task DeleteAsync(Contact contact)
    {
        contactDbContext.EmailAddresses.RemoveRange(contact.EmailAddresses);
        contactDbContext.Contacts.Remove(contact);

        await contactDbContext.SaveChangesAsync();
    }

    // Delete Email Addresses
    public async Task DeleteEmailAddressesAsync(IEnumerable<EmailAddress> emailAddresses)
    {
        contactDbContext.EmailAddresses.RemoveRange(emailAddresses);
        await contactDbContext.SaveChangesAsync();
    }

    // Delete 
    public async Task DeleteAddressesAsync(IEnumerable<Address> addresses)
    {
        contactDbContext.Addresses.RemoveRange(addresses);
        await contactDbContext.SaveChangesAsync();
    }
}
