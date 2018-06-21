using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using KatlaSport.Services.HiveManagement;
using KatlaSport.WebApi.CustomFilters;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;

namespace KatlaSport.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/hives")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CustomExceptionFilter]
    [SwaggerResponseRemoveDefaults]
    public class HivesController : ApiController
    {
        private readonly IHiveService _hiveService;
        private readonly IHiveSectionService _hiveSectionService;

        public HivesController(IHiveService hiveService, IHiveSectionService hiveSectionService)
        {
            _hiveService = hiveService ?? throw new ArgumentNullException(nameof(hiveService));
            _hiveSectionService = hiveSectionService ?? throw new ArgumentNullException(nameof(hiveSectionService));
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hives.", Type = typeof(HiveListItem[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> GetHives()
        {
            var hives = await _hiveService.GetHivesAsync();
            return Ok(hives);
        }

        [HttpGet]
        [Route("{hiveId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a hive.", Type = typeof(Hive))]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The server can not find the requested resource.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> GetHive(int hiveId)
        {
            var hive = await _hiveService.GetHiveAsync(hiveId);
            return Ok(hive);
        }

        [HttpGet]
        [Route("{hiveId:int:min(1)}/sections")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hive sections for specified hive.", Type = typeof(HiveSectionListItem))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> GetHiveSections(int hiveId)
        {
            var hive = await _hiveSectionService.GetHiveSectionsAsync(hiveId);
            return Ok(hive);
        }

        [HttpPut]
        [Route("{hiveId:int:min(1)}/status/{deletedStatus:bool}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Sets deleted status for an existed hive.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The server can not find the requested resource.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> SetStatus([FromUri] int hiveId, [FromUri] bool deletedStatus)
        {
            await _hiveService.SetStatusAsync(hiveId, deletedStatus);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "Creates a new hive.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Unknown query syntax for the server.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Request conflict with current state of server.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> AddHive([FromBody] UpdateHiveRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                
            var hive = await _hiveService.CreateHiveAsync(createRequest);
            var location = string.Format("/api/hives/{0}", hive.Id);
            return Created<Hive>(location, hive);

        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Updated hive by id.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Unknown query syntax for the server.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Request conflict with current state of server.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> UpdateHive([FromUri] int id, [FromBody] UpdateHiveRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _hiveService.UpdateHiveAsync(id, updateRequest);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Deleted a hive by id.")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "No content to answer the request.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Unknown query syntax for the server.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Request conflict with current state of server.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> DeleteHive([FromUri] int id)
        {
            await _hiveService.DeleteHiveAsync(id); 
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }
    }
}
