import { PAYLOAD_FREEZE_COLUMNS_AMOUNT } from '@/pages/Payload/contants';

export const getNumberOfFrozenColumns = (
	isDepartmentVisible: boolean,
	displayGroupColumn: boolean
) => {
	const additionalColumns = [isDepartmentVisible, displayGroupColumn].filter(
		Boolean
	).length;

	return PAYLOAD_FREEZE_COLUMNS_AMOUNT + additionalColumns;
};
