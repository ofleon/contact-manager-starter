using ContactManager.Hubs;
using ContactManager.Models;
using ContactManagerStarter.Controllers;
using ContactManagerStarter.Models;
using ContactManagerStarter.Provider.Domain.Entities;
using ContactManagerStarter.Provider.Domain.Repositories;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MimeKit;

namespace ContactManager.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IContactManagerRepository _context;
        private readonly IHubContext<ContactHub> _hubContext;
        private readonly ILogger _logger;

        public ContactsController(IContactManagerRepository context, ILogger<ContactsController> logger, IHubContext<ContactHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> DeleteContact(Guid id)
        {
            var contactToDelete = await _context.GetContactByIdAsync(id);

            if (contactToDelete is null) return BadRequest();

            try
            {
                await _context.DeleteAsync(contactToDelete);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Contact {id} was deleted sucessfully");
                //TO-DO: Implement error handler controller to catch the exceptions (404, 503, 500, etc..)
            }
            
            
            await _hubContext.Clients.All.SendAsync("Update");

            return Ok();
        }

        public async Task<IActionResult> EditContact(Guid id)
        {
            var contact = await _context.GetContactByIdAsync(id);

            if (contact is null) return NotFound();

            var viewModel = new EditContactViewModel
            {
                Id = contact.Id,
                Title = contact.Title,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                DOB = contact.DOB,
                EmailAddresses = contact.EmailAddresses,
                Addresses = contact.Addresses
            };

            return PartialView("_EditContact", viewModel);
        }

        public async Task<IActionResult> GetContacts()
        {
            var contactList = await _context.GetAsync();

            var contactViewList = contactList.Select(x => new ContactDto
            {
                Id = x.Id, 
                Title = x.Title,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DOB = x.DOB,
                Email = x.EmailAddresses.Count != 0 ?
                    x.EmailAddresses.Any(y => y.IsPrimary == true) ? x.EmailAddresses.Where(x => x.IsPrimary).Select(x => x.Email).FirstOrDefault() 
                    : x.EmailAddresses.Select(x => x.Email).FirstOrDefault()
                    : null
            });

            return PartialView("_ContactTable", new ContactViewModel { Contacts = contactViewList });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewContact()
        {
            return PartialView("_EditContact", new EditContactViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SaveContact([FromBody] SaveContactViewModel model)
        {
            var contact = model.ContactId == Guid.Empty
                ? new Contact { Title = model.Title, FirstName = model.FirstName, LastName = model.LastName, DOB = model.DOB }
                : await _context.GetContactByIdAsync(model.ContactId);

            if (contact is null) return NotFound();

            //I think this not the best why to handle this
            // TO-DO: We can check directly every record on the Db to check if they need to be removed or updated
            // we can add a new Dictionary and play with it
            try
            {
                if (contact.EmailAddresses.Any())
                    await _context.DeleteEmailAddressesAsync(contact.EmailAddresses);

                if (contact.Addresses.Any())
                    await _context.DeleteAddressesAsync(contact.Addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError("Something happened when on the delete transaction");
                //TO-DO: Implement error handler controller to catch the exceptions (404, 503, 500, etc..)
            }


            foreach (var email in model.Emails)
            {
                contact.EmailAddresses.Add(new EmailAddress
                {
                    Type = email.Type,
                    Email = email.Email,
                    IsPrimary = email.IsPrimary,
                    Contact = contact
                });
            }

            foreach (var address in model.Addresses)
            {
                contact.Addresses.Add(new Address
                {
                    Street1 = address.Street1,
                    Street2 = address.Street2,
                    City = address.City,
                    State = address.State,
                    Zip = address.Zip,
                    Type = address.Type
                });
            }

            contact.Title = model.Title;
            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;
            contact.DOB = model.DOB;

            try
            {
                if (model.ContactId == Guid.Empty)
                {
                    await _context.AddAsync(contact);
                }
                else
                {
                    await _context.UpdateAsync(contact);
                }

                _logger.LogInformation($"Contact updated sucessfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something Ocurr when we trying to update/add the contact");
            }
            
            await _hubContext.Clients.All.SendAsync("Update");

            SendEmailNotification(contact.Id);

            return Ok();
        }

        private static void SendEmailNotification(Guid contactId)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("noreply", "noreply@contactmanager.com"));
            message.To.Add(new MailboxAddress("SysAdmin", "Admin@contactmanager.com"));
            message.Subject = "ContactManager System Alert";

            message.Body = new TextPart("plain")
            {
                Text = "Contact with id:" + contactId.ToString() + " was updated"
            };

            using var client = new SmtpClient();
            // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            client.Connect("127.0.0.1", 25, false);

            client.Send(message);
            client.Disconnect(true);
        }

    }

}