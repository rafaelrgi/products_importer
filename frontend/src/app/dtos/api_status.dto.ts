import { User } from "./user.dto";

export interface ApiStatus {
  status: string;
  isAuth: boolean;
  isDocker: boolean;
  user: User;
}