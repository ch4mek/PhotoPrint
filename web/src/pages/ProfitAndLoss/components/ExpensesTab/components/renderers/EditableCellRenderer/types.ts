export interface EditableExpenseCellProps {
	expenseCategoryId: number;
	expenseId?: number;
	value: string;
	initialValue?: number;
	month: number;
}

export interface ExpenseCellInteractiveAttrs {
	backgroundColor: string;
	isDisabled: boolean;
	isClickable: boolean;
	minHeight?: number;
}
