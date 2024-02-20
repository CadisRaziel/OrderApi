namespace OrderApi.Entpoints.Categories
{
    public class CategoryResponse
    {
        //Entidade que sera retornada para o usuario que esta pedindo

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}