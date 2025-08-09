public interface ISoftDeleteService
{
    Task<bool> SoftDeleteCategoryAsync(int categoryId);
    Task<bool> SoftDeleteTestAsync(int testId);
}