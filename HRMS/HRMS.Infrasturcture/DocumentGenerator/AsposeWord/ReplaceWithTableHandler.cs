using Aspose.Words;
using Aspose.Words.Replacing;
using Aspose.Words.Tables;

public class ReplaceWithTableHandler : IReplacingCallback
{
    private readonly List<string[]> _tableData;
    private readonly DocumentBuilder _builder;

    public ReplaceWithTableHandler(Document doc, List<string[]> tableData)
    {
        _tableData = tableData;
        _builder = new DocumentBuilder(doc);
    }

    public ReplaceAction Replacing(ReplacingArgs e)
    {
        // Move builder to the matched text node
        _builder.MoveTo(e.MatchNode);

        // Insert the table
        Table table = _builder.StartTable();
        foreach (var row in _tableData)
        {
            foreach (var cell in row)
            {
                _builder.InsertCell();
                _builder.Write(cell);
            }
            _builder.EndRow();
        }
        _builder.EndTable();

        // Remove the placeholder text node
        e.MatchNode.Remove();

        return ReplaceAction.Skip;
    }
}
