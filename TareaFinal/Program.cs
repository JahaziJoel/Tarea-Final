using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        List<Task> pedidos = new List<Task>();
        CancellationTokenSource cts = new CancellationTokenSource();

<<<<<<< HEAD
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
=======
        for (int i = 1; i <= 3; i++)
>>>>>>> 89963d1 (Primer commit)
        {
            int pedidoId = i;
            pedidos.Add(ProcesarPedido(pedidoId, cts.Token));
        }

        Task primerPedido = await Task.WhenAny(pedidos);
        Console.WriteLine("Pedido completado más rápido.");

        await Task.WhenAll(pedidos);
    }

    static Task ProcesarPedido(int pedidoId, CancellationToken token)
    {
        return Task.Factory.StartNew(() =>
        {
<<<<<<< HEAD
            await Task.Delay(rnd.Next(500, 1500), token);
=======
            Console.WriteLine($"Pedido {pedidoId} iniciado.");
>>>>>>> 89963d1 (Primer commit)

            var validacion = Task.Run(async () =>
            {
                Console.WriteLine($"Validando pedido {pedidoId}...");
                await Task.Delay(1000);
            }, token);

            var empaque = validacion.ContinueWith(async t =>
            {
                Console.WriteLine($"Empacando pedido {pedidoId}...");
                await Task.Delay(1500); 
            }, TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();

            var envio = empaque.ContinueWith(async t =>
            {
                Console.WriteLine($"Enviando pedido {pedidoId}...");
                await Task.Delay(2000);
            }, TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();

            envio.ContinueWith(t =>
            {
                Console.WriteLine($"Error en pedido {pedidoId}, cancelando...");
                token.ThrowIfCancellationRequested();
            }, TaskContinuationOptions.OnlyOnCanceled);

            return envio;
        }, token, TaskCreationOptions.AttachedToParent, TaskScheduler.Default).Unwrap();
    }
}
