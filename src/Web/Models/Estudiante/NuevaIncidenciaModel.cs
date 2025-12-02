using System.ComponentModel.DataAnnotations;

namespace Web.Models.Estudiante;

public class NuevaIncidenciaModel
{
    [Required(ErrorMessage = "Selecciona una computadora")]
    [Display(Name = "Computadora")]
    public int? ComputadoraID { get; set; }

    [Required(ErrorMessage = "Ingresa un título")]
    [StringLength(200, ErrorMessage = "El título es demasiado largo")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Describe el problema")]
    [StringLength(2000, ErrorMessage = "La descripción es demasiado larga")]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public string Prioridad { get; set; } = "Media";
}
