import {
	EXPANDED_ROWS_HEIGHT,
	FIRST_ROWS_HEIGHT,
} from '../../ElementsTab/constants';
import { TransactionItemResponse } from '../types';

export const getNewCellHeight = (transactions: TransactionItemResponse[]) =>
	EXPANDED_ROWS_HEIGHT * transactions.length + FIRST_ROWS_HEIGHT;
