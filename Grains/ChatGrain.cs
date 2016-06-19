using Common;
using GrainInterfaces;
using Grains.Infrastructure;
using Grains.State;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "Default")]
    public class ChatGrain : BaseGrain<ChatState>, IChatGrain
    {
        private const string INDEX_KEY = "Id";
        private const string INDEX_VALUE = "Text";

        private RAMDirectory _directory;

        public override Task OnActivateAsync()
        {
            _directory = new RAMDirectory();
            using (var ngram = new NGramAnalyzer())
            using (var indexWriter = new IndexWriter(_directory, ngram, IndexWriter.MaxFieldLength.LIMITED))
            {
                foreach (var m in State.Messages)
                {
                    var doc = new Document();
                    doc.Add(new Field(INDEX_KEY, m.Key.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field(INDEX_VALUE, m.Value.Text, Field.Store.NO, Field.Index.ANALYZED));
                    indexWriter.AddDocument(doc);
                }

                indexWriter.Optimize();
            }

            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            if (_directory != null)
            {
                _directory.Dispose();
            }

            return base.OnDeactivateAsync();
        }

        public async Task<IEnumerable<ChatEntity>> Entities()
        {
            return await Task.WhenAll(
                State.Messages.Values
                    .Select(x => new { Id = x.EntityId, Type = x.EntityType })
                    .Union(State.Present.Select(x => new { Id = x.Key, Type = x.Value }))
                    .Distinct()
                    .Select(async x =>
                    {
                        var entity = GetEntity(x.Id, x.Type);
                        return new ChatEntity
                        {
                            IsPresent = State.Present.ContainsKey(x.Id),
                            Entity = new Entity
                            {
                                Id = entity.GetPrimaryKey(),
                                Name = await entity.GetName()
                            }
                        };
                    })
            );
        }

        public Task<IEnumerable<ChatMessage>> Messages()
        {
            var messages = State.Messages.Values
                .OrderByDescending(x => x.Received)
                .Reverse()
                .ToList()
                .AsEnumerable();

            return Task.FromResult(messages);
        }

        public async Task<string> Join(Guid entityId, EntityType entityType)
        {
            var entity = GetEntity(entityId, entityType);
            if (!State.Present.ContainsKey(entityId))
            {
                State.Present.Add(entityId, entityType);
                await base.WriteStateAsync();
            }

            return await entity.GetName();
        }

        public async Task Leave(Guid entityId)
        {
            State.Present.Remove(entityId);
            await base.WriteStateAsync();
        }

        public async Task<ChatMessage> AddMessage(Guid entityId, EntityType entityType, string text)
        {
            var entity = GetEntity(entityId, entityType);
            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                EntityId = entityId,
                EntityType = entityType,
                Name = await entity.GetName(),
                Received = DateTime.UtcNow,
                Text = text
            };

            State.Messages.Add(message.Id, message);
            await base.WriteStateAsync();

            using (var indexWriter = new IndexWriter(_directory, new NGramAnalyzer(), IndexWriter.MaxFieldLength.LIMITED))
            {
                var doc = new Document();
                doc.Add(new Field("Id", message.Id.ToString(), Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("Text", message.Text, Field.Store.NO, Field.Index.ANALYZED));
                indexWriter.AddDocument(doc);
            }

            return message;
        }

        public Task ConfirmMessage(Guid entityId, Guid messageId)
        {
            var message = State.Messages[messageId];
            if (!message.Viewed.Contains(entityId))
            {
                message.Viewed.Add(entityId);
            }

            return base.WriteStateAsync();
        }

        public Task<IEnumerable<ChatMessage>> Search(string searchValue)
        {
            var result = Enumerable.Empty<ChatMessage>();

            using (var indexReader = IndexReader.Open(_directory, true))
            using (var indexSearcher = new IndexSearcher(indexReader))
            using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            {
                var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, INDEX_VALUE, analyzer);
                var query = queryParser.Parse(searchValue);
                var hits = indexSearcher.Search(query, 10);
                if (hits.TotalHits > 0)
                {
                    result = hits.ScoreDocs
                        .Select(x =>
                        {
                            var doc = indexSearcher.Doc(x.Doc);
                            var messageId = Guid.Parse(doc.GetField(INDEX_KEY).StringValue);
                            return State.Messages[messageId];
                        })
                        .OrderBy(x => x.Received)
                        .ToList();
                }
            }

            return Task.FromResult(result);
        }
    }
}