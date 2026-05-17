using InventoryManagement.Data.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace InventoryManagement.Web.Components.Pages.Products
{
    public partial class ProductList : ComponentBase
    {
        private IEnumerable<ProductDto>? products;
        private bool isLoading = true;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                isLoading = true;
                errorMessage = null;
                products = await ApiClient.GetProductsAsync();

                if (products == null)
                {
                    errorMessage = "No se pudieron cargar los productos. Por favor, intenta de nuevo.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error al cargar productos: {ex.Message}";
                Console.WriteLine($"Error: {ex}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ViewProduct(int id)
        {
            NavigationManager.NavigateTo($"/products/{id}/view");
        }

        private void EditProduct(int id)
        {
            NavigationManager.NavigateTo($"/products/{id}/edit");
        }

        private async Task DeleteProduct(int id)
        {
            if (await ConfirmAsync("¿Estás seguro de que deseas eliminar este producto?"))
            {
                try
                {
                    var success = await ApiClient.DeleteProductAsync(id);
                    if (success)
                    {
                        errorMessage = null;
                        await LoadProducts();
                    }
                    else
                    {
                        errorMessage = "No se pudo eliminar el producto.";
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = $"Error al eliminar: {ex.Message}";
                }
            }
        }

        private async Task<bool> ConfirmAsync(string message)
        {
            return await JSRuntime.InvokeAsync<bool>("confirm", message);
        }
    }
}
