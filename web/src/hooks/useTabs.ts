import { useState } from 'react';
import { useSearchParams } from 'react-router-dom';

import { TAB_ID_PARAM } from '@/constants/searchParams';

export function useTabs(defaultTab: number, customTabIdQueryParam?: string) {
	const [searchParams] = useSearchParams();
	const tabId = searchParams.get(customTabIdQueryParam ?? TAB_ID_PARAM);
	const tabsState = useState(tabId ? Number(tabId) : defaultTab);

	return tabsState;
}
