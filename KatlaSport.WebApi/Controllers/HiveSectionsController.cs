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
    [RoutePrefix("api/sections")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CustomExceptionFilter]
    [SwaggerResponseRemoveDefaults]
    public class HiveSectionsController : ApiController
    {
        private readonly IHiveSectionService _hiveSectionService;

        public HiveSectionsController(IHiveSectionService hiveSectionService)
        {
            _hiveSectionService = hiveSectionService ?? throw new ArgumentNullException(nameof(hiveSectionService));
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hive sections.", Type = typeof(HiveSectionListItem[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSections()
        {
            var hives = await _hiveSectionService.GetHiveSectionsAsync().ConfigureAwait(false);
            return Ok(hives);
        }

        [HttpGet]
        [Route("{hiveSectionId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a hive section.", Type = typeof(HiveSection))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSection(int hiveSectionId)
        {
            var hive = await _hiveSectionService.GetHiveSectionAsync(hiveSectionId).ConfigureAwait(false);
            return Ok(hive);
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "Creates a new hive section.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Unknown query syntax for the server.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Request conflict with current state of server.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> AddHiveSection([FromUri] int? hiveId, [FromBody] UpdateHiveSectionRequest createRequest)
        {
            if(hiveId == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var section = await _hiveSectionService.CreateHiveSectionAsync(hiveId.Value, createRequest);
            var location = string.Format("/api/sections/{0}", section.Id);
            return Created(location, section);

        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Updated hive section by id.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Unknown query syntax for the server.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Request conflict with current state of server.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> UpdateHiveSection([FromUri] int id, [FromBody] UpdateHiveSectionRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _hiveSectionService.UpdateHiveSectionAsync(id, updateRequest);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Deleted a hive section by id.")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "No content to answer the request.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Unknown query syntax for the server.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Request conflict with current state of server.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "The server encountered an unhandled situation.")]
        public async Task<IHttpActionResult> DeleteHiveSection([FromUri] int id)
        {
            await _hiveSectionService.DeleteHiveSectionAsync(id);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        [HttpPut]
        [Route("{hiveSectionId:int:min(1)}/status/{deletedStatus:bool}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Sets deleted status for an existed hive section.")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> SetStatus([FromUri] int hiveSectionId, [FromUri] bool deletedStatus)
        {
            await _hiveSectionService.SetStatusAsync(hiveSectionId, deletedStatus);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }
    }
}
