namespace Common.Config;

public class ApiConfig
{
    public string Secret { get; set; }
    public string Authority { get; set; } // url of the identity provider
    public string Issuer { get; set; }
    public string Audience { get; set; }  // used open id connect 
    public string MetadataAddress { get; set; }
    public string ValidAudiences { get; set; }
    public string ValidIssuers { get; set; }
}