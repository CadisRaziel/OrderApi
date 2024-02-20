namespace OrderApi.Entpoints.Categories
{
    public class CategoryRequest
    {
        //Entidade que criamos para o usuario passa-la
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}