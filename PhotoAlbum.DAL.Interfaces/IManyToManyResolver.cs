namespace PhotoAlbum.DAL.Interfaces
{
    public interface IManyToManyResolver
    {
        void Update(int id, int[] selectedItems);
    }
}
