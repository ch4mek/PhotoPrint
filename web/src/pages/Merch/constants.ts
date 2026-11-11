import { lazy } from 'react';

import { TabWithComponent } from '@/types/common';

const StorageTab = lazy(async () => ({
	default: (await import('./components/StorageTab')).StorageTab,
}));

const ElementsTab = lazy(async () => ({
	default: (await import('./components/ElementsTab')).ElementsTab,
}));

const KitsTab = lazy(async () => ({
	default: (await import('./components/KitsTab')).KitsTab,
}));

const TransactionsTab = lazy(async () => ({
	default: (await import('./components/TransactionsTab')).TransactionsTab,
}));

export const merchTabs: TabWithComponent[] = [
	{ id: 1, text: 'Storage', Component: StorageTab },
	{ id: 2, text: 'Items', Component: ElementsTab },
	{ id: 3, text: 'Kits', Component: KitsTab },
	{ id: 4, text: 'Transactions', Component: TransactionsTab },
];

export const MerchStatuses = {
	available: 'Available',
	archived: 'Archived',
	runningOut: 'Running out',
};

const filterValueTransform: Record<keyof typeof MerchStatuses, string> = {
	available: MerchStatuses.available,
	archived: MerchStatuses.archived,
	runningOut: 'RunningOut',
} as const;

export const MERCH_STATUS_DEFAULT_FILTER = [
	filterValueTransform.available,
	filterValueTransform.runningOut,
];

export const MERCH_TAB_INDEX_LOCAL_STORAGE_KEY = 'merch-tab';

export const MerchEditCellOptionValues = {
	edit: 'Edit',
	transaction: 'Transaction',
};

export const ONE_SIZE_ID = 'One';
