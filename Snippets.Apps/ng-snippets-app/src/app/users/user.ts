export class UserEnvelope {
    user: User;
}

export class User {
    userId:      string;
    email:       string;
    displayName: string;
    score:       number;
    tokens:      Tokens;
}

export class Tokens {
    token:   string;
    refresh: string;
    debug:   string;
}
