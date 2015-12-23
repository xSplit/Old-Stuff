using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
 
namespace Tester
{
    /// <summary>
    /// Simple class for handle a web request
    /// </summary>
    public class WRequest
    {
  /// <summary>
  /// Main request
  /// </summary>
  public HttpWebRequest Request;
 
  /// <summary>
  /// Request url
  /// </summary>
  public Uri Url;
 
  /// <summary>
  /// Request method
  /// </summary>
  public string Method;
 
  /// <summary>
  /// Request data (key => value order)
  /// </summary>
  public Dictionary<string, string> Parms;
 
  /// <summary>
  /// Files to send
  /// </summary>
  public List<WFile> Files = new List<WFile>();
 
  /// <summary>
  /// Cookies storage
  /// </summary>
  public CookieContainer Cookies;
 
  /// <summary>
  /// Create a new WRequest
  /// </summary>
  /// <param name="url">Website of the request</param>
  /// <param name="method">Method for send data</param>
  /// <param name="parms">Ordered data to send</param>
  /// <param name="files">Loaded files to send</param>
  /// <param name="use_encode">Encode data for a secure transmission</param>
  /// <param name="cookies">CookieContainer for store cookies</param>
  public WRequest(string url, string method, Dictionary<string, string> parms = null, WFile[] files = null, bool use_encode = false, CookieContainer cookies = null)
  {
    try
    {
    if (use_encode)
    Parms = parms.Select(x => new KeyValuePair<string, string>(HttpUtility.UrlEncode(x.Key), HttpUtility.UrlEncode(x.Value))).ToDictionary(x => x.Key, x => x.Value);
    else
    Parms = parms;
 
    if (new[] { "POST", "GET" }.Contains(method.ToUpper()))
    Method = method.ToUpper();
    else
    throw new Exception("Invalid method, allowed only POST and GET");
 
    if (url.Contains("?"))
    url = url.Split('?')[0];
 
    Url = Method == "GET" ? new Uri(url + getParams()) : new Uri(url);
 
    Cookies = cookies;
 
    if (files != null)
    Files = files.ToList();
 
    prepareRequest();
    }
    catch (UriFormatException)
    {
    Console.WriteLine("Invalid url {0}", url);
    }
    catch (Exception e)
    {
    Console.WriteLine(e.Message);
    }
  }
 
  private void prepareRequest()
  {
    if (Url.ToString().Contains("https")) System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };
    Request = (HttpWebRequest)WebRequest.Create(Url);
    Request.Method = Method;
    if (Cookies != null)
    {
    Request.CookieContainer = Cookies;
    Request.CookieContainer.Add(new Cookie("__utma", "cookies") { Domain = Request.Host });
    }
    Request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
  }
 
  /// <summary>
  /// Send the request and get the response with WResponse
  /// </summary>
  /// <returns>The response as a new WResponse instance</returns>
  public WResponse getResponse()
  {
    try
    {
    if (Method == "POST")
    {
    Stream write = Request.GetRequestStream();
    string parmstr = getParams();
    string boundary = "Boiadeh";
 
    if (parmstr.StartsWith("?"))
    parmstr = parmstr.Remove(0, 1);
 
    if (Files.Count() > 0)
    {
    Request.ContentType = "multipart/form-data; boundary=" + boundary;
 
    if (parmstr.Length > 0)
    {
    foreach (string field in parmstr.Split('&'))
    {
    byte[] tosend = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"" + field.Split('=')[0].Replace("\"", "") + "\"\r\n\r\n" + field.Split('=')[1]);
    write.Write(tosend, 0, tosend.Length);
    }
    }
 
    foreach (WFile file in Files)
    {
    byte[] divide = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"" + file.input_name.Replace("\"", "") + "\"; filename=\"" + file.filename + "\"\nContent-Type: " + file.type + "\r\n\r\n");
 
    write.Write(divide, 0, divide.Length);
    write.Write(file.getData(), 0, file.getData().Length);
    }
 
    write.Write(Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n\r\n"), 0, Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n\r\n").Length);
    }
    else
    {
    Request.ContentType = "application/x-www-form-urlencoded";
    write.Write(Encoding.ASCII.GetBytes(parmstr), 0, parmstr.Length);
    }
 
