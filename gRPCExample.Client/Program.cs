using Grpc.Net.Client;

namespace gRPCExample.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        using var channel = GrpcChannel.ForAddress("https://localhost:5000");
        var client = new FakePersonService.FakePersonServiceClient(channel);

        GetPersons(new PersonsFilterRequest { Name = "Lester" }, client);

        CreatePerson("Denis", "denis@mail.ru", client);

        channel.Dispose();
        Console.ReadLine();
    }

    private static PersonsListReply GetPersons(PersonsFilterRequest filter,
        FakePersonService.FakePersonServiceClient client)
    {
        var reply = client.GetPersons(filter);

        foreach (var person in reply.Persons)
        {
            Console.WriteLine($"{person.Id} {person.Name} {person.Email}");
        }

        return reply;
    }

    private static PersonReply GetById(Guid id, FakePersonService.FakePersonServiceClient client)
    {
        var request = new GetPersonRequest
        {
            Id = id.ToString(),
        };

        var reply = client.GetPersonById(request);

        Console.WriteLine($"Found person: {reply.Id} {reply.Name} {reply.Email}");

        return reply;
    }

    private static PersonReply CreatePerson(string name, string email, FakePersonService.FakePersonServiceClient client)
    {
        var request = new CreatePersonRequest
        {
            Name = name,
            Email = email,
        };

        var reply = client.CreatePerson(request);

        Console.WriteLine($"Created person: {reply.Id} {reply.Name} {reply.Email}");

        return reply;
    }

}