

Authors: Jackson McKay, u1259516
		 Jaden Turner, u1095942

10/2 - Set up team: binarybros
	 - Decided to inherite Jaden's codebase
		- Chosen because Jaden had PS5 more complete at time of discussion

10/5 - Set up PS6
	 - Set up basic GUI information
	 - Set up event listener for selection changed
	 - Fixed the normalization of variable names
	 - Fixed FormulaError
		- Previous code was not returning FormulaError ever
			- Imported some of Jackson's old code to fix issue

10/7 - Added file menu 
		- Buttons new, open, close, save 
			- File menu created with a picture instead of a title, because it looks cool
	 - Added the ability to save spreadsheets
	 - Added warning dialogs when closing or overwriting an unsaved spreadsheet
	 - Fixed spreadsheet so it will not change any values in the case of a CircularException or FormulaFormatException
	 - Added the ability to have multiple windows running at once
		- This feature took us a while, becuase we neglected to look at the example given to us,
			- Where the windows ran independently
	 - Added our extra feature, NightMode
		- A button at the top of the spreadsheet with a moon/sun icon
			- When pressed, will swap the colors from daytime to nightime or vice-versa
				- Added this feature because we both use nightmode and would want users of our spreadsheet to have it
					- And because we figured it would not be that hard to implement
						- Found colors online using a hex-picker to get ones we liked
							- Had to figure out how to repaint a window, ended up using Update()
	 - Final commit of project


