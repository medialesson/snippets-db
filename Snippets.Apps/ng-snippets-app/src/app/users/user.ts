export class UserEnvelope {
    user: User;
}

export class User {
    userId:      string;
    email:       string;
    displayName: string;
    score:       number;
    token:       string;
}
