namespace application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IEmbedService
    {
        Task SeedDataAsync(string documentContent, string sourceFileName, int chunkSize = 200, int overlap = 50);
    }
}
