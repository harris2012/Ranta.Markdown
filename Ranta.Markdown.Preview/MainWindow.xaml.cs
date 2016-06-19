using Ranta.Markdown.Preview.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ranta.Markdown.Preview
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var output = MarkdownCore.Transform(InputTextBox.Text);

            OutputTextBox.Text = output;

            HtmlTemplate htmlTemplate = new HtmlTemplate();
            htmlTemplate.HtmlContent = output;
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "Content", "tmp.html");
            File.WriteAllText(path, htmlTemplate.TransformText(), Encoding.UTF8);
            OutputBrowser.Source = new Uri(path);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = @"
1.标题
# 一级标题![mahua](mahua-logo.jpg)
## 二级标题
### 三级标题
#### 四级标题
##### 五级标题
###### 六级标题

2.无序列表
* 苹果
* 香蕉
* 橘子

3.有序列表
1. 苹果
2. 香蕉
3.橘子

4.引用
>这里是引用





![mahua](mahua-logo.jpg)
####MaHua是什么?
一个在线编辑markdown文档的编辑器

向Mac下优秀的markdown编辑器mou致敬

##MaHua有哪些功能？

* 方便的`导入导出`功能
    *  直接把一个markdown的文本文件拖放到当前这个页面就可以了
    *  导出为一个html格式的文件，样式一点也不会丢失
* 编辑和预览`同步滚动`，所见即所得（右上角设置）
* `VIM快捷键`支持，方便vim党们快速的操作 （右上角设置）
* 强大的`自定义CSS`功能，方便定制自己的展示
* 有数量也有质量的`主题`,编辑器和预览区域
* 完美兼容`Github`的markdown语法
* 预览区域`代码高亮`
* 所有选项自动记忆

##有问题反馈
在使用中有任何问题，欢迎反馈给我，可以用以下联系方式跟我交流

* 邮件(dev.hubo#gmail.com, 把#换成@)
* QQ: 287759234
* weibo: [@草依山](http://weibo.com/ihubo)
* twitter: [@ihubo](http://twitter.com/ihubo)

##捐助开发者
在兴趣的驱动下,写一个`免费`的东西，有欣喜，也还有汗水，希望你喜欢我的作品，同时也能支持一下。
当然，有钱捧个钱场（右上角的爱心标志，支持支付宝和PayPal捐助），没钱捧个人场，谢谢各位。

##感激
感谢以下的项目,排名不分先后

* [mou](http://mouapp.com/) 
* [ace](http://ace.ajax.org/)
* [jquery](http://jquery.com)

##关于作者

```javascript
  var ihubo = {
    nickName  : ""草依山\"",
    site: ""http://jser.me""
  }
```";
        }
    }
}
