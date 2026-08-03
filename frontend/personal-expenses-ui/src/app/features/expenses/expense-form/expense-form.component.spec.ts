import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ExpenseFormComponent } from './expense-form.component';

describe('ExpenseFormComponent', () => {
  let fixture: ComponentFixture<ExpenseFormComponent>;
  let component: ExpenseFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ExpenseFormComponent] }).compileComponents();
    fixture = TestBed.createComponent(ExpenseFormComponent);
    component = fixture.componentInstance;
  });

  it('copies the selected expense into an editable draft', () => {
    component.expense = { id: '1', date: '2026-08-03', amount: 10, category: 'Food' };
    component.ngOnChanges({
      expense: { previousValue: null, currentValue: component.expense, firstChange: true, isFirstChange: () => true }
    });

    expect(component.draft).toEqual({ date: '2026-08-03', amount: 10, category: 'Food' });
    expect(component.draft).not.toBe(component.expense);
  });

  it('emits the draft when submitted', () => {
    const emitted = jasmine.createSpy('submitted');
    component.draft = { date: '2026-08-03', amount: 10, category: 'Food' };
    component.submitted.subscribe(emitted);

    component.submit();

    expect(emitted).toHaveBeenCalledWith(component.draft);
  });
});
