public class EnvHelpers
{
    private readonly ILogger<EnvHelpers> _logger;

    public EnvHelpers(ILogger<EnvHelpers> logger)
    {
        _logger = logger;
    }

    public string GetEnvironmentVariableContent(string environmentVariableName)
    {
        var result = Environment.GetEnvironmentVariable(environmentVariableName);
        if(string.IsNullOrWhiteSpace(result))
        {
            if(string.IsNullOrEmpty(result))
            {
                _logger.LogWarning($"Failed to read content of environment variable \"{environmentVariableName}\", got null/empty string");
            }
            else
            {
                _logger.LogWarning($"Failed to read proper content of environment variable \"{environmentVariableName}\", contained only whitespaces");
            }
            return string.Empty; // Consider making this an exception?
        }
        return result;
    }

    public string GetContentOfFileReferencedByEnvironmentVariableAsText(string environmentVariableName)
    {
        var location = GetEnvironmentVariableContent(environmentVariableName);
        if(!File.Exists(location))
        {
            _logger.LogError($"Failed to read content of file at path \"{location}\" specified in environment variable \"{environmentVariableName}\", file doesnt exist");
        }
        var resultingContent = File.ReadAllText(location);
        if(string.IsNullOrWhiteSpace(resultingContent))
        {
            if(string.IsNullOrEmpty(resultingContent))
            {
                _logger.LogWarning($"When reading content of file at path \"{location}\" specified in environment variable \"{environmentVariableName}\", got null/empty string");
            }
            else
            {
                _logger.LogWarning($"When reading content of file at path \"{location}\" specified in environment variable \"{environmentVariableName}\", resulting content was only whitespaces");
            }
        }
        return resultingContent;
    }
}
