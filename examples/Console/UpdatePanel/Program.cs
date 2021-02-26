using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ProgressExample
{
    public static class Program
    {
        public static async Task Main()
        {
            await AnsiConsole.UpdatePanel()
                .StartAsync<int, MyData>(async ctx =>
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        ctx.SetStatus(new MyData(DescriptionGenerator.Generate(), i));
                        await Task.Delay(100);
                    }

                    return 0;
                }, data => RenderTable(data));
        }

        private static IRenderable RenderTable(MyData data)
        {
            var table = new Table().AddColumns("Name", "Value");
            var style = new Style().Foreground(Color.FromInt32(data.Count % 60 / 4 + 1));
            table.AddRow(
                new Text(data.Name, style), // colors and text length can change
                new Text(data.Count.ToString())
            );

            var panel = new Panel(table)
                .Header($"update panel madness {data.Count}")
                .Padding(2, 2)
                .RoundedBorder();
            return panel;
        }
    }
}