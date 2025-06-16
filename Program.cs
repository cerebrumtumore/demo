var builder = WebApplication.CreateBuilder();
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

List<Order> repo = [
    new Order(1, new DateTime(2000,05,05), "машина", "колесо спустило", "вообще не едет","ваня", "в ожидании")
];

bool isUpdateStatus = false;
string message = "";

app.MapGet("/", () => {
    if (isUpdateStatus)
    {
        string buffer = message;
        isUpdateStatus = false;
        message = "";
        return Results.Json(new OrderUpdateStatusDTO(repo, buffer));
    }
    else
        return Results.Json(repo);
});
app.MapPost("/", (CreateOrderDTO dto) => {
    Order o = new Order(dto.Number, dto.DateTime, dto.Device, dto.Problemtype, dto.Description, dto.Client, dto.Status);
    repo.Add(o);
    });
app.MapPut("/{number}", (int number, OrderUpdateDTO dto) =>
{
    Order buffer = repo.Find(o => o.Number == number);

    if (buffer == null)
        return Results.StatusCode(404);

    if (buffer.Description != dto.Description)
        buffer.Description = dto.Description;
    if (buffer.Master != dto.Master)
        buffer.Master = dto.Master;
    if (buffer.Status != dto.Status)
    {
        buffer.Status = dto.Status;
        isUpdateStatus = true;
        message += "статус заявки номер" + buffer.Number + "изменен/n";
    }
    if (dto.comment != null || dto.comment != "")
        buffer.Comments.Add(dto.comment);
        return Results.Json(buffer);
   
});
app.MapGet("/{num}", (int num) => repo.Find(o => o.Number == num));
app.MapGet("/filter/{param}", (string param) => repo.FindAll(o =>
o.Device == param ||
o.Problemtype == param ||
o.Description == param ||
o.Client == param ||
o.Status == param ||
o.Master == param));


app.Run();

record class OrderUpdateDTO(string Status, string Description, string Master, string comment);
record class OrderUpdateStatusDTO(List<Order> repo, string massage);
record class CreateOrderDTO(int Number, DateTime DateTime, string Device, string Problemtype, string Description, string Client, string Status);
class Order(int number, DateTime dateTime, string device, string problemtype, string description, string client, string status)
{
    public int Number { get; set; } = number;
    public DateTime DateTime { get; set; } = dateTime;
    public string Device { get; set; } = device;
    public string Problemtype { get; set; } = problemtype;
    public string Description { get; set; } = description;
    public string Client { get; set; } = client;
    public string Status { get; set; } = status;
    public string Master { get; set; } = "не назначен";

    public List<string> Comments { get; set; } = [];
}