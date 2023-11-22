export interface User {
  username: string;
  token: string;
  image?: string;
  displayName: string;
}

export interface UserFormValues {
  email: string;
  password: string;
  displayName?: string;
  username?: string;
}
