import { GridColumnWidthsConfiguration } from '@/types/grid';
import { FinanceOrderingField } from '@/types/rates';

const ratesGridColumnsWidths = {
	expand: { width: 36, fixed: true },
	id: { width: 67, fixed: true },
	user: { width: 340 },
	department: { width: 159, fixed: true },
	project: { width: 340 },
	client: { width: 340 },
	legalEntity: { width: 340 },
	rate: { width: 146, fixed: true },
} satisfies GridColumnWidthsConfiguration;

export const getRatesGridColumnsWidths = (
	isDesktopLWidth: boolean
): GridColumnWidthsConfiguration => ({
	...ratesGridColumnsWidths,
	user: { width: isDesktopLWidth ? 340 : 226 },
	project: { width: isDesktopLWidth ? 340 : 226 },
	client: { width: isDesktopLWidth ? 340 : 226 },
	legalEntity: { width: isDesktopLWidth ? 340 : 200 },
});

export const ratesGridColumns = {
	expand: {
		key: 'expand',
		name: '',
		sortable: false,
	},
	id: {
		key: 'id',
		name: 'Id',
		sortable: true,
	},
	user: {
		key: 'user',
		name: 'User',
		sortable: true,
	},
	department: {
		key: 'department',
		name: 'Department',
		sortable: true,
	},
	project: {
		key: 'project',
		name: 'Project',
		sortable: false,
	},
	client: {
		key: 'client',
		name: 'Client',
		sortable: false,
	},
	legalEntity: {
		key: 'legalEntity',
		name: 'Legal entity',
		sortable: false,
	},
	rate: {
		key: 'rate',
		name: 'Rate/h',
		sortable: false,
	},
};

const ratesSortFieldByColumnKey: Record<string, FinanceOrderingField> = {
	[ratesGridColumns.id.key]: 'UserId',
	[ratesGridColumns.user.key]: 'UserFullName',
	[ratesGridColumns.department.key]: 'DepartmentId',
};

export const getRatesSortField = (columnKey: string): FinanceOrderingField =>
	ratesSortFieldByColumnKey[columnKey] ?? 'UserId';
