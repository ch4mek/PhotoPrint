import { lazy } from 'react';

import { TabWithComponent } from '@/types/common';

const ListTab = lazy(async () => ({
	default: (await import('./components/ListTab')).ListTab,
}));

const CalendarTab = lazy(async () => ({
	default: (await import('./components/CalendarTab')).CalendarTab,
}));

export const requestsPageTabs: TabWithComponent[] = [
	{ id: 1, text: 'List', Component: ListTab },
	{ id: 2, text: 'Calendar', Component: CalendarTab },
];

export const REQUESTS_TAB_INDEX_LOCAL_STORAGE_KEY = 'requests-panel-tab';
