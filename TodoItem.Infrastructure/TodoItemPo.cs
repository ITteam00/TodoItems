using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoItem.Infrastructure;

public class TodoItemPo{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Description { get; set; }
    public bool Done { get; set; }
    public bool Favorite { get; set; }
    public DateTime CreatedTimeDate { get; set; }
    public DateTime LastModifiedTimeDate { get; set; }
    public int EditTimes { get; set; }
    public DateTime? DueDate { get; set; }
}