    write.Close();
    }
 
    return new WResponse(this);
    }
    catch (Exception e)
    {
    Console.WriteLine(e.Message);
    }
 
    return null;
  }
 
  /// <summary>
  /// Create a data string to send with dictionary keys
  /// </summary>
  /// <returns>Data string in url format</returns>
  public string getParams()
  {
    if (Parms == null)
    return "";
 
    var parmstr = new StringBuilder("?");
 
    foreach (KeyValuePair<string, string> s in Parms)
    parmstr.Append(s.Key + "=" + s.Value + "&");
 
    return parmstr.ToString().Remove(parmstr.Length - 1);
  }
 
  /// <summary>
  /// A loaded file, ready to be sent
  /// </summary>
  public struct WFile
  {
    private byte[] data;
 
    /// <summary>
    /// File name
    /// </summary>
    public string filename;
 
    /// <summary>
    /// Input name for the request
    /// </summary>
    public string input_name;
 
    /// <summary>
    /// File mimetype
    /// </summary>
    public string type;
 
    /// <summary>
    /// A new loaded file
    /// </summary>
    /// <param name="data">File data as byte[]</param>
    /// <param name="filename">Filename</param>
    /// <param name="input_name">Input name(in the form)</param>
    /// <param name="type">File mimetype</param>
    public WFile(byte[] data, string filename, string input_name, string type)
    {
    this.filename = filename;
    this.input_name = input_name;
    this.type = type;
    this.data = data;
    }
 
    /// <summary>
    /// Get the file data as byte[]
    /// </summary>
    /// <returns>File data as byte[]</returns>
    public byte[] getData()
    {
    return data;
    }
  }
 
  /// <summary>
  /// Load a file to send
  /// </summary>
  /// <param name="file">File path</param>
  /// <param name="input_name">Input name(in the form)</param>
  /// <param name="type">File mimetype</param>
  /// <returns>WFile instance</returns>
  public static WFile LoadFile(string file, string input_name, string type)
  {
    if (File.Exists(file))
    return new WFile(File.ReadAllBytes(file), Path.GetFileName(file), input_name, type);    //fuck this shit for large files
    else
    throw new Exception("The file " + file + " doesn't exist");
  }
    }
 
    /// <summary>
    /// Simple class for handle a web response
    /// </summary>
    public class WResponse
    {
  /// <summary>
  /// Web response instantiated by the original request on class construct
  /// </summary>
  public HttpWebResponse Response;
 
  /// <summary>
  /// Original web request
  /// </summary>
  public WRequest OrigRequest;
 
  private List<byte[]> Data = new List<byte[]>() { new byte[1024] };
 
  /// <summary>
  /// Create a new WResponse passing a WRequest, it's better using .getResponse() of WRequest rather than directly instantiated
  /// </summary>
  /// <param name="r">Original WRequest</param>
  public WResponse(WRequest r)
  {
    OrigRequest = r;
    Response = (HttpWebResponse)r.Request.GetResponse();
  }
 
  /// <summary>
  /// Get response data as a list of byte[] (every byte[] have a 1kb length)
  /// </summary>
  /// <returns>Response data as a list of byte[]</returns>
  public List<byte[]> getData()
  {
    try
    {
    Stream resp = Response.GetResponseStream();
    int read = 0, index = 0;
    while ((read = resp.Read(Data[index], 0, 1024)) > 0)
    {
    index++;
    Data.Add(new byte[1024]);
    }
 
    Data = Data.Select(x => x.Where(s => s > 0x00).ToArray()).ToList();
 
    resp.Close();
    }
    catch (Exception e)
    {
    Console.WriteLine(e.Message);
    }
 
    return Data;
  }
 
  /// <summary>
  /// Get response data as a string in a custom encoding
  /// </summary>
  /// <param name="encoding">String encoding for byte[]</param>
  /// <returns>Response data as a string</returns>
  public string getData(string encoding)
  {
    try
    {
    var ret = new StringBuilder();
    foreach (byte[] b in getData())
    ret.Append(Encoding.GetEncoding(encoding).GetString(b));
    return ret.ToString();
    }
    catch (ArgumentException)
    {
    Console.WriteLine("Invalid encoding {0}", encoding);
    }
 
    return null;
  }
 
  /// <summary>
  /// Get cookies of the request by an url
  /// </summary>
  /// <param name="url">Cookies url</param>
  /// <returns>Cookies names and values as a dictionary object by a specific url</returns>
  public Dictionary<string, string> getCookies(string url = null)
  {
    var d = new Dictionary<string, string>();
 
    try
    {
    if (OrigRequest.Cookies == null)
    throw new Exception("CookieContainer not used in this request, can't get cookies");
 
    if (url == null)
    url = OrigRequest.Url.OriginalString;
 
    foreach (Cookie c in OrigRequest.Cookies.GetCookies(new Uri(url)))
    d.Add(c.Name, c.Value);
    }
    catch (UriFormatException)
    {
    Console.WriteLine("Invalid url {0}", url);
    }
    catch (Exception e)
    {
    Console.WriteLine(e.Message);
    }
 
    return d;
  }
    }
 
    /// <summary>
    /// General utils class
    /// </summary>
    public static class WUtils
    {
  /// <summary>
  /// Array of followed requests
  /// </summary>
  public static WResponse[] Responses;
 
  /// <summary>
  /// Prepare and send an array of WRequest
  /// </summary>
  /// <param name="act">Action to execute every request</param>
  /// <param name="usecookies">Use cookies trasmission by the first request</param>
  /// <param name="reqs">WRequests to send</param>
  public static void FollowRequests(Action<WResponse> act, bool usecookies, params WRequest[] reqs)
  {
    Responses = new WResponse[reqs.Length];
 
    for (int i = 0; i < reqs.Length; i++)
    {
    Responses[i] = reqs[i].getResponse();
 
    if (act != null)
    act(Responses[i]);
 
    if (reqs[i].Cookies != null && usecookies && i != reqs.Length - 1)
    reqs[i + 1].Cookies = Responses[i].OrigRequest.Cookies;
    }
  }
 
  /// <summary>
  /// Extension method that transforms an encoded url string into a dictionary (key => value order)
  /// </summary>
  /// <param name="st">Encoded url string</param>
  /// <returns>Dictionary order of keys and values</returns>
  public static Dictionary<string, string> getParamsFromEncodedUrlString(this string st)
  {
    var d = new Dictionary<string, string>();
 
    try
    {
    foreach (string v in st.Split('&'))
    d[HttpUtility.UrlDecode(v.Split('=')[0])] = HttpUtility.UrlDecode(v.Split('=')[1]);
    }
    catch (IndexOutOfRangeException)
    {
    Console.WriteLine("Invalid format {0}", st);
    }
 
    return d;
  }
 
  /// <summary>
  /// Extension method that transforms a url encoded string into a dictionary (key => value order)
  /// </summary>
  /// <param name="st">Url String</param>
  /// <returns>Dictionary order of keys and values</returns>
  public static Dictionary<string, string> getParamsFromUrlString(this string st)
  {
    var d = new Dictionary<string, string>();
 
    try
    {
    foreach (string v in st.Split('&'))
    d[v.Split('=')[0]] = v.Split('=')[1];
    }
    catch (IndexOutOfRangeException)
    {
    Console.WriteLine("Invalid format {0}", st);
    }
 
    return d;
  }
    }
}
