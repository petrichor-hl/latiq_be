using LaTiQ.Core.DTO.Response.Topic;
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

        public TopicResponse? GetTopic(Guid topicId)
        {
            var topic = TopicData.Topics.Find(topic => topic.Id.Equals(topicId));

            if (topic == null)
            {
                return null;
            }
            else
            {
                return new TopicResponse
                {
                    Id = topic.Id,
                    Name = topic.Name,
                    ImageUrl = topic.ImageUrl
                };
            }
        }
    }
}
