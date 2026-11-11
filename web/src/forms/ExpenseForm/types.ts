export interface ExpenseFormValues {
	expense: string;
}

export interface ExpenseFormProps {
	expenseId?: number;
	initialValues?: ExpenseFormValues;
	onClose: () => void;
	onSubmit: (values: ExpenseFormValues) => void | Promise<void>;
}
