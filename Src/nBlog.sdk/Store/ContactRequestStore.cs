using Microsoft.Extensions.Logging;
using nBlog.sdk.Extensions;
using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Azure.DataLake;
using Toolbox.Tools;

namespace nBlog.sdk.Store
{
    public class ContactRequestStore : IContactRequestStore
    {
        private readonly IDataLakeStore _dataLakeStore;
        private readonly ILogger<ContactRequestStore> _logger;

        public ContactRequestStore(IDataLakeStore dataLakeStore, ILogger<ContactRequestStore> logger)
        {
            _dataLakeStore = dataLakeStore;
            _logger = logger;
        }

        public async Task<bool> Delete(Guid id, CancellationToken token = default)
        {
            return await _dataLakeStore.Delete(ToFullFileName(id), token);
        }

        public async Task<ContactRequest?> Get(Guid id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));

            byte[] fileData = await _dataLakeStore.Read(ToFullFileName(id), token);
            if (fileData == null || fileData.Length == 0) return null;

            return fileData.ToContractRequest();
        }

        public async Task<IReadOnlyList<string>> List(QueryParameters queryParameters, CancellationToken token = default) =>
            (await _dataLakeStore.Search("/", x => x.IsDirectory == false, true, token))
                .Select(x => x.Name)
                .Skip(queryParameters.Index)
                .Take(queryParameters.Count)
                .ToList();

        public async Task Set(ContactRequest contractRequest, CancellationToken token = default)
        {
            contractRequest.VerifyNotNull(nameof(contractRequest));

            _logger.LogTrace($"{nameof(Set)}: Writing {contractRequest.RequestId}");
            await _dataLakeStore.Write(ToFullFileName(contractRequest.RequestId), contractRequest.ToBytes(), true, token);
        }

        private static string ToFullFileName(Guid id) => $"contactRequest/contactRequest_message_{id}.json";
    }
}