namespace gRPCExample.Client.Interfaces;

public interface IPersonService
{
    Task<PersonsListReply> GetPersons(PersonsFilterRequest filter);
    Task<PersonReply> GetPersonById(Guid id);
    Task<PersonReply> CreatePerson(CreatePersonRequest person);
}