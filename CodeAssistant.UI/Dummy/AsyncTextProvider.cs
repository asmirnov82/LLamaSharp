using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAssistant.UI
{
    internal class AsyncTextProvider
    {
        public static async IAsyncEnumerable<string> FetchData()
        {
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(1000);
                yield return i.ToString();
            }
        }
    }
}
