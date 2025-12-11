namespace Web.Models.Estudiante;

public class ServicioOption
{
	public int Id { get; init; }
	public string Nombre { get; init; } = string.Empty;
	public string Descripcion { get; init; } = string.Empty;

	public ServicioOption(int id, string nombre, string descripcion)
	{
		Id = id;
		Nombre = nombre;
		Descripcion = descripcion;
	}

	public ServicioOption() { }
}
