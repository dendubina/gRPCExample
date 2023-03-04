using Bogus;
using Grpc.Core;
using Person = gRPCExample.Server.Entities.Person;
using gRPCExample.Server.Protos;

namespace gRPCExample.Server.Services;

public class FakePersonService : Protos.FakePersonService.FakePersonServiceBase
{
    private static readonly List<Person> Persons;
    private readonly ILogger<FakePersonService> _logger;

    static FakePersonService()
    {
        Persons = new Faker<Person>()
            .UseSeed(100)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Name, f => f.Person.FirstName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .Generate(count: 100);
    }

    public FakePersonService(ILogger<FakePersonService> logger)
    {
        _logger = logger;
    }

    public override async Task<PersonsListReply> GetPersons(PersonsFilterRequest request, ServerCallContext context)
    {
        var result = Persons;

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            result = result.Where(x => x.Name == request.Name).ToList();
        }

        var reply = new PersonsListReply();

        reply.Persons.AddRange(result.Select(x => new PersonReply
        {
            Id = x.Id.ToString(),
            Name = x.Name,
            Email = x.Email
        }));

        _logger.LogDebug("Retrieved {count} persons", result.Count);

        return await Task.FromResult(reply);
    }

    public override async Task<PersonReply> GetPersonById(GetPersonRequest request, ServerCallContext context)
    {
        var person = Persons.FirstOrDefault(x => x.Id == Guid.Parse(request.Id));

        if (person is null)
        {
            _logger.LogDebug("Person with id {personId} not found", request.Id);

            throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));
        }

        _logger.LogDebug("Person with id {personId} retrieved", request.Id);

        return await Task.FromResult(new PersonReply
        {
            Id = request.Id,
            Name = person.Name,
            Email = person.Email,
        });
    }

    public override async Task<PersonReply> CreatePerson(CreatePersonRequest request, ServerCallContext context)
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
        };

        Persons.Add(person);

        _logger.LogDebug("Person with id {personId} has been added", person.Id);

        return await Task.FromResult(new PersonReply
        {
            Id = person.Id.ToString(),
            Name = person.Name,
            Email = person.Email,
        });
    }
}