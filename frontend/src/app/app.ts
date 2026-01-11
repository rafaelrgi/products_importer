import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Menu } from './components/menu/menu';
import { Loading } from './components/loading/loading';
import { AuthService } from './services/auth';
import { User } from './dtos/user.dto';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Menu, Loading],
  templateUrl: './app.html',
  styleUrl: './app.css',
  standalone: true,
})
export class App {
  protected readonly title = signal('Products');
  protected readonly isMenuCollapsed = signal<boolean>(false);

  constructor(private authService: AuthService) { }

  isAuth(): boolean {
    return (this.authService.isAuth());
  }

  getUser(): User | null {
    return this.authService.getUser();
  }

  toggleCollapse(): void {
    this.isMenuCollapsed.set(!this.isMenuCollapsed());
  }

}