
namespace Site
{
    public class HomeView : ViewModelBase
    {
        public HomeView()
        {
            Title = "Hello World";
        }

        public string Title { get; set; }
    };
}
