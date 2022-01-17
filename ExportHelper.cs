using System;
using System.IO;

namespace exporter {
    /// <summary>
    /// Attatch to a mods' program to run when building, with command line arguments set correctly and this will automatically copy your mods .dll file into bepinex.
    /// </summary>
    class ExportHelper {
        static void Main(string[] args) {
            Console.ForegroundColor = ConsoleColor.White;//set text to white
            if (args.Length > 2) {//if there is at least 3 arguments
                Settings1.Default.Target = args[0];//set variables as args
                Settings1.Default.VSDir = args[1];//note: this has no error checking
                Settings1.Default.ModDir = args[2];
                if (args.Length >= 4) {
                    Settings1.Default.makeSeperateModFolder = args[3].StartsWith("T", StringComparison.InvariantCultureIgnoreCase);
                }

                if (args.Length >= 5) {
                    Settings1.Default.debug = args[4].StartsWith("T", StringComparison.InvariantCultureIgnoreCase);
                    if (Settings1.Default.debug) {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Debug mode activated. Stepping.");
                        Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                Settings1.Default.Save();//update settings

                if (Settings1.Default.BC) {//if an error has occured in the last run
                    while (true) {
                        Console.Error.WriteLine("An error occured last run, if the error persists please try manual setup mode. Launch into manual setup mode? (Y/N)");
                        string res = Console.ReadLine();//wait for response and capture
                        if (res.Equals("N", StringComparison.InvariantCultureIgnoreCase)) {//if response is no
                            Settings1.Default.BC = false;//reset bugcheck
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("User choosen not to enter MS mode.");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadLine();
                            break;
                        } else if (res.Equals("Y", StringComparison.InvariantCultureIgnoreCase)) {//otherwise continue into manual setup
                            break;
                        } else {
                            Console.WriteLine("Please respond with 'Y' or 'N'.");
                        }
                    }
                }
            } else if (args.Length != 0) {//if running with incorrect arg count but not on 0 args
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Build failed. Too few arguments passed to Auto Exporter, at least 3 arguments must be provided otherwise none.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.WriteLine("Argument 1 (string): Your mods name (the namespace) which should be the same as the .dll.");
                Console.WriteLine("Argument 2 (string): Your mods .dll directory in Visual Studio.");
                Console.WriteLine("Argument 3 (string): Where your mod should be copied to.");
                Console.WriteLine("Argument 4 (bool): Before this copies the mod, should this create/look for a seperate mod folder (which will have the same name as argument 1)?");
                Console.WriteLine("Argument 5 (bool): Should this enter debugging mode?");
                Console.ReadLine();
                return;
            }

            if(args.Length == 0 && !Settings1.Default.BC && Settings1.Default.Target != "" && Settings1.Default.VSDir != "" && Settings1.Default.ModDir != "") {//if running with no command line arguments, an error mode has not occured and there are settings still set
                Console.WriteLine("Running manually. Auto Exporter has detected existing settings.");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("VS directory: " + Settings1.Default.VSDir);
                Console.WriteLine("Mod name: " + Settings1.Default.Target);
                Console.WriteLine("Mod directory: " + Settings1.Default.ModDir);
                Console.WriteLine("Creating seperate mod folder? " + Settings1.Default.makeSeperateModFolder);

                while (true) {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Run with currently set settings?");
                    string res = Console.ReadLine();//wait for response and capture
                    if (res.Equals("N", StringComparison.InvariantCultureIgnoreCase)) {//if response is no
                        Settings1.Default.BC = true;//Continue into manual mode
                        break;
                    } else if(res.Equals("Y", StringComparison.InvariantCultureIgnoreCase)) {
                        break;
                    } else {
                        Console.WriteLine("Please respond with 'Y' or 'N'.");
                    }
                }

                while (true) {
                    Console.WriteLine("Activate debug?");
                    Console.ForegroundColor = ConsoleColor.White;
                    string res = Console.ReadLine();
                    if (res.Equals("Y", StringComparison.InvariantCultureIgnoreCase)) {
                        Settings1.Default.debug = true;
                        break;
                    } else if (res.Equals("N", StringComparison.InvariantCultureIgnoreCase)) {
                        Settings1.Default.debug = false;
                        break;
                    } else {
                        Console.WriteLine("Please respond with 'Y' or 'N'.");
                    }
                }

            }

            try {
                Console.WriteLine("Welcome to Auto Exporter.");
                bool bc = false;//this sessions' bugcheck state

                if (Settings1.Default.debug) {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("VSDir: " + Settings1.Default.VSDir);
                    Console.WriteLine("Target: " + Settings1.Default.Target);
                    Console.WriteLine("ModDir: " + Settings1.Default.ModDir);
                    Console.WriteLine("BC: " + Settings1.Default.BC);
                    Console.WriteLine("Sep folder?" + Settings1.Default.makeSeperateModFolder);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadLine();
                }

                if (Settings1.Default.VSDir.Length == 0 || Settings1.Default.BC) {//if VSDir is not already set or an error has occured (or call to enter manual mode)
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Welcome to manual setup mode, to get started please input your mods .dll directory:");
                    Console.ForegroundColor = ConsoleColor.White;
                    string input = Console.ReadLine();

                    if (Settings1.Default.debug) {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Finished read for VSDir, read: " + input);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.ReadLine();
                    }

                    if (File.Exists(input)) {//if the input file already exists
                        if (Settings1.Default.debug) {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("found VSDir");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.ReadLine();
                        }
                        //no need to check directory as file already exists
                        Settings1.Default.VSDir = input;//update VSDir setting
                        Settings1.Default.Save();//and update

                        if (Settings1.Default.debug) {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("VSDir updated");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.ReadLine();
                        }
                    } else {//if the file does not exist
                        Console.ForegroundColor = ConsoleColor.Red;
                        bc = true;
                        Settings1.Default.BC = true;
                        Console.WriteLine("Input file does not exist! Closing!");
                        Settings1.Default.Target = "";
                        Settings1.Default.ModDir = "";
                        Settings1.Default.VSDir = "";
                        Settings1.Default.makeSeperateModFolder = true;
                        Settings1.Default.Save();
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadLine();
                        return;
                    }
                }

                if (Settings1.Default.Target.Length == 0 || Settings1.Default.BC) {//if the target is not already set or an error has occured (or call to enter manual mode)
                    Console.WriteLine("Please input your mods project name (should be the namespace):");
                    Console.ForegroundColor = ConsoleColor.White;
                    Settings1.Default.Target = Console.ReadLine();
                    Settings1.Default.Save();

                    if (Settings1.Default.debug) {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Target updated");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.ReadLine();
                    }
                }

                if (Settings1.Default.ModDir.Length == 0 || Settings1.Default.BC) {//if the modDir is not already set or an error has occured (or call to enter manual mode)
                    Console.WriteLine("Please input the FULL directory of the DLLs folder (including the DLL):");
                    Console.ForegroundColor = ConsoleColor.White;
                    string input = Console.ReadLine();

                    if (Settings1.Default.debug) {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Finished read for MODDir, read: " + input);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.ReadLine();
                    }

                    if (Directory.Exists(input)) {//if the input directory exists
                        if (Settings1.Default.debug) {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("found MODDir");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.ReadLine();
                        }//same rule applies here
                        Settings1.Default.ModDir = input;
                        Settings1.Default.Save();

                        if (Settings1.Default.debug) {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("ModDir updated");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.ReadLine();
                        }
                    } else {//if the input directory does not exist
                        while (true) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Input directory does not exist! The app can create a new folder with the specified directory, create it? (Y/N)");
                            Console.ForegroundColor = ConsoleColor.White;
                            string res = Console.ReadLine();
                            if (res.Equals("N", StringComparison.InvariantCultureIgnoreCase)) {
                                bc = true;
                                Settings1.Default.BC = true;
                                Settings1.Default.Target = "";
                                Settings1.Default.ModDir = "";
                                Settings1.Default.VSDir = "";
                                Settings1.Default.makeSeperateModFolder = true;
                                Settings1.Default.Save();
                                return;
                            } else if (res.Equals("Y", StringComparison.InvariantCultureIgnoreCase)) {
                                break;
                            } else {
                                Console.WriteLine("Please respond with 'Y' or 'N'.");
                            }
                        }
                        //if Y, then continue and let the rest of the system create the directory as needed
                        
                    }
                }

                if (args.Length < 4) {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Should this create a seperate mod folder? (Y/N)");
                    string resSt = Console.ReadLine();
                    Settings1.Default.makeSeperateModFolder = resSt.Equals("Y", StringComparison.InvariantCultureIgnoreCase);
                    Settings1.Default.Save();
                }

                Console.ForegroundColor = ConsoleColor.White;

                if (args.Length == 0) {
                    Console.WriteLine();
                    Console.WriteLine("If you want this to run whenever you press 'Start' in Visual Studio;\r\nopen your solution and select (up the top) 'Project' then 'Properties'.");
                    Console.WriteLine("On the left side, go to 'Debug' and select the 'Start external program' radio button.\r\nIn the box beside it copy this into it:");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(Directory.GetCurrentDirectory() + "\\exporter.exe");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine("Then in the box next to 'command line arguments' copy this into it (this is based on what you put above):");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\"" + Settings1.Default.Target + "\" \"" + Settings1.Default.VSDir + "\" \"" + Settings1.Default.ModDir + "\" " + Settings1.Default.makeSeperateModFolder.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                }
                //-------------

                if (!bc) {//if not encountered an error this run
                    Console.WriteLine("Copying built mod and moving into destination.");
                    string path = "";//path is used to store the mod's VS directory in case it needs to ever be edited later (for now it's just readability)

                    try {
                        path = Settings1.Default.VSDir;

                        if (Settings1.Default.debug) {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("path set to " + path);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadLine();
                        }

                        if (File.Exists(path)) {//if the file to copy exists
                            Console.WriteLine("VS mods directory got. Creating the mods' folder, if it doesn't already exist, and copying the file.");

                            string modDir = "";
                            if (Settings1.Default.makeSeperateModFolder) {
                                modDir = Settings1.Default.ModDir + "\\" + Settings1.Default.Target;//get the mod's folder
                            } else {
                                modDir = Settings1.Default.ModDir;
                            }

                            if (Settings1.Default.debug) {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("modDir set to " + modDir);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.ReadLine();
                            }

                            string backupDir = Settings1.Default.ModDir + "\\" + "backedup DLLs";
                            if (Settings1.Default.ModDir.Contains(@"Risk of Rain 2\BepInEx\plugins")) {
                                backupDir = Settings1.Default.ModDir.Split(new string[] { @"Risk of Rain 2\BepInEx\plugins" }, StringSplitOptions.RemoveEmptyEntries)[0]
                                    + @"\Risk of Rain 2\BepInEx\backedup mods";//get the "Risk of Rain 2\BepInEx\backedup mods" folder
                            }

                            if (Settings1.Default.debug) {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("backupDir set to " + backupDir);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.ReadLine();
                            }

                            if (!Directory.Exists(backupDir)) {//if the backup folder doesn't exist
                                if (Settings1.Default.debug) {
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.WriteLine("Could not find the backup directory, likely doesn't exist. Creating.");
                                    Console.ReadLine();
                                }

                                Directory.CreateDirectory(backupDir);//create the folder

                                if (Settings1.Default.debug) {
                                    Console.WriteLine("Created.");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }

                                if (!Directory.Exists(backupDir + "\\" + Settings1.Default.Target)) {//if the mods' backup folder doesn't exist
                                    if (Settings1.Default.debug) {
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Console.WriteLine("Could not find the mod's backup folder. Creating.");
                                        Console.ReadLine();
                                    }

                                    Directory.CreateDirectory(backupDir + "\\" + Settings1.Default.Target);//create the folder

                                    if (Settings1.Default.debug) {
                                        Console.WriteLine("Created");
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }

                                }

                            } else {//if the backup folder doesn't already exist
                                if (Settings1.Default.debug) {
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.WriteLine("Got backup folder, creating mods backup folder if it doesn't already exist.");
                                    Console.ReadLine();
                                }

                                Directory.CreateDirectory(backupDir + "\\" + Settings1.Default.Target);//create the folders

                                if (Settings1.Default.debug) {
                                    Console.WriteLine("Created.");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }

                            }
                            backupDir += "\\" + Settings1.Default.Target + "\\" + Settings1.Default.Target + " "
                                + DateTime.Now.Date.Day + "-" + DateTime.Now.Date.Month + "-" + DateTime.Now.Date.Year + " "
                                + DateTime.Now.TimeOfDay.Hours + "-" + DateTime.Now.TimeOfDay.Minutes + "-" + DateTime.Now.TimeOfDay.Seconds + ".dll";
                            //set backupDir to now be pointing to a new file name (what the old file will be renamed to when moved into backup) based on a date/time format
                            //this is so there is (realistically) never any conflicts when moving

                            if (Settings1.Default.debug) {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Set backupDir to: " + backupDir);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.ReadLine();
                            }

                            if (!Directory.Exists(modDir)) {//if the mod's folder does not already exist
                                if (Settings1.Default.debug) {
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.WriteLine("ModDir does not exist, creating directory and copying file.");
                                    Console.ReadLine();
                                }

                                Directory.CreateDirectory(modDir);//create the folder
                                modDir += "\\" + Settings1.Default.Target + ".dll";
                                File.Copy(path, modDir);//copy the file over and done

                                if (Settings1.Default.debug) {
                                    Console.WriteLine("Done.");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }

                            } else {//if the mod's folder already exists
                                modDir += "\\" + Settings1.Default.Target + ".dll";
                                if (Settings1.Default.debug) {
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.WriteLine("Got modDir, modDir upated to: " + modDir);
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.ReadLine();
                                }

                                if (File.Exists(modDir)) {//and the file already exists
                                    if (Settings1.Default.debug) {
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Console.WriteLine("Found a file already exist, updating the file by moving the old one to backups and copying the new one in.");
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.ReadLine();
                                    }

                                    try {//try to replace the file with the new version
                                        File.Move(modDir, backupDir);//move the old file into the backup folder (and rename the file)
                                        File.Copy(path, modDir);
                                        //File.Replace(path, modDir, backupDir);//Replace the file and place the old one in backup (doesn't work for some reason, try again later)
                                    } catch (Exception e) {
                                        Settings1.Default.BC = true;
                                        Settings1.Default.Target = "";
                                        Settings1.Default.ModDir = "";
                                        Settings1.Default.VSDir = "";
                                        Settings1.Default.makeSeperateModFolder = true;
                                        Settings1.Default.Save();
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.Error.WriteLine("A critical error occured while trying to replace the older version of your DLL!");
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Error.WriteLine("Error message: " + e.Message);
                                        Console.Error.WriteLine("Error stacktrace: " + e.StackTrace);
                                        Console.ReadLine();
                                        return;

                                    }

                                } else {//if the mod's directory (bepinex plugin folder) does not exist
                                    if (Settings1.Default.debug) {
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Console.WriteLine("Found not file in modDir, no versions backed up. Copying new version in.");
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.ReadLine();
                                    }

                                    File.Copy(path, modDir);//copy the file over and done
                                }
                            }
                        } else {//error only occurs when there was no previous error and the file was already defined (either previosuly or via command line arguments)
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.WriteLine("Yor mods .dll file does not exist! Did VS fail to compile or did you link incorrectly?");
                            Settings1.Default.BC = true;
                            Console.ForegroundColor = ConsoleColor.White;
                            Settings1.Default.Target = "";
                            Settings1.Default.ModDir = "";
                            Settings1.Default.VSDir = "";
                            Settings1.Default.makeSeperateModFolder = true;
                            Settings1.Default.Save();
                            Console.ReadLine();
                            return;
                        }

                    } catch (System.IO.PathTooLongException e) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine("Working directory for your project is too long! (System.IO.PathTooLongException). " + e.HelpLink);
                        Settings1.Default.BC = true;
                        Console.ForegroundColor = ConsoleColor.White;
                        Settings1.Default.Target = "";
                        Settings1.Default.ModDir = "";
                        Settings1.Default.VSDir = "";
                        Settings1.Default.makeSeperateModFolder = true;
                        Settings1.Default.Save();
                        Console.ReadLine();
                        return;
                    }

                    Console.WriteLine("Done!");
                    Settings1.Default.BC = false;
                    Settings1.Default.Save();//finish
                }

                if (Settings1.Default.debug) {
                    Console.ReadLine();//let the user see that we're done.
                }
            } catch (Exception e) {
                Settings1.Default.BC = true;
                Settings1.Default.Target = "";
                Settings1.Default.ModDir = "";
                Settings1.Default.VSDir = "";
                Settings1.Default.makeSeperateModFolder = true;
                Settings1.Default.Save();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Error.WriteLine("A critical unhandled error occured!");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Error message: " + e.Message);
                Console.Error.WriteLine("Error stacktrace: " + e.StackTrace);
                Console.ReadLine();
            }
        }
    }
}