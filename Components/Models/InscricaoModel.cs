using System.ComponentModel.DataAnnotations;

namespace SistemaInscricaoCursosBlazor.Components.Models;

/// <summary>
/// Modelo de dados do formulário de inscrição.
/// As DataAnnotations definem as regras de validação usadas pelo <DataAnnotationsValidator/>.
/// </summary>
public class InscricaoModel
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail em formato válido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A idade é obrigatória.")]
    [Range(16, 100, ErrorMessage = "A idade deve estar entre 16 e 100 anos.")]
    public int Idade { get; set; }

    [Required(ErrorMessage = "Selecione um curso.")]
    public string? CursoId { get; set; }
}
