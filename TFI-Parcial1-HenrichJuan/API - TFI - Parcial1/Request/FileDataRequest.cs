namespace API___TFI___Parcial1.Response
{
    public class FileDataRequest
    {
        public string? Nombre { get; set; }
        public byte[]? Data { get; set; }
        public string? FileType { get; set; }
        public long? Size { get; set; }
        public int? Prioridad { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public DateTime? FechaImpresion { get; set; }
        public string? Url { get; set; }
    }
}
