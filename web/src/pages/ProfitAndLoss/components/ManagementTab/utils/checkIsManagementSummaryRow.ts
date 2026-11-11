import { MANAGEMENT_TOTAL_SUMMARY_ROW_ID } from '../constants';
import { ManagementGridRow } from '../types';

export const checkIsManagementSummaryRow = (
	row: unknown
): row is ManagementGridRow =>
	typeof row === 'object' &&
	row !== null &&
	'id' in row &&
	row.id === MANAGEMENT_TOTAL_SUMMARY_ROW_ID;
