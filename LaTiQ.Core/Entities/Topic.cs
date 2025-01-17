namespace LaTiQ.Core.Entities
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> Words { get; set; } = new List<string>();

        // Private constructor to enforce the use of the Builder
        private Topic() { }

        // Nested Builder class
        public class Builder
        {
            private readonly Topic _topic;

            public Builder()
            {
                _topic = new Topic();
            }

            public Builder SetId(Guid id)
            {
                _topic.Id = id;
                return this;
            }

            public Builder SetName(string name)
            {
                _topic.Name = name;
                return this;
            }

            public Builder SetImageUrl(string imageUrl)
            {
                _topic.ImageUrl = imageUrl;
                return this;
            }

            public Builder SetWords(List<string> words)
            {
                _topic.Words = words;
                return this;
            }

            public Builder AddWord(string word)
            {
                _topic.Words.Add(word);
                return this;
            }

            public Topic Build()
            {
                return _topic;
            }
        }
    }

}
