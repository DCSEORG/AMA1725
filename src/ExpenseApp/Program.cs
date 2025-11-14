using ExpenseApp.Data;
using ExpenseApp.Services;
using ExpenseApp.Services.GenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Expense Management API",
        Version = "v1",
        Description = "REST API for managing expenses, including submission, approval, and reporting functionality"
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Check if we should use dummy data
var useDummyData = builder.Configuration.GetValue<bool>("UseDummyData", true);

if (useDummyData)
{
    // Use in-memory dummy data service
    builder.Services.AddSingleton<IExpenseService, DummyExpenseService>();
}
else
{
    // Configure database connection (for future use)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ExpenseDbContext>(options =>
        options.UseSqlServer(connectionString));
    
    // Register database service (would need to be implemented)
    // builder.Services.AddScoped<IExpenseService, DatabaseExpenseService>();
}

// Register GenAI service if enabled
var includeChatUI = builder.Configuration.GetValue<bool>("IncludeChatUI", false);
if (includeChatUI)
{
    builder.Services.AddScoped<IGenAIExpenseService, GenAIExpenseService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Expense Management API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// Redirect root to /Index
app.MapGet("/", () => Results.Redirect("/Index"));

app.Run();
