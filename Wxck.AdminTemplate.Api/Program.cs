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
    // ����Ӧ�÷����� Kestrel ���������Ϊ50MB
    options.Limits.MaxRequestBodySize = 31457280000;
});
//���ô������ļ���`NLog` �ڵ��ȡ����
var nlogConfig = builder.Configuration.GetSection("NLog");
NLog.LogManager.Configuration = new NLogLoggingConfiguration(nlogConfig);
//���������־Providers
builder.Logging.ClearProviders();
//��С��¼�ȼ�
builder.Logging.SetMinimumLevel(LogLevel.Warning);
//����������ָ��ʹ��ASP.NET Core Ĭ�ϵ���־������
var nlogOptions = new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false };
builder.Host.UseNLog(nlogOptions); //����NLog

//ȡ��unicode
builder.Services.AddControllersWithViews().AddJsonOptions(options => {
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});

// ���ݿ����ӣ�SQL Server��
builder.Services.AddPooledDbContextFactory<SqlServerContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseSqlServer(connectionString, sqlServerOptions => {
        sqlServerOptions.CommandTimeout(60); // ���ó�ʱʱ��Ϊ 60 ��
        sqlServerOptions.UseRelationalNulls(true);
    })
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution)
        .EnableServiceProviderCaching()
        .UseLoggerFactory(LoggerFactory.Create(builder =>
            builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Error)));
}, poolSize: 300);

//�����ڴ滺��
builder.Services.AddMemoryCache();
//�ֽ�����
builder.Services.Configure<FormOptions>(options => {
    options.MultipartBodyLengthLimit = long.MaxValue;
    //������������
    //options.BufferBody = true;
});
//�쳣���������/�м�����Բ���ӣ������
builder.Services.Configure<ApiBehaviorOptions>(opt => {
    opt.InvalidModelStateResponseFactory = actionContext => {
        //��ȡ��֤ʧ�ܵ�ģ���ֶ�
        var errors = actionContext.ModelState.Where(w => w.Value?.Errors.Count > 0)?.Select(s =>
            $"[{s.Key}]:{s.Value?.Errors?.FirstOrDefault()?.ErrorMessage}")?.ToList();
        return new JsonResult(new { Result = false, Msg = $"Body:{string.Join("|", errors ?? new List<string>())}" });
    };
});
//��������
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
    //ȫ�ַ������
    //options.Filters.Add<RiskControlAttribute>();
    //���ʼ�¼
    options.Filters.Add<LogRequestResponseAttribute>();
}).AddNewtonsoftJson(options => {
    // ��ʽ������ JSON
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local; // ����ʱ��Ϊ UTC
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
//JWT��֤
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

//���вִ�ע��
builder.Services.AddRepositories();
//���оۺϸ�ע��
//���з���ע��
//Ԥ��
{
    builder.Services.AddTransient<IStartupFilter>(provider =>
        new WarmupStartupFilter(provider, builder.Services));
}
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//��̬�ļ����
app.UseStaticFiles(new StaticFileOptions() {
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),//wwwroot�൱����ʵĿ¼
    RequestPath = new PathString("/scr") //src�൱�ڱ�����Ϊ�˰�ȫ
});
app.UseExceptionHandler(config => {
    config.Run(async context => {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<IExceptionHandlerFeature>();
        {
            var ex = error?.Error;
            LogManager.GetCurrentClassLogger().Log(NLog.LogLevel.Error, $"ϵͳ�쳣:{ex}");
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { Result = false, Msg = "ϵͳ�쳣" }));
        }
    });
});
//Body����
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