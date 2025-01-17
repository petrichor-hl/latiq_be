using LaTiQ.Application.DTOs;
using LaTiQ.Core.DTOs.Topic.Res;
using LaTiQ.WebAPI.ServiceContracts;
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
            return Ok(ApiResult<IEnumerable<TopicResponse>>.Success(_topicService.GetListTopic()));
        }

        [HttpGet("{topicId:guid}")]
        public IActionResult GetTopic(Guid topicId)
        {
            return Ok(ApiResult<TopicResponse>.Success(_topicService.GetTopic(topicId)));
        }
    }
}
