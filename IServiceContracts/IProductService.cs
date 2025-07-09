namespace SupplyChain.IServiceContracts
{
    public interface IProductService
    {
        Task<int> UploadOrdersFromExcelFile(IFormFile formfile);
    }
}
