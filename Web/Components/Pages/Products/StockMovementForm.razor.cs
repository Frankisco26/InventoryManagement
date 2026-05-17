using InventoryManagement.Data.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace InventoryManagement.Web.Components.Pages.Products
{
    public partial class StockMovementForm : ComponentBase
    {
        [Parameter]
        public string? ProductId { get; set; }

        private ProductDto? product;
        private IEnumerable<StockMovementDto>? movements;
        private CreateStockMovementDto movementModel = new();
        private bool isLoading = false;
        private bool isSaving = false;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(ProductId) && int.TryParse(ProductId, out int id))
            {
                await LoadData(id);
            }
        }

        private async Task LoadData(int id)
        {
            try
            {
                isLoading = true;
                errorMessage = null;

                // Cargar producto
                product = await ApiClient.GetProductByIdAsync(id);

                // Cargar movimientos
                if (product != null)
                {
                    movements = await ApiClient.GetMovementsAsync(id);
                }
                else
                {
                    errorMessage = "No se pudo cargar el producto.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error al cargar datos: {ex.Message}";
                Console.WriteLine($"Error: {ex}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void CancelClick()
        {
            NavigationManager.NavigateTo("/products");
        }

        private async Task HandleSubmit()
        {
            if (string.IsNullOrEmpty(ProductId) || !int.TryParse(ProductId, out int id))
            {
                errorMessage = "ID de producto inválido.";
                return;
            }

            try
            {
                isSaving = true;
                errorMessage = null;

                var result = await ApiClient.CreateMovementAsync(id, movementModel);

                if (result != null)
                {
                    // Recargar datos
                    await LoadData(id);

                    // Resetear formulario
                    movementModel = new();

                    // Mostrar mensaje de éxito
                    await JSRuntime.InvokeVoidAsync("alert", "Movimiento registrado exitosamente");
                }
                else
                {
                    errorMessage = "No se pudo registrar el movimiento.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                isSaving = false;
            }
        }
    }
}
