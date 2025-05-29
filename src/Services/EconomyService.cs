using MongoDB.Driver;
using Pooja.src.Exceptions.Economy;
using Pooja.src.Models.Economy;

namespace Pooja.src.Services;

public sealed class EconomyService
{
    private readonly IMongoCollection<PoojaEconomyUser> userCollection;

    internal EconomyService(IMongoDatabase database)
    {
        userCollection = database.GetCollection<PoojaEconomyUser>("Players");
    }

    public async Task<PoojaEconomyUser> CreatePoojaEconomyUser(ulong userID)
    {
        var user = await userCollection
            .Find(x => x.ID == userID)
            .FirstOrDefaultAsync();

        if (user is not null)
        {
            throw new PlayerAlreadyExistsException($"Player {userID} already exists");
        }

        user = new PoojaEconomyUser
        {
            ID = userID,
            Cash = 5_000,
            Bank = 0
        };

        await userCollection.InsertOneAsync(user);

        return user;
    }

    public async Task<PoojaEconomyUser> GetPoojaEconomyUserAsync(ulong userID)
    {
        var user = await userCollection
            .Find(x => x.ID == userID)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            throw new PlayerNotFoundException($"Player {userID} cannot be found");
        }

        return user;
    }

    public Task UpdatePoojaUserAsync(PoojaEconomyUser user) => userCollection.ReplaceOneAsync(x => x.ID == user.ID, user);
}