using LaTiQ.Application.Exceptions;
using LaTiQ.Core.DTOs.Topic.Res;
using LaTiQ.WebAPI.Constants;
using LaTiQ.WebAPI.ServiceContracts;

namespace LaTiQ.WebAPI.Services
{
    public class TopicService : ITopicService
    {
        public IEnumerable<TopicResponse> GetListTopic()
        {
            return TopicData.Topics.Select(topic => new TopicResponse
            {
                Id = topic.Id,
                Name = topic.Name,
                ImageUrl = topic.ImageUrl,
            });
        }

        public TopicResponse GetTopic(Guid topicId)
        {
            var topic = TopicData.Topics.Find(topic => topic.Id == topicId);

            if (topic == null)
            {
                throw new NotFoundException($"Không tìm thấy Topic {topicId}");
            }
            
            return new TopicResponse
            {
                Id = topic.Id,
                Name = topic.Name,
                ImageUrl = topic.ImageUrl
            };
        }
    }
}
