using LaTiQ.Core.DTO.Response.Topic;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface ITopicService
    {
        public IEnumerable<TopicResponse> GetListTopic();
        public TopicResponse? GetTopic(Guid topicId);
    }
}
