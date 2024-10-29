using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LaTiQ.Core.DTO.Request.Room
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CameraStatus
    {
        [EnumMember(Value = "On")]
        On,

        [EnumMember(Value = "Off")]
        Off,
    }
    
    public class UserRoom
    {
        public string UserEmail { get; set; } = string.Empty;
        
        public string UserAvatar { get; set; } = string.Empty;
        
        public CameraStatus CameraStatus { get; set; }
        
        public string RoomId { get; set; } = string.Empty;
    }
}
