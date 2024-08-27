﻿using banobras_bitacoras_persistence.mx.gob.banobras.bitacoras.persistence.application.inputport;
using banobras_bitacoras_persistence.mx.gob.banobras.bitacoras.persistence.dominio.model;
using banobras_bitacoras_persistence.mx.gob.banobras.bitacoras.persistence.infra.ada.inp.dto;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace banobras_bitacoras_persistence.mx.gob.banobras.bitacoras.persistence.infra.ada.inp.controller
{
    [Route("persistence/operaciones/")]
    [ApiController]
    public class BitacoraOperacionController : ControllerBase
    {
        #region Properties
        /// <summary>
        /// Instancia de la interfaz de puerto de entrada
        /// </summary>
        readonly IBitacoraOperacionInputPort iBitacoraOperacionInputPort;
        /// <summary>
        /// Instancia de la interfaz de logueo
        /// </summary>
        private readonly ILog _log;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor del controlador BitacoraOperacionController
        /// </summary>
        /// <remarks>
        /// Utiliza la inyección de dependencia de la interfaz a implementar definida en el Program.cs
        /// </remarks>
        /// <param name="iBitacoraOperacionInputPort"></param>
        /// <param name="log"></param>
        public BitacoraOperacionController(ILog log, IBitacoraOperacionInputPort iBitacoraOperacionInputPort)
        {
            _log = log;
            this.iBitacoraOperacionInputPort = iBitacoraOperacionInputPort;
            _log.Info("ENTRA -> BitacoraOperacionController");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Servicio para registrar movimientos en la bitácora de operaciones.
        /// </summary>
        /// <param name="request">Request: </param>
        /// <remarks>
        /// <br>Ejemplo request: </br>
        /// 
        ///     POST persistence/operaciones/registrar
        ///     {
        ///         "identificador":0
        ///         "aplicativoId": "MAC",
        ///         "capa": "business",
        ///         "metodo": "consultaUsuario",
        ///         "proceso": "obtenerInfo",
        ///         "subproceso": "obtenerRFC",
        ///         "detalleOperacion": "Consultar los datos de un usuario",
        ///         "transaccionId": "9680e51f-4766-4124-a3ff-02e9c3a5f9d6",
        ///         "ipEquipo": "127.0.0.1",
        ///         "fechaHoraTransaccion": "2024-08-12T02:13:36.862Z",
        ///         "usuarioOperador": "andres",
        ///         "nombreOperador": "Andres Gonz",
        ///         "expedienteOperador": "12345",
        ///         "areaOperador": "Administracion",
        ///         "puestoOperador": "Administrador",
        ///         "estatusOperacion": "C",
        ///         "respuestaOperacion": "Exito"
        ///     }
        /// </remarks>
        /// <br>Ejemplo response: </br>
        /// {
        ///     "statusCode": 200,
        ///     "message": "",
        ///     "success": true,
        ///     "responseType": 200,
        ///     "contenido": 
        ///     {
        ///         "identificador": 99
        ///     }
        /// }
        /// <response code="200">Operación exitosa</response>
        /// <response code="400">Error en la petición</response>
        /// <response code="500">Error interno en el servidor</response>
        [HttpPost]
        [Produces("application/json", Type = typeof(string))]
        [Consumes("application/json")]
        [Route("registrar")]
        [ProducesResponseType(typeof(ResponseBaseMicroservicio<BitacoraDtoResponse>), 200)]
        [ProducesResponseType(typeof(ResponseBaseMicroservicio<BitacoraDtoResponse>), 400)]
        [ProducesResponseType(typeof(ResponseBaseMicroservicio<BitacoraDtoResponse>), 500)]
        public async Task<IActionResult> registrar([FromBody] BitacoraOperacionDto request)
        {
            _log.Info("MÉTODO registrar");
            string message1 = string.Format("REQUEST: \n{0}", JsonConvert.SerializeObject(request, Formatting.Indented));
            _log.Info(message1);
            ResponseBaseMicroservicio<BitacoraDtoResponse> response = new ResponseBaseMicroservicio<BitacoraDtoResponse>();
            if (ModelState.IsValid)
            {
                var result = await iBitacoraOperacionInputPort.registrar(request);
                string message2 = string.Format("RESPONSE: \n{0}", JsonConvert.SerializeObject(result, Formatting.Indented));
                _log.Info(message2);
                if (result != null)
                {
                    if (result.Codigo == 200)
                    {
                        response.success = true;
                        response.Contenido = result.Contenido;
                        response.statusCode = 200;
                    }
                    else
                    {
                        response.statusCode = result.Codigo;
                        response.message = result.Mensaje;
                    }
                }
                else
                {
                    response.statusCode = 400;
                    response.message = "Error en la petición.";
                }
            }
            else
            {
                response.statusCode = 400;
                response.message = "Modelo inválido.";
            }
            response.responseType = new RestfulResponse().GetResponseType(response.statusCode);
            Console.WriteLine(JsonConvert.SerializeObject(new RestfulResponse().GetResponse(response), Formatting.Indented));
            string message3 = string.Format("RESPONSE MICROSERVICIO: \n{0}", JsonConvert.SerializeObject(new RestfulResponse().GetResponse(response), Formatting.Indented));
            _log.Info(message3);
            return new RestfulResponse().GetResponse(response);
        }

        /// <summary>
        /// Servicio para consultar movimientos de la bitácora de operaciones.
        /// </summary>
        /// <param name="request">Request: </param>
        /// <remarks>
        /// <br>Ejemplo request: </br>
        /// 
        ///     POST persistence/operaciones/consultar
        ///     {
        ///         "aplicativoId": "MAC",
        ///         "fechaHoraIni": "2024-08-01T19:15:44.504Z",
        ///         "fechaHoraFin": "2024-08-06T19:15:44.504Z",
        ///         "historico": false
        ///     } 
        /// </remarks>
        /// <br>Ejemplo response: </br>
        /// {
        ///     "statusCode": 200,
        ///     "message": "",
        ///     "success": true,
        ///     "responseType": 200,
        ///     "contenido": 
        ///     [
        ///         { "identificador": 99 }
        ///     ]
        /// }
        /// <response code="200">Operación exitosa</response>
        /// <response code="400">Error en la petición</response>
        /// <response code="500">Error interno en el servidor</response>
        [HttpPost]
        [Produces("application/json", Type = typeof(string))]
        [Consumes("application/json")]
        [Route("consultar")]
        [ProducesResponseType(typeof(ResponseBaseMicroservicio<List<BitacoraOperacionDto>>), 200)]
        [ProducesResponseType(typeof(ResponseBaseMicroservicio<List<BitacoraOperacionDto>>), 400)]
        [ProducesResponseType(typeof(ResponseBaseMicroservicio<List<BitacoraOperacionDto>>), 500)]
        public async Task<IActionResult> consultar([FromBody] BitacoraConsultaDto request)
        {
            _log.Info("MÉTODO consultar");
            string message1 = string.Format("REQUEST: \n{0}", JsonConvert.SerializeObject(request, Formatting.Indented));
            _log.Info(message1);
            ResponseBaseMicroservicio<List<BitacoraOperacionDto>> response = new ResponseBaseMicroservicio<List<BitacoraOperacionDto>>();
            if (ModelState.IsValid)
            {
                var result = await iBitacoraOperacionInputPort.consultar(request);
                string message2 = string.Format("RESPONSE: \n{0}", JsonConvert.SerializeObject(result, Formatting.Indented));
                _log.Info(message2);
                if (result != null)
                {
                    if (result.Codigo == 200)
                    {
                        response.success = true;
                        response.Contenido = result.Contenido;
                        response.statusCode = 200;
                    }
                    else
                    {
                        response.statusCode = result.Codigo;
                        response.message = result.Mensaje;
                    }
                }
                else
                {
                    response.statusCode = 400;
                    response.message = "Error en la petición.";
                }
            }
            else
            {
                response.statusCode = 400;
                response.message = "Modelo inválido.";
            }
            response.responseType = new RestfulResponse().GetResponseType(response.statusCode);
            Console.WriteLine(JsonConvert.SerializeObject(new RestfulResponse().GetResponse(response), Formatting.Indented));
            string message3 = string.Format("RESPONSE MICROSERVICIO: \n{0}", JsonConvert.SerializeObject(new RestfulResponse().GetResponse(response), Formatting.Indented));
            _log.Info(message3);
            return new RestfulResponse().GetResponse(response);
        }
        #endregion
    }
}