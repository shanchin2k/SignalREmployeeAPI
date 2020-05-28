using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SignalREmployee.Data
{
    public class DocumentDBRepository<T> : IDocumentDBRepository<T> where T : class
    {

        private readonly string Endpoint = "https://localhost:8081";
        private readonly string Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private readonly string DatabaseId = "ShanCosmosDB";
        private readonly DocumentClient client;

        public DocumentDBRepository()
        {
            client = new DocumentClient(new Uri(Endpoint), Key);
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, string collectionId)
        {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetItemsAsync(string collectionId)
        {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId),
                new FeedOptions { MaxItemCount = -1 })
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<Document> CreateItemAsync(T item, string collectionId)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId), item);
        }

        public async Task<Document> UpdateItemAsync(string id, T item, string collectionId)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, collectionId, id), item);
        }

        public async Task DeleteItemAsync(string id, string collectionId, string partitionKey)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, collectionId, id),
            new RequestOptions() { PartitionKey = new PartitionKey(partitionKey) });
        }
    }
}