using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private VisualTreeAsset productEntryUIAsset;
    [SerializeField] private UIDocument uiDocument;
    [Space]
    [SerializeField] private Product[] allProducts;

    private VisualElement titleScreen;
    private VisualElement gameHUD;
    private VisualElement productLibraryScreen;

    private void Start()
    {
        titleScreen = uiDocument.rootVisualElement.Q("TitleScreen");
        gameHUD = uiDocument.rootVisualElement.Q("HUD");
        productLibraryScreen = uiDocument.rootVisualElement.Q("ProductLibraryScreen");

        // Add UI elements for each product, and associated ProductDefinition
        CreateProductEntries();
        // Subscribe to pickup event from player controller
        playerController.OnProductSelected += OnProductPickedUp;

        // Show the title screen on game start
        titleScreen.style.display = DisplayStyle.Flex;

        // Add click event listener to the "Start" button
        Button startButton = titleScreen.Q<Button>("StartButton");
        startButton.clicked += OnStartButtonClicked;

        // Add click event listener to the "Add Product" button
        Button addProductButton = gameHUD.Q<Button>("AddProductButton");
        addProductButton.clicked += OnAddProductButtonClicked;

        // Add click event listener to the "Add To Scene" button
        Button addToSceneButton = productLibraryScreen.Q<Button>("AddProductToSceneButton");
        addToSceneButton.clicked += OnAddToSceneButtonClicked;
    }

    private void OnProductPickedUp(Product product)
    {
        titleScreen.style.display = DisplayStyle.None;
        gameHUD.style.display = DisplayStyle.None;
        productLibraryScreen.style.display = DisplayStyle.None;
        Debug.Log("picked up object " + product.Definition.DisplayName);

        CreateProductMaterialOptions(product);
        OnAddProductButtonClicked();
    }

    private void CreateProductEntries()
    {
        VisualElement container = productLibraryScreen.Q<VisualElement>("ProductList");
        container.Clear();

        // Create new buttons
        foreach(Product productPrefab in allProducts)
        {
            VisualElement entry = productEntryUIAsset.Instantiate();
            Button button = entry.Q<Button>("Button");
            button.clicked += () =>
            {
                Product product = playerController.GetHoldingProduct();

                if(product)
                {
                    // Delete the held object
                    Destroy(product.gameObject);
                    playerController.SetHoldingProduct(null);
                }

                product = Instantiate(productPrefab);
                playerController.SetHoldingProduct(product);
                CreateProductMaterialOptions(product);
                // TODO: Model options

                UpdateProductEntries();
            };

            // Set button text to the DisplayName of the product Definition.
            button.text = productPrefab.Definition.DisplayName;
            container.Add(entry);
        }

        UpdateProductEntries();
    }

    private void CreateProductMaterialOptions(Product product)
    {
        VisualElement container = productLibraryScreen.Q<VisualElement>("ProductOptionsGroup");
        container.Clear();

        // Show UI with this product's material and model options from the definition
        for(int i = 0; i < product.Definition.Materials.Length; i++)
        {
            MaterialDefinition material = product.Definition.Materials[i];
            int index = i;
            VisualElement entry = productEntryUIAsset.Instantiate();
            Button button = entry.Q<Button>("Button");
            button.clicked += () =>
            {
                product.SetMaterial(index);

                UpdateMaterialOptions();
            };

            button.text = material.DisplayName;
            container.Add(button);
        }

        UpdateMaterialOptions();

        // Display model options
        container.style.display = DisplayStyle.Flex;
    }

    private void UpdateProductEntries()
    {
        VisualElement container = productLibraryScreen.Q<VisualElement>("ProductList");

        // Add ".unselected" class to all buttons where the product is not the currently held product
        foreach(VisualElement entry in container.Children())
        {
            Button button = entry.Q<Button>("Button");
            Product product = playerController.GetHoldingProduct();
            if(product && button.text != product.Definition.DisplayName)
            {
                button.AddToClassList("unselected");
            } else
            {
                button.RemoveFromClassList("unselected");
            }
        }
    }

    private void UpdateMaterialOptions()
    {
        VisualElement container = productLibraryScreen.Q<VisualElement>("ProductOptionsGroup");

        // Add ".unselected" class to all buttons where the material is not the currently held product's material
        foreach(VisualElement entry in container.Children())
        {
            Button button = entry.Q<Button>("Button");
            Product product = playerController.GetHoldingProduct();
            int index = container.IndexOf(entry);
            if(product && index != product.CurrentMaterialIndex)
            {
                button.AddToClassList("unselected");
            } else
            {
                button.RemoveFromClassList("unselected");
            }
        }
    }

    private void OnStartButtonClicked()
    {
        // Hide the title screen when the "Start" button is clicked
        titleScreen.style.display = DisplayStyle.None;

        // Show the game HUD
        gameHUD.style.display = DisplayStyle.Flex;
    }

    private void OnAddProductButtonClicked()
    {
        // Hide the game HUD when the "Add Product" button is clicked
        gameHUD.style.display = DisplayStyle.None;

        // Show the product library screen
        productLibraryScreen.style.display = DisplayStyle.Flex;
    }

    private void OnAddToSceneButtonClicked()
    {
        // Hide the product library screen when the "Add To Scene" button is clicked
        productLibraryScreen.style.display = DisplayStyle.None;

        // Show the game HUD
        gameHUD.style.display = DisplayStyle.Flex;

        playerController.DropHoldingProduct();
    }
}
