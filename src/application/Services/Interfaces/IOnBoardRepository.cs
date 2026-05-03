namespace application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using model;

    public interface IOnBoardRepository
    {
        Task OnBoardAsync(TokenLimitModel model);
        Task<bool> IsOnboarded(string user);
    }
}
