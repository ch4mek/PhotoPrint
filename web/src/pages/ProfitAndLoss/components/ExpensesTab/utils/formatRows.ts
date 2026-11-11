import { ExpenseItem } from '@/types/expenses';

export const formatRows = (items: ExpenseItem[]) =>
	items.map(({ category, ...item }) => ({
		id: category.id,
		expense: category.name,
		...item,
	}));
