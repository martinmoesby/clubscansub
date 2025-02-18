namespace CLubScansub.MAUI.App2.Models
{
    public static class ContactRepository
    {
        private static List<Contact> _contacts = new List<Contact>()
        {
            new Contact() {ContactId = 1, Name="Martín Moesby", Email="martinmoesby@martinmoesby.com"},
            new Contact() {ContactId = 2, Name="Camilla Holst", Email="camillaholst@hyldenet.dk"},
            new Contact() {ContactId = 3, Name="Klaus Kjærulff-Olsen", Email="kk@kko.dk"},
            new Contact() {ContactId = 4, Name="Miranda Olsen", Email="miranda@olsen.dk"},
        };


        public static List<Contact> GetAll() => _contacts;

        public static Contact? GetContactById(int contactId)
        {
            var contact = _contacts.FirstOrDefault(x => x.ContactId == contactId);
            if (contact != null)
            {
                return new Contact() { 
                    Address = contact.Address,
                    Name = contact.Name,
                    ContactId = contact.ContactId,
                    Phone = contact.Phone,
                    Email = contact.Email
                };
            }

            return null;
        }

        public static void UpdateContact(int contactId, Contact contact)
        {
            if (contactId != contact.ContactId)
                return;
            var contactToUpdate = _contacts.FirstOrDefault(x => x.ContactId == contactId);

            if (contactToUpdate != null )
            {
                contactToUpdate.Name = contact.Name;
                contactToUpdate.Phone = contact.Phone;
                contactToUpdate.Email = contact.Email;  
                contactToUpdate.Address = contact.Address;
            }

            return;
        }

        public static void AddContact(Contact contact)
        {
            contact.ContactId = _contacts.Max(x => x.ContactId) + 1;
            _contacts.Add(contact);
        }

        public static void DeleteContact(int ContactId)
        {
            var contact = _contacts.FirstOrDefault(x => x.ContactId == ContactId);
            if (contact != null)
            {
                _contacts.Remove(contact);
            }
        }

    }
}
