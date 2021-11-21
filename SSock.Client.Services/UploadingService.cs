using SSock.Client.Services.Abstract;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using SSock.Core.Services.Abstract.Communication;

namespace SSock.Client.Services
{
    internal class UploadingService
        : IUploadingService
    {
        const int MAX_DATA_LENGTH = 960;
        const int HASH_PART_LENGTH = 128;

        private readonly IDataTransitService _dataTransitService;

        public UploadingService(IDataTransitService dataTransitService)
        {
            _dataTransitService = dataTransitService;
        }

        public IEnumerable<byte> GetUploadDataPayload(
            IEnumerable<byte> data, 
            string hash)
        {
            if (data.Count() > MAX_DATA_LENGTH)
            {
                throw new ArgumentException(
                    $"Data is too large. Max data size is {MAX_DATA_LENGTH} bytes");
            }

            var hashPart = _dataTransitService
                .AppendBytes(
                    Encoding.Unicode.GetBytes(hash), 
                    HASH_PART_LENGTH);

            var dataPart = data.Count() < MAX_DATA_LENGTH
                ? _dataTransitService.AppendBytes(data.ToArray(), MAX_DATA_LENGTH)
                : data;

            return hashPart.Concat(dataPart);
        }
    }
}
