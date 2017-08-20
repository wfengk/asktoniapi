using AskToniApi.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;

public class RecommendationRepository : IRecommendationRepository
{
    private readonly AskToniContext _context = null;

    public RecommendationRepository(IOptions<DbConnectionConfig> dbConnectionConfig)
    {
        DbConnectionConfig _dbConnectionConfig = dbConnectionConfig.Value;
        _context = new AskToniContext(_dbConnectionConfig.mLabConnectStr);
    }

    public async Task<IEnumerable<Restaurant>> GetAllRestaurants()
    {
        try {
            return await _context.Recommendations.Find(_ => true).ToListAsync();
        }
        catch (Exception ex) {
            throw ex;
        }
    }

    public async Task<Restaurant> GetRestaurant(string id)
    {
        var filter = Builders<Restaurant>.Filter.Eq(r => r.Id, ObjectId.Parse(id));
        return await _context.Recommendations
                             .Find(filter)
                             .FirstOrDefaultAsync();
    }

    public async Task AddRestaurant(Restaurant item)
    {
        await _context.Recommendations.InsertOneAsync(item);
    }

    public async Task<DeleteResult> RemoveRestaurant(string id)
    {
        return await _context.Recommendations.DeleteOneAsync(
                     Builders<Restaurant>.Filter.Eq(r => r.Id, ObjectId.Parse(id)));
    }
   
    public async Task<ReplaceOneResult> UpdateRestaurant(string id, Restaurant item)
    {
        item.Id = ObjectId.Parse(id);

        return await _context.Recommendations
                             .ReplaceOneAsync(r => r.Id.Equals(ObjectId.Parse(id)),
                                             item,
                                             new UpdateOptions { IsUpsert = true });
    }
}