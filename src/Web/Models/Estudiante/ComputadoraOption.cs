namespace Web.Models.Estudiante;

public class ComputadoraOption
{
	public int Id { get; init; }
	public string Nombre { get; init; } = string.Empty;
	public string Descripcion { get; init; } = string.Empty;
	public int LaboratorioId { get; init; }
	public string LaboratorioNombre { get; init; } = string.Empty;

	public ComputadoraOption(int id, string nombre, string descripcion, int laboratorioId, string laboratorioNombre)
	{
		Id = id;
		Nombre = nombre;
		Descripcion = descripcion;
		LaboratorioId = laboratorioId;
		LaboratorioNombre = laboratorioNombre;
	}

	public ComputadoraOption() { }
}
