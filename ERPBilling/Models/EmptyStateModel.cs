namespace ERPBilling.Models
{
    public class EmptyStateModel
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }

        public EmptyStateModel(
            string icon,
            string title,
            string subtitle,
            string? actionText = null,
            string? actionUrl = null)
        {
            Icon = icon;
            Title = title;
            Subtitle = subtitle;
            ActionText = actionText;
            ActionUrl = actionUrl;
        }
    }
}
