import { TabWithComponent } from '@/types/common';

export interface PageWithTabsProps {
	tabs: TabWithComponent[];
	tabIndexLocalStorageKey: string;
	paddingsExcludedTabIds?: number[];
}
