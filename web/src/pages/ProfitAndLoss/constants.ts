import { lazy } from 'react';

import { ratesAndLossTabsIds } from '@/constants/ratesAndLoss';
import { UserClaims } from '@/types/claims';
import { TabWithComponent } from '@/types/common';

import { ProfitAndLossFilters } from './types';

const RatesTab = lazy(async () => ({
	default: (await import('./components/RatesTab')).RatesTab,
}));

const ManagementTab = lazy(async () => ({
	default: (await import('./components/ManagementTab')).ManagementTab,
}));

const ExpensesTab = lazy(async () => ({
	default: (await import('./components/ExpensesTab')).ExpensesTab,
}));

export const profitAndLossPageTabs: TabWithComponent[] = [
	{
		id: ratesAndLossTabsIds.ratesId,
		text: 'Rates',
		Component: RatesTab,
		claims: [UserClaims.ViewRates, UserClaims.ViewAllRates],
	},
	{
		id: ratesAndLossTabsIds.managementId,
		text: 'Management',
		Component: ManagementTab,
		claims: [UserClaims.ViewManagementTab],
	},
	{
		id: ratesAndLossTabsIds.expensesId,
		text: 'Expenses',
		Component: ExpensesTab,
		claims: [UserClaims.ManageExpensesTab],
	},
];

export const initialProfitAndLossFilters: ProfitAndLossFilters = {
	id: '',
	user: '',
	department: [],
	project: '',
	client: '',
	legalEntity: '',
};

export const ratesRowClasses = {
	inactiveUser: 'inactive-user-row',
	parentRow: 'rates-parent-row',
	parentRowExpanded: 'rates-parent-row-expanded',
	childRow: 'rates-child-row',
	childRowLast: 'rates-child-row-last',
	rowEven: 'rates-row-even',
	rowOdd: 'rates-row-odd',
} as const;

export const PROJECT_CHILD_COLOR_CIRCLE_SIZE = 8;

export const RATES_PARENT_ROW_HEIGHT = 26;
export const RATES_CHILD_ROW_HEIGHT = 24;
export const RATES_CHILD_ROW_FONT_SIZE = '12px';

export const RIGHT_ALIGN_CELL_CLASS = 'profit-and-loss-right-cell';
export const RATES_CHILD_GROUP_CELL_CLASS = 'rates-child-group-cell';
export const RATES_RATE_COLUMN_HEADER_CLASS = 'rates-rate-column-header';

export const PRICE_DECIMAL_PRECISION = 2;
export const PRICE_DECIMAL_DIVIDER = '.';

export const profitAndLossGridRowFields = {
	rate: 'rate',
	currency: 'currency',
} as const;

export const profitAndLossRateAmountCellFields = {
	amountField: profitAndLossGridRowFields.rate,
	currencyField: profitAndLossGridRowFields.currency,
} as const;
