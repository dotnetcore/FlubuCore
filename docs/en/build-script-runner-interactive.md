

When executing flubu script or any other process locally in console it is recomended to run FlubuCore in interactive mode it gives you some really nice features which are listed in the [features section.](#features) To start FlubuCore interactive mode just execute command `flubu -i` in your favorite console. 

## **Demo**
![Interactive mode in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCore_Interactive_mode_full.gif)
 *[Build script](https://gist.github.com/mzorec/c2e0d0572ed023f1d3ebbe72cb5903fc) used in demo.*

## **Features**

Features in interactive mode

- Target's tab completition with tab key
- Option's (parameter) tab completition with tab key
- Option's (parameter) value tab completition with tab key for enum types
- Target help displayed at the bottom of console
- Option help displayed at the bottom of console
- Execute external commands. Meaning if you run FlubuCore interactive mode for example in powershell all powershell commands are available in FlubuCore interactive mode.
- Execute external processes. For some of them FlubuCore offers tab completion with help at the bottom of console out of the box(such as dotnet, git...[See full list](#external-processes-tab-completion-list))
- Next / previos target with up and down arrow
- Next / previos option with up and down arrow
- Next / previos target or option with tab key
- History of executed commans with up and down arrow
- No need to load script for each executed target
- reload or load another script
- Navigation beatwen folders


## **Internal commands**

- `cd` change directory.
- `dir` list files and directories
- `l|load -s={script}` Load another FlubuCore script.
- `r|reload` Reload currently loaded script.
- `e|q|exit|quit` Exit Flubu Core interactive mode.

## **Hot keys**
Following hot keys are supported:

- `Enter` Executes entered command
- `Tab` completes user's input with active target/option hint (if found)
- `Up arrow` select previous command from history if user's input is empty or select previous hint (if possible)
- `Down arrow` select next command from history if user's input is empty or select next hint (if possible)
- `Left arrow` move cursor left
- `Right arrow` move cursor right
- `Backspace` removes character before cursor
- `Delete` removes character under cursor
- `Home` move cursor to the beginning of user's input
- `End` move cursor to the end of user's input

## External processes tab completion list

List of external processes for which tab completion of options/switches is available in FlubuCore interactive mode. 
it is planned that all docker, octopus, azure, npm and chocolatey commands will also be supported in the near feature.

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