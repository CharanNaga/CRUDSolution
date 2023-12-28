using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private string Key { get; set; }
        private string Value { get; set; }
        private int Order { get; set; }

        public ResponseHeaderFilterFactoryAttribute(string key, string value,int order)
        {
            Key = key;
            Value = value;
            Order = order;
        }
        //controller invokes FilterFactory, FilterFactory invokes ActionFilter class
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            //var filter= new ResponseHeaderActionFilter();
            var filter= serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = Key;
            filter.Value = Value;
            filter.Order = Order;

            //return filter object
            return filter;
        }
    }

    //Parameterized Action Filter as it receives args key & value
    public class ResponseHeaderActionFilter : IAsyncActionFilter,IOrderedFilter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }

        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before logic
            _logger.LogInformation("Before logic in ResponseHeaderActionFilter");
            await next(); //calls subsequent filter or action method

            //after logic
            _logger.LogInformation("After logic in ResponseHeaderActionFilter");
            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}
