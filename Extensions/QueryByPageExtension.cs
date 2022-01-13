using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using System.Threading.Tasks;
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
        if(page < 1)
            page = 1;

        var countFacet = AggregateFacet.Create("count",
            PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<TDocument>()
            }));
        
        var aggregationAll = await collection.Aggregate()
            .Match(filterDefinition)
            .Facet(countFacet)
            .ToListAsync();

        var countAll = aggregationAll.First()
            .Facets.First(x => x.Name == "count")
            .Output<AggregateCountResult>()
            ?.FirstOrDefault()
            ?.Count;
        
        var totalPages = (int)Math.Ceiling((countAll is null ? 0 : (double)countAll) / PAGE_SIZE);
        // invalid page is queried
        if(page > totalPages){
            throw new PageOutOfRangeException(totalPages);
        }
        // valid page is queried
        var dataFacet = AggregateFacet.Create("data",
            PipelineDefinition<TDocument, TDocument>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * PAGE_SIZE),
                PipelineStageDefinitionBuilder.Limit<TDocument>(PAGE_SIZE),
            }));

        var aggregation = await collection.Aggregate()
            .Match(filterDefinition)
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        var count = aggregation.First()
            .Facets.First(x => x.Name == "count")
            .Output<AggregateCountResult>()
            ?.FirstOrDefault()
            ?.Count;
        
        totalPages = (int)Math.Ceiling((count is null ? 0 : (double)count) / PAGE_SIZE);        
        var data = aggregation.First()
            .Facets.First(x => x.Name == "data")
            .Output<TDocument>();

        return (totalPages, data);
    }
}