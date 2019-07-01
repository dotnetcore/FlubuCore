

When executing script locally it is recomended to run FlubuCore in interactive mode:`flubu -i`

## **Features**

Features in interactive mode

- Target's tab completition with tab key
- Option's tab completition with tab key
- Next / previos target with up and down arrow
- Next / previos option with up and down arrow
- History of executed commans with up and down arrow
- No need to load script for each executed command

## **Demo**
![Interactive mode in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCore_Interactive_mode.gif)

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