## 09-APR-2017

* NEW: Comment / Uncomment block or line via toolbar (thx vestanpance)
* NEW: GridView right click context menu. Collapse all, expand to 1, 2, or 3 levels. (thx vestanpance)
* CHG: XSemmel is now distributed as portable zip (no setup.exe anymore)
* CHG: do not auto-switch to SchemaInfo tab, in case user likes to see fragments or validation errors...
* CHG: XsdVisualizer now shows an Xml Modified warning and will be updated on Save. (thx vestanpance)
* FIX: do not close application if error occurred after question "Save before closing?"

## 13-JUL-2016

* NEW: Windows 10 supported
* CHG: .net Framework 4.6 required 

## 20-DEC-2015

* FIX: No longer possible to minimize Compare- and XQuery-dialogs
* FIX: fixed some Windows 10 design issues
* NEW: "Listening to TCP stream" window is now always on top, Mainwindow can be minimized
* CHG: "Trim" command also under "Prettyprint". Trim prettyprints even if XML is wellformed
* CHG: Automatically show schema info when selecting node

## 24-JUL-2015

* FIX: Missing icon in 'Bulk prettyprint'

## 14-APR-2015

* FIX: several focus-related issues with ribbon-buttons
* FIX: Generate XPath for attributes
* FIX: workaround: couldn't detect wellformedness of UTF8-BOM encoded XML (can be disabled in options)
* NEW: Minor improvements in search with Ctrl+F
* NEW: Search'n'Replace can handle plain text
* NEW: Copy XPath of node in XmlGridView to clipboard
* NEW: Copy value of node in XmlGridView to clipboard

## 21-JAN-2015

* FIX: Issue #4: Encoding of some xml files gets detected wrong

## 27-NOV-2014

* FIX: Remove close tag on />
* FIX: Hang when referenced DTD file is not accessible while checking for wellformedness
* FIX: Resizing on large monitors
* FIX: Question to save file if file contains only text from clipboard
* FIX: Changing options on XPath-Tab will update the error state (tooltip and backgroundcolor) of input area
* NEW: Link to homepage in About-box
* NEW: Trim file (e.g. XML contained in lines of Log files), see submenu of Validation/Tidy
* NEW: Search'n'Replace can delete found nodes
* CHG: Prettified some icons
* CHG: 'Search for text' (Ctrl+F) is now case-insensitive

## 04-SEP-2014 

* FIX: "Current file has changed" seldom comes up when file was saved by Xsemmel itself
* FIX: "expand empty tags" was sometimes offered for non-empty tags
* FIX: Crash in Schema Visualizer if schema contains extension-base of self
* FIX: Fixed rare crash when open file was deleted by another process
* FIX: Sometimes black window title on Win8 with multiple displays
* NEW: Code-completion of attributes with Ctrl+Space
* NEW: Convert decimal to hexadecimal numbers and v.v.
* NEW: Installer is now signed to reduce issues with Win8 SmartScreen
* CHG: Docking framework removed

## 29-MAY-2014

**WINDOWS XP IS NO LONGER SUPPORTED**

* FIX: Some minor issues
* NEW: Better visualisation of validation issues
* NEW: Printing
* CHG: Disabled Jumplist
* CHG: updated to .net 4.5, WinXP NO LONGER SUPPORTED

## Release 15-MAR-2014

**Last release supporting Windows XP**

* FIX: Contextmenu in TreeView maybe permanently disabled
* FIX: Open XML files even if they are not valid according to referenced schema
* FIX: Open referenced XSD file even if file path contains spaces
* FIX: Auto-complete when cursor is on apostroph
* FIX: Some minor issues
* NEW: remove closing tag on elements when enter '/', e.g. "<text></text>" --> enter '/' --> "<test/>"
* NEW: code completion to expand empty tags, e.g. "<text/>" --> Ctrl+Space --> "<test></test>"
* CHG: Improved code-completion of attributes

## Release 01-MAR-2014

