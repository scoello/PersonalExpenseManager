import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { User } from '../models/user.model';

@Component({ selector: 'app-user-list', standalone: true, imports: [CommonModule], templateUrl: './user-list.component.html', changeDetection: ChangeDetectionStrategy.OnPush })
export class UserListComponent {
  @Input({ required: true }) users: readonly User[] = [];
  trackById(_: number, user: User): string { return user.id; }
}
