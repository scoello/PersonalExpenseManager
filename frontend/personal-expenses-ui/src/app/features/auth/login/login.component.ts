import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  username = 'admin';
  password = 'Admin123!';
  errorMessage = '';
  submitting = false;

  login(): void {
    if (this.submitting) return;
    this.errorMessage = '';
    this.submitting = true;
    this.auth.login(this.username, this.password).pipe(
      finalize(() => this.submitting = false)
    ).subscribe({
      next: () => void this.router.navigate(['/expenses']),
      error: () => this.errorMessage = 'Invalid username or password.'
    });
  }
}
