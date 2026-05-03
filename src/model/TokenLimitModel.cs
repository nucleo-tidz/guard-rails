namespace model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public record TokenLimitModel(string user, TimeSpan window, long limit);

}
