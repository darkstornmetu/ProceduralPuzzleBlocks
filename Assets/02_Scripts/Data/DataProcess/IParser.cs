namespace ProceduralBlocks.Data.Process
{
    public interface IParser<out T> where T : IParsable
    {
        T ParseData(string file);
    }
}