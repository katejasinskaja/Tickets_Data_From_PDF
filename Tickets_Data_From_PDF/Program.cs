const string TicketsFolder = @"C:\Users\mi\source\repos\Tickets_Data_From_PDF\Tickets";
var app = new Application(TicketsFolder, new FileWriter(), new DocumentsFromPdfsReader());
app.Run();
