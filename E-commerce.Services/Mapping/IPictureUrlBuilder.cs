namespace E_commerce.Services.Mapping
{
    /// <summary>
    /// Turns a stored picture path (e.g. "images/products/hoodie.jpg") into a full URL the browser can load.
    /// The database keeps only the relative path, so moving the API to another host needs no data change.
    /// </summary>
    public interface IPictureUrlBuilder
    {
        string Build(string? relativePath);
    }
}
