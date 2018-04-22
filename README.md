![logo](https://raw.githubusercontent.com/nfisher23/SEPubViewer/master/SEPubViewer/imgs/logo.png)
# SEPubViewer
SEPubViewer is a dead-simple windows desktop application to view SEC filings on Edgar. 
You can browse based on filing type and/or date before, you can download one document or a collection of documents, 
and you can do it all without having to deal with the cumbersome browsing that the SEC provides us with by default on their website.

## Build Procedure
1. Clone

```
git clone https://github.com/nfisher23/SEPubViewer.git
```

2. Build with Visual Studio (or msbuild if you can find the exe)

```
Ctrl-Shift-B
```

3. Fire up /SEPubViewer/SEPubViewer/bin/Debug/SEPubViewer.exe

4. Do cartwheels with all the time you've saved

![App](https://raw.githubusercontent.com/nfisher23/SEPubViewer/master/SEPubViewer/imgs/SEPubViewer_GUI.JPG)

## Usage

Type your ticker or CIK number into the text box in the upper left, click "Get Filings," and the filings view will be populated. 
You can click on any of the listed filings and it will choose the biggest size file to look at. In the lower left will be all
the documents included in the report.

You can search for one type of filing by typing that filing into the "Filing Type" text box. E.g. to search for just 10-q filings,
type 10-q into the box and click get filings. You can also select the last date to consider in the date picker drop down.

Recent Tickers will be included in a list in the upper right. Click on any of them and it will search for that ticker again.

You can download all of the documents in the report, or just the document you're looking at with the relevent buttons on the top.
All downloads will direct to your system's Downloads folder, and if you select a group of downloads you will get a folder with all
the documents inside.

### The GUI is ugly, though

We agree that the GUI is ugly. If you don't like it, submit a pull request with a better one and we'll honestly probably take it, 
because it can't really get any worse than what it is.

