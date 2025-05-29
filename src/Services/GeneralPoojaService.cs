using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pooja.src.Exceptions.General;
using Pooja.src.Models.General;
using Pooja.src.Utils;

namespace Pooja.src.Services;

public sealed class GeneralPoojaService
{
    private readonly IMongoCollection<PoojaAdmin> adminCollection;

    internal GeneralPoojaService(IMongoDatabase database)
    {
        adminCollection = database.GetCollection<PoojaAdmin>("Admins");
    }

    public async Task<PoojaAdmin> GetPoojaAdminAsync(ulong ID)
    {
        var cursor = await adminCollection.FindAsync(x => x.ID == ID);
        var found = await cursor.FirstOrDefaultAsync();

        if (found == null)
        {
            throw new PoojaAdminNotFoundException($"Admin {ID} does not exist");
        }

        return found;
    }

    public async Task<PoojaAdmin> CreatePoojaAdminAsync(ulong ID, string name, PoojaHierarchy hierarchy)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var cursor = await adminCollection.FindAsync(x => x.ID == ID);
        var found = await cursor.FirstOrDefaultAsync();

        if (found != null)
        {
            throw new PoojaAdminAlreadyExistsException($"`admin {name} - {ID} already exists with {hierarchy}`".ToLower());
        }

        var poojaAdmin = new PoojaAdmin
        {
            ID = ID,
            Name = name,
            Position = hierarchy
        };

        await adminCollection.InsertOneAsync(poojaAdmin);

        return poojaAdmin;
    }

    public async Task<List<PoojaAdmin>> GetPoojaAdminsAsync(PoojaHierarchy? hierarchy = null)
    {
        IAsyncCursor<PoojaAdmin> cursor;

        if (hierarchy == null)
        {
            cursor = await adminCollection.FindAsync(_ => true);
        }
        else
        {
            cursor = await adminCollection.FindAsync(x => x.Position == hierarchy);
        }

        var admins = await cursor.ToListAsync();

        if (admins.Count == 0 && hierarchy is not null)
        {
            throw new PoojaAdminNotFoundException($"No admins have found regarding position {hierarchy}");
        }
        else if (admins.Count == 0 && hierarchy is null)
        {
            throw new PoojaAdminNotFoundException($"No admins have been found");
        }

        return admins;
    }
}
