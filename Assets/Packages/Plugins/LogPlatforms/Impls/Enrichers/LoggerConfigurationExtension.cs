using Serilog;
using Serilog.Configuration;

namespace Cr7Sund.Logger
{
    public static class LogEnrichExtension
    {
        /// <summary>
        /// Enriches log message wit color  .
        /// </summary>
        /// <param name="enrichmentConfiguration">The logger enrichment configuration (`Enrich`).</param>
        /// <returns>Logger configuration to allow method chaining.</returns>
        public static LoggerConfiguration WithColorLog(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With(new UnityColorEnricher());
        }
        /// <summary>
        /// Enriches log message wit log channel .
        /// </summary>
        /// <param name="enrichmentConfiguration">The logger enrichment configuration (`Enrich`).</param>
        /// <returns>Logger configuration to allow method chaining.</returns>
        public static LoggerConfiguration WithLogChannel(this LoggerEnrichmentConfiguration enrichmentConfiguration, string logChannel)
        {
            return enrichmentConfiguration.With(new LogChannelEnricher(logChannel));
        }
    }
}