using NLog;
using NLog.Web;
using System.Text;
using Newtonsoft.Json;
using System.Text.Unicode;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Newtonsoft.Json.Serialization;
using Wxck.AdminTemplate.Api.Filter;
using Wxck.AdminTemplate.Api.Warmup;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Wxck.AdminTemplate.ServiceDefaults;
using Wxck.AdminTemplate.CrossCutting.Utils;
using Wxck.AdminTemplate.CrossCutting.Auth.Jwt;
using Wxck.AdminTemplate.Infrastructure.Auth.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Wxck.AdminTemplate.Infrastructure.EntityConfigurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.WebHost.UseKestrel((context, options) => {
    // 设置应用服务器 Kestrel 请求体最大为50MB
    options.Limits.MaxRequestBodySize = 31457280000;
});
//配置从配置文件的`NLog` 节点读取配置
var nlogConfig = builder.Configuration.GetSection("NLog");
NLog.LogManager.Configuration = new NLogLoggingConfiguration(nlogConfig);
//清空其他日志Providers
builder.Logging.ClearProviders();
//最小记录等级
builder.Logging.SetMinimumLevel(LogLevel.Warning);
//该配置用来指定使用ASP.NET Core 默认的日志过滤器
var nlogOptions = new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false };
builder.Host.UseNLog(nlogOptions); //启用NLog

//取消unicode
builder.Services.AddControllersWithViews().AddJsonOptions(options => {
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});

// 数据库连接（SQL Server）
builder.Services.AddPooledDbContextFactory<SqlServerContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseSqlServer(connectionString, sqlServerOptions => {
        sqlServerOptions.CommandTimeout(60); // 设置超时时间为 60 秒
        sqlServerOptions.UseRelationalNulls(true);
    })
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution)
        .EnableServiceProviderCaching()
        .UseLoggerFactory(LoggerFactory.Create(builder =>
            builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Error)));
}, poolSize: 300);

//配置内存缓存
builder.Services.AddMemoryCache();
//字节限制
builder.Services.Configure<FormOptions>(options => {
    options.MultipartBodyLengthLimit = long.MaxValue;
    //缓存请求正文
    //options.BufferBody = true;
});
//异常处理过滤器/中间件可以不添加，看情况
builder.Services.Configure<ApiBehaviorOptions>(opt => {
    opt.InvalidModelStateResponseFactory = actionContext => {
        //获取验证失败的模型字段
        var errors = actionContext.ModelState.Where(w => w.Value?.Errors.Count > 0)?.Select(s =>
            $"[{s.Key}]:{s.Value?.Errors?.FirstOrDefault()?.ErrorMessage}")?.ToList();
        return new JsonResult(new { Result = false, Msg = $"Body:{string.Join("|", errors ?? new List<string>())}" });
    };
});
//跨域设置
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    policyBuilder => {
        policyBuilder.AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true) // =AllowAnyOrigin()
            .AllowCredentials();
        options.AddPolicy("SignalR",
            corsPolicyBuilder => {
                corsPolicyBuilder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(str => true)
                    .AllowCredentials();
            });
    }));

builder.Services.AddControllers(options => {
    //全局风控拦截
    //options.Filters.Add<RiskControlAttribute>();
    //访问记录
    options.Filters.Add<LogRequestResponseAttribute>();
}).AddNewtonsoftJson(options => {
    // 格式化返回 JSON
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local; // 设置时区为 UTC
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    options.SerializerSettings.Formatting = Formatting.Indented;
});

//SignalR
builder.Services.AddSignalR(options => {
    options.HandshakeTimeout = TimeSpan.FromMinutes(1);
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = null;
    options.KeepAliveInterval = TimeSpan.FromMinutes(1);
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
    options.MaximumParallelInvocationsPerClient = 10;
    options.StreamBufferCapacity = int.MaxValue;
});
//http
builder.Services.AddHttpClient("INSURANCE", httpClient => {
    // httpClient.Timeout = TimeSpan.FromSeconds(10);
}).ConfigurePrimaryHttpMessageHandler(() => {
    var handler = new HttpClientHandler() {
        UseDefaultCredentials = true,
        MaxConnectionsPerServer = 600,
        ServerCertificateCustomValidationCallback = (m, c, ch, _) => true,
        //UseProxy = false
    };

    return handler;
});
//JWT验证
builder.Services.Configure<TokenManagement>(builder.Configuration.GetSection("tokenConfig"));
var token = builder.Configuration.GetSection("tokenConfig").Get<TokenManagement>();
builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token?.Secret ?? string.Empty)),
        ValidIssuer = token?.Issuer,
        ValidAudience = token?.Audience,
        ValidateIssuer = false,
        ValidateAudience = false,
    };
    x.Events = new JwtBearerEvents {
        OnAuthenticationFailed = context => {
            var replace = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenBlackListService =
                context.HttpContext.RequestServices.GetRequiredService<IAuthenticateService>();
            if (tokenBlackListService.IsTokenBlacklisted(replace)) {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                context.Response.WriteAsync("Token has been blacklisted").Wait();
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddSingleton<IAuthenticateService, TokenIAuthenticateService>();

//集中仓储注入
builder.Services.AddRepositories();
//集中聚合根注入
//集中服务注入
//预热
{
    builder.Services.AddTransient<IStartupFilter>(provider =>
        new WarmupStartupFilter(provider, builder.Services));
}
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//静态文件别称
app.UseStaticFiles(new StaticFileOptions() {
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),//wwwroot相当于真实目录
    RequestPath = new PathString("/scr") //src相当于别名，为了安全
});
app.UseExceptionHandler(config => {
    config.Run(async context => {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<IExceptionHandlerFeature>();
        {
            var ex = error?.Error;
            LogManager.GetCurrentClassLogger().Log(NLog.LogLevel.Error, $"系统异常:{ex}");
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { Result = false, Msg = "系统异常" }));
        }
    });
});
//Body重用
app.Use(next => async context => {
    context.Request.EnableBuffering();
    await next.Invoke(context);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

HttpContextHelper.HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

//MessageHub
app.Run();