using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreExamples.ViewModels.Token
{
    public class TokenResponseViewModel
    {
        public string AccessToken { get; set; }

        public DateTime ExpiresIn { get; set; }
    }
}
