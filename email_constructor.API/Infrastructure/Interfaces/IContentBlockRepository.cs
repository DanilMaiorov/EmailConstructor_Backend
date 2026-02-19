namespace email_constructor.Infrastructure.Interfaces;

public interface IContentBlockRepository
{
    public void GetContentBlock(string language, string storeId);
}