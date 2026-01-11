import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Users } from './pages/users/users';
import { Products } from './pages/products/products';
import { Login } from './pages/login/login';
import { AuthGuard } from './middlewares/auth.guard';

export const routes: Routes = [
  {
    path: "",
    component: Home,
    canActivate: [AuthGuard]
  },
  {
    path: "users",
    component: Users,
    canActivate: [AuthGuard]
  },
  {
    path: "products",
    component: Products,
    canActivate: [AuthGuard]
  },
  {
    path: "login",
    component: Login
  }
];
