import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { UsersList } from './pages/users/list/users.list';
import { UserForm } from './pages/users/form/user.form';
import { ProductsList } from './pages/products/list/products.list';
import { ProductForm } from './pages/products/form/product.form';
import { Login } from './pages/login/login';
import { AuthGuard } from './middlewares/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: Home,
    canActivate: [AuthGuard]
  },
  {
    path: 'users',
    component: UsersList,
    canActivate: [AuthGuard]
  },
  {
    path: 'products',
    component: ProductsList,
    canActivate: [AuthGuard]
  },
  {
    path: 'login',
    component: Login
  },
  {
    path: 'users/:id',
    component: UserForm,
    canActivate: [AuthGuard]
  },
  {
    path: 'products/:id',
    component: ProductForm,
    canActivate: [AuthGuard]
  },
];
