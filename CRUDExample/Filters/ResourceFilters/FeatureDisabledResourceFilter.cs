using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResourceFilters
{
    public class FeatureDisabledResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisabledResourceFilter> _logger;
        private readonly bool _isDisabled;

        public FeatureDisabledResourceFilter(ILogger<FeatureDisabledResourceFilter> logger, bool isDisabled = true)
        {
            _logger = logger;
            _isDisabled = isDisabled;

        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //before execution
            _logger.LogInformation("{FilterName}.{MethodName} - before execution",nameof(FeatureDisabledResourceFilter),nameof(OnResourceExecutionAsync));
            if(_isDisabled)
                context.Result = new StatusCodeResult(501);
            else
                await next();

            //after execution
            _logger.LogInformation("{FilterName}.{MethodName} - after execution", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));
        }
    }
}