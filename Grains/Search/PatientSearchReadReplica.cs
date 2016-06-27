using Common;
using GrainInterfaces.Search;
using Grains.Infrastructure;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains.Search
{
    public class PatientSearchReadReplica : Grain, IPatientSearchReadReplica
    {
        private const string INDEX_KEY = "Id";
        private const string INDEX_VALUE = "Text";

        private Dictionary<Guid, Patient> _patients;
        private RAMDirectory _directory;

        public override async Task OnActivateAsync()
        {
            _directory = new RAMDirectory();
            _directory.SetLockFactory(NoLockFactory.Instance);

            var ps = GrainFactory.GetGrain<IPatientSearch>(Guid.Empty);
            await Sync(await ps.GetState());

            await base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            if (_directory != null)
            {
                _directory.Dispose();
            }

            return base.OnDeactivateAsync();
        }

        public Task Sync(IEnumerable<Patient> patients)
        {
            _patients = patients.ToDictionary(x => x.Id);

            if (_directory.ListAll().Length > 0)
            {
                using (var indexReader = IndexReader.Open(_directory, false))
                {
                    for (var i = 0; i < indexReader.MaxDoc; i++)
                    {
                        indexReader.DeleteDocument(i);
                    }
                }
            }

            using (var ngram = new NGramAnalyzer())
            using (var indexWriter = new IndexWriter(_directory, ngram, IndexWriter.MaxFieldLength.LIMITED))
            {
                foreach (var p in _patients)
                {
                    var doc = new Document();
                    doc.Add(new Field(INDEX_KEY, p.Key.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field(INDEX_VALUE, p.Value.Name, Field.Store.NO, Field.Index.ANALYZED));
                    indexWriter.AddDocument(doc);
                }

                indexWriter.Optimize();
            }

            return TaskDone.Done;
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
                            return _patients[patientId];
                        })
                        .OrderBy(x => x.Name)
                        .ToList();
                }
            }

            return Task.FromResult(result);
        }
    }
}