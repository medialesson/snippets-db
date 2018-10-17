export interface UserEnvelope {
    user: User;
}

export interface User {
    userId:      string;
    email:       string;
    displayName: string;
    score:       number;
    token:       string;
}
