export interface Expense {
  readonly id: string;
  readonly date: string;
  readonly amount: number;
  readonly category: string;
}

export interface SaveExpenseRequest {
  date: string;
  amount: number;
  category: string;
}
