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
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "Default")]
    [ImplicitStreamSubscription(STREAM_NAMESPACE)]
    public class PatientSearchGrain : Grain<PatientSearchState>, IPatientSearchGrain
    {
        private const string INDEX_KEY = "Id";
        private const string INDEX_VALUE = "Text";
        private RAMDirectory _directory;
        private StreamSubscriptionHandle<Patient> _subscription;

        public const string STREAM_NAMESPACE = "PatientSearch";

        public override async Task OnActivateAsync()
        {
            _directory = new RAMDirectory();
            using (var ngram = new NGramAnalyzer())
            using (var indexWriter = new IndexWriter(_directory, ngram, IndexWriter.MaxFieldLength.LIMITED))
            {
                foreach (var m in State.Patients)
                {
                    var doc = new Document();
                    doc.Add(new Field(INDEX_KEY, m.Key.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field(INDEX_VALUE, m.Value.Name, Field.Store.NO, Field.Index.ANALYZED));
                    indexWriter.AddDocument(doc);
                }

                indexWriter.Optimize();
            }

            var streamProvider = base.GetStreamProvider("Default");
            var stream = streamProvider.GetStream<Patient>(this.GetPrimaryKey(), STREAM_NAMESPACE);
            _subscription = await stream.SubscribeAsync((patient, token) =>
            {
                State.Patients[patient.Id] = patient;
                using (var indexWriter = new IndexWriter(_directory, new NGramAnalyzer(), IndexWriter.MaxFieldLength.LIMITED))
                {
                    var doc = new Document();
                    doc.Add(new Field("Id", patient.Id.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("Text", patient.Name, Field.Store.NO, Field.Index.ANALYZED));
                    indexWriter.AddDocument(doc);
                }

                return base.WriteStateAsync();
            });

            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            if (_directory != null)
            {
                _directory.Dispose();
            }

            await _subscription.UnsubscribeAsync();
            await base.OnDeactivateAsync();
        }

        public Task<IEnumerable<Patient>> Search(string searchValue)
        {
            var result = Enumerable.Empty<Patient>();

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
                            var patientId = Guid.Parse(doc.GetField(INDEX_KEY).StringValue);
                            return State.Patients[patientId];
                        })
                        .OrderBy(x => x.Name)
                        .ToList();
                }
            }

            return Task.FromResult(result);
        }
    }
}