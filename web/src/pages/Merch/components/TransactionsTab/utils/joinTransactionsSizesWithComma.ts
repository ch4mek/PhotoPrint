import { TransactionItemResponse } from '../types';

export const joinTransactionsSizesWithComma = (
	transactions: TransactionItemResponse[]
) => transactions.map(({ size }) => size).join(', ');
