import { PageWithTabs } from '@/components/PageWithTabs';
import { useConfiguration } from '@/context/Configuration';

import { MERCH_TAB_INDEX_LOCAL_STORAGE_KEY, merchTabs } from './constants';

export function MerchPage() {
	const { isLoading: isMerchPageLoading } = useConfiguration();

	if (isMerchPageLoading) {
		return null;
	}

	return (
		<PageWithTabs
			tabs={merchTabs}
			tabIndexLocalStorageKey={MERCH_TAB_INDEX_LOCAL_STORAGE_KEY}
		/>
	);
}
