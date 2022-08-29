public class Listing {
    string? data_type { get; set; }
    string? data_subreddit { get; set; } 
    string? data_author { get; set; }
    string? data_permalink { get; set; }
    DateTime datetime {get; set;}

    public Listing(string t, string s, string a, string p, DateTime d) {
        this.data_type = t;
        this.data_subreddit = s;
        this.data_author = a;
        this.data_permalink = p;
        this.datetime = d;
    }
}