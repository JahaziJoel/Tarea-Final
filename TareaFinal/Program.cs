using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class CarreraAutos
{
    static async Task Main()
    {
        Console.WriteLine("Simulador de carrera de autos!");

        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        string[] autos = { "Auto 1", "Auto 2", "Auto 3", "Auto 4" };
        int distanciaMeta = 100;

        var tareasAutos = autos.Select(auto =>
            Task.Run(() => CorrerAuto(auto, distanciaMeta, token), token).ContinueWith(t => new { Tarea = t, Auto = auto })
        ).ToArray();

        var primeraTarea = await Task.WhenAny(tareasAutos);

        Console.WriteLine($"\n {primeraTarea.Result.Auto} ha ganado la carrera. ");

        cts.Cancel();

        // Mostrar finalización de las tareas
        foreach (var tarea in tareasAutos)
        {
            await tarea.ContinueWith(t =>
            {
                if (t.Result.Tarea.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine($" {t.Result.Auto} terminó la carrera.");
                else if (t.Result.Tarea.Status == TaskStatus.Canceled)
                    Console.WriteLine($" {t.Result.Auto} fue cancelado.");
            });
        }
    }

    static async Task CorrerAuto(string nombre, int meta, CancellationToken token)
    {
        int distancia = 0;
        Random rnd = new Random();

        while (distancia < meta)
        {
            await Task.Delay(rnd.Next(500, 1500), token);

            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            distancia += rnd.Next(10, 20);
            Console.WriteLine($"{nombre} ha avanzado {distancia}/{meta}");
        }

        Console.WriteLine($" {nombre} ha llegado a la meta.");
    }
}
