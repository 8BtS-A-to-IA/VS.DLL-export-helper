# Visual Studio .DLL Export Helper
## About
Export helper is an assistant application tool for Visual Studio DLL specific projects, projects which modify non-attachable applications (like modding for games), providing a luanchable executable which automates the exportation of a solution's generated DLL file, through duplicating the file into the destination folder.

VS.DLL.EH was prominently made for streamlining modding RoR2 via Visual Studio and BepInEx, however the program is designed in that any project which needs to automatically duplicate a generated file upon compilation--regardless of the IDE--is able to use the executable in place of the application launched in debugging.

## Installation
You can either clone the entire repository and follow the instructions in [developing](#developing) or go [here](https://github.com/8BtS-A-to-IA/VS.DLL-export-helper/blob/main/bin/Debug/exporter.exe) and download the executable separately.

You can then follow the steps down in [usage](#usage) to start using the executable in your projects, or you can open it and follow the instructions in manual setup mode (either directly building and running it in VS or run the executable outside of an IDE).

**Keep in mind if you open it outside of an IDE; it's unsigned and duplicates files so your anti-virus will likely ping it as malicious/PuP. Always check the code before running a foreign executable.**

## Usage
### VS 2017/2019
To insert this into your project:
- Open your solution and select (in the top bar) 'Project' then 'Properties'
- On the left side find 'Debug' and select the 'Start external program' radio button.
- In the input beside it input the **full** directory of exporter.exe (e.g. C:\Users\Public\Desktop\exporter.exe)
- In the input beside 'Command line Arguments' fill as according to bellow (space between each):

1. (in quotes ") What to call the new file.
2. (in quotes ") DLL in IDE directory (include ###.dll).
3. (in quotes ") Directory to duplicate file to.
4. (optional) Make a separate mod folder? ({T}/F)
5. (optional) Enter debug? (T/{F})

After saving, you should be able to simply run whenever you make an update, if the program detects the file is already there (presumably an older version) it will create a 'backedup DLLs' folder in the same folder as the destination folder, if the destination folder is somewhere in "Risk of Rain 2\BepInEx\plugins" specifically it will instead create a "backedup mods" folder in BepInEx's directory, and move the file into the backup folder - so that the program never deletes anything, preventing accidents.

## Developing
### How can I develop for this project?
After cloning the repository and ensuring you have any version of [VS 2017/2019](https://visualstudio.microsoft.com/), you should be able to simply open the .snl file to open the project in VS.

Before posting a merge request, please ensure you've:
- Adequately checked for 'top level' bugs
- Provided enough commenting/sudo-code for other contributers to quickly understand the process (if necessary)

For the sake of documenting bugfixes, when posting a merge request, please ensure you detail any changes by:
- Describing what was changed (in the head)
- How the changes where made (in the 'extended description')
  - If the merge request only adds new code and does not edit any pre-existing code, feel free to only fill the head.
