namespace MyWebApi.Domain.Common
{
    public class Select2ItemResponse
    {
        public Select2ItemResponse() { }
        public Select2ItemResponse(object id, string text, bool disabled)
        {
            Id = id;
            Text = text;
            Disabled = disabled;
        }
        public object Id { get; set; }
        public string Text { get; set; }
        public bool Disabled { get; set; }
    }
}
