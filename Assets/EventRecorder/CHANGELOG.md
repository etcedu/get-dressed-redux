# v0.1.0 - 8/29/2022 2:18 PM
#### Packaged By: William Pyle
## Added
- Change log
## Changed
- Timestamp format now ISO 8601 compliant
- Event data saved in array to support sending more than one event in one message# v0.2 - 8/31/2022 10:53 AM



# v0.2.0 - 8/31/2022 10:53 AM
#### Packaged By: William Pyle
## Added
- Settings File
## Changed
- Constant variables in the GameEventManager.cs and EventPoster.cs have been removed. You should now use the Settings File.
## Fixed
- Event Poster objects will no longer be created (and never destroyed) while in Debug mode



# v0.3.0 - 9/7/2022 12:07 PM
#### Packaged By: William Pyle
## Added
- "Post Immediately" option exposed in EventRecorder settings
## Changed
- Syntax is now backwards compatible to C# v4.0
- Json processing isolated to single location
## Fixed
- Issue where Editor code leaked into runtime code.



# v0.4.0 - 9/7/2022 2:27 PM
#### Packaged By: William Pyle
## Added
- Backup json serializer
## Changed
- Increasing backwards compatibility



# v0.5.0 - 9/8/2022 2:01 PM
#### Packaged By: William Pyle
## Added
- Event Poster Backwards compatability



# v0.6.0 - 9/9/2022 3:24 PM
#### Packaged By: William Pyle
## Added
- Can now ingest Event Spreadsheets as TSV files to automatically generate the integration files we have been writing by hand



# v0.7.0 - 9/12/2022 3:13 PM
#### Packaged By: William Pyle
## Fixed
- TSV file gen backwards compatibility improvements



# v1.0.0 - 9/26/2022 12:10 PM
#### Packaged By: William Pyle
## Added
- User ID mode
- Backlog storage for events that were not posted to the server
- Options for sending backlog data to the server
- Improved Debug options
- While in editor or debug build events will be sent to an endpoint for collecting debug data



# v1.1.0 - 9/30/2022 11:02 AM
#### Packaged By: William Pyle
## Added
- Runtime debugging support in development builds



# v 1.2.0 - 11/16/2022 11:34 AM
#### Packaged By: William Pyle
## Added
- Improved settings for testing sending data to the AWS server
## Changed
- There is now a single endpoint for debug and production events
- A new data point "isDebug" has been added to sent events
## Fixed
- Settings UI



# v1.2.1 - 11/16/2022 11:46 AM
#### Packaged By: William Pyle
## Fixed
- Fix preprocessor issue in JSONHelper



# v1.3.0 - 2/1/2023 11:16 AM
#### Packaged By: William Pyle
## Added
Pre and post build checks to ensure games' app Ids are set correctly# v2.0.0 - 5/10/2023 11:58 AM
#### Packaged By: William Pyle
## Added
- Permalog system
- Event Recorder Log
- Runtime Event Recorder Log Viewer
- Build use case setting
- Endpoint configuration
- Event Id retriever
## Changed
- Overhaul of underlying systems



# v2.1.0 - 5/11/2023 10:24 AM
#### Packaged By: William Pyle
## Changed
- EventIds now pulled from Github Catalog Spreadsheet



# v2.2.0 - 5/22/2023 12:20 PM
#### Packaged By: William Pyle
## Added
- Experimental event sections
- Generated events now include optional parameter to include them in running event sections
## Changed
- Now recording milliseconds in game event timestamps
- Most developer-facing methods should now be found in the EventRecorder class
## Fixed
- Fixed bug



# v2.2.1 - 5/22/2023 12:56 PM
#### Packaged By: William Pyle
## Fixed
- Fixed Issue where saving relevant events to an event section would fail



# v2.3.0 - 6/9/2023 11:29 AM
#### Packaged By: William Pyle
## Added
- Event recorder version number appears in settings window	
## Changed
- TSV import window has been cleaned up a bit
## Fixed
- Timestamp millisecond delimiter has been corrected. Was ":" is now "."



# v2.4.0 - 7/10/2023 11:16 AM
#### Packaged By: William Pyle
## Added
- Additional check for invalid game ids before sending data to the backend


# v3.0.0-Login - 1/15/2024 12:13 PM
#### Packaged By: William Pyle
## Added
- Simcoach Login Integration
## Changed
- Always generates a device and session Id.
- Setting for ID mode removed



