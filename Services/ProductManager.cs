// Viple FilesVersion - ProductManager 1.0.0 - Date 25/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using VipleManagement.Models;

namespace VipleManagement.Services
{
    public class ProductManager
    {
        private List<Product> products;
        private string productsFilePath = Path.Combine("vipledata", "products.vff");

        public ProductManager()
        {
            products = new List<Product>();
            LoadProducts();
        }

        public bool AddProduct(Product product)
        {
            try
            {
                products.Add(product);
                SaveProducts();
                LogManager.LogAction($"Produit ajouté: {product.Name}");
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogAction($"Erreur lors de l'ajout du produit: {ex.Message}");
                return false;
            }
        }

        public bool UpdateProduct(Product product)
        {
            try
            {
                Product existingProduct = products.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct != null)
                {
                    int index = products.IndexOf(existingProduct);
                    products[index] = product;
                    SaveProducts();
                    LogManager.LogAction($"Produit mis à jour: {product.Name}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogManager.LogAction($"Erreur lors de la mise à jour du produit: {ex.Message}");
                return false;
            }
        }

        public bool DeleteProduct(string productId)
        {
            try
            {
                Product productToDelete = products.FirstOrDefault(p => p.Id == productId);
                if (productToDelete != null)
                {
                    products.Remove(productToDelete);
                    SaveProducts();
                    LogManager.LogAction($"Produit supprimé: {productToDelete.Name}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogManager.LogAction($"Erreur lors de la suppression du produit: {ex.Message}");
                return false;
            }
        }

        public Product GetProductById(string productId)
        {
            return products.FirstOrDefault(p => p.Id == productId);
        }

        public List<Product> GetAllProducts()
        {
            return products;
        }

        public List<Product> GetProductsByCategory(ProductCategory category)
        {
            return products.Where(p => p.Category == category).ToList();
        }

        public List<Product> GetExpiredProducts()
        {
            return products.Where(p => p.IsLicenseExpired()).ToList();
        }

        public List<Product> GetProductsExpiringInDays(int days)
        {
            return products.Where(p => !p.IsLicenseExpired() && p.DaysUntilExpiration() <= days).ToList();
        }

        private void LoadProducts()
        {
            try
            {
                if (File.Exists(productsFilePath))
                {
                    using (FileStream fs = new FileStream(productsFilePath, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        products = (List<Product>)formatter.Deserialize(fs);
                    }
                }
                else
                {
                    // Créer quelques produits de test si le fichier n'existe pas
                    CreateSampleProducts();
                    SaveProducts();
                }
            }
            catch (Exception ex)
            {
                LogManager.LogAction($"Erreur lors du chargement des produits: {ex.Message}");
                products = new List<Product>();
                CreateSampleProducts();
            }
        }

        private void SaveProducts()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(productsFilePath));
                using (FileStream fs = new FileStream(productsFilePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, products);
                }
                UpdateFilesLog("products.vff", "modifiée");
            }
            catch (Exception ex)
            {
                LogManager.LogAction($"Erreur lors de la sauvegarde des produits: {ex.Message}");
            }
        }

        private void CreateSampleProducts()
        {
            products.Add(new Product
            {
                Name = "Windows Server 2022",
                Description = "Système d'exploitation serveur",
                Version = "2022",
                Category = ProductCategory.Software,
                Manufacturer = "Microsoft",
                LicenseKey = "XXXX-XXXX-XXXX-XXXX",
                ExpirationDate = DateTime.Now.AddYears(3),
                Price = 899.99m,
                PurchaseDate = DateTime.Now.AddDays(-30)
            });

            products.Add(new Product
            {
                Name = "Microsoft 365 Business",
                Description = "Suite bureautique et services cloud",
                Version = "2025",
                Category = ProductCategory.License,
                Manufacturer = "Microsoft",
                LicenseKey = "YYYY-YYYY-YYYY-YYYY",
                ExpirationDate = DateTime.Now.AddYears(1),
                Price = 12.50m,
                PurchaseDate = DateTime.Now.AddDays(-60)
            });

            products.Add(new Product
            {
                Name = "Dell PowerEdge R740",
                Description = "Serveur rack 2U",
                Category = ProductCategory.Hardware,
                Manufacturer = "Dell",
                Price = 3999.99m,
                PurchaseDate = DateTime.Now.AddMonths(-3)
            });

            products.Add(new Product
            {
                Name = "Cisco Catalyst 9200",
                Description = "Switch réseau manageable 24 ports",
                Category = ProductCategory.NetworkEquipment,
                Manufacturer = "Cisco",
                Price = 1499.99m,
                PurchaseDate = DateTime.Now.AddMonths(-2)
            });

            products.Add(new Product
            {
                Name = "Symantec Endpoint Protection",
                Description = "Solution antivirus et sécurité",
                Version = "15.0",
                Category = ProductCategory.SecuritySolution,
                Manufacturer = "Symantec",
                LicenseKey = "ZZZZ-ZZZZ-ZZZZ-ZZZZ",
                ExpirationDate = DateTime.Now.AddMonths(8),
                Price = 45.00m,
                PurchaseDate = DateTime.Now.AddMonths(-4)
            });
        }

        private void UpdateFilesLog(string fileName, string action)
        {
            string filesLogPath = "elioslogs-files.txt";
            string entry = $"- Fichier : {fileName} {action} le {DateTime.Now:dd/MM/yyyy} à {DateTime.Now:HH:mm}";
            
            // Vérifier si une entrée pour ce fichier existe déjà aujourd'hui
            if (File.Exists(filesLogPath))
            {
                string[] lines = File.ReadAllLines(filesLogPath);
                bool entryExists = false;
                
                foreach (string line in lines)
                {
                    if (line.Contains(fileName) && line.Contains(DateTime.Now.ToString("dd/MM/yyyy")))
                    {
                        entryExists = true;
                        break;
                    }
                }
                
                if (!entryExists)
                {
                    File.AppendAllText(filesLogPath, entry + Environment.NewLine);
                }
            }
            else
            {
                File.AppendAllText(filesLogPath, entry + Environment.NewLine);
            }
        }
    }
}