
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        DateTime inicio = DateTime.Now;
        List<Task> pedidos = new List<Task>();
        Dictionary<int, TimeSpan> tiemposPedidos = new Dictionary<int, TimeSpan>();
        CancellationTokenSource cts = new CancellationTokenSource();

        for (int i = 1; i <= 7; i++)
        {
            int pedidoId = i;
            pedidos.Add(ProcesarPedido(pedidoId, cts.Token, tiemposPedidos));
        }
        var monitoreo = MonitorearPedidos(pedidos, cts.Token);
        Task primerPedido = await Task.WhenAny(pedidos);
        Console.WriteLine("Pedidos completados!");
        await Task.WhenAll(pedidos);
        cts.Cancel(); 
        DateTime fin = DateTime.Now;
        TimeSpan tiempoTotal = fin - inicio;

        MostrarEstadisticas(tiemposPedidos, tiempoTotal);
    }

    static async Task ProcesarPedido(int pedidoId, CancellationToken token, Dictionary<int, TimeSpan> tiemposPedidos)
    {
        Stopwatch timer = Stopwatch.StartNew();
        Console.WriteLine($"Pedido {pedidoId} iniciado.");

        var validacion = Task.Run(async () =>
        {
            Console.WriteLine($"Validando pedido {pedidoId}");
            await Task.Delay(1000);
            Console.WriteLine($"Pedido {pedidoId} validado.");
        }, token);

        await validacion;

        var empaque = Task.Run(async () =>
        {
            Console.WriteLine($"Empacando pedido {pedidoId}.");
            await Task.Delay(1500);
            Console.WriteLine($"Pedido {pedidoId} empaquetado.");
        }, token);

        await empaque;

        var ControlCalidad = Task.Run(async () =>
        {
            Console.WriteLine($"Pasando control calidad {pedidoId}.");
            await Task.Delay(1000);
            Console.WriteLine($"Pedido {pedidoId} aprobado.");
        }, token);

        await ControlCalidad;

        var envio = Task.Run(async () =>
        {
            Console.WriteLine($"Enviando pedido {pedidoId}.");
            await Task.Delay(2000);
            Console.WriteLine($"Pedido {pedidoId} enviado.");
        }, token);

        await envio;
        timer.Stop();
        lock (tiemposPedidos)
        {
            tiemposPedidos[pedidoId] = timer.Elapsed;
        }
    }

    static async Task MonitorearPedidos(List<Task> pedidos, CancellationToken token)
    {
        int pedidosPrevios = pedidos.Count;
        while (!token.IsCancellationRequested)
        {
            int pedidosEnProceso = pedidos.Count(t => !t.IsCompleted);
            if (pedidosEnProceso != pedidosPrevios)
            {
                Console.WriteLine($"Pedidos en proceso: {pedidosEnProceso}");
                pedidosPrevios = pedidosEnProceso;
            }
            if (pedidosEnProceso == 0) break;
            await Task.Delay(1000);
        }
    }

    static void MostrarEstadisticas(Dictionary<int, TimeSpan> tiemposPedidos, TimeSpan tiempoTotal)
    {
        Console.WriteLine("\nResumen de tiempos:");
        if (tiemposPedidos.Count == 0)
        {
            Console.WriteLine("No hay datos de pedidos.");
            return;
        }
        var pedidoMasRapido = tiemposPedidos.OrderBy(t => t.Value).First();
        var pedidoMasLento = tiemposPedidos.OrderByDescending(t => t.Value).First();
        double promedio = tiemposPedidos.Values.Average(t => t.TotalSeconds);

        Console.WriteLine($"El pedido más rápido: {pedidoMasRapido.Key} - {pedidoMasRapido.Value.TotalSeconds:F2} segundos");
        Console.WriteLine($"El pedido más lento: {pedidoMasLento.Key} - {pedidoMasLento.Value.TotalSeconds:F2} segundos");
        Console.WriteLine($"Tiempo promedio por pedido: {promedio:F2} segundos");
        Console.WriteLine($"Tiempo total de procesamiento: {tiempoTotal.TotalSeconds:F2} segundos");
    }
}
