import { PageWithTabs } from '@/components/PageWithTabs';
import {
	REQUESTS_TAB_INDEX_LOCAL_STORAGE_KEY,
	requestsPageTabs,
} from '@/pages/Requests/constants';

export function RequestsPage() {
	return (
		<PageWithTabs
			tabs={requestsPageTabs}
			tabIndexLocalStorageKey={REQUESTS_TAB_INDEX_LOCAL_STORAGE_KEY}
		/>
	);
}
