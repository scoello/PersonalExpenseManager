import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, ViewChild, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { CreateUserRequest, User } from '../models/user.model';
import { UserApiService } from '../services/user-api.service';
import { UserFormComponent } from '../user-form/user-form.component';
import { UserListComponent } from '../user-list/user-list.component';

@Component({ selector: 'app-users-page', standalone: true, imports: [CommonModule, UserFormComponent, UserListComponent], templateUrl: './users-page.component.html', changeDetection: ChangeDetectionStrategy.OnPush })
export class UsersPageComponent {
  private readonly userApi = inject(UserApiService);
  @ViewChild(UserFormComponent) private userForm?: UserFormComponent;
  protected readonly users = signal<User[]>([]);
  protected readonly loading = signal(false);
  protected readonly submitting = signal(false);
  protected readonly errorMessage = signal('');

  constructor() { this.loadUsers(); }

  protected createUser(request: CreateUserRequest): void {
    if (this.submitting()) return;
    this.errorMessage.set('');
    this.submitting.set(true);
    this.userApi.create(request).pipe(finalize(() => this.submitting.set(false))).subscribe({
      next: () => { this.userForm?.reset(); this.loadUsers(); },
      error: error => this.errorMessage.set(typeof error.error === 'string' ? error.error : 'Could not create user.')
    });
  }

  private loadUsers(): void {
    this.loading.set(true);
    this.userApi.list().pipe(finalize(() => this.loading.set(false))).subscribe({
      next: users => this.users.set(users),
      error: () => this.errorMessage.set('Could not load users. Please refresh the page.')
    });
  }
}
