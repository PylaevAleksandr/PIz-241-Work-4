using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;


[Serializable]
public class TextFile
{
    public string FileName { get; set; }
    public string Content { get; set; }

    // Сериализация в XML
    public void SaveAsXml(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TextFile));
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    // Десериализация из XML
    public static TextFile LoadFromXml(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TextFile));
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            return (TextFile)serializer.Deserialize(stream);
        }
    }

    // Сериализация в бинарный формат
    public void SaveAsBinary(string path)
    {
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(FileName);
            writer.Write(Content);
        }
    }

    // Десериализация из бинарного формата
    public static TextFile LoadFromBinary(string path)
    {
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            BinaryReader reader = new BinaryReader(stream);
            TextFile file = new TextFile
            {
                FileName = reader.ReadString(),
                Content = reader.ReadString()
            };
            return file;
        }
    }
}
public class FileSearcher
{
    public List<string> SearchByKeyword(string directory, string keyword)
    {
        List<string> foundFiles = new List<string>();
        foreach (string file in Directory.GetFiles(directory, "*.txt"))
        {
            string content = File.ReadAllText(file);
            if (content.Contains(keyword))
            {
                foundFiles.Add(file);
            }
        }
        return foundFiles;
    }
}
public class TextFileMemento
{
    public string Content { get; }

    public TextFileMemento(string content)
    {
        Content = content;
    }
}

public class TextFileEditor
{
    private TextFile _file;
    private Stack<TextFileMemento> _history;

    public TextFileEditor(TextFile file)
    {
        _file = file;
        _history = new Stack<TextFileMemento>();
    }

    public void Edit(string newContent)
    {
        // Сохраняем текущее состояние
        _history.Push(new TextFileMemento(_file.Content));
        _file.Content = newContent;
    }

    public void Undo()
    {
        if (_history.Count > 0)
        {
            TextFileMemento memento = _history.Pop();
            _file.Content = memento.Content;
        }
    }

    public TextFile GetFile()
    {
        return _file;
    }
}
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите имя файла:");
        string fileName = Console.ReadLine();
        TextFile textFile = new TextFile { FileName = fileName, Content = "" };

        TextFileEditor editor = new TextFileEditor(textFile);

        while (true)
        {
            Console.WriteLine("Выберите действие: 1 - редактировать, 2 - сохранить, 3 - загрузить, 4 - undo, 5 - выйти");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine("Введите новый текст:");
                string newContent = Console.ReadLine();
                editor.Edit(newContent);
                Console.WriteLine("Текущий текст: " + editor.GetFile().Content);
            }
            else if (choice == "2")
            {
                Console.WriteLine("Какой формат? 1 - XML, 2 - бинарный");
                string format = Console.ReadLine();
                if (format == "1")
                {
                    textFile.SaveAsXml(fileName + ".xml");
                }
                else
                {
                    textFile.SaveAsBinary(fileName + ".bin");
                }
            }
            else if (choice == "3")
            {
                Console.WriteLine("Введите имя файла для загрузки:");
                string loadFileName = Console.ReadLine();
                textFile = TextFile.LoadFromXml(loadFileName); // Можно также сделать бинарную загрузку
                editor = new TextFileEditor(textFile);
            }
            else if (choice == "4")
            {
                editor.Undo();
                Console.WriteLine("Изменения отменены. Текущий текст: " + editor.GetFile().Content);
            }
            else if (choice == "5")
            {
                break;
            }
        }
    }
}