﻿using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System;
using API___TFI___Parcial1.Response;
using API___TFI___Parcial1.Managers;
using BLL.Contracts;
using Domain.Models;

namespace API___TFI___Parcial1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IGestorDocService _gestorDocService;

        public DocumentosController(IGestorDocService gestorDocService)
        {
            _gestorDocService=gestorDocService;
        }

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
        [HttpGet("ValidarDocumento")]
        public async Task<IActionResult> ValidarDocumento([FromBody] FileDataModel fileDataRequest)
        {
            try
            {
                var result = _gestorDocService.ValidarDoc(fileDataRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
