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
    language:   string;
    categories: string[] = [];
}


// GET snippet details
export class SnippetDetailsEnvelope {
    snippet: SnippetDetails;
}

export class SnippetsDetailsEnvelope {
    snippets: SnippetDetails[];
    count:    number;
}

export class SnippetDetails {
    id:         string;
    title:      string;
    content:    string;
    score:      number;
    author:     Author;
    language:   string;
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

// Vote

export class VoteEnvelope {
    vote: Vote;
}

export class Vote {
    status: string;
}


export enum VoteEnum {
    upvote,
    downvote,
    removed
}
