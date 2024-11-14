namespace GestionComercioIOON.IServices
{
    public interface IAuxRepository<T>
    {
        public string UpdateCreateObject(T obj);
        public T GetObjectById(object ID);
        public List<T> GetAllObjects(int offSet, int pageSize);
    }
}
