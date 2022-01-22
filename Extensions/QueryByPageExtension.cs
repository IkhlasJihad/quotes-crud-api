using MongoDB.Driver;
using MongoDB.Bson;
using QuotesAPI.Exceptions;

namespace QuotesAPI.Extensions;

public static class QueryByPageExtension
{
    private static readonly int PAGE_SIZE = 30;
    public static async Task<(int totalPages, IReadOnlyList<TDocument> data)> AggregateByPage<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filterDefinition,
        int page=1)
    {
        try
        {
            if(page < 1)
            page = 1;

            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TDocument>()
                }));
            
            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<TDocument, TDocument>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Project<TDocument, TDocument>(
                        new BsonDocumentProjectionDefinition<TDocument>(
                                BsonDocument.Parse("{_id: 0, tags: 0}"))
                    ),
                    PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * PAGE_SIZE),
                    PipelineStageDefinitionBuilder.Limit<TDocument>(PAGE_SIZE),
                    
                }));

            var aggregation = await collection.Aggregate()
                .Match(filterDefinition)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            if(aggregation.Count == 0)
                throw new EmptyResultException();

            var count = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()
                ?.FirstOrDefault()
                ?.Count;
            
            var totalPages = (int)Math.Ceiling((count is null ? 0 : (double)count) / PAGE_SIZE);    
           
            if(totalPages < page)
                throw new PageOutOfRangeException(totalPages);    
           
            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<TDocument>();

            return (totalPages, data);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}