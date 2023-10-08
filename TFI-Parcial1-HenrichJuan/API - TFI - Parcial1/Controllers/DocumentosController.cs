using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System;
using API___TFI___Parcial1.Response;
using API___TFI___Parcial1.Managers;

namespace API___TFI___Parcial1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentosController : ControllerBase
    {
        [HttpPost("ImprimirDocumento")]
        public async Task<IActionResult> ImprimirDocumento([FromBody] FileDataRequest fileDataRequest)
        {
            try
            {
                var rabbitMqManager = new RabbitMqManager("localhost", "guest", "guest");

                // Declarar las colas
                rabbitMqManager.DeclareQueue("SenderQueue");
                // Enviar mensajes a las colas
                fileDataRequest.FechaInsercion = DateTime.Now;
                rabbitMqManager.SendMessage("SenderQueue", fileDataRequest);

                rabbitMqManager.Close();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }
}
