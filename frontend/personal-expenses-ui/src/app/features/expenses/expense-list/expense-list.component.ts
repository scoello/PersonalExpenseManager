import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { Expense } from '../models/expense.model';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './expense-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExpenseListComponent {
  @Input({ required: true }) expenses: readonly Expense[] = [];
  @Output() readonly editRequested = new EventEmitter<Expense>();
  @Output() readonly deleteRequested = new EventEmitter<Expense>();

  trackById(_: number, expense: Expense): string {
    return expense.id;
  }
}
