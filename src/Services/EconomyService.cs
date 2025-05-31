using MongoDB.Driver;
using Pooja.src.Exceptions.Economy;
using Pooja.src.Models.Economy;
using System.Linq.Expressions;

namespace Pooja.src.Services;

internal enum MaxEconomyAccountSizes : long
{
    MaxCashSize = long.MaxValue
}

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
            throw new PlayerAlreadyExistsException($"player {userID} already exists");
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
            throw new PlayerNotFoundException($"player {userID} cannot be found");
        }

        return user;
    }

    public Task UpdatePoojaUserAsync(PoojaEconomyUser user) => userCollection.ReplaceOneAsync(x => x.ID == user.ID, user);

    public async Task UpdatePoojaUserAsync(ulong playerID, long? cash = null, long? bank = null)
    {
        var updateDefinitions = new List<UpdateDefinition<PoojaEconomyUser>>();

        if (cash is not null)
        {
            var builder = Builders<PoojaEconomyUser>.Update.Set(x => x.Cash, cash.Value);
            updateDefinitions.Add(builder);
        }

        if (bank is not null)
        {
            var builder = Builders<PoojaEconomyUser>.Update.Set(x => x.Bank, bank.Value);
            updateDefinitions.Add(builder);
        }

        if (updateDefinitions.Count == 0)
            return;

        var result = await userCollection.UpdateOneAsync(x => x.ID == playerID, Builders<PoojaEconomyUser>.Update.Combine(updateDefinitions));

        if (result.MatchedCount == 0)
            throw new PlayerNotFoundException($"player {playerID} has not been found");
    }

    public async Task ChangePoojaUserBalanceAsync(ulong playerID, long cash = 0, long bank = 0)
    {
        var update = Builders<PoojaEconomyUser>.Update.Combine(
            Builders<PoojaEconomyUser>.Update.Inc(x => x.Cash, cash),
            Builders<PoojaEconomyUser>.Update.Inc(x => x.Bank, bank)
        );

        var result = await userCollection.UpdateOneAsync(x => x.ID == playerID, update);

        if(result.MatchedCount == 0)
            throw new PlayerNotFoundException($"player {playerID} has not been found");
    }

    public async Task<List<PoojaEconomyUser>> GetPoojaEconomyUsersAsync()
    {
        var players = await userCollection.FindAsync(_ => true);

        return await players.ToListAsync();
    }
}