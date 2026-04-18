namespace application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface ISharedContextService
    {
       Task Build(string user, string threadId, string query);
    }
}
