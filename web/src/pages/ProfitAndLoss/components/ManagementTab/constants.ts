import {
	profitAndLossGridRowFields,
	profitAndLossRateAmountCellFields,
} from '../../constants';

import { GridColumnWidthsConfiguration } from '@/types/grid';
import { ManagementOrderingField } from '@/types/management';

import { ManagementGridRow } from './types';

export const EXCHANGE_RATE_BUTTON_TEXT = 'Exchange rate';

const managementGridColumnsWidths = {
	expand: { width: 36, fixed: true },
	id: { width: 67, fixed: true },
	user: { width: 246 },
	department: { width: 159 },
	project: { width: 180 },
	client: { width: 180 },
	legalEntity: { width: 190 },
	monthHoursCount: { width: 78, fixed: true },
	rate: { width: 146, fixed: true },
	income: { width: 110, fixed: true },
	incomeBYN: { width: 100, fixed: true },
	salary: { width: 90, fixed: true },
	kpi: { width: 66, fixed: true },
} satisfies GridColumnWidthsConfiguration;

export const getManagementGridColumnsWidths = (
	isDesktopWidth: boolean
): GridColumnWidthsConfiguration => ({
	...managementGridColumnsWidths,
	user: { width: isDesktopWidth ? 242 : 180 },
	department: { width: isDesktopWidth ? 159 : 86 },
	project: { width: isDesktopWidth ? 242 : 180 },
	client: { width: isDesktopWidth ? 242 : 180 },
});

export const managementGridColumns = {
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
	monthHoursCount: {
		key: 'h/month',
		name: 'h/month',
		sortable: true,
	},
	rate: {
		key: 'rate',
		name: `Rate/h`,
		sortable: false,
	},
	income: {
		key: 'income',
		name: 'Income',
		sortable: false,
	},
	incomeBYN: {
		key: 'incomeBYN',
		name: `Income`,
		sortable: true,
	},
	salary: {
		key: 'salary',
		name: `Salary`,
		sortable: false,
	},
	kpi: {
		key: 'kpi',
		name: 'KPI, %',
		sortable: true,
	},
};

export const getManagementGridColumns = (isDesktopWidth: boolean) => ({
	...managementGridColumns,
	department: {
		...managementGridColumns.department,
		name: isDesktopWidth ? 'Department' : 'Dep.',
	},
});

export const managementGridRowFields = {
	...profitAndLossGridRowFields,
	incomeAmount: 'incomeAmount',
	incomeCurrency: 'incomeCurrency',
	incomeBYN: 'incomeBYN',
	salary: 'salary',
	kpi: 'kpi',
	monthHoursCount: 'monthHoursCount',
} as const satisfies Record<string, keyof ManagementGridRow>;

export const managementRateAmountCellFields = profitAndLossRateAmountCellFields;

export const managementIncomeAmountCellFields = {
	amountField: managementGridRowFields.incomeAmount,
	currencyField: managementGridRowFields.incomeCurrency,
} as const;

const managementSortFieldByColumnKey: Record<string, ManagementOrderingField> =
	{
		[managementGridColumns.id.key]: 'UserId',
		[managementGridColumns.user.key]: 'UserFullName',
		[managementGridColumns.department.key]: 'DepartmentId',
		[managementGridColumns.monthHoursCount.key]: 'TrackedHours',
		[managementGridColumns.incomeBYN.key]: 'IncomeByn',
		[managementGridColumns.kpi.key]: 'Kpi',
	};

export const getManagementSortField = (
	columnKey: string
): ManagementOrderingField =>
	managementSortFieldByColumnKey[columnKey] ?? 'UserId';

export const MANAGEMENT_TOTAL_SUMMARY_ROW_HEIGHT = 21;
export const MANAGEMENT_TOTAL_SUMMARY_ROW_ID = 'management-total-summary-row';
export const MANAGEMENT_SUMMARY_LEFT_COLUMNS_COUNT = 4;
export const MANAGEMENT_SUMMARY_LEFT_SPAN_END_COLUMN = 6;
export const MANAGEMENT_SUMMARY_MIDDLE_COLUMNS_END = 10;
export const MANAGEMENT_SUMMARY_VALUE_COLUMNS_END = 13;

export const MANAGEMENT_SUMMARY_LEFT_CELL_CLASS =
	'management-summary-left-cell';

export const summaryRowInitialValues: ManagementGridRow = {
	id: MANAGEMENT_TOTAL_SUMMARY_ROW_ID,
	userId: 0,
	canViewPersonalInfo: false,
	itemRowHeight: MANAGEMENT_TOTAL_SUMMARY_ROW_HEIGHT,
	parentId: null,
	isParent: false,
	expanded: false,
	hasChildren: false,
	isActive: true,
	parentIndex: -1,
	user: '',
	department: '',
	projects: [],
	client: '',
	legalEntity: '',
	rate: null,
	currency: null,
	incomeAmount: null,
	incomeCurrency: null,
	incomeBYN: null,
	salary: null,
	kpi: null,
	kpiColor: null,
	monthHoursCount: 0,
	expenses: null,
	profit: null,
};
