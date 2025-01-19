using LaTiQ.Core.DTOs.Topic.Res;
using LaTiQ.Core.Entities;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface ITopicService
    {
        public IEnumerable<TopicResponse> GetListTopic();
        public Topic GetTopic(Guid topicId);
    }
}
