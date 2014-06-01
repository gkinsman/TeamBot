using System;
using System.Configuration;
using Raven.Client;
using Raven.Client.Document;

namespace TeamBot.Infrastructure.Raven
{
    public static class DocumentStoreBuilder
    {
        public static readonly Lazy<IDocumentStore> DocumentStore = new Lazy<IDocumentStore>(InitializeDocumentStore);

        static IDocumentStore InitializeDocumentStore()
        {
            var documentStore = new DocumentStore();
            documentStore.ParseConnectionString(ConfigurationManager.AppSettings["RavenHqUri"]);
            documentStore.Conventions.IdentityPartsSeparator = "-";
            documentStore.Initialize();
            //IndexCreation.CreateIndexes(typeof(Customer).Assembly, documentStore);
            return documentStore;
        }
    }
}