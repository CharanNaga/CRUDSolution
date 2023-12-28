using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
    //This Filter will work even after shortcircuiting in Authorization Filter rather than ignoring like in Result Filter case
    public class PersonAlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        private readonly ILogger<PersonAlwaysRunResultFilter> _logger;

        public PersonAlwaysRunResultFilter(ILogger<PersonAlwaysRunResultFilter> logger)
        {
            _logger = logger;
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} always run filter executed",
               nameof(PersonAlwaysRunResultFilter), nameof(OnResultExecuted));
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if(context.Filters.OfType<SkipFilter>().Any())
            {
                return;
            }
            _logger.LogInformation("{FilterName}.{MethodName} always run filter executing",
                nameof(PersonAlwaysRunResultFilter), nameof(OnResultExecuting));
        }
    }
}
