// POST snippet
export class SnippetPostDataEnvelope {
	snippet: SnippetPostData;

	constructor(snippet: SnippetPostData) {
		this.snippet = snippet
	}
}

export class SnippetPostData {
    title:      string;
    content:    string;
    language:   number;
    categories: string[] = [''];
}


// GET snippet details
export class SnippetDetailsEnvelope {
    snippet: SnippetDetails;
}

export class SnippetDetails {
    snippetId:  string;
    title:      string;
    content:    string;
    score:      number;
    author:     Author;
    language:   number;
    categories: Category[];
    createdAt:  string;
    updatedAt:  string;
}

export class Author {
    authorId:    string;
    displayName: string;
}

export class Category {
    id:          string;
    displayName: string;
    color:       string;
}
