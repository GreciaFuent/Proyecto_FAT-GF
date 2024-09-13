using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        List<Archivo> archivos = new List<Archivo>();
        string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = $"{escritorio}/fat_table.json";
        string op = "0";
        
        while (op != "7")
        {
            Console.Clear();
            Console.WriteLine("1. Crear un archivo y agregar datos");
            Console.WriteLine("2. Listar archivos");
            Console.WriteLine("3. Abrir un archivo");
            Console.WriteLine("4. Modificar un archivo");
            Console.WriteLine("5. Eliminar un archivo");
            Console.WriteLine("6. Recuperar un archivo");
            Console.WriteLine("7. Salir");
            Console.WriteLine("");
            Console.WriteLine("Ingresa la opción:");
            op = Console.ReadLine()!;

            switch (op)
            {
                case "1":
                    Console.Clear();
                    CrearArchivo(archivos, filePath);
                    Console.ReadKey();
                    break;

                case "2":
                    Console.Clear();
                    ListarArchivos(archivos, filePath);
                    Console.ReadKey();
                    break;

                case "3":
                    Console.Clear();
                    AbrirArchivo(archivos, filePath);
                    Console.ReadKey();
                    break;

                case "4":
                    Console.Clear();
                    ModificarArchivo(archivos, filePath);
                    Console.ReadKey();
                    break;

                case "5":
                    Console.Clear();
                    EliminarArchivo(archivos, filePath);
                    Console.ReadKey();
                    break;

                case "6":
                    Console.Clear();
                    RecuperarArchivo(archivos, filePath);
                    Console.ReadKey();
                    break;

                case "7":
                    Console.Clear();
                    Console.WriteLine("Saliendo...");
                    Console.ReadKey();
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("Opción no válida, por favor ingresa una opción válida.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void CrearArchivo(List<Archivo> lista, string ruta)
    {
        Console.WriteLine("Ingrese el nombre del archivo:");
        string nombre = Console.ReadLine()!;

        Console.WriteLine("Ingrese los datos del archivo (texto):");
        string datos = Console.ReadLine()!;

        List<string> bloquesDatos = DividirEnBloques(datos);

        Archivo archivo = new Archivo(nombre, datos.Length, DateTime.Now, bloquesDatos, false);
        lista.Add(archivo);

        string jsonString = JsonSerializer.Serialize(lista);
        File.WriteAllText(ruta, jsonString);

        GuardarBloques(bloquesDatos, nombre);

        Console.WriteLine("Archivo creado correctamente.");
    }

    static List<string> DividirEnBloques(string datos)
    {
        var bloques = new List<string>();
        for (int i = 0; i < datos.Length; i += 20)
        {
            string bloque = datos.Substring(i, Math.Min(20, datos.Length - i));
            bloques.Add(bloque);
        }
        return bloques;
    }

    static void GuardarBloques(List<string> bloques, string nombreArchivo)
    {
        for (int i = 0; i < bloques.Count; i++)
        {
            string nombreBloque = $"{nombreArchivo}_bloque{i + 1}.json";
            string jsonBloque = JsonSerializer.Serialize(new { datos = bloques[i], siguiente_bloque = (i < bloques.Count - 1) });
            File.WriteAllText(nombreBloque, jsonBloque);
        }
    }

    static void ListarArchivos(List<Archivo> lista, string ruta)
    {
        if (File.Exists(ruta))
        {
            string jsonFromFile = File.ReadAllText(ruta);
            List<Archivo> deserializedArchivos = JsonSerializer.Deserialize<List<Archivo>>(jsonFromFile)!;

            foreach (Archivo archivo in deserializedArchivos)
            {
                if (!archivo.PapeleraReciclaje)
                {
                    archivo.MostrarInformacion();
                }
            }
        }
        else
        {
            Console.WriteLine("No hay archivos disponibles.");
        }
    }

    static void AbrirArchivo(List<Archivo> lista, string ruta)
    {
        if (File.Exists(ruta))
        {
            string jsonFromFile = File.ReadAllText(ruta);
            List<Archivo> deserializedArchivos = JsonSerializer.Deserialize<List<Archivo>>(jsonFromFile)!;

            // Mostrar lista de archivos
            MostrarArchivosDisponibles(deserializedArchivos);

            Console.WriteLine("Ingrese el nombre del archivo a abrir:");
            string nombreArchivo = Console.ReadLine()!;

            Archivo? archivo = deserializedArchivos.Find(a => a.Nombre == nombreArchivo && !a.PapeleraReciclaje);
            if (archivo != null)
            {
                Console.WriteLine($"Archivo: {archivo.Nombre}");
                Console.WriteLine($"Tamaño: {archivo.Tamaño} caracteres");
                Console.WriteLine("Contenido:");
                foreach (string bloque in archivo.BloquesDatos)
                {
                    Console.WriteLine(bloque);
                }
            }
            else
            {
                Console.WriteLine("Archivo no encontrado o está en la papelera de reciclaje.");
            }
        }
    }

    static void ModificarArchivo(List<Archivo> lista, string ruta)
    {
        if (File.Exists(ruta))
        {
            string jsonFromFile = File.ReadAllText(ruta);
            List<Archivo> deserializedArchivos = JsonSerializer.Deserialize<List<Archivo>>(jsonFromFile)!;

            MostrarArchivosDisponibles(deserializedArchivos);

            Console.WriteLine("Ingrese el nombre del archivo a modificar:");
            string nombreArchivo = Console.ReadLine()!;

            Archivo? archivo = deserializedArchivos.Find(a => a.Nombre == nombreArchivo && !a.PapeleraReciclaje);
            if (archivo != null)
            {
                Console.WriteLine($"Contenido actual del archivo: {archivo.Nombre}");
                foreach (string bloque in archivo.BloquesDatos)
                {
                    Console.WriteLine(bloque);
                }

                Console.WriteLine("\nIngrese el nuevo contenido (Presione ESCAPE para confirmar la modificación):");

                string nuevosDatos = string.Empty;
                ConsoleKeyInfo tecla;

                // Capturar entrada de datos hasta que se presione ESCAPE
                while ((tecla = Console.ReadKey(true)).Key != ConsoleKey.Escape)
                {
                    if (tecla.Key == ConsoleKey.Backspace && nuevosDatos.Length > 0)
                    {
                        nuevosDatos = nuevosDatos.Remove(nuevosDatos.Length - 1);
                        Console.Write("\b \b");
                    }
                    else if (!char.IsControl(tecla.KeyChar))
                    {
                        nuevosDatos += tecla.KeyChar;
                        Console.Write(tecla.KeyChar);
                    }
                }

                Console.WriteLine("\n¿Desea guardar los cambios? (S/N)");
                if (Console.ReadKey().Key == ConsoleKey.S)
                {
                    archivo.BloquesDatos = DividirEnBloques(nuevosDatos);
                    archivo.Tamaño = nuevosDatos.Length;
                    archivo.FechaModificacion = DateTime.Now;

                    string jsonString = JsonSerializer.Serialize(deserializedArchivos);
                    File.WriteAllText(ruta, jsonString);

                    GuardarBloques(archivo.BloquesDatos, archivo.Nombre);

                    Console.WriteLine("\nArchivo modificado correctamente.");
                }
                else
                {
                    Console.WriteLine("\nModificación cancelada.");
                }
            }
            else
            {
                Console.WriteLine("Archivo no encontrado.");
            }
        }
    }

    static void EliminarArchivo(List<Archivo> lista, string ruta)
    {
        if (File.Exists(ruta))
        {
            string jsonFromFile = File.ReadAllText(ruta);
            List<Archivo> deserializedArchivos = JsonSerializer.Deserialize<List<Archivo>>(jsonFromFile)!;

            MostrarArchivosDisponibles(deserializedArchivos);

            Console.WriteLine("Ingrese el nombre del archivo a eliminar:");
            string nombreArchivo = Console.ReadLine()!;

            Archivo? archivo = deserializedArchivos.Find(a => a.Nombre == nombreArchivo && !a.PapeleraReciclaje);
            if (archivo != null)
            {
                archivo.PapeleraReciclaje = true;
                archivo.FechaModificacion = DateTime.Now;

                string jsonString = JsonSerializer.Serialize(deserializedArchivos);
                File.WriteAllText(ruta, jsonString);

                Console.WriteLine("Archivo movido a la papelera de reciclaje.");
            }
            else
            {
                Console.WriteLine("Archivo no encontrado o ya está en la papelera de reciclaje.");
            }
        }
    }

    static void RecuperarArchivo(List<Archivo> lista, string ruta)
    {
        if (File.Exists(ruta))
        {
            string jsonFromFile = File.ReadAllText(ruta);
            List<Archivo> deserializedArchivos = JsonSerializer.Deserialize<List<Archivo>>(jsonFromFile)!;

            MostrarArchivosEnPapelera(deserializedArchivos);

            Console.WriteLine("Ingrese el nombre del archivo a recuperar:");
            string nombreArchivo = Console.ReadLine()!;

            Archivo? archivo = deserializedArchivos.Find(a => a.Nombre == nombreArchivo && a.PapeleraReciclaje);
            if (archivo != null)
            {
                archivo.PapeleraReciclaje = false;
                archivo.FechaModificacion = DateTime.Now;

                string jsonString = JsonSerializer.Serialize(deserializedArchivos);
                File.WriteAllText(ruta, jsonString);

                Console.WriteLine("Archivo recuperado correctamente.");
            }
            else
            {
                Console.WriteLine("Archivo no encontrado o no está en la papelera de reciclaje.");
            }
        }
    }

    static void MostrarArchivosDisponibles(List<Archivo> archivos)
    {
        Console.WriteLine("Archivos disponibles:");
        foreach (var archivo in archivos)
        {
            if (!archivo.PapeleraReciclaje)
            {
                Console.WriteLine($"- {archivo.Nombre}");
            }
        }
    }

    static void MostrarArchivosEnPapelera(List<Archivo> archivos)
    {
        Console.WriteLine("Archivos en la papelera de reciclaje:");
        foreach (var archivo in archivos)
        {
            if (archivo.PapeleraReciclaje)
            {
                Console.WriteLine($"- {archivo.Nombre}");
            }
        }
    }
}


