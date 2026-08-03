import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Observable, finalize } from 'rxjs';
import { ExpenseFormComponent } from '../expense-form/expense-form.component';
import { ExpenseListComponent } from '../expense-list/expense-list.component';
import { Expense, SaveExpenseRequest } from '../models/expense.model';
import { ExpenseApiService } from '../services/expense-api.service';

@Component({
  selector: 'app-expenses-page',
  standalone: true,
  imports: [CommonModule, ExpenseFormComponent, ExpenseListComponent],
  templateUrl: './expenses-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExpensesPageComponent {
  private readonly expenseApi = inject(ExpenseApiService);

  protected readonly expenses = signal<Expense[]>([]);
  protected readonly selectedExpense = signal<Expense | null>(null);
  protected readonly loading = signal(false);
  protected readonly submitting = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly total = computed(() =>
    this.expenses().reduce((sum, expense) => sum + expense.amount, 0)
  );

  constructor() {
    this.loadExpenses();
  }

  protected save(request: SaveExpenseRequest): void {
    if (this.submitting()) return;
    this.errorMessage.set('');
    this.submitting.set(true);
    const selected = this.selectedExpense();
    const operation: Observable<unknown> = selected
      ? this.expenseApi.update(selected.id, request)
      : this.expenseApi.create(request);

    operation.pipe(finalize(() => this.submitting.set(false))).subscribe({
      next: () => {
        this.selectedExpense.set(null);
        this.loadExpenses();
      },
      error: () => this.errorMessage.set('Could not save the expense. Please try again.')
    });
  }

  protected requestDelete(expense: Expense): void {
    if (!confirm(`Delete ${expense.category} expense?`)) return;
    this.expenseApi.delete(expense.id).subscribe({
      next: () => this.loadExpenses(),
      error: () => this.errorMessage.set('Could not delete the expense. Please try again.')
    });
  }

  private loadExpenses(): void {
    this.loading.set(true);
    this.expenseApi.list().pipe(finalize(() => this.loading.set(false))).subscribe({
      next: expenses => this.expenses.set(expenses),
      error: () => this.errorMessage.set('Could not load expenses. Please refresh the page.')
    });
  }
}
