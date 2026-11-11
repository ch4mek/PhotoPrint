import { PayslipRow } from '../types';

export const getRowKey = (row: PayslipRow) => row.fileId;
