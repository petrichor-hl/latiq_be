using LaTiQ.Core.DTO.Response.Topic;
using LaTiQ.WebAPI.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LaTiQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetListTopic()
        {
            return Ok(TopicData.Topics.Select(topic => new TopicResponse
            {
                Id = topic.Id,
                Name = topic.Name,
                ImageUrl = topic.ImageUrl,
            }));
        }

        [HttpGet("{topicId}")]
        public IActionResult GetTopic(Guid topicId)
        {
            var topic = TopicData.Topics.Find(topic => topic.Id.Equals(topicId));

            if (topic == null)
            {
                return BadRequest("Topic Id không tồn tại");
            } 
            else
            {
                return Ok(new TopicResponse 
                { 
                    Id = topic.Id, 
                    Name = topic.Name,
                    ImageUrl = topic.ImageUrl
                });
            }
        }
    }
}
