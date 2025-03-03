using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Inicio del procesamiento de imagenes...");
        CancellationTokenSource cts = new CancellationTokenSource();

        try
        {
            var processingTask = Task.Factory.StartNew(async () =>
            {
                var resizeTask = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Cambiando tamaño...");
                    Task.Delay(new Random().Next(500, 2000)).Wait();
                    return "Tamaño cambiado";
                }, TaskCreationOptions.AttachedToParent);

                var filterTask = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Aplicando Filtro...");
                    Task.Delay(new Random().Next(500, 2000)).Wait();
                    return "Filtro aplicado";
                }, TaskCreationOptions.AttachedToParent);

                var watermarkTask = Task.Run(() =>
                {
                    Console.WriteLine("Agregando marca de agua...");
                    Task.Delay(new Random().Next(500, 2000)).Wait();
                    return "Marca de agua agregada";
                }, cts.Token);

                resizeTask.ContinueWith(t => Console.WriteLine(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
                filterTask.ContinueWith(t => Console.WriteLine(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

                watermarkTask.ContinueWith(t => Console.WriteLine("Error: Tarea cancelada"), TaskContinuationOptions.OnlyOnCanceled);

                var firstCompleted = await Task.WhenAny(resizeTask, filterTask, watermarkTask);
                Console.WriteLine($"La primera tarea en completarse fue: {firstCompleted.Result}");
            }, cts.Token, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            await processingTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en procesamiento: {ex.Message}");
        }
        finally
        {
            cts.Dispose();
        }

        Console.WriteLine("Procesamiento finalizado.");

    }
}
