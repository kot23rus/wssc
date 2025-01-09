namespace WSSC.Client.Model
{
    public class CompanyWithChild : Company
    {
        public List<ChildEntity> Childs { get; set; } = [];
    }
}
