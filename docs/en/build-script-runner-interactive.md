

When executing script locally it is recomended to run FlubuCore in interactive mode:`flubu -i`

## **Features**

Features in interactive mode

- Target's tab completition with tab key
- Option's tab completition with tab key
- Target help displayed at the bottom of console
- Option help displayed at the bottom of console
- execute external commands and operable programs. For some of them FlubuCore offers tab completion with help out of the box(such as dotnet, git..)
- Next / previos target with up and down arrow
- Next / previos option with up and down arrow
- History of executed commans with up and down arrow
- No need to load script for each executed command
- reload or load another script
- Navigation beatwen folders


## **Demo**
![Interactive mode in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCore_Interactive_mode_full.gif)

## **Internal commands**

- `Cd` change directory.
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