* FIX: Auto-complete when cursor is on apostroph
* FIX: use XSD file if referenced by noNamespaceLocation
* NEW: Run [External tools](External-tools)
* NEW: Added ribbon-button to start search (same as Ctrl+F) to improve usability
* NEW: Doubleclick on cursor position in statusbar opens "goto line"-dialog (same as Ctrl+G) to improve usability
* NEW: ribbon-button to insert XML entities
* NEW: Code completion (Ctrl+Space) for closing comments, Auto-complete of opening comment characters ("<!" --> "<!--")

## Release 26-JAN-2014

* NEW: full Windows 8 and Windows 8.1 (desktop mode) support
* NEW: Setup optionally associates .xml-files with Xsemmel
* FIX: AutoCompletion no longer inserts invalid closing element after empty element

## Release 27-DEC-2013

* NEW: Added "Cancel" option to dialog which appears when drag'n'dopping node in XmlTree
* NEW: Added button "New Xsemmel window" to Xslt tab to improve usability
* NEW: Store last values of 'Open from TCP stream' configuration
* FIX: Fixed crash if invalid interface was specified in 'Open from TCP stream'
* FIX: some small GUI improvements

## Release 04-DEC-2013

* NEW: If a XML file has a linked Xml Schema and the schema contains an annotation/documentation for an element, the annotation can be shown by selecting the element in TreeView and clicking 'show schema info' in context menu
* NEW: Open from TCP stream
* FIX: window icon of Xsd2Xml sample generator
* FIX: Html export, fixed window title and default filename
* FIX: several small optical improvements
* FIX: Links in AboutBox now work
* CHANGE: Renamed setup file to {"setup_xsemmel_[date](date).exe"}
* CHANGE: bulk windows, renamed 'Process' to 'Start' to improve usability

## Release 10-NOV-2013

* FIX: Suppress error message when last opened file does not exist anymore
* FIX: Fixed "Search next" after document change
* NEW: configuration item to disable code completion
* CHANGE: Reworked icon and splashscreen

## Release 29-OCT-2013

* FIX: Start search when opening search (in case something was being searched before)
* FIX: Focus editor after open
* FIX: Focus issue when starting search after opening document
* NEW: Button to find next occurrence of search-string
* CHANGE: moved button 'Bulk XPath' to 'XPath Query' to improve usability
* CHANGE: moved 'Bulk Prettyprint' to ribbon Editor/Prettyprint dropdown menu to improve usability
* CHANGE: Moved 'Bulk Validation' to ribbon tab 'Validation' to improve usability

## Release 30-JUL-2013

* FIX: Save as to a non-existing file works again
* FIX: Crash when entering invalid directories in bulk operations
* CHANGE: removed Treemap, added 'expand to level 1..5' instead
* CHANGE: switched ribbon tabs 'Xslt' and 'Tools' to improve usability
* CHANGE: enable Web Browser by default, removed 'toggle Web Browser' command

## Release 24-JUL-2013

* NEW: editor contextmenu: use selected text as fragment
* NEW: Option to always prettyprint fragments
* CHANGE: Ribbon tab 'TreeView': changed order of 'move' and 'view' to improve usability
* CHANGE: moved ribbon group 'Show' from tab 'Tools' to 'Editor' to improve usability
* CHANGE: If there is a selection, Prettyprint works on selected text only (useful for non-wellformed documents)
* CHANGE: Ability to open some locked file, e.g. log files, being already used by another process

## Release 26-APR-2013

* FIX: bulk XSD validation
* NEW: added context menu to result of bulk operations (copy, save, open in new Xsemmel)
* CHANGE: Bulk window: progress and result automatically scroll to the end of text

## Release 11-MAR-2013

* FIX: Diff: Reset highlights when diffing multiple files 
* NEW: ask to change read-only flag when trying to save read-only file
* CHANGE: TreeView removes whitespace in front and at the end of element content
* CHANGE: Redesigned Search'n'Replace panel
* CHANGE: Updated avalonEdit, PropertyTools, HtmlAgilityPack

## Release 21-DEC-2012

* FIX: Auto-completion recognized closing comment ("-->") errorously as closing XML element
* NEW: Auto-completed apostroph characters will be taken into account when manually typing apostroph (avoids double apostrophs)
* CHANGE: Show first opened filename in main window's title 

## Release 28-OCT-2012

