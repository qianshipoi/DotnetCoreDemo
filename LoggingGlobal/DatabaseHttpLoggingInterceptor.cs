using System.Text;
using System.Text.Json;
using LoggingGlobal.Data;
using LoggingGlobal.Data.Models;
using Microsoft.AspNetCore.HttpLogging;

namespace LoggingGlobal
{
    public class DatabaseHttpLoggingInterceptor : IHttpLoggingInterceptor, IDisposable
    {
        private readonly ApplicationDbContext _dbContext;

        public DatabaseHttpLoggingInterceptor(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        private HttpLog? _httpLog;
        private Stream? _originBodyStream;


        public async ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
        {
            var request = logContext.HttpContext.Request;
            var cancellationToken = logContext.HttpContext.RequestAborted;
            var httpLog = new HttpLog();

            httpLog.Path = request.Path;
            httpLog.Method = request.Method;
            httpLog.Protocol = request.Protocol;
            httpLog.Host = request.Host.ToString();
            httpLog.QueryString = request.QueryString.ToString();
            httpLog.RequestHeaders = JsonSerializer.Serialize(logContext.Parameters);
            var method = request.Method.ToUpper();

            if (
                (method == "POST" || method == "PUT" || method == "PATCH")
                && request.ContentLength.HasValue
                && request.ContentLength.Value > 0
            )
            {
                // Read the request body
                if (request.HasFormContentType)
                {
                    httpLog.RequestBody = JsonSerializer.Serialize(request.Form);
                }
                else
                {
                    request.EnableBuffering();
                    Stream stream = request.Body;
                    byte[] buffer = new byte[request.ContentLength.Value];
                    await stream.ReadAsync(buffer, cancellationToken);
                    httpLog.RequestBody = Encoding.UTF8.GetString(buffer);
                    request.Body.Position = 0;
                }
            }

            _originBodyStream = logContext.HttpContext.Response.Body;

            logContext.HttpContext.Response.Body = new MemoryStream();

            _httpLog = httpLog;
        }

        public async ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext)
        {
            if (_httpLog is null) return;

            var response = logContext.HttpContext.Response;
            var cancellationToken = logContext.HttpContext.RequestAborted;

            _httpLog.StatusCode = response.StatusCode;
            _httpLog.ResponseHeaders = JsonSerializer.Serialize(logContext.Parameters);

            _httpLog.RequestBody = await GetResponse(logContext.HttpContext.Response);
            var responseBody = logContext.HttpContext.Response.Body;    
            await responseBody.CopyToAsync(_originBodyStream!);

        }

        /// <summary>
        /// 获取响应内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task<string> GetResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }

        public void Dispose()
        {
            _originBodyStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
