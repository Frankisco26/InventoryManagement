using InventoryManagement.Data.Dtos;
using InventoryManagement.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InventoryManagement.Web.Components.Pages.Products
{
    public partial class ProductForm : ComponentBase
    {
        [Parameter]
        public string? Id { get; set; }
        private CreateProductDto FormModel = new();
        private bool IsLoading = false;
        private bool IsSaving = false;
        private string? ErrorMessage;

        private bool IsEditMode => !string.IsNullOrEmpty(Id);
        private bool IsViewMode = false;
        protected override async Task OnInitializedAsync()
        {
            string currentUrl = NavigationManager.Uri;

            if (currentUrl.Contains("view", StringComparison.OrdinalIgnoreCase) && int.TryParse(Id, out int productViewId))
            {
                await LoadProduct(productViewId);
                IsViewMode = true;
            }

            if (IsEditMode && int.TryParse(Id, out int productId))
            {
                await LoadProduct(productId);
            }
        }

        protected async Task LoadProduct(int id)
        {
            try
            {
                IsLoading = true;
                var product = await ApiClient.GetProductByIdAsync(id);

                if (product != null)
                {
                    FormModel = new CreateProductDto
                    {
                        Name = product.Name,
                        SKU = product.SKU,
                        Category = product.Category,
                        UnitPrice = product.UnitPrice,
                        QuantityInStock = product.QuantityInStock,
                        MinimumStock = product.MinimumStock
                    };
                }
                else
                {
                    ErrorMessage = "Producto no encontrado.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void CancelClick()
        {
            NavigationManager.NavigateTo("/products");
        }

        public async Task HandleSubmit()
        {
            try
            {
                IsSaving = true;
                ErrorMessage = null;

                ProductDto? result = null;

                if (IsEditMode && int.TryParse(Id, out int productId))
                {
                    var updateDto = new UpdateProductDto
                    {
                        Name = FormModel.Name,
                        Category = FormModel.Category,
                        UnitPrice = FormModel.UnitPrice,
                        MinimumStock = FormModel.MinimumStock
                    };

                    result = await ApiClient.UpdateProductAsync(productId, updateDto);
                }
                else
                {
                    result = await ApiClient.CreateProductAsync(FormModel);
                }

                if (result != null)
                {
                    NavigationManager.NavigateTo("/products");
                }
                else
                {
                    ErrorMessage = "No se pudo guardar el producto.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}
