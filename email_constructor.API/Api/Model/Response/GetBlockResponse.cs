namespace email_constructor.Api.Model;

public class GetBlockResponse
{
    /// <summary>
    /// Список блоков.
    /// </summary>
    public List<Block> Blocks { get; init; }
}