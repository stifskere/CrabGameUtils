using System.Text.RegularExpressions;
using Math = System.Math;
using StringComparison = System.StringComparison;

namespace CrabGameUtils.Extensions;

[ExtensionName("autocomplete")]
public class AutoComplete : Extension
{
    private static System.Collections.Generic.List<string> _completions = new();
    private int _index;
    private string? _lastCompletion;
    private int _lastStringPosition;

    public static void AddCompletion(string completion)
    {
        _completions.Add(completion);
    }

    public static void SetCompletions(System.Collections.Generic.List<string> completions)
    {
        _completions = completions;
    }

    public static System.Collections.Generic.List<string> GetCompletions()
    {
        return _completions;
    }

    public static void ClearCompletions()
    {
        _completions.Clear();
    }
    
    public override void Awake()
    {
        
    }

    public override void Start()
    {
        ChatBox.Instance.inputField.richText = true;
    }

    public override void Update()
    {
        string text = Regex.Replace(ChatBox.Instance.inputField.text, "<color=#444444>.*?</color>", "");
        if (text == "")
        {
            ChatBox.Instance.inputField.text = "";
            return;
        }

        if (Input.GetKeyDown(UnityEngine.KeyCode.DownArrow))
        {
            ChatBox.Instance.inputField.stringPosition = _lastStringPosition;
            _index++;
            _lastCompletion = null;
        }
        
        if (Input.GetKeyDown(UnityEngine.KeyCode.RightArrow))
        {
            int idx = ChatBox.Instance.inputField.text.IndexOf("<color=#444444>", StringComparison.Ordinal);
            if (idx == -1) return;
            ChatBox.Instance.inputField.text = ChatBox.Instance.inputField.text.Replace("<color=#444444>", "").Replace("</color>", "").Insert(idx + 1, "<color=#444444>") + "</color>";
            _lastCompletion = null;
            return;
        }
        
        if (Input.GetKeyDown(UnityEngine.KeyCode.LeftArrow))
        {
            int idx = ChatBox.Instance.inputField.text.IndexOf("<color=#444444>", StringComparison.Ordinal);
            if (idx == -1) return;
            ChatBox.Instance.inputField.text = ChatBox.Instance.inputField.text.Replace("<color=#444444>", "").Replace("</color>", "").Insert(idx - 1, "<color=#444444>") + "</color>";
            _lastCompletion = null;
            return;
        }

        if (Input.GetKeyDown(UnityEngine.KeyCode.UpArrow))
        {
            ChatBox.Instance.inputField.stringPosition = _lastStringPosition;
            _lastCompletion = null;
            _index = Math.Max(_index - 1, 0);
        }
        
        _lastStringPosition = ChatBox.Instance.inputField.stringPosition;
        
        int index = 0;
        foreach (string completion in _completions)
        {
            if (completion.StartsWith(text))
            {
                if (_index > index++) continue;
                if (_lastCompletion != null && _lastCompletion != completion) _index = 0;
                _lastCompletion = completion;
                if (completion.Equals(text))
                {
                    ChatBox.Instance.inputField.text = text;
                    return;
                }
                Regex regex = new Regex(Regex.Escape(text));
                if (Input.GetKeyDown(UnityEngine.KeyCode.Tab))
                {
                    string replaced = regex.Replace(completion, "", 1);
                    string[] split = replaced.Split(" ");
                    if (split.Length > 1)
                    {
                        ChatBox.Instance.inputField.text = text + split[0];
                        ChatBox.Instance.inputField.MoveToEndOfLine(false, false);
                        ChatBox.Instance.inputField.text += "<color=#444444>" + string.Join(" ", split[1..]) + "</color>";
                    }
                    else
                    {
                        ChatBox.Instance.inputField.text = completion;
                        ChatBox.Instance.inputField.MoveToEndOfLine(false, false);
                    }

                    return;
                }
                ChatBox.Instance.inputField.text = text + "<color=#444444>" + regex.Replace(completion, "", 1) + "</color>";
                return;
            }

            if (completion == _lastCompletion) _index = 0;
        }
        ChatBox.Instance.inputField.text = text;
    }
}