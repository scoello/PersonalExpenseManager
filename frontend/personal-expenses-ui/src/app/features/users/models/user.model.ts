export interface User {
  readonly id: string;
  readonly username: string;
  readonly role: string;
}

export interface CreateUserRequest {
  readonly username: string;
  readonly password: string;
}
