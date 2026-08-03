import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateUserRequest } from '../models/user.model';

@Component({ selector: 'app-user-form', standalone: true, imports: [FormsModule], templateUrl: './user-form.component.html', changeDetection: ChangeDetectionStrategy.OnPush })
export class UserFormComponent {
  @Input() submitting = false;
  @Output() readonly submitted = new EventEmitter<CreateUserRequest>();
  username = '';
  password = '';
  submit(): void { this.submitted.emit({ username: this.username, password: this.password }); }
  reset(): void { this.username = ''; this.password = ''; }
}
