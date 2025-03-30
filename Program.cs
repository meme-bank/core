using OctopusAPI.Database;
using GraphQL.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeduzaContext>();
builder.Services.AddControllers();
builder.Services.AddGraphQL(b => b.AddAutoSchema<MeduzaSchema>().AddSystemTextJson().AddDataLoader());

var app = builder.Build();

app.UseGraphQLAltair();
app.UseHttpsRedirection();

app.Run();