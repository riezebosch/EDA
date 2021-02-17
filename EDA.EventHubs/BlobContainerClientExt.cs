using Azure.Storage.Blobs;
using System.Threading.Tasks;

namespace EDA.EventHubs
{
    public static class BlobContainerClientExt
    {
        public static async Task<BlobContainerClient> SetupStore(this BlobServiceClient blob, string container)
        {
            var store = blob.GetBlobContainerClient(container);
            await store.CreateIfNotExistsAsync();
            
            return store;
        }
    }
}