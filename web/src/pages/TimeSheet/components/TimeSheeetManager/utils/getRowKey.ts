import { TimeSheetManagerRow } from '../types';

export const getRowKey = (row: TimeSheetManagerRow) => row.userId;