* FIX: Consecutive saving of new documents
* NEW: Trim apostroph (") in filenames to be compatible with Windows Explorer's "Copy as path" feature
* NEW: Added "xslt" as file type in "save as"-dialog

## Release 20-SEP-2012

* FIX: Open empty files from other Xsemmel instances
* FIX: Code Completion whith elements only differing in last letter
* FIX: Inserting <!-- when Code Completion is active
* FIX: other minor fixes
* NEW: Convert Base64 strings

## Release 22-JUL-2012

* FIX: XSD ribbon tab: enable elements not until xsd visualizer is active
* FIX: Auto-Close of element names with special characters
* FIX: last character was double inserted when code completion proposal was entered manually
* FIX: Auto-close when entering a closing element
* FIX: update occurence highlighting when document changes
* FIX: other minor fixes
* NEW: status bar: show xsd filename in tooltip even if xsd is invalid
* NEW: re-added contextmenu to treeView
* NEW: Expand/Collapse all foldings in contextmenu of editor
* NEW: Copy XPath keeps syntax highlighting
* NEW: Xsd-mode: Select xml file to validate
* CHANGE: XPath: visualize that when text selected, only the selected text will be executed as XPath
* CHANGE: status bar: format text length
* CHANGE: TreeView: apply filter restyled; added remove filter button
* CHANGE: better feedback in bulk processing dialogs

## Release 15-JUL-2012

* NEW: small improvements

## Release 07-JUL-2012

* FIX: Automatically insert closing element 
* FIX: crash when entering </>
* FIX: PrettyPrint doesn't preserve whitespace anymore
* NEW: Navigate to next element with Ctrl+Alt+Right
* NEW: XSLT support improved

## Release 01-JUL-2012

* FIX: Memory leak when using user-defined XPath-functions
* NEW: Possibility to define mappings for XSD files referenced by noNamespaceSchemaLocation
* CHANGE: reload xsd location after saving XML document

## Release 16-JUN-2012

* FIX: don't show #whitespace in Fragment-GridView
* FIX: Object disposed-exception after opening XML from clipboard
* FIX: Input of { on german keyboard layouts
* NEW: "Open from clipboard" in quick access bar
* NEW: Bracket matching in XPath input field
* CHANGE: XPath-Results reworked
* CHANGE: Improved bracket matching

## Release 10-JUN-2012

* FIX: very long lines (>1000 chars) break performance
* NEW: better support for large XML files
* NEW: added links to w3schools.com XML references
* NEW: List of recently opened files

## Release 02-JUN-2012

* FIX: some minor fixes in code completion
* FIX: Problems with UTF8 encoding
* FIX: Problems writing config file
* NEW: Create new file

## Release 10-MAY-2012

* FIX: fixed crash when editing XSD file in external process while it is used for validating
* FIX: better error message if xsd file is invalid
* FIX: removed annoying reload question after file saving
* NEW: Bulk pretty print
* NEW: delete selected TreeView-node
* CHANGE: Improved Bulk validate and bulk XPath
* some minor changes
	
## Release 30-APR-2012

* FIX: TreeView: Drag'n'Drop issue when selecting the last item in tree
* FIX: Windows XP
* NEW: 'Open in new Xsemmel' in Fragment-Contextmenu
* NEW: Show length of current document in editor's statusbar
* NEW: Option to open last opened file or clipboard content at startup
* NEW: Setup delivers example XPathFunctions.cs to be used in XPath queries
* NEW: Tidy non-wellformed XML

## Release 21-APR-2012

* FIX: Do not show #whitespace in GridView
* NEW: Editor shows folded text in tooltip when hovering
* NEW: Highlight all occurrences of word under cursor with F11
* NEW: Highlight current line in editor
* NEW: Search for text with Ctrl+F
* NEW: Go to line with Ctrl+G
* NEW: TreeView: Copy code of selected node to clipboard
* NEW: Configuration: Change font size of editor
* NEW: Configuration: Specify if Splashscreen shall be shown
* NEW: Check if current file is being changed by external process

## Release 16-APR-2012

* FIX: Open arbitrary files
* FIX: Expand/Callapse all if there is no valid XML file
* NEW: Open result of Xsd2Xml in new window
* CHANGE: Floating windows are maximizable
* CHANGE: Restyled xsd2xml window

## Release 10-APR-2012
	
* FIX: Crash when opening "Buld validate" and "Xml Generator"
* FIX: Lot of encoding-related bugs
* NEW: Move up/down node in TreeView
* NEW: Drag'n'Drop items in Tree
* CHANGE: Reworked Ribbon design
* CHANGE: Moved 'Copy XPath to selected item' TreeView-ContextMenu-item to ribbon

## Release 29-MAR-2012

* FIX: Open file

## Release 23-MAR-2012

* FIX: Comment out with Ctrl+Alt+7 no longers inserts { on german keyboard layouts
* FIX: Xsemmel no longer crashes when trying to save a readonly-file
* NEW: Save with Ctrl+S
* NEW: Create new file
* NEW: Splash screen
* CHANGE: All files will be saved UTF-8 encoded
* CHANGE: Save no longer asks
* CHANGE: xml2xsd more userfriendly
* CHANGE: "Edit file" now called "Edit file externally"
* CHANGE: Changed icon

## Release 17-MAR-2012

* FIX: WebBrowserView
* NEW: GridView in FragmentView
* NEW: TreeView: Collapse all, Expand all
* NEW: Apply filter to TreeView

## Release 11-MAR-2012

* FIX: Comparing of two files
* NEW: Improved XSLT
* NEW: WebBrowser View
* NEW: XML-foldings in FragmentView

## Release 04-MAR-2012

* NEW: auto-select element under cursor in TreeView
* NEW: insert current date and time in ISO 8601 format
* NEW: Comment out selected TreeView item
* NEW: Rename selected TreeView item

## Release 26-FEB-2012

* CHANGE: Reworked TreeMap
* NEW: Seach and Replace via XPath
* NEW: (Un-)Escape XML-Strings

## Release 19-FEB-2012

* NEW: Treemap view
* NEW: possibility to insert noNamespaceLocation if xsd is selected
* NEW: GridView: expand all
* NEW: Added license text (new BSD license) to source and About dialog
* CHANGE: XmlDiff: 'Current document' compares now the unsaved data from editor
* CHANGE: 'Generate schema' operates on unsaved data from editor
* CHANGE: Switched display of line and column of cursor in editor
* CHANGE: Merged 'XmlTree Fragment View' and 'XSD Fragment View'
* CHANGE: TreeView operates on unsaved data from editor

## Release 05-FEB-2012

* FIX: opening arbitrary text files
* FIX: Crash when opening a XML file with an missing XSD specified
* FIX: GridView refreshes in 'fragment' mode
* NEW: Installation via setup.exe
* NEW: copy XSD filepath to clipboard
* NEW: Bulk XPath processing (works with big XML files as well)
* NEW: Unescape C# string
* NEW: open Code Completion with Ctrl+Space
* NEW: Export syntax highlighting as HTML
* NEW: XML Diff, new options: "Use current document", "use document in clipboard" 
* CHANGE: SaveAs dialog more user friendly
* CHANGE: don't insert closing element after </ automatically, instead show proposal box

## Release 29-JAN-2012

* FIX: code completion in combination with xs:extension types
* FIX: no crash when entering XSD file without prior opened XML
* FIX: auto-detection of xsd files
* NEW: XPath is being executed on live document
* NEW: Docking Framework
* NEW: Gridview shows actual Editor content (file do not need to be saved)
* NEW: Copy filename to clipboard
* NEW: Warning if file is too large to be opened
* CHANGE: more icons

## Release 14-JAN-2012

* XPath: Jump to the position in editor on click
* Prettyprint configurable
* Load XPath from file (see context menu)  
* load non-wellformed xml from clipboard
  
## Release 05-JAN-2012

* Editor: Bracket matching
* Editor: Comment out with Strg+7 
* Diff in Three-Pane-View
* Indent with Tab resp. Shift-Tab
* Ribbon 
* removed XSD Validation-Tab
* Editor shows if document was "modified" in statusbar
* Load XML from URL
* fixed XSD visualizer
* Validator: highlight errors