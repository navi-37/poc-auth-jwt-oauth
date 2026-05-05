using System.ComponentModel.DataAnnotations;

namespace Backoffice.ViewModels;

public class CreateTenantViewModel
{
    [Required(ErrorMessage = "El slug es requerido.")]
    [MaxLength(100)]
    [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Solo letras minúsculas, números y guiones.")]
    public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;
}
