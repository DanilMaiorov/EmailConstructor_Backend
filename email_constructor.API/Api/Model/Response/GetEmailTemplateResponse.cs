namespace email_constructor.Api.Model;

public class GetEmailTemplateResponse
{
    /// <summary>
    /// Список блоков.
    /// </summary>
    public List<Block> Blocks { get; init; }
}