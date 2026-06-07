using System.Text.Json.Serialization;

namespace FileService.Contracts.Shared.Enums;

/// <summary>
/// Типы ошибок приложения
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    /// <summary>
    /// Ошибка валидации данных (например, при попытке создать объект с недопустимыми значениями полей)
    /// </summary>
    Validation,
    
    /// <summary>
    /// Объект не найден (например, при попытке получить объект по несуществующему идентификатору)
    /// </summary>
    NotFound,
    
    /// <summary>
    /// Неизвестная ошибка, которая не попадает ни в одну из других категорий (например, внутренняя ошибка сервера)
    /// </summary>
    Failure,
    
    /// <summary>
    /// Конфликт данных (например, при попытке создать объект с уже существующим уникальным полем)
    /// </summary>
    Conflict,
    
    /// <summary>
    /// Ошибка аутентификации (например, неверные учетные данные)
    /// </summary>
    Authentication,
    
    /// <summary>
    /// Ошибка авторизации (например, недостаточно прав для выполнения действия)
    /// </summary>
    Authorization
}