using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoItem.Infrastructure;

public class TodoItemPo{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Description { get; set; }
    public bool IsComplete { get; set; }
}