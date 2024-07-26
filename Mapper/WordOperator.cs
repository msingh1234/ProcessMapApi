using System.Collections.Concurrent;
using System.Text;

namespace Mapper
{
    public class WordOperator 
    {

        public static ConcurrentBag<string>? wordBag = new ConcurrentBag<string>();
        public BlockingCollection<string>? wordChunks = new BlockingCollection<string>(wordBag);
        public ConcurrentDictionary<string, int> wordStore = new ConcurrentDictionary<string, int>();

        /// 
        /// This method uses Yield Return processing to break up input text into blocks of 100 characters or less
        /// using the space character as a word delimiter:
        /// 
        private IEnumerable<string> GenerateWordBlocks(string fileText)
        {
            int blockSize = 100;
            int startPos = 0;
            for (int i = 0; i < fileText.Length; i++)
            {
                i = i + blockSize > fileText.Length -1 ? fileText.Length -1 : i + blockSize;

                while (i >= startPos && fileText[i] != ' ')
                {
                    i--;
                }

                int len;
                if (i == startPos)
                {
                    i = i + blockSize > (fileText.Length - 1) ? fileText.Length - 1 : i + blockSize;
                    len = (i - startPos) + 1;
                }
                else
                {
                    len = i - startPos;
                }

                yield return fileText.Substring(startPos, len).Trim();
                startPos = i;
            }

        }

        /// <summary>
        /// This method uses multiple threads to find and clean words from the blocks of input text provided by the 
        /// Yield Return method. The Parallel.ForEach mapping process begins as soon as the first block
        /// of text is identified ,since Yield Return is being used.
        /// </summary>
        /// <param name="fileText">fileText.</param>
        private void MapWords(string fileText)
        {
            if (!string.IsNullOrEmpty(fileText))
            {
                // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
                var exceptions = new ConcurrentQueue<Exception>();
                Parallel.ForEach<string>(GenerateWordBlocks(fileText), wordBlock =>
                 {   //split the block into words
                     string[] words = wordBlock.Split(' ');
                     StringBuilder wordBuffer = new StringBuilder();
                     try
                     {
                         //map each word afyer cleanup
                         foreach (string word in words)
                         {   //Remove all spaces and punctuation
                             foreach (char c in word)
                             {
                                 if (char.IsLetterOrDigit(c) || c == '\'' || c == '-')
                                     wordBuffer.Append(c);
                             }
                             //Send word to the wordChunks Blocking Collection
                             if (wordBuffer.Length > 0)
                             {
                                 wordChunks.Add(wordBuffer.ToString());
                                 wordBuffer.Clear();
                             }
                         }
                     }
                     // Store the exception and continue with the loop.
                     catch (Exception e)
                     {
                         exceptions.Enqueue(e);
                     }

                 });
                wordChunks.CompleteAdding();
            }
            else
            {
                throw new ArgumentNullException("Input text is missingf");
            }
        }
        private void ReduceWords()
        {
            Parallel.ForEach(wordChunks.GetConsumingEnumerable(), word =>
            {   //if the word exists, use a thread safe delegate to increment the value by 1
                //otherwise, add the word with a default value of 1
                wordStore.AddOrUpdate(word, 1, (key, oldValue) => Interlocked.Increment(ref oldValue));
            });
        }

        public void MapReduce(string fileText)
        {   //Reset the Blocking Collection, if already used
            if (wordChunks.IsAddingCompleted)
            {
                wordBag = new ConcurrentBag<string>();
                wordChunks = new BlockingCollection<string>(wordBag);
            }

            //Create background process to map input data to words
            ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                MapWords(fileText);
            });

            //Reduce mapped words
            ReduceWords();
        }

    }
}
