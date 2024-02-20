using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private VisualTreeAsset productEntryUIAsset;
    [SerializeField] private UIDocument uiDocument;
    [Space]
    [SerializeField] private ARMeshManager meshManager;
    [SerializeField] private Material shadowMaterial;
    [SerializeField] private Material visibleMaterial;
    [Space]
    [SerializeField] private Product[] allProducts;

    private VisualElement titleScreen;
    private VisualElement gameHUD;
    private VisualElement productLibraryScreen;
    private VisualElement productOptionsGroup;
    private VisualElement actionButtonContainer;
    private Button dropObjectButton;
    private Button lockObjectButton;
    private Button cancelButton;
    private bool showingGeneratedMesh;
    private MeshRenderer meshRendererPrefab;

    private void Start()
    {
        meshRendererPrefab = meshManager.meshPrefab.GetComponent<MeshRenderer>();
        
        titleScreen = uiDocument.rootVisualElement.Q("TitleScreen");
        gameHUD = uiDocument.rootVisualElement.Q("HUD");
        productLibraryScreen = uiDocument.rootVisualElement.Q("ProductLibraryScreen");
        
        actionButtonContainer = productLibraryScreen.Q("ActionButtonContainer");
        actionButtonContainer.style.display = DisplayStyle.None;

        // Add UI elements for each product, and associated ProductDefinition
        CreateProductEntries();

        // Subscribe to pickup event from player controller
        playerController.OnProductSelected += OnProductSelected;

        // Show the title screen on game start
        titleScreen.style.display = DisplayStyle.Flex;
        
        productOptionsGroup = productLibraryScreen.Q<VisualElement>("ProductOptionsGroup");

        // Add click event listener to the "Start" button
        Button startButton = titleScreen.Q<Button>("StartButton");
        startButton.clicked += OnStartButtonClicked;

        // Add click event listener to the "Add Product" button
        Button addProductButton = gameHUD.Q<Button>("AddProductButton");
        addProductButton.clicked += OnAddProductButtonClicked;

        // Add click event listener to the "Add To Scene" button
        dropObjectButton = productLibraryScreen.Q<Button>("AddProductToSceneButton");
        dropObjectButton.clicked += OnDropObjectButtonClicked;
        
        // Add click event listener to the "lock object" button
        lockObjectButton = productLibraryScreen.Q<Button>("LockObjectInPlace");
        lockObjectButton.clicked += OnLockObjectButtonClicked;
        
        cancelButton = productLibraryScreen.Q<Button>("CancelLockButton");
        cancelButton.clicked += StopHoldingProduct;
        cancelButton.style.display = DisplayStyle.None;

        // Add click event listener to the "toggle mesh visibility" button
        Button toggleMeshButton = gameHUD.Q<Button>("ToggleMeshButton");
        toggleMeshButton.clicked += OnToggleMeshButtonClicked;
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
            button.clicked += () => { CreateNewProduct(productPrefab); };

            // Set button text to the DisplayName of the product Definition.
            button.text = productPrefab.Definition.DisplayName;
            container.Add(entry);
        }

        UpdateProductEntries();
    }

    private void CreateNewProduct(Product productPrefab) {
        Product product = playerController.CurrentHoldingProduct;
        if(product) {
            Product newProduct = Instantiate(productPrefab,
                product.transform.position,
                product.transform.rotation);

            newProduct.LockedInPlace = product.LockedInPlace;

            // Delete the old object
            Destroy(product.gameObject);
            product = newProduct;
        } else {
            product = Instantiate(productPrefab, playerController.HoldObjectPoint.position,
                playerController.HoldObjectRotation);
        }

        playerController.CurrentHoldingProduct = product;
        CreateProductMaterialOptions(product);
        UpdateProductEntries();
    }

    private void CreateProductMaterialOptions(Product product)
    {
        productOptionsGroup.Clear();

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
            productOptionsGroup.Add(button);
        }

        UpdateMaterialOptions();
        UpdateProductActionButtons(product);

        // Display model options
        productOptionsGroup.style.display = DisplayStyle.Flex;
    }

    private void UpdateProductEntries()
    {
        VisualElement container = productLibraryScreen.Q<VisualElement>("ProductList");

        // Add ".unselected" class to all buttons where the product is not the currently held product
        foreach(VisualElement entry in container.Children())
        {
            Button button = entry.Q<Button>("Button");
            Product product = playerController.CurrentHoldingProduct;
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
        // Add ".unselected" class to all buttons where the material is not the currently held product's material
        foreach(VisualElement entry in productOptionsGroup.Children())
        {
            Button button = entry.Q<Button>("Button");
            Product product = playerController.CurrentHoldingProduct;
            int index = productOptionsGroup.IndexOf(entry);
            if(product && index != product.CurrentMaterialIndex)
            {
                button.AddToClassList("unselected");
            } else
            {
                button.RemoveFromClassList("unselected");
            }
        }
    }

    private void UpdateProductActionButtons(Product product) {
        actionButtonContainer.style.display = DisplayStyle.Flex;
        dropObjectButton.style.display = product.LockedInPlace ? DisplayStyle.None : DisplayStyle.Flex;
        cancelButton.style.display = product.LockedInPlace ? DisplayStyle.Flex : DisplayStyle.None;
        lockObjectButton.text = product.LockedInPlace ? "Unlock" : "Lock";
    }
    
    private void OnStartButtonClicked()
    {
        // Hide the title screen when the "Start" button is clicked
        titleScreen.style.display = DisplayStyle.None;
        gameHUD.style.display = DisplayStyle.Flex;
    }

    private void OnAddProductButtonClicked()
    {
        // Hide the game HUD when the "Add Product" button is clicked
        productLibraryScreen.style.display = DisplayStyle.Flex;
        gameHUD.style.display = DisplayStyle.None;
        actionButtonContainer.style.display = DisplayStyle.None;
        productOptionsGroup.style.display = DisplayStyle.None;
    }
    
    private void OnProductSelected(Product product)
    {
        productLibraryScreen.style.display = DisplayStyle.Flex;
        gameHUD.style.display = DisplayStyle.None;

        CreateProductMaterialOptions(product);
    }

    private void OnDropObjectButtonClicked()
    {
        if (!playerController.CurrentHoldingProduct) return;
        
        Product product = playerController.CurrentHoldingProduct;
        product.Rigidbody.velocity = Vector3.zero;
        product.Rigidbody.angularVelocity = Vector3.zero;
        product.LockedInPlace = false;
        StopHoldingProduct();
    }
    
    private void OnLockObjectButtonClicked()
    {
        if (!playerController.CurrentHoldingProduct) return;
        
        Product product = playerController.CurrentHoldingProduct;
        product.LockedInPlace = !product.LockedInPlace;
        UpdateProductActionButtons(product);

        if (product.LockedInPlace)
        {
            StopHoldingProduct();
        }
    }

    private void OnToggleMeshButtonClicked()
    {
        showingGeneratedMesh = !showingGeneratedMesh;
        meshRendererPrefab.sharedMaterial = showingGeneratedMesh ? visibleMaterial : shadowMaterial;
        
        if (meshManager.meshes.Count <= 0)
        {
            return;
        }

        foreach (MeshFilter meshFilter in meshManager.meshes)
        {
            if (meshFilter.TryGetComponent(out MeshRenderer rend))
            {
                rend.sharedMaterial = showingGeneratedMesh ? visibleMaterial : shadowMaterial;
            }
        }
    }
    
    private void StopHoldingProduct()
    {
        if (!playerController.CurrentHoldingProduct) return;
        playerController.CurrentHoldingProduct = null;
        productLibraryScreen.style.display = DisplayStyle.None;
        gameHUD.style.display = DisplayStyle.Flex;
    }
}
