using System.Text.Json.Serialization;

namespace Busines_Layer.Dtos.Postex;

public class BoxDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("box_type_name")]
    public string Name { get; set; }   
}