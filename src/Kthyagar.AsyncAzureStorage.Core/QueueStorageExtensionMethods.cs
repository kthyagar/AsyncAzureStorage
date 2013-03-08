using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Kthyagar.AsyncAzureStorage.Core
{
    public static class QueueStorageExtensionMethods
    {
        /// <summary>
        /// AddMessageAsync
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task AddMessageAsync(this CloudQueue queue, CloudQueueMessage msg)
        {
            var retVal = queue.BeginAddMessage(msg, null, null);
            IAsyncResult arRetVal = retVal as IAsyncResult;

            await Task.Factory.FromAsync(arRetVal, (r) => { queue.EndAddMessage(r); });
        }

        /// <summary>
        /// GetMessageAsync
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static async Task<CloudQueueMessage> GetMessageAsync(this CloudQueue queue)
        {
            var cAsyncR = queue.BeginGetMessage(TimeSpan.FromSeconds(4.0), null, null, null, null);
            IAsyncResult asyncR = cAsyncR as IAsyncResult;

            CloudQueueMessage retVal = null;

            await Task.Factory.FromAsync(asyncR, (r) => { retVal = queue.EndGetMessage(r); });

            return retVal;
        }

        /// <summary>
        /// PeekMessageAsync
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static async Task<CloudQueueMessage> PeekMessageAsync(this CloudQueue queue)
        {
            QueueRequestOptions options = new QueueRequestOptions
            {
                MaximumExecutionTime = TimeSpan.FromSeconds(1.0),
                ServerTimeout = TimeSpan.FromSeconds(1.0)
            };

            var cAsyncR = queue.BeginPeekMessage(options, null, null, null);
            IAsyncResult asyncR = cAsyncR as IAsyncResult;

            CloudQueueMessage retVal = null;

            await Task.Factory.FromAsync(asyncR, (r) => { retVal = queue.EndPeekMessage(r); });

            return retVal;
        }

        /// <summary>
        /// DeleteMessageAsync
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task DeleteMessageAsync(this CloudQueue queue, CloudQueueMessage msg)
        {
            var retVal = queue.BeginDeleteMessage(msg, null, null, null, null);
            IAsyncResult arRetVal = retVal as IAsyncResult;

            await Task.Factory.FromAsync(arRetVal, (r) => { queue.EndAddMessage(r); });
        }
    } // class
} // namespace