# v3.0.1-Login - 1/17/2024 1:05 PM
#### Packaged By: William Pyle
## Changed
- Removed obsolete ID display in lower right of development builds
## Fixed
- Login window hiding to far off screen



# v3.1.0-Login - 1/26/2024 4:33 PM
#### Packaged By: William Pyle
## Added
- Windows OAuth support through UniWebView
- Pin now protects profile management window
## Changed
- More explicitly named SimcoachLogin related profile classes
- Methods for showing / Hiding Login window and getting the active username and profile are exposed
## Fixed
- Removed Login prefab that didn't have final art



# v3.2.0-Login - 2/21/2024 3:29 PM
#### Packaged By: William Pyle
## Changed
- Login Panel UI changes



# v3.2.1-Login - 2/22/2024 11:16 AM
#### Packaged By: William Pyle
## Fixed
- Bug where scene changes were not handling new open / close UI elements properly



# v3.3.0-Login - 3/18/2024 3:04 PM
#### Packaged By: William Pyle
## Added
- Simcoach Login info text


## Fixed
- A warning will get thrown when an event is captured before the Event System is initialized
- Gave new profile input field more breathing room



# v3.4.0-Login - 4/11/2024 1:46 PM
#### Packaged By: William Pyle
## Added
- Parental gate before accessing sign in/out
- Toggle for enabling/disabling pin before user management window
- Delete account button
## Changed
- Updated Login UI



# v3.4.1-Login - 4/11/2024 2:45 PM
#### Packaged By: William Pyle
## Fixed
- Scaling on login panel
- Scaling on Parental Gates



# v3.4.2-Login - 4/11/2024 4:14 PM
#### Packaged By: William Pyle
## Fixed
- Multitouch allowing use to submit multiple Parental Gate answers 
- Signed out UI showing before completing Parental Gate



# v3.4.3-Login - 4/11/2024 4:59 PM
#### Packaged By: William Pyle
## Added
- "Ask your parents" button
## Changed
- Parental gate has new look



# v3.4.4-Login - 4/12/2024 11:30 AM
#### Packaged By: William Pyle
## Changed
- Removed logging of parental gate. It was spamming the logs.
## Fixed
- Fixed issue with Basic Math gate causing the game to freeze



# v3.4.5-Login - 4/12/2024 2:27 PM
#### Packaged By: William Pyle
## Changed
- Use Parental Gate feature is on by deafult
## Fixed
- Delete account now is blocked by parental gate
- Was not properly including android manifest file with intent for custom tabs service



# v3.5.0-Login - 5/28/2024 2:52 PM
#### Packaged By: William Pyle
## Added
- First time launch UI
- More info UI
## Changed
- Login panel layout



# v3.6.0-LoginEnterpriseOffline - 9/12/2024 10:58 AM
#### Packaged By: Garrett Kimball
## Added
License System for Enterprise Edition
Enterprise/Consumer edition toggle tool
Offline mode functionality
## Changed
Login Panel in UI to support Offline Mode




# v3.6.1-LoginEnterpriseOffline - 9/12/2024 1:42 PM
#### Packaged By: Garrett Kimball
## Fixed
UI Scaling at 4:3



# vv3.6.2-LoginEnterpriseOffline - 10/1/2024 12:20 PM
#### Packaged By: William Pyle
## Fixed
- Removed unused directives cause errors in projects missing the dependencies



# v3.6.3-LoginEnterpriseOffline - 10/1/2024 12:27 PM
#### Packaged By: William Pyle
## Fixed
- Removed more unused directives causing errors in projects without the non-required dependencies



# vv3.6.3-LoginEnterpriseOffline - 2/24/2025 3:31 PM
#### Packaged By: Garrett Kimball
## Added
Fully automated auth callback urls upon validating event ID. WebView settings are changed and saved and the auth url applied to the game object in runtime.



# v3.6.4-LoginEnterpriseOffline - 2/24/2025 3:35 PM
#### Packaged By: Garrett Kimball
## Added
Fully automated auth callback urls upon validating event ID.
WebView settings are changed and saved and the auth url applied to the game object in runtime.
## Fixed
settings are now properly saved when using the build mode toggle between consumer and enterprise



# v3.6.4a-LoginEnterpriseOffline - 2/26/2025 9:51 AM
#### Packaged By: Garrett Kimball
## Fixed
Profile creation UI fixed at 21:9 or wider aspect ratios



