using LaTiQ.Core.DTOs.Topic.Res;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface ITopicService
    {
        public IEnumerable<TopicResponse> GetListTopic();
        public TopicResponse GetTopic(Guid topicId);
    }
}
