using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using LoggingGlobal.Data;
using LoggingGlobal.Data.Models;

namespace LoggingGlobal.Extensions
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private static readonly JsonSerializerOptions _jsonSerializerOptions =
            new(JsonSerializerDefaults.Web)
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _next = next;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            var _data = new SortedDictionary<string, object>();

            var httoLog = new HttpLog();

            var request = context.Request;
            var method = request.Method.ToUpper();

            httoLog.Path = request.Path;
            httoLog.Method = method;
            httoLog.Scheme = request.Scheme;
            httoLog.Protocol = request.Protocol;
            httoLog.Host = request.Host.ToString();
            httoLog.QueryString = request.QueryString.ToString();
            httoLog.RequestHeaders = JsonSerializer.Serialize(
                request.Headers.ToDictionary(x => x.Key, v => string.Join(";", v.Value.ToList())),
                _jsonSerializerOptions
            );
            httoLog.StartTime = DateTimeOffset.Now;

            var hasBodyMethod = new HashSet<string> { "POST", "PUT", "PATCH" };

            // 获取请求body内容
            if (
                hasBodyMethod.Contains(method)
                && request.ContentLength.HasValue
                && request.ContentLength.Value > 0
            )
            {
                request.EnableBuffering();

                if (request.HasFormContentType)
                {
                    var fileDict = new SortedDictionary<string, object>();
                    var form = await request.ReadFormAsync();
                    foreach (var item in form.Files)
                    {
                        if (string.IsNullOrWhiteSpace(item.FileName))
                            continue;
                        fileDict.Add(
                            item.Name,
                            new
                            {
                                item.FileName,
                                item.Length,
                                item.ContentType,
                                item.ContentDisposition,
                            }
                        );
                    }

                    var valueDict = new SortedDictionary<string, object>();

                    foreach (var item in form)
                    {
                        valueDict.Add(item.Key, item.Value);
                    }

                    httoLog.RequestBody = JsonSerializer.Serialize(
                        new { Files = fileDict, Values = valueDict },
                        _jsonSerializerOptions
                    );
                }
                else
                {
                    Stream stream = request.Body;
                    byte[] buffer = new byte[request.ContentLength.Value];
                    await stream.ReadAsync(buffer);
                    httoLog.RequestBody = Encoding.UTF8.GetString(buffer);
                    request.Body.Position = 0;
                }
            }

            // 获取Response.Body内容
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                httoLog.ResponseBody = await GetResponse(context.Response);
                httoLog.EndTime = DateTimeOffset.Now;

                await originalBodyStream.WriteAsync(
                    responseBody.GetBuffer(),
                    context.RequestAborted
                );
                context.Response.Body = originalBodyStream;
            }

            // 响应完成记录时间和存入日志
            context.Response.OnCompleted(() =>
            {
                _stopwatch.Stop();
                httoLog.ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
                httoLog.ResponseHeaders = JsonSerializer.Serialize(
                    context.Response.Headers.ToDictionary(
                        x => x.Key,
                        v => string.Join(";", v.Value.ToList())
                    ),
                    _jsonSerializerOptions
                );
                httoLog.StatusCode = context.Response.StatusCode;
                var json = JsonSerializer.Serialize(httoLog, _jsonSerializerOptions);

                Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var dbContext =
                        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.HttpLogs.Add(httoLog);
                    await dbContext.SaveChangesAsync(default);
                });

                _logger.LogInformation(json);
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// 获取响应内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task<string> GetResponse(HttpResponse response)
        {
            HashSet<string> TEXT_BODY_CONTENT_TYPES =
                new()
                {
                    "text/plain",
                    "text/html",
                    "application/json",
                    "application/problem+json",
                    "application/xml",
                    "application/x-www-form-urlencoded",
                    "application/javascript",
                    "application/x-javascript",
                    "text/xml",
                    "text/javascript",
                };
            // 如果是 TEXT_BODY_CONTENT_TYPES 内开头，则读取内容
            if (
                response.ContentType != null
                && TEXT_BODY_CONTENT_TYPES.Any(x => response.ContentType.StartsWith(x))
            )
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                var text = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
                return text;
            }

            return JsonSerializer.Serialize(
                new { ContentType = response.ContentType, ContentLength = response.ContentLength, },
                _jsonSerializerOptions
            );
        }
    }

    /// <summary>
    /// 扩展中间件
    /// </summary>
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
