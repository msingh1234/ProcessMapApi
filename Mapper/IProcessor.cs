namespace Mapper
{
    public interface IProcessor
    {
        /// <summary>
        /// Retur map/reduce response.
        /// </summary>
        /// <param name="longtext"></param>
        /// <returns></returns>
        string GenrateResponse(string longtext);
    }
}
