namespace email_constructor.Domain.Enums;

/// <summary>
/// Тип шаблонного письма. Названия повторяют фронт (регистр не важен).
/// </summary>
public enum TemplateType
{
    /// <summary>
    /// Шаблон S.
    /// </summary>
    Template_S,

    /// <summary>
    /// Шаблон L.
    /// </summary>
    Template_L,

    /// <summary>
    /// Шаблон M.
    /// </summary>
    Template_M,

    /// <summary>
    /// Шаблон XL.
    /// </summary>
    Template_XL,

    /// <summary>
    /// Шаблон 4MxX.
    /// </summary>
    Template_4MxX,

    /// <summary>
    /// Шаблон L_6S.
    /// </summary>
    Template_L_6S,

    /// <summary>
    /// Промо-шаблон.
    /// </summary>
    Template_Promo,

    /// <summary>
    /// Информационный шаблон.
    /// </summary>
    Template_Info,

    /// <summary>
    /// Информационный шаблон с пошаговой инструкцией.
    /// </summary>
    Template_Info_Steps
}