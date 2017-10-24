--------------------------------------------------------------------------------
                                   MaterialUI
                                 Version 1.1.4
                           contact@materialunity.com
                         http://www.materialunity.com/
--------------------------------------------------------------------------------
MaterialUI for Unity is a UI kit that follows Google's official Material Design
guidelines.

If you have questions, suggestions, comments or feature requests, feel free to
ask us anything!

Thanks very much for using our asset - we hope you love it as much as we do!
- Declan & Yohan (The MaterialUI Team)


--------------------------------------------------------------------------------
                                     Support
--------------------------------------------------------------------------------
The best way to find support is right here: http://www.materialunity.com/support/
You'll find questions/answers from others that might already answer your needs!

Otherwise, you can always send us an email at: contact@materialunity.com


--------------------------------------------------------------------------------
                                   Components
--------------------------------------------------------------------------------
In order to create components, you just have to go to GameObject -> MaterialUI
or right click in the Hierarchy and you'll find the MaterialUI menu.
From here, you'll see the list of all the components available in MaterialUI.
Just clicking on one of them, will add it in your current scene as a child of
the selected GameObject or under a Canvas if nothing is selected.

Almost every component has one or multiple custom script attached, that you can
see in the inspector, and use to modify/interact with the component however you
want.

---------- Panel
Creates a basic panel with shadow. You have to put content of the panel under
the PanelLayer GameObject.

---------- Background Image
Creates a simple Image, just like GameObject -> UI -> Image. It's just
there so you don't have to navigate through multiple menus.
This image anchors to fill its parent's size.

---------- Image
Creates a simple Image, just like GameObject -> UI -> Image. It's just
there so you don't have to navigate through multiple menus.

---------- VectorImage
Creates an icon that uses a FontIcon that renders perfectly and non-blurry at any size.
In the Inspector, you'll be able to change the icon by clicking the "Pick Icon"
button to select the icon you like in the FontIcon.
You can also use other FontIcons, this will be presented below when we talk
about "VectorImageManager".

---------- Text
Creates a simple Text, just like GameObject -> UI -> Text. It's just
there so you don't have to navigate through multiple menus.

---------- Buttons
In this menu, you'll be able to create basic buttons, and "round" buttons.
Buttons can be converted from flat to raised (and back) at any time.
----- Text (Flat or Raised - with just a text)
----- Multi Content (Flat or Raised - with an icon and a text)
----- Floating Action Button (Raised or Mini Raised - round with shadow and icon)
----- Icon Button (Normal or Mini - round with icon)

---------- Dropdown
Creates a dropdown, showing a text or an icon. When the user clicks on the
dropdown, it display the dropdown menu with a text + icon for each item.
Like buttons, dropdowns can be converted from flat to raised (and back) at any time.
You can create several types of dropdowns:
----- Flat (Showing the selected text)
----- Raised (Showing the selected text)
----- Icon Button (Showing the selected icon)
----- Mini Icon Button (Showing the selected icon)

---------- Selection Controls (Checkboxes)
----- Checkboxes (Creates a checkbox with a label or an icon)
----- Switches (Creates a switch with a label or an icon)
----- Radio buttons (Creates a group of radio buttons with a label or an icon)

---------- Input Fields
Creates an InputField with a floating hint (that appears when a user select
the inputField), a counter and validation errors.
You can create several types of input fields:
----- Basic
----- Basic with Icon (with an icon on the left)
----- Basic with clear button (with a clear "X" button on the right)
----- Basic with Icon and clear button

---------- Sliders
You can create two types of sliders:
- Continuous: A typical slider that can hava a floating point value
- Discrete: A slider that can only have an integer value.
  Dots at each increment are optional for discrete sliders.
We also provide a variety of different sliders:
----- Simple
----- Left Label (with a label on the left)
----- Left icon (with an icon on the left)
----- Left and Right labels (with a label on left and right)
----- Left label and Right input field (the input field allows to set value)
----- Left icon and Right input field (the input field allows to set value)

---------- Progress Indicators
Creates one of the two types of animated progress indicators:
----- Circle (Flat or raised - Loading circle)
----- Linear (Simple horizontal bar)

---------- Dividers
Creates a simple (Light or Dark) 1px height line

---------- Nav Drawer
Creates a ready to use Nav Drawer that you can hide/display on the left of
the screen.

---------- Tab View
Creates a fully implement tabulation view with page header and content.
The page header can either have:
----- Icon (a single icon definying what the page is)
----- Text (a single text)
----- Icon and Text (a text and an icon)

---------- App Bar
Creates a simple AppBar that you can place on top of your screens

---------- Screens
Screens allows you to handle multiple screens and their transitions with ease.
You can create:
----- Screen View (that holds and control all the screens)
----- Screen (that is just a single screen)


--------------------------------------------------------------------------------
                                 Example scenes
--------------------------------------------------------------------------------
The best way to get started with MaterialUI is by playing with the example
scenes you can find in: MaterialUI/Examples/Scenes/
It has a scene for each component, and you'll see how you can use each one of
them in multiple ways!


--------------------------------------------------------------------------------
                              Vector Image Manager
--------------------------------------------------------------------------------
When you create a VectorImage, it uses a Vector Font. This is basically just a
normal font (ttf), but instead of having characters in it, it has icons!

We include by default, the Material Design icon font that you can use (as well as
the icon set we use for the MaterialUI components). But there may be time when you
need to use other icons. So we made a Vector Image Manager that allows you to
download other icon fonts from the web (from more than 10 popular icon fonts).

If you want to use that, simply open Window -> MaterialUI -> VectorImageManager.
In this window, you'll see (at the bottom) a list of web icon fonts to download.
Simply click 'Download' to add it to your project, or click 'Website' to learn
more about a particular icon font (and the icons inside).

There's also an option to create your own fonts, based on vector files (svg) you
might have. Simply follow the instructions under the 'Import' section.