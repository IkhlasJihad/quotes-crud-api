# quotes-crud-api
CRUD Goodreads Quotes API, it's using the database created with the code here: https://github.com/IkhlasJihad/goodreadsQuotesScrap


### Available Endpoints:
- /all >> queries all available quotes.
- /random >> queries the text of a single random quote.
- /author/:authorName >> queries quotes for the given author
- /tag/:tag >> queries quotes having the given tag in their tags list
- /book/:book >> queries quotes of the given book
- add >> accepts new quote passed in body, adds it if it isn't matching anyone there, returns the inserted document. 
- addMany >> accepts list of new quotes passed in body, returning list of documnets.
- delete/:id >> deletes the quote with the given id.
- update >> updates the given quote if it's matching one exists.
- search/:keyword >> applies [text-search](https://docs.mongodb.com/manual/text-search/) for quotes having the given keyword, using string values in (tags and text) fields. If no *keyword* is given, it works as /all endpoint.

### Notes:
* Pagination is supported, there are max of 30 quotes per page.
* Default page is 1, pages allowed within the range given in the response, otherwise error message will appear.
* Both text & author fields are required in add & update.

### Sample Run:

[Link on Vimeo](https://vimeo.com/667375898)
