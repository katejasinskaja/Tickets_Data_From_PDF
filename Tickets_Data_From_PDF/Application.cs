using System.Text;
using System.Globalization;

public class Application
{

    public string _ticketsFolder;
    private readonly Dictionary<string, CultureInfo> _domainToCultureMapping = new()
    {
        [".com"] = new CultureInfo("en-US"),
        [".fr"] = new CultureInfo("fr-FR"),
        [".jp"] = new CultureInfo("ja-JP")

    };

    private readonly IFileWriter
        _writer;
    private readonly IDocumentReader _documentReader;


    public Application(string ticketsFolder, IFileWriter writer, IDocumentReader documentReader)
    {
        _ticketsFolder = ticketsFolder;
        _writer = writer;
        _documentReader = documentReader;
    }
    public void Run()
    {

        var stringBuilder = new StringBuilder();

        var ticketDocuments = _documentReader.Read(_ticketsFolder);

        foreach (var page in ticketDocuments)
        {

            var lines = ProcessPage(page);
            stringBuilder.AppendLine(
                string.Join(Environment.NewLine, lines));
        }

        _writer.Write(stringBuilder.ToString(), _ticketsFolder, "aggregatedTickets.txt");
    }

    

    private IEnumerable<string> ProcessPage(string page)
    {
        
        var split = page.Split(new[] { "Title:", "Date:", "Time:", "Visit us:" },
            StringSplitOptions.None);

        var domain = split.Last().ExtractDomain();
        var ticketCulture = _domainToCultureMapping[domain];
        for (int i = 1; i < split.Length - 3; i += 3)
        {
            string ticketData = BuildTicketData(split, ticketCulture, i);
            yield return ticketData;
        }
    }

    private static string BuildTicketData(string[] split, CultureInfo ticketCulture, int i)
    {
        var title = split[i];
        var dateAsString = split[i + 1];
        var timeAsString = split[i + 2];

        var date = DateOnly.Parse(
            dateAsString, ticketCulture);
        var time = TimeOnly.Parse(
            timeAsString, ticketCulture);
        var dateAsStringInvariant = date
            .ToString(CultureInfo.InvariantCulture);
        var timeAsStringInvariant = time.ToString(CultureInfo.InvariantCulture);

        var ticketData = $"{title,-40}|{dateAsStringInvariant}|{timeAsStringInvariant}";
        return ticketData;
    }

   
}
