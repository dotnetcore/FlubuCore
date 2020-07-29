挡在控制台中执行 FlubuCore 脚本或其他本地程序时，建议在 FlubuCore 交互模式下运行，它将带给你一些非常有意思的功能，这些功能已在[功能一节](#功能)中列明。只需要在你常用的控制台程序中运行 `flubu -i` 便可进入 FlubuCore 交互模式。

## **演示**

![交互模式](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCore_Interactive_mode_full.gif)

 *在演示中[构建脚本](https://gist.github.com/mzorec/c2e0d0572ed023f1d3ebbe72cb5903fc)。*

## **功能**

交互模式下的功能

- 使用「tab 键」来完成 Target 选项卡
- 使用「tab 键」来完成 Option 选项卡
- Option's (parameter) value tab completition with tab key for enum types
- 在控制台底部显示 Target 帮助
- 在控制台底部显示 Option 帮助
- 执行外部命令。这意味着如果在 PowerShell 中运行 FlubuCore 交互模式，则所有 PowerShell 命令都可以在 FlubuCore 的交互模式中使用。
- 执行外部程序。对于其中部分程序，FlubuCore 交互模式提供 Tab 键自动补全（比如 dotnet、git 等，[查看完整列表](通过-Tab-键补全完成的外部程序列表)）
- 使用「↑ 键」和「↓ 键」来切换 Target 选项卡
- 使用「↑ 键」和「↓ 键」来切换 Option 选项卡
- Next / previos target or option with tab key
- 使用「↑ 键」和「↓ 键」来切换命令的历史记录
- 无需为每个命令加载脚本
- 重新加载当前脚本，或加载另一个脚本
- 在文件夹间切换

## **内部命令**

- `cd` 变更目录。
- `dir` 列出文件和目录。
- `l|load -s={script}` 加载另一个 FlubuCore 脚本。
- `r|reload` 重新加载当前脚本。
- `e|q|exit|quit` 退出 FlubuCore 交互模式。

## **热键**

FlubuCore 支持以下热键：

- 「回车键」执行命令
- 「Tab 键」提示用户完成 target/option 输入
- 「↑ 键」在历史记录中选择上一条命令
- 「↓ 键」在历史记录中选择下一条命令
- 「← 键」光标左移
- 「→ 键」光标右移
- 「Backspace 键」删除光标前的一个字符
- 「Delete 键」删除光标后的一个字符
- 「Home 键」光标跳转到用户输入的开头处
- 「End 键」光标跳转到用户输入的结尾处

## 通过 Tab 键补全完成的外部程序列表

在 FlubuCore 交互模式中通过 Tab 键来补全完成 options/switches 的外部程序列表。计划在近期添加对 docker、octopus、azure、npm 和 chocolatey 命令的支持。

- dotnet build
- dotnet test
- dotnet pack
- dotnet publish
- dotnet nuget push
- dotnet restore
- dotnet tool install
- dotnet tool update
- dotnet tool uniinstall
- git add
- git commit
- git push
- git checkout
- git clone
- git submodule
- git rm
- git tag
- gitversion
- coverlet
- sqlcmd.exe