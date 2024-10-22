using LaTiQ.Core.DTO.Response.Topic;
using LaTiQ.WebAPI.Constants;
using LaTiQ.WebAPI.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LaTiQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public IActionResult GetListTopic()
        {
            return Ok(_topicService.GetListTopic());
        }

        [HttpGet("{topicId}")]
        public IActionResult GetTopic(Guid topicId)
        {
            TopicResponse? topicResponse = _topicService.GetTopic(topicId);

            if (topicResponse == null)
            {
                return BadRequest("Topic Id không tồn tại");
            } 
            else
            {
                return Ok(topicResponse);
            }
        }
    }
}
