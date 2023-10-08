using Dapper;
using Newtonsoft.Json;
using System.Data;
using UI___TFI___Parcial1.Data;
using UI___TFI___Parcial1.Helpers.Contracs;
using UI___TFI___Parcial1.Managers;
using static MudBlazor.CategoryTypes;

namespace UI___TFI___Parcial1.Services
{
    public class PrintResponseService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;
        FileDataModel _fileDataModel;
        private readonly IDapper _dapper;

        public PrintResponseService(IServiceScopeFactory scopeFactory, IDapper dapper)
        {
            _scopeFactory = scopeFactory;
            _dapper = dapper;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // Ejecutar cada 1 minuto
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                // Lógica para leer las respuestas de impresión y registrarlas en la base de datos
                // ...
                _fileDataModel = new FileDataModel();

                var rabbitMqManager = new RabbitMqManager("localhost", "guest", "guest");
                var result = await rabbitMqManager.ConsumeMessages("ReceiverQueue");

                _fileDataModel = JsonConvert.DeserializeObject<FileDataModel>(result);

                var sp = "INSERT INTO [dbo].[DocumentosRegistrados] (Nombre, FechaInsercion, FechaImpresion) VALUES (@Nombre, @FechaInsercion, @FechaImpresion)";
                var parms = new DynamicParameters();
                parms.Add("Nombre", _fileDataModel.Nombre, DbType.String);
                parms.Add("FechaInsercion", _fileDataModel.FechaInsercion, DbType.DateTime);
                parms.Add("FechaImpresion", _fileDataModel.FechaImpresion, DbType.DateTime);

                await Task.Run(() => _dapper.Execute(sp, parms, CommandType.Text));

                // Ejemplo de cómo ejecutar una consulta con Dapper
                // var result = dapperHelper.Get<TipoDeRespuesta>("NombreDelStoredProc", parametros);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
