using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private string _name;
    public string Name { get { return _name; } }

    [SerializeField] private bool _open;

    private void Start() 
    {
        gameObject.SetActive(_open);
    }

    public void Open()
    {
        _open = true;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        _open = false;
        gameObject.SetActive(false);
    }
}
