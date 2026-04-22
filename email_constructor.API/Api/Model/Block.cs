using System.ComponentModel.DataAnnotations;

namespace email_constructor.Api.Model;

public class Block
{
    public string Type { get; init; }
    public string Key { get; init; }
    public string Variant { get; init; }
    public Dictionary<string, string> Payload { get; init; }
}