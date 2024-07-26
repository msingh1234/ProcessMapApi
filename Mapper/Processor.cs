using System.Text;

namespace Mapper
{
    public class Processor : IProcessor
    {

        /// <summary>
        /// GenrateResponse;
        /// </summary>
        /// <param name="longtext"></param>
        /// <returns></returns>
        public string GenrateResponse(string longtext)
        {
            if (longtext != null)
            {
                StringBuilder sb = new StringBuilder();
                int WordCount = 0;
                var oprator = new WordOperator();
                try
                {
                    oprator.MapReduce(longtext);
                    foreach (KeyValuePair<string, int> kp in oprator.wordStore)
                    {
                        sb.Append(kp.Key+ ":" + kp.Value);
                        WordCount+=kp.Value;
                    }
                    longtext = "Total Words: " + WordCount + "\r\n";
                    longtext +=sb.ToString();
                    return longtext;
                }
                catch (AggregateException ae)
                {
                    var ignoredExceptions = new List<Exception>();
                    // This is where you can choose which exceptions to handle.
                    foreach (var ex in ae.Flatten().InnerExceptions)
                    {
                        if (ex is ArgumentException) Console.WriteLine(ex.Message);
                        else ignoredExceptions.Add(ex);
                    }
                    if (ignoredExceptions.Count > 0)
                    {
                        throw new AggregateException(ignoredExceptions);
                    }
                    else
                    {
                        throw;
                    }

                }

            }
            else
            {
                return "Missing Input text";
            }
        }
    }
}
