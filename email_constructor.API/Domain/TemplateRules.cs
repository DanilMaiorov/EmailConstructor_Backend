using email_constructor.Domain.Enums;

namespace email_constructor.Domain;

public class TemplateRules
{
    /// <summary>
    /// Содержит правила обработки шаблонов писем.
    /// </summary>
    public static bool RequiresProducts(TemplateType type) => type switch
    {
        TemplateType.Template_S 
            or TemplateType.Template_M 
            or TemplateType.Template_L
            or TemplateType.Template_XL 
            or TemplateType.Template_4MxX
            or TemplateType.Template_L_6S 
            or TemplateType.Template_Promo => true,
        
        TemplateType.Template_Info
            or TemplateType.Template_Info_Steps => false,
        
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}