﻿namespace CoreServer.Datas
{
    public class message
    {
        public DateTime times { get; }
        public string text { get; }
        public CategoryType category { get; }

        public message(DateTime times, string text, CategoryType category)
        {
            this.times = times;
            this.text = text;
            this.category = category;
        }
    }

    public enum CategoryType
    {
        Information,
        Warning,
        Error,
        Critical,
    }
}
