namespace BffMicrosoftEntraID.Server;

/// <summary>
/// This exception class is used to pass HTTP CAE unauthorized responses from a Httpclient and 
/// return the WWW-Authenticate header with the required claims challenge. 
/// This is only required if using a downstream API
/// </summary>
public class WebApiMsalUiRequiredException : Exception
{
    private readonly HttpResponseMessage _httpResponseMessage;

    public WebApiMsalUiRequiredException(string message, HttpResponseMessage httpResponseMessage) : base(message)
    {
        _httpResponseMessage = httpResponseMessage;
    }

    public HttpStatusCode StatusCode
    {
        get { return _httpResponseMessage.StatusCode; }
    }

    public HttpResponseHeaders Headers
    {
        get { return _httpResponseMessage.Headers; }
    }

    public HttpResponseMessage HttpResponseMessage
    {
        get { return _httpResponseMessage; }
    }
}