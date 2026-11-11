import {
	anotherSettings,
	COLUMNS_COUNT,
	departmentSettings,
	groupSettings,
	idSettings,
} from '../../contants';

export const getPayloadColumnWidth = (
	displayDepartmentColumn: boolean,
	displayGroupColumn: boolean
) => {
	const payloadColumnsWidth = {
		...idSettings,
		...(displayDepartmentColumn ? departmentSettings : {}),
		...(displayGroupColumn ? groupSettings : {}),
		...anotherSettings,
	};

	return payloadColumnsWidth;
};

export const getPayloadColumnCount = (
	displayDepartmentColumn: boolean,
	displayGroupColumn: boolean
) => {
	const hiddenColumns = [displayDepartmentColumn, displayGroupColumn].filter(
		(visible) => !visible
	).length;

	return COLUMNS_COUNT - hiddenColumns;
};
