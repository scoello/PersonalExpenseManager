import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Expense, SaveExpenseRequest } from '../models/expense.model';

@Component({
  selector: 'app-expense-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './expense-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExpenseFormComponent implements OnChanges {
  @Input() expense: Expense | null = null;
  @Input() submitting = false;
  @Output() readonly submitted = new EventEmitter<SaveExpenseRequest>();
  @Output() readonly cancelled = new EventEmitter<void>();

  draft: SaveExpenseRequest = this.emptyDraft();

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['expense']) {
      this.draft = this.expense
        ? { date: this.expense.date, amount: this.expense.amount, category: this.expense.category }
        : this.emptyDraft();
    }
  }

  submit(): void {
    this.submitted.emit(this.draft);
  }

  private emptyDraft(): SaveExpenseRequest {
    return { date: new Date().toISOString().slice(0, 10), amount: 0, category: '' };
  }
}
