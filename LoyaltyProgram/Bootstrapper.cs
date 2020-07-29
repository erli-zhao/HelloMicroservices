using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System;

namespace LoyaltyProgram
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration =>
            NancyInternalConfiguration
            .WithOverrides(builder => builder.StatusCodeHandlers.Clear());

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        { 

            pipelines.OnError += (ctx, ex) =>
              {
                  Log("Unhandled", ex);
                  return null;
              };
        }

        private void Log(string message, Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
