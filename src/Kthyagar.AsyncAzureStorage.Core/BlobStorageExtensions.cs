using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Kthyagar.AsyncAzureStorage
{
    public static class BlobStorageExtensions
    {
        /// <summary>
        /// CreateIfNotExistsAsync
        /// </summary>
        /// <param name="blobContainer"></param>
        /// <returns></returns>
        public static Task CreateIfNotExistsAsync(this CloudBlobContainer blobContainer)
        {
            var ar = blobContainer.BeginCreateIfNotExists(null, null);

            return Task.Factory.FromAsync(ar, (r) => { blobContainer.EndCreateIfNotExists(r); });
        }

        /// <summary>
        /// GetBlobReferenceFromServerAsync
        /// </summary>
        /// <param name="blobContainer"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public static Task<ICloudBlob> GetBlobReferenceFromServerAsync(this CloudBlobContainer blobContainer, string blobName)
        {
            var retVal = blobContainer.BeginGetBlobReferenceFromServer(blobName, null, null);
            IAsyncResult arRetVal = retVal as IAsyncResult;


            return Task.Factory.FromAsync(arRetVal,
                (Func<IAsyncResult, ICloudBlob>)blobContainer.EndGetBlobReferenceFromServer);
        }

        /// <summary>
        /// DownloadToStreamAsync
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task DownloadToStreamAsync(this ICloudBlob blob, Stream stream)
        {
            var retVal = blob.BeginDownloadToStream(stream, null, null);
            IAsyncResult arRetVal = retVal as IAsyncResult;

            await Task.Factory.FromAsync(arRetVal, (r) => { blob.EndDownloadToStream(r); });

            stream.Seek(0, 0);
        }

        /// <summary>
        /// DownloadTextAsync
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        public static async Task<string> DownloadTextAsync(this ICloudBlob blob)
        {
            using (var memStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memStream);
                memStream.Position = 0;
                using (var strRdr = new StreamReader(memStream))
                {
                    var str = strRdr.ReadToEnd();
                    return str;
                } // using
            } // using
        }

        /// <summary>
        /// UploadFromStreamAsync
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="sourceStream"></param>
        /// <param name="accessCondition"></param>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public static async Task UploadFromStreamAsync(this ICloudBlob blob, Stream sourceStream, AccessCondition accessCondition, BlobRequestOptions options, OperationContext operationContext)
        {
            Func<AsyncCallback, Object, IAsyncResult> beginMethod = (cb, state) =>
            {
                return blob.BeginUploadFromStream(sourceStream, accessCondition, options, operationContext, cb, state);
            };

            Action<IAsyncResult> endMethod = (r) =>
            {
                blob.EndUploadFromStream(r);
            };

            await Task.Factory.FromAsync(beginMethod, endMethod, null);
        }

        /// <summary>
        /// UploadTextAsync
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async Task UploadTextAsync(this ICloudBlob blob, string text)
        {
            using (var memStream = new MemoryStream())
            {
                using (var strWriter = new StreamWriter(memStream))
                {
                    strWriter.Write(text);
                    strWriter.Flush();
                    memStream.Position = 0;

                    await blob.UploadFromStreamAsync(memStream, null, null, null);
                } // using
            } // using
        }
    } // class
} // namespace