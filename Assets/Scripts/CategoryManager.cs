using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    [System.Serializable]
    public class Category
    {
        public string name;
        public string prompt;
    }

    [SerializeField] private List<Category> categories;

    public List<Category> GetCategories()
    {
        return categories;
    }
}
