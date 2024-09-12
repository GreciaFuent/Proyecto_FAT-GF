using System;

public class Archivo
{
    public string Nombre { get; set; }
    public int Tamaño { get; set; }
    public DateTime FechaCreacion { get; set; }
    public List<string> BloquesDatos { get; set; }

    public Archivo(string nombre, int tamaño, DateTime fechaCreacion, List<string> bloquesDatos)
    {
        Nombre = nombre;
        Tamaño = tamaño;
        FechaCreacion = fechaCreacion;
        BloquesDatos = bloquesDatos;
    }

    public void MostrarInformacion()
    {
        Console.WriteLine($"Nombre: {Nombre}");
        Console.WriteLine($"Tamaño: {Tamaño} caracteres");
        Console.WriteLine($"Fecha de Creación: {FechaCreacion}");
        Console.WriteLine("----------");
    }
}