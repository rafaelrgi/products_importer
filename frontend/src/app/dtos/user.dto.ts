export interface User {
  id: number;
  email: string;
  name: string;
  password: string;
  passwordNew: string;
  passwordCheck: string;
  isAdmin: boolean;
  isDeleted: boolean;
  createdAt: Date;
  updatedAt: Date;
}