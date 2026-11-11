import { useTheme } from '@emotion/react';
import { Tabs } from '@mms/mms-ui-library/components/Tabs';
import { startTransition, useEffect } from 'react';

import { TabPanel } from '../TabPanel';

import { useStickyState } from '@/hooks/useStickyState';

import { PageWithTabsContentWrapper, PageWithTabsWrapper } from './styles';
import { PageWithTabsProps } from './types';

export function PageWithTabs({
	tabs,
	tabIndexLocalStorageKey,
	paddingsExcludedTabIds,
}: PageWithTabsProps) {
	const [activeTabId, setActiveTabId] = useStickyState(
		tabs[0].id,
		tabIndexLocalStorageKey
	);

	const theme = useTheme();
	const withPaddings = !paddingsExcludedTabIds?.includes(activeTabId);

	const handleTabChange = (tabId: number) => {
		startTransition(() => setActiveTabId(tabId));
	};

	useEffect(
		() => () => {
			sessionStorage.removeItem(tabIndexLocalStorageKey);
		},
		[]
	);

	return (
		<PageWithTabsWrapper>
			<Tabs
				data={tabs}
				activeTab={activeTabId}
				onChangeTab={handleTabChange}
				customActiveTabColor={theme.palette['background-base-1']}
			/>
			<PageWithTabsContentWrapper
				scroll="horizontal"
				maxWidth="100%"
				maxHeight="100%"
				{...{ withPaddings }}
			>
				{tabs.map(({ id, Component }) => (
					<TabPanel key={id} value={activeTabId} index={id}>
						<Component />
					</TabPanel>
				))}
			</PageWithTabsContentWrapper>
		</PageWithTabsWrapper>
	);
}
