using Grpc.Net.Client;
using gRPCExample.Client.Interfaces;

namespace gRPCExample.Client.Services;

public class PersonService : IPersonService, IDisposable
{
    private readonly FakePersonService.FakePersonServiceClient _personClient;
    private readonly GrpcChannel _channel;

    public PersonService()
    {
        _channel = GrpcChannel.ForAddress("https://localhost:5000");
        _personClient = new FakePersonService.FakePersonServiceClient(_channel);
    }

    public async Task<PersonsListReply> GetPersons(PersonsFilterRequest filter)
    {
        var reply = await _personClient.GetPersonsAsync(filter);

        return reply;
    }

    public async Task<PersonReply> GetPersonById(Guid id)
    {
        var request = new GetPersonRequest
        {
            Id = id.ToString(),
        };

        var reply = await _personClient.GetPersonByIdAsync(request);

        return reply;
    }

    public async Task<PersonReply> CreatePerson(CreatePersonRequest person)
    {
        var reply = await _personClient.CreatePersonAsync(person);

        return reply;
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}